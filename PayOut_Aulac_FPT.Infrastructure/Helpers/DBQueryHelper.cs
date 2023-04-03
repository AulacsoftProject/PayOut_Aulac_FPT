using Dapper;
using PayOut_Aulac_FPT.Core.Utils.Attributes;
using PayOut_Aulac_FPT.Infrastructure.Helpers.Enums;
using System.Reflection;
using System;
using System.Xml.Linq;
using static Dapper.SqlMapper;

namespace PayOut_Aulac_FPT.Infrastructure.Utils
{
    internal static class DBQueryHelper
    {
        public static string? CreateWhereByDictionary(Dictionary<string, string>? dict, bool like = false, EWhereJoin join = EWhereJoin.AND)
        {
            if (dict != null)
            {
                List<string> columns = new List<string>();
                foreach (var key in dict.Keys)
                {
                    var value = dict[key];
                    if (value != null)
                    {
                        columns.Add(String.Format("{0} {1} N'{2}'", key, like ? "LIKE" : "=", value));
                    }
                }
                return columns.Count > 0 ? String.Join($" {join} ", columns.ToArray()) : null;
            }
            return null;
        }

        public static string? CreateWhere(object? obj, bool like = false, string? tableName = null, bool multiTable = true, EWhereJoin join = EWhereJoin.AND)
        {
            if (obj != null)
            {
                List<string> columns = new List<string>();
                var properties = obj.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(obj);
                    if (value != null)
                    {
                        if (multiTable)
                        {
                            string _tableName = tableName;
                            string columnName = property.Name;

                            TableForeignAttribute? tableForeignAttribute = property?.GetCustomAttribute<TableForeignAttribute>();
                            if (tableForeignAttribute != null)
                            {
                                _tableName = tableForeignAttribute.TableName;
                                columnName = tableForeignAttribute.ColumnName;
                            }
                            string name = String.Format("{0}.{1}", _tableName, columnName);
                            var type = value.GetType();
                            if (type == typeof(string))
                            {
                                columns.Add(String.Format("{0} {1} N'{2}'", name, like ? "LIKE" : "=", value));
                            }
                            else
                            {

                                if (type == typeof(int) || type == typeof(float))
                                {
                                    columns.Add(String.Format("{0} {1} {2}", name, "=", value));
                                }
                                else if (type == typeof(bool))
                                {
                                    columns.Add(String.Format("{0} {1} {2}", name, "=", (bool)value ? 1 : 0));
                                }
                                else
                                {
                                    columns.Add(String.Format("{0} {1} N'{2}'", name, "=", value));
                                }
                            }
                        }
                        else
                        {
                            var type = value.GetType();
                            if (type == typeof(string))
                            {
                                columns.Add(String.Format("{0} {1} N'{2}'", property.Name, like ? "LIKE" : "=", value));
                            }
                            else
                            {

                                if (type == typeof(int) || type == typeof(float))
                                {
                                    columns.Add(String.Format("{0} {1} {2}", property.Name, "=", value));
                                }
                                else if (type == typeof(bool))
                                {
                                    columns.Add(String.Format("{0} {1} {2}", property.Name, "=", (bool)value ? 1 : 0));
                                }
                                else
                                {
                                    columns.Add(String.Format("{0} {1} N'{2}'", property.Name, "=", value));
                                }
                            }
                        }
                    }
                }
                return columns.Count > 0 ? String.Join($" {join} ", columns.ToArray()) : null;
            }
            return null;
        }

        public static DynamicParameters GetParamOnlyTable(object? obj, bool allowPrimaryKey = true)
        {
            var param = new DynamicParameters();
            var typeT = obj?.GetType();
            var properties = typeT?.GetProperties();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var primaryKey = property.GetCustomAttribute<PrimaryKeyAttribute>();
                    if (primaryKey == null || allowPrimaryKey)
                    {
                        var tableForeign = property.GetCustomAttribute<TableForeignAttribute>();
                        if (tableForeign == null)
                        {
                            var value = property.GetValue(obj, null);
                            param.Add(property.Name, value);
                        }
                    }
                }
            }
            return param;
        }

        public static string? GetPrimaryKeyName<T>()
        {
            string? idName = null;
            var typeT = typeof(T);
            var properties = typeT?.GetProperties();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var primaryKey = property.GetCustomAttribute<PrimaryKeyAttribute>();
                    if (primaryKey != null)
                    {
                        idName = property.Name;
                        break;
                    }
                }
            }
            return idName;
        }
    }
}
