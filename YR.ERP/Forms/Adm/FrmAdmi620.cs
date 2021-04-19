/* 程式名稱: 程式畫面欄位設定作業
   系統代號: admi620
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
using System.Text;
using System.Windows.Forms;
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinToolbars;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Infragistics.Win.UltraWinGrid;
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.BLL.Model;

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi620 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        AdmBLL BoAdm = null;
        #endregion

        #region 建構子
        public FrmAdmi620()
        {
            InitializeComponent();
        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            // 繼承的表單要覆寫, 更改參數值            
            /*
            this.strFormID = "XXX";
            this.isDirectEdit = false;
            this.isMultiRowEdit = false;
             */
            this.StrFormID = "admi620";
            IntTabCount = 2;
            IntMasterGridPos = 2;
            uTab_Master.Tabs[0].Text = "資料內容";
            uTab_Master.Tabs[1].Text = "資料瀏覽";
            IntTabDetailCount = 1;
            uTab_Detail.Tabs[0].Text = "明細資料";
            return true;
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            //tabMaster.strBLLClassName = "YR.ERP.BLL.MSSQL.CommonBLL";
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);

            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
            return;
        }
        #endregion

        #region WfSetBllTransaction 以bomaster 註冊transaction至其他 bll
        protected override void WfSetBllTransaction()
        {
            try
            {
                if (BoMaster.TRAN != null)
                {
                    BoAdm.TRAN = BoMaster.TRAN;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示
        protected override Boolean WfDisplayMode()
        {
            try
            {
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false);//先全開後,再微調
                    WfSetControlReadonly(uGridMaster, true);//grid不可編輯
                    WfSetControlReadonly(ute_aza01, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterfDisplayMode  新增修改刪除查詢後的 focus調整
        protected override void WfAfterfDisplayMode()
        {
            try
            {
                uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                SelectNextControl(this.uTab_Master, true, true, true, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetDetailDisplayMode 新增修改時的明細列可輸入處理,需要每筆資料列微調整時再使用
        protected override void WfSetDetailDisplayMode(int pCurTabDetail, UltraGridRow pUgr, DataRow pDr)
        {
            string columnName;
            bab_tb l_bab;
            aza_tb l_aza;
            try
            {
                switch (pCurTabDetail)
                {
                    case 0:
                        l_aza=pDr.ToItem<aza_tb>();
                        WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "aza01", "aza02", "aza03" }, true);
                        WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "aza08", "aza09", "aza10", "aza11" }, true);
                        foreach (UltraGridCell ugc in pUgr.Cells)
                        {
                            columnName = ugc.Column.Key.ToLower();
                            //先控可以輸入的
                            if (
                                columnName == "aza04" ||
                                columnName == "aza05" ||
                                columnName == "aza06" ||
                                columnName == "aza07" 
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "aza12")  //字串轉換大小寫
                            {
                                if (l_aza.aza08 == "nvarchar"||
                                    l_aza.aza08 == "char"
                                    )
                                    WfSetControlReadonly(ugc, false);
                                else
                                    WfSetControlReadonly(ugc, true);


                                continue;
                            }

                            if (columnName == "aza13")  //是否產生pick按鈕
                            {
                                if (l_aza.aza08 == "nvarchar" ||
                                    l_aza.aza08 == "char" ||
                                    l_aza.aza08 == "numeric"
                                    )
                                    WfSetControlReadonly(ugc, false);
                                else
                                    WfSetControlReadonly(ugc, true);

                                continue;
                            }

                            if (columnName == "aza14")  //顯示時間
                            {
                                if (l_aza.aza08 == "datetime" ||
                                    l_aza.aza08 == "datetime2"
                                    )
                                    WfSetControlReadonly(ugc, false);
                                else
                                    WfSetControlReadonly(ugc, true);

                                continue;
                            }

                            WfSetControlReadonly(ugc, true);    //其餘的都關閉
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.IsReadOnly = true;        //假雙檔

            TabMaster.CanAddMode = false;
            TabMaster.CanCopyMode = false;


            TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("aza01", SqlDbType.NVarChar) });  //假雙檔會咬一個集合
        }
        #endregion

        #region WfIniTabDetailInfo(): 設定明細的資料來源
        protected override Boolean WfIniTabDetailInfo()
        {
            List<SqlParameter> keyParms;
            SqlParameter keyParm;
            // 設定 Detail tab1 資料 : 終端印件
            this.TabDetailList[0].TargetTable = "aza_tb";
            this.TabDetailList[0].ViewTable = "vw_admi620s";

            keyParms = new List<SqlParameter>();
            keyParm = new SqlParameter("aza01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "aza01";
            keyParms.Add(keyParm);

            this.TabDetailList[0].RelationParams = keyParms;
            return true;
        }
        #endregion

        #region WfIniDetailComboSource 處理grid下拉選單
        protected override void WfIniDetailComboSource(int pTabIndex)
        {
            UltraGrid uGrid;
            UltraGridColumn ugc;
            try
            {
                switch (pTabIndex)
                {
                    case 0:
                        uGrid = TabDetailList[pTabIndex].UGrid;
                        ugc = uGrid.DisplayLayout.Bands[0].Columns["aza12"];//元件類型
                        WfSetGridValueList(ugc, BoAdm.OfGetAza12KVPList());
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAddAction 新增action按鈕
        protected override List<ButtonTool> WfAddAction()
        {
            List<ButtonTool> listBt = new List<ButtonTool>();
            ButtonTool bt;
            try
            {
                bt = new ButtonTool("RegenViewByYR");
                bt.SharedProps.Caption = "Create View By YR DB";
                bt.SharedProps.Category = "action";
                listBt.Add(bt);

                bt = new ButtonTool("RefreshView"); //資料表有alter 欄位 新增修改刪除時,要執行
                bt.SharedProps.Caption = "更新所有的view";
                bt.SharedProps.Category = "action";
                listBt.Add(bt);

                bt = new ButtonTool("ReSortAza06"); //依原本序號順序重新排序
                bt.SharedProps.Caption = "重新排序";
                bt.SharedProps.Category = "action";
                listBt.Add(bt);

                bt = new ButtonTool("GenAza04"); //畫面欄位依atc04重新產生
                bt.SharedProps.Caption = "更新欄位名稱";
                bt.SharedProps.Category = "action";
                listBt.Add(bt);

                return listBt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfActionClick(string pActionName)
        protected override void WfActionClick(string pActionName)
        {
            DataTable dtTemp;
            DataRow[] ldrs_temp;
            DataRow ldr_temp;
            int iSeq;
            try
            {
                switch (pActionName)
                {
                    case "RegenViewByYR":  //依YR資料庫,重新產生至所有DB
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfReGenView();
                        break;
                    case "RefreshView":  //資料表有alter 欄位 新增修改刪除時,要執行
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        WfRefreshView();
                        break;
                    case "ReSortAza06":
                        if (FormEditMode == YREditType.NA)
                            return;
                        dtTemp = this.TabDetailList[0].DtSource;
                        if (dtTemp == null || dtTemp.Rows.Count == 0)
                            return;
                        //處理要顯示
                        ldrs_temp = dtTemp.Select(" aza05='Y' ", "aza06");
                        iSeq = 0;
                        for (int i = 0; i < ldrs_temp.Length; i++)
                        {
                            iSeq += 10;
                            ldr_temp = ldrs_temp[i];
                            ldr_temp["aza06"] = iSeq;
                        }
                        //處理不顯示
                        ldrs_temp = dtTemp.Select(" ISNULL(aza05,'N')<>'Y' ");
                        for (int i = 0; i < ldrs_temp.Length; i++)
                        {
                            ldr_temp = ldrs_temp[i];
                            ldr_temp["aza06"] = "9999";
                        }
                        TabDetailList[0].UGrid.Update();
                        TabDetailList[0].UGrid.UpdateData();
                        this.IsChanged = true;
                        break;
                    case "GenAza04":  //資料表有alter 欄位 新增修改刪除時,要執行
                        if (FormEditMode == YREditType.NA)
                            return;
                        WfGenAza04();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfItemCheck
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            try
            {
                //if (e.Row.Table.Prefix.ToLower() == "vw_admi620s")
                //{
                //    switch (e.Column.ToLower())
                //    {
                //        case "aza12":
                //            var aza12 = e.Value;


                //            break;
                //    }

                //}
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSeDetailGridLayout
        protected override void WfSeDetailGridLayout(UltraGrid pUgrid)
        {
            Infragistics.Win.UltraWinGrid.UltraGridColumn lugc;
            try
            {
                if (pUgrid.DisplayLayout.Bands[0].Columns.Exists("aza05"))
                {
                    lugc = pUgrid.DisplayLayout.Bands[0].Columns["aza05"];
                    WfSetUgridCheckBox(lugc);
                }

                if (pUgrid.DisplayLayout.Bands[0].Columns.Exists("aza13"))
                {
                    lugc = pUgrid.DisplayLayout.Bands[0].Columns["aza13"];
                    WfSetUgridCheckBox(lugc);
                }
                //顯示時間
                if (pUgrid.DisplayLayout.Bands[0].Columns.Exists("aza14"))
                {
                    lugc = pUgrid.DisplayLayout.Bands[0].Columns["aza14"];
                    WfSetUgridCheckBox(lugc);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_admi620 masterModel = null;
            string msg;
            Control chkControl;
            try
            {
                masterModel = DrMaster.ToItem<vw_admi620>();
                #region 單頭資料檢查
                if (GlobalFn.varIsNull(masterModel.aza02))
                {
                    chkControl = ute_aza02;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == "aza02").Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterFormCheck() 存檔後處理,通常為放入Pk
        protected override bool WfAfterFormCheck()
        {
            try
            {
                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["azacreu"] = LoginInfo.UserNo;
                            drDetail["azacreg"] = LoginInfo.DeptNo;
                            drDetail["azacred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["azamodu"] = LoginInfo.UserNo;
                            drDetail["azamodg"] = LoginInfo.DeptNo;
                            drDetail["azamodd"] = Now;
                        }
                    }
                }

                WfSetDetailPK();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetDetailPK() 依單頭與單身的對應關係賦予 PK
        protected override void WfSetDetailPK()
        {
            try
            {
                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    drDetail["aza01"] = DrMaster["aza01"];
                    drDetail["aza02"] = DrMaster["aza02"];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //*****************************表單自訂Fuction****************************************
        #region WfRegenView
        //todo:效能安全姓及來源DB連線還要調整
        private void WfReGenView()
        {
            try
            {
                StringBuilder sbDDL = new StringBuilder();
                var urns = new List<Urn>();
                System.Collections.Specialized.StringCollection strDropCollection;
                System.Collections.Specialized.StringCollection strCreateCollection;
                StringBuilder sbSql;
                DataTable dtAta;
                string ata03;   //資料庫名稱

                if (BoSecurity == null) return;
                // Connect to the local, default instance of SQL Server. 
                SqlConnection sc = new SqlConnection(YR.ERP.Shared.GlobalVar.SQLCA_SecConSTR);
                Microsoft.SqlServer.Management.Common.ServerConnection sqlConn = new Microsoft.SqlServer.Management.Common.ServerConnection(sc);
                Server server = new Server(sqlConn);

                // Reference the database.  
                Database dbSecurity = server.Databases[sqlConn.DatabaseName];

                //先刪除
                Scripter scrpDrop = new Scripter(server);
                scrpDrop.Options.ScriptDrops = true;
                scrpDrop.Options.WithDependencies = false;
                scrpDrop.Options.Indexes = false;   // To include indexes
                scrpDrop.Options.DriAllConstraints = true;   // to include referential constraints in the script
                scrpDrop.Options.Triggers = false;
                scrpDrop.Options.FullTextIndexes = false;
                scrpDrop.Options.NoCollation = false;
                scrpDrop.Options.Bindings = false;
                scrpDrop.Options.IncludeIfNotExists = false;
                scrpDrop.Options.ScriptBatchTerminator = true;
                scrpDrop.Options.ExtendedProperties = true;
                scrpDrop.PrefetchObjects = true; // some sources suggest this may speed things up
                //再建立
                Scripter scrpCreate = new Scripter(server);
                scrpCreate.Options.ScriptDrops = false;
                scrpCreate.Options.WithDependencies = false;
                scrpCreate.Options.Indexes = false;   // To include indexes
                scrpCreate.Options.DriAllConstraints = true;   // to include referential constraints in the script
                scrpCreate.Options.Triggers = false;
                scrpCreate.Options.FullTextIndexes = false;
                scrpCreate.Options.NoCollation = false;
                scrpCreate.Options.Bindings = false;
                scrpCreate.Options.IncludeIfNotExists = false;
                scrpCreate.Options.ScriptBatchTerminator = true;
                scrpCreate.Options.ExtendedProperties = true;
                scrpCreate.PrefetchObjects = true; // some sources suggest this may speed things up

                // Iterate through the views in database and script each one. Display the script.   
                foreach (Microsoft.SqlServer.Management.Smo.View view in dbSecurity.Views)
                {
                    // check if the view is not a system object
                    if (view.IsSystemObject == false)
                    {
                        urns.Add(view.Urn);
                    }
                }
                
                strCreateCollection = scrpCreate.Script(urns.ToArray());
                //foreach (string st in strCreateCollection)
                //{
                //    // It seems each string is a sensible batch, and putting GO after it makes it work in tools like SSMS.
                //    // Wrapping each string in an 'exec' statement would work better if using SqlCommand to run the script.
                //    sbDDL.AppendLine(st);
                //    sbDDL.AppendLine("GO");
                //}
                
                if (strCreateCollection != null)
                {
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT * FROM ata_tb");
                    dtAta = BoSecurity.OfGetDataTable(sbSql.ToString(), null);
                    if (dtAta != null && dtAta.Rows.Count > 0)
                    {
                        foreach (DataRow drTemp in dtAta.Rows)
                        {
                            //這裡要建立刪除的VIEW
                            ata03 = drTemp["ata03"] == DBNull.Value ? "" : drTemp["ata03"].ToString();
                            Database dbTemp = server.Databases[ata03];

                            if (dbTemp == null)
                                continue;

                            //假如主連線與要刪除view的db是相同的,刪不做處理
                            if (dbSecurity.Name.ToUpper() == dbTemp.Name.ToUpper())
                                continue;

                            urns.Clear();
                            // Iterate through the views in database and script each one. Display the script.   
                            foreach (Microsoft.SqlServer.Management.Smo.View view in dbTemp.Views)
                            {
                                // check if the view is not a system object
                                if (view.IsSystemObject == false)
                                {
                                    urns.Add(view.Urn);
                                }
                            }
                            strDropCollection = scrpDrop.Script(urns.ToArray());
                            if (strDropCollection != null && strDropCollection.Count > 0)
                            {
                                dbTemp.ExecuteNonQuery(strDropCollection);
                                strDropCollection.RemoveAt(0);//執行過後會在第一筆加入 USE DATABASE...因此在這裡移除,避免錯誤
                            }

                            dbTemp.ExecuteNonQuery(strCreateCollection);
                            strCreateCollection.RemoveAt(0);//執行過後會在第一筆加入 USE DATABASE...因此在這裡移除,避免錯誤
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfRefreshView 會順便重新產生 aza_tb的資料
        //todo:目前是以proc運作,需考量改用程式碼來執行
        private void WfRefreshView()
        {
            StringBuilder sbSql, sbViewSql;
            string lsSql;
            string viewName;
            DataTable dtAta, dtView;
            string ata01;
            CommonBLL boRefresh;

            try
            {
                if (BoSecurity == null)
                {
                    WfShowErrorMsg("未設定主要連線!");
                    return;
                }

                //先更新主要連線的view YR
                sbViewSql = new StringBuilder();
                sbViewSql.AppendLine(string.Format("SELECT  name "));
                sbViewSql.AppendLine(string.Format("FROM sys.objects"));
                sbViewSql.AppendLine(string.Format("WHERE type = 'V'"));
                dtView = BoSecurity.OfGetDataTable(sbViewSql.ToString());
                foreach (DataRow ldr in dtView.Rows)
                {
                    viewName = ldr["name"] == DBNull.Value ? "" : ldr["name"].ToString();
                    lsSql = string.Format("EXEC sp_refreshview '{0}' ", viewName);
                    BoSecurity.OfExecuteNonquery(lsSql.ToString());
                    lsSql = string.Format("EXEC sp_gen_aza '{0}' ", viewName);
                    BoSecurity.OfExecuteNonquery(lsSql.ToString());
                }

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ata_tb");
                dtAta = BoSecurity.OfGetDataTable(sbSql.ToString(), null);

                if (dtAta != null && dtAta.Rows.Count > 0)
                {
                    foreach (DataRow drTemp in dtAta.Rows)
                    {
                        ata01 = drTemp["ata01"] == DBNull.Value ? "" : drTemp["ata01"].ToString();
                        boRefresh = new CommonBLL(ata01, "", "", "");
                        //boRefresh.TRAN = admBll.TRAN;

                        dtView = boRefresh.OfGetDataTable(sbViewSql.ToString());
                        foreach (DataRow ldr in dtView.Rows)
                        {
                            viewName = ldr["name"] == DBNull.Value ? "" : ldr["name"].ToString();
                            lsSql = string.Format("EXEC sp_refreshview '{0}' ", viewName);
                            boRefresh.OfExecuteNonquery(lsSql.ToString());
                        }
                    }
                }
                //admBll.TRAN.Commit();
            }
            catch (System.Exception ex)
            {
                BoSecurity.TRAN.Rollback();
                throw ex;
            }
        }
        #endregion

        #region WfGenAza04 重新產生畫面欄位說明,依atc03
        private void WfGenAza04()
        {
            aza_tb l_aza;            
            try
            {
                if (BoMaster.TRAN != null)
                    BoAdm.TRAN = BoMaster.TRAN;
                foreach (DataRow ldr in TabDetailList[0].DtSource.Rows)
                {
                    l_aza=ldr.ToItem<aza_tb>();
                    if (l_aza.aza03 != l_aza.aza04) //有異動過的資料不處理
                        continue;
                    if (GlobalFn.isNullRet(ldr["aza03"], "") == "")    //取不到中文也不更新
                        continue;

                    ldr["aza04"] = BoAdm.OfGetAtc03(GlobalFn.isNullRet(ldr["aza03"], ""));
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
