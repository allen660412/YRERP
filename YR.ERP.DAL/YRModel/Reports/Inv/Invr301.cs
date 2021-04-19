using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Inv.Invr301
{
    public class Master : YR.ERP.DAL.YRModel.iga_tb
    {
        /// <summary>  
        /// 申請人員姓名
        /// </summary> 
        public string iga03_c { get; set; }
        /// <summary>  
        /// 申請部門名稱  
        /// </summary> 
        public string iga04_c { get; set; }
        public string iga01_c { get; set; }

        /// <summary>  
        /// 項次  
        /// </summary> 
        public decimal igb02 { get; set; }
        /// <summary>  
        /// 料號  
        /// </summary> 
        public string igb03 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string igb04 { get; set; }

        /// <summary>  
        /// 異動數量  
        /// </summary> 
        public decimal igb05 { get; set; }
        /// <summary>  
        /// 撥出單位
        /// </summary> 
        public string igb06 { get; set; }

        /// <summary>  
        /// 異動倉
        /// </summary> 
        public string igb09 { get; set; }
        /// <summary>  
        /// 備註  
        /// </summary> 
        public string igb10 { get; set; }


        /// <summary>  
        /// 規格  
        /// </summary> 
        public string ica03 { get; set; }

        /// <summary>  
        /// 撥出數量(字串)
        /// </summary> 
        public string igb05_str { get; set; }

    }
}
