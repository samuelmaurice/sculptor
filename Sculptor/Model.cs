using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static string Table => typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name.Pluralize().ToSnakeCase();

        /// <summary>
        /// The primary key name of the model.
        /// </summary>
        private static string PrimaryKeyName
        {
            get
            {
                foreach (var property in typeof(T).GetProperties())
                    if (Attribute.IsDefined(property, typeof(PrimaryKeyAttribute)))
                        return property.Name;

                return "Id";
            }
        }

        /// <summary>
        /// The primary key of the model.
        /// </summary>
        private dynamic PrimaryKey
        {
            get { return typeof(T).GetProperty(PrimaryKeyName).GetValue(this); }
            set { typeof(T).GetProperty(PrimaryKeyName).SetValue(this, value); }
        }

        /// <summary>
        /// Indicates if the model exists.
        /// </summary>
        public bool Exists => Convert.ToBoolean(typeof(T).GetProperty(PrimaryKeyName).GetValue(this));

        /// <summary>
        /// Get a new query builder instance for the model.
        /// </summary>
        public static Builder<T> Query => Builder<T>.Query(Table);

        /// <summary>
        /// The loaded relationships for the model.
        /// </summary>
        private Dictionary<string, object> Relations { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Find a model by its primary key.
        /// </summary>
        /// <param name="id">The value of the primary key.</param>
        /// <returns>An instance of the hydrated model.</returns>
        public static T Find(int id)
        {
            return Query.Where(PrimaryKeyName.ToSnakeCase(), id).First();
        }

        /// <summary>
        /// Asynchronously find a model by its primary key.
        /// </summary>
        /// <param name="id">The value of the primary key.</param>
        /// <returns>An instance of the hydrated model.</returns>
        public static async Task<T> FindAsync(int id)
        {
            return await Query.Where(PrimaryKeyName.ToSnakeCase(), id).FirstAsync();
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
        /// Define a one-to-one relationship.
        /// </summary>
        /// <typeparam name="R">The child model.</typeparam>
        /// <returns>An instance of the related model.</returns>
        public R HasOne<R>() where R : Model<R>, new()
        {
            string relationName = typeof(T).Name;

            if (Relations.TryGetValue(relationName, out object instance))
                return (R)instance;

            string foreignKeyName = typeof(T).Name.ToSnakeCase() + "_id";
            Relations.Add(relationName, Model<R>.Query.Where(foreignKeyName, PrimaryKey).First());

            return (R)Relations[relationName];
        }

        /// <summary>
        /// Asynchronously define a one-to-one relationship.
        /// </summary>
        /// <typeparam name="R">The child model.</typeparam>
        /// <returns>An instance of the related model.</returns>
        public async Task<R> HasOneAsync<R>() where R : Model<R>, new()
        {
            string relationName = typeof(T).Name;

            if (Relations.TryGetValue(relationName, out object instance))
                return (R)instance;

            string foreignKeyName = typeof(T).Name.ToSnakeCase() + "_id";
            Relations.Add(relationName, await Model<R>.Query.Where(foreignKeyName, PrimaryKey).FirstAsync());

            return (R)Relations[relationName];
        }

        /// <summary>
        /// Define an inverse one-to-one or many relationship.
        /// </summary>
        /// <typeparam name="R">The parent model.</typeparam>
        /// <returns>An instance of the related model.</returns>
        public R BelongsTo<R>() where R : Model<R>, new()
        {
            string relationName = new StackTrace().GetFrame(1).GetMethod().Name.Substring(4);

            if (Relations.TryGetValue(relationName, out object instance))
                return (R)instance;

            int foreignKey = Convert.ToInt32(typeof(T).GetProperty(relationName + "Id").GetValue(this));
            Relations.Add(relationName, Model<R>.Find(foreignKey));

            return (R)Relations[relationName];
        }

        /// <summary>
        /// Asynchronously define an inverse one-to-one or many relationship.
        /// </summary>
        /// <typeparam name="R">The parent model.</typeparam>
        /// <returns>An instance of the related model.</returns>
        public async Task<R> BelongsToAsync<R>() where R : Model<R>, new()
        {
            string relationName = new StackTrace().GetFrame(1).GetMethod().Name.Substring(4);

            if (Relations.TryGetValue(relationName, out object instance))
                return (R)instance;

            int foreignKey = Convert.ToInt32(typeof(T).GetProperty(relationName + "Id").GetValue(this));
            Relations.Add(relationName, await Model<R>.FindAsync(foreignKey));

            return (R)Relations[relationName];
        }

        /// <summary>
        /// Define a one-to-many relationship.
        /// </summary>
        /// <typeparam name="R">The child model.</typeparam>
        /// <returns>A list of related models.</returns>
        public List<R> HasMany<R>() where R : Model<R>, new()
        {
            string relationName = new StackTrace().GetFrame(1).GetMethod().Name.Substring(4);

            if (Relations.TryGetValue(relationName, out object instance))
                return (List<R>)instance;

            string foreignKeyName = typeof(T).Name.ToSnakeCase() + "_id";
            Relations.Add(relationName, Model<R>.Query.Where(foreignKeyName, PrimaryKey).Get());

            return (List<R>)Relations[relationName];
        }

        /// <summary>
        /// Asynchronously define a one-to-many relationship.
        /// </summary>
        /// <typeparam name="R">The child model.</typeparam>
        /// <returns>A list of related models.</returns>
        public async Task<List<R>> HasManyAsync<R>() where R : Model<R>, new()
        {
            string relationName = new StackTrace().GetFrame(1).GetMethod().Name.Substring(4);

            if (Relations.TryGetValue(relationName, out object instance))
                return (List<R>)instance;

            string foreignKeyName = typeof(T).Name.ToSnakeCase() + "_id";
            Relations.Add(relationName, await Model<R>.Query.Where(foreignKeyName, PrimaryKey).GetAsync());

            return (List<R>)Relations[relationName];
        }

        /// <summary>
        /// Perform a model insert operation.
        /// </summary>
        private void PerformInsert()
        {
            PrimaryKey = Query.Insert(PrepareBindings());
        }

        /// <summary>
        /// Perform a model insert operation asynchronously.
        /// </summary>
        private async Task PerformInsertAsync()
        {
            PrimaryKey = await Query.InsertAsync(PrepareBindings());
        }

        /// <summary>
        /// Perform a model update operation.
        /// </summary>
        private void PerformUpdate()
        {
            Query.Where(PrimaryKeyName.ToSnakeCase(), PrimaryKey).Update(PrepareBindings());
        }

        /// <summary>
        /// Perform a model update operation asynchronously.
        /// </summary>
        private async Task PerformUpdateAsync()
        {
            await Query.Where(PrimaryKeyName.ToSnakeCase(), PrimaryKey).UpdateAsync(PrepareBindings());
        }

        /// <summary>
        /// Get the current query value bindings.
        /// </summary>
        /// <returns>A collection of columns and values.</returns>
        private Dictionary<string, dynamic> PrepareBindings()
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Dictionary<string, dynamic> bindings = properties.Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string)).ToDictionary(p => Attribute.IsDefined(p, typeof(ColumnAttribute)) ? p.GetCustomAttribute<ColumnAttribute>().Name : p.Name.ToSnakeCase(), p => p.GetValue(this));

            bindings.Remove(PrimaryKeyName.ToSnakeCase());

            return bindings;
        }
    }
}
