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
        private static readonly object obj = new object();
        static volatile Dictionary<string, T> dictionary = new Dictionary<string, T>();
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
