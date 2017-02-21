using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Db.Common.Data
{
    [Serializable]
    [DataContract]
    public class SqlBatch
    {
        [DataMember]
        public string Sql { get; set; }
        [DataMember]
        public Params[] Params { get; set; }
        public SqlBatch(string sql, params Params[] param)
        {
            this.Sql = sql;
            this.Params = param;
        }
    }
}
