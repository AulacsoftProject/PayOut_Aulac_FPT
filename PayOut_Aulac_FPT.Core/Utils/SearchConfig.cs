using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces;
using PayOut_Aulac_FPT.Core.Utils.Attributes;
using PayOut_Aulac_FPT.Core.Utils.Enums;
using PayOut_Aulac_FPT.Core.Utils.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PayOut_Aulac_FPT.Core.Utils
{
    public class SearchConfig<T> where T : IEntity
    {
        //private readonly Dictionary<string, string>? _config;
        private readonly List<string> _where;
        private bool isAnd;

        public SearchConfig()
        {
            //_config = new Dictionary<string, string>();
            _where = new List<string>();
        }

        public SearchConfig(BaseSearch? search)
        {
            //_config = new Dictionary<string, string>();
            _where = new List<string>();
            Add(search);
        }

        public void Add(BaseSearch? search)
        {
            if (search == null) return;
            var properties = search.GetType().GetProperties();
            if (search?.All != null && search.All.Length > 0)
            {
                isAnd = false;
                foreach (var property in properties)
                {
                    string tableName = "{0}";
                    string columnName = property.Name;
                    var typeT = typeof(T);
                    var propT = typeT.GetProperty(columnName);
                    TableForeignAttribute? tableForeignAttribute = propT?.GetCustomAttribute<TableForeignAttribute>();
                    if (tableForeignAttribute != null)
                    {
                        tableName = tableForeignAttribute.TableName;
                        columnName = tableForeignAttribute.ColumnName;
                    }

                    if (property.Name != "All")
                    {
                        _where.Add(string.Format("{0}.{1} LIKE N'%{2}%'", tableName, columnName, search.All));
                    }
                }
            }
            else
            {
                isAnd = true;
                foreach (var property in properties)
                {
                    var value = property.GetValue(search);
                    var typeT = typeof(T);
                    if (value != null && property.Name != "All")
                    {
                        //_config?.Add(property.Name, $"%{value}%");
                        var type = value.GetType();
                        string tableName = "{0}";
                        string columnName = property.Name;

                        var propT = typeT.GetProperty(columnName);
                        TableForeignAttribute? tableForeignAttribute = propT?.GetCustomAttribute<TableForeignAttribute>();
                        string? name = null;
                        if (tableForeignAttribute != null)
                        {
                            tableName = tableForeignAttribute.TableName;
                            //if (columnName == tableForeignAttribute.ColumnName)
                            //{
                            //    name = String.Format("{0}.{1}", tableName, tableForeignAttribute.ColumnName);
                            //}
                            //else
                            //{
                            //    name = columnName;
                            //}
                            columnName = tableForeignAttribute.ColumnName;
                        }

                        name ??= String.Format("{0}.{1}", tableName, columnName);

                        //string name = property.Name;
                        if (type.IsArray)
                        {
                            object[]? values = null;
                            var eleType = type.GetElementType();
                            if (eleType == typeof(int) || eleType == typeof(float))
                            {
                                values = ((IEnumerable)value)?.Cast<object>()?.Select(x => String.Format("{0}", x))?.ToArray();
                            }
                            else if (eleType == typeof(bool))
                            {
                                values = ((IEnumerable)value)?.Cast<object>()?.Select(x => String.Format("{0}", (bool)value ? 1 : 0))?.ToArray();
                            }
                            else
                            {
                                values = ((IEnumerable)value)?.Cast<object>()?.Select(x => String.Format("N'{0}'", x))?.ToArray();
                            }
                            if (values != null && values.Length > 0)
                            {
                                _where.Add(String.Format("{0} IN ({1})", name, String.Join(", ", values)));
                            }
                        }
                        else if (type == typeof(string))
                        {
                            _where.Add(string.Format("{0} LIKE N'%{1}%'", name, value));
                        }
                        else if (type == typeof(DateFilterRange))
                        {
                            var dateFilterRange = (DateFilterRange)value;
                            if (dateFilterRange.Start != null && dateFilterRange.End != null)
                            {
                                DateTime _value = dateFilterRange.Start.Value;
                                DateTime start = dateFilterRange.Start.Value.AddHours(-_value.Hour).AddMinutes(-_value.Minute).AddSeconds(-_value.Second);
                                _value = dateFilterRange.End.Value;
                                DateTime end = dateFilterRange.End.Value.AddHours(23 - _value.Hour).AddMinutes(59 - _value.Minute).AddSeconds(59 - _value.Second);
                                _where.Add(string.Format("{0} BETWEEN '{1}' AND '{2}'", name, start, end));
                            }
                        }
                        else
                        {
                            if (type == typeof(int) || type == typeof(float))
                            {
                                _where.Add(string.Format("{0} = {1}", name, value));
                            }
                            else if (type == typeof(bool))
                            {
                                _where.Add(string.Format("{0} = {1}", name, (bool)value ? 1 : 0));
                            }
                            else
                            {
                                _where.Add(string.Format("{0} = N'{1}'", name, value));
                            }
                        }
                    }
                }
            }
        }

        //public Dictionary<string, string>? GetSearch()
        //{
        //    if (_config?.Count > 0)
        //    {
        //        return _config;
        //    }
        //    return null;
        //}

        public string? GetWhere(string? tableName = null)
        {
            return String.Format(String.Join(isAnd ? " AND " : " OR ", _where), tableName ?? nameof(T));
            //return String.Join(" AND ", _where);
        }
    }
}
