using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using YR.ERP.Base;
using Infragistics.Win.UltraWinGrid;
using YR.ERP.DAL.YRModel;

namespace YR.ERP.Base.Model
{
    public class BatchInfo : FormBaseInfo
    {
        public DataTable DtSource;                      // 主表的來源資料表--for query

        public bool IsCloseAfterExecuted = false;       //執行完畢後是否關閉, 主要作用由他窗引用
        public bool IsAutoExecuted = false;             //報表於開啟後是否自動執行

        public bool IsCheckSecurity = true;             // 表單是否需檢驗權限---所有權限的開關,用在開子視窗時,大多都不需檢查權限   
        public string UserColumn { get; set; }          // 個人權限欄位,供寫入或檢查
        public string GroupColumn { get; set; }         // 群組權限欄位,供寫入或檢查
    }
}
