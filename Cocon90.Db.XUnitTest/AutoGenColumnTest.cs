using Cocon90.Db.Common;
using Cocon90.Db.Common.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocon90.Db.XUnitTest
{
    [TestClass]
    public class AutoGenColumnTest
    {

        [TestMethod]
        public void TestInsert()
        {
            var dh = Cocon90.Db.Common.Db.GetDataHelper(DbTypeEnum.SqlServer, "server=.;database=Test;uid=sa;pwd=123456;");

        }
    }
}
