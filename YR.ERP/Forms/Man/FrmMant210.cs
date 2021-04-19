/* 程式名稱: 託工單維護作業
   系統代號: mant210
   作　　者: Allen
   描　　述: 
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;
using Infragistics.Win.UltraWinTree;
using YR.ERP.Shared;

namespace YR.ERP.Forms.Man
{
    public partial class FrmMant210 : YR.ERP.Base.Forms.FrmEntryMDBase
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
        public FrmMant210()
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
            this.StrFormID = "mant210";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("mea01", SqlDbType.NVarChar) });
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
                WfSetUcomboxDataSource(ucb_mea06, sourceList);

                //發票聯數
                sourceList = BoPur.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_mea09, sourceList);

                //單據狀態
                sourceList = BoMan.OfGetMeastatKVPList();
                WfSetUcomboxDataSource(ucb_meastat, sourceList);

                //單據確認
                sourceList = BoMan.OfGetMeaconfKVPList();
                WfSetUcomboxDataSource(ucb_meaconf, sourceList);
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
            this.TabDetailList[0].TargetTable = "meb_tb";
            this.TabDetailList[0].ViewTable = "vw_mant210s";
            keyParm = new SqlParameter("meb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "mea01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_mant210 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_mant210>();
                    WfSetDocPicture("", masterModel.meaconf, masterModel.meastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.mea10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.mea10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.mea10));
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

                    WfSetControlReadonly(new List<Control> { ute_meacreu, ute_meacreg, udt_meacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_meamodu, ute_meamodg, udt_meamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_measecu, ute_measecg }, true);

                    if (GlobalFn.isNullRet(masterModel.mea01, "") != "")
                        WfSetControlReadonly(ute_mea01, true);

                    //母製令有值時,料號不需輸入
                    if (GlobalFn.isNullRet(masterModel.mea24, "") != "")
                        WfSetControlReadonly(ute_mea20, true);

                    WfSetControlReadonly(new List<Control> { ute_mea01_c, ute_mea03_c, ute_mea04_c, ute_mea05_c, ute_mea11_c, ute_mea12_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_mea07, ucx_mea08, ute_mea10 }, true);
                    WfSetControlReadonly(new List<Control> { ute_mea21, ute_mea23 }, true);
                    WfSetControlReadonly(new List<Control> { udt_mea17, udt_mea18, }, true);
                    WfSetControlReadonly(new List<Control> { ute_mea14, ute_mea14t }, true);
                    WfSetControlReadonly(new List<Control> { ute_mea26, ute_mea27 }, true);
                    WfSetControlReadonly(new List<Control> { ucb_meaconf, udt_meacond, ute_meaconu, ucb_meastat }, true);
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_mea01, true);
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
                            if (columnName == "meb03" ||
                                columnName == "meb05" ||
                                columnName == "meb07" ||
                                columnName == "meb10"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "meb02")
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["mea00"] = "2";
                pDr["mea02"] = Today;
                pDr["mea04"] = LoginInfo.UserNo;
                pDr["mea04_c"] = LoginInfo.UserName;
                pDr["mea05"] = LoginInfo.DeptNo;
                pDr["mea05_c"] = LoginInfo.DeptName;
                pDr["mea06"] = "1"; //課稅別預設含稅
                WfSetMea06Relation("1");

                pDr["mea07"] = 0;
                pDr["mea08"] = "N";
                pDr["mea10"] = BaaTbModel.baa04;
                pDr["mea13"] = 0;
                pDr["mea14"] = 0;
                pDr["mea14t"] = 0;
                pDr["mea22"] = 0;
                pDr["mea26"] = 0;
                pDr["mea27"] = 0;
                pDr["meaconf"] = "N";
                pDr["meastat"] = "1";
                pDr["meacomp"] = LoginInfo.CompNo;
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
                        pDr["meb02"] = WfGetMaxSeq(pDr.Table, "meb02");
                        pDr["meb05"] = 0;
                        pDr["meb08"] = 0;
                        pDr["meb09"] = 0;
                        pDr["mebcomp"] = LoginInfo.CompNo;
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
            vw_mant210 masterModel;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_mant210>();
                if (masterModel.meaconf != "N")
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
            vw_mant210 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_mant210>();
                if (masterModel.meaconf != "N")
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
                vw_mant210 masterModel = null;
                vw_mant210s detailModel = null;

                #region 單頭-pick vw_mant210
                if (pDr.Table.Prefix.ToLower() == "vw_mant210")
                {
                    masterModel = DrMaster.ToItem<vw_mant210>();
                    switch (pColName.ToLower())
                    {
                        case "mea01"://託工單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "man"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "21"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mea03"://廠商編號
                            WfShowPickUtility("p_pca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mea04"://經辦人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mea05"://經辦部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "mea11"://付款條件
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

                        case "mea12"://取價條件
                            WfShowPickUtility("p_pbb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pbb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mea20"://主件料號
                            messageModel.StrWhereAppend = string.Format(" AND ica17 = 'S' ");
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "mea24"://母製令
                            if (FormEditMode == YREditType.修改)
                                messageModel.StrWhereAppend = string.Format(" AND mea01 <>'{0}' ", masterModel.mea01);

                            WfShowPickUtility("p_mea2", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["mea01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "mea25"://母製令項次                            
                            if (GlobalFn.varIsNull(masterModel.mea24))
                                return true;
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@meb01", masterModel.mea24));
                            messageModel.IsAutoQuery = true;
                            WfShowPickUtility("p_meb", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["meb02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mea28"://送貨地址碼
                            //messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@pcb01", masterModel.mea03));
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

                #region 單身-pick vw_mant210s
                if (pDr.Table.Prefix.ToLower() == "vw_mant210s")
                {
                    detailModel = pDr.ToItem<vw_mant210s>();
                    switch (pColName.ToLower())
                    {
                        case "meb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "meb07"://倉庫
                            if (GlobalFn.isNullRet(detailModel.meb03, "") == "")
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
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.meb03));
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_mant210 masterModel = null;
            vw_mant210s detailModel = null;
            List<vw_mant210s> detailList = null;
            bab_tb babModel = null;
            pfa_tb pfaModel = null;
            UltraGrid uGrid = null;
            int ChkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant210>();
                if (e.Column.ToLower() != "mea01" && GlobalFn.isNullRet(DrMaster["mea01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false;
                }
                #region 單頭- vw_mant210
                if (e.Row.Table.Prefix.ToLower() == "vw_mant210")
                {
                    switch (e.Column.ToLower())
                    {
                        case "mea01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "man", "21") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["mea01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetMea01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "mea03"://廠商
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mea03_c"] = "";
                                e.Row["mea28"] = "";    //送貨地址碼
                                return true;
                            }
                            if (BoPur.OfChkPcaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此廠商資料,請檢核!");
                                return false;
                            }
                            WfSetMea03Relation(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mea04"://經辦人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mea04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["mea04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mea05"://經辦部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mea05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["mea05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mea06"://課稅別
                            WfSetMea06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfSetSubAmt(e.Row);
                            break;

                        case "mea11"://付款條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mea11_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("1", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此付款條件,請檢核!");
                                return false;
                            }
                            e.Row["mea11_c"] = BoBas.OfGetBef03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mea12"://取價條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mea12_c"] = "";
                                return true;
                            }
                            if (BoPur.OfChkPbbPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此取價條件,請檢核!");
                                return false;
                            }
                            e.Row["mea12_c"] = BoPur.OfGetPbb02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mea13"://單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (masterModel.mea13 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(masterModel.mea13, BekTbModel.bek03);
                            WfSetSubAmt(e.Row);
                            break;

                        case "mea20"://主件料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["mea21"] = "";//品名
                                e.Row["mea22"] = 0;//入庫數量
                                e.Row["mea23"] = "";//單位
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }
                            var icaModel = BoInv.OfGetIcaModel(e.Value.ToString());
                            if (icaModel.ica17 != "S")
                            {
                                WfShowErrorMsg("主件料號僅能輸入托外加工件!");
                                return false;
                            }
                            if (!GlobalFn.varIsNull(masterModel.mea24))
                            {
                                var mea24MebList = BoMan.OfGetMebList(masterModel.mea24);
                                if (mea24MebList == null)
                                {
                                    WfShowErrorMsg("母製令中無料件!");
                                    return false;
                                }
                                iChkCnts = mea24MebList.Where(p => p.meb03 == masterModel.mea20).Count();
                                if (iChkCnts == 0)
                                {
                                    WfShowErrorMsg("母製令的子件料件中,無此主料件!");
                                    return false;
                                }
                            }

                            if (WfSetMea20Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;
                            break;
                        case "mea24"://母製令
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                DrMaster["mea25"] = DBNull.Value;
                                DrMaster["mea20"] = "";
                                DrMaster["mea21"] = "";
                                WfSetControlReadonly(ute_mea20, false);
                                return true;
                            }
                            if (WfChkMea24(masterModel.mea01, e.Value.ToString()) == false)
                                return false;
                            WfSetControlReadonly(ute_mea20, true);
                            break;
                        case "mea25"://母製令項次
                            if (GlobalFn.varIsNull(masterModel.mea24))
                            {
                                WfShowErrorMsg("請先輸入母製令");
                                return false;
                            }
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                DrMaster["mea20"] = "";
                                DrMaster["mea21"] = "";
                                return true;
                            }
                            if (WfChkMea25(masterModel.mea24, Convert.ToInt32(e.Value)) == false)
                                return false;
                            WfSetMea24Mea25Relation(masterModel.mea24, Convert.ToInt32(e.Value));
                            break;

                        case "mea28"://送貨地址碼
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(masterModel.mea03, "") == "")
                            {
                                WfShowErrorMsg("請先輸入廠商編號!");
                                return false;
                            }
                            if (BoPur.OfChkPcbPKExists(masterModel.mea03, e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此廠商地址碼,請檢核!");
                                return false;
                            }
                            break;
                    }
                }
                #endregion

                #region 單身- vw_mant210s
                if (e.Row.Table.Prefix.ToLower() == "vw_mant210s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_mant210s>();
                    detailList = e.Row.Table.ToList<vw_mant210s>();
                    babModel = BoBas.OfGetBabModel(masterModel.mea01);

                    switch (e.Column.ToLower())
                    {
                        case "meb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.meb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "meb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["meb04"] = "";//品名
                                e.Row["meb05"] = 0;//入庫數量
                                e.Row["meb06"] = "";//單位
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }

                            if (WfSetMeb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;
                            break;
                        case "meb05"://預計用量
                            if (GlobalFn.varIsNull(detailModel.meb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["meb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detailModel.meb06))
                            {
                                WfShowErrorMsg("請先輸入單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["meb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入預計用量!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["meb05"]);
                                return false;
                            }
                            break;
                        //case "meb06"://單位
                        //    if (GlobalFn.varIsNull(e.Value))
                        //    {
                        //        return true;
                        //    }

                        //    if (GlobalFn.varIsNull(detailModel.meb03))
                        //    {
                        //        WfShowErrorMsg("請先輸入料號!");
                        //        WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["meb03"]);
                        //        return false;
                        //    }
                        //    if (WfChkPgb06(e.Row, e.Value.ToString(), babModel.bab08) == false)
                        //        return false;

                        //    if (WfSetPgb06Relation(e.Row, e.Value.ToString(), babModel.bab08) == false)
                        //        return false;
                        //    break;

                        case "meb07"://倉庫
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_mant210 masterModel = null;
            vw_mant210s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {

                masterModel = DrMaster.ToItem<vw_mant210>();
                if (!GlobalFn.varIsNull(masterModel.mea01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.mea01, ""));
                #region 單頭資料檢查
                chkColName = "mea01";       //託工單號
                chkControl = ute_mea01;
                if (GlobalFn.varIsNull(masterModel.mea01))
                {
                    this.uTab_Header.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mea02";       //開單日期
                chkControl = udt_mea02;
                if (GlobalFn.varIsNull(masterModel.mea02))
                {
                    this.uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mea03";       //廠商編號
                chkControl = ute_mea03;
                if (GlobalFn.varIsNull(masterModel.mea03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mea04";       //人員
                chkControl = ute_mea04;
                if (GlobalFn.varIsNull(masterModel.mea04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mea05";       //部門
                chkControl = ute_mea05;
                if (GlobalFn.varIsNull(masterModel.mea05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mea06";       //課稅別
                chkControl = ucb_mea06;
                if (GlobalFn.varIsNull(masterModel.mea06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                //取價條件
                chkColName = "mea12";
                chkControl = ute_mea12;
                if (GlobalFn.varIsNull(masterModel.mea12))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                //預計開工日
                chkColName = "mea15";
                chkControl = udt_mea15;
                if (GlobalFn.varIsNull(masterModel.mea15))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                //預計完工日
                chkColName = "mea16";
                chkControl = udt_mea16;
                if (GlobalFn.varIsNull(masterModel.mea16))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                if (masterModel.mea16 < masterModel.mea15)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = "預計完工日不可小於預計開工日";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;

                }

                //預計生產數量
                chkColName = "mea22";
                chkControl = ute_mea22;
                if (GlobalFn.varIsNull(masterModel.mea22))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                if (masterModel.mea22 <= 0)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "應大於0";
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

                    detailModel = drTemp.ToItem<vw_mant210s>();
                    chkColName = "meb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.meb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "meb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.meb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "meb05";   //預計用量
                    #region pfb05 預計用量
                    if (GlobalFn.varIsNull(detailModel.meb05) || detailModel.meb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "meb07";   //倉庫別
                    if (GlobalFn.varIsNull(detailModel.meb07))
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
            string mea01New, errMsg;
            vw_mant210 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant210>();
                if (FormEditMode == YREditType.新增)
                {
                    mea01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.mea01, ModuleType.man, (DateTime)masterModel.mea02, out mea01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["mea01"] = mea01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["measecu"] = LoginInfo.UserNo;
                        DrMaster["measecg"] = LoginInfo.GroupNo;
                        DrMaster["meacreu"] = LoginInfo.UserNo;
                        DrMaster["meacreg"] = LoginInfo.DeptNo;
                        DrMaster["meacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["meamodu"] = LoginInfo.UserNo;
                        DrMaster["meamodg"] = LoginInfo.DeptNo;
                        DrMaster["meamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["mebcreu"] = LoginInfo.UserNo;
                            drDetail["mebcreg"] = LoginInfo.DeptNo;
                            drDetail["mebcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["mebmodu"] = LoginInfo.UserNo;
                            drDetail["mebmodg"] = LoginInfo.DeptNo;
                            drDetail["mebmodd"] = Now;
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

                bt = new ButtonTool("ProducedExpand");
                bt.SharedProps.Caption = "展開";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("Manr210");
                adoModel = BoAdm.OfGetAdoModel("manr210");
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

                    case "ProducedExpand":
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfProducedExpand();
                        break;

                    case "Manr210":
                        vw_manr210 queryModel;
                        vw_mant210 masterModel;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_mant210>();
                        queryModel = new vw_manr210();
                        queryModel.mea01 = masterModel.mea01;
                        queryModel.mea03 = "";
                        queryModel.jump_yn = "N";
                        queryModel.order_by = "1";

                        FrmManr210 rpt = new FrmManr210(this.LoginInfo, queryModel, true, true);
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
        #region WfSetMea01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetMea01RelReadonly(string pMea01)
        {
            bab_tb babModel = null;
            string bab01;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_mea01 }, true);
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

        #region WfSetMea03Relation 設定廠商相關聯
        private void WfSetMea03Relation(string pMea03)
        {
            pca_tb pcaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mea01"], ""));
                pcaModel = BoPur.OfGetPcaModel(pMea03);
                if (pcaModel == null)
                    return;

                DrMaster["mea03_c"] = pcaModel.pca03;
                DrMaster["mea06"] = pcaModel.pca22;    //課稅別
                WfSetMea06Relation(pcaModel.pca22);
                DrMaster["mea09"] = pcaModel.pca23;    //發票聯數
                DrMaster["mea11"] = pcaModel.pca21;    //付款條件
                DrMaster["mea11_c"] = BoBas.OfGetBef03("1", pcaModel.pca21);
                DrMaster["mea12"] = pcaModel.pca24;    //取價條件
                DrMaster["mea12_c"] = BoPur.OfGetPbb02(pcaModel.pca24);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMea20Relation 設定主件料號關聯
        private bool WfSetMea20Relation(string pMea20, DataRow pDr)
        {
            ica_tb icaModel;
            decimal peb08;
            try
            {
                icaModel = BoInv.OfGetIcaModel(pMea20);
                peb08 = 0;
                if (icaModel == null)
                {
                    pDr["mea21"] = "";//品名
                    pDr["mea22"] = 0;//預計用量
                    pDr["mea23"] = "";//單位
                }
                else
                {
                    //if (BoInv.OfGetUnitCovert(pMeb03, icaModel.ica08, icaModel.ica07, out peb08) == false)
                    //{
                    //    WfShowErrorMsg("未設定料號轉換,請先設定!");
                    //    return false;
                    //}
                    pDr["mea21"] = icaModel.ica02;//品名
                    pDr["mea22"] = 0;//入庫數量
                    pDr["mea23"] = icaModel.ica07;////單位-預設庫存單位
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMea06Relation 設定稅別關聯
        private void WfSetMea06Relation(string pMea06)
        {
            try
            {
                if (pMea06 == "1")
                {
                    DrMaster["mea07"] = BaaTbModel.baa05;
                    DrMaster["mea08"] = "Y";
                }
                else if (pMea06 == "2")
                {
                    DrMaster["mea07"] = BaaTbModel.baa05;
                    DrMaster["mea08"] = "N";
                }
                else
                {
                    DrMaster["mea07"] = 0;
                    DrMaster["mea08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMeb03Relation 設定子件料號關聯
        private bool WfSetMeb03Relation(string pMeb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal peb08;
            try
            {
                icaModel = BoInv.OfGetIcaModel(pMeb03);
                peb08 = 0;
                if (icaModel == null)
                {
                    pDr["meb04"] = "";//品名
                    pDr["meb05"] = 0;//預計用量
                    pDr["meb06"] = "";//單位
                }
                else
                {
                    //if (BoInv.OfGetUnitCovert(pMeb03, icaModel.ica08, icaModel.ica07, out peb08) == false)
                    //{
                    //    WfShowErrorMsg("未設定料號轉換,請先設定!");
                    //    return false;
                    //}
                    pDr["meb04"] = icaModel.ica02;//品名
                    pDr["meb05"] = 0;//預計用量
                    pDr["meb06"] = icaModel.ica07;////單位-預設庫存單位
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkMea24 母製令相關檢查
        private bool WfChkMea24(string mea01, string mea24)
        {
            mea_tb meaParetnsModel = null;
            try
            {
                meaParetnsModel = BoMan.OfGetMeaModel(mea24);
                if (meaParetnsModel == null)
                {
                    WfShowErrorMsg("無此母製令單號,請確認!");
                    return false;
                }
                if (meaParetnsModel.meaconf != "Y")
                {
                    WfShowErrorMsg("母製令未確認,請檢核!");
                    return false;
                }
                if (meaParetnsModel.mea00 != "2")
                {
                    WfShowErrorMsg("僅可挑選託外生產製令!");
                    return false;
                }
                if (FormEditMode == YREditType.修改 && mea01 == mea24)
                {
                    WfShowErrorMsg("母製令不可與託工單號相同!");
                    return false;
                }
                if (!GlobalFn.varIsNull(meaParetnsModel.mea24))
                {
                    if (mea24 == meaParetnsModel.mea24)
                    {
                        WfShowErrorMsg("母製令與上層母製令產生遁環參考,請確認!");
                        return false;
                    }
                    if (WfChkMea24Recursive(mea24, meaParetnsModel.mea24) == false)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查詢母製令是否遞迴,由下往上處理
        /// </summary>
        /// <param name="mea24Leave"></param>
        /// <param name="mea24Parents"></param>
        /// <returns></returns>
        private bool WfChkMea24Recursive(string mea24Leave, string mea24Parents)
        {
            mea_tb meaParentsModel = null;
            try
            {
                meaParentsModel = BoMan.OfGetMeaModel(mea24Parents);
                if (meaParentsModel == null || GlobalFn.varIsNull(meaParentsModel.mea24))
                    return true;
                if (meaParentsModel.mea24 == mea24Leave)
                {
                    WfShowErrorMsg("母製令與上層母製令產生遁環參考,請確認");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region WfSetMea24Mea25Relation 設定母製令相關資料
        private bool WfSetMea24Mea25Relation(string mea24, decimal mea25)
        {
            meb_tb mebModel;

            try
            {
                mebModel = BoMan.OfGetMebModel(mea24, Convert.ToInt32(mea25));
                if (mebModel == null)
                    return false;
                DrMaster["mea20"] = mebModel.meb03;
                DrMaster["mea21"] = mebModel.meb04;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkMea25 母製令項次檢查
        private bool WfChkMea25(string mea24, decimal mea25)
        {
            meb_tb mebModel;
            try
            {
                mebModel = BoMan.OfGetMebModel(mea24, Convert.ToInt32(mea25));
                if (mebModel == null)
                {
                    WfShowErrorMsg("查無此託工單項次,請確認!");
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
            mea_tb meaModel = null;
            meb_tb mebModel = null;

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

                meaModel = DrMaster.ToItem<mea_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                meaModel.meaconf = "Y";
                meaModel.meacond = Today;
                meaModel.meaconu = LoginInfo.UserNo;
                meaModel.meastat = "2";


                DrMaster["meaconf"] = "Y";
                DrMaster["meastat"] = "2";
                DrMaster["meacond"] = Today;
                DrMaster["meaconu"] = LoginInfo.UserNo;
                DrMaster["meamodu"] = LoginInfo.UserNo;
                DrMaster["meamodg"] = LoginInfo.DeptNo;
                DrMaster["meamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                meaModel = DrMaster.ToItem<mea_tb>();
                WfSetDocPicture("", meaModel.meaconf, meaModel.meastat, pbxDoc);
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
            vw_mant210 masterModel = null;
            vw_mant210s detailModel = null;
            List<vw_mant210s> detailList = null;
            int chkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant210>();
                if (masterModel.meaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認!");
                    WfRollback();
                    return false;
                }
                if (masterModel.meastat != "1")
                {
                    WfShowErrorMsg("單據非製單中!");
                    WfRollback();
                    return false;
                }

                //有母製令時，需檢查是否已確認
                if (!GlobalFn.varIsNull(masterModel.mea24))
                {
                    var meaParents = BoMan.OfGetMeaModel(masterModel.mea24);
                    if (meaParents==null)
                    {
                        WfShowErrorMsg("查無母製令!");
                        WfRollback();
                        return false;
                    }
                    if (meaParents.meaconf!="Y")
                    {
                        WfShowErrorMsg("母製令未確認!");
                        WfRollback();
                        return false;
                    }
                }

                //有母製令項次時，需檢查是否存在
                if (!GlobalFn.varIsNull(masterModel.mea25))
                {
                    var mebParents = BoMan.OfGetMebModel(masterModel.mea24, Convert.ToInt16(masterModel.mea25));
                    if (mebParents == null)
                    {
                        WfShowErrorMsg("查無此母製令項次!");
                        WfRollback();
                        return false;
                    }
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_mant210s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_mant210s>();
                    //if (WfChkPgb05(drTemp, detailModel) == false)
                    //{
                    //    WfRollback();
                    //    return false;
                    //}
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
            mea_tb meaModel = null;
            meb_tb mebModel = null;
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
                meaModel = DrMaster.ToItem<mea_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                //foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                //{
                //    mebModel = dr.ToItem<meb_tb>();
                //}

                DrMaster["meaconf"] = "N";
                DrMaster["meastat"] = "1";
                DrMaster["meacond"] = DBNull.Value;
                DrMaster["meaconu"] = "";
                DrMaster["meamodu"] = LoginInfo.UserNo;
                DrMaster["meamodg"] = LoginInfo.DeptNo;
                DrMaster["meamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                meaModel = DrMaster.ToItem<mea_tb>();
                WfSetDocPicture("", meaModel.meaconf, meaModel.meastat, pbxDoc);
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
            vw_mant210 masterModel = null;
            List<meb_tb> mebList = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant210>();
                if (masterModel.meaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認!");
                    return false;
                }

                if (masterModel.meastat != "2")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }

                //檢查是否有送料單
                sbSql = new StringBuilder();
                sbSql.AppendLine(string.Format("SELECT COUNT(1) FROM mfa_tb"));
                sbSql.AppendLine(string.Format("WHERE mfa06=@mfa06"));
                sbSql.AppendLine(string.Format("AND mfaconf <>'X' "));
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@mfa06", masterModel.mea01));
                iChkCnts = GlobalFn.isNullRet(BoMan.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (iChkCnts > 0)
                {
                    WfShowErrorMsg("已有送料單，不可取消確認!");
                    return false;
                }
                //檢查是否有入庫單
                sbSql = new StringBuilder();
                sbSql.AppendLine(string.Format("SELECT COUNT(1) FROM mha_tb"));
                sbSql.AppendLine(string.Format("    INNER JOIN mhb_tb ON mha01=mhb01"));
                sbSql.AppendLine(string.Format("WHERE mhb11=@mhb11"));
                sbSql.AppendLine(string.Format("AND mhaconf <>'X' "));
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@mhb11", masterModel.mea01));
                iChkCnts = GlobalFn.isNullRet(BoMan.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (iChkCnts > 0)
                {
                    WfShowErrorMsg("已有入庫單，不可取消確認!");
                    return false;
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
            vw_mant210 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_mant210>();

                if (masterModel.meaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.meaconf == "N")//走作廢
                {

                    DrMaster["meaconf"] = "X";
                    DrMaster["meaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.meaconf == "X")
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
                    DrMaster["meaconf"] = "N";
                    DrMaster["meaconu"] = "";
                }

                DrMaster["meamodu"] = LoginInfo.UserNo;
                DrMaster["meamodg"] = LoginInfo.DeptNo;
                DrMaster["meamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_mant210>();
                WfSetDocPicture("", masterModel.meaconf, masterModel.meastat, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfSetSubAmt 處理小計
        private bool WfSetSubAmt(DataRow drMea)
        {
            mea_tb meaModel;
            decimal mea14t = 0, mea14 = 0;
            try
            {
                meaModel = DrMaster.ToItem<mea_tb>();
                if (meaModel.mea08 == "Y")//稅內含
                {
                    mea14t = meaModel.mea13 * meaModel.mea22;
                    mea14t = GlobalFn.Round(mea14t, BekTbModel.bek04);
                    mea14 = mea14t / (1 + (meaModel.mea07 / 100));
                    mea14 = GlobalFn.Round(mea14, BekTbModel.bek04);
                }
                else//稅外加
                {
                    mea14 = meaModel.mea13 * meaModel.mea22;
                    mea14 = GlobalFn.Round(mea14, BekTbModel.bek04);
                    mea14t = mea14 * (1 + (meaModel.mea07 / 100));
                    mea14t = GlobalFn.Round(mea14t, BekTbModel.bek04);
                }
                drMea["mea14"] = mea14;
                drMea["mea14t"] = mea14t;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfProducedExpand
        private void WfProducedExpand()
        {
            mea_tb meaModel = null;
            meb_tb mebModel = null;
            StringBuilder sbSql = null;
            List<SqlParameter> sqlParmList = null;
            DataTable dtMant2101 = null;
            DataSet ds = null;
            int chkChildCnts = 0;
            string rootMea01;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;

                meaModel = DrMaster.ToItem<mea_tb>();
                if (meaModel.meaconf != "Y")
                    return;
                //新增一個空的資料表
                dtMant2101 = BoMan.OfGetDataTable("SELECT * FROM vw_mant210_1");
                dtMant2101.TableName = "mant210_1";

                //檢查本身是否為母製令
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM mea_tb");
                sbSql.AppendLine("WHERE mea24=@mea24");
                sbSql.AppendLine("AND meaconf='Y'");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@mea24", meaModel.mea01));
                chkChildCnts = GlobalFn.isNullRet(BoMan.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (GlobalFn.varIsNull(meaModel.mea24) && chkChildCnts == 0)
                    return;
                //取得最上層節點
                if (!GlobalFn.varIsNull(meaModel.mea24))
                {
                    rootMea01 = meaModel.mea24;
                    WfRecursiveUp(meaModel, meaModel.mea24);
                    //重新抓取meaModel
                    meaModel = BoMan.OfGetMeaModel(rootMea01);
                }
                else
                    rootMea01 = meaModel.mea01;
                
                //新增節點及DataRow
                UltraTreeNode utnRoot = new UltraTreeNode(rootMea01, meaModel.mea21);
                DataRow drNew = dtMant2101.NewRow();
                drNew["level"] = 0;
                drNew["mea01"] = meaModel.mea01;
                drNew["keySelf"] = meaModel.mea01;
                drNew["keyParents"] = "";
                drNew["mea20"] = meaModel.mea20;
                drNew["mea21"] = meaModel.mea21;
                drNew["mea22"] = meaModel.mea22;
                drNew["mea23"] = meaModel.mea23;
                drNew["mea26"] = meaModel.mea26;
                drNew["mea27"] = meaModel.mea27;
                dtMant2101.Rows.Add(drNew);

                WfLoadTree(rootMea01, meaModel.mea01, 0, utnRoot, dtMant2101);
                ds = dtMant2101.DataSet;
                ds.Relations.Add("mant210_1", ds.Tables["mant210_1"].Columns["keySelf"], ds.Tables["mant210_1"].Columns["keyParents"], false);

                WfShowForm("mant210_1", false, new object[] { this.LoginInfo, ds, utnRoot });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfRecursiveUp 向上找父節點
        private void WfRecursiveUp(mea_tb meaChildModel, string rootMea01)
        {
            StringBuilder sbSql = null;
            List<SqlParameter> sqlParmList = null;
            string mea24 = "";
            DataRow drMeaParents;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM mea_tb");
                sbSql.AppendLine("WHERE mea01=@mea01");
                sbSql.AppendLine("AND meaconf='Y'");
                //sbSql.AppendLine("AND ISNULL(mea24,'')<>'' ");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@mea01", rootMea01));
                drMeaParents = BoMan.OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                if (drMeaParents != null)
                {
                    var meaParentsModel = drMeaParents.ToItem<mea_tb>();
                    if (!GlobalFn.varIsNull(meaParentsModel.mea24))
                    {
                        rootMea01 = meaParentsModel.mea24;
                        WfRecursiveUp(meaParentsModel, rootMea01);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfloadTree
        private void WfLoadTree(string pMea01, string pKeyPanents, int pLevel, UltraTreeNode pUltraTreeNode, DataTable pDtMant2101)
        {
            StringBuilder sbSql = null;
            List<SqlParameter> sqlParmList = null;
            try
            {
                //取託工單明細資料
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM meb_tb");
                sbSql.AppendLine("LEFT JOIN mea_tb ON meb01=mea24 AND meb02=mea25");
                sbSql.AppendLine("WHERE meb01=@meb01");
                sbSql.AppendLine("ORDER BY meb02");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@meb01", pMea01));
                var dtTemp = BoMan.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                if (dtTemp != null && dtTemp.Rows.Count > 0)
                {
                    foreach (DataRow drDetail in dtTemp.Rows)
                    {
                        var childNode = new UltraTreeNode();
                        //var mebChildModel = drMeb.ToItem<meb_tb>();
                        //childNode.Key = mebChildModel.meb01;
                        //childNode.Text = mebChildModel.meb04;
                        childNode.Key = drDetail["meb01"].ToString();
                        childNode.Text = drDetail["meb04"].ToString();
                        //新增節點及資料列
                        pUltraTreeNode.Nodes.Add(childNode);
                        DataRow drNew = pDtMant2101.NewRow();
                        drNew["level"] = pLevel + 1;
                        drNew["mea01"] = drDetail["mea01"];//有可能是null
                        drNew["keyParents"] = pKeyPanents;
                        drNew["keySelf"] = drDetail["meb01"].ToString() + drDetail["meb02"].ToString();
                        drNew["mea20"] = drDetail["meb03"];
                        drNew["mea21"] = drDetail["meb04"];
                        drNew["mea22"] = drDetail["mea22"];
                        drNew["mea23"] = drDetail["meb06"];
                        drNew["mea26"] = drDetail["mea26"];
                        drNew["mea27"] = drDetail["mea27"];
                        pDtMant2101.Rows.Add(drNew);

                        sbSql = new StringBuilder();
                        sbSql.AppendLine("SELECT * FROM mea_tb");
                        sbSql.AppendLine("WHERE mea24=@mea24");
                        sbSql.AppendLine("AND mea25=@mea25");
                        sbSql.AppendLine("AND meaconf='Y'");
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@mea24", drDetail["meb01"]));
                        sqlParmList.Add(new SqlParameter("@mea25", drDetail["meb02"]));
                        var dtMea = BoMan.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                        if (dtMea != null && dtMea.Rows.Count > 0)
                        {
                            foreach (DataRow drMea in dtMea.Rows)
                            {
                                WfLoadTree(GlobalFn.isNullRet(drMea["mea01"], ""),
                                    drDetail["meb01"].ToString() + drDetail["meb02"].ToString(),
                                    pLevel + 1,
                                    childNode, pDtMant2101);
                            }
                        }
                    }
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
