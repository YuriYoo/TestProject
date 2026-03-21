using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Plugins
{
	/// <summary>
	/// 文件读取插件
	/// 提供AI对本地文件系统进行读取、目录获取的能力
	/// </summary>
	public class FileSystemReaderPlugin
	{
		// 工作目录（所有文件操作的基础路径）
		private readonly string _workingDirectory;

		/// <summary>
		/// 初始化文件系统插件
		/// </summary>
		/// <param name="workingDirectory">允许操作的工作目录路径</param>
		public FileSystemReaderPlugin(string workingDirectory)
		{
			_workingDirectory = workingDirectory;
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
				string fullPath = string.IsNullOrEmpty(directoryPath) ? _workingDirectory : ResolvePath(directoryPath);
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
					if (_workingDirectory.StartsWith(fullPath, StringComparison.OrdinalIgnoreCase))
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
		/// 获取当前工作目录路径
		/// </summary>
		[KernelFunction("get_working_directory")]
		[Description("获取当前AI操作的工作目录路径。")]
		public string GetWorkingDirectory() => $"当前工作目录: {_workingDirectory}";

		/// <summary>
		/// 解析路径：如果是相对路径则基于工作目录，绝对路径则直接使用
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private string ResolvePath(string path)
		{
			// 获取合并后的绝对路径（自动处理 ../ 等相对符号）
			string fullPath = Path.IsPathRooted(path) ? Path.GetFullPath(path) : Path.GetFullPath(Path.Combine(_workingDirectory, path));
			string checkPath = fullPath;
			if (!checkPath.EndsWith(Path.DirectorySeparatorChar.ToString())) checkPath += Path.DirectorySeparatorChar;

			// 验证最终路径是否依然在工作目录内部
			if (!checkPath.StartsWith(_workingDirectory, StringComparison.OrdinalIgnoreCase))
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
