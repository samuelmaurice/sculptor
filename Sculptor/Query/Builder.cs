using System;
using System.Collections.Generic;
using System.Linq;
using Sculptor.Database;
using Sculptor.Exceptions;
using Sculptor.Query.Grammars;

namespace Sculptor.Query
{
    public class Builder<T> where T : Model<T>, new()
    {
        public string[] Columns { get; } = new string[] { "*" };
        public string From { get; } = Model<T>.Table;
        public List<WhereClause> Wheres { get; } = new List<WhereClause>();
        public int Limit { get; private set; }

        public Builder<T> Where(string column, dynamic value)
        {
            Wheres.Add(new WhereClause(column, "=", value));

            return this;
        }

        public Builder<T> Where(string column, string ope, dynamic value)
        {
            Wheres.Add(new WhereClause(column, ope, value));

            return this;
        }

        public T First()
        {
            Limit = 1;

            List<ResultRow<T>> resultSet = Connection.Fetch<T>(Grammar.CompileSelect(this));

            if (resultSet.Count == 0)
                throw new ModelNotFoundException();

            return resultSet.First().Model;
        }

        public List<T> Get()
        {
            List<ResultRow<T>> resultSet = Connection.Fetch<T>(Grammar.CompileSelect(this));

            return resultSet.Select(r => r.Model).ToList();
        }
    }
}
