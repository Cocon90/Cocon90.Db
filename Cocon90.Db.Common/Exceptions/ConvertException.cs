using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Exceptions
{

    [Serializable]
    public class ConvertException : Exception
    {
        public object NeedConvertValue { get; set; }
        public Type TargetType { get; set; }
        public ConvertException() { }
        public ConvertException(string message) : base(message) { }
        public ConvertException(string message, Exception inner) : base(message, inner) { }
    }
}
