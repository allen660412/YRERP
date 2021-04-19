using System.Collections.Generic;
using System.Data;
using YR.ERP.DAL.YRModel;

namespace YR.ERP.BLL.Model
{
    public class ReportInfo : FormBaseInfo
    {
        //public ReportInfo()
        //{
        //    TargetColumn = "";
        //    TargetTable = "";
        //    ViewTable = "";
        //}

        //public YR.ERP.BLL.MSSQL.AdmBLL BoBasic;         // 一般 Insert, Update, Delete 的 business object
        //public List<aza_tb> AzaTbList;                  // 畫面說明檔資料
        //public add_tb AddTbModel;                       // 表單權限    

        //public string TargetTable { get; set; }         // CRUD資料表
        //public string TargetColumn { get; set; }        // CRUD CLUMN 目前是 *
        //public string ViewTable { get; set; }           // 使用的view
        public DataTable DtSource;                      // 主表的來源資料表--for query

        public DataSet DsReport;                        //報表回傳的資料來源
        public string ReportName { get; set; }          //報表文件名
        public bool IsCloseAfterExecuted = false;       //執行完畢後是否關閉, 主要作用由他窗引用
        public bool IsAutoExecuted = false;             //報表於開啟後是否自動執行

        public bool IsCheckSecurity = true;             // 表單是否需檢驗權限---所有權限的開關,用在開子視窗時,大多都不需檢查權限   
        public string UserColumn { get; set; }          // 個人權限欄位,供寫入或檢查
        public string GroupColumn { get; set; }         // 群組權限欄位,供寫入或檢查
    }
}
