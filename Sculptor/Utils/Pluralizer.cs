namespace Sculptor.Utils
{
    public static class Pluralizer
    {
        /// <summary>
        /// Get the plural form of an English word.
        /// </summary>
        /// <param name="str">The word to pluralize.</param>
        /// <returns>The plural form of the word.</returns>
        public static string Plural(string str)
        {
            return str + "s";
        }
    }
}
