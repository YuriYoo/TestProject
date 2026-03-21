using OpenTelemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting
{
	/// <summary>
	/// OpenTelemetry 追踪数据拦截器
	/// </summary>
	public class UnescapeTelemetryProcessor : BaseProcessor<Activity>
	{
		public override void OnEnd(Activity activity)
		{
			// 获取 Semantic Kernel 产生的所有标签（比如 genai.prompt, genai.completion）
			var tags = activity.TagObjects.ToList();

			foreach (var tag in tags)
			{
				// 如果标签的值是字符串，并且包含了 Unicode 转义符 "\u"
				if (tag.Value is string strValue && strValue.Contains("\\u"))
				{
					try
					{
						// 神奇的魔法：使用正则将 \uXXXX 完美还原回中文
						var unescapedStr = Regex.Unescape(strValue);

						// 用中文覆盖掉原本的乱码标签
						activity.SetTag(tag.Key, unescapedStr);
					}
					catch
					{
						// 如果解析失败，就保持原样，不影响程序运行
					}
				}
			}
		}
	}
}
