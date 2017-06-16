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
    public static class DataHelperOrmUpdateExtension
    {
        /// <summary>
        /// Updates by the specified model.
        /// </summary>
        public static int Update<T>(this IDataHelper dh, T model, bool isNullMeansIgnore = true, string otherWhereCondition = null)
        {
            var sql = dh.GetUpdateSql<T>( model, isNullMeansIgnore, otherWhereCondition);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }
        /// <summary>
        /// Updates by the specified models.
        /// </summary>
        public static int Update<T>(this IDataHelper dh, T[] model, bool isNullMeansIgnore = true, string otherWhereCondition = null)
        {
            if (model == null || model.Length == 0) return 0;
            List<SqlBatch> sqls = new List<SqlBatch>();
            foreach (var item in model)
            {
                var sql = dh.GetUpdateSql<T>(item, isNullMeansIgnore, otherWhereCondition);
                sqls.Add(sql);
            }
            var successRows = dh.ExecBatch(sqls.ToArray(), true, true);
            return successRows;
        }
        /// <summary>
        /// Updates table by primary key.
        public static int UpdateByPrimaryKey<T>(this IDataHelper dh, T model, bool isNullMeansIgnore, string otherWhereCondition, params object[] allParmaryKeysSortByColumnName)
        {
            var sql = dh.GetUpdateSqlByPrimaryKey<T>(model, isNullMeansIgnore, otherWhereCondition, allParmaryKeysSortByColumnName);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }

        /// <summary>
        /// Updates the by by where.
        /// </summary>
        public static int UpdateByByWhere<T>(this IDataHelper dh, T model, bool isNullMeansIgnore, string whereCondition, params Params[] paramKeyAndValue)
        {
            var sql = dh.GetUpdateSqlByWhere<T>(model, isNullMeansIgnore, whereCondition, paramKeyAndValue);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }

        /// <summary>
        /// Updates the by by where.
        /// </summary>
        public static int UpdateByByWhere<T>(this IDataHelper dh, T model, bool isNullMeansIgnore, string whereCondition, object paramUsingModel)
        {
            return UpdateByByWhere<T>(dh, model, isNullMeansIgnore, whereCondition, paramKeyAndValue: AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }
         
    }
}
