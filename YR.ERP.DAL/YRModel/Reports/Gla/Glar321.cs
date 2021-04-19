using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Gla.Glar321
{
    public class Master
    {
        public string gfa01 { get; set; }
        public DateTime? gfa02 { get; set; }
        public decimal gfa03 { get; set; }
        public decimal gfa04 { get; set; }
        public string gfa05 { get; set; }
        public string gfa06 { get; set; }
        public string gfa07 { get; set; }
        public decimal gfa08 { get; set; }
        public decimal gfa09 { get; set; }

        public decimal gfb02 { get; set; }
        public string gfb03 { get; set; }
        public string gfb04 { get; set; }
        public string gfb05 { get; set; }
        public string gfb06 { get; set; }
        public decimal gfb07 { get; set; }
        public string gfb08 { get; set; }
        public decimal gfb09 { get; set; }
        public decimal gfb10 { get; set; }
        public string gfb03_c { get; set; }
        public string gfb05_c { get; set; }
        /// <summary>
        /// 原幣 小數位數
        /// </summary>
        public decimal bek04 { get; set; }      
        /// <summary>
        /// 借餘或貸餘科目 1.借 2.貸
        /// </summary>
        public string gba05 { get; set; }      

        public decimal gfb07c { get; set; }
        public decimal gfb07d { get; set; }
        public decimal gfb10c { get; set; }
        public decimal gfb10d { get; set; }

        public decimal gfb07c_str { get; set; }
        public decimal gfb07d_str { get; set; }
        public decimal gfb10c_str { get; set; }
        public decimal gfb10d_str { get; set; }

        public decimal gfb07_blance { get; set; }
        public decimal gfb10_blance { get; set; }

        public decimal gfb07_blance_str { get; set; }
        public decimal gfb10_blance_str { get; set; }
    }
}


