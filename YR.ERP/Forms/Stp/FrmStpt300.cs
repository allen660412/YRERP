/* 程式名稱: 訂單維護作業
   系統代號: stpt300
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


namespace YR.ERP.Forms.Stp
{
    public partial class FrmStpt300 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        StpBLL BoStp = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;

        baa_tb BaaTbModel = null;
        bek_tb BekTbModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmStpt300()
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
            this.StrFormID = "stpt300";
            uTab_Header.Tabs[0].Text = "基本資料";
            uTab_Header.Tabs[1].Text = "狀態";

            this.IntTabCount = 2;
            this.IntMasterGridPos = 2;
            uTab_Master.Tabs[0].Text = "資料內容";
            uTab_Master.Tabs[1].Text = "資料瀏覽";

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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("sfa01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "sfasecu";
                TabMaster.GroupColumn = "sfasecg";
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

            BoStp = new StpBLL(BoMaster.OfGetConntion());
            BoBas = new BasBLL(BoMaster.OfGetConntion());
            BoInv = new InvBLL(BoMaster.OfGetConntion());
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
                    BoStp.TRAN = BoMaster.TRAN;
                    BoBas.TRAN = BoMaster.TRAN;
                    BoInv.TRAN = BoMaster.TRAN;
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
                //課稅別
                sourceList = BoStp.OfGetTaxTypeKVPList();
                WfSetUcomboxDataSource(ucb_sfa06, sourceList);

                //發票聯數
                sourceList = BoStp.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_sfa09, sourceList);

                //單據確認
                sourceList = BoStp.OfGetSfaconfKVPList();
                WfSetUcomboxDataSource(ucb_sfaconf, sourceList);

                //單據狀態
                sourceList = BoStp.OfGetSfastatKVPList();
                WfSetUcomboxDataSource(ucb_sfastat, sourceList);
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

            this.TabDetailList[0].TargetTable = "sfb_tb";
            this.TabDetailList[0].ViewTable = "vw_stpt300s";
            keyParm = new SqlParameter("sfb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "sfa01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion
        
        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_stpt300 masterModel = null;
            try
            {                
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_stpt300>();
                    WfSetDocPicture("", masterModel.sfaconf, masterModel.sfastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.sfa10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.sfa10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.sfa10));
                        }
                    }
                }
                else
                {
                    WfSetDocPicture("", "", "", pbxDoc);
                }
                
                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false); //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯

                    WfSetControlReadonly(new List<Control> { ute_sfacreu, ute_sfacreg, udt_sfacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_sfamodu, ute_sfamodg, udt_sfamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_sfasecu, ute_sfasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.sfa01, "") != "")
                    {
                        WfSetControlReadonly(ute_sfa01, true);
                        WfSetSfa01RelReadonly(GlobalFn.isNullRet(masterModel.sfa01, ""));
                    }

                    if (!GlobalFn.varIsNull(masterModel.sfa19)) //客戶狀態處理
                        WfSetControlReadonly(ute_sfa03, true);

                    WfSetControlReadonly(new List<Control> { ute_sfa01_c, ute_sfa03_c, ute_sfa04_c, ute_sfa05_c, ute_sfa11_c, ute_sfa12_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_sfa07, ucx_sfa08 }, true);
                    WfSetControlReadonly(new List<Control> { ute_sfa13, ute_sfa13t, ute_sfa13g }, true);
                    WfSetControlReadonly(new List<Control> { ute_sfa14_c, ute_sfa15_c, ute_sfa16_c }, true);
                    WfSetControlReadonly(new List<Control> { ucb_sfaconf, udt_sfacond, ute_sfaconu, ucb_sfastat }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_sfa01, true);
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
                            //先控可以輸入的
                            if (
                                columnName == "sfb05" ||
                                columnName == "sfb06" ||
                                columnName == "sfb09" ||
                                columnName == "sfb16"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "sfb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }

                            if (columnName == "sfb03" ||
                                columnName == "sfb11" ||
                                columnName == "sfb12"
                                )
                            {
                                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sfa01"], ""));
                                if (babModel == null)
                                {
                                    WfShowErrorMsg("請先輸入訂單頭資料!");
                                }
                                if (GlobalFn.isNullRet(babModel.bab08, "") == "Y")    //有來源單據
                                {
                                    if (columnName == "sfb03")//料號
                                        WfSetControlReadonly(ugc, true);    //不可輸入
                                    else
                                        WfSetControlReadonly(ugc, false);
                                }
                                else
                                {
                                    if (columnName == "sfb03")//料號
                                        WfSetControlReadonly(ugc, false);    //可輸入
                                    else
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

        #region WfAfterfDisplayMode  新增修改刪除查詢後的 focus調整
        protected override void WfAfterfDisplayMode()
        {
            try
            {
                uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                SelectNextControl(this.uTab_Header, true, true, true, false);
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
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                vw_stpt300 masterModel = null;
                vw_stpt300s detailModel = null;
                #region 單頭-pick vw_stpt300
                if (pDr.Table.Prefix.ToLower() == "vw_stpt300")
                {
                    masterModel = DrMaster.ToItem<vw_stpt300>();
                    switch (pColName.ToLower())
                    {
                        case "sfa01"://訂單單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "stp"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "20"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "sfa03"://客戶編號
                            WfShowPickUtility("p_sca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sca01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;
                        case "sfa04"://業務人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;
                        case "sfa05"://業務部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sfa10"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                            
                        case "sfa11"://收款條件
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@bef01", "2"));
                            WfShowPickUtility("p_bef1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bef02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sfa12"://取價條件
                            WfShowPickUtility("p_sbb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sfa14"://貨運方式
                            WfShowPickUtility("p_bel1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bel01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sfa15"://貨運起點
                            WfShowPickUtility("p_sbg1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbg01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sfa16"://貨運終點
                            WfShowPickUtility("p_sbg1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbg01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sfa18"://送貨地址碼
                            if (GlobalFn.varIsNull(masterModel.sfa03))
                                return false;
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@scb01", masterModel.sfa03));
                            WfShowPickUtility("p_scb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["scb02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                            
                        case "sfa19"://報價單號
                            if (!GlobalFn.varIsNull(masterModel.sfa03))
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@sea03", masterModel.sfa03));
                                messageModel.StrWhereAppend = " AND sea03=@sea03";
                            }

                            WfShowPickUtility("p_seb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["seb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_stpt300s
                if (pDr.Table.Prefix.ToLower() == "vw_stpt300s")
                {
                    masterModel = DrMaster.ToItem<vw_stpt300>();
                    detailModel = pDr.ToItem<vw_stpt300s>();
                    switch (pColName.ToLower())
                    {
                        case "sfb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sfb06"://訂單單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sfb11"://報價單號
                            if (GlobalFn.isNullRet(masterModel.sfa03, "") == "")
                                WfShowPickUtility("p_seb1", messageModel);
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@sea03", masterModel.sfa03));
                                WfShowPickUtility("p_seb2", messageModel);
                            }
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                {
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["seb01"], "");
                                    pDr["sfb12"] = GlobalFn.isNullRet(messageModel.DataRowList[0]["seb02"], "");
                                }
                                else
                                {
                                    pDr[pColName] = "";
                                    pDr["sfb12"] = "";
                                }
                            }
                            break;

                        case "sfb16"://倉庫
                            if (GlobalFn.isNullRet(detailModel.sfb03, "") == "")
                                WfShowPickUtility("p_icb1", messageModel);
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.sfb03));
                                WfShowPickUtility("p_icc1", messageModel);
                            }
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icc02"], "");
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["sfa02"] = Today;
                pDr["sfa04"] = LoginInfo.UserNo;
                pDr["sfa04_c"] = LoginInfo.UserName;
                pDr["sfa05"] = LoginInfo.DeptNo;
                pDr["sfa05_c"] = LoginInfo.DeptName;
                pDr["sfa07"] = 0;
                pDr["sfa08"] = "N";
                pDr["sfa10"] = BaaTbModel.baa04;
                pDr["sfa13"] = 0;
                pDr["sfa13t"] = 0;
                pDr["sfa13g"] = 0;
                pDr["sfa21"] = 1;       //匯率
                pDr["sfaconf"] = "N";
                pDr["sfastat"] = "0";
                pDr["sfacomp"] = LoginInfo.CompNo;
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
                        pDr["sfb02"] = WfGetMaxSeq(pDr.Table, "sfb02");
                        pDr["sfb05"] = 0;
                        pDr["sfb08"] = 0;
                        pDr["sfb09"] = 0;
                        pDr["sfb10"] = 0;
                        pDr["sfb10t"] = 0;
                        if (GlobalFn.isNullRet(DrMaster["sfa19"], "") != "")
                            pDr["sfb11"] = DrMaster["sfa19"];
                        pDr["sfb13"] = 0;
                        pDr["sfb14"] = 0;
                        pDr["sfb17"] = 0;
                        pDr["sfb18"] = DrMaster["sfa02"];
                        pDr["sfbcomp"] = LoginInfo.CompNo;
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

        #region WfPreInInsertModeCheck 進新增模式前的檢查及清變數與設定變數
        protected override bool WfPreInInsertModeCheck()
        {
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreInUpdateModeCheck() 進存檔模式前檢查,及設定變數
        protected override bool WfPreInUpdateModeCheck()
        {
            vw_stpt300 masterModel;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_stpt300>();
                if (masterModel.sfaconf != "N")
                    return false;
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
            vw_stpt300 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_stpt300>();
                if (masterModel.sfaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可刪除!");
                    return false;
                }

                //還需檢查出貨單
                if (WfChkSgaExists(masterModel.sfa01) == true)
                {
                    WfShowErrorMsg("已有出貨資料!不可取消確認!");
                    return false;
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_stpt300 masterModel = null;
            vw_stpt300s detailModel = null, newDetailModel = null;
            List<vw_stpt300s> detailList = null;
            bab_tb babModel = null;
            sea_tb seaModel = null;
            seb_tb sebModel = null;
            DataTable dtSeb = null;
            UltraGrid uGrid = null;
            DataRow drSfbNew = null;
            decimal price = 0;
            Result result = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt300>();
                if (e.Column.ToLower() != "sfa01" && GlobalFn.isNullRet(DrMaster["sfa01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false;
                }
                #region 單頭 vw_stpt300
                if (e.Row.Table.Prefix.ToLower() == "vw_stpt300")
                {
                    switch (e.Column.ToLower())
                    {
                        case "sfa01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "stp", "20") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["sfa01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetSfa01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            if (ute_sfa19.ReadOnly != true)
                                WfItemChkForceFocus(ute_sfa19);
                            break;
                        case "sfa03"://客戶編號
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sfa03_c"] = "";
                                e.Row["sfa14"] = "";    //送貨地址
                                e.Row["sfa15"] = "";    //起運地
                                e.Row["sfa16"] = "";    //到達地
                                e.Row["sfa17"] = "";    //客戶單號
                                e.Row["sfa18"] = "";    //送貨地址碼
                                e.Row["sfa19"] = "";    //報價單號
                                return true;
                            }
                            if (BoStp.OfChkScaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此客戶資料,請檢核!");
                                return false;
                            }
                            WfSetSfa03Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtBySfa06();
                            break;

                        case "sfa04"://業務人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sfa04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["sfa04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sfa05"://業務部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sfa05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["sfa05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "sfa06"://課稅別
                            WfSetSfa06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtBySfa06();
                            break;

                        case "sfa10"://幣別
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBekPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此幣別");
                                return false;
                            }
                            BekTbModel = BoBas.OfGetBekModel(e.Value.ToString());
                            break;

                        case "sfa11"://付款條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sfa11_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("2", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此收款條件,請檢核!");
                                return false;
                            }
                            e.Row["sfa11_c"] = BoBas.OfGetBef03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sfa12"://取價條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sfa12_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbbPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此取價條件,請檢核!");
                                return false;
                            }
                            e.Row["sfa12_c"] = BoStp.OfGetSbb02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sfa14"://貨運方式
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sfa14_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBelPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運方式,請檢核!");
                                return false;
                            }
                            e.Row["sfa14_c"] = BoBas.OfGetBel02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sfa15"://貨運起點
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sfa15_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbgPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運起點,請檢核!");
                                return false;
                            }
                            e.Row["sfa15_c"] = BoStp.OfGetSbg02(GlobalFn.isNullRet(e.Value, ""));
                            break;
                            
                        case "sfa16"://貨運終點
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sfa16_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbgPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運終點,請檢核!");
                                return false;
                            }
                            e.Row["sfa16_c"] = BoStp.OfGetSbg02(GlobalFn.isNullRet(e.Value, ""));
                            break;
                            
                        case "sfa19"://報價單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfSetControlReadonly(new List<Control> { ute_sfa03 }, false);
                                DrMaster["sfa12"] = "";
                                DrMaster["sfa12_c"] = "";
                                return true;
                            }
                            seaModel = BoStp.OfGetSeaModel(GlobalFn.isNullRet(e.Value, ""));
                            if (seaModel == null)
                            {
                                WfShowErrorMsg("無此報價單!");
                                return false;
                            }
                            if (seaModel.seaconf != "Y")
                            {
                                WfShowErrorMsg("報價單非確認狀態!");
                                return false;
                            }
                            if (seaModel.sea03 != masterModel.sfa03)
                            {
                                WfShowErrorMsg("客戶不同,請檢核!");
                                return false;
                            }

                            //這裡會有客戶跟訂單搶壓資料的問題:先以訂單為主
                            DrMaster["sfa03"] = seaModel.sea03;
                            DrMaster["sfa03_c"] = BoStp.OfGetSca02(seaModel.sea03);
                            WfSetControlReadonly(new List<Control> { ute_sfa03 }, true);
                            DrMaster["sfa06"] = seaModel.sea06;
                            DrMaster["sfa07"] = seaModel.sea07;
                            DrMaster["sfa08"] = seaModel.sea08;
                            DrMaster["sfa09"] = seaModel.sea09;
                            DrMaster["sfa10"] = seaModel.sea10;
                            DrMaster["sfa11"] = seaModel.sea11;
                            DrMaster["sfa11_c"] = BoBas.OfGetBef03("2", seaModel.sea11);   //收款條件
                            DrMaster["sfa12"] = seaModel.sea12;
                            DrMaster["sfa12_c"] = BoStp.OfGetSbb02(seaModel.sea12);
                            DrMaster["sfa14"] = seaModel.sea14;
                            DrMaster["sfa15"] = seaModel.sea15;
                            DrMaster["sfa16"] = seaModel.sea16;
                            dtSeb = BoStp.OfGetSebDt(e.Value.ToString());
                            if (dtSeb != null && dtSeb.Rows.Count > 0)
                            {
                                var dialogResult = WfShowConfirmMsg("是否要轉入報價單資料?");

                                //if (WfShowConfirmMsg("是否要轉入報價單資料?")==1)
                                if (dialogResult == DialogResult.Yes)
                                {
                                    foreach (DataRow drTemp in dtSeb.Rows)
                                    {
                                        sebModel = drTemp.ToItem<seb_tb>();
                                        drSfbNew = TabDetailList[0].DtSource.NewRow();
                                        TabDetailList[0].DtSource.Rows.Add(drSfbNew);
                                        WfSetDetailRowDefault(0, drSfbNew);
                                        drSfbNew["sfb11"] = sebModel.seb01;
                                        drSfbNew["sfb12"] = sebModel.seb02;
                                        WfSetSfb12Relation(drSfbNew, sebModel.seb01, Convert.ToInt16(sebModel.seb02));
                                    }
                                }
                                WfSetTotalAmt();
                            }
                            break;
                        case "sfa21": //匯率
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0m) <= 0)
                            {
                                WfShowErrorMsg("匯率應大於0,請檢核!");
                                return false;
                            }
                            break;
                    }
                }
                #endregion

                #region 單身 vw_stpt300s
                if (e.Row.Table.Prefix.ToLower() == "vw_stpt300s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_stpt300s>();
                    babModel = BoBas.OfGetBabModel(masterModel.sfa01);

                    switch (e.Column.ToLower())
                    {
                        case "sfb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            detailList = e.Row.Table.ToList<vw_stpt300s>();
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.sfb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "sfb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sfb04"] = "";//品名
                                e.Row["sfb05"] = 0;//訂單數量
                                e.Row["sfb06"] = "";//訂單單位
                                e.Row["sfb07"] = "";//庫存單位
                                e.Row["sfb08"] = 0;//庫存轉換率
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }
                            
                            if (WfSetSfb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;

                            if (!GlobalFn.varIsNull(masterModel.sfa12))
                            {
                                newDetailModel = e.Row.ToItem<vw_stpt300s>();
                                result=BoStp.OfGetPrice(masterModel.sfa12, masterModel.sfa03, e.Value.ToString(), newDetailModel.sfb06,
                                    masterModel.sfa02, masterModel.sfa10, "2", newDetailModel.sfb05,
                                    masterModel.sfa08, masterModel.sfa07, masterModel.sfa21, out price);
                                if (result.Success == true)
                                {
                                    e.Row["sfb09"] = price;
                                }
                                else
                                {
                                    WfShowErrorMsg(result.Message);
                                    return false;
                                }
                            }
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "sfb05"://訂單數量
                            if (GlobalFn.varIsNull(detailModel.sfb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sfb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detailModel.sfb06))
                            {
                                WfShowErrorMsg("請先輸入訂單單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sfb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入訂單數量!");
                                return false;
                            }
                            detailModel.sfb05 = BoBas.OfGetUnitRoundQty(detailModel.sfb06, detailModel.sfb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.sfb05;
                            //if (WfChkSfb05(pDr, listDetail) == false)
                            //    return false;
                            e.Row["sfb14"] = BoBas.OfGetUnitRoundQty(detailModel.sfb15, detailModel.sfb05 * detailModel.sfb13); //轉換報價數量(並四拾伍入)
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "sfb06"://訂單單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.sfb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sfb03"]);
                                return false;
                            }
                            if (WfChkSfb06(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;

                            if (WfSetSfb06Relation(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;
                            break;

                        case "sfb09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.sfb09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.sfb09, BekTbModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "sfb11"://報價單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("報價單號不可為空白!");
                                return false;
                            }

                            if (WfChkSfb11(e.Value.ToString()) == false)
                                return false;

                            if (!GlobalFn.varIsNull(detailModel.sfb12))
                            {
                                if (WfChkSfb12(GlobalFn.isNullRet(detailModel.sfb11, ""), GlobalFn.isNullRet(detailModel.sfb12, 0)) == false)
                                    return false;

                                if (WfSetSfb12Relation(e.Row, GlobalFn.isNullRet(detailModel.sfb11, ""), GlobalFn.isNullRet(detailModel.sfb12, 0)) == false)
                                    return false;
                            }
                            WfSetTotalAmt();
                            break;

                        case "sfb12"://報價項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("報價項次不可為空白!");
                                return false;
                            }

                            if (WfChkSfb12(GlobalFn.isNullRet(detailModel.sfb11, ""), GlobalFn.isNullRet(detailModel.sfb12, 0)) == false)
                                return false;
                            WfSetSfb12Relation(e.Row, GlobalFn.isNullRet(detailModel.sfb11, ""), GlobalFn.isNullRet(detailModel.sfb12, 0));
                            WfSetTotalAmt();
                            break;

                        case "sfb16"://倉庫
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoInv.OfChkIcbPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此倉庫別,請確認!");
                                return false;
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

        #region WfPreInsertDetailCheck() :新增明細資料前檢查
        protected override bool WfPreInsertDetailCheck(int pCurTabDetail)
        {
            try
            {
                if (WfFormCheck() == false)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterDetailDelete() :刪除明細後調用
        protected override bool WfAfterDetailDelete(int pCurTabDetail, DataRow pDr)
        {
            try
            {
                switch (pDr.Table.Prefix.ToLower())
                {
                    case "vw_stpt300s":
                        WfSetTotalAmt();
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_stpt300 masterModel = null;
            vw_stpt300s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {

                masterModel = DrMaster.ToItem<vw_stpt300>();
                if (!GlobalFn.varIsNull(masterModel.sfa01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.sfa01, ""));
                #region 單頭資料檢查
                chkColName = "sfa01";       //訂單單號
                chkControl = ute_sfa01;
                if (GlobalFn.varIsNull(masterModel.sfa01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sfa02";       //訂單日期
                chkControl = udt_sfa02;
                if (GlobalFn.varIsNull(masterModel.sfa02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sfa03";       //客戶編號
                chkControl = ute_sfa03;
                if (GlobalFn.varIsNull(masterModel.sfa03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sfa04";       //業務人員
                chkControl = ute_sfa04;
                if (GlobalFn.varIsNull(masterModel.sfa04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sfa05";       //業務部門
                chkControl = ute_sfa05;
                if (GlobalFn.varIsNull(masterModel.sfa05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sfa06";       //課稅別
                chkControl = ucb_sfa06;
                if (GlobalFn.varIsNull(masterModel.sfa06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sfa12";
                chkControl = ute_sfa12;
                if (GlobalFn.varIsNull(masterModel.sfa12) && babModel.bab08 != "Y")//無來源單據取價條件要輸入
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sfa21";
                chkControl = ute_sfa21;
                if (GlobalFn.varIsNull(masterModel.sfa21))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion
                
                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    detailModel = drTemp.ToItem<vw_stpt300s>();
                    chkColName = "sfb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.sfb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sfb11";   //報價單號
                    if (GlobalFn.varIsNull(detailModel.sfb11) && babModel.bab08 == "Y")  //有來源單據報價單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sfb12";   //報價項次
                    if (GlobalFn.varIsNull(detailModel.sfb12) && babModel.bab08 == "Y")  //有來源單據報價單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sfb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.sfb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sfb05";   //訂單數量
                    #region sfb05 訂單數量
                    if (GlobalFn.varIsNull(detailModel.sfb05) || detailModel.sfb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    //if (WfChkPfb05(drTemp, listDetail) == false)
                    //{
                    //    this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                    //    WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                    //    return false;
                    //}
                    #endregion
                    
                    chkColName = "sfb06";   //訂單單位
                    if (GlobalFn.varIsNull(detailModel.sfb06))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }                                        

                    chkColName = "sfb18";   //預定交貨日
                    if (GlobalFn.varIsNull(detailModel.sfb18))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
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
            string sfa01New, errMsg;
            vw_stpt300 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt300>();
                if (FormEditMode == YREditType.新增)
                {
                    sfa01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.sfa01, ModuleType.stp, (DateTime)masterModel.sfa02, out sfa01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["sfa01"] = sfa01New;
                }
                
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["sfasecu"] = LoginInfo.UserNo;
                        DrMaster["sfasecg"] = LoginInfo.GroupNo;
                        DrMaster["sfacreu"] = LoginInfo.UserNo;
                        DrMaster["sfacreg"] = LoginInfo.DeptNo;
                        DrMaster["sfacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["sfamodu"] = LoginInfo.UserNo;
                        DrMaster["sfamodg"] = LoginInfo.DeptNo;
                        DrMaster["sfamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["sfbcreu"] = LoginInfo.UserNo;
                            drDetail["sfbcreg"] = LoginInfo.DeptNo;
                            drDetail["sfbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["sfbmodu"] = LoginInfo.UserNo;
                            drDetail["sfbmodg"] = LoginInfo.DeptNo;
                            drDetail["sfbmodd"] = Now;
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
        
        #region WfAddAction 新增action按鈕
        protected override List<ButtonTool> WfAddAction()
        {
            List<ButtonTool> buttonList = new List<ButtonTool>();
            ButtonTool bt;
            ado_tb adoModel;
            try
            {
                bt = new ButtonTool("Confirm");
                bt.SharedProps.Caption = "確認";
                bt.SharedProps.Category = "Confirm";
                buttonList.Add(bt);

                bt = new ButtonTool("CancelConfirm");
                bt.SharedProps.Caption = "取消確認";
                bt.SharedProps.Category = "CancelConfirm";
                buttonList.Add(bt);

                bt = new ButtonTool("Invalid");
                bt.SharedProps.Caption = "作廢";
                bt.SharedProps.Category = "Invalid";
                buttonList.Add(bt);

                bt = new ButtonTool("Stpr300");
                adoModel = BoAdm.OfGetAdoModel("Stpr300");
                bt.SharedProps.Caption = adoModel.ado02;
                bt.SharedProps.Category = "Report";
                bt.Tag = adoModel;
                buttonList.Add(bt);

                return buttonList;
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
            try
            {
                switch (pActionName)
                {
                    case "Confirm":
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfConfirm();
                        break;
                    case "CancelConfirm":
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfCancelConfirm();
                        break;
                    case "Invalid":
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfInvalid();
                        break;

                    case "Stpr300":
                        vw_stpr300 stpr300Model;
                        vw_stpt300 masterModel;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_stpt300>();
                        stpr300Model = new vw_stpr300();
                        stpr300Model.sfa01 = masterModel.sfa01;
                        stpr300Model.sfa03 = "";
                        stpr300Model.jump_yn = "N";
                        stpr300Model.order_by = "1";

                        FrmStpr300 rpt = new FrmStpr300(this.LoginInfo, stpr300Model, true, true);
                        rpt.WindowState = FormWindowState.Minimized;
                        rpt.ShowInTaskbar = false;
                        rpt.Show();
                        
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //*****************************表單自訂Fuction****************************************
        #region WfSetSfa01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetSfa01RelReadonly(string psfa01)
        {
            bab_tb babModel = null;
            string bab01;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_sfa01 }, true);
                bab01 = psfa01.Substring(0, GlobalFn.isNullRet(BaaTbModel.baa06, 0));
                babModel = BoBas.OfGetBabModel(bab01);
                if (babModel.bab08 == "Y")   //有來源單據
                {
                    WfSetControlReadonly(new List<Control> { ute_sfa12 }, true);
                    WfSetControlReadonly(new List<Control> { ute_sfa19 }, false);
                }
                else
                {
                    WfSetControlReadonly(new List<Control> { ute_sfa12 }, false);
                    WfSetControlReadonly(new List<Control> { ute_sfa19 }, true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSfa03Relation 設定客戶相關聯
        private void WfSetSfa03Relation(string pSfa03)
        {
            sca_tb scaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sfa01"], ""));
                scaModel = BoStp.OfGetScaModel(pSfa03);
                if (scaModel == null)
                    return;

                DrMaster["sfa03_c"] = scaModel.sca03;
                DrMaster["sfa06"] = scaModel.sca22;    //課稅別
                WfSetSfa06Relation(scaModel.sca22);
                DrMaster["sfa09"] = scaModel.sca23;    //發票聯數

                DrMaster["sfa11"] = scaModel.sca21;    //收款條件
                DrMaster["sfa11_c"] = BoBas.OfGetBef03("1", scaModel.sca21);
                //DRMASTER["pfa14"] = l_sca.sca35;    //送貨地址
                //DRMASTER["pfa15"] = l_sca.pca36;    //帳單地址

                if (babModel.bab08 != "Y")   //無前置單據則取價條件帶入
                {
                    DrMaster["sfa12"] = scaModel.sca24;    //取價條件
                    DrMaster["sfa12_c"] = BoStp.OfGetSbb02(scaModel.sca24);
                }
                else
                {
                    DrMaster["sfa12"] = "";
                    DrMaster["sfa12_c"] = "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSfa06Relation 設定稅別關聯
        private void WfSetSfa06Relation(string pSfa06)
        {
            try
            {
                if (pSfa06 == "1")
                {
                    DrMaster["sfa07"] = BaaTbModel.baa05;
                    DrMaster["sfa08"] = "Y";
                }
                else if (pSfa06 == "2")
                {
                    DrMaster["sfa07"] = BaaTbModel.baa05;
                    DrMaster["sfa08"] = "N";
                }
                else
                {
                    DrMaster["sfa07"] = 0;
                    DrMaster["sfa08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSfb03Relation 設定料號關聯
        private bool WfSetSfb03Relation(string pSfb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal seb08;
            try
            {
                icaModel = BoInv.OfGetIcaModel(pSfb03);
                seb08 = 0;
                if (icaModel == null)
                {
                    pDr["sfb04"] = "";//品名
                    pDr["sfb05"] = 0;//訂單數量
                    pDr["sfb06"] = "";//訂單單位
                    pDr["sfb07"] = "";//庫存單位
                    pDr["sfb08"] = 0;//庫存轉換率
                }
                else
                {
                    if (BoInv.OfGetUnitCovert(pSfb03, icaModel.ica09, icaModel.ica07, out seb08) == false)
                    {
                        WfShowErrorMsg("未設定料號轉換,請先設定!");
                        return false;
                    }
                    pDr["sfb04"] = icaModel.ica02;//品名
                    pDr["sfb05"] = 0;//訂單數量
                    pDr["sfb06"] = icaModel.ica09;//訂單單位帶銷售單位
                    pDr["sfb07"] = icaModel.ica07;//庫存單位
                    pDr["sfb08"] = seb08;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSfb06Relation 設定訂單單位關聯
        //檢查前要先確認料號是否已輸入
        private bool WfSetSfb06Relation(DataRow pDr, string pSfb06, string pBab08)
        {
            vw_stpt300s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_stpt300s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pSfb06, "")) == false)
                {
                    WfShowErrorMsg("無此採購單位!請確認");
                    return false;
                }
                //取得是否有採購對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.sfb03, pSfb06, detailModel.sfb07, out dConvert) == true)
                {
                    pDr["sfb08"] = dConvert;
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有訂單對報價的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.sfb03, pSfb06, detailModel.sfb15, out dConvert) == true)
                    {
                        pDr["sfb13"] = dConvert;
                        pDr["sfb14"] = BoBas.OfGetUnitRoundQty(pSfb06, dConvert * detailModel.sfb05);
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

        #region WfSetSfb12Relation 設定報價單明細資料關聯
        private bool WfSetSfb12Relation(DataRow pdr, string pSfb11, int pSfb12)
        {
            seb_tb sebModel;

            try
            {
                sebModel = BoStp.OfGetSebModel(pSfb11, pSfb12);
                if (sebModel == null)
                    return false;
                pdr["sfb03"] = sebModel.seb03;//料號
                pdr["sfb04"] = sebModel.seb04;//品名
                pdr["sfb05"] = sebModel.seb05;//訂單數量
                pdr["sfb06"] = sebModel.seb06;//訂單單位
                pdr["sfb07"] = sebModel.seb07;//庫存單位
                pdr["sfb08"] = sebModel.seb08;//庫存轉換率
                pdr["sfb09"] = sebModel.seb09;//單價
                pdr["sfb10"] = sebModel.seb10;//未稅金額
                pdr["sfb10t"] = sebModel.seb10t;//含稅金額
                pdr["sfb13"] = 1;          //訂單對報價轉換率
                pdr["sfb14"] = sebModel.seb05;//轉換報價數量
                pdr["sfb15"] = sebModel.seb06;//原報價單位

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkSfb06 檢查訂單單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkSfb06(DataRow pDr, string pSfb06, string pBab08)
        {
            vw_stpt300s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_stpt300s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pSfb06, "")) == false)
                {
                    WfShowErrorMsg("無此訂單單位!請確認");
                    return false;
                }
                //檢查是否有訂單對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.sfb03, pSfb06, detailModel.sfb07, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定訂單單位對庫存單位的轉換率,請先設定!");
                    return false;
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有訂單對報價的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.sfb03, pSfb06, detailModel.sfb15, out dConvert) == false)
                    {
                        WfShowErrorMsg("未設定訂單單位對報價單位的轉換率,請先設定!");
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

        #region WfChkSfb11 報價單檢查
        private bool WfChkSfb11(string pSfb11)
        {
            string errMsg;
            sea_tb seaModel;
            try
            {
                seaModel = BoStp.OfGetSeaModel(pSfb11);
                if (seaModel == null)
                {
                    errMsg = "查無此報價單!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (seaModel.seaconf != "Y")
                {
                    errMsg = "報價單非確認狀態!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //if (l_pea.peastat == "9")
                //{
                //    errMsg = "報價單已結案!";
                //    WfShowMsg(errMsg);
                //    return false;
                //}

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkSfb12 報價項次檢查
        private bool WfChkSfb12(string pSfb11, int pSfb12)
        {
            string errMsg;
            seb_tb sebModel;
            try
            {
                sebModel = BoStp.OfGetSebModel(pSfb11, pSfb12);
                if (sebModel == null)
                {
                    errMsg = "查無此報價項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //if ((l_peb.peb05 - l_peb.peb11) <= 0)
                //{
                //    errMsg = string.Format("無可用採購數量!(可用採購數量為{0})", Global_Fn.isNullRet(l_peb.peb05 - l_peb.peb11, 0));
                //    WfShowMsg(errMsg);
                //    return false;
                //}
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfConfirm 確認
        private void WfConfirm()
        {
            vw_stpt300 masterModel = null;
            List<vw_stpt300s> detailList = null;
            try
            {
                if (DrMaster == null)
                    return;
                masterModel = DrMaster.ToItem<vw_stpt300>();
                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                if (TabDetailList[0].DtSource != null && TabDetailList[0].DtSource.Rows.Count > 0)
                {
                    detailList = TabDetailList[0].DtSource.ToList<vw_stpt300s>();
                    if (WfUpdSdd(masterModel, detailList) == false)
                    {
                        WfRollback();
                        return;
                    }
                }

                DrMaster["sfastat"] = "1";
                DrMaster["sfaconf"] = "Y";
                DrMaster["sfacond"] = Today;
                DrMaster["sfaconu"] = LoginInfo.UserNo;
                DrMaster["sfamodu"] = LoginInfo.UserNo;
                DrMaster["sfamodg"] = LoginInfo.DeptNo;
                DrMaster["sfamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_stpt300>();
                WfSetDocPicture("", masterModel.sfaconf, masterModel.sfastat, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfCancelConfirm 取消確認
        private void WfCancelConfirm()
        {
            vw_stpt300 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_stpt300>();
                if (masterModel.sfaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    WfRollback();
                    return;
                }

                //還需檢查訂單
                if (WfChkSgaExists(masterModel.sfa01) == true)
                {
                    WfShowErrorMsg("已有出貨資料!不可取消確認!");
                    WfRollback();
                    return;
                }

                DrMaster["sfastat"] = "0";
                DrMaster["sfaconf"] = "N";
                DrMaster["sfacond"] = DBNull.Value;
                DrMaster["sfaconu"] = "";
                DrMaster["sfamodu"] = LoginInfo.UserNo;
                DrMaster["sfamodg"] = LoginInfo.DeptNo;
                DrMaster["sfamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_stpt300>();
                WfSetDocPicture("", masterModel.sfaconf, masterModel.sfastat, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfInvalid 作廢/作廢還原
        private void WfInvalid()
        {
            vw_stpt300 masterModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChkCnts = 0;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_stpt300>();

                if (masterModel.sfaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.sfaconf == "N")//走作廢
                {

                    DrMaster["sfastat"] = "X";
                    DrMaster["sfaconf"] = "X";
                    DrMaster["sfaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.sfaconf == "X")
                {
                    //檢查報價單是否為有效資料
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT COUNT(1)");
                    sbSql.AppendLine("FROM sea_tb");
                    sbSql.AppendLine("WHERE exists ");
                    sbSql.AppendLine("      (SELECT 1 FROM sfb_tb WHERE sfb01=@sfb01");
                    sbSql.AppendLine("      AND sfb11=sea01)");
                    sbSql.AppendLine("AND ISNULL(seaconf,'')<>'Y'");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@sfb01", GlobalFn.isNullRet(DrMaster["sfa01"], "")));
                    iChkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                    if (iChkCnts > 0)
                    {
                        WfShowErrorMsg("報價單含有未確認資料!不可作廢還原!");
                        WfRollback();
                        return;
                    }
                    DrMaster["sfastat"] = "0";
                    DrMaster["sfaconf"] = "N";
                    DrMaster["sfaconu"] = "";
                }
                DrMaster["sfamodu"] = LoginInfo.UserNo;
                DrMaster["sfamodg"] = LoginInfo.DeptNo;
                DrMaster["sfamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_stpt300>();
                WfSetDocPicture("", masterModel.sfaconf, masterModel.sfastat, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfConfirmChk 確認前檢查
        private bool WfConfirmChk()
        {
            vw_stpt300 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt300>();
                if (masterModel.sfaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    WfRollback();
                    return false;
                }

                //listDetails = TabDetailList[0].DtSource.ToList<vw_stpt300s>();
                //foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                //{
                //    listDetail = drTemp.ToItem<vw_purt300s>();
                //    if (WfChkPfb05(drTemp, listDetail) == false)
                //        return false;
                //}

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetDetailAmt 處理小計
        private bool WfSetDetailAmt(DataRow drSeb)
        {
            sfa_tb sfaModel;
            sfb_tb sfbModel;
            decimal sfb10t = 0, sfb10 = 0;
            try
            {
                sfaModel = DrMaster.ToItem<sfa_tb>();
                sfbModel = drSeb.ToItem<sfb_tb>();

                if (sfaModel.sfa08 == "Y")//稅內含
                {
                    sfb10t = sfbModel.sfb09 * sfbModel.sfb05;
                    sfb10t = GlobalFn.Round(sfb10t, BekTbModel.bek04);
                    sfb10 = sfb10t / (1 + (sfaModel.sfa07 / 100));
                    sfb10 = GlobalFn.Round(sfb10, BekTbModel.bek04);
                }
                else//稅外加
                {
                    sfb10 = sfbModel.sfb09 * sfbModel.sfb05;
                    sfb10 = GlobalFn.Round(sfb10, BekTbModel.bek04);
                    sfb10t = sfb10 * (1 + (sfaModel.sfa07 / 100));
                    sfb10t = GlobalFn.Round(sfb10t, BekTbModel.bek04);
                }
                drSeb["sfb10"] = sfb10;
                drSeb["sfb10t"] = sfb10t;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetTotalAmt 處理總計
        private bool WfSetTotalAmt()
        {
            sfa_tb sfaModel;
            decimal sfa13 = 0, sfa13t = 0, sfa13g;
            try
            {
                sfaModel = DrMaster.ToItem<sfa_tb>();
                if (sfaModel.sfa08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (sfb_tb l_sfb in TabDetailList[0].DtSource.ToList<sfb_tb>())
                    {
                        sfa13t += l_sfb.sfb10t;
                    }
                    sfa13t = GlobalFn.Round(sfa13t, BekTbModel.bek04);
                    sfa13 = sfa13t / (1 + sfaModel.sfa07 / 100);
                    sfa13 = GlobalFn.Round(sfa13, BekTbModel.bek04);
                    sfa13g = sfa13t - sfa13;

                }
                else//稅外加
                {
                    foreach (sfb_tb l_sfb in TabDetailList[0].DtSource.ToList<sfb_tb>())
                    {
                        sfa13 += l_sfb.sfb10;
                    }
                    sfa13 = GlobalFn.Round(sfa13, BekTbModel.bek04);
                    sfa13g = sfa13 * (sfaModel.sfa07 / 100);
                    sfa13g = GlobalFn.Round(sfa13g, BekTbModel.bek04);
                    sfa13t = sfa13 + sfa13g;
                }

                DrMaster["sfa13"] = sfa13;
                DrMaster["sfa13t"] = sfa13t;
                DrMaster["sfa13g"] = sfa13g;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkSgaExists 檢查出貨單是否存在
        private bool WfChkSgaExists(string pSfa01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParms;
            int iChkCnts = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sga_tb");
                sbSql.AppendLine("  INNER JOIN sgb_tb ON sga01=sgb01");
                sbSql.AppendLine("WHERE");
                sbSql.AppendLine(" sgaconf <>'X' ");
                sbSql.AppendLine(" AND sgb11=@sga11 ");
                sqlParms = new List<SqlParameter>();
                sqlParms.Add(new SqlParameter("@sga11", pSfa01));
                iChkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParms.ToArray()), 0);
                if (iChkCnts > 0)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfResetAmtBySfa06 依稅別更新單身及單頭的金額
        private void WfResetAmtBySfa06()
        {
            try
            {
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    WfSetDetailAmt(dr);
                    WfSetTotalAmt();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfUpdSdd 訂單確認時,更新客戶產品價格檔
        private bool WfUpdSdd(vw_stpt300 pMasterModel, List<vw_stpt300s> pDetailList)
        {
            StpBLL boSddAdd = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            DataTable dtSdd = null;
            DataRow drSdd = null;
            try
            {
                boSddAdd = new StpBLL(BoMaster.OfGetConntion());
                boSddAdd.TRAN = BoMaster.TRAN;
                boSddAdd.OfCreateDao("sdd_tb", "*", "");
                sqlSelect = @"SELECT * FROM sdd_tb
                              WHERE sdd01=@sdd01 AND sdd02=@sdd02 AND sdd03=@sdd03
                            ";
                foreach (vw_stpt300s detailModel in pDetailList)
                {
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@sdd01", detailModel.sfb03)); //料號
                    sqlParmList.Add(new SqlParameter("@sdd02", pMasterModel.sfa03)); //客戶編號
                    sqlParmList.Add(new SqlParameter("@sdd03", pMasterModel.sfa10)); //幣別
                    dtSdd = boSddAdd.OfGetDataTable(sqlSelect, sqlParmList.ToArray());
                    if (dtSdd.Rows.Count == 0) //新增
                    {
                        drSdd = dtSdd.NewRow();
                        drSdd["sdd01"] = detailModel.sfb03;//料號
                        drSdd["sdd02"] = pMasterModel.sfa03;//客戶編號
                        drSdd["sdd03"] = pMasterModel.sfa10;//幣別
                        drSdd["sdd04"] = pMasterModel.sfa02;//最近訂單日期
                        drSdd["sdd05"] = detailModel.sfb06;//銷售單位
                        drSdd["sdd06"] = pMasterModel.sfa06;//稅別
                        drSdd["sdd07"] = pMasterModel.sfa07;//稅率
                        drSdd["sdd08"] = pMasterModel.sfa08;//含稅否
                        drSdd["sdd09"] = detailModel.sfb09;//銷售單價
                        drSdd["sdd10"] = detailModel.sfb05;//最近訂單數量
                        if (pMasterModel.sfa08 == "Y")
                            drSdd["sdd11"] = detailModel.sfb10t;//最近訂單金額
                        else
                            drSdd["sdd11"] = detailModel.sfb10;//最近訂單金額

                        drSdd["sddcreu"] = LoginInfo.UserNo;
                        drSdd["sddcreg"] = LoginInfo.DeptNo;
                        drSdd["sddcred"] = Now;
                        //drSddNew["sddmodu"] = "";
                        //drSddNew["sddmodg"] = "";
                        //drSddNew["sddmodd"] = "";
                        drSdd["sddsecu"] = LoginInfo.UserNo;
                        drSdd["sddsecg"] = LoginInfo.GroupNo;
                        dtSdd.Rows.Add(drSdd);
                    }
                    else    //修改
                    {
                        drSdd = dtSdd.Rows[0];
                        //drSdd["sdd01"] = detailModel.sfb03;//料號
                        //drSdd["sdd02"] = pMasterModel.sfa03;//客戶編號
                        //drSdd["sdd03"] = pMasterModel.sfa10;//幣別
                        drSdd["sdd04"] = pMasterModel.sfa02;//最近訂單日期
                        drSdd["sdd05"] = detailModel.sfb06;//銷售單位
                        drSdd["sdd06"] = pMasterModel.sfa06;//稅別
                        drSdd["sdd07"] = pMasterModel.sfa07;//稅率
                        drSdd["sdd08"] = pMasterModel.sfa08;//含稅否
                        drSdd["sdd09"] = detailModel.sfb09;//銷售單價
                        drSdd["sdd10"] = detailModel.sfb05;//最近訂單數量
                        if (pMasterModel.sfa08 == "Y")
                            drSdd["sdd11"] = detailModel.sfb10t;//最近訂單金額
                        else
                            drSdd["sdd11"] = detailModel.sfb10;//最近訂單金額

                        //drSdd["sddcreu"] = LoginInfo.UserNo;
                        //drSdd["sddcreg"] = LoginInfo.DeptNo;
                        //drSdd["sddcred"] = Now;
                        drSdd["sddmodu"] = LoginInfo.UserNo;
                        drSdd["sddmodg"] = LoginInfo.DeptNo;
                        drSdd["sddmodd"] = Now;
                        drSdd["sddsecu"] = LoginInfo.UserNo;
                        drSdd["sddsecg"] = LoginInfo.GroupNo;
                    }

                    if (boSddAdd.OfUpdate(dtSdd) < 1)
                    {
                        WfShowErrorMsg("異動料號客戶金額(sdd_tb)失敗!");
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
    }
}
