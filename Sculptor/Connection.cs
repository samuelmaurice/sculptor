using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace Sculptor
{
    public static class Connection
    {
        public static string ConnectionString { get; set; }

        public static void Execute(string query, Dictionary<string, dynamic> parameters = null)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            BindParameters(command, parameters);
            command.ExecuteNonQuery();
        }

        public static async Task ExecuteAsync(string query, Dictionary<string, dynamic> parameters = null)
        {
            using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(query, connection);
            BindParameters(command, parameters);
            await command.ExecuteNonQueryAsync();
        }

        private static void BindParameters(MySqlCommand command, Dictionary<string, dynamic> parameters = null)
        {
            if (parameters != null)
                foreach (var parameter in parameters)
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
    }
}
