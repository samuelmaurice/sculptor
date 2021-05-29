using System.Collections.Generic;

namespace Sculptor.Database.Connectors
{
    public interface IConnectionSync
    {
        /// <summary>
        /// Run a select statement against the database.
        /// </summary>
        /// <typeparam name="T">The model related to the rows.</typeparam>
        /// <param name="query">The prepared query to run.</param>
        /// <param name="bindings">The parameterized place-holders to be replaced.</param>
        /// <returns>A result set of rows.</returns>
        public ResultSet<T> Select<T>(string query, Dictionary<string, dynamic> bindings = null) where T : Model<T>, new();

        /// <summary>
        /// Run an insert statement against the database.
        /// </summary>
        /// <param name="query">The prepared query to run.</param>
        /// <param name="bindings">The parameterized place-holders to be replaced.</param>
        /// <returns>The last inserted ID in the database.</returns>
        public int Insert(string query, Dictionary<string, dynamic> bindings = null);

        /// <summary>
        /// Run an update statement against the database.
        /// </summary>
        /// <param name="query">The prepared query to run.</param>
        /// <param name="bindings">The parameterized place-holders to be replaced.</param>
        public void Update(string query, Dictionary<string, dynamic> bindings = null);

        /// <summary>
        /// Run a delete statement against the database.
        /// </summary>
        /// <param name="query">The prepared query to run.</param>
        /// <param name="bindings">The parameterized place-holders to be replaced.</param>
        public void Delete(string query, Dictionary<string, dynamic> bindings = null);
    }
}
