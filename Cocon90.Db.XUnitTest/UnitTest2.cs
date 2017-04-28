using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cocon90.Db.Common.Data;

namespace Cocon90.Db.XUnitTest
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void GetDataHelperTest()
        {
            for (int i = 0; i < 100000; i++)
            {
                var dh = Cocon90.Db.Common.Db.GetDataHelper("Mysql", "Server=127.0.0.1;Port=3306;Database=DataRepair;Uid=root;Pwd=123456;");
            }
        }
        [TestMethod]
        public void GetOneTest()
        {
            //var dh = Cocon90.Db.Common.Db.GetDataHelper("Mysql", "Server=127.0.0.1;Port=3306;Database=DataRepair;Uid=root;Pwd=123456;");
            var dh = Cocon90.Db.Common.Db.GetDataHelper(DbTypeEnum.SqlServer, "Server=127.0.0.1;Database=DataRepair;Uid=sa;Pwd=123456;");
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
        public void InsertTest()
        {
            //var dh = Cocon90.Db.Common.Db.GetDataHelper("Mysql", "Server=127.0.0.1;Port=3306;Database=DataRepair;Uid=root;Pwd=123456;");
            var dh = Cocon90.Db.Common.Db.GetDataHelper(DbTypeEnum.SqlServer, "Server=.;Database=DataRepair;Uid=sa;Pwd=123456;");
            dh.CreateOrUpdateTable(typeof(Student));

            List<Student> lst = new List<Student>();
            for (int i = 0; i < 100000; i++)
            {
                lst.Add(new Student
                {
                    Age = i,
                    Birth = DateTime.UtcNow.AddHours(8),
                    Name = "Test" + i,
                    RowId = Guid.NewGuid()
                });
            }
            //var sql = dh.GetInsertSql(lst.ToArray());
            dh.Insert<Student>(lst.ToArray());
        }
        [TestMethod]
        public void GetListByWhereTest()
        {
            var dh = Cocon90.Db.Common.Db.GetDataHelper(DbTypeEnum.SqlServer, "Server=.;Database=DataRepair;Uid=sa;Pwd=123456;");
            //var dh = Cocon90.Db.Common.Db.GetDataHelper("Mysql", "Server=127.0.0.1;Port=3306;Database=DataRepair;Uid=root;Pwd=123456;");
            dh.CreateOrUpdateTable(typeof(Student));
            var lst = dh.GetListByWhere<Student>("Age=@age", new { age = 30 });
            var lst2 = dh.GetListByWhere<Student>("Age>@age", new { age = 30 });
            var lst3 = dh.GetListByWhere<Student>("CusName=@cusname", new { cusname = "Test9698" });

        }

        [TestMethod]
        public void GetAllListTest()
        {
            var dh = Cocon90.Db.Common.Db.GetDataHelper(DbTypeEnum.SqlServer, "Server=.;Database=DataRepair;Uid=sa;Pwd=123456;");
            //var dh = Cocon90.Db.Common.Db.GetDataHelper("Mysql", "Server=127.0.0.1;Port=3306;Database=DataRepair;Uid=root;Pwd=123456;");
            var lst4 = dh.GetList<Student>();
        }

        [TestMethod]
        public void GetGetObjectTest()
        {
            var dh = Cocon90.Db.Common.Db.GetDataHelper("Mysql", "Server=127.0.0.1;Port=3306;Database=DataRepair;Uid=root;Pwd=123456;");
            for (int i = 0; i < 10000; i++)
            {
                var lst4 = dh.GetInt("select age from student limit 1");
            }

        }
    }
}
