using System;
using Xunit;

namespace Cocon90.Core.Db.XUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var dh = Cocon90.Db.Common.Db.GetDataHelper();
            var createSql = dh.GetCreateTableSql<Model.CountryLanguageModel>();
            var updateTabSql = dh.GetUpdateTableSql(typeof(Model.CountryLanguageModel));
            var effRow = dh.CreateOrUpdateTable<Model.CountryLanguageModel>();
            Console.WriteLine(createSql);
            Console.WriteLine(updateTabSql);
            Console.WriteLine(effRow);
            Assert.True(effRow > 0);
        }
    }
}
