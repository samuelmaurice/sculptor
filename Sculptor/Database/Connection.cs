using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace Sculptor.Database
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

        public static ResultSet<T> Fetch<T>(string query, Dictionary<string, dynamic> parameters = null) where T : Model<T>, new()
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            BindParameters(command, parameters);

            using var reader = command.ExecuteReader();
            ResultSet<T> resultSet = new ResultSet<T>();

            while (reader.Read())
            {
                ResultRow<T> resultRow = new ResultRow<T>();

                for (int column = 0; column < reader.FieldCount; column++)
                    resultRow.Columns.Add(reader.GetName(column), reader.GetValue(column));

                resultSet.Rows.Add(resultRow);
            }

            return resultSet;
        }

        public static async Task<ResultSet<T>> FetchAsync<T>(string query, Dictionary<string, dynamic> parameters = null) where T : Model<T>, new()
        {
            using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(query, connection);
            BindParameters(command, parameters);

            using var reader = await command.ExecuteReaderAsync();
            ResultSet<T> resultSet = new ResultSet<T>();

            while (await reader.ReadAsync())
            {
                ResultRow<T> resultRow = new ResultRow<T>();

                for (int column = 0; column < reader.FieldCount; column++)
                    resultRow.Columns.Add(reader.GetName(column), reader.GetValue(column));

                resultSet.Rows.Add(resultRow);
            }

            return resultSet;
        }

        private static void BindParameters(MySqlCommand command, Dictionary<string, dynamic> parameters = null)
        {
            if (parameters != null)
                foreach (var parameter in parameters)
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
    }
}
