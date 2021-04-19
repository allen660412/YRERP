/* 程式名稱: 廠內領料維護作業
   系統代號: mant301
   作　　者: Allen
   描　　述: 
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.BLL.Model;

namespace YR.ERP.Forms.Man
{
    public partial class FrmMant301 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        public FrmMant301()
        {
            InitializeComponent();
        }
    }
}
