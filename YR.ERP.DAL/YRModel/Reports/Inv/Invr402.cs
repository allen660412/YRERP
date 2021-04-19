using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Inv.Invr402
{
    public class Master : YR.ERP.DAL.YRModel.ima_tb
    {
        /// <summary>  
        /// 客戶名稱
        /// </summary> 
        public string ima03_c { get; set; }
        /// <summary>  
        /// 業務人員姓名
        /// </summary> 
        public string ima04_c { get; set; }
        /// <summary>  
        /// 業務部門名稱  
        /// </summary> 
        public string ima05_c { get; set; }
        public string ima01_c { get; set; }

        /// <summary>  
        /// 項次  
        /// </summary> 
        public decimal imb02 { get; set; }
        /// <summary>  
        /// 料號  
        /// </summary> 
        public string imb03 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string imb04 { get; set; }
        /// <summary>  
        /// 歸還數量  
        /// </summary> 
        public decimal imb05 { get; set; }
        /// <summary>  
        /// 借出單位  
        /// </summary> 
        public string imb06 { get; set; }
        /// <summary>  
        /// 借出單號  
        /// </summary> 
        public string imb11 { get; set; }

        /// <summary>  
        /// 借出項次  
        /// </summary> 
        public decimal? imb12 { get; set; }
        /// <summary>  
        /// 備註  
        /// </summary> 
        public string imb17 { get; set; }
        /// <summary>  
        /// 出庫倉  
        /// </summary> 
        public string imb18 { get; set; }
        /// <summary>  
        /// 入庫倉  
        /// </summary> 
        public string imb19 { get; set; }

        /// <summary>  
        /// 規格  
        /// </summary> 
        public string ica03 { get; set; }
        /// <summary>
        /// 單位的小數位數
        /// </summary>
        public decimal bej03 { get; set; }

        /// <summary>  
        /// 歸還數量(字串)
        /// </summary> 
        public string imb05_str { get; set; }

    }
}
