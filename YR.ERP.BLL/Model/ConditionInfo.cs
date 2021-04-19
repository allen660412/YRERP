using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.BLL.Model
{
    /// <summary>
    /// 供AdvanceQueryInfo 裝載運算式使用
    /// </summary>
    public class ConditionInfo
    {
        public int id { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnDesc { get; set; }
        public string Condition { get; set; }
        public string AndOr { get; set; }
        public string ColumnType { get; set; }  //DBTYPE
        public object Value { get; set; }
        public decimal? orderSeq { get; set; }        
    }
}
