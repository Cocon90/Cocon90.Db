using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Cocon90.Db.Common.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace Cocon90.Db.Mysql
{
    public class DbDriver : Common.Driver.BaseDriver
    {
        public DbDriver(string connectionString) : base(connectionString)
        {
        }

        public override DirverType DirverType
        {
            get
            {
                return DirverType.MySql;
            }
        }
        public override DbDataReader CreateDataReader(string tsqlParamed, CommandType commandType = CommandType.Text, CommandBehavior behavior = CommandBehavior.Default, params Params[] param)
        {
            var cmd = (MySqlCommand)CreateCommand(tsqlParamed, commandType, param);
            var reader = cmd.ExecuteReader(behavior);
            return reader;
        }

        public override DbCommand CreateCommand(string tsqlParamed, CommandType commandType, params Params[] param)
        {
            var conn = (MySqlConnection)this.CreateConnection();
            var cmd = new MySqlCommand(tsqlParamed, conn);
            cmd.CommandType = commandType;
            if (param != null)
            {
                foreach (var p in param)
                {
                    cmd.Parameters.Add(this.CreateParameter(p.Name, p.Value));
                }
            }
            return cmd;
        }

        public override DbConnection CreateConnection()
        {
            MySqlConnectionStringBuilder connString = new MySqlConnectionStringBuilder(this.ConnectionString);
            connString.AllowUserVariables = true;
            MySqlConnection conn = new MySqlConnection(connString.ConnectionString);
            conn.Open();
            return conn;
        }

        public override DbParameter CreateParameter(string key, object value)
        {
            object objValue = value;
            if (value == null)
            {
                objValue = DBNull.Value;
            }
            var keyStr = key.Replace('@', this.ParameterChar);
            if (!keyStr.Contains(this.ParameterChar)) { keyStr = this.ParameterChar + keyStr.Trim(); }
            MySqlParameter param = new MySqlParameter(keyStr, objValue);
            return param;
        }

        public override char ParameterChar
        {
            get { return '?'; }
        }

        public override string SafeName(string columnNameOrTableName)
        {
            return string.Format("`{0}`", columnNameOrTableName);
        }

        public override string TypeMapping(Type csType, int len = 0)
        {
            Dictionary<string, string> instence = new Dictionary<string, string>();
            instence.Add(typeof(int).FullName, "int");
            instence.Add(typeof(bool).FullName, "bit");
            instence.Add(typeof(char).FullName, "varchar(2)");
            instence.Add(typeof(byte).FullName, "tinyint");
            instence.Add(typeof(short).FullName, "smallint");
            instence.Add(typeof(long).FullName, "bigint");
            instence.Add(typeof(float).FullName, "float");
            instence.Add(typeof(double).FullName, "decimal");
            instence.Add(typeof(decimal).FullName, "decimal");
            instence.Add(typeof(string).FullName, "longtext");
            instence.Add(typeof(byte[]).FullName, "longblob");
            instence.Add(typeof(DateTime).FullName, "datetime");
            instence.Add(typeof(DateTimeOffset).FullName, "longtext");
            instence.Add(typeof(Guid).FullName, "varchar(36)");

            instence.Add(typeof(int?).FullName, "int");
            instence.Add(typeof(bool?).FullName, "bit");
            instence.Add(typeof(char?).FullName, "varchar(2)");
            instence.Add(typeof(byte?).FullName, "tinyint");
            instence.Add(typeof(short?).FullName, "smallint");
            instence.Add(typeof(long?).FullName, "bigint");
            instence.Add(typeof(float?).FullName, "float");
            instence.Add(typeof(double?).FullName, "double");
            instence.Add(typeof(decimal?).FullName, "double");
            instence.Add(typeof(DateTime?).FullName, "datetime");
            instence.Add(typeof(DateTimeOffset?).FullName, "longtext");
            instence.Add(typeof(Guid?).FullName, "varchar(36)");

            //返回相应类型。
            foreach (var key in instence.Keys)
            {
                if (key == csType.FullName)
                {
                    return instence[key];
                }
            }
            return "longtext";
        }

        public override SqlBatch GetCreateTableSql(string tableName, Dictionary<string, string> columnDdls, List<string> primaryKeyColumns)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("Create Table If Not Exists {0}(", SafeName(tableName));
            foreach (var ddl in columnDdls)
            {
                sql.AppendFormat("{0} {1},", SafeName(ddl.Key), ddl.Value);
            }
            if (primaryKeyColumns != null && primaryKeyColumns.Count > 0)
            {
                var pkString = string.Join(",", primaryKeyColumns.ConvertToAll(pk => SafeName(pk)));
                sql.AppendFormat("Primary key ({0}) ", pkString);
            }
            sql.AppendFormat(")Engine InnoDB;");
            return new SqlBatch(sql.ToString());
        }

        public override SqlBatch GetUpdateTableSql(string tableName, Dictionary<string, string> columnDdls, List<string> primaryKeyColumns)
        {
            var sql = new StringBuilder();
            foreach (var ddl in columnDdls)
            {
                sql.AppendLine(string.Format("SELECT count(*) INTO @exist FROM information_schema.columns WHERE table_schema = database() AND table_name = '{0}' and COLUMN_NAME = '{1}';set @query = IF(@exist <= 0, 'Alter table `{0}` add column `{1}` {2}','select 1 status'); prepare stmt from @query; EXECUTE stmt;", tableName, ddl.Key, ddl.Value));
            }
            return new SqlBatch(sql.ToString());
        }

        public override SqlBatch GetSaveSql(string tableNameWithSchema, List<string> primaryKeys, List<string> columnList, params Params[] param)
        {
            var columnString = string.Join(",", columnList.ConvertToAll(p => SafeName(p)));
            var columnParamString = string.Join(",", columnList.ConvertToAll(p => "@" + p));
            var sql = string.Format("REPLACE INTO {0}({1}) VALUES({2})", tableNameWithSchema, columnString, columnParamString);
            return new SqlBatch(sql.ToString(), param);
        }

        public override PagedSqlBatch GetPagedSql(string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize, params Params[] param)
        {
            var limitStart = (pageNumber - 1) * pageSize;
            var pagedSql = string.Format("select * from ({0}) oldsqlstring order by {1} " + (isAsc ? "Asc" : "Desc") + " limit {2},{3}", sourceSql, orderColumnName, limitStart, pageSize);
            var countSql = string.Format("select count(1) from ({0}) oldsqlstring", sourceSql, orderColumnName, pageNumber, pageSize);
            return new PagedSqlBatch() { PagedSql = new SqlBatch(pagedSql, param), CountSql = new SqlBatch(countSql, param) };
        }
    }
}
