using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Db.Common.Data
{
    [Serializable]
    [DataContract]
    public class Params
    {
        public Params() { }
        public Params(string name, object value)
        { this.Name = name; this.Value = value; }
        /// <summary>
        /// 参数键.请用@开头。
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 参数值.
        /// </summary>
        [DataMember]
        public object Value { get { return paramValue; } set { paramValue = value ?? DBNull.Value; } }
        private object paramValue;
        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Value);
        }
        /// <summary>
        /// 定义由DbParameter到Parameter的隐式转换
        /// </summary>
        /// <param name="dbParameter"></param>
        /// <returns></returns>
        public static implicit operator Params(System.Data.Common.DbParameter dbParameter)
        {
            return new Params(dbParameter.ParameterName, dbParameter.Value);
        }
    }
}
