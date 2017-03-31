using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Db.Common.Data.Schema
{
    /// <summary>
    /// Class MColumn.
    /// </summary>
    [Serializable]
    [DataContract]
    public class MColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MColumn"/> class.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="dataType">Type of the data.</param>
        public MColumn(string caption, Type dataType)
        {
            this.Caption = caption;
            this.DataType = dataType;
        }
        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption { get; set; }
        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public Type DataType { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[Caption]:{0},[DataType]:{1}", Caption, DataType?.Name);
        }
      
    }
}
