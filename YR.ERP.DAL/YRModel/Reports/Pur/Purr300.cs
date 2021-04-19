using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Pur.Purr300
{
    public class Master:YR.ERP.DAL.YRModel.pfa_tb
    {
        /// <summary>  
        /// 廠商名稱
        /// </summary> 
        public string pfa03_c { get; set; }
        /// <summary>  
        /// 採購人員姓名
        /// </summary> 
        public string pfa04_c { get; set; }
        /// <summary>  
        /// 採購部門名稱  
        /// </summary> 
        public string pfa05_c { get; set; }
        /// <summary>  
        /// 付款條件
        /// </summary> 
        public string pfa11_c { get; set; }
        /// <summary>  
        /// 取價條件
        /// </summary> 
        public string pfa12_c { get; set; }
        /// <summary>  
        /// 單別名稱
        /// </summary> 
        public string pfa01_c { get; set; }

        public string pfa14_c { get; set; }
        public string pfa15_c { get; set; }
        public string pfa13_str { get; set; }
        public string pfa13t_str { get; set; }
        public string pfa13g_str { get; set; }
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
        public decimal pfb02 { get; set; }
        /// <summary>  
        /// 料號  
        /// </summary> 
        public string pfb03 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string pfb04 { get; set; }
        /// <summary>  
        /// 採購數量  
        /// </summary> 
        public decimal pfb05 { get; set; }
        /// <summary>  
        /// 採購單位  
        /// </summary> 
        public string pfb06 { get; set; }

        /// <summary>  
        /// 單價  
        /// </summary> 
        public decimal pfb09 { get; set; }
        /// <summary>  
        /// 未稅金額  
        /// </summary> 
        public decimal pfb10 { get; set; }
        /// <summary>  
        /// 含稅金額  
        /// </summary> 
        public decimal pfb10t { get; set; }
        public DateTime? pfb18 { get; set; }
        public DateTime? pfb19 { get; set; }

        /// <summary>  
        /// 規格  
        /// </summary> 
        public string ica03 { get; set; }
        /// <summary>
        /// 單位的小數位數
        /// </summary>
        public decimal bej03 { get; set; }

        /// <summary>  
        /// 採購數量(字串)
        /// </summary> 
        public string pfb05_str { get; set; }
        /// <summary>  
        /// 單價(字串)
        /// </summary> 
        public string pfb09_str { get; set; }
        /// <summary>  
        /// 未稅金額(字串)
        /// </summary> 
        public string pfb10_str { get; set; }
        /// <summary>  
        /// 含稅金額(字串)
        /// </summary> 
        public string pfb10t_str { get; set; }

    }
}
