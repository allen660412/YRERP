using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Inv.Invr330
{
    public class Master : YR.ERP.DAL.YRModel.ifa_tb
    {
        /// <summary>  
        /// 申請人員姓名
        /// </summary> 
        public string ifa03_c { get; set; }
        /// <summary>  
        /// 申請部門名稱  
        /// </summary> 
        public string ifa04_c { get; set; }
        public string ifa01_c { get; set; }

        /// <summary>  
        /// 項次  
        /// </summary> 
        public decimal ifb02 { get; set; }
        /// <summary>  
        /// 料號  
        /// </summary> 
        public string ifb03 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string ifb04 { get; set; }

        /// <summary>  
        /// 撥出數量  
        /// </summary> 
        public decimal ifb05 { get; set; }
        /// <summary>  
        /// 撥出單位
        /// </summary> 
        public string ifb06 { get; set; }
        /// <summary>  
        /// 撥出倉  
        /// </summary> 
        public string ifb07 { get; set; }

        /// <summary>  
        /// 撥入數量  
        /// </summary> 
        public decimal ifb08 { get; set; }
        /// <summary>  
        /// 撥入單位
        /// </summary> 
        public string ifb09 { get; set; }
        /// <summary>  
        /// 撥入倉  
        /// </summary> 
        public string ifb10 { get; set; }


        /// <summary>  
        /// 規格  
        /// </summary> 
        public string ica03 { get; set; }

        /// <summary>  
        /// 撥出數量(字串)
        /// </summary> 
        public string ifb05_str { get; set; }

        /// <summary>  
        /// 撥入數量(字串)
        /// </summary> 
        public string ifb08_str { get; set; }

    }
}
