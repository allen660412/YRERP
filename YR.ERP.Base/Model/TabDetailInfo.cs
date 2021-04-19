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
    public class TabDetailInfo : FormBaseInfo
    {
        //public TabDetailInfo()
        //{
        //    TargetColumn = "";
        //    TargetTable = "";
        //    ViewTable = "";
        //}
        public bool CanAddMode = true;                      // 是否可新增
        public bool CanDeleteMode = true;                   // 是否可刪除
        //public bool CanQueryMode = true;                  // 是否可查詢--暫無作用
        //public bool CanUpdateMode = true;                 // 是否可修改--暫無作用

        public bool CanUgridQuery = true;                   // grid可否查詢的功能
        public bool IsReadOnly = false;                     // 是否唯讀
        public bool IsGridFormatFinshed = false;            // 是否已格式化 Grid 的抬頭

        public DataTable DtSource;
        public UltraGrid UGrid;

        //public YR.ERP.BLL.MSSQL.AdmBLL BoBasic;             // 一般 Insert, Update, Delete 的 business object
        public List<SqlParameter> RelationParams;           // master detial 的 relation 參數

        public string QueryWhereString { get; set; }

        //public string TargetTable { get; set; }
        //public string TargetColumn { get; set; }
        //public string ViewTable { get; set; }
        //public List<aza_tb> AzaTbList;
    }
}

