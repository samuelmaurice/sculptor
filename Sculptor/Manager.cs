using System.Collections.Generic;
using Sculptor.Database.Connectors;

namespace Sculptor
{
    public static class Manager
    {
        /// <summary>
        /// The database connection instances.
        /// </summary>
        public static Dictionary<string, IConnection> Connections { get; } = new Dictionary<string, IConnection>();
    }
}
