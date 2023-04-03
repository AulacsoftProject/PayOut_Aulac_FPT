using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TableForeignAttribute : Attribute
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public TableForeignAttribute(string tableName, string columnName)
        {
            TableName = tableName;
            ColumnName = columnName;
        }
    }
}
