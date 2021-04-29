using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sculptor.Utils;

namespace Sculptor
{
    public abstract class Model
    {
        public string Table
        {
            get { return this.GetType().Name.Pluralize().ToSnakeCase(); }
        }

        public void Save()
        {
            Connection.Execute(CompileInsert().Query, CompileInsert().Parameters);
            this.GetType().GetProperty("Id").SetValue(this, Connection.LastInsertId);
        }

        public async Task SaveAsync()
        {
            await Connection.ExecuteAsync(CompileInsert().Query, CompileInsert().Parameters);
            this.GetType().GetProperty("Id").SetValue(this, Connection.LastInsertId);
        }

        private (string Query, Dictionary<string, dynamic> Parameters) CompileInsert()
        {
            PropertyInfo[] properties = this.GetType().GetProperties();
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();

            string[] bannedProperties = { "Id", "Table" };

            foreach (var property in properties)
                if (!bannedProperties.Any(property.Name.Contains))
                    parameters.Add(property.Name.ToSnakeCase(), property.GetValue(this));

            return (String.Format("INSERT INTO {0} ({1}) VALUES ({2})", Table, String.Join(", ", parameters.Keys), String.Join(", ", parameters.Select(p => "@" + p.Key))), parameters);
        }
    }
}
