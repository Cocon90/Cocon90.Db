using Cocon90.Db.Common.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Demo.Model
{
    [Table(TableName = "CountryLanguage")]
    public class CountryLanguageTab
    {
        [Column(PrimaryKey = true, ColumnName = "CountryCode")]
        public int? Code { get; set; }
        [Column(PrimaryKey = true, CreateDDL = "varchar(20)")]
        public string Language { get; set; }
        public bool? IsOfficial { get; set; }
        [Column(ColumnName = "Percentage", PrimaryKey = false)]
        public decimal? Percent { get; set; }
        public DateTime? Date { get; set; }
        public Guid? Guid { get; set; }
        public DateTimeOffset? MyDateOff { get; set; }
    }

    [Table(TableName = "CountryLanguage")]
    public class CountryLanguageModel : CountryLanguageTab
    {
        [Ignore]
        public string CodeAndLang { get; set; }
    }
}
