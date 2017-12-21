using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Cocon90.Db.Common.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;
using System.Collections.Concurrent;

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
            ConcurrentDictionary<string, string> instence = new ConcurrentDictionary<string, string>();
            instence.TryAdd(typeof(int?).FullName, "int");
            instence.TryAdd(typeof(bool?).FullName, "bit");
            instence.TryAdd(typeof(char?).FullName, "varchar(2)");
            instence.TryAdd(typeof(byte?).FullName, "tinyint");
            instence.TryAdd(typeof(short?).FullName, "smallint");
            instence.TryAdd(typeof(long?).FullName, "bigint");
            instence.TryAdd(typeof(float?).FullName, "float");
            instence.TryAdd(typeof(double?).FullName, "double");
            instence.TryAdd(typeof(decimal?).FullName, "double");
            instence.TryAdd(typeof(DateTime?).FullName, "datetime");
            instence.TryAdd(typeof(DateTimeOffset?).FullName, "datetime");
            instence.TryAdd(typeof(Guid?).FullName, "varchar(36)");

            instence.TryAdd(typeof(int).FullName, "int");
            instence.TryAdd(typeof(bool).FullName, "bit");
            instence.TryAdd(typeof(char).FullName, "varchar(2)");
            instence.TryAdd(typeof(byte).FullName, "tinyint");
            instence.TryAdd(typeof(short).FullName, "smallint");
            instence.TryAdd(typeof(long).FullName, "bigint");
            instence.TryAdd(typeof(float).FullName, "float");
            instence.TryAdd(typeof(double).FullName, "decimal");
            instence.TryAdd(typeof(decimal).FullName, "decimal");
            instence.TryAdd(typeof(string).FullName, "longtext");
            instence.TryAdd(typeof(byte[]).FullName, "longblob");
            instence.TryAdd(typeof(DateTime).FullName, "datetime");
            instence.TryAdd(typeof(DateTimeOffset).FullName, "datetime");
            instence.TryAdd(typeof(Guid).FullName, "varchar(36)");

            //返回相应类型。
            if (instence.ContainsKey(csType.FullName))
                return instence[csType.FullName];
            if (csType.IsEnum || (Nullable.GetUnderlyingType(csType) ?? csType).IsEnum)
                return "int";
            return "longtext";
        }

        public override SqlBatch GetCreateTableSql(string tableName, ConcurrentDictionary<string, string> columnDdls, List<string> primaryKeyColumns, ConcurrentDictionary<string, string> columnName2IndexNames)
        {
            /*
             CREATE TABLE IF NOT EXISTS `myindextab`(
	            `RowId` varchar(36),
	            `UserType` int,
	            `AgeType` int,
	            `NameType` varchar(255),
	            Primary key (`RowId`),
	            INDEX `idx_myindextab_usertype`(`UserType`),
	            INDEX `idx_myindextab_group`(`AgeType`,`NameType`)
            )Engine InnoDB;
             */
            var sql = new StringBuilder();
            sql.AppendFormat("CREATE TABLE IF NOT EXISTS {0}(", SafeName(tableName));
            for (int i = 0; i < columnDdls.Count; i++)
            {
                var ddl = columnDdls.ElementAt(i);
                var dot = (i == columnDdls.Count - 1 && primaryKeyColumns.Count == 0) ? string.Empty : ",";
                sql.AppendFormat("{0} {1}{2}", SafeName(ddl.Key), ddl.Value, dot);
            }
            if (primaryKeyColumns != null && primaryKeyColumns.Count > 0)
            {
                var pkString = string.Join(",", primaryKeyColumns.ConvertToAll(pk => SafeName(pk)));
                var dot = columnName2IndexNames.Count > 0 ? "," : string.Empty;
                sql.AppendFormat("Primary key ({0}){1}", pkString, dot);
            }
            //add index.
            var kvs = ConvertIndexStrings(columnName2IndexNames);
            List<string> indexStringList = new List<string>();
            foreach (var kv in kvs)
            {
                indexStringList.Add(string.Format("INDEX {0}({1})", SafeName(kv.Key), kv.Value));
            }
            sql.Append(string.Join(",", indexStringList));
            sql.AppendFormat(")Engine InnoDB;");
            return new SqlBatch(sql.ToString());
        }

        public override SqlBatch GetUpdateTableSql(string tableName, ConcurrentDictionary<string, string> columnDdls, List<string> primaryKeyColumns, ConcurrentDictionary<string, string> columnName2IndexNames)
        {
            var sql = new StringBuilder();
            foreach (var ddl in columnDdls)
            {
                sql.AppendLine(string.Format("SELECT count(*) INTO @exist FROM information_schema.columns WHERE table_schema = database() AND table_name = '{0}' and COLUMN_NAME = '{1}';set @query = IF(@exist <= 0, 'Alter table `{0}` add column `{1}` {2}','select 1 status'); prepare stmt from @query; EXECUTE stmt;", tableName, ddl.Key, ddl.Value));
            }
            //add index.
            var kvs = ConvertIndexStrings(columnName2IndexNames);
            List<string> indexStringList = new List<string>();
            foreach (var kv in kvs)
            {
                sql.AppendLine(string.Format("DROP PROCEDURE IF EXISTS schema_change_cocon90_mysql; CREATE PROCEDURE schema_change_cocon90_mysql() BEGIN  IF NOT EXISTS (SELECT * FROM information_schema.statistics WHERE table_schema=database() AND table_name = '{0}' AND index_name = '{1}') THEN   ALTER TABLE `{0}` ADD INDEX `{1}` ({2}); END IF; END ; CALL schema_change_cocon90_mysql(); DROP PROCEDURE IF EXISTS schema_change_cocon90_mysql;", tableName, kv.Key, kv.Value));
            }
            return new SqlBatch(sql.ToString());
        }
        /// <summary>
        /// convert to indexName:[pro1],[prop2]
        /// </summary>
        private List<KeyValuePair<string, string>> ConvertIndexStrings(ConcurrentDictionary<string, string> columnName2IndexNames)
        {
            if (columnName2IndexNames != null && columnName2IndexNames.Count > 0)
            {
                Dictionary<string, List<string>> indexName2Props = new Dictionary<string, List<string>>();
                foreach (var kv in columnName2IndexNames)
                {
                    var indexName = kv.Value;
                    var columnName = kv.Key;
                    if (!indexName2Props.ContainsKey(indexName))
                    {
                        List<string> cols = new List<string>();
                        cols.Add(columnName);
                        indexName2Props.Add(indexName, cols);
                    }
                    else
                    {
                        if (indexName2Props[indexName] != null && !indexName2Props[indexName].Contains(columnName))
                            indexName2Props[indexName].Add(columnName);
                    }
                }
                var kvs = indexName2Props.ConvertToAll(s => new KeyValuePair<string, string>(s.Key, string.Join(",", s.Value.ConvertToAll(v => SafeName(v)))));
                return kvs;
            }
            return new List<KeyValuePair<string, string>>();
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

        public override SqlBatch GetInsertIfNotExistSql(string tableNameWithSchema, List<string> insertColumns, ConcurrentDictionary<string, object> insertColumnsValues, string selectSqlCondition, params Params[] param)
        {
            /*
            "INSERT INTO books (name) SELECT 'SongXingzhu' FROM dual WHERE NOT EXISTS (SELECT id FROM books WHERE id = 1)"
            */
            if (string.IsNullOrWhiteSpace(tableNameWithSchema) || insertColumns == null || insertColumns.Count == 0 || insertColumnsValues.Count != insertColumns.Count || string.IsNullOrWhiteSpace(selectSqlCondition))
                throw new ArgumentException("please check input arguments:tableNameWithSchema/insertColumns/insertColumnsValues/selectSqlCondition,and insertColumnsValues count must equal with insertColumns count.");
            var insertColumnsStr = string.Join(",", insertColumns.ConvertToAll(s => SafeName(s)));
            Func<string, string> buildNewColumnName = (s => ParameterChar + "SysParam_" + s);
            var insertColumnsArgs = insertColumns.ConvertToAll(buildNewColumnName);
            List<Params> paramlist = new List<Params>();
            if (param != null && param.Length > 0) paramlist.AddRange(param);
            foreach (var col in insertColumns)
            {
                if (insertColumnsValues.ContainsKey(col))
                    paramlist.Add(new Params(buildNewColumnName(col), insertColumnsValues[col]));
                else
                    paramlist.Add(new Params(buildNewColumnName(col), null));
            }
            var sql = string.Format("INSERT INTO {0} ({1}) SELECT {2} FROM dual WHERE NOT EXISTS ({3})", tableNameWithSchema, insertColumnsStr, string.Join(",", insertColumnsArgs), selectSqlCondition);
            return new SqlBatch(sql, paramlist.ToArray());
        }
    }
}
