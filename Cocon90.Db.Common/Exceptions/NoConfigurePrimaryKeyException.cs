using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Exceptions
{

    [Serializable]
    public class NoConfigurePrimaryKeyExceptionException : Exception
    {
        public NoConfigurePrimaryKeyExceptionException() { }
        public NoConfigurePrimaryKeyExceptionException(string message) : base(message) { }
        public NoConfigurePrimaryKeyExceptionException(string message, Exception inner) : base(message, inner) { }
        protected NoConfigurePrimaryKeyExceptionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
