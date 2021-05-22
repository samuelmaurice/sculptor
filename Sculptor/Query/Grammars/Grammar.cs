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
                sql.Add(component.Value.Invoke(query.GetType().GetProperty(component.Key).GetValue(query, null)));

            return string.Join(" ", sql);
        }

        /// <summary>
        /// Compile the "select" portion of the query.
        /// </summary>
        /// <param name="columns">The columns that should be returned.</param>
        /// <returns>The compiled "select" portion.</returns>
        private static string CompileColumns(dynamic columns)
        {
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
                return string.Format("WHERE {0}", string.Join(" AND ", whereClauses)); // To-Do: Parameterize values

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
    }
}
