using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Pur.Purr200
{
    /// <summary>  
    /// 請購單單頭  
    /// </summary>  
    public class Master 
    {
        /// <summary>  
        /// 請購單單號  
        /// </summary> 
        public string pea01 { get; set; }
        /// <summary>  
        /// 請購日期  
        /// </summary> 
        public Nullable<System.DateTime> pea02 { get; set; }
        /// <summary>  
        /// 廠商編號  
        /// </summary> 
        public string pea03 { get; set; }
        /// <summary>  
        /// 請購人員  
        /// </summary> 
        public string pea04 { get; set; }
        /// <summary>  
        /// 請購部門  
        /// </summary> 
        public string pea05 { get; set; }
        /// <summary>  
        /// 課稅別  
        /// </summary> 
        public string pea06 { get; set; }
        /// <summary>  
        /// 稅率  
        /// </summary> 
        public decimal pea07 { get; set; }
        /// <summary>  
        /// 含稅否  
        /// </summary> 
        public string pea08 { get; set; }
        /// <summary>  
        /// 幣別  
        /// </summary> 
        public string pea10 { get; set; }
        /// <summary>  
        /// 付款條件  
        /// </summary> 
        public string pea11 { get; set; }
        /// <summary>  
        /// 取價條件  
        /// </summary> 
        public string pea12 { get; set; }
        /// <summary>  
        /// 公司別  
        /// </summary> 
        public string peacomp { get; set; }
        /// <summary>  
        /// 廠商名稱
        /// </summary> 
        public string pea03_c { get; set; }
        /// <summary>  
        /// 請購人員姓名
        /// </summary> 
        public string pea04_c { get; set; }
        /// <summary>  
        /// 請購部門名稱  
        /// </summary> 
        public string pea05_c { get; set; }
        /// <summary>  
        /// 付款條件
        /// </summary> 
        public string pea11_c { get; set; }
        /// <summary>  
        /// 取價條件
        /// </summary> 
        public string pea12_c { get; set; }
        /// <summary>  
        /// 單別名稱
        /// </summary> 
        public string pea01_c { get; set; }
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

    }

    /// <summary>  
    /// 請購單單身  
    /// </summary>  
    public class Detail
    {
        /// <summary>  
        /// 請購單號  
        /// </summary> 
        public string peb01 { get; set; }
        /// <summary>  
        /// 項次  
        /// </summary> 
        public decimal peb02 { get; set; }
        /// <summary>  
        /// 料號  
        /// </summary> 
        public string peb03 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string peb04 { get; set; }
        /// <summary>  
        /// 請購數量  
        /// </summary> 
        public decimal peb05 { get; set; }
        /// <summary>  
        /// 請購單位  
        /// </summary> 
        public string peb06 { get; set; }
        /// <summary>  
        /// 規格  
        /// </summary> 
        public string ica03 { get; set; }
        /// <summary>
        /// 單位的小數位數
        /// </summary>
        public decimal bej03 { get; set; }

        /// <summary>  
        /// 請購數量(字串)
        /// </summary> 
        public string peb05_str { get; set; }
    }
}
