using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Gla.Glar300
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
        public decimal gfaprno { get; set; }
        public string gfaconf { get; set; }
        public DateTime? gfacond { get; set; }
        public string gfaconu { get; set; }
        public string gfapost { get; set; }
        public DateTime? gfaposd { get; set; }
        public string gfaposu { get; set; }
        public string gfa01_c { get; set; }
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
        public decimal bek04 { get; set; }
        public string gba05 { get; set; }

        public string gfa03_str { get; set; }
        public string gfa04_str { get; set; }
        public string gfb07c_str { get; set; }
        public string gfb07d_str { get; set; }
        public string gfb10c_str { get; set; }
        public string gfb10d_str { get; set; }

        /// <summary>
        /// 每個group 重新刷頁
        /// </summary>
        public int groupPageNo { get; set; }
        /// <summary>
        /// 每個group的總頁數
        /// </summary>
        public int groupTotalPages { get; set; }

        public int groupSeqNo { get; set; }
        public bool isPageEnd { get; set; }
    }
}
