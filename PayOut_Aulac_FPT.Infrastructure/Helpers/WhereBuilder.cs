using PayOut_Aulac_FPT.Core.Utils.Attributes;
using PayOut_Aulac_FPT.Infrastructure.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PayOut_Aulac_FPT.Infrastructure.Helpers
{
    internal class WhereBuilder
    {
        private Dictionary<string, string>? dict;
        private object? obj;
        private string? tableName;
        private bool like;
        private string[]? notEquals;
        private EWhereJoin joinT;
        private bool multiTable;

        public WhereBuilder(string tableName)
        {
            this.tableName = tableName;
            like = false;
            multiTable = false;
            joinT = EWhereJoin.AND;
        }

        public WhereBuilder WithDictionary(Dictionary<string, string>? dict)
        {
            this.dict = dict;
            var someObject = new Object();
            var someObjectType = someObject.GetType();
            foreach (var item in dict)
            {
                someObjectType.GetProperty(item.Key)?
                    .SetValue(someObject, item.Value, null);
            }
            this.obj = someObject;
            return this;
        }

        public WhereBuilder WithLike(bool like = false)
        {
            this.like = like;
            return this;
        }

        public WhereBuilder WithTableName(string? tableName)
        {
            this.tableName = tableName;
            return this;
        }

        public WhereBuilder WithJoin(EWhereJoin joinT = EWhereJoin.AND)
        {
            this.joinT = joinT;
            return this;
        }

        public WhereBuilder WithMultiTable(bool multiTable = false)
        {
            this.multiTable = multiTable;
            return this;
        }

        public WhereBuilder WithNot(string[] notEquals)
        {
            this.notEquals = notEquals;
            return this;
        }

        public WhereBuilder WithObj(object? obj)
        {
            this.obj = obj;
            return this;
        }

        public string? Build()
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
                        string key = property.Name;
                        var type = value.GetType();
                        if (multiTable)
                        {
                            string? _tableName = tableName;
                            string columnName = property.Name;

                            TableForeignAttribute? tableForeignAttribute = property?.GetCustomAttribute<TableForeignAttribute>();
                            if (tableForeignAttribute != null)
                            {
                                _tableName = tableForeignAttribute.TableName;
                                columnName = tableForeignAttribute.ColumnName;
                            }
                            key = String.Format("{0}.{1}", _tableName, columnName);
                        }
                        columns.Add(GetClause(property?.Name, key, value, type));
                    }
                }

                return columns.Count > 0 ? String.Join($" {joinT} ", columns.ToArray()) : null;
            }
            return null;
        }

        private string? GetClause(string? column, string? key, object value, Type type)
        {
            bool isString = type == typeof(string);
            string compare = "";

            if (notEquals?.Contains(column) == true)
            {
                compare = (like && isString) ? "NOT LIKE" : "!=";
            }
            else
            {
                compare = (like && isString) ? "LIKE" : "=";
            }

            if (isString)
            {
                return String.Format("{0} {1} N'{2}'", key, compare, value);
            }
            else
            {
                if (type == typeof(int) || type == typeof(float))
                {
                    return String.Format("{0} {1} {2}", key, compare, value);
                }
                else if (type == typeof(bool))
                {
                    return String.Format("{0} {1} {2}", key, compare, (bool)value ? 1 : 0);
                }
                else
                {
                    return String.Format("{0} {1} N'{2}'", key, compare, value);
                }
            }
        }
    }
}
