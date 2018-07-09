using System;
using System.Collections.Generic;
using System.Text;

namespace CN_Core
{
    public static class StringExtension
    {
        public static string GetFirstLineOrWords(this string content,int cutNums)
        {
            if (string.IsNullOrEmpty(content)) return "Task Title";
            content = content.TrimStart(new[] { '\r', '\n', ' ', '\t' });
            content = content.TrimEnd(new[] { ' ', '\t' });
            int endIndex = content.IndexOf("\r\n", StringComparison.CurrentCulture);

            var result = endIndex >= 0 ? content.Substring(0, endIndex) : content;
            return result.Length > cutNums ? result.Substring(0, cutNums) : result;
        }
    }
}
