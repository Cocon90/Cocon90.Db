using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Tools
{
    public class TypeConverter
    {
        public static object To(Type targetType, object value)
        {
            try
            {
                if (value == null) return null;
                if (targetType == typeof(string))
                    return value + "";
                if (targetType == typeof(long) || targetType == typeof(long?))
                    return long.Parse(value + "");
                if (targetType == typeof(double) || targetType == typeof(double?))
                    return double.Parse(value + "");
                if (targetType == typeof(Guid) || targetType == typeof(Guid?))
                    return Guid.Parse(value + "");
                if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                    return DateTime.Parse(value + "");
                if (targetType == typeof(DateTimeOffset) || targetType == typeof(DateTimeOffset?))
                    return DateTimeOffset.Parse(value + "");
                if (targetType == typeof(int) || targetType == typeof(int?))
                    return int.Parse(value + "");
                if (targetType == typeof(decimal) || targetType == typeof(decimal?))
                    return decimal.Parse(value + "");
                if (targetType == typeof(float) || targetType == typeof(float?))
                    return float.Parse(value + "");
                if (targetType == typeof(bool) || targetType == typeof(bool?))
                    return ToBool(value + "");
                if (targetType == typeof(char) || targetType == typeof(char?))
                    return char.Parse(value + "");
                if (targetType == typeof(byte) || targetType == typeof(byte?))
                    return byte.Parse(value + "");
                if (targetType == typeof(short) || targetType == typeof(short?))
                    return short.Parse(value + "");
                return value;
            }
            catch (Exception ex) { throw new Exceptions.ConvertException("Convert '" + value + "' to target type '" + targetType.FullName + "' error.", ex) { NeedConvertValue = value, TargetType = targetType }; }
        }
        public static bool ToBool(string boolString)
        {
            boolString = boolString + "";
            if (boolString.ToLower() == "on" || boolString.ToLower() == "1" || boolString.ToLower() == "true" || boolString.ToLower() == "yes" || boolString.ToLower() == "t") return true;
            else return false;
        }
    }
}
