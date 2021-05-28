using System.Linq;
using System.Text.RegularExpressions;

namespace Sculptor.Utils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Pluralize the last word of an English, pascal case string.
        /// </summary>
        /// <param name="str">The string to deal with.</param>
        /// <returns>The formatted string.</returns>
        public static string Pluralize(this string str)
        {
            string[] words = Regex.Split(str, "([A-Z])");

            return string.Join("", words.SkipLast(1)) + Pluralizer.Plural(words.Last());
        }

        /// <summary>
        /// Convert a string to snake case.
        /// </summary>
        /// <param name="str">The string to deal with.</param>
        /// <returns>The formatted string.</returns>
        public static string ToSnakeCase(this string str)
        {
            return Regex.Replace(str, "[A-Z]", "_$0").ToLower().TrimStart('_');
        }

        /// <summary>
        /// Convert a value to pascal case.
        /// </summary>
        /// <param name="str">The string to deal with.</param>
        /// <returns>The formatted string.</returns>
        public static string ToPascalCase(this string str)
        {
            return Regex.Replace(str, "(_[a-z])", m => m.Value.Substring(1).UcFirst()).UcFirst();
        }

        /// <summary>
        /// Make a string's first character uppercase.
        /// </summary>
        /// <param name="str">The string to deal with.</param>
        /// <returns>The formatted string.</returns>
        public static string UcFirst(this string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
