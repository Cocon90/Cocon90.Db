﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Cocon90.Db.Common.Data;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections.Concurrent;

namespace Cocon90.Db.SqlServer
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
                return DirverType.SQLServer;
            }
        }
        public override DbDataReader CreateDataReader(string tsqlParamed, CommandType commandType = CommandType.Text, CommandBehavior behavior = CommandBehavior.Default, params Params[] param)
        {
            var cmd = (SqlCommand)CreateCommand(tsqlParamed, commandType, param);
            var reader = cmd.ExecuteReader(behavior);
            return reader;
        }

        public override DbCommand CreateCommand(string tsqlParamed, CommandType commandType, params Params[] param)
        {
            var conn = (SqlConnection)this.CreateConnection();
            var cmd = new SqlCommand(tsqlParamed, conn);
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
            SqlConnectionStringBuilder connString = new SqlConnectionStringBuilder(this.ConnectionString);
            SqlConnection conn = new SqlConnection(connString.ConnectionString);
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
            SqlParameter param = new SqlParameter(keyStr, objValue);
            return param;
        }

        public override char ParameterChar
        {
            get { return '@'; }
        }

        public override string SafeName(string columnNameOrTableName)
        {
            return string.Format("[{0}]", columnNameOrTableName);
        }

        public override string TypeMapping(Type csType, int len = 0)
        {
            ConcurrentDictionary<string, string> instence = new ConcurrentDictionary<string, string>();

            instence.TryAdd(typeof(int).FullName, "int");
            instence.TryAdd(typeof(bool).FullName, "bit");
            instence.TryAdd(typeof(char).FullName, "nvarchar(1)");
            instence.TryAdd(typeof(byte).FullName, "tinyint");
            instence.TryAdd(typeof(short).FullName, "smallint");
            instence.TryAdd(typeof(long).FullName, "bigint");
            instence.TryAdd(typeof(float).FullName, "real");
            instence.TryAdd(typeof(double).FullName, "decimal(18,8)");
            instence.TryAdd(typeof(decimal).FullName, "decimal(18,8)");
            instence.TryAdd(typeof(string).FullName, "nvarchar(max)");
            instence.TryAdd(typeof(byte[]).FullName, "image");
            instence.TryAdd(typeof(DateTime).FullName, "datetime");
            instence.TryAdd(typeof(DateTimeOffset).FullName, "datetimeoffset");
            instence.TryAdd(typeof(Guid).FullName, "uniqueidentifier");

            instence.TryAdd(typeof(int?).FullName, "int");
            instence.TryAdd(typeof(bool?).FullName, "bit");
            instence.TryAdd(typeof(char?).FullName, "nvarchar(1)");
            instence.TryAdd(typeof(byte?).FullName, "tinyint");
            instence.TryAdd(typeof(short?).FullName, "smallint");
            instence.TryAdd(typeof(long?).FullName, "bigint");
            instence.TryAdd(typeof(float?).FullName, "real");
            instence.TryAdd(typeof(double?).FullName, "decimal(18,8)");
            instence.TryAdd(typeof(decimal?).FullName, "decimal(18,8)");
            instence.TryAdd(typeof(DateTime?).FullName, "datetime");
            instence.TryAdd(typeof(DateTimeOffset?).FullName, "datetimeoffset");
            instence.TryAdd(typeof(Guid?).FullName, "uniqueidentifier");
            //返回相应类型。
            if (instence.ContainsKey(csType.FullName))
                return instence[csType.FullName];

            if (csType.IsEnum || (Nullable.GetUnderlyingType(csType) ?? csType).IsEnum)
                return "int";

            return "nvarchar(max)";
        }

        public override SqlBatch GetCreateTableSql(string tableName, ConcurrentDictionary<string, string> columnDdls, List<string> primaryKeyColumns, ConcurrentDictionary<string, string> columnName2IndexNames)
        {
            /* 
           IF NOT EXISTS (select * from sysobjects where id = object_id('[myindextab]') and OBJECTPROPERTY(id, 'IsUserTable') = 1) BEGIN 
            CREATE TABLE [myindextab]
            ([RowId] uniqueidentifier,[UserType] int,[AgeType] int,[NameType] varchar(255));
            Primary key([RowId],[UserType])
            CREATE INDEX [idx_myindextab_usertype] ON [myindextab]([UserType]);
            CREATE INDEX [idx_myindextab_group] ON [myindextab]([AgeType],[NameType]);
            END
            */

            var sql = new StringBuilder();
            //add create tab.
            sql.AppendFormat("IF NOT EXISTS (select * from sysobjects where id = object_id('{0}') and OBJECTPROPERTY(id, 'IsUserTable') = 1) BEGIN CREATE TABLE {0}(", SafeName(tableName));
            for (int i = 0; i < columnDdls.Count; i++)
            {
                var ddl = columnDdls.ElementAt(i);
                var dot = (i == columnDdls.Count - 1 && primaryKeyColumns.Count == 0) ? string.Empty : ",";
                sql.AppendFormat("{0} {1}{2}", SafeName(ddl.Key), ddl.Value, dot);
            }
            if (primaryKeyColumns != null && primaryKeyColumns.Count > 0)
            {
                var pkString = string.Join(",", primaryKeyColumns.ConvertToAll(pk => SafeName(pk)));
                sql.AppendFormat("Primary key ({0})", pkString);
            }
            sql.Append(");");
            //add index.
            var kvs = ConvertIndexStrings(columnName2IndexNames);
            foreach (var kv in kvs)
            {
                sql.AppendFormat("CREATE INDEX {0} ON {1}({2});", SafeName(kv.Key), SafeName(tableName), kv.Value);
            }
            sql.Append("END");
            return new SqlBatch(sql.ToString());
        }

        public override SqlBatch GetUpdateTableSql(string tableName, ConcurrentDictionary<string, string> columnDdls, List<string> primaryKeyColumns, ConcurrentDictionary<string, string> columnName2IndexNames)
        {
            /*
             BEGIN 
             if not exists (select * from syscolumns where id=object_id('myindextab') and name='RowId') alter table [myindextab] add [RowId] uniqueidentifier;
             if not exists (select * from syscolumns where id=object_id('myindextab') and name='UserType') alter table [myindextab] add [UserType] int;
             if not exists (select * from syscolumns where id=object_id('myindextab') and name='AgeType') alter table [myindextab] add [AgeType] int;
             if not exists (select * from syscolumns where id=object_id('myindextab') and name='NameType') alter table [myindextab] add [NameType] varchar(255);
             IF Not EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('[myindextab]') AND NAME ='idx_myindextab_usertype') CREATE INDEX [idx_myindextab_usertype] ON [myindextab]([UserType]);
             IF Not EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('[myindextab]') AND NAME ='idx_myindextab_group') CREATE INDEX [idx_myindextab_group] ON [myindextab]([AgeType],[NameType]); 
             END
              */
            var sql = new StringBuilder();
            sql.Append("BEGIN ");
            foreach (var ddl in columnDdls)
            {
                sql.Append(string.Format("IF NOT EXISTS (SELECT * FROM syscolumns WHERE id=object_id('{0}') AND NAME='{1}') ALTER TABLE [{0}] ADD [{1}] {2};", tableName, ddl.Key, ddl.Value));
            }
            //add index.
            var kvs = ConvertIndexStrings(columnName2IndexNames);
            foreach (var kv in kvs)
            {
                sql.AppendFormat("IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('{1}') AND NAME ='{3}') CREATE INDEX {0} ON {1}({2});", SafeName(kv.Key), SafeName(tableName), kv.Value, kv.Key);
            }
            sql.Append(" END");
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
            /*
             if NOT exists(SELECT * FROM Setting WHERE ID=0)
             INSERT INTO Setting(ID,Title,SearchKeys, ServiceIntervalSecond,SleepMillisecondPerSearch) VALUES(0,@title,@searchKeys,@serviceIntervalSecond,@sleepMillisecondPerSearch) 
             ELSE 
             UPDATE Setting SET Title=@title,SearchKeys=@searchKeys,ServiceIntervalSecond=@serviceIntervalSecond, SleepMillisecondPerSearch=@sleepMillisecondPerSearch
            */
            var columnString = string.Join(",", columnList.ConvertToAll(p => SafeName(p)));
            var columnParamString = string.Join(",", columnList.ConvertToAll(p => "@" + p));
            var insertSql = string.Format("INSERT INTO {0}({1}) VALUES({2})", tableNameWithSchema, columnString, columnParamString);
            if (primaryKeys.Count == 0)
                return new SqlBatch(insertSql, param);

            string columnParaString = string.Join(",", columnList.ConvertToAll(pk => SafeName(pk) + "=@" + pk)); //[Col1]=@Col1 , [Col2]=@Col2, [Col3]=@Col3
            var primeryParamString = string.Join(" and ", primaryKeys.ConvertToAll(pk => SafeName(pk) + "=@" + pk)); //[PK1]=@PK1 and [PK2]=@PK2 and [PK3]=@PK3
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("if NOT exists(SELECT * FROM {0} WHERE {1}) ", tableNameWithSchema, primeryParamString);
            sb.Append(insertSql);
            sb.Append(" ELSE ");
            sb.AppendFormat(" UPDATE {0} SET {1} WHERE {2}", tableNameWithSchema, columnParaString, primeryParamString);

            return new SqlBatch(sb.ToString(), param);
        }

        public override PagedSqlBatch GetPagedSql(string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize, params Params[] param)
        {
            var limitStart = (pageNumber - 1) * pageSize + 1;
            var limitEnd = pageNumber * pageSize;
            var rownum = "rownum" + DateTime.Now.Millisecond + "" + DateTime.Now.Second + "" + DateTime.Now.Minute;
            var pagedSql = string.Format("select * from (select *,ROW_NUMBER() over (order by {0} " + (isAsc ? " Asc " : " Desc ") + ") " + rownum + " from ({1}) oldsqlstring" + DateTime.Now.Millisecond + "" + DateTime.Now.Second + "" + DateTime.Now.Minute + ") tab where tab." + rownum + " between {2} and {3}", orderColumnName, sourceSql, limitStart, limitEnd);
            var countSql = string.Format("select count(1) from ({0}) oldsqlstring", sourceSql, orderColumnName);
            return new PagedSqlBatch() { PagedSql = new SqlBatch(pagedSql, param), CountSql = new SqlBatch(countSql, param) };
        }

        public override SqlBatch GetInsertIfNotExistSql(string tableNameWithSchema, List<string> insertColumns, ConcurrentDictionary<string, object> insertColumnsValues, string selectSqlCondition, params Params[] param)
        {
            /*
            "IF NOT EXISTS (SELECT id FROM books WHERE id = 1)  INSERT INTO books (name) SELECT 'Songxingzhu'"
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
            var sql = string.Format("IF NOT EXISTS ({1}) INSERT INTO {0} ({2}) SELECT {3}", tableNameWithSchema, selectSqlCondition, insertColumnsStr, string.Join(",", insertColumnsArgs));
            return new SqlBatch(sql, paramlist.ToArray());
        }
    }
}
