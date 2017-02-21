using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Db.Common.Data
{
    [Serializable]
    [DataContract]
    public class PagedSqlBatch
    {
        [DataMember]
        public SqlBatch PagedSql { get; set; }

        [DataMember]
        public SqlBatch CountSql { get; set; }
    }
}
