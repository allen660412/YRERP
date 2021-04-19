using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Man.Manr311
{
    public class Master
    {
        /// <summary>  
        /// 單據類別  
        /// </summary> 
        public string mfa00 { get; set; }
        /// <summary>  
        /// 單號  
        /// </summary> 
        public string mfa01 { get; set; }
        /// <summary>  
        /// 出庫日期  
        /// </summary> 
        public Nullable<System.DateTime> mfa02 { get; set; }
        /// <summary>  
        /// 廠商  
        /// </summary> 
        public string mfa03 { get; set; }
        /// <summary>  
        /// 經辦人  
        /// </summary> 
        public string mfa04 { get; set; }
        /// <summary>  
        /// 經辦部門  
        /// </summary> 
        public string mfa05 { get; set; }
        /// <summary>  
        /// 來源單號  
        /// </summary> 
        public string mfa06 { get; set; }
        /// <summary>  
        /// 廠商地址碼  
        /// </summary> 
        public string mfa07 { get; set; }
        /// <summary>  
        /// 備註  
        /// </summary> 
        public string mfa08 { get; set; }
        
        /// <summary>  
        /// 單別名稱
        /// </summary> 
        public string mfa01_c { get; set; }
        /// <summary>  
        /// 廠商名稱
        /// </summary> 
        public string mfa03_c { get; set; }
        public string mfa04_c { get; set; }
        public string mfa05_c { get; set; }

        /// <summary>  
        /// 項次  
        /// </summary> 
        public decimal mfb02 { get; set; }
        /// <summary>  
        /// 子件料號  
        /// </summary> 
        public string mfb03 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string mfb04 { get; set; }
        /// <summary>  
        /// 數量  
        /// </summary> 
        public decimal mfb05 { get; set; }
        /// <summary>  
        /// 單位  
        /// </summary> 
        public string mfb06 { get; set; }
        /// <summary>  
        /// 出庫倉  
        /// </summary> 
        public string mfb07 { get; set; }
        /// <summary>  
        /// 備註  
        /// </summary> 
        public string mfb09 { get; set; }

        /// <summary>  
        /// 規格  
        /// </summary> 
        public string ica03 { get; set; }

        /// <summary>
        /// 單位的小數位數
        /// </summary>
        public decimal bej03 { get; set; }

        /// <summary>  
        /// 入庫數量(字串)
        /// </summary> 
        public string mfb05_str { get; set; }

        public string pcb03 { get; set; }
        public string pcb04 { get; set; }
        public string pcb06 { get; set; }
        public string pcb07 { get; set; }
    }
}
