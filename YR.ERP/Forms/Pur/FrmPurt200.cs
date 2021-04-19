/* 程式名稱: 請購單維護作業
   系統代號: purt200
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
using YR.ERP.Shared;

namespace YR.ERP.Forms.Pur
{
    public partial class FrmPurt200 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region property
        PurBLL BoPur = null;
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        baa_tb BaaTbModel = null;
        AdmBLL BoAdm = null;

        bek_tb BekModel = null;      //幣別檔,在display mode載入
        #endregion
        
        #region 建構子
        public FrmPurt200()
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
            this.StrFormID = "purt200";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("pea01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "peasecu";
                TabMaster.GroupColumn = "peasecg";

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

            BoPur = new PurBLL(BoMaster.OfGetConntion());
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
                    BoPur.TRAN = BoMaster.TRAN;
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
                sourceList = BoPur.OfGetTaxTypeKVPList();
                WfSetUcomboxDataSource(ucb_pea06, sourceList);

                //發票聯數
                sourceList = BoPur.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_pea09, sourceList);

                //單據確認
                sourceList = BoPur.OfGetPeaconfKVPList();
                WfSetUcomboxDataSource(ucb_peaconf, sourceList);

                //單據確認
                sourceList = BoPur.OfGetPeastatKVPList();
                WfSetUcomboxDataSource(ucb_peastat, sourceList);
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
            this.TabDetailList[0].TargetTable = "peb_tb";
            this.TabDetailList[0].ViewTable = "vw_purt200s";
            keyParm = new SqlParameter("peb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "pea01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_purt200 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_purt200>();
                    WfSetDocPicture("", masterModel.peaconf, masterModel.peastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.pea10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekModel = BoBas.OfGetBekModel(masterModel.pea10);
                        if (BekModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.pea10));
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

                    WfSetControlReadonly(new List<Control> { ute_peacreu, ute_peacreg, udt_peacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_peamodu, ute_peamodg, udt_peamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_peasecu, ute_peasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.pea01, "") != "")
                        WfSetControlReadonly(ute_pea01, true);

                    WfSetControlReadonly(new List<Control> { ute_pea03_c, ute_pea04_c, ute_pea05_c, ute_pea11_c, ute_pea12_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_pea07, ucx_pea08 }, true);
                    WfSetControlReadonly(new List<Control> { ucb_peaconf, udt_peacond, ute_peaconu, ucb_peastat }, true);
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_pea01, true);
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
                            if (columnName == "peb03" ||
                                columnName == "peb05" ||
                                columnName == "peb06" ||
                                columnName == "peb09"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "peb02")
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
            //int iChkCnts = 0;
            //string errMsg;
            //StringBuilder sbSql;
            //List<SqlParameter> sqlParms;
            try
            {
                MessageInfo messageModel = new MessageInfo();
                //this.MsgInfoReturned = new MessageInfo();
                #region 單頭-pick vw_purt200
                if (pDr.Table.Prefix.ToLower() == "vw_purt200")
                {
                    switch (pColName.ToLower())
                    {
                        case "pea01"://採購單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "pur"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "10"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "pea03"://廠商編號
                            WfShowPickUtility("p_pca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "pea04"://請購人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "pea05"://請購部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                            
                        case "pea10"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pea11"://付款條件
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
                            
                        case "pea12"://取價條件
                            WfShowPickUtility("p_pbb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pbb01"], "");
                                else
                                    pDr[pColName] = "";                                
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_purt200s
                if (pDr.Table.Prefix.ToLower() == "vw_purt200s")
                {
                    switch (pColName.ToLower())
                    {
                        case "peb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "peb06"://請購單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
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
                pDr["pea02"] = Today;
                pDr["pea04"] = LoginInfo.UserNo;
                pDr["pea04_c"] = LoginInfo.UserName;
                pDr["pea05"] = LoginInfo.DeptNo;
                pDr["pea05_c"] = LoginInfo.DeptName;
                pDr["pea07"] = 0;
                pDr["pea08"] = "N";
                pDr["pea10"] = BaaTbModel.baa04;
                pDr["pea14"] = 1;   //匯率
                pDr["peaconf"] = "N";
                pDr["peastat"] = "0";
                pDr["peacomp"] = LoginInfo.CompNo;
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
                        pDr["peb02"] = WfGetMaxSeq(pDr.Table, "peb02");
                        pDr["peb05"] = 0;
                        pDr["peb08"] = 0;
                        pDr["peb09"] = 0;
                        pDr["peb10"] = 0;
                        pDr["peb10t"] = 0;
                        pDr["peb11"] = 0;
                        pDr["pebcomp"] = LoginInfo.CompNo;
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
            vw_purt200 masterModel;
            string errMsg = "";
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_purt200>();
                if (masterModel.peaconf != "N")
                {
                    if (masterModel.peaconf == "Y")
                        errMsg = "單據已確認,不可更改";
                    else if (masterModel.peaconf == "X")
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
            vw_purt200 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_purt200>();
                if (masterModel.peaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可作廢!");
                    return false;
                }

                //還需檢查採購單
                if (WfChkPfaExists(masterModel.pea01) == true)
                {
                    WfShowErrorMsg("已有採購單資料!不可取消確認!");
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
            vw_purt200s detailModel = null;
            List<vw_purt200s> detailList = null;
            UltraGrid uGrid;
            try
            {
                #region 單頭-pick vw_purt200
                if (e.Row.Table.Prefix.ToLower() == "vw_purt200")
                {
                    switch (e.Column.ToLower())
                    {
                        case "pea01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "pur", "10") == false)
                            {
                                WfShowErrorMsg("無此請購單別,請檢核!");
                                return false;
                            }
                            WfSetControlReadonly(ute_pea01, true);
                            break;
                        case "pea03"://廠商
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pea03_c"] = "";
                                return true;
                            }
                            if (BoPur.OfChkPcaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此廠商資料,請檢核!");
                                return false;
                            }
                            WfSetPea03Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByPea06();
                            break;

                        case "pea04"://請購人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pea04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["pea04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pea05"://請購部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pea05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["pea05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pea06"://課稅別
                            WfSetPea06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByPea06();
                            break;

                        case "pea10"://幣別
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBekPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此幣別");
                                return false;
                            }
                            BekModel = BoBas.OfGetBekModel(e.Value.ToString());
                            break;

                        case "pea11"://付款條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pea11_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("1", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此付款條件,請檢核!");
                                return false;
                            }
                            e.Row["pea11_c"] = BoBas.OfGetBef03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pea12"://取價條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pea12_c"] = "";
                                return true;
                            }
                            if (BoPur.OfChkPbbPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此採購取價條件,請檢核!");
                                return false;
                            }
                            e.Row["pea12_c"] = BoPur.OfGetPbb02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pea14": //匯率
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

                #region 單身-pick vw_purt200s
                if (e.Row.Table.Prefix.ToLower() == "vw_purt200s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_purt200s>();
                    detailList = e.Row.Table.ToList<vw_purt200s>();
                    switch (e.Column.ToLower())
                    {
                        case "peb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.peb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "peb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["peb04"] = "";
                                e.Row["peb05"] = 0;
                                e.Row["peb06"] = "";
                                e.Row["peb07"] = "";
                                e.Row["peb08"] = 0;
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }
                            if (WfSetPeb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;
                            
                            break;

                        case "peb05"://請購數量
                            if (GlobalFn.varIsNull(detailModel.peb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["peb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detailModel.peb06))
                            {
                                WfShowErrorMsg("請先輸入請購單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["peb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入採購數量!");
                                return false;
                            }
                            detailModel.peb05 = BoBas.OfGetUnitRoundQty(detailModel.peb06, detailModel.peb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.peb05;
                            WfSetDetailAmt(e.Row);
                            break;

                        case "peb06"://請購單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.peb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["peb03"]);
                                return false;
                            }
                            if (WfChkPeb06(e.Row, e.Value.ToString()) == false)
                                return false;

                            if (WfSetPeb06Relation(e.Row, e.Value.ToString()) == false)
                                return false;
                            break;
                        case "peb09":
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.peb09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.peb09, BekModel.bek03);
                            WfSetDetailAmt(e.Row);
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
            vw_purt200 masterModel = null;
            vw_purt200s detailModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt200>();
                #region 單頭資料檢查
                chkColName = "pea01";       //請購單號
                chkControl = ute_pea01;
                if (GlobalFn.varIsNull(masterModel.pea01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pea02";       //採購日期
                chkControl = udt_pea02;
                if (GlobalFn.varIsNull(masterModel.pea02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pea10";       //幣別
                chkControl = ute_pea10;
                if (GlobalFn.varIsNull(masterModel.pea10))
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

                    detailModel = drTemp.ToItem<vw_purt200s>();
                    #region peb02-項次
                    chkColName = "peb02";
                    if (GlobalFn.varIsNull(detailModel.peb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region peb03-料號
                    chkColName = "peb03";
                    if (GlobalFn.varIsNull(detailModel.peb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region peb05-請購數量
                    chkColName = "peb05";
                    if (GlobalFn.varIsNull(detailModel.peb05) || detailModel.peb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "應大於0";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region peb06-請購單位
                    chkColName = "peb06";
                    if (GlobalFn.varIsNull(detailModel.peb05))
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
            string pea01New, errMsg;
            vw_purt200 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt200>();
                if (FormEditMode == YREditType.新增)
                {
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.pea01, ModuleType.pur, (DateTime)masterModel.pea02, out pea01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["pea01"] = pea01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["peasecu"] = LoginInfo.UserNo;
                        DrMaster["peasecg"] = LoginInfo.GroupNo;
                        DrMaster["peacreu"] = LoginInfo.UserNo;
                        DrMaster["peacreg"] = LoginInfo.DeptNo;
                        DrMaster["peacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["peamodu"] = LoginInfo.UserNo;
                        DrMaster["peamodg"] = LoginInfo.DeptNo;
                        DrMaster["peamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["pebcreu"] = LoginInfo.UserNo;
                            drDetail["pebcreg"] = LoginInfo.DeptNo;
                            drDetail["pebcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["pebmodu"] = LoginInfo.UserNo;
                            drDetail["pebmodg"] = LoginInfo.DeptNo;
                            drDetail["pebmodd"] = Now;
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

                bt = new ButtonTool("Purr200");
                adoModel = BoAdm.OfGetAdoModel("Purr200");
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

                    case "Purr200":
                        vw_purr200 l_vw_purr200;
                        vw_purt200 lMaster;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        lMaster = DrMaster.ToItem<vw_purt200>();
                        l_vw_purr200 = new vw_purr200();
                        l_vw_purr200.pea01 = lMaster.pea01;
                        l_vw_purr200.pea03 = "";
                        l_vw_purr200.jump_yn = "N";
                        l_vw_purr200.order_by = "1";

                        FrmPurr200 rpt = new FrmPurr200(this.LoginInfo, l_vw_purr200, true, true);
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
        #region WfSetPea03Relation 設定廠商相關聯
        private void WfSetPea03Relation(string pPea03)
        {
            pca_tb pcaModel;
            try
            {
                pcaModel = BoPur.OfGetPcaModel(pPea03);
                if (pcaModel == null)
                    return;

                DrMaster["pea03_c"] = pcaModel.pca03;
                DrMaster["pea06"] = pcaModel.pca22;    //課稅別
                WfSetPea06Relation(pcaModel.pca22);
                DrMaster["pea09"] = pcaModel.pca23;    //發票聯數

                DrMaster["pea11"] = pcaModel.pca21;    //付款條件
                DrMaster["pea11_c"] = BoBas.OfGetBef03("1", pcaModel.pca21);
                DrMaster["pea12"] = pcaModel.pca24;    //取價條件
                DrMaster["pea12_c"] = BoPur.OfGetPbb02(pcaModel.pca24);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPea06Relation 設定稅別關聯
        private void WfSetPea06Relation(string pPea06)
        {
            try
            {
                if (pPea06 == "1")
                {
                    DrMaster["pea07"] = BaaTbModel.baa05;
                    DrMaster["pea08"] = "Y";
                }
                else if (pPea06 == "2")
                {
                    DrMaster["pea07"] = BaaTbModel.baa05;
                    DrMaster["pea08"] = "N";
                }
                else
                {
                    DrMaster["pea07"] = 0;
                    DrMaster["pea08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPeb03Relation 設定料號關聯
        private bool WfSetPeb03Relation(string pPeb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal peb08;
            try
            {
                icaModel = BoInv.OfGetIcaModel(pPeb03);

                if (BoInv.OfGetUnitCovert(pPeb03, icaModel.ica08, icaModel.ica07, out peb08) == false)
                {
                    WfShowErrorMsg("未設定料號轉換,請先設定!");
                    return false;
                }

                pDr["peb04"] = icaModel.ica02;
                pDr["peb06"] = icaModel.ica08;   //請購單位
                pDr["peb07"] = icaModel.ica07;   //庫存單位
                pDr["peb08"] = peb08;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPfb06Relation 設定採購單位關聯
        //檢查前要先確認料號是否已輸入
        private bool WfSetPeb06Relation(DataRow pDr, string pPeb06)
        {
            vw_purt200s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_purt200s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pPeb06, "")) == false)
                {
                    WfShowErrorMsg("無此採購單位!請確認");
                    return false;
                }
                //取得是否有採購對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.peb03, pPeb06, detailModel.peb07, out dConvert) == true)
                {
                    pDr["peb08"] = dConvert;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkPfb06 檢查採購單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkPeb06(DataRow pDr, string pPeb06)
        {
            vw_purt200s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_purt200s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pPeb06, "")) == false)
                {
                    WfShowErrorMsg("無此請購單位!請確認");
                    return false;
                }
                //檢查是否有採購對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.peb03, pPeb06, detailModel.peb07, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定請購單位對庫存單位的轉換率,請先設定!");
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
            vw_purt200 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_purt200>();
                if (masterModel.peaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    WfRollback();
                    return;
                }
                DrMaster["peastat"] = "1";
                DrMaster["peaconf"] = "Y";
                DrMaster["peacond"] = Today;
                DrMaster["peaconu"] = LoginInfo.UserNo;
                DrMaster["peamodu"] = LoginInfo.UserNo;
                DrMaster["peamodg"] = LoginInfo.DeptNo;
                DrMaster["peamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_purt200>();
                WfSetDocPicture("", masterModel.peaconf, masterModel.peastat, pbxDoc);
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
            vw_purt200 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_purt200>();
                if (masterModel.peaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    WfRollback();
                    return;
                }
                //還需檢查採購單
                if (WfChkPfaExists(masterModel.pea01) == true)
                {
                    WfShowErrorMsg("已有採購資料!請先作廢採購單!");
                    WfRollback();
                    return;
                }

                DrMaster["peastat"] = "0";
                DrMaster["peaconf"] = "N";
                DrMaster["peacond"] = DBNull.Value;
                DrMaster["peaconu"] = "";
                DrMaster["peamodu"] = LoginInfo.UserNo;
                DrMaster["peamodg"] = LoginInfo.DeptNo;
                DrMaster["peamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_purt200>();
                WfSetDocPicture("", masterModel.peaconf, masterModel.peastat, pbxDoc);
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
            vw_purt200 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_purt200>();

                if (masterModel.peaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.peaconf == "N")//走作廢
                {
                    //還需檢查採購單
                    if (WfChkPfaExists(masterModel.pea01) == true)
                    {
                        WfShowErrorMsg("已有採購資料!請先作廢採購單!");
                        WfRollback();
                        return;
                    }

                    DrMaster["peastat"] = "X";
                    DrMaster["peaconf"] = "X";
                    DrMaster["peaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.peaconf == "X")
                {
                    DrMaster["peastat"] = "0";
                    DrMaster["peaconf"] = "N";
                    DrMaster["peaconu"] = "";
                }
                DrMaster["peamodu"] = LoginInfo.UserNo;
                DrMaster["peamodg"] = LoginInfo.DeptNo;
                DrMaster["peamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_purt200>();
                WfSetDocPicture("", masterModel.peaconf, masterModel.peastat, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfChkPfaExists 檢查採購單是否存在
        private bool WfChkPfaExists(string pPea01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChkCnts = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM pfa_tb");
                sbSql.AppendLine("  INNER JOIN pfb_tb ON pfa01=pfb01");
                sbSql.AppendLine("WHERE");
                sbSql.AppendLine(" pfaconf <>'X' ");
                sbSql.AppendLine(" AND pfb11=@pfa11 ");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pfa11", pPea01));
                iChkCnts = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
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
        private bool WfSetDetailAmt(DataRow drPeb)
        {
            pea_tb peaModel;
            peb_tb pebModel;
            decimal peb10t = 0, peb10 = 0;
            try
            {
                peaModel = DrMaster.ToItem<pea_tb>();
                pebModel = drPeb.ToItem<peb_tb>();

                if (peaModel.pea08 == "Y")//稅內含
                {
                    peb10t = pebModel.peb09 * pebModel.peb05;
                    peb10t = GlobalFn.Round(peb10t, BekModel.bek04);
                    peb10 = peb10t / (1 + (peaModel.pea07 / 100m));
                    peb10 = GlobalFn.Round(peb10, BekModel.bek04);
                }
                else//稅外加
                {
                    peb10 = pebModel.peb09 * pebModel.peb05;
                    peb10 = GlobalFn.Round(peb10, BekModel.bek04);
                    peb10t = peb10 * (1 + (peaModel.pea07 / 100m));
                    peb10t = GlobalFn.Round(peb10t, BekModel.bek04);
                }
                drPeb["peb10"] = peb10;
                drPeb["peb10t"] = peb10t;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfResetAmtByPfe06 依稅別更新單身金額
        private void WfResetAmtByPea06()
        {
            try
            {
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    WfSetDetailAmt(dr);
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
