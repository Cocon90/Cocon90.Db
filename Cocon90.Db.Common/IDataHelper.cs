using System.Collections.Generic;
using Cocon90.Db.Common.Data;
using Cocon90.Db.Common.Data.Schema;
using Cocon90.Db.Common.Driver;

namespace Cocon90.Db.Common
{
    /// <summary>
    /// Database connection operate helper.
    /// </summary>
    public interface IDataHelper
    {
        /// <summary>
        /// The base operate from database.
        /// </summary>
        BaseDriver Driver { get; set; }
        /// <summary>
        /// execute sqls use a transaction.
        /// </summary>
        /// <param name="sqlBatch">the sql with it's params.</param>
        /// <param name="isCommit">is commit on finish.</param>
        /// <param name="allowThrowException">is throw exception allowed.</param>
        /// <returns>it's will return the sqls count before error.</returns>
        int ExecBatch(IEnumerable<SqlBatch> sqlBatch, bool isCommit, bool allowThrowException = true);
        /// <summary>
        /// execute a sql return cmd.ExecuteNonQuery result.
        /// </summary>
        /// <returns></returns>
        int ExecNoQuery(string tsqlParamed, params Params[] paramKeyAndValue);
        /// <summary>
        /// get a datatable collections.
        /// </summary>
        MDataSet GetDataSet(string tsqlParamed, params Params[] paramKeyAndValue);
        /// <summary>
        /// get a object.
        /// </summary>
        object GetScalar(string tsqlParamed, params Params[] paramKeyAndValue);
    }
}