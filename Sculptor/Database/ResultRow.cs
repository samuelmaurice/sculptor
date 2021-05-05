using System;
using System.Collections.Generic;
using Sculptor.Utils;

namespace Sculptor.Database
{
    public class ResultRow<T> where T : Model<T>, new()
    {
        private readonly T _model = new T();
        public Dictionary<string, dynamic> Columns { get; } = new Dictionary<string, dynamic>();
        public T Model { get => Initialize(); }

        private T Initialize()
        {
            foreach (KeyValuePair<string, dynamic> column in Columns)
                if (typeof(T).GetProperty(GetPropertyName(column.Key)) != null)
                    SetProperty(GetPropertyName(column.Key), column.Value);

            return _model;
        }

        private string GetPropertyName(string column)
        {
            return column.ToPascalCase().UcFirst();
        }

        private void SetProperty(string property, dynamic value)
        {
            typeof(T).GetProperty(property).SetValue(_model, Convert.ChangeType(value, typeof(T).GetProperty(property).PropertyType));
        }
    }
}
