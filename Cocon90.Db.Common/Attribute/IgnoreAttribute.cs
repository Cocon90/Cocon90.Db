using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Attribute
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IgnoreAttribute : System.Attribute
    {
        public IgnoreAttribute(bool isIgnore = true)
        {
            this.IsIgnore = isIgnore;
        }
        public bool IsIgnore { get; set; }

    }
}
