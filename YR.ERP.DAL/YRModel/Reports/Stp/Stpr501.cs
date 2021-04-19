using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Stp.Stpr501
{
    public class Master 
    {
        public string sha01 { get; set; }
        public DateTime sha02 { get; set; }
        public string sha03 { get; set; }
        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string sha03_c { get; set; }

        public string sha06 { get; set; }
        public string sha09 { get; set; }

        public decimal sha13 { get; set; }
        public decimal sha13t { get; set; }
        public decimal sha13g { get; set; }

        public string sha13_str { get; set; }
        public string sha13t_str { get; set; }
        public string sha13g_str { get; set; }

        /// <summary>
        /// 公司全名
        /// </summary>
        public string bea01 { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string bea03 { get; set; }
        /// <summary>
        /// 電話
        /// </summary>
        public string bea04 { get; set; }
        /// <summary>
        /// 傳真
        /// </summary>
        public string bea05 { get; set; }
        /// <summary>
        /// 統編
        /// </summary>
        public string bea06 { get; set; }

        /// <summary>
        /// 客戶統編
        /// </summary>
        public string sca12 { get; set; }
        /// <summary>
        /// 客戶帳單地址
        /// </summary>
        public string sca17 { get; set; }

        /// <summary>
        /// 單價小數位數
        /// </summary>
        public decimal bek03 { get; set; }
        /// <summary>
        /// 總計小數位數
        /// </summary>
        public decimal bek04 { get; set; }

        ///*****以下為明細內容****/
        public decimal shb02 { get; set; }
        public string shb03 { get; set; }
        public string shb04 { get; set; }
        public decimal shb05 { get; set; }
        public decimal shb09 { get; set; }
        public decimal shb10 { get; set; }
        public decimal shb10t { get; set; }
        public string shb17 { get; set; }
        public string shb19 { get; set; }
        public DateTime? shb20 { get; set; }
        public decimal bej03 { get; set; }

        public string shb05_str { get; set; }
        public string shb09_str { get; set; }
        public string shb10_str { get; set; }
        public string shb10g_str { get; set; }
    }

}
