/* 程式名稱: 託工送料單維護作業
   系統代號: mant311
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
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.BLL.Model;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using System.Text;
using YR.ERP.Shared;

namespace YR.ERP.Forms.Man
{
    public partial class FrmMant311 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;
        ManBLL BoMan = null;
        PurBLL BoPur = null;

        baa_tb BaaTbModel = null;
        #endregion

        #region 建構子
        public FrmMant311()
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
            this.StrFormID = "mant311";

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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("mfa01", SqlDbType.NVarChar) });
                //TabMaster.UserColumn = "seasecu";
                //TabMaster.GroupColumn = "seasecg";

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
            BoInv = new InvBLL(BoMaster.OfGetConntion());
            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
            BoMan = new ManBLL(BoMaster.OfGetConntion());
            BoPur = new PurBLL(BoMaster.OfGetConntion());
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
                    BoInv.TRAN = BoMaster.TRAN;
                    BoAdm.TRAN = BoMaster.TRAN;
                    BoMan.TRAN = BoMaster.TRAN;
                    BoPur.TRAN = BoMaster.TRAN;
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
                //單據確認
                sourceList = BoMan.OfGetMfaconfKVPList();
                WfSetUcomboxDataSource(ucb_mfaconf, sourceList);
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
            this.TabDetailList[0].TargetTable = "mfb_tb";
            this.TabDetailList[0].ViewTable = "vw_mant311s";
            keyParm = new SqlParameter("mfb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "mfa01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_mant311 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_mant311>();
                    WfSetDocPicture("", masterModel.mfaconf, "", pbxDoc);
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

                    WfSetControlReadonly(new List<Control> { ute_mfacreu, ute_mfacreg, udt_mfacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_mfamodu, ute_mfamodg, udt_mfamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_mfasecu, ute_mfasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.mfa01, "") != "")
                        WfSetControlReadonly(ute_mfa01, true);

                    WfSetControlReadonly(new List<Control> { ute_mfa01_c, ute_mfa03, ute_mfa03_c, ute_mfa04_c, ute_mfa05_c }, true);
                    WfSetControlReadonly(new List<Control> { ucb_mfaconf, udt_mfacond, ute_mfaconu }, true);
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_mfa01, true);
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
                            if (columnName == "mfb05" ||
                                //columnName == "mfb08" ||  //移除入庫倉功能
                                columnName == "mfb09" ||
                                columnName == "mfb11"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "mfb02")
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
                uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                SelectNextControl(this.uTab_Master, true, true, true, false);
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
                pDr["mfa00"] = "2";
                pDr["mfa02"] = Today;
                pDr["mfa04"] = LoginInfo.UserNo;
                pDr["mfa04_c"] = LoginInfo.UserName;
                pDr["mfa05"] = LoginInfo.DeptNo;
                pDr["mfa05_c"] = LoginInfo.DeptName;

                pDr["mfaconf"] = "N";
                pDr["mfacomp"] = LoginInfo.CompNo;
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
                        pDr["mfb02"] = WfGetMaxSeq(pDr.Table, "mfb02");
                        pDr["mfb05"] = 0;
                        //來源項次目前均預帶單頭的資料
                        pDr["mfb10"] = DrMaster["mfa06"];
                        pDr["mfbcomp"] = LoginInfo.CompNo;
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
            vw_mant311 masterModel;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_mant311>();
                if (masterModel.mfaconf != "N")
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
            vw_mant311 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_mant311>();
                if (masterModel.mfaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可刪除!");
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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                MessageInfo messageModel = new MessageInfo();
                vw_mant311 masterModel = null;
                vw_mant311s detailModel = null;

                #region 單頭-pick vw_mant311
                if (pDr.Table.Prefix.ToLower() == "vw_mant311")
                {
                    masterModel = DrMaster.ToItem<vw_mant311>();
                    switch (pColName.ToLower())
                    {
                        case "mfa01"://送料單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "man"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "22"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mfa04"://經辦人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mfa05"://經辦部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mfa06"://來源託工單挑選
                            WfShowPickUtility("p_mea2", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["mea01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mfa07"://送貨地址碼
                            //messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@pcb01", masterModel.mfa03));
                            WfShowPickUtility("p_pcb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pcb02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_mant311s
                if (pDr.Table.Prefix.ToLower() == "vw_mant311s")
                {
                    detailModel = pDr.ToItem<vw_mant311s>();
                    switch (pColName.ToLower())
                    {
                        case "mfb07"://出庫倉
                            if (GlobalFn.isNullRet(detailModel.mfb03, "") == "")
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
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.mfb03));
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

                        //case "mfb08"://入庫倉
                        //    if (GlobalFn.isNullRet(detailModel.mfb03, "") == "")
                        //    {
                        //        WfShowPickUtility("p_icb1", messageModel);
                        //        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        //        {
                        //            if (messageModel.DataRowList.Count > 0)
                        //                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icb01"], "");
                        //            else
                        //                pDr[pColName] = "";
                        //        }
                        //    }
                        //    else
                        //    {
                        //        messageModel.ParamSearchList = new List<SqlParameter>();
                        //        messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.mfb03));
                        //        WfShowPickUtility("p_icc1", messageModel);
                        //        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        //        {
                        //            if (messageModel.DataRowList.Count > 0)
                        //                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icc02"], "");
                        //            else
                        //                pDr[pColName] = "";
                        //        }
                        //    }
                        //    break;
                        case "mfb11"://來源項次
                            if (GlobalFn.varIsNull(detailModel.mfb10))
                            {
                                WfShowErrorMsg("來源單號未輸入,請確認!");
                                return false;
                            }
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@meb01", detailModel.mfb10));
                            messageModel.IsAutoQuery = true;
                            WfShowPickUtility("p_meb", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["meb02"], "");
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_mant311 masterModel = null;
            vw_mant311s detailModel = null;
            List<vw_mant311s> detailList = null;
            bab_tb babModel = null;
            UltraGrid uGrid = null;
            int ChkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant311>();
                if (e.Column.ToLower() != "mfa01" && GlobalFn.isNullRet(DrMaster["mfa01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false; ;
                }
                #region 單頭- vw_mant311
                if (e.Row.Table.Prefix.ToLower() == "vw_mant311")
                {
                    switch (e.Column.ToLower())
                    {
                        case "mfa01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "man", "22") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["mfa01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetMfa01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "mfa02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            var result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;

                        case "mfa04"://經辦人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mfa04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["mfa04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mfa05"://經辦部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mfa05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["mfa05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mfa06":
                            #region mfa06 託工單號
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                DrMaster["mfa03"] = "";
                                DrMaster["mfa03_c"] = "";
                                DrMaster["mfa07"] = "";
                                return true;
                            }
                            else
                            {
                                var meaModel = BoMan.OfGetMeaModel(e.Value.ToString());
                                if (meaModel == null)
                                {
                                    WfShowErrorMsg("無此託工單,請檢核!");
                                    return false;
                                }
                                if (meaModel.meaconf != "Y")
                                {
                                    WfShowErrorMsg("並非有效的託工單,請檢核!");
                                    return false;
                                }

                                var pcaModel = BoPur.OfGetPcaModel(meaModel.mea03);
                                if (pcaModel == null)
                                {
                                    WfShowErrorMsg("無此廠商資料,請檢核!");
                                    return false;
                                }
                                if (pcaModel.pcaconf != "Y")
                                {
                                    WfShowErrorMsg("廠商資料未確認,請檢核!");
                                    return false;
                                }
                                DrMaster["mfa03"] = pcaModel.pca01;
                                DrMaster["mfa03_c"] = pcaModel.pca03;
                                DrMaster["mfa07"] = "";

                                var msg = "請問是否要自動帶入單身資料?";
                                if (TabDetailList[0].DtSource != null && TabDetailList[0].DtSource.Rows.Count > 0)
                                {
                                    msg += "\n若單身有資料時，會先清除原本的資料！";
                                }
                                var dialogResult = WfShowConfirmMsg(msg, MessageBoxButtons.YesNo);
                                //var inWarehouse = "";//入庫倉變數
                                if (dialogResult == DialogResult.Yes)
                                {
                                    //清除單身資料
                                    if (TabDetailList[0].DtSource != null && TabDetailList[0].DtSource.Rows.Count > 0)
                                    {
                                        for (int i = TabDetailList[0].DtSource.Rows.Count - 1; i >= 0; i--)
                                        {
                                            if (WfToolbarDetailDelete(0, TabDetailList[0].DtSource.Rows[i], false) == false)
                                                return false;
                                        }
                                    }

                                    ////取得要入庫的倉
                                    //MessageInfo messageModel = new MessageInfo();
                                    //messageModel.ParamSearchList = new List<SqlParameter>();
                                    //messageModel.IsAutoQuery = true;
                                    //WfShowPickUtility("p_icb1", messageModel);
                                    //if (messageModel.Result != System.Windows.Forms.DialogResult.OK)
                                    //    return true;
                                    //inWarehouse = GlobalFn.isNullRet(messageModel.DataRowList[0]["icb01"], "");//入庫倉

                                    //勾選要帶入的明細
                                    MessageInfo messageModel = new MessageInfo();
                                    messageModel.ParamSearchList = new List<SqlParameter>();
                                    messageModel.IsAutoQuery = true;
                                    messageModel.IntMaxRow = 999;
                                    messageModel.StrMultiColumn = "meb02";//回傳欄位
                                    messageModel.ParamSearchList.Add(new SqlParameter("@meb01", e.Value.ToString()));
                                    WfShowPickUtility("p_meb", messageModel);
                                    if (messageModel.Result != System.Windows.Forms.DialogResult.OK
                                        || string.IsNullOrWhiteSpace(messageModel.StrMultiRtn)
                                        )
                                        return true;
                                    var meb02StrList = messageModel.StrMultiRtn.Split('|');
                                    foreach (string strMeb02 in meb02StrList.OrderBy(p => p))
                                    {
                                        var mebModel = BoMan.OfGetMebModel(e.Value.ToString(), Convert.ToInt32(strMeb02));
                                        DataRow drMfbNew = TabDetailList[0].DtSource.NewRow();
                                        WfSetDetailRowDefault(0, drMfbNew);
                                        drMfbNew["mfb03"] = mebModel.meb03;//料號
                                        drMfbNew["mfb04"] = mebModel.meb04;//品名
                                        if (mebModel.meb05 - mebModel.meb08 <= 0)//數量
                                            drMfbNew["mfb05"] = 0;
                                        else
                                            drMfbNew["mfb05"] = mebModel.meb05 - mebModel.meb08;
                                        drMfbNew["mfb06"] = mebModel.meb06;//單位
                                        drMfbNew["mfb07"] = mebModel.meb07;//出庫倉
                                        //drMfbNew["mfb08"] = inWarehouse;//入庫倉
                                        drMfbNew["mfb09"] = "";//備註
                                        drMfbNew["mfb10"] = mebModel.meb01;//來源單別
                                        drMfbNew["mfb11"] = mebModel.meb02;//來源項次
                                        TabDetailList[0].DtSource.Rows.Add(drMfbNew);
                                    }
                                    TabDetailList[0].UGrid.DataBind();
                                }
                            }
                            break;
                            #endregion

                        case "mfa07"://送貨地址碼
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(masterModel.mfa03, "") == "")
                            {
                                WfShowErrorMsg("請先輸入廠商編號!");
                                return false;
                            }
                            if (BoPur.OfChkPcbPKExists(masterModel.mfa03, e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此廠商地址碼,請檢核!");
                                return false;
                            }
                            break;
                    }
                }
                #endregion

                #region 單身- vw_mant311s
                if (e.Row.Table.Prefix.ToLower() == "vw_mant311s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_mant311s>();
                    detailList = e.Row.Table.ToList<vw_mant311s>();
                    babModel = BoBas.OfGetBabModel(masterModel.mfa01);

                    switch (e.Column.ToLower())
                    {
                        case "mfb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.mfb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;
                        case "mfb05"://數量
                            if (GlobalFn.varIsNull(detailModel.mfb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["meb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入數量!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["meb05"]);
                                return false;
                            }
                            if (WfChkMeb05(e.Row, detailModel) == false)
                                return false;
                            break;

                        case "mfb07"://出庫倉
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoInv.OfChkIcbPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此倉庫別,請確認!");
                                return false;
                            }
                            if (!GlobalFn.varIsNull(detailModel.mfb08) && e.Value.ToString() == detailModel.mfb08)
                            {
                                WfShowErrorMsg("出庫倉與入庫倉不可相同,請確認!");
                                return false;
                            }

                            break;
                        //case "mfb08"://入庫倉
                        //    if (GlobalFn.varIsNull(e.Value))
                        //    {
                        //        return true;
                        //    }
                        //    if (BoInv.OfChkIcbPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                        //    {
                        //        WfShowErrorMsg("無此倉庫別,請確認!");
                        //        return false;
                        //    }
                        //    if (!GlobalFn.varIsNull(detailModel.mfb07)&& e.Value.ToString()==detailModel.mfb07)
                        //    {
                        //        WfShowErrorMsg("出庫倉與入庫倉不可相同,請確認!");
                        //        return false;
                        //    }
                        //    break;

                        case "mfb11"://來源項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.mfb11, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("來源項次已存在,請檢核!");
                                e.Row["mfb03"] = "";//料號
                                e.Row["mfb04"] = "";//品名
                                e.Row["mfb05"] = 0;//數量
                                e.Row["mfb06"] = "";//單位
                                e.Row["mfb07"] = "";//出庫倉
                                return false;
                            }
                            var mebModel = BoMan.OfGetMebModel(detailModel.mfb10, Convert.ToInt16(detailModel.mfb11));
                            if (mebModel == null)
                            {
                                WfShowErrorMsg("無此來源項次,請檢核!");
                                return false;
                            }
                            e.Row["mfb03"] = mebModel.meb03;//料號
                            e.Row["mfb04"] = mebModel.meb04;//品名
                            if (mebModel.meb05 - mebModel.meb08 <= 0)//數量
                                e.Row["mfb05"] = 0;
                            else
                                e.Row["mfb05"] = mebModel.meb05 - mebModel.meb08;
                            e.Row["mfb06"] = mebModel.meb06;//單位
                            e.Row["mfb07"] = mebModel.meb07;//出庫倉
                            //e.Row["mfb08"] = "";//入庫倉
                            //e.Row["mfb09"] = "";//備註
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
            vw_mant311 masterModel = null;
            vw_mant311s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {

                masterModel = DrMaster.ToItem<vw_mant311>();
                if (!GlobalFn.varIsNull(masterModel.mfa01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.mfa01, ""));
                #region 單頭資料檢查
                chkColName = "mfa01";       //託工單號
                chkControl = ute_mfa01;
                if (GlobalFn.varIsNull(masterModel.mfa01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mfa02";       //出庫日期
                chkControl = udt_mfa02;
                if (GlobalFn.varIsNull(masterModel.mfa02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.mfa02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "mfa06";       //託工單號
                chkControl = ute_mfa06;
                if (GlobalFn.varIsNull(masterModel.mfa06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mfa03";       //廠商編號
                chkControl = ute_mfa03;
                if (GlobalFn.varIsNull(masterModel.mfa03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mfa04";       //人員
                chkControl = ute_mfa04;
                if (GlobalFn.varIsNull(masterModel.mfa04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mfa05";       //部門
                chkControl = ute_mfa05;
                if (GlobalFn.varIsNull(masterModel.mfa05))
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

                    detailModel = drTemp.ToItem<vw_mant311s>();
                    chkColName = "mfb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.mfb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "mfb11";   //來源項次
                    if (GlobalFn.varIsNull(detailModel.mfb11))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "mfb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.mfb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "mfb05";   //數量
                    #region mfb05 數量
                    if (GlobalFn.varIsNull(detailModel.mfb05) || detailModel.mfb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    if (WfChkMeb05(drTemp, detailModel) == false)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        //msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        //msg += "不可為空白或小於0!";
                        //WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "mfb07";   //出庫倉
                    if (GlobalFn.varIsNull(detailModel.mfb07))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    //chkColName = "mfb08";   //入庫倉
                    //if (GlobalFn.varIsNull(detailModel.mfb08))
                    //{
                    //    this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                    //    msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();                        msg += "不可為空白";
                    //    WfShowErrorMsg(msg);
                    //    WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                    //    return false;
                    //}
                    ////檢查出庫倉與入庫倉是否相同
                    //if (detailModel.mfb07==detailModel.mfb08)
                    //{
                    //    this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                    //    msg = "出庫倉與入庫倉不可為相同";
                    //    WfShowErrorMsg(msg);
                    //    WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                    //    return false;
                    //}
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
            string mfa01New, errMsg;
            vw_mant311 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant311>();
                if (FormEditMode == YREditType.新增)
                {
                    mfa01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.mfa01, ModuleType.man, (DateTime)masterModel.mfa02, out mfa01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["mfa01"] = mfa01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["mfasecu"] = LoginInfo.UserNo;
                        DrMaster["mfasecg"] = LoginInfo.GroupNo;
                        DrMaster["mfacreu"] = LoginInfo.UserNo;
                        DrMaster["mfacreg"] = LoginInfo.DeptNo;
                        DrMaster["mfacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["mfamodu"] = LoginInfo.UserNo;
                        DrMaster["mfamodg"] = LoginInfo.DeptNo;
                        DrMaster["mfamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["mfbcreu"] = LoginInfo.UserNo;
                            drDetail["mfbcreg"] = LoginInfo.DeptNo;
                            drDetail["mfbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["mfbmodu"] = LoginInfo.UserNo;
                            drDetail["mfbmodg"] = LoginInfo.DeptNo;
                            drDetail["mfbmodd"] = Now;
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

                bt = new ButtonTool("Manr311");
                adoModel = BoAdm.OfGetAdoModel("manr311");
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

                    case "Manr311":
                        vw_manr311 queryModel;
                        vw_mant311 masterModel;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_mant311>();
                        queryModel = new vw_manr311();
                        queryModel.mfa01 = masterModel.mfa01;
                        queryModel.mfa03 = "";
                        queryModel.mfa06 = "";
                        queryModel.jump_yn = "N";
                        queryModel.order_by = "1";

                        FrmManr311 rpt = new FrmManr311(this.LoginInfo, queryModel, true, true);
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

        #region WfAppendUpdate 存檔後處理額外的資料表 新增、修改、刪除...
        protected override bool WfAppendUpdate()
        {
            DataTable dtIcc;
            DataRow drIcc;
            CommonBLL boAppendIcc;
            StringBuilder sbSql;
            vw_mant311s detailModel = null;
            try
            {
                //boAppendIcc = new InvBLL(BoMaster.OfGetConntion()); //新增料號庫存明細資料
                //boAppendIcc.TRAN = BoMaster.TRAN;
                //boAppendIcc.OfCreateDao("icc_tb", "*", "");
                //sbSql = new StringBuilder();
                //sbSql.AppendLine("SELECT * FROM icc_tb");
                //sbSql.AppendLine("WHERE 1<>1");
                //dtIcc = boAppendIcc.OfGetDataTable(sbSql.ToString());

                //foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                //{
                //    detailModel = dr.ToItem<vw_mant311s>();
                //    if (BoInv.OfChkIccPKExists(detailModel.mfb03, detailModel.mfb08) == false)
                //    {
                //        if (dtIcc.Rows.Count > 0)
                //        {
                //            var drIccs = dtIcc.Select(string.Format("icc01='{0}' AND icc02='{1}'", detailModel.mfb03, detailModel.mfb08));
                //            if (drIccs != null && drIccs.Length > 0)
                //                continue;
                //        }
                //        drIcc = dtIcc.NewRow();
                //        drIcc["icc01"] = detailModel.mfb03;  //料號
                //        drIcc["icc02"] = detailModel.mfb08;
                //        drIcc["icc03"] = "";
                //        drIcc["icc04"] = detailModel.mfb06;
                //        drIcc["icc05"] = 0;
                //        dtIcc.Rows.Add(drIcc);
                //    }
                //}

                //if (dtIcc.Rows.Count > 0)
                //{
                //    if (boAppendIcc.OfUpdate(dtIcc) < 1)
                //    {
                //        WfShowErrorMsg("新增料號庫存明細檔(icc_tb)失敗!");
                //        return false;
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

        //*****************************表單自訂Fuction****************************************
        #region WfSetMfa01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetMfa01RelReadonly(string pMfa01)
        {
            bab_tb babModel = null;
            string bab01;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_mfa01 }, true);
                //bab01 = pMea01.Substring(0, GlobalFn.isNullRet(BaaTbModel.baa06, 0));
                //babModel = BoBas.OfGetBabModel(bab01);
                //if (babModel.bab08 == "Y")   //有來源單據
                //{
                //    WfSetControlReadonly(new List<Control> { ute_pga12 }, true);
                //    WfSetControlReadonly(new List<Control> { ute_pga16 }, false);
                //}
                //else
                //{
                //    WfSetControlReadonly(new List<Control> { ute_pga12 }, false);
                //    WfSetControlReadonly(new List<Control> { ute_pga16 }, true);

                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkMeb05 數量檢查--已送料量是否大於預計數量
        //目前先檢查是否有超出託工單的預計數量,後續要在單別加入是否可超出的檢查
        private bool WfChkMeb05(DataRow pdr, vw_mant311s pListDetail)
        {
            List<vw_mant311s> detailList = null;
            meb_tb mebModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換送料單數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的送料單數量加總
            decimal otherQtyTotal = 0;      //其他單據的數量加總
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                detailList = pdr.Table.ToList<vw_mant311s>();
                if (GlobalFn.varIsNull(pListDetail.mfb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.mfb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.mfb06))
                {
                    errMsg = "請先輸入單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (pListDetail.mfb05 <= 0)
                {
                    errMsg = "數量應大於0!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mfa01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                mebModel = BoMan.OfGetMebModel(pListDetail.mfb10, Convert.ToInt16(pListDetail.mfb11));
                if (babModel == null)
                {
                    errMsg = "Get meb_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //取得本身單據中的所有數量轉換銷貨單位後的總和!
                //先取本身這筆
                docThisQty = pListDetail.mfb05;
                //再取其他筆加總
                docOtherQtyTotal = detailList.Where(p => p.mfb02 != pListDetail.mfb02)
                                   .Where(p => p.mfb10 == pListDetail.mfb10 && p.mfb11 == pListDetail.mfb11)
                                   .Sum(p => p.mfb05);
                //取得其他單據上的加總
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT SUM(mfb05) FROM mfa_tb");
                sbSql.AppendLine("  INNER JOIN mfb_tb ON mfa01=mfb01");
                sbSql.AppendLine("WHERE mfaconf<>'X'");
                sbSql.AppendLine("  AND mfa01 <> @mfa01");
                sbSql.AppendLine("  AND mfb10 = @mfb10 AND mfb11 = @mfb11");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@mfa01", GlobalFn.isNullRet(DrMaster["mfa01"], "")));
                sqlParmList.Add(new SqlParameter("@mfb10", pListDetail.mfb10));
                sqlParmList.Add(new SqlParameter("@mfb11", pListDetail.mfb11));
                otherQtyTotal = GlobalFn.isNullRet(BoMan.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (mebModel.meb05 < (docThisQty + docOtherQtyTotal + otherQtyTotal))
                {
                    errMsg = string.Format("項次{0} 送料單最大可輸入數量為 {1}",
                                            pListDetail.mfb02.ToString(),
                                            (mebModel.meb05 - docOtherQtyTotal - otherQtyTotal).ToString()
                                           );
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

        #region WfChkMeb08 數量檢查--已送料量被還原後是否小於耗用量
        private bool WfChkMeb08(DataRow pdr, vw_mant311s pListDetail)
        {
            List<vw_mant311s> detailList = null;
            meb_tb mebModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換送料單數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的送料單數量加總
            decimal otherQtyTotal = 0;      //其他單據的數量加總
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                detailList = pdr.Table.ToList<vw_mant311s>();
                if (GlobalFn.varIsNull(pListDetail.mfb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.mfb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.mfb06))
                {
                    errMsg = "請先輸入單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (pListDetail.mfb05 <= 0)
                {
                    errMsg = "數量應大於0!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mfa01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                mebModel = BoMan.OfGetMebModel(pListDetail.mfb10, Convert.ToInt16(pListDetail.mfb11));
                if (babModel == null)
                {
                    errMsg = "Get meb_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //取得本身單據中的所有數量總和!
                //先取本身這筆
                docThisQty = pListDetail.mfb05;
                //再取其他筆加總
                docOtherQtyTotal = detailList.Where(p => p.mfb02 != pListDetail.mfb02)
                                   .Where(p => p.mfb10 == pListDetail.mfb10 && p.mfb11 == pListDetail.mfb11)
                                   .Sum(p => p.mfb05);
                //取得其他單據上的加總--no use
                //sbSql = new StringBuilder();
                //sbSql.AppendLine("SELECT SUM(mfb05) FROM mfa_tb");
                //sbSql.AppendLine("  INNER JOIN mfb_tb ON mfa01=mfb01");
                //sbSql.AppendLine("WHERE mfaconf<>'X'");
                //sbSql.AppendLine("  AND mfa01 <> @mfa01");
                //sbSql.AppendLine("  AND mfb10 = @mfb10 AND mfb11 = @mfb11");
                //sqlParmList = new List<SqlParameter>();
                //sqlParmList.Add(new SqlParameter("@mfa01", GlobalFn.isNullRet(DrMaster["mfa01"], "")));
                //sqlParmList.Add(new SqlParameter("@mfb10", pListDetail.mfb10));
                //sqlParmList.Add(new SqlParameter("@mfb11", pListDetail.mfb11));
                //otherQtyTotal = GlobalFn.isNullRet(BoMan.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (mebModel.meb09 > (mebModel.meb08 - docOtherQtyTotal - docThisQty))
                {
                    errMsg = string.Format("項次{0} 送料單取消確認後託工單總送料量不得小於已耗用量!",
                                            pListDetail.mfb02.ToString()
                                           );
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

        #region WfConfirm 確認
        private void WfConfirm()
        {
            mfa_tb mfaModel = null;
            mfb_tb mfbModel = null;
            mea_tb meaBeforeModel = null;   //保留更新前的託工單狀態
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
                mfaModel = DrMaster.ToItem<mfa_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                meaBeforeModel = BoMan.OfGetMeaModel(mfaModel.mfa06);//保留更新前的託工單狀態

                //檢查後更改單頭狀態,讓後面可以直接使用
                mfaModel.mfaconf = "Y";
                mfaModel.mfacond = Today;
                mfaModel.mfaconu = LoginInfo.UserNo;

                //更新託工單明細已送料量
                if (WfUpdMeb08(true) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    mfbModel = dr.ToItem<mfb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("2", mfbModel.mfb03, mfbModel.mfb07, mfbModel.mfb05, out errMsg) == false)//出庫
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                    //更新庫存交易歷史檔
                    if (BoInv.OfStockPost("mant311", mfaModel, mfbModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }
                #region 確認託工單狀態,是否需要做更新
                if (meaBeforeModel.meastat == "2")
                {
                    if (WfUpdMea(mfaModel.mfa06, "3") == false)
                    {
                        WfRollback();
                        return;
                    }
                }
                #endregion

                DrMaster["mfaconf"] = "Y";
                DrMaster["mfacond"] = Today;
                DrMaster["mfaconu"] = LoginInfo.UserNo;
                DrMaster["mfamodu"] = LoginInfo.UserNo;
                DrMaster["mfamodg"] = LoginInfo.DeptNo;
                DrMaster["mfamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                mfaModel = DrMaster.ToItem<mfa_tb>();
                WfSetDocPicture("", mfaModel.mfaconf, "", pbxDoc);
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
            vw_mant311 masterModel = null;
            vw_mant311s detailModel = null;
            List<vw_mant311s> detailList = null;
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant311>();
                if (masterModel.mfaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.mfa02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_mant311s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_mant311s>();
                    //檢查託工單是否要超出預計數量
                    if (WfChkMeb05(drTemp, detailModel) == false)
                    {
                        return false;
                    }
                }

                //檢查送料單的數量是否足夠做出庫
                var MfbSumList =                         //依送料單的料號及出庫倉做加總
                        from mfb in detailList
                        where mfb.mfb05 > 0
                        group mfb by new { mfb.mfb03, mfb.mfb07 } into mfbSumTemp
                        select new
                        {
                            mfb03 = mfbSumTemp.Key.mfb03,
                            mfb07 = mfbSumTemp.Key.mfb07,
                            mfb05 = mfbSumTemp.Sum(p => p.mfb05)
                        }
                    ;
                foreach (var mfbSumModel in MfbSumList)
                {
                    icc05 = BoInv.OfGetIcc05(mfbSumModel.mfb03, mfbSumModel.mfb07);
                    if (icc05 < mfbSumModel.mfb05)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", mfbSumModel.mfb03, icc05));
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
            mfa_tb mfaModel = null;
            mfb_tb mfbModel = null;
            mea_tb meaBeforeModel = null;   //保留更新前的託工單狀態
            string errMsg;
            StringBuilder sbSql = null;
            List<SqlParameter> sqlParmsList = null;
            int chkCnts = 0;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)        //這裡會LOCK資料
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;

                WfSetBllTransaction();
                mfaModel = DrMaster.ToItem<mfa_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }
                meaBeforeModel = BoMan.OfGetMeaModel(mfaModel.mfa06);//保留更新前的託工單狀態

                //更新託工單明細已送料量
                if (WfUpdMeb08(false) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    mfbModel = dr.ToItem<mfb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("1", mfbModel.mfb03, mfbModel.mfb07, mfbModel.mfb05, out errMsg) == false) //取消時出庫倉應入庫
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔
                    if (BoInv.OfDelIna(mfbModel.mfb01, mfbModel.mfb02, "2", out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                }

                #region 確認託工單狀態,是否需要做更新
                if (meaBeforeModel.meastat == "3")
                {
                    //檢核該筆託工單是否仍有其他已送量,直接抓資料庫
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT COUNT(1)");
                    sbSql.AppendLine("FROM meb_tb");
                    sbSql.AppendLine("WHERE meb01=@meb01");
                    sbSql.AppendLine("AND meb08>0");
                    sqlParmsList = new List<SqlParameter>();
                    sqlParmsList.Add(new SqlParameter("meb01", mfaModel.mfa06));
                    chkCnts = GlobalFn.isNullRet(BoMan.OfGetFieldValue(sbSql.ToString(), sqlParmsList.ToArray()), 0);
                    if (chkCnts == 0)
                    {
                        if (WfUpdMea(mfaModel.mfa06, "2") == false)
                        {
                            WfRollback();
                            return;
                        }
                    }

                }
                #endregion

                DrMaster["mfaconf"] = "N";
                DrMaster["mfacond"] = DBNull.Value;
                DrMaster["mfaconu"] = "";
                DrMaster["mfamodu"] = LoginInfo.UserNo;
                DrMaster["mfamodg"] = LoginInfo.DeptNo;
                DrMaster["mfamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                mfaModel = DrMaster.ToItem<mfa_tb>();
                WfSetDocPicture("", mfaModel.mfaconf, "", pbxDoc);
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
            vw_mant311 masterModel = null;
            vw_mant311s detailModel = null;
            List<vw_mant311s> detailList = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            decimal icc05 = 0;
            int iChkCnts = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant311>();
                if (masterModel.mfaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.mfa02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_mant311s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_mant311s>();
                    //檢查託工單是否要取消確認後總送料量是否小於已耗用量
                    if (WfChkMeb08(drTemp, detailModel) == false)
                    {
                        return false;
                    }
                }

                //todo:需檢查是否為母製令

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
            vw_mant311 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_mant311>();

                if (masterModel.mfaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.mfaconf == "N")//走作廢
                {

                    DrMaster["mfaconf"] = "X";
                    DrMaster["mfaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.mfaconf == "X")
                {
                    ////檢查採購單是否為有效資料
                    //sbSql = new StringBuilder();
                    //sbSql.AppendLine("SELECT COUNT(1)");
                    //sbSql.AppendLine("FROM pfa_tb");
                    //sbSql.AppendLine("WHERE exists ");
                    //sbSql.AppendLine("      (SELECT 1 FROM pgb_tb WHERE pgb01=@pgb01");
                    //sbSql.AppendLine("      AND pgb11=pfa01)");
                    //sbSql.AppendLine("AND ISNULL(pfaconf,'')<>'Y'");
                    //sqlParmList = new List<SqlParameter>();
                    //sqlParmList.Add(new SqlParameter("@pgb01", GlobalFn.isNullRet(DrMaster["pga01"], "")));
                    //iChkCnts = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                    //if (iChkCnts > 0)
                    //{
                    //    WfShowErrorMsg("採購單含有未確認資料!不可作廢還原!");
                    //    WfRollback();
                    //    return;
                    //}
                    DrMaster["mfaconf"] = "N";
                    DrMaster["mfaconu"] = "";
                }

                DrMaster["mfamodu"] = LoginInfo.UserNo;
                DrMaster["mfamodg"] = LoginInfo.DeptNo;
                DrMaster["mfamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_mant311>();
                WfSetDocPicture("", masterModel.mfaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfUpdMeb08 更新託工單已出貨數量 確認/取消確認
        private bool WfUpdMeb08(bool pbConfirm)
        {
            bab_tb babModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            DataTable dtMfbDistinct = null;
            string meb01;
            decimal meb02;
            decimal docQty = 0, otherDocQty = 0;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mfa01"], ""));
                if (babModel == null)
                {
                    WfShowErrorMsg("Get bab_tb Error!");
                    return false;
                }
                if (babModel.bab08 == "Y")   //更新託工單已送料量
                {
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT DISTINCT mfb10,mfb11,SUM(mfb05) mfb05");
                    sbSql.AppendLine("FROM mfb_tb");
                    sbSql.AppendLine("WHERE mfb01=@mfb01");
                    sbSql.AppendLine("  AND ISNULL(mfb10,'')<>''");
                    sbSql.AppendLine("GROUP BY mfb10,mfb11");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@mfb01", GlobalFn.isNullRet(DrMaster["mfa01"], "")));
                    dtMfbDistinct = BoMan.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                    if (dtMfbDistinct == null)
                        return true;
                    foreach (DataRow dr in dtMfbDistinct.Rows)
                    {
                        meb01 = dr["mfb10"].ToString();
                        meb02 = Convert.ToDecimal(dr["mfb11"]);
                        docQty = Convert.ToDecimal(dr["mfb05"]);

                        sbSql = new StringBuilder();
                        sbSql.AppendLine("SELECT SUM(mfb05) FROM mfa_tb");
                        sbSql.AppendLine("  INNER JOIN mfb_tb ON mfa01=mfb01");
                        sbSql.AppendLine("WHERE mfaconf='Y'");
                        sbSql.AppendLine("AND mfb10=@mfb10 AND mfb11=@mfb11");
                        sbSql.AppendLine("AND mfa01<>@mfa01");


                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@mfb10", meb01));
                        sqlParmList.Add(new SqlParameter("@mfb11", meb02));
                        sqlParmList.Add(new SqlParameter("@mfa01", GlobalFn.isNullRet(DrMaster["mfa01"], "")));
                        otherDocQty = 0;
                        otherDocQty = GlobalFn.isNullRet(BoMan.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);

                        sbSql = new StringBuilder();
                        sbSql = sbSql.AppendLine("UPDATE meb_tb");
                        sbSql = sbSql.AppendLine("SET meb08=@meb08");
                        sbSql = sbSql.AppendLine("WHERE meb01=@meb01");
                        sbSql = sbSql.AppendLine("AND meb02=@meb02");
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@meb01", meb01));
                        sqlParmList.Add(new SqlParameter("@meb02", meb02));
                        if (pbConfirm)  //確認的要加本身單據
                            sqlParmList.Add(new SqlParameter("@meb08", docQty + otherDocQty));
                        else
                            sqlParmList.Add(new SqlParameter("@meb08", otherDocQty));

                        if (BoMan.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray()) != 1)
                        {
                            WfShowErrorMsg("更新託工單已送料數量失敗!");
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

        #region WfUpdMea 更新託工單狀態碼
        private bool WfUpdMea(string pMea01, string pMeastat)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine(string.Format("UPDATE mea_tb"));
                sbSql.AppendLine(string.Format("SET meastat=@meastat"));
                sbSql.AppendLine(string.Format("WHERE mea01=@mea01"));
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@mea01", pMea01));
                sqlParmList.Add(new SqlParameter("@meastat", pMeastat));
                if (BoMan.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray()) <= 0)
                {
                    WfShowErrorMsg("更新託工單狀態碼，失敗!");
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
    }
}
