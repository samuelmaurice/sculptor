using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sculptor.Attributes;
using Sculptor.Query;
using Sculptor.Utils;

namespace Sculptor
{
    public abstract class Model<T> where T : Model<T>, new()
    {
        /// <summary>
        /// The connection name for the model.
        /// </summary>
        public static string Connection => typeof(T).GetCustomAttribute<ConnectionAttribute>()?.Name ?? Manager.Connection;

        /// <summary>
        /// The table associated with the model.
        /// </summary>
        public static string Table => typeof(T).Name.Pluralize().ToSnakeCase();

        /// <summary>
        /// The primary key of the model.
        /// </summary>
        private int PrimaryKey
        {
            get { return Convert.ToInt32(typeof(T).GetProperty("Id").GetValue(this)); }
            set { typeof(T).GetProperty("Id").SetValue(this, value); }
        }

        /// <summary>
        /// Indicates if the model exists.
        /// </summary>
        public bool Exists => Convert.ToBoolean(typeof(T).GetProperty("Id").GetValue(this));

        /// <summary>
        /// Get a new query builder instance for the model.
        /// </summary>
        public static Builder<T> Query => Builder<T>.Query(Table);

        /// <summary>
        /// Find a model by its primary key.
        /// </summary>
        /// <param name="id">The value of the primary key.</param>
        /// <returns>An instance of the hydrated model.</returns>
        public static T Find(int id)
        {
            return Query.Where("id", id).First();
        }

        /// <summary>
        /// Asynchronously find a model by its primary key.
        /// </summary>
        /// <param name="id">The value of the primary key.</param>
        /// <returns>An instance of the hydrated model.</returns>
        public static async Task<T> FindAsync(int id)
        {
            return await Query.Where("id", id).FirstAsync();
        }

        /// <summary>
        /// Get all of the models from the database.
        /// </summary>
        /// <returns>A list of hydrated models.</returns>
        public static List<T> All()
        {
            return Query.Get();
        }

        /// <summary>
        /// Asynchronously get all of the models from the database.
        /// </summary>
        /// <returns>A list of hydrated models.</returns>
        public static async Task<List<T>> AllAsync()
        {
            return await Query.GetAsync();
        }

        /// <summary>
        /// Save the model to the database.
        /// </summary>
        public void Save()
        {
            if (Exists)
                PerformUpdate();
            else
                PerformInsert();
        }

        /// <summary>
        /// Asynchronously save the model to the database.
        /// </summary>
        public async Task SaveAsync()
        {
            if (Exists)
                await PerformUpdateAsync();
            else
                await PerformInsertAsync();
        }

        /// <summary>
        /// Perform a model insert operation.
        /// </summary>
        private void PerformInsert()
        {
            PrimaryKey = Query.Insert(GetParameters());
        }

        /// <summary>
        /// Perform a model insert operation asynchronously.
        /// </summary>
        private async Task PerformInsertAsync()
        {
            PrimaryKey = await Query.InsertAsync(GetParameters());
        }

        /// <summary>
        /// Perform a model update operation.
        /// </summary>
        private void PerformUpdate()
        {
            Query.Where("id", PrimaryKey).Update(GetParameters());
        }

        /// <summary>
        /// Perform a model update operation asynchronously.
        /// </summary>
        private async Task PerformUpdateAsync()
        {
            await Query.Where("id", PrimaryKey).UpdateAsync(GetParameters());
        }

        /// <summary>
        /// Build a list of parameters.
        /// </summary>
        /// <returns>A collection of columns and values.</returns>
        private Dictionary<string, dynamic> GetParameters()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();

            string[] bannedProperties = { "Id", "Table", "PrimaryKey", "Exists", "Query" };

            foreach (var property in properties)
                if (!bannedProperties.Any(property.Name.Contains))
                    parameters.Add(property.Name.ToSnakeCase(), property.GetValue(this));

            return parameters;
        }
    }
}
