using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Attribute
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ColumnAttribute : System.Attribute
    {
        public Data.DirverType DirverType { get; set; }
        public string ColumnName { get; set; }
        public bool PrimaryKey { get; set; }
        public string CreateDDL { get; set; }
        
    }
}
