using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Tools
{
    /// <summary>
    /// type converter.
    /// </summary>
    public class TypeConverter
    {
        /// <summary>
        /// convert value to targetType.
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object To(Type targetType, object value)
        {
            try
            {
                if (value == null || value is DBNull)
                    return null;

                if (targetType == value.GetType())
                    return value;

                if (targetType == typeof(string))
                    return Convert.ToString(value);

                if (targetType == typeof(int?) || targetType == typeof(int))
                    return Convert.ToInt32(value);

                if (targetType == typeof(Guid?) || targetType == typeof(Guid))
                    return Guid.Parse(Convert.ToString(value));

                if (targetType == typeof(DateTime?) || targetType == typeof(DateTime))
                    return Convert.ToDateTime(value);

                if (targetType == typeof(float?) || targetType == typeof(float))
                    return Convert.ToSingle(value);

                if (targetType == typeof(bool?) || targetType == typeof(bool))
                    return Convert.ToBoolean(value);

                if (targetType == typeof(long?) || targetType == typeof(long))
                    return Convert.ToInt64(value);

                if (targetType == typeof(double?) || targetType == typeof(double))
                    return Convert.ToDouble(value);

                if (targetType == typeof(DateTimeOffset?) || targetType == typeof(DateTimeOffset))
                    return DateTimeOffset.Parse(value + "");

                if (targetType == typeof(decimal?) || targetType == typeof(decimal))
                    return Convert.ToDecimal(value);

                if (targetType == typeof(char?) || targetType == typeof(char))
                    return Convert.ToChar(value);

                if (targetType == typeof(byte?) || targetType == typeof(byte))
                    return Convert.ToByte(value);

                if (targetType == typeof(short?) || targetType == typeof(short))
                    return Convert.ToInt16(value);

                if (targetType == typeof(byte[]) && value.GetType() == typeof(string))
                    return Convert.FromBase64String(value + "");
                return value;
            }
            catch (Exception ex) { throw new Exceptions.ConvertException("Convert '" + value + "' to target type '" + targetType.FullName + "' error.", ex) { NeedConvertValue = value, TargetType = targetType }; }
        }
    }
}
