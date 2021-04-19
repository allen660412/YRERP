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
    public class TabInfo : FormBaseInfo
    {
        //public TabInfo()
        //{
        //    TargetColumn = "";
        //    TargetTable = "";
        //    ViewTable = "";
        //}
        //public YR.ERP.BLL.MSSQL.AdmBLL BoBasic;  // 一般 Insert, Update, Delete 的 business object

        public bool CanAddMode = true;              // 是否可新增
        public bool CanDeleteMode = true;           // 是否可刪除
        public bool CanCopyMode = true;             // 是否可複製
        public bool CanQueryMode = true;            // 是否可查詢
        public bool CanAdvancedQueryMode = true;    // 是否可進階查詢
        public bool CanUpdateMode = true;           // 是否可修改
        public bool CanNavigator = true;            // 是否可使用navgigator RibbonGroup
        public bool CanUseRowLock = true;           // 主表是否在修改模式時,使用(updlock)
        public bool CanUgridQuery = false;          // Grid可否查詢的功能
        public bool IsReadOnly = false;             // 是否唯讀--假雙檔時使用
        public List<SqlParameter> PKParms = null;   // 主表pk值,搭配WfRetrieveMaster 重刷單頭資料
        public bool IsGridFormatFinshed = false;    // 是否已格式化 Grid 的抬頭
        public bool IsAutoQueryFistLoad = false;    // 是否於視窗初始化時，自動查詢(單檔提供的功能,雙檔目前不支援)

        public DataTable DtSource;                  // 主表的來源資料表
        public UltraGrid UGrid;
        //public string QueryWhereString{get;set;}
        //public string TargetTable{get;set;}         // CRUD資料表
        //public string TargetColumn{get;set;}        // CRUD CLUMN 目前是 *(全部)
        //public string ViewTable{get;set;}           // 使用的view

        //public List<aza_tb> AzaTbList{get;set;}     // 畫面說明檔資料
        //public add_tb AddTbModel{get;set;}          // 表單權限

        public bool IsCheckSecurity = true;         // 表單是否需檢驗權限---所有權限的開關,用在開子視窗時,大多都不需檢查權限   
        public string UserColumn { get; set; }      // 個人權限欄位,供寫入或檢查
        public string GroupColumn { get; set; }     // 群組權限欄位,供寫入或檢查
    }
}

