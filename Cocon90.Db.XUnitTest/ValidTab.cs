using Cocon90.Db.Common.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocon90.Db.XUnitTest
{
    /// <summary>
    /// 验证失败的表
    /// </summary>
    public class ValidTab
    {
        [Column(PrimaryKey = true)]
        public Guid? RowId { get; set; }
        /// <summary>
        /// 是否是基础规则
        /// </summary>
        public bool? IsBaseRule { get; set; }
        /// <summary>
        /// 错误的原因
        /// </summary>
        public string ErrorReasion { get; set; }
        /// <summary>
        /// 验证时间
        /// </summary>
        public DateTime? ValidTime { get; set; }
        /// <summary>
        /// 验证失败的相应AlarmRuleTab的RowID 快照
        /// </summary>
        public Guid? AlarmRuleIdSnap { get; set; }
        /// <summary>
        /// 行政区域的名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 国内9位，国外9或不定位。
        /// </summary>
        public string AreaId { get; set; }
        /// <summary>
        /// 规则ID快照，(可能Rule表已被修改，这里只留备用)
        /// </summary>
        public Guid? RuleIdSnap { get; set; }
        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName { get; set; }
        /// <summary>
        /// ValiderType Name
        /// </summary>
        public string ValiderType { get; set; }
        /// <summary>
        /// 规则类型，如 Forecast12h，Forecast3h，Index...
        /// </summary>
        public int? RuleType { get; set; }
        /// <summary>
        /// 项类型，温度、还是风向、还是温度、还是天气状况...
        /// </summary>
        public string ItemType { get; set; }
        /// <summary>
        /// 项类型所在数据行中的索引。
        /// </summary>
        public int? ItemTypeIndex { get; set; }
        /// <summary>
        /// 项类型所对应的Xml类的文档中的XPath区分。
        /// </summary>
        public string ItemTypeXPath { get; set; }
        /// <summary>
        /// 告警验证代码
        /// </summary>
        public string AlarmValidCode { get; set; }
        /// <summary>
        /// 告警验证是否被触发
        /// </summary>
        public bool? AlarmValidTriggered { get; set; }
        /// <summary>
        /// 过滤条件验证代码
        /// </summary>
        public string FilterValidCode { get; set; }
        /// <summary>
        /// 过滤条件是否被触发。
        /// </summary>
        public bool? FilterValidTriggered { get; set; }
        /// <summary>
        /// 验证的文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 当前站点的StationID。
        /// </summary>
        public string StationId { get; set; }

        /// <summary>
        /// 验证状态码
        /// </summary>
        [Column(CreateDDL = "int not null default 0")]
        public int? Status { get; set; }
        /// <summary>
        /// 错误的数据行
        /// </summary>
        public string ErrorRowData { get; set; }
        /// <summary>
        /// 出错的行号
        /// </summary>
        public int? ErrorLineNumber { get; set; }
    }
}
