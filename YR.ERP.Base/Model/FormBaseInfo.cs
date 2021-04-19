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
    public class FormBaseInfo
    {
        public FormBaseInfo()
        {
            TargetColumn = "";
            TargetTable = "";
            ViewTable = "";
        }
        public YR.ERP.BLL.MSSQL.AdmBLL BoBasic;         // 一般 Insert, Update, Delete 的 business object
        public string TargetTable { get; set; }         // CRUD資料表
        public string TargetColumn { get; set; }        // CRUD CLUMN 目前是 *(全部)
        public string ViewTable { get; set; }           // 使用的view

        public List<aza_tb> AzaTbList { get; set; }     // 畫面說明檔資料
        public add_tb AddTbModel { get; set; }          // 表單權限
    }
}
