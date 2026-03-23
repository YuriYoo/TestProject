using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAgent.Utility
{
    static class StringUtility
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
    }
}
