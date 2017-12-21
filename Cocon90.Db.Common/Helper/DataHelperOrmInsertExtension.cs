using Cocon90.Db.Common;
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
    public static class DataHelperOrmInsertExtension
    {


        /// <summary>
        /// Inserts by the specified models.
        /// </summary>
        public static int Insert<T>(this IDataHelper dh, params T[] models)
        {
            var sqls = dh.GetInsertSql<T>(models);
            var successRows = dh.ExecBatch(sqls, true, true);
            return successRows;
        }
        /// <summary>
        /// Inserts or Replace into tables by the specified models.
        /// </summary>
        public static int Save<T>(this IDataHelper dh, params T[] models)
        {
            var sqls = dh.GetSaveSql<T>(models);
            var successRows = dh.ExecBatch(sqls, true, true);
            return successRows;
        }
        /// <summary>
        /// Inserts into tables if selectSqlCondition(such as 'select 1 from student where name=@name') has no record.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dh"></param>
        /// <param name="model"></param>
        /// <param name="selectSqlCondition"></param>
        /// <param name="paramKeyAndValue"></param>
        /// <returns></returns>
        public static int InsertIfNotExist<T>(this IDataHelper dh, T model, string selectSqlCondition, params Params[] paramKeyAndValue)
        {
            var sqls = dh.GetInsertIfNotExistSql<T>(model, selectSqlCondition, paramKeyAndValue: paramKeyAndValue);
            return dh.ExecBatch(new[] { sqls }, true, true);
        }
        /// <summary>
        /// Inserts into tables when selectSqlCondition(such as 'select 1 from student where name=@name') has no record.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dh"></param>
        /// <param name="model"></param>
        /// <param name="selectSqlCondition">such as 'select 1 from student where name=@name'</param>
        /// <param name="paramUsingModel"></param>
        /// <returns></returns>
        public static int InsertIfNotExist<T>(this IDataHelper dh, T model, string selectSqlCondition, object paramUsingModel)
        {
            var sqls = dh.GetInsertIfNotExistSql<T>(model, selectSqlCondition, paramUsingModel: paramUsingModel);
            return dh.ExecBatch(new[] { sqls }, true, true);
        }
        /// <summary>
        /// Inserts into tables when primary key value is not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dh"></param>
        /// <param name="model"></param>
        /// <param name="allParmaryKeysSortByColumnName"></param>
        /// <returns></returns>
        public static int InsertIfNotExistPrimeryKey<T>(this IDataHelper dh, T model, params object[] allParmaryKeysSortByColumnName)
        {
            var sqls = dh.GetInsertIfNotExistPrimeryKeySql<T>(model, allParmaryKeysSortByColumnName);
            return dh.ExecBatch(new[] { sqls }, true, true);
        }
    }
}
