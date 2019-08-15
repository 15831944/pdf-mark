using System.Collections.Generic;
using System.Text;

namespace PdfMark.WpfApp
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string text) => string.IsNullOrEmpty(text);
        public static string EmptyAsNull(this string text) => string.IsNullOrEmpty(text) ? null : text;

        public static string ConcatString(this IEnumerable<string> text, string delimeter = " ")
        {
            var sb = new StringBuilder();
            var isFirst = true;
            foreach (var item in text)
            {
                if (!isFirst) { sb.Append(delimeter); }
                isFirst = false;
                sb.Append(item);
            }

            return sb.ToString();
        }
    }
}
