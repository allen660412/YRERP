/* 程式名稱: 明細分類帳列印
   系統代號: glar322
   作　　者: Allen
   描　　述: 
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using Stimulsoft.Report.Components;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.Base.Forms;
using System.Linq;
using YR.ERP.DAL.YRModel.Reports.Gla.Glar301;
using Stimulsoft.Report;

namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlar322 : YR.ERP.Base.Forms.FrmReportBase
    {
        public FrmGlar322()
        {
            InitializeComponent();
        }
    }
}
