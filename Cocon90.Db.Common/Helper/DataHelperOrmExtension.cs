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
    public static class DataHelperOrmExtension
    {
        /// <summary>
        /// Executes the no query. but the parameter using model.
        /// </summary>
        public static int ExecNoQuery(this DataHelper dh, string tsqlParamed, object paramUsingModel)
        {
            List<Params> paramList = new List<Params>();
            if (paramUsingModel != null)
            {
                var columns = AttributeHelper.GetColumnNames(dh.Driver.DirverType, paramUsingModel.GetType());
                var props = ReflectHelper.GetPropertyValues(paramUsingModel.GetType(), paramUsingModel, true, false, false);
                foreach (var prop in props)
                {
                    paramList.Add(new Params(columns[prop.Key], prop.Value));
                }
            }
            return dh.ExecNoQuery(tsqlParamed, paramList.ToArray());
        }
        /// <summary>
        /// Gets the list.
        /// </summary>
        public static List<T> GetList<T>(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue) where T : new()
        {
            var tab = dh.GetTable(tsqlParamed, paramKeyAndValue);
            var lst = ModelHelper.GetList<T>(tab, dh.Driver.DirverType);
            return lst;
        }
        /// <summary>
        /// Gets the paged result.
        /// </summary>
        public static PagedResult<T> GetPagedResult<T>(this DataHelper dh, string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize, params Params[] paramKeyAndValue) where T : new()
        {
            PagedResult<T> pr = new PagedResult<T>();
            var page = dh.Driver.GetPagedSql(sourceSql, orderColumnName, isAsc, pageNumber, pageSize, paramKeyAndValue);
            if (page == null || page.PagedSql == null || page.CountSql == null) throw new NotImplementedException("the driver has not implatement the paging.");
            var tab = dh.GetTable(page.PagedSql.Sql, page.PagedSql.Params);
            var total = dh.GetInt(page.CountSql.Sql, page.CountSql.Params);
            var lst = ModelHelper.GetList<T>(tab, dh.Driver.DirverType);
            pr.Data = lst;
            pr.Total = total;
            pr.PageNumber = pageNumber;
            pr.PageSize = pageSize;
            return pr;
        }

        /// <summary>
        /// Gets the paged result.
        /// </summary>
        public static PagedResult GetPagedResult(this DataHelper dh, string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize, params Params[] paramKeyAndValue)
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
        /// Gets the list by where.
        /// </summary>
        public static List<T> GetListByWhere<T>(this DataHelper dh, string whereCondition, params Params[] paramKeyAndValue) where T : new()
        {
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var tsql = string.Format("SELECT * FROM {0} WHERE {1}", tableName, whereCondition);
            var lst = GetList<T>(dh, tsql, paramKeyAndValue);
            return lst;
        }

        /// <summary>
        /// Gets the one.
        /// </summary>
        public static T GetOne<T>(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue) where T : new()
        {
            var lst = GetList<T>(dh, tsqlParamed, paramKeyAndValue);
            if (lst == null || lst.Count == 0) return default(T);
            return lst.FirstOrDefault();
        }

        /// <summary>
        /// Gets the one by where.
        /// </summary>
        public static T GetOneByWhere<T>(this DataHelper dh, string whereCondition, params Params[] paramKeyAndValue) where T : new()
        {
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var tsql = string.Format("SELECT * FROM {0} WHERE {1}", tableName, whereCondition);
            var one = GetOne<T>(dh, tsql, paramKeyAndValue);
            return one;
        }

        /// <summary>
        /// Gets the one by primary key.
        /// </summary>
        public static T GetOneByPrimaryKey<T>(this DataHelper dh, object primaryKeyValue, params object[] otherParmaryKeysSortByColumnName) where T : new()
        {
            var type = typeof(T);
            var primaryKeys = AttributeHelper.GetPrimaryKeys(dh.Driver.DirverType, type);
            if (primaryKeys.Count == 0) throw new NoConfigurePrimaryKeyExceptionException("the class of '" + type.FullName + "' has no configure any primary key column.");
            primaryKeys.Sort((s1, s2) => string.Compare(s1, s2));
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);

            List<object> primaryKeyValues = new List<object>();
            primaryKeyValues.Add(primaryKeyValue ?? DBNull.Value);
            if (otherParmaryKeysSortByColumnName != null) primaryKeyValues.AddRange(otherParmaryKeysSortByColumnName.ToList().ConvertToAll(obj => obj ?? DBNull.Value));
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
        /// Inserts by the specified models.
        /// </summary>
        public static int Insert<T>(this DataHelper dh, params T[] models)
        {
            var sqls = GetInsertSql<T>(dh, models);
            var successRows = dh.ExecBatch(sqls, true, true);
            return successRows;
        }
        /// <summary>
        /// Inserts or Replace into tables by the specified models.
        /// </summary>
        public static int Save<T>(this DataHelper dh, params T[] models)
        {
            var sqls = GetSaveSql<T>(dh, models);
            var successRows = dh.ExecBatch(sqls, true, true);
            return successRows;
        }

        /// <summary>
        /// Gets the insert SQL.
        /// </summary>
        public static List<SqlBatch> GetInsertSql<T>(this DataHelper dh, params T[] models)
        {
            List<SqlBatch> sqls = new List<SqlBatch>();
            if (models == null || models.Length == 0) { return sqls; }
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var columnNameDic = AttributeHelper.GetColumnNames(dh.Driver.DirverType, type);
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
        /// Gets the insert or replace SQL.
        /// </summary>
        public static List<SqlBatch> GetSaveSql<T>(this DataHelper dh, params T[] models)
        {
            List<SqlBatch> sqls = new List<SqlBatch>();
            if (models == null || models.Length == 0) { return sqls; }
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var columnNameDic = AttributeHelper.GetColumnNames(dh.Driver.DirverType, type);
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
        /// Gets the update SQL.
        /// </summary>
        public static SqlBatch GetUpdateSql<T>(this DataHelper dh, T model, bool isNullMeansIgnore, string otherWhereCondition)
        {
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var columnNameDic = AttributeHelper.GetColumnNames(dh.Driver.DirverType, type);
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
        public static SqlBatch GetUpdateSqlByPrimaryKey<T>(this DataHelper dh, T model, bool isNullMeansIgnore, string otherWhereCondition, object primaryKeyValue, params object[] otherParmaryKeysSortByColumnName)
        {
            var type = typeof(T);
            var columnNameDic = AttributeHelper.GetColumnNames(dh.Driver.DirverType, type);
            var primaryKeyProps = AttributeHelper.GetPrimaryKeys(dh.Driver.DirverType, type, false);
            if (primaryKeyProps.Count <= 0)
                throw new NoConfigurePrimaryKeyExceptionException("Not config any primary key in model.");
            primaryKeyProps.Sort((s1, s2) => string.Compare(s1, s2));
            List<string> wherePropList = new List<string>();
            List<Params> paramList = new List<Params>();
            List<object> primaryValues = new List<object>();
            primaryValues.Add(primaryKeyValue);
            if (otherParmaryKeysSortByColumnName != null && otherParmaryKeysSortByColumnName.Length > 0)
                primaryValues.AddRange(otherParmaryKeysSortByColumnName);
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
            var sql = GetUpdateSqlByWhere(dh, model, isNullMeansIgnore, whereSql, paramList.ToArray());
            return sql;
        }

        /// <summary>
        /// Gets the update SQL by where.
        /// </summary>
        public static SqlBatch GetUpdateSqlByWhere<T>(this DataHelper dh, T model, bool isNullMeansIgnore, string whereCondition, params Params[] paramKeyAndValue)
        {
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var columnNameDic = AttributeHelper.GetColumnNames(dh.Driver.DirverType, type);
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
        /// Updates by the specified model.
        /// </summary>
        public static int Update<T>(this DataHelper dh, T model, bool isNullMeansIgnore, string otherWhereCondition)
        {
            var sql = GetUpdateSql<T>(dh, model, isNullMeansIgnore, otherWhereCondition);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }

        /// <summary>
        /// Updates table by primary key.
        public static int UpdateByPrimaryKey<T>(this DataHelper dh, T model, bool isNullMeansIgnore, string otherWhereCondition, object primaryKeyValue, params object[] otherParmaryKeysSortByColumnName)
        {
            var sql = GetUpdateSqlByPrimaryKey<T>(dh, model, isNullMeansIgnore, otherWhereCondition, primaryKeyValue, otherParmaryKeysSortByColumnName);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }

        /// <summary>
        /// Updates the by by where.
        /// </summary>
        public static int UpdateByByWhere<T>(this DataHelper dh, T model, bool isNullMeansIgnore, string whereCondition, params Params[] paramKeyAndValue)
        {
            var sql = GetUpdateSqlByWhere<T>(dh, model, isNullMeansIgnore, whereCondition, paramKeyAndValue);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }

        /// <summary>
        /// Gets the delete SQL by where.
        /// </summary>
        public static SqlBatch GetDeleteSqlByWhere<T>(this DataHelper dh, string whereCondition, params Params[] paramKeyAndValue)
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
        /// Gets the delete SQL by primary key.
        /// </summary>
        public static SqlBatch GetDeleteSqlByPrimaryKey<T>(this DataHelper dh, string otherWhereCondition, object primaryKeyValue, params object[] otherParmaryKeysSortByColumnName)
        {
            var type = typeof(T);
            var columnNameDic = AttributeHelper.GetColumnNames(dh.Driver.DirverType, type);
            var primaryKeyProps = AttributeHelper.GetPrimaryKeys(dh.Driver.DirverType, type, false);
            if (primaryKeyProps.Count <= 0)
                throw new NoConfigurePrimaryKeyExceptionException("Not config any primary key in model.");
            primaryKeyProps.Sort((s1, s2) => string.Compare(s1, s2));
            List<string> wherePropList = new List<string>();
            List<Params> paramList = new List<Params>();
            List<object> primaryValues = new List<object>();
            primaryValues.Add(primaryKeyValue);
            if (otherParmaryKeysSortByColumnName != null && otherParmaryKeysSortByColumnName.Length > 0)
                primaryValues.AddRange(otherParmaryKeysSortByColumnName);
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
        public static SqlBatch GetDeleteSql<T>(this DataHelper dh, T model, string otherWhereCondition, params Params[] paramKeyAndValue)
        {
            var type = typeof(T);
            var tableName = AttributeHelper.GetTableName(type, true, dh.Driver.SafeName);
            var columnNameDic = AttributeHelper.GetColumnNames(dh.Driver.DirverType, type);
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
        /// Deletes the specified model.
        /// </summary>
        public static int Delete<T>(this DataHelper dh, T model)
        {
            return Delete(dh, model, null, null);
        }

        /// <summary>
        /// Deletes table rows by the specified model.
        /// </summary>
        public static int Delete<T>(this DataHelper dh, T model, string otherWhereCondition, params Params[] paramKeyAndValue)
        {
            var sql = GetDeleteSql<T>(dh, model, otherWhereCondition, paramKeyAndValue);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }

        /// <summary>
        /// Deletes table rows the by primary key.
        /// </summary>
        public static int DeleteByPrimaryKey<T>(this DataHelper dh, string otherWhereCondition, object primaryKeyValue, params object[] otherParmaryKeysSortByColumnName)
        {
            var sql = GetDeleteSqlByPrimaryKey<T>(dh, otherWhereCondition, primaryKeyValue, otherParmaryKeysSortByColumnName);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }

        /// <summary>
        /// Deletes table rows the by where.
        /// </summary>
        public static int DeleteByWhere<T>(this DataHelper dh, string whereCondition, params Params[] paramKeyAndValue)
        {
            var sql = GetDeleteSqlByWhere<T>(dh, whereCondition, paramKeyAndValue);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }


    }
}
