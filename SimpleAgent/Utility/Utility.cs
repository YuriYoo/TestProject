using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimpleAgent.Utility
{
    public static class Utility
    {
        /// <summary>
        /// 保留字符串的指定数量的字符
        /// 将保留 x / 2 个字符在开头，x / 2 个字符在结尾，中间用提示信息替代
        /// </summary>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string TruncateByChars(string text, int x)
        {
            // 如果字符串为空，或者总长度还没有x长，则直接返回原字符串
            if (string.IsNullOrEmpty(text) || text.Length <= x)
            {
                return text;
            }

            int half = x / 2;
            string head = text.Substring(0, half);
            string tail = text.Substring(text.Length - half);

            return $"{head}\n...[内容过长，已被系统隐藏 {text.Length - x} 个字符]...\n{tail}";
        }

        /// <summary>
        /// 聊天上下文保存(调试用)
        /// </summary>
        /// <param name="agentType"></param>
        /// <param name="chatHistory"></param>
        public static void ChatHistorySave(AgentType agentType, ChatHistory chatHistory)
        {
            var path = $"Logs\\{DateTime.Now:yyyyMMdd_HHmmss}_{agentType}.txt";
            StringBuilder sb = new();
            foreach (var message in chatHistory)
            {
                sb.AppendLine($"////// {message.Role} //////");
                if (message.Items.Count > 0)
                {
                    sb.AppendLine($"[Items]");
                    foreach (var item in message.Items)
                    {
                        sb.AppendLine($"- InnerContent:{item.InnerContent}");
                        if (item.Metadata != null)
                        {
                            sb.AppendLine($"- RefusalUpdate: {item.Metadata["RefusalUpdate"]}");
                            sb.AppendLine($"- FinishReason: {item.Metadata["FinishReason"]}");

                            var call = item.Metadata["ChatResponseMessage.FunctionToolCalls"];
                            if (call != null && call is List<OpenAI.Chat.ChatToolCall> toolCalls)
                            {
                                foreach (var toolCall in toolCalls)
                                {
                                    sb.AppendLine($"- {toolCall.FunctionName}");
                                    sb.AppendLine($"  - Kind:{toolCall.Kind}");
                                    sb.AppendLine($"  - Arguments:{toolCall.FunctionArguments}");
                                }
                            }
                        }
                    }
                }

                sb.AppendLine($"[Content]");
                sb.AppendLine($"{message.Content}");
            }
            File.WriteAllText(path, sb.ToString());
        }

        /// <summary>
        /// 工具消息格式化
        /// </summary>
        /// <param name="name"></param>
        /// <param name="line"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static string ToolMessageFormatter(string? name, int line, string? arguments)
        {
            string outstr;
            JObject args = [];
            try
            {
                if (line >= 0 && !string.IsNullOrWhiteSpace(arguments)) args = JObject.Parse(arguments);
            }
            catch (Exception)
            {
                Log.Logger.Warning("工具参数解析失败: {arg}", arguments);
            }

            switch (name)
            {
                case "file_system-read_file":
                    outstr = $"读取文件";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("filePath", out var filePath) ? filePath.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  路径: {arg1}";
                    }
                    break;

                case "file_system-list_directory":
                    outstr = $"获取文件列表";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("directoryPath", out var directoryPath) ? directoryPath.ToString() : "当前工作目录";
                        outstr = $"[ {outstr} 工具调用完成]  路径: {arg1}";
                    }
                    break;

                case "file_system-path_exists":
                    outstr = $"路径判断";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("path", out var path) ? path.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  路径: {arg1}";
                    }
                    break;

                case "file_system-get_working_directory":
                    outstr = $"获取工作目录";
                    if (line >= 0)
                    {
                        outstr = $"[ {outstr} 工具调用完成]";
                    }
                    break;

                case "file_system-create_directory":
                    outstr = $"创建目录";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("directoryPath", out var directoryPath) ? directoryPath.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  路径: {arg1}";
                    }
                    break;

                case "file_system-delete_file":
                    outstr = $"删除文件";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("filePath", out var filePath) ? filePath.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  路径: {arg1}";
                    }
                    break;

                case "file_system-append_file":
                    outstr = $"向文件追加内容";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("content", out var content) ? content.ToString().Length + " 字节" : "参数错误";
                        var arg2 = args.TryGetValue("filePath", out var filePath) ? filePath : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  追加内容长度: {arg1}  路径: {arg2}";
                    }
                    break;

                case "file_system-write_file":
                    outstr = $"全量写入文件";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("content", out var content) ? content.ToString().Length + " 字节" : "参数错误";
                        var arg2 = args.TryGetValue("filePath", out var filePath) ? filePath : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  内容长度: {arg1}  路径: {arg2}";
                    }
                    break;

                case "file_system-edit_file":
                    outstr = $"编辑文件";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("filePath", out var filePath) ? filePath : "参数错误";
                        var arg2 = args.TryGetValue("searchBlock", out var searchBlock) ? searchBlock.ToString().Length + " 字节" : "参数错误";
                        var arg3 = args.TryGetValue("replaceBlock", out var replaceBlock) ? replaceBlock.ToString().Length + " 字节" : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  原始内容长度: {arg2}  修改内容长度: {arg3}  路径: {arg1}";
                    }
                    break;

                case "terminal-execute_command":
                    outstr = $"执行命令";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("command", out var command) ? command.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  命令: {arg1}";
                    }
                    break;

                case "terminal-start_background_service":
                    outstr = $"启动后台服务";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("serviceId", out var serviceId) ? serviceId.ToString() : "参数错误";
                        var arg2 = args.TryGetValue("command", out var command) ? command.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  服务ID: {arg1}  命令: {arg2}";
                    }
                    break;

                case "terminal-stop_background_service":
                    outstr = $"停止后台服务";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("serviceId", out var serviceId) ? serviceId.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  服务ID: {arg1}";
                    }
                    break;

                case "terminal-get_service_logs":
                    outstr = $"读取服务日志";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("serviceId", out var serviceId) ? serviceId.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  服务ID: {arg1}";
                    }
                    break;

                case "http_test-send_http_request":
                    outstr = $"发送HTTP请求";
                    if (line >= 0)
                    {
                        var arg1 = "参数错误";
                        var arg2 = "参数错误";
                        if (args.TryGetValue("url", out var url))
                        {
                            arg2 = url.ToString();
                            arg1 = args.TryGetValue("method", out var method) ? method.ToString() : "GET";
                        }
                        outstr = $"[ {outstr} 工具调用完成]  方法: {arg1}  URL: {arg2}";
                    }
                    break;

                case "sub_agent-delegate_sub_task":
                    outstr = string.Empty;
                    break;

                case "sub_workflow-finish_subtask":
                    outstr = string.Empty;
                    break;

                default:
                    if (name != null && name.StartsWith("workflow-"))
                    {
                        outstr = line < 0 ? "切换智能体" : "[切换智能体完成]";
                    }
                    else
                    {
                        outstr = line < 0 ? $"未知({name})" : $"[未知工具 {name} 调用完成]";
                    }
                    break;
            }

            return outstr;
        }
    }
}
