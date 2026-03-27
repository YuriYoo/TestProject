using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SimpleAgent.Models;
using SimpleAgent.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleAgent.Plugins
{
    /// <summary>
    /// 文件系统读写插件
    /// 提供AI对本地文件系统进行读取、写入、删除、创建目录等操作的能力
    /// </summary>
    public class FileSystemPlugin
    {
        private readonly AgentContext context;

        public FileSystemPlugin(AgentContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 读取文件内容（异步）
        /// </summary>
        /// <param name="filePath">文件路径（相对于工作目录或绝对路径）</param>
        /// <returns>文件文本内容</returns>
        [KernelFunction("read_file")]
        [Description("读取指定文件的文本内容。filePath可以是相对于工作目录的路径或绝对路径。")]
        public async Task<string> ReadFileAsync(
            [Description("要读取的文件路径")] string filePath)
        {
            try
            {
                string fullPath = ResolvePath(filePath);
                if (!File.Exists(fullPath)) return $"[错误] 文件不存在: {fullPath}";
                return await File.ReadAllTextAsync(fullPath);
            }
            catch (Exception ex)
            {
                return $"[错误] 读取文件失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 列出目录中的文件和子目录
        /// </summary>
        [KernelFunction("list_directory")]
        [Description("列出指定目录中的文件和子目录。留空则列出工作目录根目录。")]
        public string ListDirectory(
            [Description("要列出的目录路径，留空则使用工作目录")] string directoryPath = "")
        {
            try
            {
                string fullPath = string.IsNullOrEmpty(directoryPath) ? context.WorkingDirectory : ResolvePath(directoryPath);
                if (!Directory.Exists(fullPath)) return $"[错误] 目录不存在: {fullPath}";

                var sb = new StringBuilder();
                sb.AppendLine($"[目录列表 - {fullPath}]");
                var len = sb.Length;

                // 列出子目录
                foreach (var dir in Directory.GetDirectories(fullPath))
                {
                    sb.AppendLine($"{Path.GetFileName(dir)}/");
                }

                // 列出文件
                foreach (var file in Directory.GetFiles(fullPath))
                {
                    var info = new FileInfo(file);
                    sb.AppendLine($"{info.Name} ({FormatSize(info.Length)})");
                }

                // 空目录
                if (len == sb.Length)
                {
                    if (context.WorkingDirectory.StartsWith(fullPath, StringComparison.OrdinalIgnoreCase))
                    {
                        sb.AppendLine("当前目录为工作目录，且没有包含任何文件");
                    }
                    else
                    {
                        sb.AppendLine("当前目录为空目录");
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"[错误] 列出目录失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 检查文件或目录是否存在
        /// </summary>
        [KernelFunction("path_exists")]
        [Description("检查指定路径的文件或目录是否存在。")]
        public string PathExists(
            [Description("要检查的路径")] string path)
        {
            string fullPath = ResolvePath(path);
            bool isFile = File.Exists(fullPath);
            bool isDir = Directory.Exists(fullPath);
            if (isFile) return $"[存在] 这是一个文件: {fullPath}";
            if (isDir) return $"[存在] 这是一个目录: {fullPath}";
            return $"[不存在] 路径不存在: {fullPath}";
        }

        /// <summary>
        /// 写入文件内容（覆盖写入，不存在则创建）
        /// </summary>
        /// <param name="filePath">目标文件路径</param>
        /// <param name="content">要写入的内容</param>
        /// <returns>操作结果描述</returns>
        [KernelFunction("write_file")]
        [Description("将内容写入文件，如果文件不存在则创建，已存在则覆盖。会自动创建必要的父目录。")]
        public async Task<string> WriteFileAsync(
            [Description("目标文件路径")] string filePath,
            [Description("要写入的文件内容")] string content)
        {
            try
            {
                string fullPath = ResolvePath(filePath);
                if (string.IsNullOrEmpty(fullPath)) return "[错误] 传入路径为空";
                string dir = Path.GetDirectoryName(fullPath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                await File.WriteAllTextAsync(fullPath, content);
                return $"[成功] 已写入文件: {fullPath}（{content.Length} 字符）";
            }
            catch (Exception ex)
            {
                return $"[错误] 写入文件失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 追加内容到文件末尾
        /// </summary>
        [KernelFunction("append_file")]
        [Description("向文件末尾追加内容，不存在则创建。")]
        public async Task<string> AppendFileAsync(
            [Description("目标文件路径")] string filePath,
            [Description("要追加的内容")] string content)
        {
            try
            {
                string fullPath = ResolvePath(filePath);
                if (string.IsNullOrEmpty(fullPath)) return "[错误] 传入路径为空";
                string dir = Path.GetDirectoryName(fullPath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                await File.AppendAllTextAsync(fullPath, content);
                return $"[成功] 已追加内容到: {fullPath}（{content.Length} 字符）";
            }
            catch (Exception ex)
            {
                return $"[错误] 追加文件失败: {ex.Message}";
            }
        }

        [KernelFunction("edit_file")]
        [Description("使用差异替换机制（Search/Replace）修改现有文件中的代码。")]
        public async Task<string> EditFileAsync(
        [Description("目标文件路径")] string filePath,
        [Description("需要被替换的原文代码块（SEARCH）。必须提供足够的上下文以保证该代码块在文件中是唯一的。")] string searchBlock,
        [Description("用于替换的新代码块（REPLACE）。")] string replaceBlock)
        {
            try
            {
                string fullPath = ResolvePath(filePath);
                if (!File.Exists(fullPath)) return "[错误] 传入的文件路径不存在";

                string fileContent = await File.ReadAllTextAsync(fullPath);

                // 尝试精确匹配
                //int matchCount = Regex.Matches(Regex.Escape(fileContent), Regex.Escape(searchBlock)).Count;
                int matchCount = Regex.Matches(fileContent, Regex.Escape(searchBlock)).Count;

                // 如果精确匹配失败，尝试统一换行符后再次匹配 (解决大模型输出 \n 但文件是 \r\n 的常见问题)
                if (matchCount == 0)
                {
                    string normalizedFile = fileContent.Replace("\r\n", "\n");
                    string normalizedSearch = searchBlock.Replace("\r\n", "\n");

                    matchCount = Regex.Matches(Regex.Escape(normalizedFile), Regex.Escape(normalizedSearch)).Count;

                    if (matchCount > 0)
                    {
                        fileContent = normalizedFile;
                        searchBlock = normalizedSearch;
                    }
                }

                // 根据匹配结果执行不同逻辑
                if (matchCount == 0)
                {
                    // 返回明确的错误信息给大模型，引导其重试
                    return $"[错误] 匹配失败。未在文件中找到提供的 SEARCH 块。请确保：\n" +
                           $"1. SEARCH 块的代码与文件中的原文完全一致，包括空格和缩进。\n" +
                           $"2. 不要省略任何中间的代码行，必须是连续的完整文本段落。";
                }
                else if (matchCount > 1)
                {
                    // 如果找到多处，说明上下文不够，模型需要提供更多行来定位
                    return $"[错误] 找到 {matchCount} 处匹配的代码块。替换操作被中止，因为无法确定要替换哪一处。\n" +
                           $"请在 SEARCH 块中包含更多上下文代码（例如目标代码的上一行和下一行），以确保匹配的唯一性。";
                }
                else
                {
                    // 找到唯一的匹配项，执行替换
                    string updatedContent = fileContent.Replace(searchBlock, replaceBlock);
                    await File.WriteAllTextAsync(fullPath, updatedContent);

                    return $"[成功] 成功修改了文件: {fullPath}";
                }
            }
            catch (Exception ex)
            {
                return $"[错误] 编辑文件失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        [KernelFunction("create_directory")]
        [Description("创建目录（包括所有必要的父目录）。")]
        public string CreateDirectory(
            [Description("要创建的目录路径")] string directoryPath)
        {
            try
            {
                string fullPath = ResolvePath(directoryPath);
                Directory.CreateDirectory(fullPath);
                return $"[成功] 已创建目录: {fullPath}";
            }
            catch (Exception ex)
            {
                return $"[错误] 创建目录失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        [KernelFunction("delete_file")]
        [Description("删除指定文件。")]
        public string DeleteFile(
            [Description("要删除的文件路径")] string filePath)
        {
            try
            {
                string fullPath = ResolvePath(filePath);
                if (!File.Exists(fullPath)) return $"[警告] 文件不存在: {fullPath}";
                File.Delete(fullPath);
                return $"[成功] 已删除文件: {fullPath}";
            }
            catch (Exception ex)
            {
                return $"[错误] 删除文件失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取当前工作目录路径
        /// </summary>
        [KernelFunction("get_working_directory")]
        [Description("获取当前AI操作的工作目录路径。")]
        public string GetWorkingDirectory() => $"当前工作目录: {context.WorkingDirectory}";

        /// <summary>
        /// 解析路径：如果是相对路径则基于工作目录，绝对路径则直接使用
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string ResolvePath(string path)
        {
            // 获取合并后的绝对路径（自动处理 ../ 等相对符号）
            string fullPath = Path.IsPathRooted(path) ? Path.GetFullPath(path) : Path.GetFullPath(Path.Combine(context.WorkingDirectory, path));
            string checkPath = fullPath;
            if (!checkPath.EndsWith(Path.DirectorySeparatorChar.ToString())) checkPath += Path.DirectorySeparatorChar;
            
            // 确保对比的基准路径也带有斜杠
            string safeWorkDir = context.WorkingDirectory;
            if (!safeWorkDir.EndsWith(Path.DirectorySeparatorChar.ToString())) safeWorkDir += Path.DirectorySeparatorChar;

            // 验证最终路径是否依然在工作目录内部
            if (!checkPath.StartsWith(safeWorkDir, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException($"安全拦截：禁止越权访问工作目录之外的路径 ({fullPath})");
            }

            return fullPath;
        }

        /// <summary>
        /// 格式化文件大小显示
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes}B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1}KB";
            return $"{bytes / (1024.0 * 1024):F1}MB";
        }
    }
}
