using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Cocon90.Db.Common.Tools;

namespace Cocon90.Db.XUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dics = AttributeHelper.GetColumn2PropNameDics(Common.Data.DirverType.MySql, typeof(Student));
            var dic2s = AttributeHelper.GetProp2ColumnNameDics(Common.Data.DirverType.MySql, typeof(Student));
        }
    }

}
