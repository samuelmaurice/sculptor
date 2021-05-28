using System;
using System.Collections.Generic;
using System.Reflection;
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
                if (HasProperty(column.Key))
                    SetProperty(column.Key, column.Value);

            return Model;
        }

        /// <summary>
        /// Check if the model has the given property.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        /// <returns>True if the model has it, false otherwise.</returns>
        private bool HasProperty(string property)
        {
            return typeof(T).GetProperty(property.ToPascalCase()) != null;
        }

        /// <summary>
        /// Set the given property on the model.
        /// </summary>
        /// <param name="column">The name of the property.</param>
        /// <param name="value">The value for the property.</param>
        private void SetProperty(string column, dynamic value)
        {
            PropertyInfo property = typeof(T).GetProperty(column.ToPascalCase());

            property.SetValue(Model, Convert.ChangeType(value, property.PropertyType));
        }
    }
}
