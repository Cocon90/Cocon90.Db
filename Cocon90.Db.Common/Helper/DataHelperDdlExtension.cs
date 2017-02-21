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
    public static class DataHelperDdlExtension
    {
        /// <summary>
        /// Gets the create table SQL.
        /// </summary>
        public static SqlBatch GetCreateTableSql<T>(this DataHelper dh)
        {
            return GetCreateTableSql(dh, typeof(T));
        }
        /// <summary>
        /// Gets the create table SQL.
        /// </summary>
        public static SqlBatch GetCreateTableSql(this DataHelper dh, Type modelType)
        {
            List<SqlBatch> sqls = new List<SqlBatch>();
            var columnNameDic = AttributeHelper.GetColumnNames(dh.Driver.DirverType, modelType);
            var primaryKeyProps = AttributeHelper.GetPrimaryKeys(dh.Driver.DirverType, modelType, false);
            var createDdls = AttributeHelper.GetCreateDDLs(dh.Driver.DirverType, modelType, dh.Driver.TypeMapping);
            var tableName = AttributeHelper.GetTableName(modelType, false, null);
            Dictionary<string, string> columnDdls = new Dictionary<string, string>();
            foreach (var ddl in createDdls)
            {
                columnDdls.Add(columnNameDic[ddl.Key], ddl.Value);
            }
            var primaryKeyColumns = primaryKeyProps.ConvertAll(p => columnNameDic[p]);
            SqlBatch sql = dh.Driver.GetCreateTableSql(tableName, columnDdls, primaryKeyColumns);
            return sql;
        }
        /// <summary>
        /// Gets the update table schema SQL.
        /// </summary>
        public static SqlBatch GetUpdateTableSql(this DataHelper dh, Type modelType)
        {
            List<SqlBatch> sqls = new List<SqlBatch>();
            var columnNameDic = AttributeHelper.GetColumnNames(dh.Driver.DirverType, modelType);
            var primaryKeyProps = AttributeHelper.GetPrimaryKeys(dh.Driver.DirverType, modelType, false);
            var createDdls = AttributeHelper.GetCreateDDLs(dh.Driver.DirverType, modelType, dh.Driver.TypeMapping);
            var tableName = AttributeHelper.GetTableName(modelType, false, null);
            Dictionary<string, string> columnDdls = new Dictionary<string, string>();
            foreach (var ddl in createDdls)
            {
                columnDdls.Add(columnNameDic[ddl.Key], ddl.Value);
            }
            var primaryKeyColumns = primaryKeyProps.ConvertAll(p => columnNameDic[p]);
            SqlBatch sql = dh.Driver.GetUpdateTableSql(tableName, columnDdls, primaryKeyColumns);
            return sql;
        }
        /// <summary>
        /// Create or update table schema. if success it will return a number greater than 0. otherwise it will threw a exception.
        /// </summary>
        public static int CreateOrUpdateTable<T>(this DataHelper dh)
        {
            return CreateOrUpdateTable(dh, typeof(T));
        }
        /// <summary>
        /// Create or update table schema.if success it will return a number greater than 0. otherwise it will threw a exception.
        /// </summary>
        public static int CreateOrUpdateTable(this DataHelper dh, Type type)
        {
            var createSql = GetCreateTableSql(dh, type);
            var updateSql = GetUpdateTableSql(dh, type);
            List<SqlBatch> sqls = new List<SqlBatch>();
            if (createSql != null)
                sqls.Add(createSql);
            if (updateSql != null)
                sqls.Add(updateSql);
            var successRows = dh.ExecBatch(sqls, true, true);
            return successRows;
        }
    }
}
