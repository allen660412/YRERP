using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Pur.Purr410
{
    public class MasterA
    {
        public string type { get; set; }
        public string pga01 { get; set; }
        public DateTime pga02 { get; set; }
        public string pga03 { get; set; }
        /// <summary>
        /// 廠商簡稱
        /// </summary>
        public string pga03_c { get; set; }
        public string pga04 { get; set; }
        /// <summary>
        /// 業務人員
        /// </summary>
        public string pga04_c { get; set; }
        public string pga05 { get; set; }
        /// <summary>
        /// 業務部門
        /// </summary>
        public string pga05_c { get; set; }
        public string pga10 { get; set; }

        public decimal pgb02 { get; set; }
        public string pgb03 { get; set; }
        public string pgb04 { get; set; }
        public decimal pgb05 { get; set; }
        public string pgb05_str { get; set; }
        public string pgb06 { get; set; }
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
        public decimal pgb10 { get; set; }
        public string pgb10_string { get; set; }
        public string pgb16 { get; set; }
    }
}
