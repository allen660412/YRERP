using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.Base.Model
{
    /// <summary>
    /// 一般查詢使用的Model
    /// </summary>
    public class QueryInfo
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnDesc { get; set; }
        public string ColumnType { get; set; }
        public object Value { get; set; }
    }
}
