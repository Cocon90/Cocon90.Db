﻿using Cocon90.Db.Common;
using Cocon90.Db.Common.Data;
using Cocon90.Db.Common.Driver;
using Cocon90.Db.Common.Exceptions;
using Cocon90.Db.Common.Helper;
using Cocon90.Db.Common.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace System
{
    public static class DataHelperOrmGetExtension
    {

        /// <summary>
        /// Gets the list.
        /// </summary>
        public static List<T> GetList<T>(this IDataHelper dh) where T : new()
        {
            var tableName = AttributeHelper.GetTableName(typeof(T), true, dh.Driver.SafeName);
            var tsqlParamed = "SELECT * FROM " + tableName;
            return GetList<T>(dh, tsqlParamed);
        }
        /// <summary>
        /// Gets the list.
        /// </summary>
        public static List<T> GetList<T>(this IDataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue) where T : new()
        {
            Data.Common.DbDataReader dr = null;
            using (dr = dh.Driver.CreateDataReader(tsqlParamed, CommandType.Text, CommandBehavior.CloseConnection, paramKeyAndValue))
            {
                return ModelHelper.GetList<T>(dr, dh.Driver.DirverType);
            }
        }
        /// <summary>
        /// Gets the list.
        /// </summary>
        public static List<T> GetList<T>(this IDataHelper dh, string tsqlParamed, object paramUsingModel) where T : new()
        {
            return GetList<T>(dh, tsqlParamed, paramKeyAndValue: AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }
        /// <summary>
        /// Gets the paged result.
        /// </summary>
        public static PagedResult<T> GetPagedResult<T>(this IDataHelper dh, string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize, params Params[] paramKeyAndValue) where T : new()
        {
            PagedResult<T> pr = new PagedResult<T>();
            var page = dh.Driver.GetPagedSql(sourceSql, orderColumnName, isAsc, pageNumber, pageSize, paramKeyAndValue);
            if (page == null || page.PagedSql == null || page.CountSql == null) throw new NotImplementedException("the driver has not implatement the paging.");
            var lst = dh.GetList<T>(page.PagedSql.Sql, page.PagedSql.Params);
            var total = dh.GetInt(page.CountSql.Sql, page.CountSql.Params);
            pr.Data = lst;
            pr.Total = total;
            pr.PageNumber = pageNumber;
            pr.PageSize = pageSize;
            return pr;
        }
        /// <summary>
        /// Gets the paged result.
        /// </summary>
        public static PagedResult<T> GetPagedResult<T>(this IDataHelper dh, string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize, object paramUsingModel) where T : new()
        {
            return GetPagedResult<T>(dh, sourceSql, orderColumnName, isAsc, pageNumber, pageSize, AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }
        /// <summary>
        /// Gets the paged result.
        /// </summary>
        public static PagedResult GetPagedResult(this IDataHelper dh, string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize, params Params[] paramKeyAndValue)
        {
            PagedResult pr = new PagedResult();
            var page = dh.Driver.GetPagedSql(sourceSql, orderColumnName, isAsc, pageNumber, pageSize, paramKeyAndValue);
            if (page == null || page.PagedSql == null || page.CountSql == null) throw new NotImplementedException("the driver has not implatement the paging.");
            var tab = dh.GetTable(page.PagedSql.Sql, page.PagedSql.Params);
            var total = dh.GetInt(page.CountSql.Sql, page.CountSql.Params);
            pr.Data = tab;
            pr.Total = total;
            pr.PageNumber = pageNumber;
            pr.PageSize = pageSize;
            return pr;
        }
        /// <summary>
        /// Gets the paged result.
        /// </summary>
        public static PagedResult GetPagedResult(this IDataHelper dh, string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize, object paramUsingModel)
        {
            return GetPagedResult(dh, sourceSql, orderColumnName, isAsc, pageNumber, pageSize, AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }
        /// <summary>
        /// Gets the list by where.
        /// </summary>
        public static List<T> GetListByWhere<T>(this IDataHelper dh, string whereCondition, params Params[] paramKeyAndValue) where T : new()
        {
            if (string.IsNullOrWhiteSpace(whereCondition)) whereCondition = " 1=1 ";
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var tsql = string.Format("SELECT * FROM {0} WHERE {1}", tableName, whereCondition);
            var lst = GetList<T>(dh, tsql, paramKeyAndValue);
            return lst;
        }
        /// <summary>
        /// Gets the list by where.
        /// </summary>
        public static List<T> GetListByWhere<T>(this IDataHelper dh, string whereCondition, object paramUsingModel) where T : new()
        {
            return GetListByWhere<T>(dh, whereCondition, paramKeyAndValue: AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }

        /// <summary>
        /// Gets the one.
        /// </summary>
        public static T GetOne<T>(this IDataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue) where T : new()
        {
            var lst = GetList<T>(dh, tsqlParamed, paramKeyAndValue);
            if (lst == null || lst.Count == 0) return default(T);
            return lst.FirstOrDefault();
        }
        /// <summary>
        /// Gets the one.
        /// </summary>
        public static T GetOne<T>(this IDataHelper dh, string tsqlParamed, object paramUsingModel) where T : new()
        {
            return GetOne<T>(dh, tsqlParamed, paramKeyAndValue: AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }

        /// <summary>
        /// Gets the one by where.
        /// </summary>
        public static T GetOneByWhere<T>(this IDataHelper dh, string whereCondition, params Params[] paramKeyAndValue) where T : new()
        {
            if (string.IsNullOrWhiteSpace(whereCondition)) whereCondition = " 1=1 ";
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var tsql = string.Format("SELECT * FROM {0} WHERE {1}", tableName, whereCondition);
            var one = GetOne<T>(dh, tsql, paramKeyAndValue);
            return one;
        }
        /// <summary>
        /// Gets the one by where.
        /// </summary>
        public static T GetOneByWhere<T>(this IDataHelper dh, string whereCondition, object paramUsingModel) where T : new()
        {
            return GetOneByWhere<T>(dh, whereCondition, paramKeyAndValue: AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }
        /// <summary>
        /// Gets the one by primary key.
        /// </summary>
        public static T GetOneByPrimaryKey<T>(this IDataHelper dh, params object[] allParmaryKeysSortByColumnName) where T : new()
        {
            var type = typeof(T);
            var primaryKeys = AttributeHelper.GetPrimaryKeys(dh.Driver.DirverType, type);
            if (primaryKeys.Count == 0) throw new NoConfigurePrimaryKeyExceptionException("the class of '" + type.FullName + "' has no configure any primary key column.");
            primaryKeys.Sort((s1, s2) => string.Compare(s1, s2));
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);

            List<object> primaryKeyValues = new List<object>();
            if (allParmaryKeysSortByColumnName != null && allParmaryKeysSortByColumnName.Length > 0)
                primaryKeyValues.AddRange(allParmaryKeysSortByColumnName);
            if (primaryKeyValues.Count > primaryKeys.Count) throw new PrimaryKeyCountUnequalExceptionException("primary key and it's values count unequal.") { PrimaryKeys = primaryKeys.ToArray(), PrimaryKeyValues = primaryKeyValues.ToArray() };
            var conditionPrimaryKeys = new List<string>();
            List<Params> paraList = new List<Params>();
            for (int i = 0; i < primaryKeyValues.Count; i++)
            {
                var pk = primaryKeys[i];
                var pkValue = primaryKeyValues[i];
                if (pkValue == null || pkValue is DBNull)
                {
                    conditionPrimaryKeys.Add(string.Format("{0} IS NULL", dh.Driver.SafeName(pk)));
                }
                else
                {
                    conditionPrimaryKeys.Add(string.Format("{0}=@SysParam{1}", dh.Driver.SafeName(pk), pk));
                    paraList.Add(new Params("SysParam" + primaryKeys[i], primaryKeyValues[i]));
                }
            }
            var whereCondition = string.Join(" AND ", conditionPrimaryKeys);
            var tsql = string.Format("SELECT * FROM {0} WHERE {1}", tableName, whereCondition);

            var one = GetOne<T>(dh, tsql, paraList.ToArray());
            return one;
        }

        /// <summary>
        /// Gets the insert SQL.
        /// </summary>
        public static List<SqlBatch> GetInsertSql<T>(this IDataHelper dh, params T[] models)
        {
            List<SqlBatch> sqls = new List<SqlBatch>();
            if (models == null || models.Length == 0) { return sqls; }
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var columnNameDic = AttributeHelper.GetProp2ColumnNameDics(dh.Driver.DirverType, type);
            foreach (var md in models)
            {
                List<string> columnList = new List<string>();
                List<Params> param = new List<Params>();
                var modelValues = ReflectHelper.GetPropertyValues(type, md, false, true, true);
                foreach (var key in modelValues.Keys)
                {
                    columnList.Add(columnNameDic[key]);
                    param.Add(new Params(columnNameDic[key], modelValues[key]));
                }
                var columnString = string.Join(",", columnList.ConvertToAll(p => dh.Driver.SafeName(p)));
                var columnParamString = string.Join(",", columnList.ConvertToAll(p => "@" + p));
                var sql = string.Format("INSERT INTO {0}({1}) VALUES({2})", tableName, columnString, columnParamString);
                sqls.Add(new SqlBatch(sql, param.ToArray()));
            }
            return sqls;
        }

        /// <summary>
        /// Gets the insert or replace SQL. The Save method is not suitable for tables that contain the primary key in the auto-increment column.
        /// </summary>
        public static List<SqlBatch> GetSaveSql<T>(this IDataHelper dh, params T[] models)
        {
            List<SqlBatch> sqls = new List<SqlBatch>();
            if (models == null || models.Length == 0) { return sqls; }
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var columnNameDic = AttributeHelper.GetProp2ColumnNameDics(dh.Driver.DirverType, type);
            var primaryKeys = AttributeHelper.GetPrimaryKeys(dh.Driver.DirverType, type, true);
            foreach (var md in models)
            {
                List<string> columnList = new List<string>();
                List<Params> param = new List<Params>();
                var modelValues = ReflectHelper.GetPropertyValues(type, md, false, true, true);
                foreach (var key in modelValues.Keys)
                {
                    columnList.Add(columnNameDic[key]);
                    param.Add(new Params(columnNameDic[key], modelValues[key]));
                }
                var sql = dh.Driver.GetSaveSql(tableName, primaryKeys, columnList, param.ToArray());
                sqls.Add(sql);
            }
            return sqls;
        }


        /// <summary>
        /// Gets the Inserts into tables if selectSqlCondition(such as 'select 1 from student where name=@name') has no record SQL.
        /// </summary>
        public static SqlBatch GetInsertIfNotExistSql<T>(this IDataHelper dh, T model, string selectSqlCondition, object paramUsingModel)
        {
            return GetInsertIfNotExistSql<T>(dh, model, selectSqlCondition, paramKeyAndValue: AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }


        /// <summary>
        /// Gets the Inserts into tables if selectSqlCondition(such as 'select 1 from student where name=@name') has no record SQL.
        /// </summary>
        public static SqlBatch GetInsertIfNotExistSql<T>(this IDataHelper dh, T model, string selectSqlCondition, params Params[] paramKeyAndValue)
        {
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var columnNameDic = AttributeHelper.GetProp2ColumnNameDics(dh.Driver.DirverType, type);
            var insertColumnsValues = ReflectHelper.GetPropertyValues(type, model, false, true, true);
            Collections.Concurrent.ConcurrentDictionary<string, object> columnValueDic = new Collections.Concurrent.ConcurrentDictionary<string, object>();
            foreach (var item in insertColumnsValues)
            {
                if (columnNameDic.ContainsKey(item.Key))
                    columnValueDic.TryAdd(columnNameDic[item.Key], item.Value);
            }
            var insertColumns = columnValueDic.Keys.ToList();
            return dh.Driver.GetInsertIfNotExistSql(tableName, insertColumns, columnValueDic, selectSqlCondition, paramKeyAndValue);
        }

        /// <summary>
        /// Gets the Inserts into tables if selectSqlCondition(such as 'select 1 from student where name=@name') has no record SQL.
        /// </summary>
        public static SqlBatch GetInsertIfNotExistPrimeryKeySql<T>(this IDataHelper dh, T model, params object[] allParmaryKeysSortByColumnName)
        {
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var columnNameDic = AttributeHelper.GetProp2ColumnNameDics(dh.Driver.DirverType, type);
            var primaryKeyProps = AttributeHelper.GetPrimaryKeys(dh.Driver.DirverType, type, false);
            if (primaryKeyProps.Count <= 0)
                throw new NoConfigurePrimaryKeyExceptionException("Not config any primary key in model.");
            primaryKeyProps.Sort((s1, s2) => string.Compare(s1, s2));
            List<string> wherePropList = new List<string>();
            List<Params> paramList = new List<Params>();
            List<object> primaryValues = new List<object>();
            if (allParmaryKeysSortByColumnName != null && allParmaryKeysSortByColumnName.Length > 0)
                primaryValues.AddRange(allParmaryKeysSortByColumnName);

            for (int i = 0; i < Math.Min(primaryKeyProps.Count, primaryValues.Count); i++)
            {
                var key = primaryKeyProps[i];
                var columnName = columnNameDic[key];
                wherePropList.Add(string.Format("{0}=@SysWhere{1}", dh.Driver.SafeName(columnName), columnName));
                paramList.Add(new Params("SysWhere" + columnName, primaryValues[i]));
            }

            var whereSql = "SELECT 1 FROM  " + tableName + " WHERE " + string.Join(" AND ", wherePropList);
            return GetInsertIfNotExistSql(dh, model, whereSql, paramList.ToArray());
        }

        /// <summary>
        /// Gets the update SQL.
        /// </summary>
        public static SqlBatch GetUpdateSql<T>(this IDataHelper dh, T model, bool isNullMeansIgnore, string otherWhereCondition)
        {
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            //var columnNameDic = AttributeHelper.GetProp2ColumnNameDics(dh.Driver.DirverType, type);
            var modelValues = ReflectHelper.GetPropertyValues(type, model, !isNullMeansIgnore, true, true);
            var primaryKeyProps = AttributeHelper.GetPrimaryKeys(dh.Driver.DirverType, type, false);
            if (primaryKeyProps.Count <= 0)
            {
                throw new NoConfigurePrimaryKeyExceptionException("Not config any primary key in model.");
            }
            primaryKeyProps.Sort((s1, s2) => string.Compare(s1, s2));
            List<object> primaryKeyValues = new List<object>();
            for (int i = 0; i < primaryKeyProps.Count; i++)
            {
                if (modelValues.ContainsKey(primaryKeyProps[i]))
                {
                    var val = modelValues[primaryKeyProps[i]];
                    if (val != null) primaryKeyValues.Add(val);
                }
            }
            if (primaryKeyValues.Count <= 0) throw new PropertyNoneValueException("Primary key property must have a value.") { ModelObject = model };
            var sql = GetUpdateSqlByPrimaryKey(dh, model, isNullMeansIgnore, otherWhereCondition, primaryKeyValues.ToArray());
            return sql;
        }

        /// <summary>
        /// Gets the update SQL by primary key.
        /// </summary>
        public static SqlBatch GetUpdateSqlByPrimaryKey<T>(this IDataHelper dh, T model, bool isNullMeansIgnore, string otherWhereCondition, params object[] allParmaryKeysSortByColumnName)
        {
            var type = typeof(T);
            var columnNameDic = AttributeHelper.GetProp2ColumnNameDics(dh.Driver.DirverType, type);
            var primaryKeyProps = AttributeHelper.GetPrimaryKeys(dh.Driver.DirverType, type, false);
            if (primaryKeyProps.Count <= 0)
                throw new NoConfigurePrimaryKeyExceptionException("Not config any primary key in model.");
            primaryKeyProps.Sort((s1, s2) => string.Compare(s1, s2));
            List<string> wherePropList = new List<string>();
            List<Params> paramList = new List<Params>();
            List<object> primaryValues = new List<object>();
            if (allParmaryKeysSortByColumnName != null && allParmaryKeysSortByColumnName.Length > 0)
                primaryValues.AddRange(allParmaryKeysSortByColumnName);

            for (int i = 0; i < Math.Min(primaryKeyProps.Count, primaryValues.Count); i++)
            {
                var key = primaryKeyProps[i];
                var columnName = columnNameDic[key];
                wherePropList.Add(string.Format("{0}=@SysWhere{1}", dh.Driver.SafeName(columnName), columnName));
                paramList.Add(new Params("SysWhere" + columnName, primaryValues[i]));
            }
            if (!string.IsNullOrWhiteSpace(otherWhereCondition))
                wherePropList.Add(otherWhereCondition);
            var whereSql = string.Join(" AND ", wherePropList);
            foreach (var primaryKeyPropertyName in primaryKeyProps)
            {
                var propInfo = type.GetProperty(primaryKeyPropertyName);
                if (propInfo != null) propInfo.SetValue(model, null);
            }
            var sql = GetUpdateSqlByWhere(dh, model, isNullMeansIgnore, whereSql, paramList.ToArray());
            return sql;
        }

        /// <summary>
        /// Gets the update SQL by where.
        /// </summary>
        public static SqlBatch GetUpdateSqlByWhere<T>(this IDataHelper dh, T model, bool isNullMeansIgnore, string whereCondition, params Params[] paramKeyAndValue)
        {
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var columnNameDic = AttributeHelper.GetProp2ColumnNameDics(dh.Driver.DirverType, type);
            var modelValues = ReflectHelper.GetPropertyValues(type, model, !isNullMeansIgnore, true, true);
            List<string> upPropList = new List<string>();
            List<Params> paramList = new List<Params>();
            if (modelValues.Count == 0) { throw new PropertyNoneValueException() { ModelObject = model }; }
            foreach (var prop in modelValues)
            {
                var key = prop.Key;
                var columnName = columnNameDic[key];
                upPropList.Add(string.Format("{0}=@SysParam{1}", dh.Driver.SafeName(columnName), columnName));
                paramList.Add(new Params("SysParam" + columnName, prop.Value));
            }
            if (string.IsNullOrWhiteSpace(whereCondition))
                throw new ArgumentNullException("The parameter 'whereCondition' must have a value.");
            var updateSql = string.Join(",", upPropList);
            var sqlString = string.Format("UPDATE {0} SET {1} WHERE {2}", tableName, updateSql, whereCondition);
            if (paramKeyAndValue != null && paramKeyAndValue.Length > 0)
                paramList.AddRange(paramKeyAndValue);
            var sql = new SqlBatch(sqlString, paramList.ToArray());
            return sql;
        }
        /// <summary>
        /// Gets the update SQL by where.
        /// </summary>
        public static SqlBatch GetUpdateSqlByWhere<T>(this IDataHelper dh, T model, bool isNullMeansIgnore, string whereCondition, object paramUsingModel)
        {
            return GetUpdateSqlByWhere(dh, model, isNullMeansIgnore, whereCondition, paramKeyAndValue: AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }


        /// <summary>
        /// Gets the delete SQL by where.
        /// </summary>
        public static SqlBatch GetDeleteSqlByWhere<T>(this IDataHelper dh, string whereCondition, params Params[] paramKeyAndValue)
        {
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            List<Params> paramList = new List<Params>();
            if (string.IsNullOrWhiteSpace(whereCondition))
                throw new ArgumentNullException("The parameter 'whereCondition' must have a value.");
            var sqlString = string.Format("DELETE FROM {0} WHERE {1}", tableName, whereCondition);
            if (paramKeyAndValue != null && paramKeyAndValue.Length > 0)
                paramList.AddRange(paramKeyAndValue);
            var sql = new SqlBatch(sqlString, paramList.ToArray());
            return sql;
        }
        /// <summary>
        /// Gets the delete SQL by where.
        /// </summary>
        public static SqlBatch GetDeleteSqlByWhere<T>(this IDataHelper dh, string whereCondition, object paramUsingModel)
        {
            return GetDeleteSqlByWhere<T>(dh, whereCondition, paramKeyAndValue: AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }
        /// <summary>
        /// Gets the delete SQL by primary key.
        /// </summary>
        public static SqlBatch GetDeleteSqlByPrimaryKey<T>(this IDataHelper dh, string otherWhereCondition, params object[] allParmaryKeysSortByColumnName)
        {
            var type = typeof(T);
            var columnNameDic = AttributeHelper.GetProp2ColumnNameDics(dh.Driver.DirverType, type);
            var primaryKeyProps = AttributeHelper.GetPrimaryKeys(dh.Driver.DirverType, type, false);
            if (primaryKeyProps.Count <= 0)
                throw new NoConfigurePrimaryKeyExceptionException("Not config any primary key in model.");
            primaryKeyProps.Sort((s1, s2) => string.Compare(s1, s2));
            List<string> wherePropList = new List<string>();
            List<Params> paramList = new List<Params>();
            List<object> primaryValues = new List<object>();
            if (allParmaryKeysSortByColumnName != null && allParmaryKeysSortByColumnName.Length > 0)
                primaryValues.AddRange(allParmaryKeysSortByColumnName);
            for (int i = 0; i < Math.Min(primaryKeyProps.Count, primaryValues.Count); i++)
            {
                var key = primaryKeyProps[i];
                var columnName = columnNameDic[key];
                wherePropList.Add(string.Format("{0}=@SysWhere{1}", dh.Driver.SafeName(columnName), columnName));
                paramList.Add(new Params("SysWhere" + columnName, primaryValues[i]));
            }
            if (!string.IsNullOrWhiteSpace(otherWhereCondition))
                wherePropList.Add(otherWhereCondition);
            var whereSql = string.Join(" AND ", wherePropList);
            var sql = GetDeleteSqlByWhere<T>(dh, whereSql, paramList.ToArray());
            return sql;
        }

        /// <summary>
        /// Gets the delete SQL.
        /// </summary>
        public static SqlBatch GetDeleteSql<T>(this IDataHelper dh, T model, string otherWhereCondition, params Params[] paramKeyAndValue)
        {
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var columnNameDic = AttributeHelper.GetProp2ColumnNameDics(dh.Driver.DirverType, type);
            var modelValues = ReflectHelper.GetPropertyValues(type, model, false, true, true);

            List<string> conditionList = new List<string>();
            List<Params> paramList = new List<Params>();

            foreach (var mod in modelValues)
            {
                var columnName = columnNameDic[mod.Key];
                var columnValue = mod.Value;
                if (columnValue == null) continue;
                conditionList.Add(string.Format("{0}=@SysWhere{1}", dh.Driver.SafeName(columnName), columnName));
                paramList.Add(new Params("SysWhere" + columnName, columnValue));
            }
            if (!string.IsNullOrWhiteSpace(otherWhereCondition))
                conditionList.Add(otherWhereCondition);
            if (paramKeyAndValue != null && paramKeyAndValue.Length > 0)
                paramList.AddRange(paramKeyAndValue);
            if (conditionList.Count <= 0)
                throw new PropertyNoneValueException("model's one property or the parameter 'otherWhereCondition' must have a value at least.") { ModelObject = model };
            var whereString = string.Join(" AND ", conditionList);
            var sql = GetDeleteSqlByWhere<T>(dh, whereString, paramList.ToArray());
            return sql;
        }
        /// <summary>
        /// Gets the delete SQL.
        /// </summary>
        public static SqlBatch GetDeleteSql<T>(this IDataHelper dh, T model, string otherWhereCondition, object paramUsingModel)
        {
            return GetDeleteSql<T>(dh, model, otherWhereCondition, paramKeyAndValue: AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }


    }
}
