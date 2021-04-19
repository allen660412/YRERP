using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Inv.Invr410
{
    public class Master
    {
        public string ila01 { get; set; }
        public DateTime ila02 { get; set; }
        public string ila03 { get; set; }
        /// <summary>
        /// 客戶簡稱
        /// </summary>
        public string ila03_c { get; set; }
        public string ila04 { get; set; }
        /// <summary>
        /// 業務人員
        /// </summary>
        public string ila04_c { get; set; }
        public string ila05 { get; set; }
        /// <summary>
        /// 業務部門
        /// </summary>
        public string ila05_c { get; set; }
        public decimal ilb02 { get; set; }
        public string ilb03 { get; set; }
        public string ilb04 { get; set; }
        public decimal ilb05 { get; set; }
        public string ilb05_str { get; set; }
        public string ilb06 { get; set; }
        //單位小數位數
        public decimal bej03 { get; set; }
        public string ilb11 { get; set; }

        public decimal ilb15 { get; set; }
        public string ilb15_str { get; set; }
    }
}
