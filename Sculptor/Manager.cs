using System;
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
        /// The current debug mode status.
        /// </summary>
        public static bool DebugMode { get; private set; } = false;

        /// <summary>
        /// The delegate to invoke for logging.
        /// </summary>
        public static Action<string> Logger { get; private set; }

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

        /// <summary>
        /// Enable or disable the debug mode.
        /// </summary>
        /// <param name="status">The status of the debug mode.</param>
        public static void EnableDebugMode(bool status)
        {
            DebugMode = status;
        }

        /// <summary>
        /// Set the logger to use in debug mode.
        /// </summary>
        /// <param name="logger">The delegate to invoke in debug mode.</param>
        public static void SetLogger(Action<string> logger)
        {
            Logger = logger;
        }
    }
}
