using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Stp.Stpr410
{
    public class MasterA
    {
        public string sale_type { get; set; }
        public string sga01 { get; set; }
        public DateTime sga02 { get; set; }
        public string sga03 { get; set; }
        /// <summary>
        /// 客戶簡稱
        /// </summary>
        public string sga03_c { get; set; }
        public string sga04 { get; set; }
        /// <summary>
        /// 業務人員
        /// </summary>
        public string sga04_c { get; set; }
        public string sga05 { get; set; }
        /// <summary>
        /// 業務部門
        /// </summary>
        public string sga05_c { get; set; }
        public string sga10 { get; set; }
        /// <summary>
        /// 發票===>匯率
        /// </summary>
        public decimal sga21 { get; set; }
        public decimal sgb02 { get; set; }
        public string sgb03 { get; set; }
        public string sgb04 { get; set; }
        public decimal sgb05 { get; set; }
        public string sgb05_str { get; set; }
        public string sgb06 { get; set; }
        //單位小數位數
        public decimal bej03 { get; set; }
        /// <summary>
        /// 未稅單價
        /// </summary>
        public decimal price { get; set; }
        public string price_string { get; set; }  

        /// <summary>
        /// 未稅小計
        /// </summary>
        public decimal sgb10 { get; set; }
        public string sgb10_string { get; set; }


        /// <summary>
        /// 含稅小計
        /// </summary>
        public decimal sgb10t { get; set; }
        public string sgb10t_string { get; set; }
        public string sgb16 { get; set; }
    }
}
