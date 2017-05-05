using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Attribute
{
    /// <summary>
    /// Sets the name of the configuration table, and the schema name of the table.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TableAttribute : System.Attribute
    {
        /// <summary>
        /// Configuration the name of the table
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Sets the name of the schema name of the table.
        /// </summary>
        public string SchemaName { get; set; }
    }
}
