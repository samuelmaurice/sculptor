using System;
using System.Collections.Generic;
using System.Linq;
using Sculptor.Database;
using Sculptor.Exceptions;

namespace Sculptor.Query
{
    public class Builder<T> where T : Model<T>, new()
    {
        private List<WhereClause> _wheres = new List<WhereClause>();

        public Builder<T> Where(string column, dynamic value)
        {
            _wheres.Add(new WhereClause(column, "=", value));

            return this;
        }

        public Builder<T> Where(string column, string ope, dynamic value)
        {
            _wheres.Add(new WhereClause(column, ope, value));

            return this;
        }

        public T First()
        {
            Console.WriteLine(string.Join(" AND ", _wheres));
            List<ResultRow<T>> resultSet = Connection.Fetch<T>(string.Format("SELECT * FROM {0} WHERE {1} LIMIT 1", Model<T>.Table, string.Join(" AND ", _wheres)));

            if (resultSet.Count == 0)
                throw new ModelNotFoundException();

            return resultSet.First().Model;
        }

        public List<T> Get()
        {
            List<ResultRow<T>> resultSet = Connection.Fetch<T>(String.Format("SELECT * FROM {0} WHERE {1}", Model<T>.Table, string.Join(" AND ", _wheres)));

            return resultSet.Select(r => r.Model).ToList();
        }
    }
}
