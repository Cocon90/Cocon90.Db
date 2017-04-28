using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cocon90.Db.XUnitTest
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod()
        {
            var dh = Cocon90.Db.Common.Db.GetDataHelper("Mysql", "Server=127.0.0.1;Port=3306;Database=DataRepair;Uid=root;Pwd=123456;");
            for (int i = 0; i < 10000; i++)
            {
                //var dt = dh.GetList<ValidTab>("select * from validtab limit 10000");
                //var sql = dh.GetInsertSql(new ValidTab
                //{
                //    AlarmRuleIdSnap = Guid.NewGuid(),
                //    AlarmValidCode = "asdffasdf",
                //    AreaId = "asdfasf",
                //    RowId = Guid.NewGuid(),
                //    ErrorLineNumber = i,
                //    FileName = "asdfasdf"
                //});
                var one = dh.GetOne<ValidTab>("select * from validtab limit 1");
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            var dh = Cocon90.Db.Common.Db.GetDataHelper("Mysql", "Server=127.0.0.1;Port=3306;Database=DataRepair;Uid=root;Pwd=123456;");
            dh.CreateOrUpdateTable(typeof(Student));

            List<Student> lst = new List<Student>();
            for (int i = 0; i < 100000; i++)
            {
                lst.Add(new Student
                {
                    Age = i,
                    Birth = DateTime.UtcNow.AddHours(8),
                    //Name = "Test" + i,
                    RowId = Guid.NewGuid()
                });
            }
            //var sql = dh.GetInsertSql(lst.ToArray());
            dh.Insert<Student>(lst.ToArray());
        }
        [TestMethod]
        public void TestMethod3()
        {
            var dh = Cocon90.Db.Common.Db.GetDataHelper("Mysql", "Server=127.0.0.1;Port=3306;Database=DataRepair;Uid=root;Pwd=123456;");
            dh.CreateOrUpdateTable(typeof(Student));
            var lst = dh.GetListByWhere<Student>("Age=@age", new { age = 30 });
            var lst2 = dh.GetListByWhere<Student>("Age>@age", new { age = 30 });
            var lst3 = dh.GetListByWhere<Student>("CusName=@cusname", new { cusname = "Test9698" });
           
        }

        [TestMethod]
        public void GetAllListTest()
        {
            var dh = Cocon90.Db.Common.Db.GetDataHelper("Mysql", "Server=127.0.0.1;Port=3306;Database=DataRepair;Uid=root;Pwd=123456;");
            var lst4 = dh.GetList<Student>();
        }
    }
}
