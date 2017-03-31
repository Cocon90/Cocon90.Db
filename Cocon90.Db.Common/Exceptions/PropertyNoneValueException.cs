using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Exceptions
{

    [Serializable]
    public class PropertyNoneValueException : Exception
    {
        public object ModelObject { get; set; }
        public PropertyNoneValueException() { }
        public PropertyNoneValueException(string message) : base(message) { }
        public PropertyNoneValueException(string message, Exception inner) : base(message, inner) { }
    }
}
