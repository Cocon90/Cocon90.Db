using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Db.Common.Data.Schema
{
    /// <summary>
    /// Class MDataSet. The Tables Collections.
    /// </summary>
    [Serializable]
    [DataContract]
    public class MDataSet
    {
        public MDataSet()
        {
            this.Tables = new List<MDataTable>();
        }
        /// <summary>
        /// Gets or sets the tables.
        /// </summary>
        /// <value>The tables.</value>
        public List<MDataTable> Tables { get; set; }
    }
}
