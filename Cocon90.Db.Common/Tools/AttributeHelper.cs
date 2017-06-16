using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocon90.Db.Common.Data;
using System.Reflection;
using Cocon90.Db.Common.Helper;
using System.Collections.Concurrent;

namespace Cocon90.Db.Common.Tools
{
    /// <summary>
    /// attribute helper.
    /// </summary>
    public class AttributeHelper
    {
        /// <summary>
        /// Gets the column names.the key is PropertyName and the value is ColumnName.
        /// </summary>
        public static ConcurrentDictionary<string, string> GetProp2ColumnNameDics(Data.DirverType dirverType, Type modelType)
        {
            return MemcacheHelper<ConcurrentDictionary<string, string>>.ReadAndWrite("[GetProp2ColumnNameDics][" + modelType.FullName + "][" + dirverType + "]", () =>
            {
                ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();
                var props = modelType.GetProperties().ToList();
                foreach (var prop in props)
                {
                    if (!prop.CanRead || !prop.CanWrite) continue;
                    var attrs = prop.GetCustomAttributes(typeof(Common.Attribute.ColumnAttribute), true);
                    if (attrs == null) { dic.TryAdd(prop.Name, prop.Name); continue; }
                    var lstAttrs = attrs.ToList().Select(a => (Common.Attribute.ColumnAttribute)a);
                    var targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == dirverType);
                    if (targetAttr == null) targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == Data.DirverType.UnKnown);
                    if (targetAttr != null && !string.IsNullOrWhiteSpace(targetAttr.ColumnName)) dic[prop.Name] = targetAttr.ColumnName;
                    else dic[prop.Name] = prop.Name;
                }
                return dic;
            });
        }
        /// <summary>
        /// Gets the index names.the key is PropertyName and the value is IndexName.
        /// </summary>
        public static ConcurrentDictionary<string, string> GetColumn2IndexNameDics(Data.DirverType dirverType, Type modelType)
        {
            return MemcacheHelper<ConcurrentDictionary<string, string>>.ReadAndWrite("[GetColumn2IndexNameDics][" + modelType.FullName + "][" + dirverType + "]", () =>
            {
                ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();
                var props = modelType.GetProperties().ToList();
                foreach (var prop in props)
                {
                    if (!prop.CanRead || !prop.CanWrite) continue;
                    var attrs = prop.GetCustomAttributes(typeof(Common.Attribute.ColumnAttribute), true);
                    if (attrs == null) { continue; }
                    var lstAttrs = attrs.ToList().Select(a => (Common.Attribute.ColumnAttribute)a);
                    var targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == dirverType);
                    if (targetAttr == null) targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == Data.DirverType.UnKnown);
                    if (targetAttr != null && !string.IsNullOrWhiteSpace(targetAttr.IndexName))
                    {
                        var colName = prop.Name;
                        if (!string.IsNullOrWhiteSpace(targetAttr.ColumnName))
                            colName = targetAttr.ColumnName;
                        dic[colName] = targetAttr.IndexName;
                    }
                }
                return dic;
            });
        }
        /// <summary>
        /// Gets the column names.the key is ColumnName and the value is PropertyName.
        /// </summary>
        public static ConcurrentDictionary<string, string> GetColumn2PropNameDics(Data.DirverType dirverType, Type modelType)
        {
            return MemcacheHelper<ConcurrentDictionary<string, string>>.ReadAndWrite("[GetColumn2PropNameDics][" + modelType.FullName + "][" + dirverType + "]", () =>
            {
                ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();
                var props = modelType.GetProperties().ToList();
                foreach (var prop in props)
                {
                    if (!prop.CanRead || !prop.CanWrite) continue;
                    var attrs = prop.GetCustomAttributes(typeof(Common.Attribute.ColumnAttribute), true);
                    if (attrs == null) { dic.TryAdd(prop.Name, prop.Name); continue; }
                    var lstAttrs = attrs.ToList().Select(a => (Common.Attribute.ColumnAttribute)a);
                    var targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == dirverType);
                    if (targetAttr == null) targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == Data.DirverType.UnKnown);
                    if (targetAttr != null && !string.IsNullOrWhiteSpace(targetAttr.ColumnName)) dic[targetAttr.ColumnName] = prop.Name;
                    else dic[prop.Name] = prop.Name;
                }
                return dic;
            });
        }
        /// <summary>
        /// Gets the create ddl script. the key is PropertyName and the value is CreateDdl.
        /// </summary>
        public static ConcurrentDictionary<string, string> GetPropertyName2DDLs(DirverType dirverType, Type modelType, Func<Type, int, string> typeMappingFunc)
        {
            return MemcacheHelper<ConcurrentDictionary<string, string>>.ReadAndWrite("[GetPropertyName2DDLs][" + modelType.FullName + "][" + dirverType + "]", () =>
            {
                ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();
                var props = modelType.GetProperties().ToList();
                var ignoreProperys = GetIgnorePropertys(modelType);
                foreach (var prop in props)
                {
                    if (!prop.CanRead || !prop.CanWrite) continue;
                    if (ignoreProperys.Contains(prop.Name)) continue;
                    var attrs = prop.GetCustomAttributes(typeof(Common.Attribute.ColumnAttribute), true);
                    if (attrs == null) { dic.TryAdd(prop.Name, prop.Name); continue; }
                    var lstAttrs = attrs.ToList().ConvertToAll(a => (Common.Attribute.ColumnAttribute)a);
                    var targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == dirverType);
                    if (targetAttr == null) targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == Data.DirverType.UnKnown);
                    if (targetAttr != null && !string.IsNullOrWhiteSpace(targetAttr.CreateDDL)) dic.TryAdd(prop.Name, targetAttr.CreateDDL);
                    else if (typeMappingFunc != null) dic.TryAdd(prop.Name, typeMappingFunc(prop.PropertyType, 0));
                }
                return dic;
            });
        }

        /// <summary>
        /// Gets the primary keys.
        /// </summary>
        public static List<string> GetPrimaryKeys(Data.DirverType dirverType, Type modelType, bool isColumnNameOrPropertyName = true)
        {
            return MemcacheHelper<List<string>>.ReadAndWrite("[GetPrimaryKeys][" + modelType.FullName + "][" + dirverType + "][" + isColumnNameOrPropertyName + "]", () =>
            {
                List<string> lst = new List<string>();
                var props = modelType.GetProperties().ToList();
                foreach (var prop in props)
                {
                    if (!prop.CanRead || !prop.CanWrite) continue;
                    var attrs = prop.GetCustomAttributes(typeof(Common.Attribute.ColumnAttribute), true);
                    if (attrs == null) continue;
                    var lstAttrs = attrs.ToList().ConvertToAll(a => (Common.Attribute.ColumnAttribute)a);
                    var targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == dirverType);
                    if (targetAttr == null) targetAttr = lstAttrs.FirstOrDefault(a => a.DirverType == Data.DirverType.UnKnown);
                    if (targetAttr != null && targetAttr.PrimaryKey)
                    { if (isColumnNameOrPropertyName && !string.IsNullOrWhiteSpace(targetAttr.ColumnName)) lst.Add(targetAttr.ColumnName); else lst.Add(prop.Name); }
                }
                return lst;
            });
        }
        /// <summary>
        /// Gets the ignore columns but only with can read and can write property.
        /// </summary>
        public static List<string> GetIgnorePropertys(Type modelType)
        {
            return MemcacheHelper<List<string>>.ReadAndWrite("[GetIgnorePropertys][" + modelType.FullName + "]", () =>
            {
                List<string> lst = new List<string>();
                var props = modelType.GetProperties().ToList();
                foreach (var prop in props)
                {
                    if (!prop.CanRead || !prop.CanWrite) continue;
                    var attrs = prop.GetCustomAttributes(typeof(Common.Attribute.IgnoreAttribute), true);
                    if (attrs == null) continue;
                    var lstAttrs = attrs.ToList().ConvertToAll(a => (Common.Attribute.IgnoreAttribute)a);
                    var targetAttr = lstAttrs.FirstOrDefault();
                    if (targetAttr != null && targetAttr.IsIgnore)
                        lst.Add(prop.Name);
                }
                return lst;
            });
        }
        /// <summary>
        /// Gets the name of the table. with schemaName.tableName, if you set safeNameFunc it can return like `schemaName`.`tableName` on mysql.
        /// </summary>
        public static string GetTableName(Type modelType, bool isWithSchemaName, Func<string, string> safeNameFunc = null)
        {
            if (safeNameFunc == null) safeNameFunc = (name) => name;
            var tup = MemcacheHelper<Attribute.TableAttribute>.ReadAndWrite("[GetTableName][" + modelType.FullName + "][" + isWithSchemaName + "]", () =>
            {
#if NETSTANDARD
            List<System.Attribute> tableAttrList = new List<System.Attribute>();
            var attrs = modelType.GetTypeInfo().GetCustomAttributes(typeof(Common.Attribute.TableAttribute), true);
            if (attrs.Count() > 0)
                tableAttrList.AddRange(attrs);
            System.Attribute[] tableAttr = tableAttrList.ToArray();
#elif NETFRAMEWORK
                var tableAttr = modelType.GetCustomAttributes(typeof(Common.Attribute.TableAttribute), true);
#endif
                if (tableAttr == null || tableAttr.Length == 0)
                    return new Attribute.TableAttribute() { TableName = modelType.Name };
                var tabAttr = tableAttr[0] as Common.Attribute.TableAttribute;
                if (string.IsNullOrWhiteSpace(tabAttr.TableName)) tabAttr.TableName = modelType.Name;
                if (isWithSchemaName && !string.IsNullOrWhiteSpace(tabAttr.SchemaName))
                    return new Attribute.TableAttribute() { SchemaName = tabAttr.SchemaName, TableName = tabAttr.TableName };
                return new Attribute.TableAttribute { TableName = tabAttr.TableName };
            });
            if (tup.SchemaName == null) return safeNameFunc(tup.TableName);
            else return string.Format("{0}.{1}", safeNameFunc(tup.SchemaName), safeNameFunc(tup.TableName));
        }


        /// <summary>
        /// Get the parameters by a model object.
        /// </summary>
        /// <param name="dh"></param>
        /// <param name="paramUsingModel"></param>
        /// <returns></returns>
        public static Params[] GetParamsArrayByModel(IDataHelper dh, object paramUsingModel)
        {
            List<Params> paramList = new List<Params>();
            if (paramUsingModel == null) return paramList.ToArray();
            var paramUsingModelType = paramUsingModel.GetType();
            var props = ReflectHelper.GetPropertyValues(paramUsingModelType, paramUsingModel, false, false, false);
            foreach (var prop in props)
            {
                paramList.Add(new Params(prop.Key, prop.Value));
            }
            return paramList.ToArray();
        }
    }
}
