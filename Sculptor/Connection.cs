using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace Sculptor
{
    public static class Connection
    {
        public static string ConnectionString { get; set; }
        public static int LastInsertId { get; private set; }

        public static void Execute(string query, Dictionary<string, dynamic> parameters = null)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            BindParameters(command, parameters);
            command.ExecuteNonQuery();

            LastInsertId = Convert.ToInt32(command.LastInsertedId);
        }

        public static async Task ExecuteAsync(string query, Dictionary<string, dynamic> parameters = null)
        {
            using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(query, connection);
            BindParameters(command, parameters);
            await command.ExecuteNonQueryAsync();

            LastInsertId = Convert.ToInt32(command.LastInsertedId);
        }

        public static List<Dictionary<string, dynamic>> Fetch(string query, Dictionary<string, dynamic> parameters = null)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            BindParameters(command, parameters);

            using var reader = command.ExecuteReader();
            List<Dictionary<string, dynamic>> results = new List<Dictionary<string, dynamic>>();

            while (reader.Read())
            {
                Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();

                for (int column = 0; column < reader.FieldCount; column++)
                    result.Add(reader.GetName(column), reader.GetValue(column));

                results.Add(result);
            }

            return results;
        }

        public static async Task<List<Dictionary<string, dynamic>>> FetchAsync(string query, Dictionary<string, dynamic> parameters = null)
        {
            using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(query, connection);
            BindParameters(command, parameters);

            using var reader = await command.ExecuteReaderAsync();
            List<Dictionary<string, dynamic>> results = new List<Dictionary<string, dynamic>>();

            while (await reader.ReadAsync())
            {
                Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();

                for (int column = 0; column < reader.FieldCount; column++)
                    result.Add(reader.GetName(column), reader.GetValue(column));

                results.Add(result);
            }

            return results;
        }

        private static void BindParameters(MySqlCommand command, Dictionary<string, dynamic> parameters = null)
        {
            if (parameters != null)
                foreach (var parameter in parameters)
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
    }
}
