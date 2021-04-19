using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Inv.Invr521
{
    public class Master 
    {
        /// <summary>
        /// 盤點單號
        /// </summary>
        public string ipa01 { get; set; }
        /// <summary>
        /// 項次
        /// </summary>
        public decimal ipb02 { get; set; }
        /// <summary>
        /// 料號
        /// </summary>
        public string ipb03 { get; set; }
        /// <summary>
        /// 倉庫
        /// </summary>
        public string ipb04 { get; set; }
        /// <summary>
        /// 儲位
        /// </summary>
        public string ipb05 { get; set; }
        /// <summary>
        /// 庫存量
        /// </summary>
        public decimal ipb06 { get; set; }
        /// <summary>
        /// 庫存單位
        /// </summary>
        public string ipb07 { get; set; }
        /// <summary>
        /// 初盤數量
        /// </summary>
        public decimal ipb30 { get; set; }
        /// <summary>
        /// 複盤數量
        /// </summary>
        public decimal ipb40 { get; set; }
        /// <summary>
        /// 監盤數量
        /// </summary>
        public decimal ipb50 { get; set; }

        /// <summary>
        /// 單位-小數位數
        /// </summary>
        public decimal bej03 { get; set; }

        public string ica02 { get; set; }
        public string ica03 { get; set; }

        public decimal qty1 { get; set; }
        public decimal qty2 { get; set; }
        public decimal dif_qty { get; set; }


        public string qty1_str { get; set; }
        public string qty2_str { get; set; }
        public string dif_qty_str { get; set; }
    }
}
