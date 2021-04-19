/* 程式名稱: 分錄底稿維護作業
   系統代號: glat200
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
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;
using YR.ERP.Shared;


namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlat200 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        GlaBLL BoGla = null;
        AdmBLL BoAdm = null;
        #endregion

        #region 建構子
        public FrmGlat200()
        {
            InitializeComponent();
        }


        public FrmGlat200(string pSourceForm, YR.ERP.Shared.UserInfo pUserInfo, string pWhere)
        {
            InitializeComponent();
            StrQueryWhereAppend = pWhere;
            this.LoginInfo = pUserInfo;
        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            this.StrFormID = "glat200";

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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("gea01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "geasecu";
                TabMaster.GroupColumn = "geasecg";

                TabMaster.CanCopyMode = false;
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
                    BoBas.TRAN = BoMaster.TRAN;
                    BoGla.TRAN = BoMaster.TRAN;
                    BoAdm.TRAN = BoMaster.TRAN;
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

            this.TabDetailList[0].TargetTable = "geb_tb";
            this.TabDetailList[0].ViewTable = "vw_glat200s";
            keyParm = new SqlParameter("geb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "gea01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            keyParm = new SqlParameter("geb02", SqlDbType.NVarChar);
            keyParm.SourceColumn = "gea02";
            this.TabDetailList[0].RelationParams.Add(keyParm);
            keyParm = new SqlParameter("geb03", SqlDbType.Decimal);
            keyParm.SourceColumn = "gea03";
            this.TabDetailList[0].RelationParams.Add(keyParm);
            keyParm = new SqlParameter("geb04", SqlDbType.Decimal);
            keyParm.SourceColumn = "gea04";
            this.TabDetailList[0].RelationParams.Add(keyParm);

            return true;
        }
        #endregion

        #region WfIniDetailComboSource 處理grid下拉選單
        protected override void WfIniDetailComboSource(int pTabIndex)
        {
            UltraGrid uGrid;
            UltraGridColumn ugc;
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                switch (pTabIndex)
                {
                    case 0:
                        //借/貸
                        sourceList = new List<KeyValuePair<string, string>>();
                        sourceList.Add(new KeyValuePair<string, string>("1", "1.借"));
                        sourceList.Add(new KeyValuePair<string, string>("2", "2.貸"));
                        uGrid = TabDetailList[pTabIndex].UGrid;
                        ugc = uGrid.DisplayLayout.Bands[0].Columns["geb09"];    
                        WfSetGridValueList(ugc, sourceList);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["geacomp"] = LoginInfo.CompNo;
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
                        pDr["geb05"] = WfGetMaxSeq(pDr.Table, "geb05");
                        pDr["geb10"] = 0;
                        pDr["geb11"] = 0;
                        pDr["geb12"] = 0;
                        pDr["geb13"] = 0;
                        pDr["gebcomp"] = LoginInfo.CompNo;
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
        
        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_glat300 masterModel = null;
            try
            {
                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false); //先全開


                    WfSetControlsReadOnlyRecursion(uTab_Master,true);
                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "geb05","geb06_c" }, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreInUpdateModeCheck() 進修改模式前檢查,及設定變數
        protected override bool WfPreInUpdateModeCheck()
        {
            vw_glat200 masterModel;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            int chkCnts=0;
            try
            {
                masterModel = DrMaster.ToItem<vw_glat200>();

                if (!GlobalFn.varIsNull(masterModel.gea06))
                {
                    WfShowBottomStatusMsg("已拋轉傳票,不可修改!");
                    return false;
                }

                if (masterModel.gea02=="AR" && masterModel.gea03==1)    //應收帳款
                {
                    sqlSelect = @"SELECT COUNT(1) FROM cea_tb WHERE cea01=@cea01 AND ceaconf<>'N' ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@cea01",masterModel.gea01));
                    chkCnts = GlobalFn.isNullRet(BoGla.OfGetFieldValue(sqlSelect,sqlParmList.ToArray()),0);
                    if (chkCnts !=0)
                    {
                        WfShowBottomStatusMsg("單據非未確認狀態,不可修改!");
                        return false;
                    }
                }

                if (masterModel.gea02 == "AR" && masterModel.gea03 == 2)    //應收帳款
                {
                    sqlSelect = @"SELECT COUNT(1) FROM cfa_tb WHERE cfa01=@cfa01 AND cfaconf<>'N' ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@cea01", masterModel.gea01));
                    chkCnts = GlobalFn.isNullRet(BoGla.OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0);
                    if (chkCnts != 0)
                    {
                        WfShowBottomStatusMsg("單據非未確認狀態,不可修改!");
                        return false;
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
        
        #region WfPreDeleteCheck 進主檔刪除前檢查
        protected override bool WfPreDeleteCheck(DataRow pDr)
        {
            vw_glat200 masterModel;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            int chkCnts = 0;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_glat200>();

                masterModel = DrMaster.ToItem<vw_glat200>();

                if (!GlobalFn.varIsNull(masterModel.gea06))
                {
                    WfShowBottomStatusMsg("已拋轉傳票,不可刪除!");
                    return false;
                }
                if (masterModel.gea02 == "AR" && masterModel.gea03 == 1)    //應收帳款
                {
                    sqlSelect = @"SELECT COUNT(1) FROM cea_tb WHERE cea01=@cea01 AND ceaconf<>'N' ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@cea01", masterModel.gea01));
                    chkCnts = GlobalFn.isNullRet(BoGla.OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0);
                    if (chkCnts != 0)
                    {
                        WfShowBottomStatusMsg("單據非未確認狀態,不可刪除!");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                WfRollback();

                DrMaster.RejectChanges();
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
                WfSetFirstVisibelCellFocus(TabDetailList[0].UGrid);

                SelectNextControl(this.uTab_Master, true, true, true, false);
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
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["geasecu"] = LoginInfo.UserNo;
                        DrMaster["geasecg"] = LoginInfo.GroupNo;
                        DrMaster["geacreu"] = LoginInfo.UserNo;
                        DrMaster["geacreg"] = LoginInfo.DeptNo;
                        DrMaster["geacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["geamodu"] = LoginInfo.UserNo;
                        DrMaster["geamodg"] = LoginInfo.DeptNo;
                        DrMaster["geamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["gebcreu"] = LoginInfo.UserNo;
                            drDetail["gebcreg"] = LoginInfo.DeptNo;
                            drDetail["gebcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["gebmodu"] = LoginInfo.UserNo;
                            drDetail["gebmodg"] = LoginInfo.DeptNo;
                            drDetail["gebmodd"] = Now;
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

    }
}
