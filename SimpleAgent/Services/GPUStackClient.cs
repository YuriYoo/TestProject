using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SimpleAgent.Services
{
    public class GPUStackClient
    {
        private readonly ILogger<GPUStackClient> logger;
        private readonly ISettingsService settings;

        private HttpClient _httpClient;

        public GPUStackClient(ILogger<GPUStackClient> logger, ISettingsService settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialization()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(settings.Current.ApiBaseUrl) };

            // GPUStack 使用标准的 Bearer Token 进行鉴权
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.Current.ApiKey);
        }

        /// <summary>
        /// 检测GPUStack服务是否在线且就绪
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckGPUStackIsStartAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("readyz");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException httpEx)
            {
                // 捕获网络连接层面的异常（比如 GPUStack 服务宕机、网络不通）
                logger.LogWarning("网络请求异常: {msg}", httpEx.Message);
            }
            catch (Exception ex)
            {
                // 捕获其他未知错误
                logger.LogWarning("未知的异常: {msg}", ex.Message);
            }

            return false;
        }

        /// <summary>
        /// 检测指定模型是否在线且就绪
        /// </summary>
        public async Task<bool> CheckModelIsOnlineAsync()
        {
            var targetModelName = settings.Current.ModelId;

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("v2/models?state=ready&page=1&perPage=100&watch=false");

                // 如果请求失败
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("请求失败: 状态码 {codeNumber} ({code})", (int)response.StatusCode, response.StatusCode);
                    return false;
                }

                // 读取响应的 JSON 字符串
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var root = JObject.Parse(jsonResponse);
                var itemsToken = root["items"];

                if (itemsToken != null && itemsToken.Type == JTokenType.Array)
                {
                    var items = (JArray)itemsToken;

                    // 遍历比对模型名称和就绪状态
                    foreach (JToken item in items)
                    {
                        var name = item["name"]?.ToString();

                        // 忽略大小写匹配模型名称
                        if (string.Equals(name, targetModelName, StringComparison.OrdinalIgnoreCase))
                        {
                            // 确保 ready_replicas 大于 0，代表模型已加载到显存并准备好推理
                            int readyReplicas = item["ready_replicas"]?.ToObject<int>() ?? 0;
                            return readyReplicas > 0;
                        }
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                // 捕获网络连接层面的异常（比如 GPUStack 服务宕机、网络不通）
                logger.LogWarning("网络请求异常: {msg}", httpEx.Message);
            }
            catch (Exception ex)
            {
                // 捕获 JSON 解析异常或其他未知错误
                logger.LogWarning("解析或处理数据时发生异常: {msg}", ex.Message);
            }

            // 如果走到了这里，说明要么请求正常但没找到模型，要么发生了异常，统统返回 false
            return false;
        }

        /// <summary>
        /// 获取全局硬件运行状态
        /// </summary>
        public async Task<GpuGlobalStatus> GetGlobalGpuStatusAsync()
        {
            var result = new GpuGlobalStatus();

            try
            {
                // 发起请求
                HttpResponseMessage response = await _httpClient.GetAsync("v2/gpu-devices");

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("请求硬件状态失败: {code}", response.StatusCode);
                    return null;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();

                // 解析 JSON
                var root = JObject.Parse(jsonResponse);
                if (root["items"] is not JArray items || items.Count == 0)
                {
                    logger.LogWarning("未找到任何 GPU 设备信息。");
                    return null;
                }

                long totalMemoryBytes = 0;
                long usedMemoryBytes = 0;
                int totalCoreLoad = 0;

                // 遍历提取数据并进行汇总计算
                foreach (JToken gpu in items)
                {
                    // 提取单卡数据
                    long memTotal = gpu["memory"]?["total"]?.ToObject<long>() ?? 0;
                    long memUsed = gpu["memory"]?["used"]?.ToObject<long>() ?? 0;
                    double memUtil = gpu["memory"]?["utilization_rate"]?.ToObject<double>() ?? 0;
                    int coreUtil = gpu["core"]?["utilization_rate"]?.ToObject<int>() ?? 0;

                    // 累加到全局
                    totalMemoryBytes += memTotal;
                    usedMemoryBytes += memUsed;
                    totalCoreLoad += coreUtil;

                    // 保存单卡详情
                    result.Devices.Add(new GpuDeviceStatus
                    {
                        Name = gpu["name"]?.ToString(),
                        Id = gpu["id"]?.ToString(),
                        TotalMemoryGB = BytesToGB(memTotal),
                        UsedMemoryGB = BytesToGB(memUsed),
                        MemoryUtilizationPercent = memUtil,
                        CoreUtilizationPercent = coreUtil
                    });
                }

                // 最终赋值给汇总对象
                result.TotalMemoryGB = BytesToGB(totalMemoryBytes);
                result.UsedMemoryGB = BytesToGB(usedMemoryBytes);

                // 平均核心负载 = 所有卡的核心负载总和 / 显卡数量
                result.AverageCoreUtilizationPercent = (double)totalCoreLoad / items.Count;
            }
            catch (Exception ex)
            {
                logger.LogWarning("[错误] 获取或解析硬件数据时发生异常: {msg}", ex.Message);
                return null;
            }

            return result;
        }

        /// <summary>
        /// 辅助方法：将 Bytes 转换为 GB (保留两位小数以便阅读)
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static double BytesToGB(long bytes)
        {
            return Math.Round(bytes / (1024.0 * 1024.0 * 1024.0), 2);
        }
    }

    /// <summary>
    /// 所有 GPU 的全局汇总状态
    /// </summary>
    public class GpuGlobalStatus
    {
        /// <summary>
        /// 所有显卡的总显存 (GB)
        /// </summary>
        public double TotalMemoryGB { get; set; }

        /// <summary>
        /// 所有显卡的已用显存 (GB)
        /// </summary>
        public double UsedMemoryGB { get; set; }

        /// <summary>
        /// 整体显存占用率 (%)
        /// </summary>
        public double GlobalMemoryUtilizationPercent => TotalMemoryGB > 0 ? (UsedMemoryGB / TotalMemoryGB) * 100 : 0;

        /// <summary>
        /// 所有显卡的平均核心算力负载 (%)
        /// </summary>
        public double AverageCoreUtilizationPercent { get; set; }

        /// <summary>
        /// 存放每张卡的单独详情（备用）
        /// </summary>
        public List<GpuDeviceStatus> Devices { get; set; } = new List<GpuDeviceStatus>();
    }

    /// <summary>
    /// 单张显卡的状态
    /// </summary>
    public class GpuDeviceStatus
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public double TotalMemoryGB { get; set; }
        public double UsedMemoryGB { get; set; }
        public double MemoryUtilizationPercent { get; set; }
        public int CoreUtilizationPercent { get; set; }
    }
}
