using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.ERP.DAL.YRModel.Reports.Stp.Stpr500
{
    /// <summary>  
    /// 銷退單單頭  
    /// </summary>  
    public class Master : YR.ERP.DAL.YRModel.sha_tb
    {
        public string sha03_c { get; set; }
        public string sha04_c { get; set; }
        public string sha05_c { get; set; }
        public string sha01_c { get; set; }

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
    }

    /// <summary>  
    /// 銷退單單身  
    /// </summary>  
    public class Detail : YR.ERP.DAL.YRModel.shb_tb
    {

    }
}
