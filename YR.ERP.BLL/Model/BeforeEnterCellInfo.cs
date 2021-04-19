using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.BLL.Model
{
    /// <summary>
    /// Grid輸入時,用來傳遞物件用的類別
    /// </summary>
    public class BeforeEnterCellInfo
    {
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string Column { get; set; }

        public UltraGridCell Cell { get; set; }

        /// <summary>
        /// 欄位目目前資料
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 驗證的資料列 DataRow
        /// </summary>
        public DataRow Row { get; set; }

        public UltraGridRow GridRow {get; set;}

        /// <summary>
        /// 是否要中斷 active後續
        /// </summary>
        public bool Cancel {get;set;}
    }
}
