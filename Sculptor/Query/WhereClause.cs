namespace Sculptor.Query
{
    public class WhereClause
    {
        /// <summary>
        /// The boolean that applies for the where clause.
        /// </summary>
        public string Boolean { get; private set; }

        /// <summary>
        /// The column on which the where clause applies.
        /// </summary>
        public string Column { get; private set; }

        /// <summary>
        /// The operator for the where clause.
        /// </summary>
        public string Operator { get; private set; }

        /// <summary>
        /// The required value for the column.
        /// </summary>
        public dynamic Value { get; private set; }

        /// <summary>
        /// Create a new where clause instance.
        /// </summary>
        /// <param name="column">The column on which the where clause applies.</param>
        /// <param name="ope">The operator for the where clause.</param>
        /// <param name="value">The required value for the column.</param>
        /// <param name="boolean">The boolean that applies for the where clause.</param>
        public WhereClause(string column, string ope, dynamic value, string boolean = "AND")
        {
            Boolean = boolean;
            Column = column;
            Operator = ope;
            Value = value;
        }

        /// <summary>
        /// Compile the where clause.
        /// </summary>
        /// <returns>The compiled where clause with a place-holder for the value.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2} @{1}", Boolean, Column, Operator);
        }
    }
}
