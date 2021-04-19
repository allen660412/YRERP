/* 程式名稱: 借出歸還單單建立作業
   系統代號: invt402
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

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvt402 : YR.ERP.Base.Forms.FrmEntryMDBase
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
        public FrmInvt402()
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
            this.StrFormID = "invt402";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ima01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "imasecu";
                TabMaster.GroupColumn = "imasecg";
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
                WfSetUcomboxDataSource(ucb_ima06, sourceList);

                //發票聯數
                sourceList = BoStp.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_ima09, sourceList);

                //單據確認
                sourceList = BoInv.OfGetImaconfKVPList();
                WfSetUcomboxDataSource(ucb_imaconf, sourceList);
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

            this.TabDetailList[0].TargetTable = "imb_tb";
            this.TabDetailList[0].ViewTable = "vw_invt402s";
            keyParm = new SqlParameter("imb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "ima01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_invt402 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_invt402>();
                    WfSetDocPicture("", masterModel.imaconf, masterModel.imastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.ima10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.ima10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.ima10));
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
                    WfSetControlReadonly(new List<Control> { ute_imacreu, ute_imacreg, udt_imacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_imamodu, ute_imamodg, udt_imamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_imasecu, ute_imasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.ima01, "") != "")
                    {
                        WfSetControlReadonly(ute_ima01, true);
                        WfSetIma01RelReadonly(GlobalFn.isNullRet(masterModel.ima01, ""));
                    }
                    //else
                    //    WfSetControlReadonly(ute_ila19, true);

                    if (!GlobalFn.varIsNull(masterModel.ima11)) //客戶狀態處理
                        WfSetControlReadonly(ute_ima03, true);

                    WfSetControlReadonly(new List<Control> { ute_ima01_c, ute_ima03_c, ute_ima04_c, ute_ima05_c, }, true);
                    WfSetControlReadonly(new List<Control> { ute_ima07, ucx_ima08 }, true);
                    WfSetControlReadonly(new List<Control> { ute_ima13, ute_ima13t, ute_ima13g }, true);
                    WfSetControlReadonly(new List<Control> { ucb_imaconf, ude_imacond, ute_imaconu }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_ima01, true);
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
                                columnName == "imb05" ||
                                columnName == "imb09" ||
                                columnName == "imb17" ||
                                columnName == "imb19"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "imb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }

                            //有來源單據時
                            if (columnName == "imb03" ||
                                columnName == "imb11" ||
                                columnName == "imb12" ||
                                columnName == "imb18"
                                )
                            {
                                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["ima01"], ""));
                                if (babModel == null)
                                {
                                    WfShowErrorMsg("請先輸入歸還單號!");
                                }
                                if (GlobalFn.isNullRet(babModel.bab08, "") == "Y")    //有來源單據
                                {
                                    if (columnName == "imb03" ||
                                        columnName == "imb18"
                                        )//料號或出庫倉
                                        WfSetControlReadonly(ugc, true);    //不可輸入
                                    else
                                        WfSetControlReadonly(ugc, false);
                                }
                                else
                                {
                                    if (columnName == "imb03" ||
                                        columnName == "imb18"
                                        )//料號或出庫倉
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
                vw_invt402 masterModel = null;
                vw_invt402s detailModel = null;
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_invt401
                if (pDr.Table.Prefix.ToLower() == "vw_invt402")
                {
                    switch (pColName.ToLower())
                    {
                        case "ima01"://借出單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "inv"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "31"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ima03"://客戶編號
                            WfShowPickUtility("p_sca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ima04"://業務人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ima05"://業務部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                            
                        case "ima10"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_invt401s
                if (pDr.Table.Prefix.ToLower() == "vw_invt402s")
                {
                    masterModel = DrMaster.ToItem<vw_invt402>();
                    detailModel = pDr.ToItem<vw_invt402s>();
                    switch (pColName.ToLower())
                    {
                        case "imb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "imb06"://借出單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "imb11"://借出單號
                            WfShowPickUtility("p_ilb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                {
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ilb01"], "");
                                    pDr["imb12"] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ilb02"], "");
                                }
                                else
                                {
                                    pDr[pColName] = "";
                                    pDr["imb12"] = "";
                                }
                            }
                            break;

                        case "imb18"://出庫倉
                            if (GlobalFn.isNullRet(detailModel.imb03, "") == "")
                            {
                                WfShowPickUtility("p_icb1", messageModel);
                                if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (messageModel.DataRowList.Count > 0)
                                        pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icb01"], "");
                                    else
                                        pDr[pColName] = "";
                                }
                            }
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.imb03));
                                WfShowPickUtility("p_icc1", messageModel);
                                if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (messageModel.DataRowList.Count > 0)
                                        pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icc02"], "");
                                    else
                                        pDr[pColName] = "";
                                }
                            }
                            break;

                        case "imb19"://入庫倉
                            if (GlobalFn.isNullRet(detailModel.imb03, "") == "")
                            {
                                WfShowPickUtility("p_icb1", messageModel);
                                if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (messageModel.DataRowList.Count > 0)
                                        pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icb01"], "");
                                    else
                                        pDr[pColName] = "";
                                }
                            }
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.imb03));
                                WfShowPickUtility("p_icc1", messageModel);
                                if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (messageModel.DataRowList.Count > 0)
                                        pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icc02"], "");
                                    else
                                        pDr[pColName] = "";
                                }
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
                pDr["ima02"] = Today;
                pDr["ima04"] = LoginInfo.UserNo;
                pDr["ima04_c"] = LoginInfo.UserName;
                pDr["ima05"] = LoginInfo.DeptNo;
                pDr["ima05_c"] = LoginInfo.DeptName;
                pDr["ima07"] = 0;
                pDr["ima08"] = "N";
                pDr["ima10"] = BaaTbModel.baa04;
                pDr["ima13"] = 0;
                pDr["ima13t"] = 0;
                pDr["ima13g"] = 0;
                pDr["ima14"] = 1;       //匯率
                pDr["imaconf"] = "N";
                pDr["imastat"] = "1";
                pDr["imacomp"] = LoginInfo.CompNo;
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
                        pDr["imb02"] = WfGetMaxSeq(pDr.Table, "imb02");
                        pDr["imb05"] = 0;
                        pDr["imb08"] = 0;
                        pDr["imb09"] = 0;
                        pDr["imb10"] = 0;
                        pDr["imb10t"] = 0;
                        pDr["imb11"] = DrMaster["ima11"];
                        pDr["imb13"] = 0;
                        pDr["imb14"] = 0;
                        pDr["imb16"] = 0;
                        pDr["imbcomp"] = LoginInfo.CompNo;
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
            vw_invt402 masterModel;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_invt402>();
                if (masterModel.imaconf != "N")
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
            vw_invt402 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_invt402>();
                if (masterModel.imaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可作廢!");
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
            vw_invt402 masterModel = null;
            vw_invt402s detailModel = null;
            List<vw_invt402s> detailList = null;
            bab_tb babModel = null;
            UltraGrid uGrid = null;
            ila_tb ilaModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt402>();
                if (e.Column.ToLower() != "ima01" && GlobalFn.isNullRet(DrMaster["ima01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false;
                }
                #region 單頭- vw_invt402
                if (e.Row.Table.Prefix.ToLower() == "vw_invt402")
                {
                    switch (e.Column.ToLower())
                    {
                        case "ima01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "inv", "31") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["ima01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetIma01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            if (ute_ima11.ReadOnly != true)
                                WfItemChkForceFocus(ute_ima11);
                            break;
                        case "ima02"://歸還日期
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;

                            var result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;
                        case "ima03"://客戶
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ima03_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkScaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此客戶資料,請檢核!");
                                return false;
                            }
                            WfSetIma03Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByIma06();
                            break;

                        case "ima11"://銷退單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfSetControlReadonly(new List<Control> { ute_ima03 }, false);
                                return true;
                            }
                            ilaModel = BoInv.OfGetIlaModel(GlobalFn.isNullRet(e.Value, ""));
                            if (ilaModel == null)
                            {
                                WfShowErrorMsg("無此出貨!");
                                return false;
                            }
                            if (ilaModel.ilaconf != "Y")
                            {
                                WfShowErrorMsg("出貨單非確認狀態!");
                                return false;
                            }
                            //檢查與單身是否一致!
                            detailList = TabDetailList[0].DtSource.ToList<vw_invt402s>();
                            iChkCnts = detailList.Where(p => p.imb11 != ilaModel.ila01 && GlobalFn.isNullRet(p.imb11, "") != "")
                                               .Count();
                            if (iChkCnts > 0)
                            {
                                WfShowErrorMsg("單頭與單身的銷貨單單號不一致!");
                                return false;
                            }
                            //這裡會有客戶跟銷退單單搶壓資料的問題:先以銷退單單為主
                            DrMaster["ima03"] = ilaModel.ila03;
                            DrMaster["ima03_c"] = BoStp.OfGetSca02(ilaModel.ila03);
                            WfSetControlReadonly(new List<Control> { ute_ima03 }, true);
                            DrMaster["ima06"] = ilaModel.ila06;
                            DrMaster["ima07"] = ilaModel.ila07;
                            DrMaster["ima08"] = ilaModel.ila08;
                            DrMaster["ima09"] = ilaModel.ila09;
                            DrMaster["ima10"] = ilaModel.ila10;
                            break;

                        case "ima04"://業務人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ima04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["ima04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ima05"://業務部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ima05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["ima05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ima06"://課稅別
                            WfSetIma06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByIma06();
                            break;

                        case "ima10"://幣別
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBekPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此幣別");
                                return false;
                            }
                            break;

                        case "ima14": //匯率
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

                #region 單身- vw_invt402s
                if (e.Row.Table.Prefix.ToLower() == "vw_invt402s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_invt402s>();
                    detailList = e.Row.Table.ToList<vw_invt402s>();
                    babModel = BoBas.OfGetBabModel(masterModel.ima01);

                    switch (e.Column.ToLower())
                    {
                        case "imb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            detailList = e.Row.Table.ToList<vw_invt402s>();
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.imb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "imb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["imb04"] = "";//品名
                                e.Row["imb05"] = 0;//借出數量
                                e.Row["imb06"] = "";//借出單位
                                e.Row["imb07"] = "";//庫存單位
                                e.Row["imb08"] = 0;//庫存轉換率
                                e.Row["imb14"] = 0;//借出數量
                                e.Row["imb16"] = 0;//庫存數量
                                WfSetDetailAmt(e.Row);
                                WfSetTotalAmt();
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }

                            if (WfSetImb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "imb05"://借出數量
                            if (GlobalFn.varIsNull(detailModel.imb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["imb03"]);
                                return false;
                            }
                            
                            if (GlobalFn.varIsNull(detailModel.imb06))
                            {
                                WfShowErrorMsg("請先輸入歸還單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["imb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入歸還數量!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["imb05"]);
                                return false;
                            }
                            detailModel.imb05 = BoBas.OfGetUnitRoundQty(detailModel.imb06, detailModel.imb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.imb05;
                            if (WfChkImb05(e.Row, detailModel) == false)
                                return false;
                            e.Row["imb16"] = BoBas.OfGetUnitRoundQty(detailModel.imb07, detailModel.imb05 * detailModel.imb08); //轉換庫存數量(並四拾伍入)
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "imb06"://借出單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.imb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["imb03"]);
                                return false;
                            }
                            if (WfChkImb06(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;

                            if (WfSetImb06Relation(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;
                            break;

                        case "imb09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.imb09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.imb09, BekTbModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "imb11"://借出單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("借出單單號不可為空白!");
                                return false;
                            }

                            if (WfChkImb11(e.Value.ToString()) == false)
                                return false;

                            if (!GlobalFn.varIsNull(detailModel.imb12))
                            {
                                if (WfChkImb12(GlobalFn.isNullRet(detailModel.imb11, ""), GlobalFn.isNullRet(detailModel.imb12, 0)) == false)
                                    return false;

                                if (WfSetImb12Relation(e.Row, GlobalFn.isNullRet(detailModel.imb11, ""), GlobalFn.isNullRet(detailModel.imb12, 0)) == false)
                                    return false;
                            }
                            WfSetTotalAmt();
                            break;

                        case "imb12"://借出項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("借出單項次不可為空白!");
                                return false;
                            }

                            if (WfChkImb12(GlobalFn.isNullRet(detailModel.imb11, ""), GlobalFn.isNullRet(detailModel.imb12, 0)) == false)
                                return false;
                            WfSetImb12Relation(e.Row, GlobalFn.isNullRet(detailModel.imb11, ""), GlobalFn.isNullRet(detailModel.imb12, 0));
                            WfSetTotalAmt();
                            break;

                        case "imb18"://出庫倉
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

                        case "imb19"://入庫倉
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (!GlobalFn.varIsNull(detailModel.imb18) && detailModel.imb18.ToUpper() == e.Value.ToString().ToUpper())
                            {
                                WfShowErrorMsg("出庫倉與入庫倉不可相同!");
                                return false;
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_invt402 masterModel = null;
            vw_invt402s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt402>();
                if (!GlobalFn.varIsNull(masterModel.ima01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.ima01, ""));
                #region 單頭資料檢查
                chkColName = "ima01";       //歸還單號
                chkControl = ute_ima01;
                if (GlobalFn.varIsNull(masterModel.ima01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ima02";       //單據日期
                chkControl = udt_ima02;
                if (GlobalFn.varIsNull(masterModel.ima02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.ima02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "ima03";       //客戶編號
                chkControl = ute_ima03;
                if (GlobalFn.varIsNull(masterModel.ima03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ima04";       //業務人員
                chkControl = ute_ima04;
                if (GlobalFn.varIsNull(masterModel.ima04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ima05";       //業務部門
                chkControl = ute_ima05;
                if (GlobalFn.varIsNull(masterModel.ima05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ima06";       //課稅別
                chkControl = ucb_ima06;
                if (GlobalFn.varIsNull(masterModel.ima06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ima10";       //幣別
                chkControl = ute_ima10;
                if (GlobalFn.varIsNull(masterModel.ima10))
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

                    detailModel = drTemp.ToItem<vw_invt402s>();
                    chkColName = "imb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.imb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "imb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.imb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "imb05";   //出庫數量
                    #region imb05 出庫數量
                    if (GlobalFn.varIsNull(detailModel.imb05) || detailModel.imb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    if (WfChkImb05(drTemp, detailModel) == false)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "imb06";   //出庫單位
                    if (GlobalFn.varIsNull(detailModel.imb06))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "imb18";   //出庫倉
                    if (GlobalFn.varIsNull(detailModel.imb18))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "imb19";   //入庫倉
                    if (GlobalFn.varIsNull(detailModel.imb19))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    if (detailModel.imb18.ToUpper() == detailModel.imb19.ToUpper())
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = "出庫與入庫倉不可相同!";
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
            string ima01New, errMsg;
            vw_invt402 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt402>();
                if (FormEditMode == YREditType.新增)
                {
                    ima01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.ima01, ModuleType.inv, (DateTime)masterModel.ima02, out ima01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["ima01"] = ima01New;
                }
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["imasecu"] = LoginInfo.UserNo;
                        DrMaster["imasecg"] = LoginInfo.GroupNo;
                        DrMaster["imacreu"] = LoginInfo.UserNo;
                        DrMaster["imacreg"] = LoginInfo.DeptNo;
                        DrMaster["imacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["imamodu"] = LoginInfo.UserNo;
                        DrMaster["imamodg"] = LoginInfo.DeptNo;
                        DrMaster["imamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["imbcreu"] = LoginInfo.UserNo;
                            drDetail["imbcreg"] = LoginInfo.DeptNo;
                            drDetail["imbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["imbmodu"] = LoginInfo.UserNo;
                            drDetail["imbmodg"] = LoginInfo.DeptNo;
                            drDetail["imbmodd"] = Now;
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

        #region WfAppendUpdate 存檔後處理額外的資料表 新增、修改、刪除...
        protected override bool WfAppendUpdate()
        {
            DataTable dtIcc;
            DataRow drIcc;
            CommonBLL boAppendIcc;
            StringBuilder sbSql;
            ima_tb imaModel = null;
            imb_tb imbModel = null;
            try
            {
                imaModel = DrMaster.ToItem<ima_tb>();

                boAppendIcc = new InvBLL(BoMaster.OfGetConntion()); //新增料號庫存明細資料
                boAppendIcc.TRAN = BoMaster.TRAN;
                boAppendIcc.OfCreateDao("icc_tb", "*", "");
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icc_tb");
                sbSql.AppendLine("WHERE 1<>1");
                dtIcc = boAppendIcc.OfGetDataTable(sbSql.ToString());

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    imbModel = dr.ToItem<imb_tb>();
                    if (BoInv.OfChkIccPKExists(imbModel.imb03, imbModel.imb18) == false)  //入庫倉不存在於icc 則新增一筆
                    {
                        if (dtIcc.Rows.Count > 0)
                        {
                            var drIccs = dtIcc.Select(string.Format("icc01='{0}' AND icc02='{1}'", imbModel.imb03, imbModel.imb12));
                            if (drIccs != null && drIccs.Length > 0)
                                continue;
                        }
                        drIcc = dtIcc.NewRow();
                        drIcc["icc01"] = imbModel.imb03;  //料號
                        drIcc["icc02"] = imbModel.imb18;
                        drIcc["icc03"] = "";
                        drIcc["icc04"] = imbModel.imb07;  //庫存單位
                        drIcc["icc05"] = 0;
                        dtIcc.Rows.Add(drIcc);
                    }
                }

                if (dtIcc.Rows.Count > 0)
                {
                    if (boAppendIcc.OfUpdate(dtIcc) < 1)
                    {
                        WfShowErrorMsg("新增料號庫存明細檔(icc_tb)失敗!");
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

                bt = new ButtonTool("Invr402");
                adoModel = BoAdm.OfGetAdoModel("invr402");
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
            vw_invt402 masterModel;
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

                    case "Invr402":
                        vw_invr402 l_vw_stpr402;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_invt402>();
                        l_vw_stpr402 = new vw_invr402();
                        l_vw_stpr402.ima01 = masterModel.ima01;
                        l_vw_stpr402.ima03 = "";
                        l_vw_stpr402.jump_yn = "N";
                        l_vw_stpr402.order_by = "1";

                        FrmInvr402 rptInvr402 = new FrmInvr402(this.LoginInfo, l_vw_stpr402, true, true);
                        rptInvr402.WindowState = FormWindowState.Minimized;
                        rptInvr402.ShowInTaskbar = false;
                        rptInvr402.Show();
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

        #region WfSetIma01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetIma01RelReadonly(string pIma01)
        {
            bab_tb babModel = null;
            string bab01;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_ima01 }, true);
                bab01 = pIma01.Substring(0, GlobalFn.isNullRet(BaaTbModel.baa06, 0));
                babModel = BoBas.OfGetBabModel(bab01);
                if (babModel.bab08 == "Y")   //有來源單據
                {
                    WfSetControlReadonly(new List<Control> { ute_ima11 }, false);
                }
                else
                {
                    WfSetControlReadonly(new List<Control> { ute_ima11 }, true);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetIma03Relation 設定客戶相關聯
        private void WfSetIma03Relation(string pIma03)
        {
            sca_tb scaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["ima01"], ""));
                scaModel = BoStp.OfGetScaModel(pIma03);
                if (scaModel == null)
                    return;

                DrMaster["ima03_c"] = scaModel.sca03;
                DrMaster["ima06"] = scaModel.sca22;    //課稅別
                WfSetIma06Relation(scaModel.sca22);
                DrMaster["ima09"] = scaModel.sca23;    //發票聯數
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetIma06Relation 設定稅別關聯
        private void WfSetIma06Relation(string pIma06)
        {
            try
            {
                if (pIma06 == "1")
                {
                    DrMaster["ima07"] = BaaTbModel.baa05;
                    DrMaster["ima08"] = "Y";
                }
                else if (pIma06 == "2")
                {
                    DrMaster["ima07"] = BaaTbModel.baa05;
                    DrMaster["ima08"] = "N";
                }
                else
                {
                    DrMaster["ima07"] = 0;
                    DrMaster["ima08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetImb03Relation 設定料號關聯
        private bool WfSetImb03Relation(string pImb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal lib08;   //轉換率
            try
            {
                icaModel = BoInv.OfGetIcaModel(pImb03);
                lib08 = 0;
                if (icaModel == null)
                {
                    pDr["imb04"] = "";//品名
                    pDr["imb05"] = 0;//歸還數量
                    pDr["imb06"] = "";//歸還單位
                    pDr["imb07"] = "";//庫存單位
                    pDr["imb08"] = 0;//庫存轉換率
                    pDr["imb13"] = 0;//對借出單轉換率
                    pDr["imb14"] = 0;//轉換借貨數量
                    pDr["imb15"] = "";//借貨數量
                    pDr["imb16"] = 0;//庫存轉換數量
                }
                else
                {
                    if (BoInv.OfGetUnitCovert(pImb03, icaModel.ica07, icaModel.ica07, out lib08) == false)     //都先以庫存單位處理
                    {
                        WfShowErrorMsg("未設定料號轉換,請先設定!");
                        return false;
                    }
                    pDr["imb04"] = icaModel.ica02;//品名
                    pDr["imb05"] = 0;//歸還數量
                    pDr["imb06"] = icaModel.ica07;//歸還帶庫存單位
                    pDr["imb07"] = icaModel.ica07;//庫存單位
                    pDr["imb08"] = lib08;    //對庫存轉換率
                    pDr["imb16"] = 0;//庫存轉換數量
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetImb06Relation 設定借出單位關聯
        //檢查前要先確認料號是否已輸入
        private bool WfSetImb06Relation(DataRow pDr, string pImb06, string pBab08)
        {
            vw_invt402s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_invt402s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pImb06, "")) == false)
                {
                    WfShowErrorMsg("無此借出單位!請確認");
                    return false;
                }
                //取得是否有銷售對庫存的轉換率
                dConvert = 0;
                if (BoInv.OfGetUnitCovert(detailModel.imb03, pImb06, detailModel.imb07, out dConvert) == true)
                {
                    pDr["imb08"] = dConvert;
                    pDr["imb16"] = BoBas.OfGetUnitRoundQty(detailModel.imb07, detailModel.imb05 * dConvert); //轉換庫存數量(並四拾伍入)
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetImb12Relation 設定借出單明細資料關聯
        private bool WfSetImb12Relation(DataRow pdr, string pImb11, int pImb12)
        {
            ilb_tb ilbModel;
            decimal imb16;
            try
            {
                ilbModel = BoInv.OfGetIlbModel(pImb11, pImb12);
                if (ilbModel == null)
                    return false;
                imb16 = BoBas.OfGetUnitRoundQty(ilbModel.ilb07, ilbModel.ilb05 * ilbModel.ilb08); //轉換庫存數量(並四拾伍入) 先取,避免錯誤

                pdr["imb03"] = ilbModel.ilb03;//料號
                pdr["imb04"] = ilbModel.ilb04;//品名
                pdr["imb05"] = ilbModel.ilb05;//歸還數量
                pdr["imb06"] = ilbModel.ilb06;//歸還單位
                pdr["imb07"] = ilbModel.ilb07;//庫存單位
                pdr["imb08"] = ilbModel.ilb08;//庫存轉換率
                pdr["imb09"] = ilbModel.ilb09;//單價
                pdr["imb10"] = ilbModel.ilb10;//未稅金額
                pdr["imb10t"] = ilbModel.ilb10t;//含稅金額

                pdr["imb13"] = 1;//對借出單轉換率 (目前不可更改歸還單位先設1)
                pdr["imb14"] = ilbModel.ilb05;//轉換借貨數量 (目前不可更改同歸還量)
                pdr["imb15"] = ilbModel.ilb06;//原借貨單單位
                pdr["imb16"] = imb16; //轉換庫存數量(並四拾伍入)
                pdr["imb18"] = ilbModel.ilb12;//出庫倉-對應到借出單的入庫倉
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkImb05 數量檢查
        private bool WfChkImb05(DataRow pdr, vw_invt402s pListDetail)
        {
            List<vw_invt402s> detailList = null;
            ilb_tb ilbModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換借貨數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的借貨單數量加總
            decimal otherQtyTotal = 0;      //其他單據的數量加總
            StringBuilder sbSql;
            List<SqlParameter> sqlParms;
            try
            {
                detailList = pdr.Table.ToList<vw_invt402s>();
                if (GlobalFn.varIsNull(pListDetail.imb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.imb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.imb06))
                {
                    errMsg = "請先輸入借出單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (pListDetail.imb05 <= 0)
                {
                    errMsg = "借出數量應大於0!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["ima01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (babModel.bab08 == "Y")
                {
                    ilbModel = BoInv.OfGetIlbModel(pListDetail.imb11, Convert.ToInt16(pListDetail.imb12));
                    if (babModel == null)
                    {
                        errMsg = "Get ilb_tb Error!";
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    //取得本身單據中的所有數量轉換借出單位後的總和!
                    //先取本身這筆
                    docThisQty = BoBas.OfGetUnitRoundQty(pListDetail.imb15, pListDetail.imb05 * pListDetail.imb13);
                    //再取其他筆加總
                    docOtherQtyTotal = detailList.Where(p => p.imb02 != pListDetail.imb02)
                                       .Where(p => p.imb11 == pListDetail.imb11 && p.imb12 == pListDetail.imb12)
                                       .Sum(p => p.imb14);
                    //取得其他單據上的加總
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT SUM(imb14) FROM ima_tb");
                    sbSql.AppendLine("  INNER JOIN imb_tb ON ima01=imb01");
                    sbSql.AppendLine("WHERE imaconf<>'X'");
                    sbSql.AppendLine("  AND ima01 <> @ima01");
                    sbSql.AppendLine("  AND imb11 = @imb11 AND ima12 = @imb12");
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@ima01", GlobalFn.isNullRet(DrMaster["ima01"], "")));
                    sqlParms.Add(new SqlParameter("@imb11", pListDetail.imb11));
                    sqlParms.Add(new SqlParameter("@imb12", pListDetail.imb12));
                    otherQtyTotal = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParms.ToArray()), 0);
                    if (ilbModel.ilb05 < (docThisQty + docOtherQtyTotal + otherQtyTotal))
                    {
                        errMsg = string.Format("項次{0}借出單最大可輸入數量為 {1}",
                                                pListDetail.imb02.ToString(),
                                                (ilbModel.ilb05 - docOtherQtyTotal - otherQtyTotal).ToString()
                                               );
                        WfShowErrorMsg(errMsg);
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

        #region WfChkImb06 檢查歸還單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkImb06(DataRow pDr, string pImb06, string pBab08)
        {
            vw_invt402s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_invt402s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pImb06, "")) == false)
                {
                    WfShowErrorMsg("無此歸還單位!請確認");
                    return false;
                }
                //檢查是否有借出對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.imb03, pImb06, detailModel.imb07, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定歸還單位對庫存單位的轉換率,請先設定!");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkImb11 借出單檢查
        private bool WfChkImb11(string pImb11)
        {
            string errMsg;
            ila_tb ilaModel;
            string ima11;
            try
            {
                ilaModel = BoInv.OfGetIlaModel(pImb11);
                if (ilaModel == null)
                {
                    errMsg = "查無此借出單!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (ilaModel.ilaconf != "Y")
                {
                    errMsg = "借單非確認狀態!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                ima11 = GlobalFn.isNullRet(DrMaster["ima11"], "");
                if (ima11 != "" && ima11 != pImb11)
                {
                    errMsg = "借出單單號與單頭不同!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkImb12 借出單項次檢查
        private bool WfChkImb12(string pIlb11, int pIlb12)
        {
            string errMsg;
            ilb_tb ilbModel;
            try
            {
                ilbModel = BoInv.OfGetIlbModel(pIlb11, pIlb12);
                if (ilbModel == null)
                {
                    errMsg = "查無此借出單項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if ((ilbModel.ilb05 - ilbModel.ilb15) <= 0)
                {
                    errMsg = string.Format("無可用借出單數量!(可用借出單數量為{0})", GlobalFn.isNullRet(ilbModel.ilb05 - ilbModel.ilb15, 0));
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetDetailAmt 處理小計
        private bool WfSetDetailAmt(DataRow drImb)
        {
            ima_tb imaModel;
            imb_tb imbModel;
            decimal imb10t = 0, imb10 = 0;
            try
            {
                imaModel = DrMaster.ToItem<ima_tb>();
                imbModel = drImb.ToItem<imb_tb>();

                if (imaModel.ima08 == "Y")//稅內含
                {
                    imb10t = imbModel.imb09 * imbModel.imb05;
                    imb10t = GlobalFn.Round(imb10t, BekTbModel.bek04);
                    imb10 = imb10t / (1 + (imaModel.ima07 / 100));
                    imb10 = GlobalFn.Round(imb10, BekTbModel.bek04);
                }
                else//稅外加
                {
                    imb10 = imbModel.imb09 * imbModel.imb05;
                    imb10 = GlobalFn.Round(imb10, BekTbModel.bek04);
                    imb10t = imb10 * (1 + (imaModel.ima07 / 100));
                    imb10t = GlobalFn.Round(imb10t, BekTbModel.bek04);
                }
                drImb["imb10"] = imb10;
                drImb["imb10t"] = imb10t;

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
            ima_tb imaModel;
            decimal ima13 = 0, ima13t = 0, ima13g;
            try
            {
                imaModel = DrMaster.ToItem<ima_tb>();
                if (imaModel.ima08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (imb_tb l_imb in TabDetailList[0].DtSource.ToList<imb_tb>())
                    {
                        ima13t += l_imb.imb10t;
                    }
                    ima13t = GlobalFn.Round(ima13t, BekTbModel.bek04);
                    ima13 = ima13t / (1 + imaModel.ima07 / 100);
                    ima13 = GlobalFn.Round(ima13, BekTbModel.bek04);
                    ima13g = ima13t - ima13;

                }
                else//稅外加
                {
                    foreach (imb_tb l_imb in TabDetailList[0].DtSource.ToList<imb_tb>())
                    {
                        ima13 += l_imb.imb10;
                    }
                    ima13 = GlobalFn.Round(ima13, BekTbModel.bek04);
                    ima13g = ima13 * (imaModel.ima07 / 100);
                    ima13g = GlobalFn.Round(ima13g, BekTbModel.bek04);
                    ima13t = ima13 + ima13g;
                }

                DrMaster["ima13"] = ima13;
                DrMaster["ima13t"] = ima13t;
                DrMaster["ima13g"] = ima13g;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfResetAmtByIma06 依稅別更新單身及單頭的金額
        private void WfResetAmtByIma06()
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

        #region WfUpdIlb15 更新借出單已歸還數量 確認/取消確認
        private bool WfUpdIlb15(bool pbConfirm)
        {
            bab_tb babModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            DataTable dtImbDistinct = null;
            string ilb01;
            decimal ilb02;
            decimal docQty = 0, otherDocQty = 0;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["ima01"], ""));
                if (babModel == null)
                {
                    WfShowErrorMsg("Get bab_tb Error!");
                    return false;
                }
                if (babModel.bab08 == "Y")   //更新銷貨單出庫數量
                {
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT DISTINCT imb11,imb12,SUM(imb14) imb14");
                    sbSql.AppendLine("FROM imb_tb");
                    sbSql.AppendLine("WHERE imb01=@imb01");
                    sbSql.AppendLine("  AND ISNULL(imb11,'')<>''");
                    sbSql.AppendLine("GROUP BY imb11,imb12");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@imb01", GlobalFn.isNullRet(DrMaster["ima01"], "")));
                    dtImbDistinct = BoStp.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                    if (dtImbDistinct == null)
                        return true;
                    foreach (DataRow dr in dtImbDistinct.Rows)
                    {
                        ilb01 = dr["imb11"].ToString();
                        ilb02 = Convert.ToDecimal(dr["imb12"]);
                        docQty = Convert.ToDecimal(dr["imb14"]);

                        sbSql = new StringBuilder();
                        sbSql.AppendLine("SELECT SUM(imb14) FROM ima_tb");
                        sbSql.AppendLine("  INNER JOIN imb_tb ON ima01=imb01");
                        sbSql.AppendLine("WHERE imaconf='Y'");
                        sbSql.AppendLine("AND imb11=@imb11 AND imb12=@imb12");
                        sbSql.AppendLine("AND ima01<>@ima01");

                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@imb11", ilb01));
                        sqlParmList.Add(new SqlParameter("@imb12", ilb02));
                        sqlParmList.Add(new SqlParameter("@ima01", GlobalFn.isNullRet(DrMaster["ima01"], "")));
                        otherDocQty = 0;
                        otherDocQty = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);

                        sbSql = new StringBuilder();
                        sbSql = sbSql.AppendLine("UPDATE ilb_tb");
                        sbSql = sbSql.AppendLine("SET ilb15=@ilb15");
                        sbSql = sbSql.AppendLine("WHERE ilb01=@ilb01");
                        sbSql = sbSql.AppendLine("AND ilb02=@ilb02");
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@ilb01", ilb01));
                        sqlParmList.Add(new SqlParameter("@ilb02", ilb02));
                        if (pbConfirm)  //確認的要加本身單據
                            sqlParmList.Add(new SqlParameter("@ilb15", docQty + otherDocQty));
                        else
                            sqlParmList.Add(new SqlParameter("@ilb15", otherDocQty));

                        if (BoStp.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray()) != 1)
                        {
                            WfShowErrorMsg("更新借出單歸還數量失敗!");
                            return false;
                        }
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

        #region WfConfirm 確認
        private void WfConfirm()
        {
            ima_tb imaModel = null;
            imb_tb imbModel = null;

            string errMsg;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();

                imaModel = DrMaster.ToItem<ima_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                imaModel.imaconf = "Y";
                imaModel.imacond = Today;
                imaModel.imaconu = LoginInfo.UserNo;

                //更新借出單歸還量
                if (WfUpdIlb15(true) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                if (WfUpdIlastatByConfirm()==false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    imbModel = dr.ToItem<imb_tb>();
                    //更新料件庫存量 icc_tb --出庫
                    if (BoInv.OfUpdIcc05("2", imbModel.imb03, imbModel.imb18, imbModel.imb16, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }

                    //更新料件庫存量 icc_tb --入庫
                    if (BoInv.OfUpdIcc05("1", imbModel.imb03, imbModel.imb19, imbModel.imb16, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔
                    if (BoInv.OfStockPost("invt402", imaModel, imbModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }

                DrMaster["imaconf"] = "Y";
                DrMaster["imacond"] = Today;
                DrMaster["imaconu"] = LoginInfo.UserNo;
                DrMaster["imamodu"] = LoginInfo.UserNo;
                DrMaster["imamodg"] = LoginInfo.DeptNo;
                DrMaster["imamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                imaModel = DrMaster.ToItem<ima_tb>();
                WfSetDocPicture("", imaModel.imaconf, "", pbxDoc);
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
            vw_invt402 masterModel = null;
            List<vw_invt402s> detailList = null;
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt402>();
                if (masterModel.imaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.ima02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_invt402s>();
                //檢查撥出倉別的數量是否足夠做出庫
                detailList = TabDetailList[0].DtSource.ToList<vw_invt402s>();
                var listSumImb =                         //依料號及倉庫做加總
                        from imb in detailList
                        where imb.imb16 > 0 //庫存轉換數量
                        group imb by new { imb.imb03, imb.imb18 } into imb_sum
                        select new
                        {
                            imb03 = imb_sum.Key.imb03,
                            imb18 = imb_sum.Key.imb18,
                            imb16 = imb_sum.Sum(p => p.imb16)
                        }
                    ;
                foreach (var sumImb in listSumImb)
                {
                    icc05 = BoInv.OfGetIcc05(sumImb.imb03, sumImb.imb18);
                    if (icc05 < sumImb.imb16)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", sumImb.imb03, icc05));
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

        #region WfCancelConfirm 取消確認
        private void WfCancelConfirm()
        {
            ima_tb imaModel = null;
            imb_tb imbModel = null;
            string errMsg;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)        //這裡會LOCK資料
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;

                WfSetBllTransaction();
                imaModel = DrMaster.ToItem<ima_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                if (WfUpdIlb15(false) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                if (WfUpdIlastatByCancleConfirm() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    imbModel = dr.ToItem<imb_tb>();
                    //更新料件庫存量 icc_tb  --出庫倉
                    if (BoInv.OfUpdIcc05("1", imbModel.imb03, imbModel.imb18, imbModel.imb16, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        DrMaster.RejectChanges();
                        return;
                    }

                    //更新料件庫存量 icc_tb --入庫倉
                    if (BoInv.OfUpdIcc05("2", imbModel.imb03, imbModel.imb19, imbModel.imb16, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        DrMaster.RejectChanges();
                        return;
                    }

                    //刪除庫存交易歷史檔
                    if (BoInv.OfDelIna(imbModel.imb01, imbModel.imb02, "1", this.StrFormID, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        DrMaster.RejectChanges();
                        return;
                    }

                    //刪除庫存交易歷史檔
                    if (BoInv.OfDelIna(imbModel.imb01, imbModel.imb02, "2", this.StrFormID, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        DrMaster.RejectChanges();
                        return;
                    }
                }

                DrMaster["imaconf"] = "N";
                DrMaster["imacond"] = DBNull.Value;
                DrMaster["imaconu"] = "";
                DrMaster["imamodu"] = LoginInfo.UserNo;
                DrMaster["imamodg"] = LoginInfo.DeptNo;
                DrMaster["imamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                imaModel = DrMaster.ToItem<ima_tb>();
                WfSetDocPicture("", imaModel.imaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfCancelConfirmChk 取消確認前檢查
        private bool WfCancelConfirmChk()
        {
            vw_invt402 masterModel = null;
            List<vw_invt402s> detailList = null;
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt402>();
                if (masterModel.imaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.ima02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                //檢查入庫倉的數量是否足夠做出庫
                detailList = TabDetailList[0].DtSource.ToList<vw_invt402s>();
                var listSumIlb =                         //依料號及倉庫做加總
                        from imb in detailList
                        where imb.imb16 > 0
                        group imb by new { imb.imb03, imb.imb19 } into ilb_sum
                        select new
                        {
                            imb03 = ilb_sum.Key.imb03,
                            imb19 = ilb_sum.Key.imb19,
                            imb16 = ilb_sum.Sum(p => p.imb16)
                        }
                    ;
                foreach (var sumImb in listSumIlb)
                {
                    icc05 = BoInv.OfGetIcc05(sumImb.imb03, sumImb.imb19);
                    if (icc05 < sumImb.imb16)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", sumImb.imb16, icc05));
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

        #region WfInvalid 作廢/作廢還原
        private void WfInvalid()
        {
            vw_invt402 masterModel = null;
            List<SqlParameter> sqlParmList;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_invt402>();

                if (masterModel.imaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認,不可作廢!");
                    WfRollback();
                    return;
                }

                if (masterModel.imaconf == "N")//走作廢
                {

                    DrMaster["imaconf"] = "X";
                    DrMaster["imaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.imaconf == "X")
                {
                    DrMaster["imaconf"] = "N";
                    DrMaster["imaconu"] = "";
                }
                DrMaster["imamodu"] = LoginInfo.UserNo;
                DrMaster["imamodg"] = LoginInfo.DeptNo;
                DrMaster["imamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_invt402>();
                WfSetDocPicture("", masterModel.imaconf, masterModel.imaconf, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfUpdIlastatByConfirm() 更新借出單狀態 確認
        private bool WfUpdIlastatByConfirm()
        {
            List<vw_invt402s> detailList;
            int chkCnts = 0;
            string querySql = "", updateSql = "";
            List<SqlParameter> sqlParms;
            try
            {
                detailList = TabDetailList[0].DtSource.ToList<vw_invt402s>();
                //取得請購單單號
                var imbDistinct = from o in detailList
                                  where !GlobalFn.varIsNull(o.imb11)
                                  group o by new { o.imb11 } into imbTemp
                                  select new
                                  {
                                      imb11 = imbTemp.Key.imb11
                                  }
                               ;

                if (imbDistinct == null)
                    return true;
                //查詢
                querySql = @"SELECT COUNT(1)
                        FROM ila_tb
	                        INNER JOIN ilb_tb ON ila01=ilb01
                        WHERE ila01=@ila01
	                        AND ilastat='1'
	                        AND (ilb05-ilb15)>0
                        ";
                //逐筆更新
                updateSql = @"UPDATE dbo.ila_tb
                            SET ilastat='9'
                            WHERE ila01=@ila01
                        ";

                foreach (var imbModel in imbDistinct)
                {
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@ila01", imbModel.imb11));
                    chkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(querySql, sqlParms.ToArray()), 0);
                    if (chkCnts > 0)
                        continue;
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@ila01", imbModel.imb11));
                    if (BoStp.OfExecuteNonquery(updateSql, sqlParms.ToArray()) < 1)
                    {
                        WfShowErrorMsg("更新ilastat失敗!");
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

        #region WfUpdIlastatByCancleConfirm() 更新借出歸還單狀態 取消確認
        private bool WfUpdIlastatByCancleConfirm()
        {
            List<vw_invt402s> detailList;
            int chkCnts = 0;
            string querySql = "", updateSql = "";
            List<SqlParameter> sqlParms;
            try
            {
                detailList = TabDetailList[0].DtSource.ToList<vw_invt402s>();
                //取得採購單單號
                var imbDistinct = from o in detailList
                                  where !GlobalFn.varIsNull(o.imb11)
                                  group o by new { o.imb11 } into imbTemp
                                  select new
                                  {
                                      imb11 = imbTemp.Key.imb11
                                  }
                               ;

                if (imbDistinct == null)
                    return true;
                //查詢
                querySql = @"SELECT COUNT(1)
                        FROM ila_tb
                        WHERE ila01=@ila01
	                        AND ilastat='9'
                        ";
                //逐筆更新
                updateSql = @"UPDATE dbo.ila_tb
                            SET ilastat='1'
                            WHERE ila01=@ila01
                        ";

                foreach (var imbModel in imbDistinct)
                {
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@ila01", imbModel.imb11));
                    chkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(querySql, sqlParms.ToArray()), 0);
                    if (chkCnts == 0)
                        continue;
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@ila01", imbModel.imb11));
                    if (BoStp.OfExecuteNonquery(updateSql, sqlParms.ToArray()) < 1)
                    {
                        WfShowErrorMsg("更新ilastat失敗!");
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
