using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

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
    }
}
