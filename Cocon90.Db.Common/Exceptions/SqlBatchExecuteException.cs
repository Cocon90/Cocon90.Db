using Cocon90.Db.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Exceptions
{

    [Serializable]
    public class SqlBatchExecuteException : Exception
    {
        public SqlBatch CurrentSqlBatch { get; set; }
        public IEnumerable<SqlBatch> AllSqlBatch { get; set; }
        public SqlBatchExecuteException() { }
        public SqlBatchExecuteException(string message) : base(message) { }
        public SqlBatchExecuteException(string message, Exception inner) : base(message, inner) { }
        protected SqlBatchExecuteException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
