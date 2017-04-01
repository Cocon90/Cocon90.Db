using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Db.Common.Data.Schema
{
    /// <summary>
    /// Class MDataTable.
    /// </summary>
    [Serializable]
    [DataContract]
    public class MDataTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MDataTable"/> class.
        /// </summary>
        public MDataTable(List<MColumn> columns = null)
        {
            this.Columns = columns ?? new List<MColumn>();
            this.Rows = new List<MRow>();
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public List<MColumn> Columns { get; set; }
        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public List<MRow> Rows { get; set; }

        /// <summary>
        /// Add a row to this table.
        /// </summary>
        /// <param name="cells"></param>
        public void AddRow(IEnumerable<MCell> cells)
        {
            var row = new MRow(this.Columns);
            row.Cells.AddRange(cells);
            this.Rows.Add(row);
        }

        /// <summary>
        /// Return is contains columnName.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool ContainsColumn(string columnName)
        {
            return ColumnIndexOf(columnName) >= 0;
        }
        /// <summary>
        /// Return is columnName index.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int ColumnIndexOf(string columnName)
        {
            var index = this.Columns.FindIndex(m => m.Caption?.ToLower() == columnName?.ToLower());
            return index;
        }
    }
}
