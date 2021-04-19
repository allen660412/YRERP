/* 程式名稱: 託工退回單維護作業
   系統代號: mant412
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
using System.Text;
using Infragistics.Win.UltraWinToolbars;
using YR.ERP.Shared;

namespace YR.ERP.Forms.Man
{
    public partial class FrmMant412 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;
        PurBLL BoPur = null;
        ManBLL BoMan = null;

        baa_tb BaaTbModel = null;
        bek_tb BekTbModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmMant412()
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
            this.StrFormID = "mant412";
            uTab_Header.Tabs[0].Text = "基本資料";
            uTab_Header.Tabs[1].Text = "狀態";

            this.IntTabCount = 2;
            this.IntMasterGridPos = 2;
            uTab_Master.Tabs[0].Text = "基本資料";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("mia01", SqlDbType.NVarChar) });
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
            BoPur = new PurBLL(BoMaster.OfGetConntion());
            BoMan = new ManBLL(BoMaster.OfGetConntion());
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
                    BoPur.TRAN = BoMaster.TRAN;
                    BoMan.TRAN = BoMaster.TRAN;
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
                sourceList = BoPur.OfGetTaxTypeKVPList();
                WfSetUcomboxDataSource(ucb_mia06, sourceList);

                //發票聯數
                sourceList = BoPur.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_mia09, sourceList);

                //單據確認
                sourceList = BoMan.OfGetMiaconfKVPList();
                WfSetUcomboxDataSource(ucb_miaconf, sourceList);
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
            this.TabDetailList[0].TargetTable = "mib_tb";
            this.TabDetailList[0].ViewTable = "vw_mant412s";
            keyParm = new SqlParameter("mib01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "mia01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_mant412 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_mant412>();
                    WfSetDocPicture("", masterModel.miaconf, "", pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.mia10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.mia10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.mia10));
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

                    WfSetControlReadonly(new List<Control> { ute_miacreu, ute_miacreg, udt_miacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_miamodu, ute_miamodg, udt_miamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_miasecu, ute_miasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.mia01, "") != "")
                        WfSetControlReadonly(ute_mia01, true);

                    WfSetControlReadonly(new List<Control> { ute_mia01_c,  ute_mia03_c, ute_mia04_c, ute_mia05_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_mia07, ucx_mia08, ute_mia10 }, true);
                    WfSetControlReadonly(new List<Control> { ute_mia11_c}, true);
                    WfSetControlReadonly(new List<Control> { ute_mia13, ute_mia13t, ute_mia13g }, true);
                    WfSetControlReadonly(new List<Control> { ucb_miaconf, udt_miacond, ute_miaconu }, true);
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_mia01, true);
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
                //switch (pCurTabDetail)
                //{
                //    case 0:
                //        foreach (UltraGridCell ugc in pUgr.Cells)
                //        {
                //            columnName = ugc.Column.Key.ToLower();
                //            //先控可以輸入的
                //            if (columnName == "mfb05" ||
                //                columnName == "mfb09" ||
                //                columnName == "mfb11"
                //                )
                //            {
                //                WfSetControlReadonly(ugc, false);
                //                continue;
                //            }

                //            if (columnName == "mfb02")
                //            {
                //                if (pDr.RowState == DataRowState.Added)//新增時
                //                    WfSetControlReadonly(ugc, false);
                //                else    //修改時
                //                {
                //                    WfSetControlReadonly(ugc, true);
                //                }
                //                continue;
                //            }
                //            WfSetControlReadonly(ugc, true);
                //        }
                //        break;
                //}
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["mia00"] = "2";
                pDr["mia02"] = Today;
                pDr["mia04"] = LoginInfo.UserNo;
                pDr["mia04_c"] = LoginInfo.UserName;
                pDr["mia05"] = LoginInfo.DeptNo;
                pDr["mia05_c"] = LoginInfo.DeptName;
                pDr["mia06"] = "1"; //課稅別預設含稅
                WfSetMia06Relation("1");

                pDr["mia10"] = BaaTbModel.baa04;
                pDr["mia13"] = 0;
                pDr["mia13t"] = 0;
                pDr["mia13g"] = 0;
                pDr["miaconf"] = "N";
                pDr["miacomp"] = LoginInfo.CompNo;
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
                        pDr["mib02"] = WfGetMaxSeq(pDr.Table, "mib02");
                        pDr["mib05"] = 0;
                        //來源項次目前均預帶單頭的資料
                        pDr["mib10"] = DrMaster["mia06"];
                        pDr["mibcomp"] = LoginInfo.CompNo;
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
            vw_mant412 masterModel;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_mant412>();
                if (masterModel.miaconf != "N")
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
                    case "vw_mant412s":
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

        #region WfPreDeleteCheck 進主檔刪除前檢查
        protected override bool WfPreDeleteCheck(DataRow pDr)
        {
            vw_mant412 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_mant412>();
                if (masterModel.miaconf != "N")
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
                vw_mant412 masterModel = null;
                vw_mant412s detailModel = null;

                masterModel = DrMaster.ToItem<vw_mant412>();
                #region 單頭-pick vw_mant411
                if (pDr.Table.Prefix.ToLower() == "vw_mant412")
                {
                    switch (pColName.ToLower())
                    {
                        case "mia01"://退回單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "man"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "25"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "mia03"://廠商編號
                            WfShowPickUtility("p_pca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "mia04"://經辦人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mia05"://經辦部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "mia11"://付款條件
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
                            break;

                    }
                }
                #endregion

                #region 單身-pick vw_mant412s
                if (pDr.Table.Prefix.ToLower() == "vw_mant412s")
                {
                    detailModel = pDr.ToItem<vw_mant412s>();
                    switch (pColName.ToLower())
                    {
                        case "mib07"://退庫倉
                            if (GlobalFn.isNullRet(detailModel.mib03, "") == "")
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
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.mib03));
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

                        case "mib11"://來源託工單
                            //todo:先抓全部之後再控卡
                            if (GlobalFn.isNullRet(masterModel.mia03, "") != "")
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.StrWhereAppend = " AND mea03=@mea03";
                                messageModel.StrWhereAppend += " AND (mea26-mea27)>0";
                                messageModel.ParamSearchList.Add(new SqlParameter("@mea03", masterModel.mia03));
                            }

                            WfShowPickUtility("p_mea2", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["mea01"], "");
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_mant412 masterModel = null;
            vw_mant412s detailModel = null;
            List<vw_mant412s> detailList = null;
            bab_tb babModel = null;
            UltraGrid uGrid = null;
            int ChkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant412>();
                if (e.Column.ToLower() != "mia01" && GlobalFn.isNullRet(DrMaster["mia01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false; ;
                }
                #region 單頭- vw_mant412
                if (e.Row.Table.Prefix.ToLower() == "vw_mant412")
                {
                    switch (e.Column.ToLower())
                    {
                        case "mia01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "man", "25") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["mia01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetMia01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "mia02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            var result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;

                        case "mia03"://廠商
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mia03_c"] = "";
                                return true;
                            }
                            if (BoPur.OfChkPcaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此廠商資料,請檢核!");
                                return false;
                            }
                            WfSetMia03Relation(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mia04"://經辦人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mia04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["mia04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mia05"://經辦部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mia05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["mia05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mia06"://課稅別
                            WfSetMia06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByMia06();
                            break;


                        case "mia11"://付款條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mia11_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("1", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此付款條件,請檢核!");
                                return false;
                            }
                            e.Row["mia11_c"] = BoBas.OfGetBef03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;

                    }
                }
                #endregion

                #region 單身- vw_mant411s
                if (e.Row.Table.Prefix.ToLower() == "vw_mant412s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_mant412s>();
                    detailList = e.Row.Table.ToList<vw_mant412s>();
                    babModel = BoBas.OfGetBabModel(masterModel.mia01);

                    switch (e.Column.ToLower())
                    {
                        case "mib02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.mib02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;
                        case "mib05"://數量
                            if (GlobalFn.varIsNull(detailModel.mib11))
                            {
                                WfShowErrorMsg("請先輸入託工單號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["mib11"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入數量!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["mib05"]);
                                return false;
                            }
                            //todo:之後要考量折讓行為
                            if (GlobalFn.isNullRet(e.Value,0m) <= 0)
                            {                                
                                WfShowErrorMsg("退庫量應大於0!");
                                return false;
                            }

                            //檢查可輸入數量
                            if (WfChkMea26Mea27(e.Row, detailModel) == false)
                                return false;
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "mib07"://入庫倉
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

                        case "mib09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.mib09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.mib09, BekTbModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "mib11"://託工單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["mib03"] = "";//主件料號
                                e.Row["mib04"] = "";//主件品名
                                e.Row["mib05"] = 0;//入庫量
                                e.Row["mib06"] = "";//單位
                                e.Row["mib09"] = "";//單價
                                WfSetDetailAmt(e.Row);
                                WfSetTotalAmt();
                                return true;
                            }
                            iChkCnts = detailList.Where(x => x.mib11 == e.Value.ToString())
                                               .Count();
                            if (iChkCnts >= 2)
                            {
                                WfShowErrorMsg("託工單號不可重覆!");
                                return false;

                            }

                            var meaModel = BoMan.OfGetMeaModel(e.Value.ToString());
                            if (meaModel == null)
                            {
                                WfShowErrorMsg("無此託工單號!");
                                return false;
                            }
                            if (meaModel.meaconf != "Y")
                            {
                                WfShowErrorMsg("單據非確認狀態!");
                                return false;
                            }

                            if (meaModel.mea03!=masterModel.mia03)
                            {
                                WfShowErrorMsg("廠商不同,請檢核!");
                                return false;
                            }

                            if ((meaModel.mea22 - (meaModel.mea26 - meaModel.mea27)) < 0)
                            {
                                WfShowErrorMsg("此託工單已無可入庫數量!");
                                return false;
                            }
                            e.Row["mib03"] = meaModel.mea20;//主件料號
                            e.Row["mib04"] = meaModel.mea21;//主件品名
                            e.Row["mib05"] = 0;//入庫量
                            e.Row["mib06"] = meaModel.mea23;//單位
                            e.Row["mib09"]=meaModel.mea13;//單價
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            
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
            vw_mant412 masterModel = null;
            vw_mant412s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {

                masterModel = DrMaster.ToItem<vw_mant412>();
                if (!GlobalFn.varIsNull(masterModel.mia01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.mia01, ""));
                #region 單頭資料檢查
                chkColName = "mia01";       //託工退貨單號
                chkControl = ute_mia01;
                if (GlobalFn.varIsNull(masterModel.mia01))
                {
                    this.uTab_Header.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mia02";       //入庫日期
                chkControl = udt_mia02;
                if (GlobalFn.varIsNull(masterModel.mia02))
                {
                    this.uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.mia02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "mia03";       //廠商編號
                chkControl = ute_mia03;
                if (GlobalFn.varIsNull(masterModel.mia03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mia04";       //人員
                chkControl = ute_mia04;
                if (GlobalFn.varIsNull(masterModel.mia04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mia05";       //部門
                chkControl = ute_mia05 ;
                if (GlobalFn.varIsNull(masterModel.mia05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mia06";       //課稅別
                chkControl = ucb_mia06;
                if (GlobalFn.varIsNull(masterModel.mia06))
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

                    detailModel = drTemp.ToItem<vw_mant412s>();
                    chkColName = "mib02";   //項次
                    if (GlobalFn.varIsNull(detailModel.mib02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    #region mib11 託工單號檢查
                    chkColName = "mib11";
                    if (GlobalFn.varIsNull(detailModel.mib11))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    var meaModel = BoMan.OfGetMeaModel(detailModel.mib11);
                    if (babModel == null)
                    {
                        WfShowErrorMsg("無此託工單號!");
                        return false;
                    }
                    if (meaModel.meaconf != "Y")
                    {
                        WfShowErrorMsg("單據非確認狀態!");
                        return false;
                    }
                    if (masterModel.mia03 != meaModel.mea03)
                    {
                        WfShowErrorMsg("廠商不同，請確認!");
                        return false;
                    }
                    if (masterModel.mia06 != meaModel.mea06)
                    {
                        WfShowErrorMsg("稅別不同，請確認!");
                        return false;
                    }
                    #endregion

                    chkColName = "mib07";   //倉庫別
                    if (GlobalFn.varIsNull(detailModel.mib07))
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
            string mia01New, errMsg;
            vw_mant412 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant412>();
                if (FormEditMode == YREditType.新增)
                {
                    mia01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.mia01, ModuleType.man, (DateTime)masterModel.mia02, out mia01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["mia01"] = mia01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["miasecu"] = LoginInfo.UserNo;
                        DrMaster["miasecg"] = LoginInfo.GroupNo;
                        DrMaster["miacreu"] = LoginInfo.UserNo;
                        DrMaster["miacreg"] = LoginInfo.DeptNo;
                        DrMaster["miacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["miamodu"] = LoginInfo.UserNo;
                        DrMaster["miamodg"] = LoginInfo.DeptNo;
                        DrMaster["miamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["mibcreu"] = LoginInfo.UserNo;
                            drDetail["mibcreg"] = LoginInfo.DeptNo;
                            drDetail["mibcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["mibmodu"] = LoginInfo.UserNo;
                            drDetail["mibmodg"] = LoginInfo.DeptNo;
                            drDetail["mibmodd"] = Now;
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


                //bt = new ButtonTool("Purr400");
                //adoModel = BoAdm.OfGetAdoModel("Purr400");
                //bt.SharedProps.Caption = adoModel.ado02;
                //bt.SharedProps.Category = "Report";
                //bt.Tag = adoModel;
                //buttonList.Add(bt);

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

                    //case "Purr400":
                    //    vw_purr400 l_vw_purr400;
                    //    vw_purt400 lMaster;
                    //    if (DrMaster == null)
                    //    {
                    //        WfShowBottomStatusMsg("未指定任何資料!");
                    //        return;
                    //    }
                    //    lMaster = DrMaster.ToItem<vw_purt400>();
                    //    l_vw_purr400 = new vw_purr400();
                    //    l_vw_purr400.pga01 = lMaster.pga01;
                    //    l_vw_purr400.pga03 = "";
                    //    l_vw_purr400.jump_yn = "N";
                    //    l_vw_purr400.order_by = "1";

                    //    FrmPurr400 rpt = new FrmPurr400(this.LoginInfo, l_vw_purr400, true, true);
                    //    rpt.WindowState = FormWindowState.Minimized;
                    //    rpt.ShowInTaskbar = false;
                    //    rpt.Show();

                    //    break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //*****************************表單自訂Fuction****************************************
        #region WfSetMia01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetMia01RelReadonly(string pMia01)
        {
            bab_tb babModel = null;
            string bab01;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_mia01 }, true);
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

        #region WfSetMia03Relation 設定廠商相關聯
        private void WfSetMia03Relation(string pMia03)
        {
            pca_tb pcaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mia01"], ""));
                pcaModel = BoPur.OfGetPcaModel(pMia03);
                if (pcaModel == null)
                    return;

                DrMaster["mia03_c"] = pcaModel.pca03;
                DrMaster["mia06"] = pcaModel.pca22;    //課稅別
                WfSetMia06Relation(pcaModel.pca22);
                DrMaster["mia09"] = pcaModel.pca23;    //發票聯數
                DrMaster["mia11"] = pcaModel.pca21;    //付款條件
                DrMaster["mia11_c"] = BoBas.OfGetBef03("1", pcaModel.pca21);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMia06Relation 設定稅別關聯
        private void WfSetMia06Relation(string pMia06)
        {
            try
            {
                if (pMia06 == "1")
                {
                    DrMaster["mia07"] = BaaTbModel.baa05;
                    DrMaster["mia08"] = "Y";
                }
                else if (pMia06 == "2")
                {
                    DrMaster["mia07"] = BaaTbModel.baa05;
                    DrMaster["mia08"] = "N";
                }
                else
                {
                    DrMaster["mia07"] = 0;
                    DrMaster["mia08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetDetailAmt 處理小計
        private bool WfSetDetailAmt(DataRow drMib)
        {
            mia_tb miaModel;
            mib_tb mibModel;
            decimal mib10t = 0, mib10 = 0;
            try
            {
                miaModel = DrMaster.ToItem<mia_tb>();
                mibModel = drMib.ToItem<mib_tb>();

                if (miaModel.mia08 == "Y")//稅內含
                {
                    mib10t = mibModel.mib09 * mibModel.mib05;
                    mib10t = GlobalFn.Round(mib10t, BekTbModel.bek04);
                    mib10 = mib10t / (1 + (miaModel.mia07 / 100));
                    mib10 = GlobalFn.Round(mib10, BekTbModel.bek04);
                }
                else//稅外加
                {
                    mib10 = mibModel.mib09 * mibModel.mib05;
                    mib10 = GlobalFn.Round(mib10, BekTbModel.bek04);
                    mib10t = mib10 * (1 + (miaModel.mia07 / 100));
                    mib10t = GlobalFn.Round(mib10t, BekTbModel.bek04);
                }
                drMib["mib10"] = mib10;
                drMib["mib10t"] = mib10t;

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
            mia_tb miaModel;
            decimal mia13 = 0, mia13t = 0, mia13g;
            try
            {
                miaModel = DrMaster.ToItem<mia_tb>();
                if (miaModel.mia08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (mib_tb mibModel in TabDetailList[0].DtSource.ToList<mib_tb>())
                    {
                        mia13t += mibModel.mib10t;
                    }
                    mia13t = GlobalFn.Round(mia13t, BekTbModel.bek04);
                    mia13 = mia13t / (1 + miaModel.mia07 / 100);
                    mia13 = GlobalFn.Round(mia13, BekTbModel.bek04);
                    mia13g = mia13t - mia13;

                }
                else//稅外加
                {
                    foreach (mib_tb mibModel in TabDetailList[0].DtSource.ToList<mib_tb>())
                    {
                        mia13 += mibModel.mib10;
                    }
                    mia13 = GlobalFn.Round(mia13, BekTbModel.bek04);
                    mia13g = mia13 * (miaModel.mia07 / 100);
                    mia13g = GlobalFn.Round(mia13g, BekTbModel.bek04);
                    mia13t = mia13 + mia13g;
                }

                DrMaster["mia13"] = mia13;
                DrMaster["mia13t"] = mia13t;
                DrMaster["mia13g"] = mia13g;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfResetAmtByMia06 依稅別更新單身及單頭的金額
        private void WfResetAmtByMia06()
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

        #region WfChkMea26Mea27 數量檢查--輸入數量是否大於託工單實際入庫量
        private bool WfChkMea26Mea27(DataRow pdr, vw_mant412s pListDetail)
        {
            List<vw_mant412s> detailList = null;
            mea_tb meaModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換送料單數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的送料單數量加總
            //decimal otherQtyTotal = 0;      //其他單據的數量加總
            decimal maxAvailableQty = 0;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                detailList = pdr.Table.ToList<vw_mant412s>();
                if (GlobalFn.varIsNull(pListDetail.mib02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.mib11))
                {
                    errMsg = "請先輸入託工單號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }


                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mia01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                meaModel = BoMan.OfGetMeaModel(pListDetail.mib11);
                if (babModel == null)
                {
                    errMsg = "Get meb_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //取得可輸入數量加總
                maxAvailableQty = meaModel.mea26 - meaModel.mea27;

                //先取本身這筆
                docThisQty = pListDetail.mib05;
                //再取其他筆加總
                docOtherQtyTotal = detailList.Where(p => p.mib02 != pListDetail.mib02)
                                   .Where(p => p.mib11 == pListDetail.mib11)
                                   .Sum(p => p.mib05);
                if ((maxAvailableQty - (docThisQty + docOtherQtyTotal)) < 0)
                {
                    errMsg = string.Format("項次{0} 最大可輸入數量為 {1}",
                                            pListDetail.mib02.ToString(),
                                            (maxAvailableQty - docOtherQtyTotal).ToString()
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
            mia_tb miaModel = null;
            mib_tb mibModel = null;

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

                miaModel = DrMaster.ToItem<mia_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                miaModel.miaconf = "Y";
                miaModel.miacond = Today;
                miaModel.miaconu = LoginInfo.UserNo;

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    mibModel = dr.ToItem<mib_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("2", mibModel.mib03, mibModel.mib07, mibModel.mib05, out errMsg) == false)//出庫
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔 
                    if (BoInv.OfStockPost("mant412", miaModel, mibModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }
                if (WfUpdMea27(true) == false)      //更新託工單退庫數量
                {
                    DrMaster.RejectChanges();
                    WfRollback();
                    return;
                }

                DrMaster["miaconf"] = "Y";
                DrMaster["miacond"] = Today;
                DrMaster["miaconu"] = LoginInfo.UserNo;
                DrMaster["miamodu"] = LoginInfo.UserNo;
                DrMaster["miamodg"] = LoginInfo.DeptNo;
                DrMaster["miamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                miaModel = DrMaster.ToItem<mia_tb>();
                WfSetDocPicture("", miaModel.miaconf, "", pbxDoc);
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
            vw_mant412 masterModel = null;
            vw_mant412s detailModel = null;
            List<vw_mant412s> detailList = null;
            List<mib_tb> mibList = null;    //檢查庫存
            decimal icc05;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant412>();
                if (masterModel.miaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    WfRollback();
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.mia02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_mant412s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_mant412s>();
                    if (WfChkMea26Mea27(drTemp, detailModel) == false)
                    {
                        WfRollback();
                        return false;
                    }
                }

                //檢查退回單的數量是否足夠
                mibList = TabDetailList[0].DtSource.ToList<mib_tb>();
                var sumMibList =                         //依料號及倉庫做加總
                        from mib in mibList
                        where mib.mib05 > 0
                        group mib by new { mib.mib03, mib.mib07 } into sumMibModel
                        select new
                        {
                            mhb03 = sumMibModel.Key.mib03,
                            mhb07 = sumMibModel.Key.mib07,
                            mhb05 = sumMibModel.Sum(p => p.mib05)
                        }
                    ;
                foreach (var sumMibModel in sumMibList)
                {
                    icc05 = BoInv.OfGetIcc05(sumMibModel.mhb03, sumMibModel.mhb07);
                    if (icc05 < sumMibModel.mhb05)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足扣量!", sumMibModel.mhb03, icc05));
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
            mia_tb miaModel = null;
            mib_tb mibModel = null;
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
                miaModel = DrMaster.ToItem<mia_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    mibModel = dr.ToItem<mib_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("1", mibModel.mib03, mibModel.mib07, mibModel.mib05, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                    //刪除庫存交易歷史檔
                    if (BoInv.OfDelIna(mibModel.mib01, mibModel.mib02, "2", out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                }
                if (WfUpdMea27(false) == false)
                {
                    DrMaster.RejectChanges();
                    WfRollback();
                    return;
                }

                DrMaster["miaconf"] = "N";
                DrMaster["miacond"] = DBNull.Value;
                DrMaster["miaconu"] = "";
                DrMaster["miamodu"] = LoginInfo.UserNo;
                DrMaster["miamodg"] = LoginInfo.DeptNo;
                DrMaster["miamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                miaModel = DrMaster.ToItem<mia_tb>();
                WfSetDocPicture("", miaModel.miaconf, "", pbxDoc);
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
            vw_mant412 masterModel = null;
            vw_mant412s detailModel = null;
            List<vw_mant412s> detailList = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            List<mib_tb> mibList = null;    //檢查庫存
            int iChkCnts = 0;
            decimal icc05;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant412>();
                if (masterModel.miaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                detailList = TabDetailList[0].DtSource.ToList<vw_mant412s>();

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
            vw_mant412 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_mant412>();

                if (masterModel.miaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.miaconf == "N")//走作廢
                {

                    DrMaster["miaconf"] = "X";
                    DrMaster["miaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.miaconf == "X")
                {
                    DrMaster["miaconf"] = "N";
                    DrMaster["miaconu"] = "";
                }

                DrMaster["miamodu"] = LoginInfo.UserNo;
                DrMaster["miamodg"] = LoginInfo.DeptNo;
                DrMaster["miamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_mant412>();
                WfSetDocPicture("", masterModel.miaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfUpdMea27 更新託工單退庫數量 確認/取消確認
        private bool WfUpdMea27(bool pbConfirm)
        {
            bab_tb babModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParms;
            DataTable dtMibDistinct = null;
            string mib11;
            decimal docQty = 0, otherDocQty = 0;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mia01"], ""));
                if (babModel == null)
                {
                    WfShowErrorMsg("Get bab_tb Error!");
                    return false;
                }
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT DISTINCT mib11,SUM(mib05) mib05");
                sbSql.AppendLine("FROM mib_tb");
                sbSql.AppendLine("WHERE mib01=@mib01");
                sbSql.AppendLine("  AND ISNULL(mib11,'')<>''");
                sbSql.AppendLine("GROUP BY mib11");
                sqlParms = new List<SqlParameter>();
                sqlParms.Add(new SqlParameter("@mib01", GlobalFn.isNullRet(DrMaster["mia01"], "")));
                dtMibDistinct = BoMan.OfGetDataTable(sbSql.ToString(), sqlParms.ToArray());
                if (dtMibDistinct == null)
                    return true;
                foreach (DataRow dr in dtMibDistinct.Rows)
                {
                    mib11 = dr["mib11"].ToString();
                    docQty = Convert.ToDecimal(dr["mib05"]);

                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT SUM(mib05) FROM mia_tb");
                    sbSql.AppendLine("  INNER JOIN mib_tb ON mia01=mib01");
                    sbSql.AppendLine("WHERE miaconf='Y'");
                    sbSql.AppendLine("AND mib11=@mib11 ");
                    sbSql.AppendLine("AND mia01<>@mia01");


                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@mib11", mib11));
                    sqlParms.Add(new SqlParameter("@mia01", GlobalFn.isNullRet(DrMaster["mia01"], "")));
                    otherDocQty = 0;
                    otherDocQty = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParms.ToArray()), 0);

                    sbSql = new StringBuilder();
                    sbSql = sbSql.AppendLine("UPDATE mea_tb");
                    sbSql = sbSql.AppendLine("SET mea27=@mea27");
                    sbSql = sbSql.AppendLine("WHERE mea01=@mea01");
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@mea01", mib11));
                    if (pbConfirm)  //確認的要加本身單據
                        sqlParms.Add(new SqlParameter("@mea27", docQty + otherDocQty));
                    else
                        sqlParms.Add(new SqlParameter("@mea27", otherDocQty));

                    if (BoPur.OfExecuteNonquery(sbSql.ToString(), sqlParms.ToArray()) != 1)
                    {
                        WfShowErrorMsg("更新託工單實際退庫數量失敗!");
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
