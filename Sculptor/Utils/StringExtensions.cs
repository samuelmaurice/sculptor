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

        public static string ToPascalCase(this string str)
        {
            return Regex.Replace(str, "(_[a-z])", m => m.ToString().Substring(1).ToUpper());
        }

        public static string UcFirst(this string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
