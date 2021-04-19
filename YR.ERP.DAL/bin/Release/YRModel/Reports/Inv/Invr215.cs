using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Inv.Invr215
{
    public class Master 
    {
        public string icc01 { get; set; }
        public string icc02 { get; set; }
        public string icc03 { get; set; }
        public decimal icc05 { get; set; }

        /// <summary>  
        /// 品名
        /// </summary> 
        public string ica02 { get; set; }
        /// <summary>  
        /// 規格  
        /// </summary> 
        public string ica03 { get; set; }
        /// <summary>
        /// 庫存單位
        /// </summary>
        public string ica07 { get; set; }

        /// <summary>
        /// 單位的小數位數
        /// </summary>
        public decimal bej03 { get; set; }

        /// <summary>  
        /// 歸還數量(字串)
        /// </summary> 
        public string icc05_str { get; set; }

    }
}
