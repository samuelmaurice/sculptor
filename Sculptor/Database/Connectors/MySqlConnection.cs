using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sculptor.Query.Grammars;
using Command = MySqlConnector.MySqlCommand;
using Connection = MySqlConnector.MySqlConnection;

namespace Sculptor.Database.Connectors
{
    public class MySqlConnection : IConnection
    {
        /// <inheritdoc />
        public Grammar Grammar => new MySqlGrammar();

        /// <inheritdoc />
        public ResultSet<T> Select<T>(string query, Dictionary<string, dynamic> bindings = null) where T : Model<T>, new()
        {
            using var connection = new Connection(Manager.ConnectionString);
            connection.Open();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);

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

        /// <inheritdoc />
        public async Task<ResultSet<T>> SelectAsync<T>(string query, Dictionary<string, dynamic> bindings = null) where T : Model<T>, new()
        {
            using var connection = new Connection(Manager.ConnectionString);
            await connection.OpenAsync();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);

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

        /// <inheritdoc />
        public int Insert(string query, Dictionary<string, dynamic> bindings = null)
        {
            using var connection = new Connection(Manager.ConnectionString);
            connection.Open();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            command.ExecuteNonQuery();

            return Convert.ToInt32(command.LastInsertedId);
        }

        /// <inheritdoc />
        public async Task<int> InsertAsync(string query, Dictionary<string, dynamic> bindings = null)
        {
            using var connection = new Connection(Manager.ConnectionString);
            await connection.OpenAsync();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            await command.ExecuteNonQueryAsync();

            return Convert.ToInt32(command.LastInsertedId);
        }

        /// <inheritdoc />
        public void Update(string query, Dictionary<string, dynamic> bindings = null)
        {
            using var connection = new Connection(Manager.ConnectionString);
            connection.Open();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            command.ExecuteNonQuery();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(string query, Dictionary<string, dynamic> bindings = null)
        {
            using var connection = new Connection(Manager.ConnectionString);
            await connection.OpenAsync();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            await command.ExecuteNonQueryAsync();
        }

        /// <inheritdoc />
        public void Delete(string query, Dictionary<string, dynamic> bindings = null)
        {
            using var connection = new Connection(Manager.ConnectionString);
            connection.Open();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            command.ExecuteNonQuery();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string query, Dictionary<string, dynamic> bindings = null)
        {
            using var connection = new Connection(Manager.ConnectionString);
            await connection.OpenAsync();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            await command.ExecuteNonQueryAsync();
        }

        private static void BindParameters(Command command, Dictionary<string, dynamic> parameters = null)
        {
            if (parameters != null)
                foreach (var parameter in parameters)
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
    }
}
