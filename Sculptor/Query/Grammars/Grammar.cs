using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sculptor.Query.Grammars
{
    public abstract class Grammar
    {
        /// <summary>
        /// The components that make up a select clause.
        /// </summary>
        protected Dictionary<string, Func<dynamic, string>> Components { get; private set; }

        /// <summary>
        /// Create a new grammar instance.
        /// </summary>
        public Grammar()
        {
            Components = new Dictionary<string, Func<dynamic, string>>()
            {
                { "Aggregates", CompileAggregates },
                { "Columns", CompileColumns },
                { "From", CompileFrom },
                { "Wheres", CompileWheres },
                { "Limit", CompileLimit }
            };
        }

        /// <summary>
        /// Compile a select query into SQL.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <returns>The compiled select query.</returns>
        public virtual string CompileSelect(dynamic query)
        {
            List<string> sql = new List<string>();

            foreach (var component in Components)
                sql.Add(component.Value.Invoke(query));

            return string.Join(" ", sql.Where(s => !string.IsNullOrEmpty(s)));
        }

        /// <summary>
        /// Compile aggregated select clauses.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <returns>The compiled "select" portion.</returns>
        protected virtual string CompileAggregates(dynamic query)
        {
            List<Aggregate> aggregates = query.Aggregates;

            if (aggregates.Any())
                return string.Format("SELECT {0}", string.Join(", ", aggregates));

            return "";
        }

        /// <summary>
        /// Compile the "select" portion of the query.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <returns>The compiled "select" portion.</returns>
        protected virtual string CompileColumns(dynamic query)
        {
            if (query.Aggregates.Any())
                return "";

            return string.Format("SELECT {0}", string.Join(", ", query.Columns ?? new string[] { "*" }));
        }

        /// <summary>
        /// Compile the "from" portion of the query.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <returns>The compiled "from" portion.</returns>
        protected virtual string CompileFrom(dynamic query)
        {
            return string.Format("FROM {0}", query.From);
        }

        /// <summary>
        /// Compile the "where" portion of the query.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <returns>The compiled "where" portion.</returns>
        protected virtual string CompileWheres(dynamic query)
        {
            List<WhereClause> whereClauses = query.Wheres;

            if (whereClauses.Any())
                return string.Format("WHERE {0}", RemoveLeadingBoolean(string.Join(" ", whereClauses)));

            return "";
        }

        /// <summary>
        /// Compile the "limit" portion of the query.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <returns>The compiled "limit" portion.</returns>
        protected virtual string CompileLimit(dynamic query)
        {
            if (query.Limit > 0)
                return string.Format("LIMIT {0}", query.Limit);

            return "";
        }

        /// <summary>
        /// Compile an insert statement into SQL.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <param name="values">The values to insert.</param>
        /// <returns>The compiled insert query.</returns>
        public virtual string CompileInsert(dynamic query, Dictionary<string, dynamic> values)
        {
            string columns = string.Join(", ", values.Keys);
            string parameters = string.Join(", ", values.Select(v => "@" + v.Key));

            return string.Format("INSERT INTO {0} ({1}) VALUES ({2})", query.From, columns, parameters);
        }

        /// <summary>
        /// Compile an update statement into SQL.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <param name="values">The columns to update.</param>
        /// <returns>The compiled update query.</returns>
        public virtual string CompileUpdate(dynamic query, Dictionary<string, dynamic> values)
        {
            return string.Format("UPDATE {0} SET {1} {2}", query.From, CompileUpdateColumns(values), CompileWheres(query));
        }

        /// <summary>
        /// Compile the columns for an update statement.
        /// </summary>
        /// <param name="values">The columns to update.</param>
        /// <returns>The compiled "set" portion.</returns>
        protected virtual string CompileUpdateColumns(Dictionary<string, dynamic> values)
        {
            return string.Join(", ", values.Select(v => string.Format("{0} = @{0}", v.Key)));
        }

        /// <summary>
        /// Remove the leading boolean from a statement.
        /// </summary>
        /// <returns>The statement without the leading boolean.</returns>
        protected static string RemoveLeadingBoolean(string value)
        {
            return new Regex("AND |OR ", RegexOptions.IgnoreCase).Replace(value, "", 1);
        }
    }
}
