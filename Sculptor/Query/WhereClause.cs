namespace Sculptor.Query
{
    public class WhereClause
    {
        private readonly string _column;
        private readonly string _operator;
        private readonly dynamic _value;

        public WhereClause(string column, string ope, dynamic value)
        {
            _column = column;
            _operator = ope;
            _value = value;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} '{2}'", _column, _operator, _value);
        }
    }
}
