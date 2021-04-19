using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Man.Manr411
{
    public class Master
    {
        /// <summary>  
        /// 單據類別  
        /// </summary> 
        public string mha00 { get; set; }
        /// <summary>  
        /// 單號  
        /// </summary> 
        public string mha01 { get; set; }
        /// <summary>  
        /// 入庫日期  
        /// </summary> 
        public Nullable<System.DateTime> mha02 { get; set; }
        /// <summary>  
        /// 廠商編號  
        /// </summary> 
        public string mha03 { get; set; }
        /// <summary>  
        /// 經辦人員  
        /// </summary> 
        public string mha04 { get; set; }
        /// <summary>  
        /// 經辦部門  
        /// </summary> 
        public string mha05 { get; set; }
        /// <summary>  
        /// 課稅別  
        /// </summary> 
        public string mha06 { get; set; }
        /// <summary>  
        /// 稅率  
        /// </summary> 
        public decimal mha07 { get; set; }
        /// <summary>  
        /// 含稅否  
        /// </summary> 
        public string mha08 { get; set; }
        /// <summary>  
        /// 發票聯數  
        /// </summary> 
        public string mha09 { get; set; }
        /// <summary>  
        /// 幣別  
        /// </summary> 
        public string mha10 { get; set; }
        /// <summary>  
        /// 付款條件  
        /// </summary> 
        public string mha11 { get; set; }
        /// <summary>  
        /// 保留  
        /// </summary> 
        public string mha12 { get; set; }
        /// <summary>  
        /// 未稅金額  
        /// </summary> 
        public decimal mha13 { get; set; }
        /// <summary>  
        /// 含稅金額  
        /// </summary> 
        public decimal mha13t { get; set; }
        /// <summary>  
        /// 稅額  
        /// </summary> 
        public decimal mha13g { get; set; }
        /// <summary>  
        /// 備註  
        /// </summary> 
        public string mha14 { get; set; }

        /// <summary>  
        /// 單別名稱
        /// </summary> 
        public string mha01_c { get; set; }
        /// <summary>  
        /// 廠商名稱
        /// </summary> 
        public string mha03_c { get; set; }
        /// <summary>  
        /// 入庫人員姓名
        /// </summary> 
        public string mha04_c { get; set; }
        /// <summary>  
        /// 入庫部門名稱  
        /// </summary> 
        public string mha05_c { get; set; }
        /// <summary>  
        /// 付款條件
        /// </summary> 
        public string mha11_c { get; set; }

        public string mha13_str { get; set; }
        public string mha13t_str { get; set; }
        public string mha13g_str { get; set; }



        /// <summary>  
        /// 項次  
        /// </summary> 
        public decimal mhb02 { get; set; }
        /// <summary>  
        /// 主件料號  
        /// </summary> 
        public string mhb03 { get; set; }
        /// <summary>  
        /// 主件品名  
        /// </summary> 
        public string mhb04 { get; set; }
        /// <summary>  
        /// 入庫量  
        /// </summary> 
        public decimal mhb05 { get; set; }
        /// <summary>  
        /// 單位  
        /// </summary> 
        public string mhb06 { get; set; }
        /// <summary>  
        /// 倉庫  
        /// </summary> 
        public string mhb07 { get; set; }
        /// <summary>  
        /// 備註  
        /// </summary> 
        public string mhb08 { get; set; }
        /// <summary>  
        /// 單價  
        /// </summary> 
        public decimal mhb09 { get; set; }
        /// <summary>  
        /// 未稅金額  
        /// </summary> 
        public decimal mhb10 { get; set; }
        /// <summary>  
        /// 含稅金額  
        /// </summary> 
        public decimal mhb10t { get; set; }
        /// <summary>  
        /// 託工單號  
        /// </summary> 
        public string mhb11 { get; set; }

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
        public string mhb05_str { get; set; }
    }
}
