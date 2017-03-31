using Cocon90.Db.Common.Data.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cocon90.Db.Common.Tools
{
    public class ModelHelper
    {
        public static List<T> GetList<T>(MDataTable table, Data.DirverType attrDirverType = Data.DirverType.UnKnown) where T : new()
        {
            List<T> result;
            if (table == null)
            {
                result = new List<T>();
            }
            else
            {
                List<T> list = new List<T>();
                Type typeFromHandle = typeof(T);
                var attrDic = AttributeHelper.GetColumnNames(attrDirverType, typeFromHandle);
                PropertyInfo[] properties = typeFromHandle.GetProperties();
                foreach (var dataRow in table.Rows)
                {
                    T t = (T)((object)Activator.CreateInstance(typeFromHandle));
                    PropertyInfo[] array = properties;
                    for (int i = 0; i < array.Length; i++)
                    {
                        PropertyInfo propertyInfo = array[i];
                        var columnName = propertyInfo.Name;
                        if (attrDic.ContainsKey(columnName)) columnName = attrDic[columnName];
                        if (table.ContainsColumn(columnName))
                        {
                            if (propertyInfo.CanWrite)
                            {
                                var cellValue = dataRow[columnName].Value;
                                if (cellValue is DBNull || cellValue == null)
                                {
                                    propertyInfo.SetValue(t, null, null);
                                }
                                else
                                {
                                    var obj = TypeConverter.To(propertyInfo.PropertyType, cellValue);
                                    propertyInfo.SetValue(t, obj, null);
                                }
                            }
                        }
                    }
                    list.Add(t);
                }
                result = list;
            }
            return result ?? new List<T>();
        }
        public static MDataTable GetDataTable<T>(IList<T> list)
        {
            //检查实体集合不能为空
            if (list == null || list.Count < 1)
            {
                return new MDataTable();
            }

            //取出第一个实体的所有Propertie
            Type entityType = list[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            var dt = new MDataTable();
            foreach (PropertyInfo t in entityProperties)
            {
                object value = t.GetValue(list[0], null);
                if (value == null) { dt.Columns.Add(new MColumn(t.Name, typeof(string))); }
                else { dt.Columns.Add(new MColumn(t.Name, value.GetType())); }
            }
            //将所有entity添加到DataTable中
            foreach (object entity in list)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    continue;
                }
                var entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);
                }
                var row = new MRow(dt.Columns);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    row.AddCell(dt.Columns[i], entityValues[i]);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
