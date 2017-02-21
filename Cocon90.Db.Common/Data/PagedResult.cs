using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Db.Common.Data
{
    [Serializable]
    [DataContract]
    public class PagedResult
    {
        [DataMember]
        public int Total { get; set; }

        [DataMember]
        public DataTable Data { get; set; }
        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public int PageSize { get; set; }
    }

    [Serializable]
    [DataContract]
    public class PagedResult<T>
    {
        [DataMember]
        public int Total { get; set; }

        [DataMember]
        public List<T> Data { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public int PageSize { get; set; }
    }
}
