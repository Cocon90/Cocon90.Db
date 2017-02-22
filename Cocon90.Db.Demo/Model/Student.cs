using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Demo.Model
{
    public class Student
    {
        [Cocon90.Db.Common.Attribute.Column(PrimaryKey = true)]
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Addrss { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
