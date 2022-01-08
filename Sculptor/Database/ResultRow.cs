using System;
using System.Collections.Generic;
using System.Reflection;
using Sculptor.Attributes;
using Sculptor.Utils;

namespace Sculptor.Database
{
    public class ResultRow<T> where T : Model<T>, new()
    {
        /// <summary>
        /// The columns for the row.
        /// </summary>
        public Dictionary<string, dynamic> Columns { get; private set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// The model related to the row.
        /// </summary>
        public T Model { get; private set; }

        /// <summary>
        /// Initialize and hydrate the model.
        /// </summary>
        /// <returns>An instance of the hydrated model.</returns>
        public T Hydrate()
        {
            Model = new T();

            foreach (var column in Columns)
                SetProperty(column.Key, column.Value);

            return Model;
        }

        /// <summary>
        /// Set the given property on the model if it exists.
        /// </summary>
        /// <param name="column">The name of the property.</param>
        /// <param name="value">The value for the property.</param>
        private void SetProperty(string column, dynamic value)
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (PropertyInfo property in properties)
            {
                if (Attribute.IsDefined(property, typeof(ColumnAttribute)))
                    if (property.GetCustomAttribute<ColumnAttribute>().Name == column)
                        property.SetValue(Model, Convert.ChangeType(value, property.PropertyType));

                if (property.Name == column.ToPascalCase())
                    property.SetValue(Model, Convert.ChangeType(value, property.PropertyType));
            }
        }
    }
}
