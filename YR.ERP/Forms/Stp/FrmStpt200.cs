/* 程式名稱: 報價單維護作業
   系統代號: stpt200
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
    public partial class FrmStpt200 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region property
        StpBLL BoStp = null;
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;

        baa_tb BaaTbModel = null;
        bek_tb BekTbModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmStpt200()
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
            this.StrFormID = "stpt200";
            uTab_Header.Tabs[0].Text = "基本資料";
            uTab_Header.Tabs[1].Text = "狀態";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "一般資料";
            uTab_Master.Tabs[1].Text = "其他資料";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("sea01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "seasecu";
                TabMaster.GroupColumn = "seasecg";

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
                WfSetUcomboxDataSource(ucb_sea06, sourceList);

                //發票聯數
                sourceList = BoStp.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_sea09, sourceList);

                //單據確認
                sourceList = BoStp.OfGetSeaconfKVPList();
                WfSetUcomboxDataSource(ucb_seaconf, sourceList);
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
            this.TabDetailList[0].TargetTable = "seb_tb";
            this.TabDetailList[0].ViewTable = "vw_stpt200s";
            keyParm = new SqlParameter("seb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "sea01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_stpt200 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_stpt200>();
                    WfSetDocPicture("", masterModel.seaconf, masterModel.seastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.sea10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.sea10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.sea10));
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
                    //listMaster = DRMASTER.ToItem<vw_purt200>();
                    WfSetControlsReadOnlyRecursion(this, false); //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯

                    if (GlobalFn.isNullRet(masterModel.sea01, "") != "")
                        WfSetControlReadonly(ute_sea01, true);

                    WfSetControlReadonly(new List<Control> { ute_seacreu,ute_seacreg,udt_seacred}, true);
                    WfSetControlReadonly(new List<Control> { ute_seamodu, ute_seamodg, udt_seamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_seasecu, ute_seasecg }, true);

                    WfSetControlReadonly(new List<Control> { ute_sea01_c, ute_sea03_c, ute_sea04_c, ute_sea05_c, ute_sea11_c, ute_sea12_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_sea07, ucx_sea08 }, true);
                    WfSetControlReadonly(new List<Control> { ute_sea13, ute_sea13t, ute_sea13g }, true);
                    WfSetControlReadonly(new List<Control> { ute_sea14_c, ute_sea15_c, ute_sea16_c }, true);
                    WfSetControlReadonly(new List<Control> { ucb_seaconf, udt_seacond, ute_seaconu, ute_seastat }, true);
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_sea01, true);
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
            try
            {
                switch (pCurTabDetail)
                {
                    case 0:
                        foreach (UltraGridCell ugc in pUgr.Cells)
                        {
                            columnName = ugc.Column.Key.ToLower();
                            //先控可以輸入的
                            if (columnName == "seb03" ||
                                columnName == "seb05" ||
                                columnName == "seb06" ||
                                columnName == "seb09"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "seb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }
                            WfSetControlReadonly(ugc, true);
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
                #region 單頭-pick vw_stpt200
                if (pDr.Table.Prefix.ToLower() == "vw_stpt200")
                {
                    switch (pColName.ToLower())
                    {
                        case "sea01"://報價單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "stp"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "10"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                            //}
                            break;
                        case "sea03"://客戶編號
                            WfShowPickUtility("p_sca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sca01"], "");
                            //}

                            break;
                        case "sea04"://業務人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                            //}

                            break;
                        case "sea05"://業務部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                            //}
                            break;

                        case "sea10"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sea11"://收款條件
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@bef01", "1"));
                            WfShowPickUtility("p_bef1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bef02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bef02"], "");
                            //}
                            break;

                        case "sea12"://取價條件
                            WfShowPickUtility("p_sbb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbb01"], "");
                            //}
                            break;

                        case "sea14"://貨運方式
                            WfShowPickUtility("p_bel1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bel01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["bel01"];
                            //}
                            break;

                        case "sea15"://貨運起點
                            WfShowPickUtility("p_sbg1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbg01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["sbg01"];
                            //}
                            break;

                        case "sea16"://貨運終點
                            WfShowPickUtility("p_sbg1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbg01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["sbg01"];
                            //}
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_stpt200s
                if (pDr.Table.Prefix.ToLower() == "vw_stpt200s")
                {
                    switch (pColName.ToLower())
                    {
                        case "seb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                            //}
                            break;

                        case "seb06"://報價單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                            //}
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
                pDr["sea02"] = Today;
                pDr["sea04"] = LoginInfo.UserNo;
                pDr["sea04_c"] = LoginInfo.UserName;
                pDr["sea05"] = LoginInfo.DeptNo;
                pDr["sea05_c"] = LoginInfo.DeptName;
                pDr["sea07"] = 0;
                pDr["sea08"] = "N";
                pDr["sea10"] = BaaTbModel.baa04;
                pDr["sea13"] = 0;
                pDr["sea13t"] = 0;
                pDr["sea13g"] = 0;
                pDr["sea18"] = 1;
                pDr["seaconf"] = "N";
                pDr["seastat"] = "1";
                pDr["seacomp"] = LoginInfo.CompNo;
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
                        pDr["seb02"] = WfGetMaxSeq(pDr.Table, "seb02");
                        pDr["seb05"] = 0;
                        pDr["seb08"] = 0;
                        pDr["seb09"] = 0;
                        pDr["seb10"] = 0;
                        pDr["seb10t"] = 0;
                        pDr["sebcomp"] = LoginInfo.CompNo;
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
            vw_stpt200 masterModel;
            string errMsg = "";
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_stpt200>();
                if (masterModel.seaconf != "N")
                {
                    if (masterModel.seaconf == "Y")
                        errMsg = "單據已確認,不可更改";
                    else if (masterModel.seaconf == "X")
                        errMsg = "單據已作廢,不可更改";
                    WfShowBottomStatusMsg(errMsg);
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

        #region WfPreDeleteCheck 進主檔刪除前檢查
        protected override bool WfPreDeleteCheck(DataRow pDr)
        {
            vw_stpt200 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_stpt200>();
                if (masterModel.seaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可作廢!");
                    return false;
                }

                //還需檢查訂單
                if (WfChkSfaExists(masterModel.sea01) == true)
                {
                    WfShowErrorMsg("已有訂單資料!不可取消確認!");
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
            vw_stpt200 masterModel = null;
            vw_stpt200s detailModel,newDetailModel = null;
            List<vw_stpt200s> detailList = null;
            UltraGrid uGrid;
            decimal price=0;
            Result result = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt200>();
                #region 單頭- vw_stpt200
                if (e.Row.Table.Prefix.ToLower() == "vw_stpt200")
                {
                    switch (e.Column.ToLower())
                    {
                        case "sea01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sea01_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "stp", "10") == false)
                            {
                                WfShowErrorMsg("無此報價單別,請檢核!");
                                return false;
                            }
                            e.Row["sea01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetControlReadonly(ute_sea01, true);
                            break;
                        case "sea03"://客戶
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sea03_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkScaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此客戶資料,請檢核!");
                                return false;
                            }
                            WfSetSea03Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtBySea06();
                            break;

                        case "sea04"://業務人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sea04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["sea04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sea05"://業務部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sea05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["sea05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sea06"://課稅別
                            WfSetSea06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtBySea06();
                            break;

                        case "sea10"://幣別
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

                        case "sea11"://付款條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sea11_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("2", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此付款條件,請檢核!");
                                return false;
                            }
                            e.Row["sea11_c"] = BoBas.OfGetBef03("2", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sea12"://取價條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sea12_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbbPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此報價取價條件,請檢核!");
                                return false;
                            }
                            e.Row["sea12_c"] = BoStp.OfGetSbb02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sea14"://貨運方式
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sea14_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBelPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運方式,請檢核!");
                                return false;
                            }
                            e.Row["sea14_c"] = BoBas.OfGetBel02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sea15"://貨運起點
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sea15_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbgPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運起點,請檢核!");
                                return false;
                            }
                            e.Row["sea15_c"] = BoStp.OfGetSbg02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sea16"://貨運終點
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sea16_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbgPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運終點,請檢核!");
                                return false;
                            }
                            e.Row["sea16_c"] = BoStp.OfGetSbg02(GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "sea18": //匯率
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

                #region 單身- vw_stpt200s
                if (e.Row.Table.Prefix.ToLower() == "vw_stpt200s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_stpt200s>();
                    switch (e.Column.ToLower())
                    {
                        case "seb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            detailList = e.Row.Table.ToList<vw_stpt200s>();
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.seb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "seb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["seb04"] = "";
                                e.Row["seb05"] = 0;
                                e.Row["seb06"] = "";
                                e.Row["seb07"] = "";
                                e.Row["seb08"] = 0;
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }
                            if (WfSetSeb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;
                            if (!GlobalFn.varIsNull(masterModel.sea12))
                            {
                                newDetailModel=e.Row.ToItem<vw_stpt200s>();
                                result=BoStp.OfGetPrice(masterModel.sea12, masterModel.sea03, e.Value.ToString(), newDetailModel.seb06,
                                    masterModel.sea02, masterModel.sea10, "1", newDetailModel.seb05, masterModel.
                                    sea08, masterModel.sea07, masterModel.sea18, out price);
                                 if (result.Success == true)
                                {
                                    e.Row["seb09"] = price;
                                }
                                else
                                {
                                    WfShowErrorMsg(result.Message);
                                    return false;
                                }
                            }
                            break;

                        case "seb05"://報價數量
                            if (GlobalFn.varIsNull(detailModel.seb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["seb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detailModel.seb06))
                            {
                                WfShowErrorMsg("請先輸入報價單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["seb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入報價數量!");
                                return false;
                            }
                            detailModel.seb05 = BoBas.OfGetUnitRoundQty(detailModel.seb06, detailModel.seb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.seb05;
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "seb09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.seb09<0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.seb09, BekTbModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "seb06"://報價單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.seb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["seb03"]);
                                return false;
                            }
                            if (WfChkSfb06(e.Row, e.Value.ToString()) == false)
                                return false;

                            if (WfSetSfb06Relation(e.Row, e.Value.ToString()) == false)
                                return false;
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
                    case "vw_stpt200s":
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
            vw_stpt200 masterModel = null;
            vw_stpt200s detailModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt200>();
                #region 單頭資料檢查
                chkColName = "sea01";       //報價單號
                chkControl = ute_sea01;
                if (GlobalFn.varIsNull(masterModel.sea01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sea02";       //報價日期
                chkControl = udt_sea02;
                if (GlobalFn.varIsNull(masterModel.sea02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sea03";       //客戶別
                chkControl = ute_sea03;
                if (GlobalFn.varIsNull(masterModel.sea03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sea04";       //業務人員
                chkControl = ute_sea04;
                if (GlobalFn.varIsNull(masterModel.sea04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sea05";       //業務部門
                chkControl = ute_sea05;
                if (GlobalFn.varIsNull(masterModel.sea05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sea06";       //課稅別
                chkControl = ucb_sea06;
                if (GlobalFn.varIsNull(masterModel.sea06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sea10";       //匯率
                chkControl = ute_sea10;
                if (GlobalFn.varIsNull(masterModel.sea10))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sea12";       //價格條件
                chkControl = ute_sea12;
                if (GlobalFn.varIsNull(masterModel.sea12))
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
                    if (drTemp.RowState == DataRowState.Unchanged)  //未異動的資料就不檢查了
                        continue;

                    detailModel = drTemp.ToItem<vw_stpt200s>();
                    #region seb02-項次
                    chkColName = "seb02";
                    if (GlobalFn.varIsNull(detailModel.seb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region seb03-料號
                    chkColName = "seb03";
                    if (GlobalFn.varIsNull(detailModel.seb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region seb05-報價數量
                    chkColName = "seb05";
                    if (GlobalFn.varIsNull(detailModel.seb05) || detailModel.seb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "應大於0";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region seb05-報價單位
                    chkColName = "seb05";
                    if (GlobalFn.varIsNull(detailModel.seb05))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion
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
            string sea01New, errMsg;
            vw_stpt200 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt200>();
                if (FormEditMode == YREditType.新增)
                {
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.sea01, ModuleType.stp, (DateTime)masterModel.sea02, out sea01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["sea01"] = sea01New;
                }
                
                //填入系統資訊
                if (DrMaster.RowState !=DataRowState.Unchanged)
                {
                    if (DrMaster.RowState==DataRowState.Added)
                    {
                        DrMaster["seasecu"] = LoginInfo.UserNo;
                        DrMaster["seasecg"] = LoginInfo.GroupNo;
                        DrMaster["seacreu"] = LoginInfo.UserNo;
                        DrMaster["seacreg"] = LoginInfo.DeptNo;
                        DrMaster["seacred"] = Now;
                    }
                    else if(DrMaster.RowState==DataRowState.Modified)
                    {
                        DrMaster["seamodu"] = LoginInfo.UserNo;
                        DrMaster["seamodg"] = LoginInfo.DeptNo;
                        DrMaster["seamodd"] = Now;
                    }
                }

                foreach(DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["sebcreu"] = LoginInfo.UserNo;
                            drDetail["sebcreg"] = LoginInfo.DeptNo;
                            drDetail["sebcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["sebmodu"] = LoginInfo.UserNo;
                            drDetail["sebmodg"] = LoginInfo.DeptNo;
                            drDetail["sebmodd"] = Now;
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
                
                bt = new ButtonTool("Stpr200");
                adoModel = BoAdm.OfGetAdoModel("stpr200");
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

                    case "Stpr200":
                        vw_stpr200 stpr200Model;
                        vw_stpt200 stpt200Model;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        stpt200Model = DrMaster.ToItem<vw_stpt200>();
                        stpr200Model = new vw_stpr200();
                        stpr200Model.sea01 = stpt200Model.sea01;
                        stpr200Model.sea03 = "";
                        stpr200Model.jump_yn = "N";
                        stpr200Model.order_by = "1";

                        FrmStpr200 rpt = new FrmStpr200(this.LoginInfo, stpr200Model, true, true);
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
        #region WfSetSea03Relation 設定客戶相關聯
        private void WfSetSea03Relation(string pSea03)
        {
            sca_tb scaModel;
            try
            {
                scaModel = BoStp.OfGetScaModel(pSea03);
                if (scaModel == null)
                    return;

                DrMaster["sea03_c"] = scaModel.sca03;
                DrMaster["sea06"] = scaModel.sca22;    //課稅別
                WfSetSea06Relation(scaModel.sca22);
                DrMaster["sea09"] = scaModel.sca23;    //發票聯數

                DrMaster["sea11"] = scaModel.sca21;    //收款條件
                DrMaster["sea11_c"] = BoBas.OfGetBef03("2", scaModel.sca21);
                DrMaster["sea12"] = scaModel.sca24;    //取價條件
                DrMaster["sea12_c"] = BoStp.OfGetSbb02(scaModel.sca24);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSea06Relation 設定稅別關聯
        private void WfSetSea06Relation(string pSea06)
        {
            try
            {
                if (pSea06 == "1")
                {
                    DrMaster["sea07"] = BaaTbModel.baa05;
                    DrMaster["sea08"] = "Y";
                }
                else if (pSea06 == "2")
                {
                    DrMaster["sea07"] = BaaTbModel.baa05;
                    DrMaster["sea08"] = "N";
                }
                else
                {
                    DrMaster["sea07"] = 0;
                    DrMaster["sea08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSeb03Relation 設定料號關聯
        private bool WfSetSeb03Relation(string pSeb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal seb08;
            try
            {
                icaModel = BoInv.OfGetIcaModel(pSeb03);

                if (BoInv.OfGetUnitCovert(pSeb03, icaModel.ica09, icaModel.ica07, out seb08) == false)
                {
                    WfShowErrorMsg("未設定料號轉換,請先設定!");
                    return false;
                }

                pDr["seb04"] = icaModel.ica02;
                pDr["seb06"] = icaModel.ica09;   //報價單位帶銷售單位
                pDr["seb07"] = icaModel.ica07;   //庫存單位
                pDr["seb08"] = seb08;

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
        private bool WfSetSfb06Relation(DataRow pDr, string pSeb06)
        {
            vw_stpt200s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_stpt200s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pSeb06, "")) == false)
                {
                    WfShowErrorMsg("無此訂單單位!請確認");
                    return false;
                }
                //取得是否有報價對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.seb03, pSeb06, detailModel.seb07, out dConvert) == true)
                {
                    pDr["seb08"] = dConvert;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkPfb06 檢查報價單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkSfb06(DataRow pDr, string pSeb06)
        {
            vw_stpt200s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_stpt200s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pSeb06, "")) == false)
                {
                    WfShowErrorMsg("無此報價單位!請確認");
                    return false;
                }
                //檢查是否有報價對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.seb03, pSeb06, detailModel.seb07, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定報價單位對庫存單位的轉換率,請先設定!");
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

        #region WfConfirm 確認
        private void WfConfirm()
        {
            vw_stpt200 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_stpt200>();
                if (masterModel.seaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    WfRollback();
                    return;
                }

                DrMaster["seaconf"] = "Y";
                DrMaster["seacond"] = Today;
                DrMaster["seaconu"] = LoginInfo.UserNo;
                DrMaster["seamodu"] = LoginInfo.UserNo;
                DrMaster["seamodg"] = LoginInfo.DeptNo;
                DrMaster["seamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_stpt200>();
                WfSetDocPicture("", masterModel.seaconf, masterModel.seastat, pbxDoc);
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
            vw_stpt200 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_stpt200>();
                if (masterModel.seaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    WfRollback();
                    return;
                }
                //還需檢查訂單
                if (WfChkSfaExists(masterModel.sea01) == true)
                {
                    WfShowErrorMsg("已有訂單資料!不可取消確認!");
                    WfRollback();
                    return;
                }

                DrMaster["seaconf"] = "N";
                DrMaster["seacond"] = DBNull.Value;
                DrMaster["seaconu"] = "";
                DrMaster["seamodu"] = LoginInfo.UserNo;
                DrMaster["seamodg"] = LoginInfo.DeptNo;
                DrMaster["seamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_stpt200>();
                WfSetDocPicture("", masterModel.seaconf, masterModel.seastat, pbxDoc);
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
            vw_stpt200 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_stpt200>();

                if (masterModel.seaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.seaconf == "N")//走作廢
                {
                    ////還需檢查訂單
                    //if (WfChkSfaExists(listMaster.pea01) == true)
                    //{
                    //    WfShowMsg("已有採購資料!請先作廢訂單!");
                    //    WfRollback();
                    //    return;
                    //}

                    DrMaster["seaconf"] = "X";
                    DrMaster["seaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.seaconf == "X")
                {
                    DrMaster["seaconf"] = "N";
                    DrMaster["seaconu"] = "";
                }
                DrMaster["seamodu"] = LoginInfo.UserNo;
                DrMaster["seamodg"] = LoginInfo.DeptNo;
                DrMaster["seamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_stpt200>();
                WfSetDocPicture("", masterModel.seaconf, masterModel.seastat, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfChkSfaExists 檢查訂單是否存在
        private bool WfChkSfaExists(string pSea01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChkCnts = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sfa_tb");
                sbSql.AppendLine("  INNER JOIN sfb_tb ON sfa01=sfb01");
                sbSql.AppendLine("WHERE");
                sbSql.AppendLine(" sfaconf <>'X' ");
                sbSql.AppendLine(" AND sfb11=@sfa11 ");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sfa11", pSea01));
                iChkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
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

        #region WfSetDetailAmt 處理小計
        private bool WfSetDetailAmt(DataRow drSeb)
        {
            sea_tb seaModel;
            seb_tb sebModel;
            decimal seb10t = 0, seb10 = 0;
            try
            {
                seaModel = DrMaster.ToItem<sea_tb>();
                sebModel = drSeb.ToItem<seb_tb>();

                if (seaModel.sea08 == "Y")//稅內含
                {
                    seb10t = sebModel.seb09 * sebModel.seb05;
                    seb10t = GlobalFn.Round(seb10t, BekTbModel.bek04);
                    seb10 = seb10t / (1 + (seaModel.sea07 / 100));
                    seb10 = GlobalFn.Round(seb10, BekTbModel.bek04);
                }
                else//稅外加
                {
                    seb10 = sebModel.seb09 * sebModel.seb05;
                    seb10 = GlobalFn.Round(seb10, BekTbModel.bek04);
                    seb10t = seb10 * (1 + (seaModel.sea07 / 100));
                    seb10t = GlobalFn.Round(seb10t, BekTbModel.bek04);
                }
                drSeb["seb10"] = seb10;
                drSeb["seb10t"] = seb10t;

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
            sea_tb seaModel;
            decimal sea13 = 0, sea13t = 0, sea13g;
            try
            {
                seaModel = DrMaster.ToItem<sea_tb>();
                if (seaModel.sea08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (seb_tb l_seb in TabDetailList[0].DtSource.ToList<seb_tb>())
                    {
                        sea13t += l_seb.seb10t;
                    }
                    sea13t=GlobalFn.Round(sea13t, BekTbModel.bek04);
                    sea13 = sea13t / (1 + seaModel.sea07 / 100);
                    sea13 = GlobalFn.Round(sea13, BekTbModel.bek04);
                    sea13g = sea13t - sea13;

                }
                else//稅外加
                {
                    foreach (seb_tb l_seb in TabDetailList[0].DtSource.ToList<seb_tb>())
                    {
                        sea13 += l_seb.seb10;
                    }
                    sea13 = GlobalFn.Round(sea13, BekTbModel.bek04);
                    sea13g = sea13 * (seaModel.sea07 / 100);
                    sea13g = GlobalFn.Round(sea13g, BekTbModel.bek04);
                    sea13t = sea13 + sea13g;
                }

                DrMaster["sea13"] = sea13;
                DrMaster["sea13t"] = sea13t;
                DrMaster["sea13g"] = sea13g;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion

        #region WfResetAmtBySea06 依稅別更新單身及單頭的金額
        private void WfResetAmtBySea06()
        {
            try
            {
                foreach(DataRow dr in TabDetailList[0].DtSource.Rows)
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
    }
}
