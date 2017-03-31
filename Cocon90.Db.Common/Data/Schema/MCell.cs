using Cocon90.Db.Common.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Db.Common.Data.Schema
{
    /// <summary>
    /// Class MCell.
    /// </summary>
    [Serializable]
    [DataContract]
    public class MCell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MCell"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public MCell(MColumn column, object value)
        { this.Column = column; this.Value = value; }
        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>The column.</value>
        public MColumn Column { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }
        /// <summary>
        /// Gets the value of string.
        /// </summary>
        /// <value>The value of string.</value>
        public string ValueOfString() { return Value + ""; }
        /// <summary>
        /// Values the of int.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Int32.</returns>
        public int ValueOfInt(int defaultValue) { return ValueOf(defaultValue); }
        /// <summary>
        /// Values the of int.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int ValueOfInt() { return (int)ValueOf(typeof(int), true); }
        /// <summary>
        /// Values the of double.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Double.</returns>
        public double ValueOfDouble(double defaultValue) { return ValueOf(defaultValue); }
        /// <summary>
        /// Values the of double.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double ValueOfDouble() { return (double)ValueOf(typeof(double), true); }
        /// <summary>
        /// Values the of bool.
        /// </summary>
        public bool ValueOfBool(bool defaultValue) { return ValueOf(defaultValue); }
        /// <summary>
        /// Values the of bool.
        /// </summary>
        public bool ValueOfBool() { return (bool)ValueOf(typeof(bool), true); }
        /// <summary>
        /// Values the of targetType.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="isThrewException">if set to <c>true</c> [is threw exception].</param>
        /// <returns>System.Object.</returns>
        public object ValueOf(Type targetType, bool isThrewException = true)
        {
            try
            {
                return TypeConverter.To(targetType, Value);
            }
            catch (Exception ex) { if (isThrewException) throw ex; return null; }
        }
        /// <summary>
        /// Values the of type.
        /// </summary>
        public T ValueOf<T>(T defaultValue = default(T))
        {
            try
            {
                var value = (T)ValueOf(typeof(T), true);
                return value;
            }
            catch { return defaultValue; }
        }
        public override string ToString()
        {
            return ValueOfString();
        }
    }
}
