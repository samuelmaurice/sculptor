using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sculptor.Database.Connectors
{
    public interface IConnectionAsync
    {
        /// <summary>
        /// Run an asynchronous select statement against the database.
        /// </summary>
        /// <typeparam name="T">The model related to the rows.</typeparam>
        /// <param name="query">The prepared query to run.</param>
        /// <param name="bindings">The parameterized place-holders to be replaced.</param>
        /// <returns>A result set of rows.</returns>
        public Task<ResultSet<T>> SelectAsync<T>(string query, Dictionary<string, dynamic> bindings = null) where T : Model<T>, new();

        /// <summary>
        /// Run an asynchronous insert statement against the database.
        /// </summary>
        /// <param name="query">The prepared query to run.</param>
        /// <param name="bindings">The parameterized place-holders to be replaced.</param>
        /// <returns>The last inserted ID in the database.</returns>
        public Task<int> InsertAsync(string query, Dictionary<string, dynamic> bindings = null);

        /// <summary>
        /// Run an asynchronous update statement against the database.
        /// </summary>
        /// <param name="query">The prepared query to run.</param>
        /// <param name="bindings">The parameterized place-holders to be replaced.</param>
        public Task UpdateAsync(string query, Dictionary<string, dynamic> bindings = null);

        /// <summary>
        /// Run an asynchronous delete statement against the database.
        /// </summary>
        /// <param name="query">The prepared query to run.</param>
        /// <param name="bindings">The parameterized place-holders to be replaced.</param>
        public Task DeleteAsync(string query, Dictionary<string, dynamic> bindings = null);
    }
}
