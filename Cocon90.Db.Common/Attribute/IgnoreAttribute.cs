using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Attribute
{
    /// <summary>
    /// If set to True, the current column is ignored automatically
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IgnoreAttribute : System.Attribute
    {
        /// <summary>
        /// If set to True, the current column is ignored automatically
        /// </summary>
        /// <param name="isIgnore"></param>
        public IgnoreAttribute(bool isIgnore = true)
        {
            this.IsIgnore = isIgnore;
        }
        /// <summary>
        /// If set to True, the current column is ignored automatically
        /// </summary>
        public bool IsIgnore { get; set; }

    }
}
