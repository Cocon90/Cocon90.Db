using Cocon90.Db.Common;
using Cocon90.Db.Common.Attribute;
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
        Cocon90.Db.Common.IDataHelper dh { get { return Cocon90.Db.Common.Db.GetDataHelper(DbTypeEnum.SqlServer, "server=.;database=Test;uid=sa;pwd=123456;"); } }

        [TestMethod]
        public void TestInsert()
        {
            dh.CreateOrUpdateTable(typeof(Worker));
            var eff = dh.Insert<Worker>(new Worker() { N = "张三" + DateTime.Now.Second, Money = DateTime.Now.Millisecond / 3.0d, Age = DateTime.Now.Second });
            eff = dh.Insert<Worker>(new Worker() { N = "李四" + DateTime.Now.Second, Money = DateTime.Now.Millisecond / 3.0d, Age = DateTime.Now.Second });
        }

        [TestMethod]
        public void TestSelect()
        {
            dh.CreateOrUpdateTable(typeof(Worker));
            var tabs = dh.GetDataSet("select * from Worker where id=@id2", new Params("id2", 2));
            foreach (var item in tabs.Tables)
            {
                foreach (var row in item.Rows)
                {

                }
            }

            var money = dh.GetFloat("select money from worker where id=@id", new Params("id", 2));

            var namelst = dh.GetListString(true, "select Name from worker where money > @mymoney", new Params("mymoney", 0));

            var one = dh.GetOne<Worker>("select * from Worker where Id=1");

            var worker1 = dh.GetOneByPrimaryKey<Worker>(1);
            var worker2 = dh.GetOneByPrimaryKey<Worker>(2);
            var worker3 = dh.GetOneByWhere<Worker>("id=@id", new Params("id", 2));
            var worker4 = dh.GetOneByWhere<Worker>("id=@id", new { id = 2 });

            var worklst = dh.GetList<Worker>();

            var paged = dh.GetPagedResult<Worker>("select * from Worker", "id", true, 1, 20);
        }

        [TestMethod]
        public void TestSave()
        {

            var sql2 = dh.GetInsertIfNotExistPrimeryKeySql(new Worker { N = "张三" }, 1);
            var isok2 = dh.InsertIfNotExistPrimeryKey(new Worker { N = "张三" }, 1) > 0;
            var sql = dh.GetSaveSql(new Worker { N = "张三", Id = 1 });

            dh.CreateOrUpdateTable<Product>();
            // The Save method is not suitable for tables that contain the primary key in the auto-increment column.
            var isok = dh.Save(new Product { Name = "好东西", Id = Guid.Empty }) > 0;
        }
        [TestMethod]
        public void TestUpdate()
        {

            var sql = dh.GetUpdateSql(new Worker { N = "王五", Age = 3, Id = 2 }, true, "Name is not null");
            //Note that, in the condition occurs, id = 3 modified conditions will be ignored. Because the Update statement, no longer modify the primary key.
            var sql2 = dh.GetUpdateSqlByPrimaryKey(new Worker { N = "王五", Age = 3, Id = 3 }, true, "Name is not null", 2);
            var sql3 = dh.GetUpdateSqlByWhere(new Worker { N = "王五", Age = 3 }, true, "Name is not null AND ID=@id", new { id = 2 });
        }



    }
    public class Worker
    {
        [Column(PrimaryKey = true, CreateDDL = "int identity(1,1)")]
        public long? Id { get; set; }
        [Column(ColumnName = "Name")]
        public string N { get; set; }
        public int? Age { get; set; }
        public double? Money { get; set; }
    }
    public class Product
    {
        [Column(PrimaryKey = true)]
        public Guid? Id { get; set; }
        public string Name { get; set; }

    }
}
