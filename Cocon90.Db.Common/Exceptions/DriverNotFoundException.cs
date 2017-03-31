using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Exceptions
{

    [Serializable]
    public class DriverNotFoundException : Exception
    {
        public DriverNotFoundException() { }
        public DriverNotFoundException(string message) : base(message) { }
        public DriverNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
