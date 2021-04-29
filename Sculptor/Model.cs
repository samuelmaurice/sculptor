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
            if (Convert.ToInt32(this.GetType().GetProperty("Id").GetValue(this)) == 0)
            {
                Connection.Execute(CompileInsert().Query, CompileInsert().Parameters);
                this.GetType().GetProperty("Id").SetValue(this, Connection.LastInsertId);
            }
            else
            {
                Connection.Execute(CompileUpdate().Query, CompileUpdate().Parameters);
            }
        }

        public async Task SaveAsync()
        {
            if (Convert.ToInt32(this.GetType().GetProperty("Id").GetValue(this)) == 0)
            {
                await Connection.ExecuteAsync(CompileInsert().Query, CompileInsert().Parameters);
                this.GetType().GetProperty("Id").SetValue(this, Connection.LastInsertId);
            }
            else
            {
                await Connection.ExecuteAsync(CompileUpdate().Query, CompileUpdate().Parameters);
            }
        }

        private (string Query, Dictionary<string, dynamic> Parameters) CompileInsert()
        {
            Dictionary<string, dynamic> parameters = GetParameters();

            string columns = String.Join(", ", parameters.Keys);
            string values = String.Join(", ", parameters.Select(p => "@" + p.Key));

            return (String.Format("INSERT INTO {0} ({1}) VALUES ({2})", Table, columns, values), parameters);
        }

        private (string Query, Dictionary<string, dynamic> Parameters) CompileUpdate()
        {
            Dictionary<string, dynamic> parameters = GetParameters();

            string columns = String.Join(", ", parameters.Select(p => String.Format("{0} = @{0}", p.Key)));
            int id = Convert.ToInt32(this.GetType().GetProperty("Id").GetValue(this));

            return (String.Format("UPDATE {0} SET {1} WHERE id = {2}", Table, columns, id), parameters);
        }

        private Dictionary<string, dynamic> GetParameters()
        {
            PropertyInfo[] properties = this.GetType().GetProperties();
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();

            string[] bannedProperties = { "Id", "Table" };

            foreach (var property in properties)
                if (!bannedProperties.Any(property.Name.Contains))
                    parameters.Add(property.Name.ToSnakeCase(), property.GetValue(this));

            return parameters;
        }
    }
}
