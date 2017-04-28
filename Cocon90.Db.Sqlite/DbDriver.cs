using Cocon90.Db.Common.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocon90.Db.Common.Data;
using System.Data;
using System.Data.Common;
#if NETSTANDARD
using Microsoft.Data.Sqlite;
using System.Reflection;
#else
using System.Data.SQLite;
#endif
namespace Cocon90.Db.Sqlite
{
    public class DbDriver : BaseDriver
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
#if NETSTANDARD
            var cmd = (SqliteCommand)CreateCommand(tsqlParamed, commandType, param);
#else
            var cmd = (SQLiteCommand)CreateCommand(tsqlParamed, commandType, param);
#endif
            var reader = cmd.ExecuteReader(behavior);
            return reader;
        }

        public override DbCommand CreateCommand(string tsqlParamed, CommandType commandType, params Params[] param)
        {
#if NETSTANDARD
            var conn = (SqliteConnection)this.CreateConnection();
            var cmd = new SqliteCommand(tsqlParamed, conn);
#else
            var conn = (SQLiteConnection)this.CreateConnection();
            var cmd = new SQLiteCommand(tsqlParamed, conn);
#endif

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
#if NETSTANDARD
            SqliteConnectionStringBuilder connString = new SqliteConnectionStringBuilder(this.ConnectionString);
            connString.Add("BinaryGUID", false);
            SqliteConnection conn = new SqliteConnection(connString.ConnectionString);
#else
            SQLiteConnectionStringBuilder connString = new SQLiteConnectionStringBuilder(this.ConnectionString);
            connString.BinaryGUID = false;
            SQLiteConnection conn = new SQLiteConnection(connString.ConnectionString);
#endif



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

#if NETSTANDARD
            SqliteParameter param = new SqliteParameter(keyStr, objValue);
#else
            SQLiteParameter param = new SQLiteParameter(keyStr, objValue);
#endif
            return param;
        }

        public override char ParameterChar
        {
            get { return '@'; }
        }

        public override string SafeName(string columnNameOrTableName)
        {
            return string.Format("'{0}'", columnNameOrTableName);
        }

        public override string TypeMapping(Type csType, int len = 0)
        {
            Dictionary<string, string> instence = new Dictionary<string, string>();
            instence.Add(typeof(int).FullName, "int");
            instence.Add(typeof(bool).FullName, "int");
            instence.Add(typeof(char).FullName, "varchar(2)");
            instence.Add(typeof(byte).FullName, "tinyint");
            instence.Add(typeof(short).FullName, "smallint");
            instence.Add(typeof(long).FullName, "bigint");
            instence.Add(typeof(float).FullName, "float");
            instence.Add(typeof(double).FullName, "decimal");
            instence.Add(typeof(decimal).FullName, "decimal");
            instence.Add(typeof(string).FullName, "mediumtext");
            instence.Add(typeof(byte[]).FullName, "longblob");
            instence.Add(typeof(DateTime).FullName, "datetime");
            instence.Add(typeof(Guid).FullName, "guid");

            instence.Add(typeof(int?).FullName, "int");
            instence.Add(typeof(bool?).FullName, "int");
            instence.Add(typeof(char?).FullName, "varchar(2)");
            instence.Add(typeof(byte?).FullName, "tinyint");
            instence.Add(typeof(short?).FullName, "smallint");
            instence.Add(typeof(long?).FullName, "bigint");
            instence.Add(typeof(float?).FullName, "float");
            instence.Add(typeof(double?).FullName, "double");
            instence.Add(typeof(decimal?).FullName, "double");
            instence.Add(typeof(DateTime?).FullName, "datetime");
            instence.Add(typeof(Guid?).FullName, "guid");

            //返回相应类型。
            if (instence.ContainsKey(csType.FullName))
                return instence[csType.FullName];
#if NETSTANDARD
            if (csType.GetTypeInfo().IsEnum || (Nullable.GetUnderlyingType(csType)??csType).GetTypeInfo().IsEnum)
                return "int";
#else
            if (csType.IsEnum || (Nullable.GetUnderlyingType(csType) ?? csType).IsEnum)
                return "int";
#endif
            return "mediumtext";
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
            sql.AppendFormat(");");
            return new SqlBatch(sql.ToString());
        }

        public override SqlBatch GetUpdateTableSql(string tableName, Dictionary<string, string> columnDdls, List<string> primaryKeyColumns)
        {
#if NETSTANDARD
            var conn = (SqliteConnection)CreateConnection();
#else
            var conn = (SQLiteConnection)CreateConnection();
#endif
            var dap = CreateDataReader(string.Format("PRAGMA table_info({0})", SafeName(tableName)), CommandType.Text);
            List<string> columns = new List<string>();
            int nameColumnIndex = 1;
            for (int i = 0; i < dap.FieldCount; i++)
            {
                if (dap.GetName(i) + "".ToLower() == "name")
                { nameColumnIndex = i; break; }
            }
            while (dap.Read())
            {
                columns.Add(dap.GetString(nameColumnIndex));
            }
            dap.Dispose();
            conn.Close();
            conn.Dispose();
            var sql = new StringBuilder();
            foreach (var ddl in columnDdls)
            {
                if (columns.Contains((ddl.Key + "").ToLower()))
                {
                    sql.AppendLine(string.Format("ALTER TABLE {0} Add Column {1} {2};", SafeName(tableName), SafeName(ddl.Key), ddl.Value));
                }
            }
            if (string.IsNullOrWhiteSpace(sql.ToString())) sql.AppendLine(";");
            return new SqlBatch(sql.ToString());
        }

        public override SqlBatch GetSaveSql(string tableNameWithSchema, List<string> primaryKeys, List<string> columnList, params Params[] param)
        {
            var columnString = string.Join(",", columnList.ConvertToAll(p => SafeName(p)));
            var columnParamString = string.Join(",", columnList.ConvertToAll(p => "@" + p));
            var sql = string.Format("INSERT OR REPLACE INTO {0}({1}) VALUES({2})", tableNameWithSchema, columnString, columnParamString);
            return new SqlBatch(sql.ToString(), param);
        }

        public override PagedSqlBatch GetPagedSql(string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize, params Params[] param)
        {
            var limitStart = (pageNumber - 1) * pageSize;
            var pagedSql = string.Format("select * from ({0}) oldsqlstring order by {1} " + (isAsc ? "Asc" : "Desc") + " LIMIT {3} OFFSET {2}", sourceSql, orderColumnName, limitStart, pageSize);
            var countSql = string.Format("select count(1) from ({0}) oldsqlstring", sourceSql, orderColumnName, pageNumber, pageSize);
            return new PagedSqlBatch() { PagedSql = new SqlBatch(pagedSql, param), CountSql = new SqlBatch(countSql, param) };
        }
    }
}
