using Cocon90.Db.Common.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Driver
{
    public abstract class BaseDriver
    {
        public BaseDriver(string connectionString) { this.ConnectionString = connectionString; }
        public string ConnectionString { get; set; }
        public abstract DirverType DirverType { get; }
        public abstract DbConnection CreateConnection();
        public abstract DbCommand CreateCommand(string tsqlParamed, CommandType commandType, params Params[] param);
        public abstract DbDataReader CreateDataReader(string tsqlParamed, CommandType commandType = CommandType.Text, CommandBehavior behavior = CommandBehavior.Default, params Params[] param);
        public abstract DbParameter CreateParameter(string key, object value);
        public abstract char ParameterChar { get; }
        public abstract string SafeName(string columnNameOrTableName);
        public abstract string TypeMapping(Type csType, int len = 0);
        public abstract SqlBatch GetCreateTableSql(string tableName, Dictionary<string, string> columnDdls, List<string> primaryKeyColumns, Dictionary<string, string> columnName2IndexNames);
        public abstract SqlBatch GetUpdateTableSql(string tableName, Dictionary<string, string> columnDdls, List<string> primaryKeyColumns, Dictionary<string, string> columnName2IndexNames);
        public abstract SqlBatch GetSaveSql(string tableNameWithSchema, List<string> primaryKeys, List<string> columnList, params Params[] param);
        public abstract PagedSqlBatch GetPagedSql(string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize, params Params[] param);
    }
}