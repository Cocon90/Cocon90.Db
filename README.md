##Cocon90.DB 使用说明
**一、简介**
   Cocon90.Db是由Cocon90.Db.Common为核心的类库与其它数据库操作库组合而成，以方便调用为主要目的，支持ORM操作，增、删、改、查、事务、批量执行、创建表、插入或保存记录 等等，并提供多种数据库支持。当前已支持Mysql、Sqlite、SqlServer。
>Cocon90.Db is a core class library which is composed of Cocon90.Db.Common and other database operations, which is convenient to call for the main purpose, and provides a variety of database support. Currently supports Mysql, Sqlite, SqlServer.

**二、运行环境**

>| 环境 | 值 |
|--------|--------|
|NetFramework4.0+|Required|
|VisualStudio 2015+|Required|

**三、使用方法**
（1）新建一件空的项目，取名`Cocon90.Db.Demo`。
（2）添加对库`Cocon90.Db.Common.dll`的引用，或者执行`Install-Package Cocon90.Db.Common`。
（3）接下来，如果要操作SqlServer数据库则引入`Cocon90.Db.SqlServer`，如果要操作Sqlite数据库，则引入`Cocon90.Db.Sqlite`，如果要操作Mysql，则需要引入`Cocon90.Db.Mysql`库。
>| DataBase | Require Library |
|--------|--------|
|MySql|Cocon90.Db.Common,Cocon90.Db.Mysql|
|Sqlite|Cocon90.Db.Common,Cocon90.Db.Sqlite|
|SqlServer|Cocon90.Db.Common,Cocon90.Db.SqlServer|

（4）新建Program类，加入Main函数，比如我们要操作Sqlite数据库加入下列代码：
```C#
static void Main(string[] args)
{
 	var dh = Cocon90.Db.Common.Db.GetDataHelper("Cocon90.Db.Sqlite.dll", "Cocon90.Db.Sqlite.DbDriver", "D:\\mysqlite.db;");
    // or you can use app.config by this code:
    var dh = Cocon90.Db.Common.Db.GetDataHelper();
}
```
如果在要读取配置文件中的app.config中的连接语句，则需要在App.config中进行如下配置：

```xml
<configuration>
  <connectionStrings>
    <!--<add name="ConnectionString" providerName="${app}\Cocon90.Db.Mysql.dll|Cocon90.Db.Mysql.DbDriver" connectionString="Server=127.0.0.1;Port=3306;Database=world;Uid=root;Pwd=123456;"/>-->
    <!--<add  name="ConnectionString" providerName="${app}\Cocon90.Db.SqlServer.dll|Cocon90.Db.SqlServer.DbDriver" connectionString="Server=127.0.0.1;Database=world;Uid=sa;Pwd=123456;"/>-->
    <add name="ConnectionString" providerName="${app}\Cocon90.Db.Sqlite.dll|Cocon90.Db.Sqlite.DbDriver" connectionString="Data Source=${app}\SQLiteSpy.db3;"/>
  </connectionStrings>
</configuration>
```
然后即可进行数据库的常规操作：
```C#
    var tab = dh.GetTable("select * from Student");
    dh.ExecNoQuery("...");
    dh.GetString("...");
    dh.GetBoolean("...");
    dh.GetInt("...");
    dh.GetListString("...");
    dh.GetPagedResult("...");
    dh.CreateOrUpdateTable(typeof(Model.Student));
```

下面是常用的一些测试：
```C#
 class Program
    {
        static void Main(string[] args)
        {
            //var dh = Cocon90.Db.Common.Db.GetDataHelper("Cocon90.Db.Sqlite.dll", "Cocon90.Db.Sqlite.DbDriver", "D:\\Application\\DbTools\\sqliteSpy\\SQLiteSpy.db3;");

            var dh = Cocon90.Db.Common.Db.GetDataHelper();

            var createSql = dh.GetCreateTableSql<Model.CountryLanguageModel>();
            var updateTabSql = dh.GetUpdateTableSql(typeof(Model.CountryLanguageModel));
            var effRow = dh.CreateOrUpdateTable<Model.CountryLanguageModel>();

            var needInserts = new List<Model.CountryLanguageModel>();
            Random rand = new Random();
            for (int i = 0; i < 500; i++)
            {
                needInserts.Add(new Model.CountryLanguageModel() { Percent = (decimal)(rand.NextDouble() * 10), Date = DateTime.Now.AddDays(-1 * i), Guid = Guid.NewGuid(), IsOfficial = rand.Next(0, 2) > 0, Code = i, Language = "Lang_" + i });
            }
            var succRows = dh.Save(needInserts.ToArray());
            dh.GetTable("SELECT * FROM countrylanguage");
            var lst = dh.GetList<Model.CountryLanguageModel>("SELECT * FROM countrylanguage");
            Console.WriteLine(lst.Count);
            var oneModel = dh.GetOne<Model.CountryLanguageModel>("select * from countrylanguage");
            var oneModel2 = dh.GetOneByPrimaryKey<Model.CountryLanguageModel>(1, "Lang_1");
            //var successRows = dh.Insert(new Model.CountryLanguage() { Percent = 1.555m, IsOfficial = false, Code = 2, Language = "Lang" },
            //      new Model.CountryLanguage() { Percent = 1.66m, IsOfficial = true, Code = 3, Language = "Lang" });
            var updateSql = dh.GetUpdateSqlByPrimaryKey(new Model.CountryLanguageModel() { Percent = 9.9m }, true, "1=1 AND 2=2", 3, "Lang");
            var updateSql2 = dh.GetUpdateSql(new Model.CountryLanguageModel { Code = 3, Percent = 3.3m }, false, null);
            var updateSql3 = dh.GetUpdateSqlByWhere(new Model.CountryLanguageModel { Code = 3, Percent = 3.3m }, true, "Language='Lang'", new Common.Data.Params("@Name", "song"));
            var updateRow3 = dh.UpdateByByWhere(new Model.CountryLanguageModel { Percent = 3.3m }, true, "Language='Lang'");
            var updateRow = dh.UpdateByPrimaryKey(new Model.CountryLanguageModel { Percent = 4.5m }, true, null, 3, "Lang");
            var deleteSql = dh.GetDeleteSqlByPrimaryKey<Model.CountryLanguageModel>("1=1", 3, "Lang");
            var deleteSql1 = dh.GetDeleteSqlByPrimaryKey<Model.CountryLanguageModel>(null, 3, "Lang_111");
            var deleteSql2 = dh.GetDeleteSqlByWhere<Model.CountryLanguageModel>("Percentage=@Perc", new Common.Data.Params("Perc", 100));
            var deleteSql3 = dh.GetDeleteSql(new Model.CountryLanguageModel { Code = 3, Percent = 3.3m }, "1=@myParam", new Common.Data.Params("myParam", 1));
            var deleteSql4 = dh.GetDeleteSql<Model.CountryLanguageModel>(null, "1=@myParam", new Common.Data.Params("myParam", 1));
            var successRow = dh.Delete(new Model.CountryLanguageModel { Code = 3, Percent = 4.5m });

            var saveSql = dh.GetSaveSql(new Model.CountryLanguageModel() { Percent = 1.555m, IsOfficial = false, Code = 2, Language = "Lang" },
                 new Model.CountryLanguageModel() { Percent = 1.66m, IsOfficial = true, Code = 3, Language = "Lang" });
            var saveRows = dh.Save(new Model.CountryLanguageModel() { Percent = 1.555m, IsOfficial = false, Code = 2, Language = "Lang" },
               new Model.CountryLanguageModel() { Percent = 1.66m, IsOfficial = true, Code = 3, Language = "Lang" });
            var executeNoQuery = dh.ExecNoQuery("update countrylanguage set Percentage=4.4 where Percentage=@Percentage", new Model.CountryLanguageModel { Percent = 1.6m });
            var pageSql = dh.Driver.GetPagedSql("select * from countrylanguage", "CountryCode", true, 1, 10);
            var pageResult = dh.GetPagedResult<Model.CountryLanguageModel>("select * from countrylanguage", "countrycode", true, 1, 10);

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