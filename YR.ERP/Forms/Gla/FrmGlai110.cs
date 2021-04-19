/* 程式名稱: 部門層級設定作業
   系統代號: glai110
   作　　者: Allen
   描　　述: 

   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.BLL.MSSQL.Gla;
using YR.ERP.DAL.YRModel;
using YR.Util;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlai110 : YR.ERP.Base.Forms.FrmEntryMDBase
    {

        #region Property
        BasBLL BoBas;
        GlaBLL BoGla;
        #endregion

        #region 建構子
        public FrmGlai110()
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
            this.StrFormID = "glai110";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "基本資料";
            uTab_Master.Tabs[1].Text = "狀態";
            uTab_Master.Tabs[2].Text = "資料瀏覽";

            IntTabDetailCount = 1;
            uTab_Detail.Tabs[0].Text = "明細資料";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("gbd01", SqlDbType.NVarChar) });

                TabMaster.CanCopyMode = false;
                TabMaster.IsReadOnly = true;
                TabMaster.CanUseRowLock = false;//假雙檔不使用rowlock
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);

            BoBas = new BasBLL(BoMaster.OfGetConntion());
            BoGla = new GlaBLL(BoMaster.OfGetConntion());
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
                    BoBas.TRAN = BoMaster.TRAN;
                    BoGla.TRAN = BoMaster.TRAN;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                ////單據確認
                //sourceList = BoStp.OfGetSgaconfKVPList();
                //WfSetUcomboxDataSource(ucb_sgaconf, sourceList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniTabDetailInfo(): 設定明細的資料來源
        protected override Boolean WfIniTabDetailInfo()
        {
            SqlParameter keyParm;

            this.TabDetailList[0].TargetTable = "gbd_tb";
            this.TabDetailList[0].ViewTable = "vw_glai110s";
            keyParm = new SqlParameter("gbd01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "gbd01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };

            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            try
            {

                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false); //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯

                    WfSetControlReadonly(new List<Control> { ute_gbd01_c }, true);

                    WfSetControlReadonly(new List<Control> { ute_gbdcreu, ute_gbdcreg, udt_gbdcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_gbdmodu, ute_gbdmodg, udt_gbdmodd }, true);

                    WfSetControlReadonly(uGridMaster, true);//grid不可編輯
                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "gbd02_c" }, true);
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_gbd01, true);
                    }
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

                if (FormEditMode == YREditType.新增)
                    SelectNextControl(this.uTab_Master, true, true, true, false);
                //else
                //{
                //    WfSetFirstVisibelCellFocus(TabDetailList[0].UGrid);
                //}
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
            bab_tb babModel;
            try
            {
                switch (pCurTabDetail)
                {
                    case 0:
                        foreach (UltraGridCell ugc in pUgr.Cells)
                        {
                            columnName = ugc.Column.Key.ToLower();
                            ////先控可以輸入的
                            //if (
                            //    columnName == "gfb03" 
                            //    )
                            //{
                            //    WfSetControlReadonly(ugc, false);
                            //    continue;
                            //}

                            if (columnName == "gbd02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                vw_glai110 masterModel = null;
                vw_glai110s detailModel = null;
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_glai110
                if (pDr.Table.Prefix.ToLower() == "vw_glai110")
                {
                    masterModel = DrMaster.ToItem<vw_glai110>();
                    switch (pColName.ToLower())
                    {
                        case "gbd01"://上層部門
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_glai110s
                if (pDr.Table.Prefix.ToLower() == "vw_glai110s")
                {
                    masterModel = DrMaster.ToItem<vw_glai110>();
                    detailModel = pDr.ToItem<vw_glai110s>();
                    switch (pColName.ToLower())
                    {
                        case "gbd02"://下層部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
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

        #region WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr) 設定明細資料列的初始值
        protected override bool WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr)
        {
            try
            {

                switch (pCurTabDetail)
                {
                    case 0:
                        pDr["gbdcomp"] = LoginInfo.CompNo;
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int chkCnts = 0;
            vw_glai110 masterModel = null;
            vw_glai110s detailModel = null;
            List<vw_glai110s> detailList = null;
            gba_tb gbaModel = null;
            UltraGrid uGrid = null;
            UltraGridRow uGridRow = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_glai110>();
                if (e.Column.ToLower() != "gbd01" && GlobalFn.isNullRet(DrMaster["gbd01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入上層部門!");
                    return false;
                }
                #region 單頭-vw_glai110
                if (e.Row.Table.Prefix.ToLower() == "vw_glai110")
                {
                    switch (e.Column.ToLower())
                    {
                        case "gbd01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["gbd01_c"] = "";
                                return true;
                            }
                            var babModel = BoBas.OfGetBebModel(e.Value.ToString());
                            if (babModel == null)
                            {
                                WfShowErrorMsg("無此上層部門,請檢核!");
                                return false;
                            }
                            if (babModel.bebvali != "Y")
                            {
                                WfShowErrorMsg("此上層部門已失效,請檢核!");
                                return false;
                            }
                            if (WfChkGbd01Exists(e.Value.ToString())==true)
                            {
                                WfShowErrorMsg("此上層部門已存在,請檢核!");
                                return false;
                            }

                            e.Row["gbd01_c"] = babModel.beb03;
                            break;
                    }
                }
                #endregion

                #region 單身-vw_glai110s
                if (e.Row.Table.Prefix.ToLower() == "vw_glai110s")
                {
                    uGrid = sender as UltraGrid;
                    uGridRow = uGrid.ActiveRow;
                    detailModel = e.Row.ToItem<vw_glai110s>();
                    detailList = e.Row.Table.ToList<vw_glai110s>();
                    switch (e.Column.ToLower())
                    {
                        case "gbd02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["gbd02_c"] = "";
                                return true;
                            }
                            var babModel = BoBas.OfGetBebModel(e.Value.ToString());
                            if (babModel == null)
                            {
                                WfShowErrorMsg("無此下層部門,請檢核!");
                                return false;
                            }
                            if (babModel.bebvali != "Y")
                            {
                                WfShowErrorMsg("此下層部門已失效,請檢核!");
                                return false;
                            }
                            if (masterModel.gbd01.ToUpper()==e.Value.ToString().ToUpper())
                            {
                                WfShowErrorMsg("不可與上層部門相同,請檢核!");
                                return false;
                            }
                            chkCnts = detailList.Where(p => p.gbd02.ToUpper() == e.Value.ToString().ToUpper()).Count();
                            if (chkCnts>1)
                            {
                                WfShowErrorMsg("下層部門不可重覆,請檢核!");
                                return false;
                            }
                            sqlSelect = "SELECT COUNT(1) FROM gbd_tb WHERE gbd02=@gbd02 AND gbd01<>@gbd01";
                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@gbd01",masterModel.gbd01));
                            sqlParmList.Add(new SqlParameter("@gbd02",e.Value.ToString()));
                            chkCnts = GlobalFn.isNullRet(BoGla.OfGetFieldValue(sqlSelect, sqlParmList.ToArray()),0);
                            if (chkCnts>0)
                            {
                                WfShowErrorMsg("此下層部門已隸屬於其他上層部門,請檢核!");
                                return false;
                            }
                            e.Row["gbd02_c"] = babModel.beb03;
                            break;
                    }
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            int chkCnts = 0;
            vw_glai110 masterModel = null;
            vw_glai110s detailModel = null;
            List<vw_glai110s> detailList = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_glai110>();
                #region 單頭資料檢查
                chkColName = "gbd01";       //傳票單號
                chkControl = ute_gbd01;
                if (GlobalFn.varIsNull(masterModel.gbd01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                if (FormEditMode==YREditType.新增)
                {
                    if (WfChkGbd01Exists(masterModel.gbd01) == true)
                    {
                        msg = "此上層部門已存在,請檢核!";
                        WfShowErrorMsg(msg);
                        errorProvider.SetError(chkControl, msg);
                        WfShowErrorMsg(msg);
                        return false;
                    }
                }

                #endregion
                
                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                detailList=TabDetailList[iChkDetailTab].DtSource.ToList<vw_glai110s>();
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;
                    
                    detailModel = drTemp.ToItem<vw_glai110s>();
                    chkColName = "gbd02";   //下層部門
                    if (GlobalFn.varIsNull(detailModel.gbd02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    if (drTemp.RowState==DataRowState.Added)
                    {
                        chkCnts = detailList.Where(p => p.gbd02.ToUpper() == detailModel.gbd02.ToUpper()).Count();
                        if (chkCnts > 1)
                        {
                            WfShowErrorMsg("下層部門不可重覆,請檢核!");
                            return false;
                        }
                        
                        sqlSelect = "SELECT COUNT(1) FROM gbd_tb WHERE gbd02=@gbd02 AND gbd01<>@gbd01";
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gbd01", masterModel.gbd01));
                        sqlParmList.Add(new SqlParameter("@gbd02", detailModel.gbd02));
                        chkCnts = GlobalFn.isNullRet(BoGla.OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0);
                        if (chkCnts > 0)
                        {
                            WfShowErrorMsg("此下層部門已隸屬於其他上層部門,請檢核!");
                            return false;
                        }
                    }
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
            vw_glai110 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_glai110>();

                //填入系統資訊
                //if (DrMaster.RowState != DataRowState.Unchanged)
                //{
                //    if (DrMaster.RowState == DataRowState.Added)
                //    {
                //        DrMaster["gbdcreu"] = LoginInfo.UserNo;
                //        DrMaster["gbdcreg"] = LoginInfo.DeptNo;
                //        DrMaster["gbdcred"] = Now;
                //    }
                //    else if (DrMaster.RowState == DataRowState.Modified)
                //    {
                //        DrMaster["gbdmodu"] = LoginInfo.UserNo;
                //        DrMaster["gbdmodg"] = LoginInfo.DeptNo;
                //        DrMaster["gbdmodd"] = Now;
                //    }
                //}

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["gbdcreu"] = LoginInfo.UserNo;
                            drDetail["gbdcreg"] = LoginInfo.DeptNo;
                            drDetail["gbdcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["gbdmodu"] = LoginInfo.UserNo;
                            drDetail["gbdmodg"] = LoginInfo.DeptNo;
                            drDetail["gbdmodd"] = Now;
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

        //*****************************表單自訂Fuction****************************************	
        
        #region WfChkGbd01Exists
        private bool WfChkGbd01Exists(string pGbd01)
        {
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            try
            {
                sqlSelect = "SELECT COUNT(1) FROM gbd_tb WHERE gbd01=@gbd01";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbd01", pGbd01));
                var chkCnts = GlobalFn.isNullRet(BoGla.OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0);
                if (chkCnts > 0)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion


    }
}
