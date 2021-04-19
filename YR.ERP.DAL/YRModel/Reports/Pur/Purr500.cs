using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Pur.Purr500
{
    public class Master : YR.ERP.DAL.YRModel.pha_tb
    {
        /// <summary>  
        /// 廠商名稱
        /// </summary> 
        public string pha03_c { get; set; }
        /// <summary>  
        /// 退回人員姓名
        /// </summary> 
        public string pha04_c { get; set; }
        /// <summary>  
        /// 退回部門名稱  
        /// </summary> 
        public string pha05_c { get; set; }
        ///// <summary>  
        ///// 付款條件
        ///// </summary> 
        //public string pha11_c { get; set; }
        ///// <summary>  
        ///// 取價條件
        ///// </summary> 
        //public string pha12_c { get; set; }
        /// <summary>  
        /// 單別名稱
        /// </summary> 
        public string pha01_c { get; set; }

        //public string pha14_c { get; set; }
        //public string pha15_c { get; set; }
        public string pha13_str { get; set; }
        public string pha13t_str { get; set; }
        public string pha13g_str { get; set; }
        /// <summary>
        /// 公司住址
        /// </summary>
        public string bea03 { get; set; }
        /// <summary>
        /// 公司電話
        /// </summary>
        public string bea04 { get; set; }
        /// <summary>
        /// 公司傳真
        /// </summary>
        public string bea05 { get; set; }

        /// <summary>  
        /// 項次  
        /// </summary> 
        public decimal phb02 { get; set; }
        /// <summary>  
        /// 料號  
        /// </summary> 
        public string phb03 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string phb04 { get; set; }
        /// <summary>  
        /// 退回數量  
        /// </summary> 
        public decimal phb05 { get; set; }
        /// <summary>  
        /// 退回單位  
        /// </summary> 
        public string phb06 { get; set; }

        /// <summary>  
        /// 單價  
        /// </summary> 
        public decimal phb09 { get; set; }
        /// <summary>  
        /// 未稅金額  
        /// </summary> 
        public decimal phb10 { get; set; }
        /// <summary>  
        /// 含稅金額  
        /// </summary> 
        public decimal phb10t { get; set; }
        /// <summary>
        /// 倉庫別
        /// </summary>
        public string phb16 { get; set; }
        /// <summary>
        /// 退回類型
        /// </summary>
        public string phb17 { get; set; }
        public DateTime? phb18 { get; set; }
        public DateTime? phb19 { get; set; }

        /// <summary>  
        /// 規格  
        /// </summary> 
        public string ica03 { get; set; }
        /// <summary>
        /// 單位的小數位數
        /// </summary>
        public decimal bej03 { get; set; }

        /// <summary>  
        /// 退回數量(字串)
        /// </summary> 
        public string phb05_str { get; set; }
        /// <summary>  
        /// 單價(字串)
        /// </summary> 
        public string phb09_str { get; set; }
        /// <summary>  
        /// 未稅金額(字串)
        /// </summary> 
        public string phb10_str { get; set; }
        /// <summary>  
        /// 含稅金額(字串)
        /// </summary> 
        public string phb10t_str { get; set; }

    }
}
