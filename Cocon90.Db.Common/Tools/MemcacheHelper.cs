using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Tools
{
    /// <summary>
    /// Cache helper.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MemcacheHelper<T>
    {
        static MemcacheHelper()
        {
            dictionary = new Dictionary<string, T>();
        }
        private static readonly object obj = new object();
        private volatile static Dictionary<string, T> dictionary = null;
        /// <summary>
        /// get or set data by key from cache
        /// </summary>
        public static T ReadAndWrite(string key, Func<T> fatchData)
        {
            lock (obj)
            {
                if (dictionary.ContainsKey(key))
                {
                    return dictionary[key];
                }
                return dictionary[key] = fatchData();
            }
        }
    }
}
