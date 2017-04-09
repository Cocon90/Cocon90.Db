using Cocon90.Db.Common.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Db.Common.Data.Schema
{
    /// <summary>
    /// Class MRow.
    /// </summary>
    [Serializable]
    [DataContract]
    public class MRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MRow"/> class.
        /// </summary>
        /// <param name="columns">The columns.</param>
        public MRow(List<MColumn> columns)
        {
            this.Columns = columns ?? new List<MColumn>();
            this.Cells = new List<Schema.MCell>();
        }
        /// <summary>
        /// Get the cell.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public MCell this[int columnIndex]
        {
            get
            {
                return Cells[columnIndex];
            }
        }
        /// <summary>
        /// Get the cell.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public MCell this[string columnName]
        {
            get
            {
                return Cells.Find(mc => mc.Column?.Caption?.ToLower() == columnName?.ToLower());
            }
        }
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public List<MColumn> Columns { get; set; }
        /// <summary>
        /// Gets or sets the cells.
        /// </summary>
        /// <value>The cells.</value>
        public List<MCell> Cells { get; set; }
        public override string ToString()
        {
            return "[Row]:" + string.Join("|", Cells.ConvertToAll(mc => mc.Column?.Caption + "=" + mc.Value));
        }

        /// <summary>
        /// add the cell.
        /// </summary>
        public void AddCell(MColumn column, object value)
        {
            var cell = new MCell(column, value);
            this.Cells.Add(cell);
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
