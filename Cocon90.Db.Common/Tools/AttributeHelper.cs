using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocon90.Db.Common.Data;

namespace Cocon90.Db.Common.Tools
{
    public class AttributeHelper
    {
        /// <summary>
        /// Gets the column names.the key is PropertyName and the value is ColumnName.
        /// </summary>
        public static Dictionary<string, string> GetColumnNames(Data.DirverType dirverType, Type modelType)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var props = modelType.GetProperties().ToList();
            foreach (var prop in props)
            {
                if (!prop.CanRead || !prop.CanWrite) continue;
                var attrs = prop.GetCustomAttributes(typeof(Common.Attribute.ColumnAttribute), true);
                if (attrs == null) { dic.Add(prop.Name, prop.Name); continue; }
                var lstAttrs = attrs.ToList().ConvertAll(a => (Common.Attribute.ColumnAttribute)a);
                var targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == dirverType);
                if (targetAttr == null) targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == Data.DirverType.UnKnown);
                if (targetAttr != null && !string.IsNullOrWhiteSpace(targetAttr.ColumnName)) dic.Add(prop.Name, targetAttr.ColumnName);
                else dic.Add(prop.Name, prop.Name);
            }
            return dic;
        }
        /// <summary>
        /// Gets the create ddl script. the key is PropertyName and the value is CreateDdl.
        /// </summary>
        public static Dictionary<string, string> GetCreateDDLs(DirverType dirverType, Type modelType, Func<Type, int, string> typeMappingFunc)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var props = modelType.GetProperties().ToList();
            var ignoreProperys = GetIgnorePropertys(modelType);
            foreach (var prop in props)
            {
                if (!prop.CanRead || !prop.CanWrite) continue;
                if (ignoreProperys.Contains(prop.Name)) continue;
                var attrs = prop.GetCustomAttributes(typeof(Common.Attribute.ColumnAttribute), true);
                if (attrs == null) { dic.Add(prop.Name, prop.Name); continue; }
                var lstAttrs = attrs.ToList().ConvertAll(a => (Common.Attribute.ColumnAttribute)a);
                var targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == dirverType);
                if (targetAttr == null) targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == Data.DirverType.UnKnown);
                if (targetAttr != null && !string.IsNullOrWhiteSpace(targetAttr.CreateDDL)) dic.Add(prop.Name, targetAttr.CreateDDL);
                else if (typeMappingFunc != null) dic.Add(prop.Name, typeMappingFunc(prop.PropertyType, 0));
            }
            return dic;
        }

        /// <summary>
        /// Gets the primary keys.
        /// </summary>
        public static List<string> GetPrimaryKeys(Data.DirverType dirverType, Type modelType, bool isColumnNameOrPropertyName = true)
        {
            List<string> lst = new List<string>();
            var props = modelType.GetProperties().ToList();
            foreach (var prop in props)
            {
                if (!prop.CanRead || !prop.CanWrite) continue;
                var attrs = prop.GetCustomAttributes(typeof(Common.Attribute.ColumnAttribute), true);
                if (attrs == null) continue;
                var lstAttrs = attrs.ToList().ConvertAll(a => (Common.Attribute.ColumnAttribute)a);
                var targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == dirverType);
                if (targetAttr == null) targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == Data.DirverType.UnKnown);
                if (targetAttr != null && targetAttr.PrimaryKey)
                { if (isColumnNameOrPropertyName && !string.IsNullOrWhiteSpace(targetAttr.ColumnName)) lst.Add(targetAttr.ColumnName); else lst.Add(prop.Name); }
            }
            return lst;
        }
        /// <summary>
        /// Gets the ignore columns but only with can read and can write property.
        /// </summary>
        public static List<string> GetIgnorePropertys(Type modelType)
        {
            List<string> lst = new List<string>();
            var props = modelType.GetProperties().ToList();
            foreach (var prop in props)
            {
                if (!prop.CanRead || !prop.CanWrite) continue;
                var attrs = prop.GetCustomAttributes(typeof(Common.Attribute.IgnoreAttribute), true);
                if (attrs == null) continue;
                var lstAttrs = attrs.ToList().ConvertAll(a => (Common.Attribute.IgnoreAttribute)a);
                var targetAttr = lstAttrs.FirstOrDefault();
                if (targetAttr != null && targetAttr.IsIgnore)
                    lst.Add(prop.Name);
            }
            return lst;
        }
        /// <summary>
        /// Gets the name of the table. with schemaName.tableName, if you set safeNameFunc it can return like `schemaName`.`tableName` on mysql.
        /// </summary>
        public static string GetTableName(Type modelType, bool isWithSchemaName, Func<string, string> safeNameFunc = null)
        {
            if (safeNameFunc == null) safeNameFunc = (name) => name;
            var tableAttr = modelType.GetCustomAttributes(typeof(Common.Attribute.TableAttribute), true);
            if (tableAttr == null || tableAttr.Length == 0)
                return safeNameFunc(modelType.Name);
            var tabAttr = tableAttr[0] as Common.Attribute.TableAttribute;
            if (!string.IsNullOrWhiteSpace(tabAttr.SchemaName) && isWithSchemaName)
                return string.Format("{0}.{1}", safeNameFunc(tabAttr.SchemaName), safeNameFunc(tabAttr.TableName));
            return safeNameFunc(tabAttr.TableName);
        }
    }
}
