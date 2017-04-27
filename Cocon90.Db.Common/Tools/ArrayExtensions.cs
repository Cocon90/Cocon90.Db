using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// 将传入的结果，转换为指定类型的List输出。
        /// </summary>
        public static List<TOutput> ConvertToAll<TInput, TOutput>(this IEnumerable<TInput> array, Func<TInput, TOutput> converter)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            var len = array.Count();
            List<TOutput> array2 = new List<TOutput>(len);
            for (int i = 0; i < len; i++)
            {
                array2.Add(converter(array.ElementAt(i)));
            }
            return array2;
        }
    }
}
