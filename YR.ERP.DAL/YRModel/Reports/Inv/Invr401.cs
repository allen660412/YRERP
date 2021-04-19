using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Inv.Invr401
{
    public class Master : YR.ERP.DAL.YRModel.ila_tb
    {
        /// <summary>  
        /// 客戶名稱
        /// </summary> 
        public string ila03_c { get; set; }
        /// <summary>  
        /// 業務人員姓名
        /// </summary> 
        public string ila04_c { get; set; }
        /// <summary>  
        /// 業務部門名稱  
        /// </summary> 
        public string ila05_c { get; set; }
        public string ila01_c { get; set; }

        /// <summary>
        /// 貨運方式
        /// </summary>
        public string ila14_c { get; set; }
        /// <summary>
        /// 起運地
        /// </summary>
        public string ila15_c { get; set; }
        /// <summary>
        /// 到達地
        /// </summary>
        public string ila16_c { get; set; }

        /// <summary>  
        /// 項次  
        /// </summary> 
        public decimal ilb02 { get; set; }
        /// <summary>  
        /// 料號  
        /// </summary> 
        public string ilb03 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string ilb04 { get; set; }
        /// <summary>  
        /// 借出數量  
        /// </summary> 
        public decimal ilb05 { get; set; }
        /// <summary>  
        /// 借出單位  
        /// </summary> 
        public string ilb06 { get; set; }
        /// <summary>  
        /// 借出倉  
        /// </summary> 
        public string ilb11 { get; set; }
        /// <summary>  
        /// 預計歸還日期  
        /// </summary> 
        public DateTime? ilb13 { get; set; }

        /// <summary>  
        /// 規格  
        /// </summary> 
        public string ica03 { get; set; }
        /// <summary>
        /// 單位的小數位數
        /// </summary>
        public decimal bej03 { get; set; }

        /// <summary>  
        /// 借出數量(字串)
        /// </summary> 
        public string ilb05_str { get; set; }

    }
}
