using System.Text.RegularExpressions;

namespace Sculptor.Utils
{
    public static class StringExtensions
    {
        public static string Pluralize(this string str)
        {
            return str + "s";
        }

        public static string ToSnakeCase(this string str)
        {
            return Regex.Replace(str, "[A-Z]", "_$0").ToLower().TrimStart('_');
        }
    }
}
