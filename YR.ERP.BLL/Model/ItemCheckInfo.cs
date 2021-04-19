using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.BLL.Model
{    
    public class ItemCheckInfo
    {
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string Column { get; set; }
        /// <summary>
        /// 欄位輸入值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 驗證的資料列 DataRow
        /// </summary>
        public DataRow Row { get; set; }
    }
}
