/* 程式名稱: 託工退料單維護作業
   系統代號: mant312
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
    public partial class FrmMant312 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;
        ManBLL BoMan = null;
        PurBLL BoPur = null;

        baa_tb BaaTbModel = null;
        bek_tb BekTbModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmMant312()
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
            this.StrFormID = "mant312";

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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("mga01", SqlDbType.NVarChar) });
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
                WfSetUcomboxDataSource(ucb_mgaconf, sourceList);
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
            this.TabDetailList[0].TargetTable = "mgb_tb";
            this.TabDetailList[0].ViewTable = "vw_mant312s";
            keyParm = new SqlParameter("mgb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "mga01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_mant312 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_mant312>();
                    WfSetDocPicture("", masterModel.mgaconf, "", pbxDoc);
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

                    WfSetControlReadonly(new List<Control> { ute_mgacreu, ute_mgacreg, udt_mgacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_mgamodu, ute_mgamodg, udt_mgamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_mgasecu, ute_mgasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.mga01, "") != "")
                        WfSetControlReadonly(ute_mga01, true);

                    WfSetControlReadonly(new List<Control> { ute_mga01_c, ute_mga03, ute_mga03_c, ute_mga04_c, ute_mga05_c }, true);
                    WfSetControlReadonly(new List<Control> { ucb_mgaconf, udt_mgacond, ute_mgaconu }, true);
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_mga01, true);
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
                            if (columnName == "mgb05" ||
                                columnName == "mgb07" ||
                                columnName == "mgb09" ||
                                columnName == "mgb11"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "mgb02")
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
                pDr["mga00"] = "2";
                pDr["mga02"] = Today;
                pDr["mga04"] = LoginInfo.UserNo;
                pDr["mga04_c"] = LoginInfo.UserName;
                pDr["mga05"] = LoginInfo.DeptNo;
                pDr["mga05_c"] = LoginInfo.DeptName;

                pDr["mgaconf"] = "N";
                pDr["mgacomp"] = LoginInfo.CompNo;
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
                        pDr["mgb02"] = WfGetMaxSeq(pDr.Table, "mgb02");
                        pDr["mgb05"] = 0;
                        //來源項次目前均預帶單頭的資料
                        pDr["mgb10"] = DrMaster["mga06"];
                        pDr["mgbcomp"] = LoginInfo.CompNo;
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
            vw_mant312 masterModel;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_mant312>();
                if (masterModel.mgaconf != "N")
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
            vw_mant312 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_mant312>();
                if (masterModel.mgaconf != "N")
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
                vw_mant312 masterModel = null;
                vw_mant312s detailModel = null;

                #region 單頭-pick vw_mant312
                if (pDr.Table.Prefix.ToLower() == "vw_mant312")
                {
                    masterModel = DrMaster.ToItem<vw_mant312>();
                    switch (pColName.ToLower())
                    {
                        case "mga01"://退料單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "man"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "23"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mga04"://經辦人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mga05"://經辦部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mga06"://來源託工單挑選
                            WfShowPickUtility("p_mea2", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["mea01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mga07"://送貨地址碼
                            //messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@pcb01", masterModel.mga03));
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
                if (pDr.Table.Prefix.ToLower() == "vw_mant312s")
                {
                    detailModel = pDr.ToItem<vw_mant312s>();
                    switch (pColName.ToLower())
                    {
                        case "mgb07"://出庫倉
                            if (GlobalFn.isNullRet(detailModel.mgb03, "") == "")
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
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.mgb03));
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

                        case "mgb08"://入庫倉
                            if (GlobalFn.isNullRet(detailModel.mgb03, "") == "")
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
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.mgb03));
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
                        case "mgb11"://來源項次
                            if (GlobalFn.varIsNull(detailModel.mgb10))
                            {
                                WfShowErrorMsg("來源單號未輸入,請確認!");
                                return false;
                            }
                            messageModel.StrWhereAppend = string.Format(" AND (meb08-meb09)>0 ");//託工單已送料量減耗用量應大於0方可挑選
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@meb01", detailModel.mgb10));
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
            vw_mant312 masterModel = null;
            vw_mant312s detailModel = null;
            List<vw_mant312s> detailList = null;
            bab_tb babModel = null;
            UltraGrid uGrid = null;
            int ChkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant312>();
                if (e.Column.ToLower() != "mga01" && GlobalFn.isNullRet(DrMaster["mga01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false; ;
                }
                #region 單頭- vw_mant312
                if (e.Row.Table.Prefix.ToLower() == "vw_mant312")
                {
                    switch (e.Column.ToLower())
                    {
                        case "mga01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "man", "23") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["mga01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            //WfSetMfa01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mga02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            var result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;

                        case "mga04"://經辦人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mga04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["mga04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mga05"://經辦部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mga05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["mga05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mga06":
                            #region mga06 託工單號
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                DrMaster["mga03"] = "";
                                DrMaster["mga03_c"] = "";
                                DrMaster["mga07"] = "";
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
                                DrMaster["mga03"] = pcaModel.pca01;
                                DrMaster["mga03_c"] = pcaModel.pca03;
                                DrMaster["mga07"] = "";
                            }
                            break;
                            #endregion

                        case "mga07"://送貨地址碼
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(masterModel.mga03, "") == "")
                            {
                                WfShowErrorMsg("請先輸入廠商編號!");
                                return false;
                            }
                            if (BoPur.OfChkPcbPKExists(masterModel.mga03, e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此廠商地址碼,請檢核!");
                                return false;
                            }
                            break;
                    }
                }
                #endregion

                #region 單身- vw_mant312s
                if (e.Row.Table.Prefix.ToLower() == "vw_mant312s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_mant312s>();
                    detailList = e.Row.Table.ToList<vw_mant312s>();
                    babModel = BoBas.OfGetBabModel(masterModel.mga01);

                    switch (e.Column.ToLower())
                    {
                        case "mgb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.mgb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;
                        case "mgb05"://數量
                            if (GlobalFn.varIsNull(detailModel.mgb03))
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
                            //if (WfChkMeb05(e.Row, detailModel) == false)
                            //    return false;
                            break;

                        case "mgb07"://出庫倉
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoInv.OfChkIcbPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此倉庫別,請確認!");
                                return false;
                            }
                            //if (!GlobalFn.varIsNull(detailModel.mgb08) && e.Value.ToString() == detailModel.mgb08)
                            //{
                            //    WfShowErrorMsg("出庫倉與入庫倉不可相同,請確認!");
                            //    return false;
                            //}

                            break;
                        case "mgb11"://來源項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.mgb11, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("來源項次已存在,請檢核!");
                                e.Row["mgb03"] = "";//料號
                                e.Row["mgb04"] = "";//品名
                                e.Row["mgb05"] = 0;//數量
                                e.Row["mgb06"] = "";//單位
                                e.Row["mgb07"] = "";//出庫倉
                                //e.Row["mgb08"] = "";//入庫倉
                                return false;
                            }
                            var mebModel = BoMan.OfGetMebModel(detailModel.mgb10, Convert.ToInt16(detailModel.mgb11));
                            if (mebModel == null)
                            {
                                WfShowErrorMsg("無此來源項次,請檢核!");
                                return false;
                            }
                            if ((mebModel.meb08 - mebModel.meb09) <= 0)
                            {
                                WfShowErrorMsg("此來源項次無可退料量,請檢核!");
                                return false;
                            }

                            e.Row["mgb03"] = mebModel.meb03;//料號
                            e.Row["mgb04"] = mebModel.meb04;//品名
                            if (mebModel.meb08 - mebModel.meb09 <= 0)//數量
                                e.Row["mgb05"] = 0;
                            else
                                e.Row["mgb05"] = mebModel.meb08 - mebModel.meb09;
                            e.Row["mgb06"] = mebModel.meb06;//單位
                            //e.Row["mgb07"] = mebModel.meb07;//出庫倉
                            //e.Row["mgb08"] = mebModel.meb07;//入庫倉
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
            vw_mant312 masterModel = null;
            vw_mant312s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {

                masterModel = DrMaster.ToItem<vw_mant312>();
                if (!GlobalFn.varIsNull(masterModel.mga01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.mga01, ""));
                #region 單頭資料檢查
                chkColName = "mga01";       //託工單號
                chkControl = ute_mga01;
                if (GlobalFn.varIsNull(masterModel.mga01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mga02";       //退料日期
                chkControl = udt_mga02;
                if (GlobalFn.varIsNull(masterModel.mga02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.mga02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "mga06";       //託工單號
                chkControl = ute_mga06;
                if (GlobalFn.varIsNull(masterModel.mga06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mga03";       //廠商編號
                chkControl = ute_mga03;
                if (GlobalFn.varIsNull(masterModel.mga03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mga04";       //人員
                chkControl = ute_mga04;
                if (GlobalFn.varIsNull(masterModel.mga04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mga05";       //部門
                chkControl = ute_mga05;
                if (GlobalFn.varIsNull(masterModel.mga05))
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

                    detailModel = drTemp.ToItem<vw_mant312s>();
                    chkColName = "mgb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.mgb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "mgb11";   //來源項次
                    if (GlobalFn.varIsNull(detailModel.mgb11))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "mgb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.mgb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "mgb05";   //數量
                    #region mfb05 數量
                    if (GlobalFn.varIsNull(detailModel.mgb05) || detailModel.mgb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);

                        return false;
                    }

                    //if (WfChkMeb05(drTemp, detailModel) == false)
                    //{
                    //    this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                    //    //msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    //    //msg += "不可為空白或小於0!";
                    //    //WfShowErrorMsg(msg);
                    //    WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                    //    return false;
                    //}
                    #endregion

                    chkColName = "mgb07";   //出庫倉
                    if (GlobalFn.varIsNull(detailModel.mgb07))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    //chkColName = "mgb08";   //入庫倉
                    //if (GlobalFn.varIsNull(detailModel.mgb08))
                    //{
                    //    this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                    //    msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault(); msg += "不可為空白";
                    //    WfShowErrorMsg(msg);
                    //    WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                    //    return false;
                    //}
                    ////檢查出庫倉與入庫倉是否相同
                    //if (detailModel.mgb07 == detailModel.mgb08)
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
            string mga01New, errMsg;
            vw_mant312 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant312>();
                if (FormEditMode == YREditType.新增)
                {
                    mga01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.mga01, ModuleType.man, (DateTime)masterModel.mga02, out mga01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["mga01"] = mga01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["mgasecu"] = LoginInfo.UserNo;
                        DrMaster["mgasecg"] = LoginInfo.GroupNo;
                        DrMaster["mgacreu"] = LoginInfo.UserNo;
                        DrMaster["mgacreg"] = LoginInfo.DeptNo;
                        DrMaster["mgacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["mgamodu"] = LoginInfo.UserNo;
                        DrMaster["mgamodg"] = LoginInfo.DeptNo;
                        DrMaster["mgamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["mgbcreu"] = LoginInfo.UserNo;
                            drDetail["mgbcreg"] = LoginInfo.DeptNo;
                            drDetail["mgbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["mgbmodu"] = LoginInfo.UserNo;
                            drDetail["mgbmodg"] = LoginInfo.DeptNo;
                            drDetail["mgbmodd"] = Now;
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

        #region WfChkMeb05 數量檢查--已退料量是否大於已送料量
        //目前先檢查是否有超出託工單的預計數量,後續要在單別加入是否可超出的檢查
        private bool WfChkMeb08(DataRow pdr, vw_mant312s pListDetail)
        {
            List<vw_mant312s> detailList = null;
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
                detailList = pdr.Table.ToList<vw_mant312s>();
                if (GlobalFn.varIsNull(pListDetail.mgb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.mgb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.mgb06))
                {
                    errMsg = "請先輸入單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (pListDetail.mgb05 <= 0)
                {
                    errMsg = "數量應大於0!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mga01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                mebModel = BoMan.OfGetMebModel(pListDetail.mgb10, Convert.ToInt16(pListDetail.mgb11));
                if (babModel == null)
                {
                    errMsg = "Get meb_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //取得本身單據中的所有數量轉換銷貨單位後的總和!
                //先取本身這筆
                docThisQty = pListDetail.mgb05;
                //再取其他筆加總
                docOtherQtyTotal = detailList.Where(p => p.mgb02 != pListDetail.mgb02)
                                   .Where(p => p.mgb10 == pListDetail.mgb10 && p.mgb11 == pListDetail.mgb11)
                                   .Sum(p => p.mgb05);
                //取得其他單據上的加總
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT SUM(mgb05) FROM mga_tb");
                sbSql.AppendLine("  INNER JOIN mgb_tb ON mga01=mgb01");
                sbSql.AppendLine("WHERE mgaconf<>'X'");
                sbSql.AppendLine("  AND mga01 <> @mga01");
                sbSql.AppendLine("  AND mgb10 = @mgb10 AND mgb11 = @mgb11");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@mga01", GlobalFn.isNullRet(DrMaster["mga01"], "")));
                sqlParmList.Add(new SqlParameter("@mgb10", pListDetail.mgb10));
                sqlParmList.Add(new SqlParameter("@mgb11", pListDetail.mgb11));
                otherQtyTotal = GlobalFn.isNullRet(BoMan.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (mebModel.meb08 < (docThisQty + docOtherQtyTotal + otherQtyTotal))
                {
                    errMsg = string.Format("項次{0} 退料單最大可輸入數量為 {1}",
                                            pListDetail.mgb02.ToString(),
                                            (mebModel.meb08 - docOtherQtyTotal - otherQtyTotal).ToString()
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

        #region WfChkMeb08Meb09 數量檢查--退料單確認/取消確認後,對託工單已送料量做檢查
        private bool WfChkMeb08Meb09(DataRow pdr, vw_mant312s pListDetail)
        {
            List<vw_mant312s> detailList = null;
            meb_tb mebModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換退料單數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的退料單數量加總
            decimal otherQtyTotal = 0;      //其他單據的數量加總
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                detailList = pdr.Table.ToList<vw_mant312s>();
                if (GlobalFn.varIsNull(pListDetail.mgb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.mgb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.mgb06))
                {
                    errMsg = "請先輸入單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (pListDetail.mgb05 <= 0)
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

                mebModel = BoMan.OfGetMebModel(pListDetail.mgb10, Convert.ToInt16(pListDetail.mgb11));
                if (babModel == null)
                {
                    errMsg = "Get meb_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //取得本身單據中的所有數量總和!
                //先取本身這筆
                docThisQty = pListDetail.mgb05;
                //再取其他筆加總
                docOtherQtyTotal = detailList.Where(p => p.mgb02 != pListDetail.mgb02)
                                   .Where(p => p.mgb10 == pListDetail.mgb10 && p.mgb11 == pListDetail.mgb11)
                                   .Sum(p => p.mgb05);
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

                if ((mebModel.meb08 - mebModel.meb09) > (docOtherQtyTotal + docThisQty))
                {
                    errMsg = string.Format("項次{0} 退料單確認後託工單總退料量不得大於{1}!",
                                            pListDetail.mgb02.ToString(),
                                            (mebModel.meb08 - mebModel.meb09).ToString()
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
            mga_tb mgaModel = null;
            mgb_tb mgbModel = null;
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
                mgaModel = DrMaster.ToItem<mga_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                mgaModel.mgaconf = "Y";
                mgaModel.mgacond = Today;
                mgaModel.mgaconu = LoginInfo.UserNo;

                //更新託工單明細已退料量
                if (WfUpdMeb09(true) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    mgbModel = dr.ToItem<mgb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("2", mgbModel.mgb03, mgbModel.mgb07, mgbModel.mgb05, out errMsg) == false)//出庫
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                    //if (BoInv.OfUpdIcc05("1", mgbModel.mgb03, mgbModel.mgb08, mgbModel.mgb05, out errMsg) == false)//入庫
                    //{
                    //    WfShowErrorMsg(errMsg);
                    //    DrMaster.RejectChanges();
                    //    WfRollback();
                    //    return;
                    //}
                    //更新庫存交易歷史檔 出+入 合併更新
                    if (BoInv.OfStockPost("mant312", mgaModel, mgbModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }

                DrMaster["mgaconf"] = "Y";
                DrMaster["mgacond"] = Today;
                DrMaster["mgaconu"] = LoginInfo.UserNo;
                DrMaster["mgamodu"] = LoginInfo.UserNo;
                DrMaster["mgamodg"] = LoginInfo.DeptNo;
                DrMaster["mgamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                mgaModel = DrMaster.ToItem<mga_tb>();
                WfSetDocPicture("", mgaModel.mgaconf, "", pbxDoc);
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
            vw_mant312 masterModel = null;
            vw_mant312s detailModel = null;
            List<vw_mant312s> detailList = null;
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant312>();
                if (masterModel.mgaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.mga02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_mant312s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_mant312s>();
                    //檢查託工單是否要超出預計數量
                    if (WfChkMeb08(drTemp, detailModel) == false)
                    {
                        return false;
                    }
                }

                //檢查送料單的數量是否足夠做出庫
                var MgbSumList =                         //依送料單的料號及出庫倉做加總
                        from mgb in detailList
                        where mgb.mgb05 > 0
                        group mgb by new { mgb.mgb03, mgb.mgb07 } into mfbSumTemp
                        select new
                        {
                            mfb03 = mfbSumTemp.Key.mgb03,
                            mfb07 = mfbSumTemp.Key.mgb07,
                            mfb05 = mfbSumTemp.Sum(p => p.mgb05)
                        }
                    ;
                foreach (var mgbSumModel in MgbSumList)
                {
                    icc05 = BoInv.OfGetIcc05(mgbSumModel.mfb03, mgbSumModel.mfb07);
                    if (icc05 < mgbSumModel.mfb05)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", mgbSumModel.mfb03, icc05));
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
            mga_tb mgaModel = null;
            mgb_tb mgbModel = null;
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
                mgaModel = DrMaster.ToItem<mga_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                //更新託工單明細已送料量
                if (WfUpdMeb09(false) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    mgbModel = dr.ToItem<mgb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("1", mgbModel.mgb03, mgbModel.mgb07, mgbModel.mgb05, out errMsg) == false) //取消時出庫倉應入庫
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                    if (BoInv.OfUpdIcc05("2", mgbModel.mgb03, mgbModel.mgb08, mgbModel.mgb05, out errMsg) == false) //取消時入庫倉應出庫
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔
                    if (BoInv.OfDelIna(mgbModel.mgb01, mgbModel.mgb02, "1", out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                    if (BoInv.OfDelIna(mgbModel.mgb01, mgbModel.mgb02, "2", out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                }

                DrMaster["mgaconf"] = "N";
                DrMaster["mgacond"] = DBNull.Value;
                DrMaster["mgaconu"] = "";
                DrMaster["mgamodu"] = LoginInfo.UserNo;
                DrMaster["mgamodg"] = LoginInfo.DeptNo;
                DrMaster["mgamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                mgaModel = DrMaster.ToItem<mga_tb>();
                WfSetDocPicture("", mgaModel.mgaconf, "", pbxDoc);
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
            vw_mant312 masterModel = null;
            vw_mant312s detailModel = null;
            List<vw_mant312s> detailList = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            decimal icc05 = 0;
            int iChkCnts = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant312>();
                if (masterModel.mgaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.mga02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_mant312s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_mant312s>();
                    //檢查託工單是否要取消確認後總送料量是否小於已耗用量
                    if (WfChkMeb08(drTemp, detailModel) == false)
                    {
                        return false;
                    }
                }

                //檢查送料單的入庫倉數量是否足夠做出庫
                var MgbSumList =                         //依送料單的料號及出庫倉做加總
                        from mgb in detailList
                        where mgb.mgb05 > 0
                        group mgb by new { mgb.mgb03, mgb.mgb08 } into mgbSumTemp
                        select new
                        {
                            mgb03 = mgbSumTemp.Key.mgb03,
                            mgb08 = mgbSumTemp.Key.mgb08,
                            mgb05 = mgbSumTemp.Sum(p => p.mgb05)
                        }
                    ;
                foreach (var mgbSumModel in MgbSumList)
                {
                    icc05 = BoInv.OfGetIcc05(mgbSumModel.mgb03, mgbSumModel.mgb08);
                    if (icc05 < mgbSumModel.mgb05)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", mgbSumModel.mgb05, icc05));
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
            vw_mant312 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_mant312>();

                if (masterModel.mgaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.mgaconf == "N")//走作廢
                {

                    DrMaster["mgaconf"] = "X";
                    DrMaster["mgaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.mgaconf == "X")
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
                    DrMaster["mgaconf"] = "N";
                    DrMaster["mgaconu"] = "";
                }

                DrMaster["mgamodu"] = LoginInfo.UserNo;
                DrMaster["mgamodg"] = LoginInfo.DeptNo;
                DrMaster["mgamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_mant312>();
                WfSetDocPicture("", masterModel.mgaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfUpdMeb09 更新託工單已出貨數量 確認/取消確認
        private bool WfUpdMeb09(bool pbConfirm)
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
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mga01"], ""));
                if (babModel == null)
                {
                    WfShowErrorMsg("Get bab_tb Error!");
                    return false;
                }
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT DISTINCT mgb10,mgb11,SUM(mgb05) mgb05");
                sbSql.AppendLine("FROM mgb_tb");
                sbSql.AppendLine("WHERE mgb01=@mgb01");
                sbSql.AppendLine("  AND ISNULL(mgb10,'')<>''");
                sbSql.AppendLine("GROUP BY mgb10,mgb11");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@mgb01", GlobalFn.isNullRet(DrMaster["mga01"], "")));
                dtMfbDistinct = BoMan.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                if (dtMfbDistinct == null)
                    return true;
                foreach (DataRow dr in dtMfbDistinct.Rows)
                {
                    meb01 = dr["mgb10"].ToString();
                    meb02 = Convert.ToDecimal(dr["mgb11"]);
                    docQty = Convert.ToDecimal(dr["mgb05"]);

                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT SUM(mgb05) FROM mga_tb");
                    sbSql.AppendLine("  INNER JOIN mgb_tb ON mga01=mgb01");
                    sbSql.AppendLine("WHERE mgaconf='Y'");
                    sbSql.AppendLine("AND mgb10=@mgb10 AND mgb11=@mgb11");
                    sbSql.AppendLine("AND mga01<>@mga01");


                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@mgb10", meb01));
                    sqlParmList.Add(new SqlParameter("@mgb11", meb02));
                    sqlParmList.Add(new SqlParameter("@mga01", GlobalFn.isNullRet(DrMaster["mga01"], "")));
                    otherDocQty = 0;
                    otherDocQty = GlobalFn.isNullRet(BoMan.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);

                    sbSql = new StringBuilder();
                    sbSql = sbSql.AppendLine("UPDATE meb_tb");
                    sbSql = sbSql.AppendLine("SET meb09=@meb09");
                    sbSql = sbSql.AppendLine("WHERE meb01=@meb01");
                    sbSql = sbSql.AppendLine("AND meb02=@meb02");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@meb01", meb01));
                    sqlParmList.Add(new SqlParameter("@meb02", meb02));
                    if (pbConfirm)  //確認的要加本身單據
                        sqlParmList.Add(new SqlParameter("@meb09", docQty + otherDocQty));
                    else
                        sqlParmList.Add(new SqlParameter("@meb09", otherDocQty));

                    if (BoMan.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray()) != 1)
                    {
                        WfShowErrorMsg("更新託工單已送料數量失敗!");
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
