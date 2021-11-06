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
        /// <summary>
        /// The database connection string.
        /// </summary>
        public static string ConnectionString { get; private set; }

        /// <inheritdoc />
        public Grammar Grammar => new MySqlGrammar();

        /// <summary>
        /// Create a new MySql connection instance.
        /// </summary>
        /// <param name="host">The hostname of the MySQL server.</param>
        /// <param name="database">The name of the database.</param>
        /// <param name="user">The user for the connection.</param>
        /// <param name="password">The password for the given user.</param>
        /// <param name="port">The port of the MySQL server.</param>
        public MySqlConnection(string host, string database, string user, string password, int port = 3306)
        {
            ConnectionString = string.Format("Host={0};Port={1};User={2};Password={3};Database={4}", host, port, user, password, database);
        }

        /// <inheritdoc />
        public ResultSet<T> Select<T>(string query, Dictionary<string, dynamic> bindings = null) where T : Model<T>, new()
        {
            using var connection = new Connection(ConnectionString);
            connection.Open();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);

            using var reader = command.ExecuteReader();
            ResultSet<T> resultSet = new ResultSet<T>();

            if (Manager.DebugMode)
                Manager.Logger(query);

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
            using var connection = new Connection(ConnectionString);
            await connection.OpenAsync();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);

            using var reader = await command.ExecuteReaderAsync();
            ResultSet<T> resultSet = new ResultSet<T>();

            if (Manager.DebugMode)
                Manager.Logger(query);

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
            using var connection = new Connection(ConnectionString);
            connection.Open();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            command.ExecuteNonQuery();

            if (Manager.DebugMode)
                Manager.Logger(query);

            return Convert.ToInt32(command.LastInsertedId);
        }

        /// <inheritdoc />
        public async Task<int> InsertAsync(string query, Dictionary<string, dynamic> bindings = null)
        {
            using var connection = new Connection(ConnectionString);
            await connection.OpenAsync();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            await command.ExecuteNonQueryAsync();

            if (Manager.DebugMode)
                Manager.Logger(query);

            return Convert.ToInt32(command.LastInsertedId);
        }

        /// <inheritdoc />
        public void Update(string query, Dictionary<string, dynamic> bindings = null)
        {
            using var connection = new Connection(ConnectionString);
            connection.Open();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            command.ExecuteNonQuery();

            if (Manager.DebugMode)
                Manager.Logger(query);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(string query, Dictionary<string, dynamic> bindings = null)
        {
            using var connection = new Connection(ConnectionString);
            await connection.OpenAsync();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            await command.ExecuteNonQueryAsync();

            if (Manager.DebugMode)
                Manager.Logger(query);
        }

        /// <inheritdoc />
        public void Delete(string query, Dictionary<string, dynamic> bindings = null)
        {
            using var connection = new Connection(ConnectionString);
            connection.Open();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            command.ExecuteNonQuery();

            if (Manager.DebugMode)
                Manager.Logger(query);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string query, Dictionary<string, dynamic> bindings = null)
        {
            using var connection = new Connection(ConnectionString);
            await connection.OpenAsync();

            using var command = new Command(query, connection);
            BindParameters(command, bindings);
            await command.ExecuteNonQueryAsync();

            if (Manager.DebugMode)
                Manager.Logger(query);
        }

        private static void BindParameters(Command command, Dictionary<string, dynamic> parameters = null)
        {
            if (parameters != null)
                foreach (var parameter in parameters)
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
    }
}
