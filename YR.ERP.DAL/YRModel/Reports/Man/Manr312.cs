using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Man.Manr312
{
    public class Master
    {
        /// <summary>  
        /// 單據類別  
        /// </summary> 
        public string mga00 { get; set; }
        /// <summary>  
        /// 單號  
        /// </summary> 
        public string mga01 { get; set; }
        /// <summary>  
        /// 退料日期  
        /// </summary> 
        public Nullable<System.DateTime> mga02 { get; set; }
        /// <summary>  
        /// 廠商  
        /// </summary> 
        public string mga03 { get; set; }
        /// <summary>  
        /// 經辦人  
        /// </summary> 
        public string mga04 { get; set; }
        /// <summary>  
        /// 經辦部門  
        /// </summary> 
        public string mga05 { get; set; }
        /// <summary>  
        /// 來源單號  
        /// </summary> 
        public string mga06 { get; set; }
        /// <summary>  
        /// 廠商地址碼  
        /// </summary> 
        public string mga07 { get; set; }
        /// <summary>  
        /// 備註  
        /// </summary> 
        public string mga08 { get; set; }

        /// <summary>  
        /// 單別名稱
        /// </summary> 
        public string mga01_c { get; set; }
        /// <summary>  
        /// 廠商名稱
        /// </summary> 
        public string mga03_c { get; set; }
        public string mga04_c { get; set; }
        public string mga05_c { get; set; }

        /// <summary>  
        /// 項次  
        /// </summary> 
        public decimal mgb02 { get; set; }
        /// <summary>  
        /// 子件料號  
        /// </summary> 
        public string mgb03 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string mgb04 { get; set; }
        /// <summary>  
        /// 數量  
        /// </summary> 
        public decimal mgb05 { get; set; }
        /// <summary>  
        /// 單位  
        /// </summary> 
        public string mgb06 { get; set; }
        /// <summary>  
        /// 出庫倉  
        /// </summary> 
        public string mgb07 { get; set; }
        /// <summary>  
        /// 備註  
        /// </summary> 
        public string mgb09 { get; set; }

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
        public string mgb05_str { get; set; }

        public string pcb03 { get; set; }
        public string pcb04 { get; set; }
        public string pcb06 { get; set; }
        public string pcb07 { get; set; }
    }
}
