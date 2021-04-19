using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Inv.Invr337
{
    public class Master
    {
        public string ifa01 { get; set; }
        public DateTime ifa02 { get; set; }
        public string ifa03 { get; set; }
        /// <summary>
        /// 客戶簡稱
        /// </summary>
        public string ifa03_c { get; set; }
        public string ifa04 { get; set; }
        /// <summary>
        /// 業務人員
        /// </summary>
        public string ifa04_c { get; set; }
        public string ifa05 { get; set; }
        public string ifa06 { get; set; }
        /// <summary>
        /// 撥入確認否
        /// </summary>
        public string ifa09 { get; set; }
        public string ifaconf { get; set; }
        /// <summary>
        /// 業務部門
        /// </summary>
        public decimal ifb02 { get; set; }
        public string ifb03 { get; set; }
        public string ifb04 { get; set; }
        public decimal ifb05 { get; set; }
        public string ifb05_str { get; set; }
        public string ifb06 { get; set; }
        //單位小數位數
        public decimal bej03 { get; set; }
        public string ifb07 { get; set; }
        public decimal ifb08 { get; set; }
        public string ifb08_str { get; set; }
        public string ifb09 { get; set; }
        public string ifb10 { get; set; }
        /// <summary>
        /// 規格
        /// </summary>
        public string ica03 { get; set; }


    }
}
