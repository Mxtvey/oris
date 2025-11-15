using System;
using System.Reflection;
using System.Text;
using ConsoleApp1;
using Library;  


namespace Library
{
    public static class MigrationGenerator
    {
        public static (string up, string down) Generate(Assembly asm)
        {
            StringBuilder up = new StringBuilder();
            StringBuilder down = new StringBuilder();

        
            var all = asm.GetTypes();

            foreach (var type in all)
            {
                
                var tableAttr = type.GetCustomAttribute<TableAttribute>();
                if (tableAttr == null)
                    continue;

                string tableName = tableAttr.Name;

             
                up.AppendLine("CREATE TABLE IF NOT EXISTS " + tableName + " (");

                var props = type.GetProperties();
                bool first = true;

                foreach (var p in props)
                {
                    var col = p.GetCustomAttribute<ColumnAttribute>();
                    var pk = p.GetCustomAttribute<PrimaryKeyAttribute>();

                  
                    if (col == null && pk == null)
                        continue;

                    if (!first)
                        up.AppendLine(",");

                    string sqlType = "TEXT";
                    if (p.PropertyType == typeof(int))
                        sqlType = "INTEGER";

                    up.Append("    " + p.Name + " " + sqlType);

                    if (pk != null)
                        up.Append(" PRIMARY KEY");

                    first = false;
                }
                up.AppendLine();
                up.AppendLine(");");
                down.AppendLine("DROP TABLE IF EXISTS " + tableName + ";");
            }
            

            return (up.ToString(), down.ToString());
        }
    }
}
