**Cocon90.DB 使用说明**

**一、简介**

   Cocon90.Db是由Cocon90.Db.Common为核心的类库与其它数据库操作库组合而成，以方便调用为主要目的，支持ORM操作，增、删、改、查、事务、批量执行、创建表、插入或保存记录 等等，并提供多种数据库支持。当前已支持Mysql、Sqlite、SqlServer。
>Cocon90.Db is a core class library which is composed of Cocon90.Db.Common and other database operations, which is convenient to call for the main purpose, and provides a variety of database support. Currently supports Mysql, Sqlite, SqlServer.

**二、运行环境**

    (1)Windows平台下，需要.NetFramework4.0+
    (2)Linux或MAC平台下，需要.NetStandard1.3+ 支持.Net Core 1.0+


**三、使用方法**

（1）新建一件空的项目，取名`Cocon90.Db.Demo`。

（2）添加引用(此步可省略，因为下一步会自动执行这一步)，或者执行`Install-Package Cocon90.Db.Common`或者.NetCore平台使用`Install-Package Cocon90.DbCore.Common`。

（3）接下来，引入数据库操作库。

1. 如果要操作`SqlServer`数据库则引入`Cocon90.Db.SqlServer`或者.NetCore平台使用`Install-Package Cocon90.DbCore.SqlServer`，

2. 如果要操作`Sqlite`数据库，则引入`Cocon90.Db.Sqlite`或者.NetCore平台使用`Install-Package Cocon90.DbCore.Sqlite`，

3. 如果要操作`Mysql`，则需要引入`Install-Package Cocon90.Db.Mysql`库或者.NetCore平台使用`Install-Package Cocon90.DbCore.Mysql`，

（4）新建Program类，加入Main函数，比如我们要操作Sqlite数据库加入下列代码：
```C#
static void Main(string[] args)
{
    // 依次传入数据库类型，连接字符串来构建操作对像（此方法不会连接数据库，可以提前实例好）。
    var dh = Cocon90.Db.Common.Db.GetDataHelper("Sqlite", "Data Source=${app}\\mysqlite.db;");
    // 或者也可以直接通过读取app.config或者web.config或者.NetCore中的appsettings.json来读取连接字符串。
    var dh = Cocon90.Db.Common.Db.GetDataHelper();
    
    //通过实体（下方有实体的例子），执行创建数据表
    var effRow = dh.CreateOrUpdateTable<Model.CountryLanguageModel>();
    //...
    var tab = dh.GetTable("select * from Student");
    var eff = dh.ExecNoQuery("...");
    var str = dh.GetString("...");
    var isok = dh.GetBoolean("...");
    var age = dh.GetInt("...");
    var mylst = dh.GetListString("...");
    var pr = dh.GetPagedResult("...");
}

```
在NetFramework中，如果在要读取配置文件中的app.config中的连接语句，则需要在App.config中进行如下配置：

```xml
<configuration>
  <connectionStrings>
    <!--<add name="dbConnString" providerName="Mysql" connectionString="Server=127.0.0.1;Port=3306;Database=world;Uid=root;Pwd=123456;"/>-->
    <!--<add  name="dbConnString" providerName="SqlServer" connectionString="Server=127.0.0.1;Database=world;Uid=sa;Pwd=123456;"/>-->
    <add name="dbConnString" providerName="Sqlite" connectionString="Data Source=${app}\SQLiteSpy.db3;"/>
  </connectionStrings>
</configuration>
```


在NetCore或者其它NetStandard中如果在要读取配置文件中的appsettings.json中的连接语句，则需要在appsettings.json中进行如下配置：

```json
{
  "dbType": "SqlServer",
  "dbConnString": "Server=.;Database=world;Uid=sa;Pwd=123456;"
}
```


下面是常用的一些测试：
```C#
 class Program
    {
        static void Main(string[] args)
        {
            //构建数据库操作类
            //var dh = Cocon90.Db.Common.Db.GetDataHelper("Cocon90.Db.Sqlite.dll", "Cocon90.Db.Sqlite.DbDriver", "D:\\Application\\DbTools\\sqliteSpy\\SQLiteSpy.db3;");
            var dh = Cocon90.Db.Common.Db.GetDataHelper();

            //生成建表的Sql
            var createSql = dh.GetCreateTableSql<Model.CountryLanguageModel>();
            //生成更新表结构的Sql，用于当实体发生变化时，加入对新加的列。
            var updateTabSql = dh.GetUpdateTableSql(typeof(Model.CountryLanguageModel));
            //执行建表的Sql，如果表结构已存在，且和实体相匹配，则自动跳过。
            var effRow = dh.CreateOrUpdateTable<Model.CountryLanguageModel>();
            
            //批量保存实体。
            var needInserts = new List<Model.CountryLanguageModel>();
            Random rand = new Random();
            for (int i = 0; i < 500; i++)
            {
                needInserts.Add(new Model.CountryLanguageModel() { Percent = (decimal)(rand.NextDouble() * 10), Date = DateTime.Now.AddDays(-1 * i), Guid = Guid.NewGuid(), IsOfficial = rand.Next(0, 2) > 0, Code = i, Language = "Lang_" + i });
            }
            //可以保存单个实体，也可以一个事务中保存多个实体，要求有主键，有则保存，无则更新，可以选择只更新非Null值。
            var succRows = dh.Save(needInserts.ToArray());
            //可以插入单个实体，也可以一个事务中插入多个实体。
            //var succRows = dh.Insert(needInserts.ToArray());

            //常规查询表（返回类似于DataTable结构的轻量集类MDataTable）
            var tab = dh.GetTable("SELECT * FROM countrylanguage");
            
            //返回List实体集合
            var lst = dh.GetList<Model.CountryLanguageModel>("SELECT * FROM countrylanguage");
            Console.WriteLine(lst.Count);

            //返回1个实体
            var oneModel = dh.GetOne<Model.CountryLanguageModel>("select * from countrylanguage");

            //通过主键返回1个实体，如果是多主键，则依次传入多个主键(多主建时，按主键列名排序依次传入)。
            var oneModel2 = dh.GetOneByPrimaryKey<Model.CountryLanguageModel>(1, "Lang_1");
            
            //一个事务中插入记录
            //var successRows = dh.Insert(new Model.CountryLanguage() { Percent = 1.555m, IsOfficial = false, Code = 2, Language = "Lang" },
            //      new Model.CountryLanguage() { Percent = 1.66m, IsOfficial = true, Code = 3, Language = "Lang" });
            
            //获取通过主键更新记录的Sql语句，要更新的字段设置值，其它为NULL即可。
            var updateSql = dh.GetUpdateSqlByPrimaryKey(new Model.CountryLanguageModel() { Percent = 9.9m }, true, "1=1 AND 2=2", 3, "Lang");

            //获取通过实体中的主键更新记录的Sql语句，要更新的字段设置值，其它为NULL即可。
            var updateSql2 = dh.GetUpdateSql(new Model.CountryLanguageModel { Code = 3, Percent = 3.3m }, false, null);
             
            //获取通过自定义Where中的主键更新记录的Sql语句，要更新的字段设置值，其它为NULL即可。
            var updateSql3 = dh.GetUpdateSqlByWhere(new Model.CountryLanguageModel { Code = 3, Percent = 3.3m }, true, "Language='Lang'", new Common.Data.Params("@Name", "song"));

            //通过自定义where更新实体，要更新的字段设置值，其它为NULL即可。
            var updateRow3 = dh.UpdateByByWhere(new Model.CountryLanguageModel { Percent = 3.3m }, true, "Language='Lang'");

            //通过传入的主键列表(多主建时，按主键列名排序依次传入)更新实体，要更新的字段设置值，其它为NULL即可。
            var updateRow = dh.UpdateByPrimaryKey(new Model.CountryLanguageModel { Percent = 4.5m }, true, null, 3, "Lang");
            var deleteSql = dh.GetDeleteSqlByPrimaryKey<Model.CountryLanguageModel>("1=1", 3, "Lang");
            var deleteSql1 = dh.GetDeleteSqlByPrimaryKey<Model.CountryLanguageModel>(null, 3, "Lang_111");
            var deleteSql2 = dh.GetDeleteSqlByWhere<Model.CountryLanguageModel>("Percentage=@Perc", new Common.Data.Params("Perc", 100));
            var deleteSql3 = dh.GetDeleteSql(new Model.CountryLanguageModel { Code = 3, Percent = 3.3m }, "1=@myParam", new Common.Data.Params("myParam", 1));
            var deleteSql4 = dh.GetDeleteSql<Model.CountryLanguageModel>(null, "1=@myParam", new Common.Data.Params("myParam", 1));
            var successRow = dh.Delete(new Model.CountryLanguageModel { Code = 3, Percent = 4.5m });

            //返回保存Sql，无则添加，有则更新。
            var saveSql = dh.GetSaveSql(new Model.CountryLanguageModel() { Percent = 1.555m, IsOfficial = false, Code = 2, Language = "Lang" },
                 new Model.CountryLanguageModel() { Percent = 1.66m, IsOfficial = true, Code = 3, Language = "Lang" });
            var saveRows = dh.Save(new Model.CountryLanguageModel() { Percent = 1.555m, IsOfficial = false, Code = 2, Language = "Lang" },
               new Model.CountryLanguageModel() { Percent = 1.66m, IsOfficial = true, Code = 3, Language = "Lang" });
            var executeNoQuery = dh.ExecNoQuery("update countrylanguage set Percentage=4.4 where Percentage=@Percentage", new Model.CountryLanguageModel { Percent = 1.6m });

            //取得分页查询的Sql，传入Sql语句，和排序列，是否正序，返回第1页，每页返回10条。
            var pageSql = dh.Driver.GetPagedSql("select * from countrylanguage", "CountryCode", true, 1, 10);
            //取得分页查询的结果，传入Sql语句，和排序列，是否正序，返回第1页，每页返回10条。
            var pageResult = dh.GetPagedResult<Model.CountryLanguageModel>("select * from countrylanguage", "countrycode", true, 1, 10);
            List<Model.CountryLanguageModel> data = pageResult.Data;
            int totalRecordCount = pageResult.Total;
            int pageNum = pageResult.PageNumber;
            int pageSize = pageResult.PageSize;

            //一个事务中，批量执行Sql语句。
            var sql1 = new SqlBatch("update countrylanguage set Percentage=4.4 where 1=2");
            var sql2 = new SqlBatch("update countrylanguage set Percentage=4.4 where 1=4");
            var sql3 = new SqlBatch("update countrylanguage set Percentage=4.4 where 1=6");
            var isCommitOk = dh.ExecBatch(new SqlBatch[] { sql1, sql2, sql3 }, true)>0;
        }
    }
```

其中测试实体类：

```C#
    public class Student
    {
        [Cocon90.Db.Common.Attribute.Column(PrimaryKey = true)]
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Addrss { get; set; }
        public DateTime? Birthday { get; set; }
    }
```

```C#
    [Table(TableName = "CountryLanguage")]
    public class CountryLanguageTab
    {
        [Column(PrimaryKey = true, ColumnName = "CountryCode")]
        public int? Code { get; set; }
        [Column(PrimaryKey = true, CreateDDL = "varchar(20)")]
        public string Language { get; set; }
        public bool? IsOfficial { get; set; }
        [Column(ColumnName = "Percentage", PrimaryKey = false)]
        public decimal? Percent { get; set; }
        public DateTime? Date { get; set; }
        public Guid? Guid { get; set; }
    }

    [Table(TableName = "CountryLanguage")]
    public class CountryLanguageModel : CountryLanguageTab
    {
        [Ignore]
        public string CodeAndLang { get; set; }
    }
```

需要注意的是，实体的设计时，所有类型都必须可以为NULL值（如果是结构体类型，请采用可空类型）。