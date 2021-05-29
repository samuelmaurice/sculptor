using System.Collections.Generic;
using System.Threading.Tasks;
using Sculptor.Database;
using Sculptor.Database.Connectors;
using Sculptor.Exceptions;

namespace Sculptor.Query
{
    public class Builder<T> where T : Model<T>, new()
    {
        /// <summary>
        /// The database connection instance.
        /// </summary>
        private IConnection Connection => Manager.Connections[typeof(Model<T>).GetProperty("Connection").GetValue(null).ToString()];

        /// <summary>
        /// The columns that should be returned.
        /// </summary>
        public string[] Columns { get; private set; }

        /// <summary>
        /// The table which the query is targeting.
        /// </summary>
        public string From { get; private set; }

        /// <summary>
        /// The where constraints for the query.
        /// </summary>
        public List<WhereClause> Wheres { get; private set; }

        /// <summary>
        /// The maximum number of records to return.
        /// </summary>
        public int Limit { get; private set; }

        /// <summary>
        /// Create a new instance of the query builder.
        /// </summary>
        /// <param name="table">The table which the query is targeting.</param>
        private Builder(string table)
        {
            From = table;
            Wheres = new List<WhereClause>();
        }

        /// <summary>
        /// Get a new instance of the query builder.
        /// </summary>
        /// <param name="table">The table which the query is targeting.</param>
        /// <returns>The current query.</returns>
        public static Builder<T> Query(string table)
        {
            return new Builder<T>(table);
        }

        /// <summary>
        /// Set the columns to be selected.
        /// </summary>
        /// <param name="columns">The columns that should be returned.</param>
        /// <returns>The current query.</returns>
        public Builder<T> Select(params string[] columns)
        {
            Columns = columns;

            return this;
        }

        /// <summary>
        /// Add a basic where clause to the query.
        /// </summary>
        /// <param name="column">The column to which the clause applies.</param>
        /// <param name="value">The value of the clause.</param>
        /// <param name="boolean">The boolean that applies for the where clause.</param>
        /// <returns>The current query.</returns>
        public Builder<T> Where(string column, dynamic value, string boolean = "AND")
        {
            Wheres.Add(new WhereClause(column, "=", value, boolean));

            return this;
        }

        /// <summary>
        /// Add a basic where clause with a custom operator to the query.
        /// </summary>
        /// <param name="column">The column to which the clause applies.</param>
        /// <param name="ope">The operator for the where clause.</param>
        /// <param name="value">The value of the clause.</param>
        /// <param name="boolean">The boolean that applies for the where clause.</param>
        /// <returns>The current query.</returns>
        public Builder<T> Where(string column, string ope, dynamic value, string boolean = "AND")
        {
            Wheres.Add(new WhereClause(column, ope, value, boolean));

            return this;
        }

        /// <summary>
        /// Add an "or where" clause to the query.
        /// </summary>
        /// <param name="column">The column to which the clause applies.</param>
        /// <param name="value">The value of the clause.</param>
        /// <returns>The current query.</returns>
        public Builder<T> OrWhere(string column, dynamic value)
        {
            return this.Where(column, value, "OR");
        }

        /// <summary>
        /// Add an "or where" clause with a custom operator to the query.
        /// </summary>
        /// <param name="column">The column to which the clause applies.</param>
        /// <param name="ope">The operator for the where clause.</param>
        /// <param name="value">The value of the clause.</param>
        /// <returns>The current query.</returns>
        public Builder<T> OrWhere(string column, string ope, dynamic value)
        {
            return this.Where(column, ope, value, "OR");
        }

        /// <summary>
        /// Set the "limit" value of the query.
        /// </summary>
        /// <param name="limit">The maximum number of records to return.</param>
        /// <returns>The current query.</returns>
        public Builder<T> Take(int limit)
        {
            if (limit > 0)
                Limit = limit;

            return this;
        }

        /// <summary>
        /// Execute the query and get the first result or throw an exception.
        /// </summary>
        /// <returns>An instance of the hydrated model.</returns>
        public T First()
        {
            ResultSet<T> resultSet = Connection.Select<T>(Connection.Grammar.CompileSelect(this.Take(1)), GetParameters());

            if (resultSet.Rows.Count == 0)
                throw new ModelNotFoundException();

            return resultSet.First();
        }

        /// <summary>
        /// Execute the query asynchronously and get the first result or throw an exception.
        /// </summary>
        /// <returns>An instance of the hydrated model.</returns>
        public async Task<T> FirstAsync()
        {
            ResultSet<T> resultSet = await Connection.SelectAsync<T>(Connection.Grammar.CompileSelect(this.Take(1)), GetParameters());

            if (resultSet.Rows.Count == 0)
                throw new ModelNotFoundException();

            return resultSet.First();
        }

        /// <summary>
        /// Execute the query as a "select" statement.
        /// </summary>
        /// <returns>A list of hydrated models.</returns>
        public List<T> Get()
        {
            ResultSet<T> resultSet = Connection.Select<T>(Connection.Grammar.CompileSelect(this), GetParameters());

            return resultSet.All();
        }

        /// <summary>
        /// Execute the query asynchronously as a "select" statement.
        /// </summary>
        /// <returns>A list of hydrated models.</returns>
        public async Task<List<T>> GetAsync()
        {
            ResultSet<T> resultSet = await Connection.SelectAsync<T>(Connection.Grammar.CompileSelect(this), GetParameters());

            return resultSet.All();
        }

        /// <summary>
        /// Insert a new record and get the value of the primary key.
        /// </summary>
        /// <param name="values">The values of the new record.</param>
        /// <returns>The value of the primary key.</returns>
        public int Insert(Dictionary<string, dynamic> values)
        {
            return Connection.Insert(Connection.Grammar.CompileInsert(this, values), values);
        }

        /// <summary>
        /// Insert a new record asynchronously and get the value of the primary key.
        /// </summary>
        /// <param name="values">The values of the new record.</param>
        /// <returns>The value of the primary key.</returns>
        public async Task<int> InsertAsync(Dictionary<string, dynamic> values)
        {
            return await Connection.InsertAsync(Connection.Grammar.CompileInsert(this, values), values);
        }

        /// <summary>
        /// Update records in the database.
        /// </summary>
        /// <param name="values">The columns to update.</param>
        public void Update(Dictionary<string, dynamic> values)
        {
            Connection.Update(Connection.Grammar.CompileUpdate(this, values), GetParameters(values));
        }

        /// <summary>
        /// Asynchronously update records in the database.
        /// </summary>
        /// <param name="values">The columns to update.</param>
        public async Task UpdateAsync(Dictionary<string, dynamic> values)
        {
            await Connection.UpdateAsync(Connection.Grammar.CompileUpdate(this, values), GetParameters(values));
        }

        /// <summary>
        /// Build a list of parameters including the where clauses.
        /// </summary>
        /// <returns>A collection of columns and values.</returns>
        private Dictionary<string, dynamic> GetParameters(Dictionary<string, dynamic> values = null)
        {
            Dictionary<string, dynamic> parameters = values is null ? new Dictionary<string, dynamic>() : values;
            Wheres.ForEach(w => parameters.Add(w.Column, w.Value));

            return parameters;
        }
    }
}
