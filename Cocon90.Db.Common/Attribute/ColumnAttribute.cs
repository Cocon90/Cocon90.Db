using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Attribute
{
    /// <summary>
    /// column option attribute.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ColumnAttribute : System.Attribute
    {
        /// <summary>
        /// option the database type to which the current attribute tag is applied priority.
        /// </summary>
        public Data.DirverType DirverType { get; set; }
        /// <summary>
        /// option the name of the column in the table, and if it is not set, the attribute name is automatically used.
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// option the table primary key. A table can have more than one primary key
        /// </summary>
        public bool PrimaryKey { get; set; }
        /// <summary>
        /// option how to create table use sql ddl except ColumnName. like setting 'int not null default 0' in mysql and sqlite or 'int not null default (0)' in sqlserver.
        /// </summary>
        public string CreateDDL { get; set; }
        /// <summary>
        /// option the index name for the column. if setting same indexName for different column, it's will generate one index in a group. 
        /// </summary>
        public string IndexName { get; set; }
    }
}
