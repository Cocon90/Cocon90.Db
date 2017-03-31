using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Exceptions
{

    [Serializable]
    public class PrimaryKeyCountUnequalExceptionException : Exception
    {
        public string[] PrimaryKeys { get; set; }
        public object[] PrimaryKeyValues { get; set; }
        public PrimaryKeyCountUnequalExceptionException() { }
        public PrimaryKeyCountUnequalExceptionException(string message) : base(message) { }
        public PrimaryKeyCountUnequalExceptionException(string message, Exception inner) : base(message, inner) { }
    }
}
