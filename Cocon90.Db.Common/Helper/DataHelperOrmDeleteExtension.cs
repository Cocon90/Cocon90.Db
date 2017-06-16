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
    public static class DataHelperOrmExtension
    {

        /// <summary>
        /// Deletes the specified model.
        /// </summary>
        public static int Delete<T>(this IDataHelper dh, T model)
        {
            return dh.Delete(model, null, null);
        }

        /// <summary>
        /// Deletes table rows by the specified model.
        /// </summary>
        public static int Delete<T>(this IDataHelper dh, T model, string otherWhereCondition, params Params[] paramKeyAndValue)
        {
            var sql = dh.GetDeleteSql<T>(model, otherWhereCondition, paramKeyAndValue);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }

        /// <summary>
        /// Deletes table rows by the specified model.
        /// </summary>
        public static int Delete<T>(this IDataHelper dh, T model, string otherWhereCondition, object paramUsingModel)
        {
            return Delete<T>(dh, model, otherWhereCondition, paramKeyAndValue: AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }

        /// <summary>
        /// Deletes table rows the by primary key.
        /// </summary>
        public static int DeleteByPrimaryKey<T>(this IDataHelper dh, string otherWhereCondition, params object[] allParmaryKeysSortByColumnName)
        {
            var sql = dh.GetDeleteSqlByPrimaryKey<T>(otherWhereCondition, allParmaryKeysSortByColumnName);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }

        /// <summary>
        /// Deletes table rows the by where.
        /// </summary>
        public static int DeleteByWhere<T>(this IDataHelper dh, string whereCondition, params Params[] paramKeyAndValue)
        {
            var sql = dh.GetDeleteSqlByWhere<T>(whereCondition, paramKeyAndValue);
            var successRows = dh.ExecBatch(new[] { sql }, true, true);
            return successRows;
        }

        /// <summary>
        /// Deletes table rows the by where.
        /// </summary>
        public static int DeleteByWhere<T>(this IDataHelper dh, string whereCondition, object paramUsingModel)
        {
            return DeleteByWhere<T>(dh, whereCondition, paramKeyAndValue: AttributeHelper.GetParamsArrayByModel(dh, paramUsingModel));
        }
    }
}
