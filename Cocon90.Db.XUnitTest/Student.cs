using Cocon90.Db.Common.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocon90.Db.XUnitTest
{
    public class Student
    {
        [Column(PrimaryKey = true)]
        public Guid? RowId { get; set; }
        [Column(ColumnName = "CusName")]
        public string Name { get; set; }
        public int? Age { get; set; }
        public DateTime? Birth { get; set; }
        public string BirthString { get { return Birth?.ToString("yyyy年MM月dd日"); } }

    }
}
