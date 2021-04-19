using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Pur.Purr400
{
    public class Master : YR.ERP.DAL.YRModel.pga_tb
    {
        /// <summary>  
        /// 廠商名稱
        /// </summary> 
        public string pga03_c { get; set; }
        /// <summary>  
        /// 入庫人員姓名
        /// </summary> 
        public string pga04_c { get; set; }
        /// <summary>  
        /// 入庫部門名稱  
        /// </summary> 
        public string pga05_c { get; set; }
        /// <summary>  
        /// 付款條件
        /// </summary> 
        public string pga11_c { get; set; }
        /// <summary>  
        /// 取價條件
        /// </summary> 
        public string pga12_c { get; set; }
        /// <summary>  
        /// 單別名稱
        /// </summary> 
        public string pga01_c { get; set; }

        public string pga14_c { get; set; }
        public string pga15_c { get; set; }
        public string pga13_str { get; set; }
        public string pga13t_str { get; set; }
        public string pga13g_str { get; set; }
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
        public decimal pgb02 { get; set; }
        /// <summary>  
        /// 料號  
        /// </summary> 
        public string pgb03 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string pgb04 { get; set; }
        /// <summary>  
        /// 入庫數量  
        /// </summary> 
        public decimal pgb05 { get; set; }
        /// <summary>  
        /// 入庫單位  
        /// </summary> 
        public string pgb06 { get; set; }

        /// <summary>  
        /// 單價  
        /// </summary> 
        public decimal pgb09 { get; set; }
        /// <summary>  
        /// 未稅金額  
        /// </summary> 
        public decimal pgb10 { get; set; }
        /// <summary>  
        /// 含稅金額  
        /// </summary> 
        public decimal pgb10t { get; set; }
        public DateTime? pgb18 { get; set; }
        public DateTime? pgb19 { get; set; }

        /// <summary>  
        /// 規格  
        /// </summary> 
        public string ica03 { get; set; }
        /// <summary>
        /// 單位的小數位數
        /// </summary>
        public decimal bej03 { get; set; }

        /// <summary>  
        /// 入庫數量(字串)
        /// </summary> 
        public string pgb05_str { get; set; }
        /// <summary>  
        /// 單價(字串)
        /// </summary> 
        public string pgb09_str { get; set; }
        /// <summary>  
        /// 未稅金額(字串)
        /// </summary> 
        public string pgb10_str { get; set; }
        /// <summary>  
        /// 含稅金額(字串)
        /// </summary> 
        public string pgb10t_str { get; set; }

    }
}
