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
	/// 文件系统写入插件
	/// 提供AI对本地文件系统进行写入、删除、创建目录等操作的能力
	/// </summary>
	public class FileSystemWriterPlugin
	{
		// 工作目录（所有文件操作的基础路径）
		private readonly string _workingDirectory;

		/// <summary>
		/// 初始化文件系统插件
		/// </summary>
		/// <param name="workingDirectory">允许操作的工作目录路径</param>
		public FileSystemWriterPlugin(string workingDirectory)
		{
			_workingDirectory = workingDirectory;
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
		public async Task<string> AppendFile(
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
	}
}
