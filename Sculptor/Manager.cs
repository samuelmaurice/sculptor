using Sculptor.Database.Connectors;

namespace Sculptor
{
    public static class Manager
    {
        /// <summary>
        /// The database connection instance.
        /// </summary>
        public static IConnection Connection { get; set; }

        /// <summary>
        /// The database connection string.
        /// </summary>
        public static string ConnectionString { get; set; }
    }
}
