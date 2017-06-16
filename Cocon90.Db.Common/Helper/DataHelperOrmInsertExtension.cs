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

    }
}
