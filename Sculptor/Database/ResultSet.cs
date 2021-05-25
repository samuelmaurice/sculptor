using System.Collections.Generic;
using System.Linq;

namespace Sculptor.Database
{
    public class ResultSet<T> where T : Model<T>, new()
    {
        /// <summary>
        /// The set of rows.
        /// </summary>
        public List<ResultRow<T>> Rows { get; private set; } = new List<ResultRow<T>>();

        /// <summary>
        /// Get the first model of the result set.
        /// </summary>
        /// <returns>The first hydrated model.</returns>
        public T First()
        {
            return Rows.First().Hydrate();
        }

        /// <summary>
        /// Get all the models of the result set.
        /// </summary>
        /// <returns>A list of hydrated models.</returns>
        public List<T> All()
        {
            return Rows.Select(r => r.Hydrate()).ToList();
        }
    }
}
