using Cocon90.Db.Common.Data;
using Cocon90.Db.Common.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Helper
{
    public class DataHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataHelper"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="NoNullAllowedException">driver can not be null.</exception>
        public DataHelper(BaseDriver driver) { this.Driver = driver; if (driver == null) throw new NoNullAllowedException("driver can not be null."); }
       
        /// <summary>
        /// Gets or sets the driver.
        /// </summary>
        /// <value>The driver.</value>
        public BaseDriver Driver { get; set; }

        /// <summary>
        /// Executes the batch. if success it will return a number greater than 0. otherwise it will threw a exception.
        /// </summary>
        /// <param name="sqlBatch">The SQL batch.</param>
        /// <param name="isCommit">if set to <c>true</c> [is commit].</param>
        /// <param name="allowThrowException">if set to <c>true</c> [allow throw exception].</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="Exceptions.SqlBatchExecuteException"></exception>
        public virtual int ExecBatch(IEnumerable<SqlBatch> sqlBatch, bool isCommit, bool allowThrowException = true)
        {
            int count = 0;
            var conn = this.Driver.CreateConnection();
            var trans = conn.BeginTransaction();
            using (conn)
            {
                try
                {
                    foreach (var sql in sqlBatch)
                    {
                        var cmd = conn.CreateCommand();
                        cmd.Transaction = trans;
                        cmd.CommandText = sql.Sql;
                        cmd.CommandType = CommandType.Text;
                        if (sql.Params != null)
                        {
                            foreach (var p in sql.Params)
                            {
                                cmd.Parameters.Add(this.Driver.CreateParameter(p.Name, p.Value));
                            }
                        }
                        cmd.ExecuteNonQuery();
                        count += 1;
                    }
                    if (isCommit) { trans.Commit(); } else { trans.Rollback(); }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    if (allowThrowException)
                    {
                        var sb = sqlBatch.ToList()[count];
                        var sql = string.Join("\r\n", sb.Sql + "|Argument:" + (sb.Params == null ? "" : string.Join(",", sb.Params.ToList().ConvertAll(p => p.Name + "=" + p.Value))));
                        throw new Exceptions.SqlBatchExecuteException(string.Format("An error occurred while executing the {0} SqlBatch: {1}.\r\n The exception is: {2}.", count + 1, sql, ex.Message), ex) { AllSqlBatch = sqlBatch, CurrentSqlBatch = sb };
                    }
                }
            }
            return count;

        }

        /// <summary>
        /// Gets the scalar.
        /// </summary>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>System.Object.</returns>
        public virtual object GetScalar(string tsqlParamed, params Params[] paramKeyAndValue)
        {
            using (var cmd = this.Driver.CreateCommand(tsqlParamed, CommandType.Text, paramKeyAndValue))
            {
                var obj = cmd.ExecuteScalar();
                return obj;
            }
        }

        /// <summary>
        /// Executes the no query. if success it will return a number equal or greater than 0. otherwise it will threw a exception.
        /// </summary>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>System.Int32.</returns>
        public virtual int ExecNoQuery(string tsqlParamed, params Params[] paramKeyAndValue)
        {
            using (var cmd = this.Driver.CreateCommand(tsqlParamed, CommandType.Text, paramKeyAndValue))
            {
                var obj = cmd.ExecuteNonQuery();
                return obj;
            }
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>DataSet.</returns>
        public virtual DataSet GetDataSet(string tsqlParamed, params Params[] paramKeyAndValue)
        {
            using (var dap = this.Driver.CreateAdapter(tsqlParamed, CommandType.Text, paramKeyAndValue))
            {
                DataSet ds = new DataSet();
                dap.Fill(ds);
                if (dap.SelectCommand != null && dap.SelectCommand.Connection != null && dap.SelectCommand.Connection.State == ConnectionState.Open)
                {
                    dap.SelectCommand.Connection.Close();
                    dap.SelectCommand.Connection.Dispose();
                }
                return ds;
            }
        }

    }
}
