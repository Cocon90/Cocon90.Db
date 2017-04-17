using Cocon90.Db.Common.Data;
using Cocon90.Db.Common.Data.Schema;
using Cocon90.Db.Common.Driver;
using Cocon90.Db.Common.Tools;
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
        public DataHelper(BaseDriver driver) { this.Driver = driver; if (driver == null) throw new ArgumentNullException("driver can not be null."); }

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
                        using (var cmd = conn.CreateCommand())
                        {
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
                    }
                    if (isCommit) { trans.Commit(); } else { trans.Rollback(); }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    if (allowThrowException)
                    {
                        var sb = sqlBatch.ToList()[count];
                        var sql = string.Join("\r\n", sb.Sql + "|Argument:" + (sb.Params == null ? "" : string.Join(",", sb.Params.ToList().ConvertToAll(p => p.Name + "=" + p.Value))));
                        throw new Exceptions.SqlBatchExecuteException(string.Format("An error occurred while executing the {0} SqlBatch: {1}.\r\n The exception is: {2}.", count + 1, sql, ex.Message), ex) { AllSqlBatch = sqlBatch, CurrentSqlBatch = sb };
                    }
                }
                if (conn.State != ConnectionState.Closed) conn.Close();
            }
            trans.Dispose();
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
                using (cmd.Connection)
                {
                    var obj = cmd.ExecuteScalar();
                    if (cmd.Connection.State != ConnectionState.Closed) cmd.Connection.Close();
                    return obj;
                }
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
                using (cmd.Connection)
                {
                    var obj = cmd.ExecuteNonQuery();
                    if (cmd.Connection.State != ConnectionState.Closed) cmd.Connection.Close();
                    return obj;
                }
            }
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>DataSet.</returns>
        public virtual MDataSet GetDataSet(string tsqlParamed, params Params[] paramKeyAndValue)
        {
            MDataSet ds = new MDataSet();
            using (var dr = this.Driver.CreateDataReader(tsqlParamed, CommandType.Text, CommandBehavior.CloseConnection, paramKeyAndValue))
            {
                List<MColumn> columns = new List<MColumn>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    var fieldType = dr.GetFieldType(i);
                    var caption = dr.GetName(i);
                    columns.Add(new MColumn(caption, fieldType));
                }
                MDataTable dt = new MDataTable(columns);
                while (dr.Read())
                {
                    List<MCell> cells = new List<MCell>();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        var value = dr[i];
                        cells.Add(new MCell(columns[i], value));
                    }
                    dt.AddRow(cells);
                }
                ds.Tables.Add(dt);
            }
            return ds;
        }

    }
}
