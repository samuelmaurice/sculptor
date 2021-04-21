using System.Threading.Tasks;
using MySqlConnector;

namespace Sculptor
{
    public static class Connection
    {
        public static string ConnectionString { get; set; }

        public static void Execute(string query)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();
        }

        public static async Task ExecuteAsync(string query)
        {
            using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(query, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
