using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Attribute
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TableAttribute : System.Attribute
    {
        public string TableName { get; set; }
        public string SchemaName { get; set; }
    }
}
