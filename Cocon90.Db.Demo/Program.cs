using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace Cocon90.Db.Demo
{
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
}
