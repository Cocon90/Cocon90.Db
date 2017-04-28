using Cocon90.DynamicReflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cocon90.Db.Common.Tools
{
    public class ReflectHelper
    {
        /// <summary>
        /// Gets the property values. the key is PropertyName and the value is the parameter obj's value.
        /// </summary>
        public static Dictionary<string, object> GetPropertyValues(Type type, object obj, bool isWithNullValue, bool isOnlyCanReadWrite, bool isNotWithIgnoreProperty)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (obj == null) return dic;
            List<string> ignoreProps = null;
            if (isNotWithIgnoreProperty)
                ignoreProps = AttributeHelper.GetIgnorePropertys(type);
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(obj, null);
                if (isWithNullValue || value != null)
                {
                    if (!isOnlyCanReadWrite || (prop.CanRead && prop.CanWrite))
                    {
                        if (!isNotWithIgnoreProperty || !ignoreProps.Contains(prop.Name))
                        {
                            dic.Add(prop.Name, value);
                        }
                    }
                }
            }
            return dic;
        }
    }
}
