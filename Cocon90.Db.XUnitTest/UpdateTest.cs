using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cocon90.Db.Common.Data;

namespace Cocon90.Db.XUnitTest
{
    [TestClass]
    public class UpdateTest
    {
        [TestMethod]
        public void TestSqliteUpdate()
        {
            var dh = Cocon90.Db.Common.Db.GetDataHelper(DbTypeEnum.Sqlite, @"Data Source=D:\Applicaton\DbTools\sqlspy\World.db3;BinaryGUID=False");
            var id = Guid.NewGuid();
            var sql = dh.Insert<MyIndexTab>(new MyIndexTab()
            {
                AgeType = 20,
                NameType = "Song",
                RowId = id,
                UserType = UserType.GoodStudent
            });
            var eff = dh.Update<MyIndexTab>(new MyIndexTab
            {
                RowId = id,
                NameType = "xingzhu"
            });

        }
        [TestMethod]
        public void TestMsSqlUpdate()
        {
            var dh = Cocon90.Db.Common.Db.GetDataHelper(DbTypeEnum.SqlServer, @"Data Source=.;DataBase=test;Uid=sa;Pwd=123456;");
            dh.CreateOrUpdateTable<MyIndexTab>();
            var id = Guid.NewGuid();
            var sql = dh.Insert<MyIndexTab>(new MyIndexTab()
            {
                AgeType = 20,
                NameType = "Song",
                RowId = id,
                UserType = UserType.GoodStudent
            });
            var eff = dh.Update<MyIndexTab>(new MyIndexTab
            {
                RowId = id,
                NameType = "xingzhu"
            });

        }
    }
}
