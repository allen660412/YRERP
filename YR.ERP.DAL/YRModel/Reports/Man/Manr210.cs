using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Man.Manr210
{
    public class Master
    {
        /// <summary>  
        /// 單據類別  
        /// </summary> 
        public string mea00 { get; set; }
        /// <summary>  
        /// 單號  
        /// </summary> 
        public string mea01 { get; set; }
        /// <summary>  
        /// 開單日期  
        /// </summary> 
        public Nullable<System.DateTime> mea02 { get; set; }
        /// <summary>  
        /// 廠商編號  
        /// </summary> 
        public string mea03 { get; set; }
        /// <summary>  
        /// 經辦人員  
        /// </summary> 
        public string mea04 { get; set; }
        /// <summary>  
        /// 經辦部門  
        /// </summary> 
        public string mea05 { get; set; }
        /// <summary>  
        /// 課稅別  
        /// </summary> 
        public string mea06 { get; set; }
        /// <summary>  
        /// 稅率  
        /// </summary> 
        public decimal mea07 { get; set; }
        /// <summary>  
        /// 含稅否  
        /// </summary> 
        public string mea08 { get; set; }
        /// <summary>  
        /// 發票聯數  
        /// </summary> 
        public string mea09 { get; set; }
        /// <summary>  
        /// 幣別  
        /// </summary> 
        public string mea10 { get; set; }
        /// <summary>  
        /// 付款條件  
        /// </summary> 
        public string mea11 { get; set; }
        /// <summary>  
        /// 取價條件  
        /// </summary> 
        public string mea12 { get; set; }
        /// <summary>  
        /// 單價  
        /// </summary> 
        public decimal mea13 { get; set; }
        /// <summary>  
        /// 未稅金額  
        /// </summary> 
        public decimal mea14 { get; set; }
        /// <summary>  
        /// 含稅金額  
        /// </summary> 
        public decimal mea14t { get; set; }
        /// <summary>  
        /// 預計開工日  
        /// </summary> 
        public Nullable<System.DateTime> mea15 { get; set; }
        /// <summary>  
        /// 預計完工日  
        /// </summary> 
        public Nullable<System.DateTime> mea16 { get; set; }
        /// <summary>  
        /// 實際開工日  
        /// </summary> 
        public Nullable<System.DateTime> mea17 { get; set; }
        /// <summary>  
        /// 實際完工日  
        /// </summary> 
        public Nullable<System.DateTime> mea18 { get; set; }
        /// <summary>  
        /// 備註  
        /// </summary> 
        public string mea19 { get; set; }
        /// <summary>  
        /// 料號  
        /// </summary> 
        public string mea20 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string mea21 { get; set; }
        /// <summary>  
        /// 預計生產數量  
        /// </summary> 
        public decimal mea22 { get; set; }
        /// <summary>  
        /// 單位  
        /// </summary> 
        public string mea23 { get; set; }
        /// <summary>  
        /// 母製令  
        /// </summary> 
        public string mea24 { get; set; }
        /// <summary>  
        /// 母製令項次  
        /// </summary> 
        public Nullable<decimal> mea25 { get; set; }
        /// <summary>  
        /// 實際入庫量  
        /// </summary> 
        public decimal mea26 { get; set; }
        /// <summary>  
        /// 實際退庫量  
        /// </summary> 
        public decimal mea27 { get; set; }
        
        /// <summary>  
        /// 送貨地址碼  
        /// </summary> 
        public string mea28 { get; set; }
        /// <summary>  
        /// 來源訂單  
        /// </summary> 
        public string mea29 { get; set; }

        
        /// <summary>  
        /// 單別名稱
        /// </summary> 
        public string mea01_c { get; set; }
        /// <summary>  
        /// 廠商名稱
        /// </summary> 
        public string mea03_c { get; set; }
        /// <summary>  
        /// 入庫人員姓名
        /// </summary> 
        public string mea04_c { get; set; }
        /// <summary>  
        /// 入庫部門名稱  
        /// </summary> 
        public string mea05_c { get; set; }
        /// <summary>  
        /// 付款條件
        /// </summary> 
        public string mea11_c { get; set; }
        /// <summary>  
        /// 取價條件
        /// </summary> 
        public string mea12_c { get; set; }
        /// <summary>  
        /// 主件規格  
        /// </summary> 
        public string ica03_main { get; set; }

        public string mea13_str { get; set; }
        public string mea14_str { get; set; }
        public string mea14t_str { get; set; }
        public string mea22_str { get; set; }
        public decimal bej03_main { get; set; }

        /// <summary>  
        /// 項次  
        /// </summary> 
        public decimal meb02 { get; set; }
        /// <summary>  
        /// 料號  
        /// </summary> 
        public string meb03 { get; set; }
        /// <summary>  
        /// 品名  
        /// </summary> 
        public string meb04 { get; set; }
        /// <summary>  
        /// 預計數量  
        /// </summary> 
        public decimal meb05 { get; set; }
        /// <summary>  
        /// 入庫單位  
        /// </summary> 
        public string meb06 { get; set; }

        public string meb07 { get; set; }

        /// <summary>  
        /// 規格  
        /// </summary> 
        public string ica03_detail { get; set; }

        /// <summary>
        /// 單位的小數位數
        /// </summary>
        public decimal bej03_detail { get; set; }

        /// <summary>  
        /// 入庫數量(字串)
        /// </summary> 
        public string meb05_str { get; set; }
    }
}
