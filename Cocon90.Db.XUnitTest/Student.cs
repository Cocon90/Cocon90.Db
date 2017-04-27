using Cocon90.Db.Common.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocon90.Db.XUnitTest
{
    [Table()]
    public class Student
    {
        [Column(PrimaryKey = true)]
        public Guid? RowId { get; set; }
        [Column(ColumnName = "cusName")]
        public string Name { get; set; }
        public int? Age { get; set; }
    }
}
