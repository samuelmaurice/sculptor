using System.Collections.Generic;
using Sculptor.Database.Connectors;
using Sculptor.Exceptions;

namespace Sculptor
{
    public static class Manager
    {
        /// <summary>
        /// The database connection instances.
        /// </summary>
        public static Dictionary<string, IConnection> Connections { get; } = new Dictionary<string, IConnection>();

        /// <summary>
        /// The default database connection name.
        /// </summary>
        public static string Connection { get; private set; }

        /// <summary>
        /// Set the default database connection name.
        /// </summary>
        /// <param name="connection">The name of the database connection.</param>
        public static void SetDefaultConnection(string connection)
        {
            if (!Connections.ContainsKey(connection))
                throw new ConnectionNotFoundException();

            Connection = connection;
        }
    }
}
