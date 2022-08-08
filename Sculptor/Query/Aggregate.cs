namespace Sculptor.Query
{
    public class Aggregate
    {
        /// <summary>
        /// The aggregate function to execute on the database.
        /// </summary>
        public string Function { get; private set; }

        /// <summary>
        /// The column on which the aggregate function applies.
        /// </summary>
        public string Column { get; private set; }

        /// <summary>
        /// The alias to get the result of the aggregate function.
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// Create a new aggregate property instance.
        /// </summary>
        /// <param name="function">The aggregate function to execute on the database.</param>
        /// <param name="column">The column on which the aggregate function applies.</param>
        /// <param name="alias">The alias to get the result of the aggregate function.</param>
        public Aggregate(string function, string column, string alias)
        {
            Function = function;
            Column = column;
            Alias = alias;
        }

        /// <summary>
        /// Compile the aggregate function.
        /// </summary>
        /// <returns>The compiled aggregate function.</returns>
        public override string ToString()
        {
            return string.Format("{0}({1}) as {2}", Function.ToUpper(), Column, Alias);
        }
    }
}
