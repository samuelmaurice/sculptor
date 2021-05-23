using System;
using System.Collections.Generic;
using System.Linq;

namespace Sculptor.Query.Grammars
{
    public static class Grammar
    {
        /// <summary>
        /// The components that make up a select clause.
        /// </summary>
        private static readonly Dictionary<string, Func<dynamic, string>> _components = new Dictionary<string, Func<dynamic, string>>()
        {
            { "Columns", CompileColumns },
            { "From", CompileFrom },
            { "Wheres", CompileWheres },
            { "Limit", CompileLimit }
        };

        /// <summary>
        /// Compile a select query into SQL.
        /// </summary>
        /// <typeparam name="T">The model on which the query is performed.</typeparam>
        /// <param name="query">The query builder instance.</param>
        /// <returns>The compiled select query.</returns>
        public static string CompileSelect<T>(Builder<T> query) where T : Model<T>, new()
        {
            List<string> sql = new List<string>();

            foreach (var component in _components)
                sql.Add(component.Value.Invoke(query.GetType().GetProperty(component.Key).GetValue(query)));

            return string.Join(" ", sql);
        }

        /// <summary>
        /// Compile the "select" portion of the query.
        /// </summary>
        /// <param name="columns">The columns that should be returned.</param>
        /// <returns>The compiled "select" portion.</returns>
        private static string CompileColumns(dynamic columns)
        {
            if (columns is null)
                columns = new string[] { "*" };

            return string.Format("SELECT {0}", string.Join(", ", columns));
        }

        /// <summary>
        /// Compile the "from" portion of the query.
        /// </summary>
        /// <param name="table">The table which the query is targeting.</param>
        /// <returns>The compiled "from" portion.</returns>
        private static string CompileFrom(dynamic table)
        {
            return string.Format("FROM {0}", table);
        }

        /// <summary>
        /// Compile the "where" portion of the query.
        /// </summary>
        /// <param name="wheres">The where constraints for the query.</param>
        /// <returns>The compiled "where" portion.</returns>
        private static string CompileWheres(dynamic wheres)
        {
            List<WhereClause> whereClauses = (List<WhereClause>) wheres;

            if (whereClauses.Any())
                return string.Format("WHERE {0}", string.Join(" AND ", whereClauses));

            return "";
        }

        /// <summary>
        /// Compile the "limit" portion of the query.
        /// </summary>
        /// <param name="limit">The maximum number of records to return.</param>
        /// <returns>The compiled "limit" portion.</returns>
        private static string CompileLimit(dynamic limit)
        {
            if (limit > 0)
                return string.Format("LIMIT {0}", limit);

            return "";
        }

        /// <summary>
        /// Compile an insert statement into SQL.
        /// </summary>
        /// <typeparam name="T">The model on which the query is performed.</typeparam>
        /// <param name="query">The query builder instance.</param>
        /// <param name="values">The values to insert.</param>
        /// <returns>The compiled insert query.</returns>
        public static string CompileInsert<T>(Builder<T> query, Dictionary<string, dynamic> values) where T : Model<T>, new()
        {
            string columns = string.Join(", ", values.Keys);
            string parameters = string.Join(", ", values.Select(v => "@" + v.Key));

            return string.Format("INSERT INTO {0} ({1}) VALUES ({2})", query.From, columns, parameters);
        }

        /// <summary>
        /// Compile an update statement into SQL.
        /// </summary>
        /// <typeparam name="T">The model on which the query is performed.</typeparam>
        /// <param name="query">The query builder instance.</param>
        /// <param name="values">The columns to update.</param>
        /// <returns>The compiled update query.</returns>
        public static string CompileUpdate<T>(Builder<T> query, Dictionary<string, dynamic> values) where T : Model<T>, new()
        {
            return string.Format("UPDATE {0} SET {1} {2}", query.From, CompileUpdateColumns(values), CompileWheres(query.Wheres));
        }

        /// <summary>
        /// Compile the columns for an update statement.
        /// </summary>
        /// <param name="values">The columns to update.</param>
        /// <returns>The compiled "set" portion.</returns>
        private static string CompileUpdateColumns(Dictionary<string, dynamic> values)
        {
            return string.Join(", ", values.Select(v => string.Format("{0} = @{0}", v.Key)));
        }
    }
}
