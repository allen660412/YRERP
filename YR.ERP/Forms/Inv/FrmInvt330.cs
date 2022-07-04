/* 程式名稱: 直接調撥作業
   系統代號: invt330
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
    public partial class FrmInvt330 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;

        baa_tb BaaTbModel = null;
        #endregion

        #region 建構子
        public FrmInvt330()
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
            this.StrFormID = "invt330";

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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ifa01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "ifasecu";
                TabMaster.GroupColumn = "ifasecg";
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
                sourceList = BoInv.OfGetIfaconfKVPList();
                WfSetUcomboxDataSource(ucb_ifaconf, sourceList);
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

            this.TabDetailList[0].TargetTable = "ifb_tb";
            this.TabDetailList[0].ViewTable = "vw_invt330s";
            keyParm = new SqlParameter("ifb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "ifa01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_invt330 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_invt330>();
                    WfSetDocPicture("", masterModel.ifaconf, "", pbxDoc);
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
                    WfSetControlReadonly(new List<Control> { ute_ifacreu, ute_ifacreg, udt_ifacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_ifamodu, ute_ifamodg, udt_ifamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_ifasecu, ute_ifasecg }, true);

                    WfSetControlReadonly(new List<Control> { ute_ifa01_c, ute_ifa03_c, ute_ifa04_c }, true);
                    WfSetControlReadonly(new List<Control> { ucb_ifaconf, udp_ifacond, ute_ifaconu }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_ifa01, true);
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
                            if (
                                columnName == "ifb03" ||
                                columnName == "ifb05" ||
                                columnName == "ifb07" ||
                                columnName == "ifb10"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "ifb02")
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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                vw_invt330 masterModel = null;
                vw_invt330s detailModel = null;
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_invt330
                if (pDr.Table.Prefix.ToLower() == "vw_invt330")
                {
                    switch (pColName.ToLower())
                    {
                        case "ifa01"://調撥單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "inv"));
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
                        case "ifa03"://申請人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ifa04"://申請部門
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

                #region 單身-pick vw_invt330s
                if (pDr.Table.Prefix.ToLower() == "vw_invt330s")
                {
                    masterModel = DrMaster.ToItem<vw_invt330>();
                    detailModel = pDr.ToItem<vw_invt330s>();
                    switch (pColName.ToLower())
                    {
                        case "ifb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "ifb06"://撥出單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "ifb07"://撥出倉庫
                            if (GlobalFn.isNullRet(detailModel.ifb03, "") == "")
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
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.ifb03));
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

                        case "ifb10"://撥入倉庫
                            WfShowPickUtility("p_icb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icb01"], "");
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
                pDr["ifa00"] = "1";
                pDr["ifa02"] = Today;
                pDr["ifa03"] = LoginInfo.UserNo;
                pDr["ifa03_c"] = LoginInfo.UserName;
                pDr["ifa04"] = LoginInfo.DeptNo;
                pDr["ifa04_c"] = LoginInfo.DeptName;
                pDr["ifa07"] = Today;
                pDr["ifa09"] = "N";
                pDr["ifaconf"] = "N";
                pDr["ifacomp"] = LoginInfo.CompNo;
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
                        pDr["ifb02"] = WfGetMaxSeq(pDr.Table, "ifb02");
                        pDr["ifb05"] = 0;
                        pDr["ifb08"] = 0;
                        pDr["ifb12"] = 0;
                        pDr["ifb13"] = 0;
                        pDr["ifbcomp"] = LoginInfo.CompNo;
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
            vw_invt330 listMaster;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                listMaster = DrMaster.ToItem<vw_invt330>();
                if (listMaster.ifaconf != "N")
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
            vw_invt330 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_invt330>();
                if (masterModel.ifaconf != "N")
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_invt330 masterModel = null;
            vw_invt330s detailModel = null;
            List<vw_invt330s> listDetails = null;
            bab_tb l_bab = null;
            UltraGrid uGrid = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt330>();
                if (e.Column.ToLower() != "ifa01" && GlobalFn.isNullRet(DrMaster["ifa01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    WfItemChkForceFocus(ute_ifa01);
                    return false;
                }
                #region 單頭-pick vw_invt330
                if (e.Row.Table.Prefix.ToLower() == "vw_invt330")
                {
                    switch (e.Column.ToLower())
                    {
                        case "ifa01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "inv", "20") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["ifa01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ifa03"://申請人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ifa03_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["ifa03_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ifa04"://申請部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ifa04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["ifa04_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ifa07"://撥出日期
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (masterModel.ifa02 !=null)
                            {
                                if (DateTime.Compare(Convert.ToDateTime(masterModel.ifa02),Convert.ToDateTime(e.Value))>0)
                                {
                                    WfShowErrorMsg("調撥日期，不可超過撥出日期！");
                                    return false;
                                }
                            }

                            var result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;

                    }
                }
                #endregion

                #region 單身-pick vw_invt330s
                if (e.Row.Table.Prefix.ToLower() == "vw_invt330s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_invt330s>();
                    listDetails = e.Row.Table.ToList<vw_invt330s>();
                    l_bab = BoBas.OfGetBabModel(masterModel.ifa01);

                    switch (e.Column.ToLower())
                    {
                        case "ifb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            listDetails = e.Row.Table.ToList<vw_invt330s>();
                            iChkCnts = listDetails.Where(p => GlobalFn.isNullRet(p.ifb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "ifb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ifb04"] = "";//品名
                                e.Row["ifb05"] = 0;//撥出數量
                                e.Row["ifb06"] = "";//撥出單位
                                e.Row["ifb08"] = 0;//撥入數量
                                e.Row["ifb09"] = "";//撥入單位
                                e.Row["ifb11"] = "";//庫存單位
                                e.Row["ifb12"] = 0;//庫存轉換率
                                e.Row["ifb13"] = 0;//庫存數量
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }

                            if (WfSetIfb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;
                            break;
                        case "ifb05"://撥出數量
                            if (GlobalFn.varIsNull(detailModel.ifb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["ifb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detailModel.ifb06))
                            {
                                WfShowErrorMsg("請先輸入撥出單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["ifb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入撥出數量!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["ifb05"]);
                                return false;
                            }
                            detailModel.ifb05 = BoBas.OfGetUnitRoundQty(detailModel.ifb06, detailModel.ifb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.ifb05;
                            e.Row["ifb08"] = detailModel.ifb05;
                            e.Row["ifb13"] = detailModel.ifb05; ;
                            break;

                        case "ifb07"://撥出倉庫
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoInv.OfChkIcbPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此倉庫別,請確認!");
                                return false;
                            }
                            e.Row["ifb07_c"]= BoInv.OfGetIcb02(e.Value.ToString());
                            break;

                        case "ifb10"://撥入倉庫
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoInv.OfChkIcbPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此倉庫別,請確認!");
                                return false;
                            }
                            if (!GlobalFn.varIsNull(detailModel.ifb07) && detailModel.ifb07 == detailModel.ifb10)
                            {
                                WfShowErrorMsg("撥出倉與撥入倉不可相同,請確認!");
                                return false;
                            }
                            e.Row["ifb10_c"]= BoInv.OfGetIcb02(e.Value.ToString());
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
            vw_invt330 masterModel = null;
            vw_invt330s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {

                masterModel = DrMaster.ToItem<vw_invt330>();
                if (!GlobalFn.varIsNull(masterModel.ifa01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.ifa01, ""));
                #region 單頭資料檢查
                chkColName = "ifa01";       //調撥單號
                chkControl = ute_ifa01;
                if (GlobalFn.varIsNull(masterModel.ifa01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ifa02";       //調撥日期
                chkControl = udt_ifa02;
                if (GlobalFn.varIsNull(masterModel.ifa02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ifa03";       //申請人員
                chkControl = ute_ifa03;
                if (GlobalFn.varIsNull(masterModel.ifa03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ifa04";       //部門
                chkControl = ute_ifa04;
                if (GlobalFn.varIsNull(masterModel.ifa04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ifa07";       //撥出日期
                chkControl = udt_ifa07;
                if (GlobalFn.varIsNull(masterModel.ifa07))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.ifa07), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
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

                    detailModel = drTemp.ToItem<vw_invt330s>();
                    chkColName = "ifb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.ifb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ifb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.ifb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ifb05";   //撥出數量
                    #region ifb05 異動數量
                    if (GlobalFn.varIsNull(detailModel.ifb05) || detailModel.ifb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    #endregion

                    chkColName = "ifb06";   //撥出單位
                    if (GlobalFn.varIsNull(detailModel.ifb06))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ifb07";   //撥出倉庫
                    if (GlobalFn.varIsNull(detailModel.ifb07))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ifb10";   //撥入倉庫
                    if (GlobalFn.varIsNull(detailModel.ifb10))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    if (detailModel.ifb07 == detailModel.ifb10)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = "撥出倉與撥入倉不可相同";
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
            string ifa01New, errMsg;
            vw_invt330 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt330>();
                if (FormEditMode == YREditType.新增)
                {
                    ifa01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.ifa01, ModuleType.inv, (DateTime)masterModel.ifa02, out ifa01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["ifa01"] = ifa01New;
                }
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["ifasecu"] = LoginInfo.UserNo;
                        DrMaster["ifasecg"] = LoginInfo.GroupNo;
                        DrMaster["ifacreu"] = LoginInfo.UserNo;
                        DrMaster["ifacreg"] = LoginInfo.DeptNo;
                        DrMaster["ifacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["ifamodu"] = LoginInfo.UserNo;
                        DrMaster["ifamodg"] = LoginInfo.DeptNo;
                        DrMaster["ifamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["ifbcreu"] = LoginInfo.UserNo;
                            drDetail["ifbcreg"] = LoginInfo.DeptNo;
                            drDetail["ifbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["ifbmodu"] = LoginInfo.UserNo;
                            drDetail["ifbmodg"] = LoginInfo.DeptNo;
                            drDetail["ifbmodd"] = Now;
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
            ifa_tb ifaModel = null;
            ifb_tb ifbModel = null;
            try
            {
                ifaModel = DrMaster.ToItem<ifa_tb>();

                boAppendIcc = new InvBLL(BoMaster.OfGetConntion()); //新增料號庫存明細資料
                boAppendIcc.TRAN = BoMaster.TRAN;
                boAppendIcc.OfCreateDao("icc_tb", "*", "");
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icc_tb");
                sbSql.AppendLine("WHERE 1<>1");
                dtIcc = boAppendIcc.OfGetDataTable(sbSql.ToString());

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    ifbModel = dr.ToItem<ifb_tb>();
                    if (BoInv.OfChkIccPKExists(ifbModel.ifb03, ifbModel.ifb10) == false)
                    {
                        if (dtIcc.Rows.Count > 0)
                        {
                            var drIccs = dtIcc.Select(string.Format("icc01='{0}' AND icc02='{1}'", ifbModel.ifb03, ifbModel.ifb10));
                            if (drIccs != null && drIccs.Length > 0)
                                continue;
                        }
                        drIcc = dtIcc.NewRow();
                        drIcc["icc01"] = ifbModel.ifb03;  //料號
                        drIcc["icc02"] = ifbModel.ifb10;
                        drIcc["icc03"] = "";
                        drIcc["icc04"] = ifbModel.ifb11;
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

                bt = new ButtonTool("Invr330");
                adoModel = BoAdm.OfGetAdoModel("invr330");
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
            vw_invt330 masterModel;
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

                    case "Invr330":
                        vw_invr330 l_vw_invr330;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_invt330>();
                        l_vw_invr330 = new vw_invr330();
                        l_vw_invr330.ifa00 = "1";
                        l_vw_invr330.ifa01 = masterModel.ifa01;
                        l_vw_invr330.jump_yn = "N";

                        FrmInvr330 rpInvr330 = new FrmInvr330(this.LoginInfo, l_vw_invr330, true, true);
                        rpInvr330.WindowState = FormWindowState.Minimized;
                        rpInvr330.ShowInTaskbar = false;
                        rpInvr330.Show();
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
        #region WfSetIfb03Relation 設定料號關聯
        private bool WfSetIfb03Relation(string pIfb03, DataRow pDr)
        {
            ica_tb icaModel;
            try
            {
                icaModel = BoInv.OfGetIcaModel(pIfb03);
                if (icaModel == null)
                {
                    pDr["ifb04"] = "";//品名
                    pDr["ifb05"] = 0;//撥出數量
                    pDr["ifb06"] = "";//撥出單位
                    pDr["ifb08"] = 0;//撥入數量
                    pDr["ifb09"] = "";//撥入單位
                    pDr["ifb11"] = "";//庫存單位
                    pDr["ifb12"] = 0;//庫存轉換率
                    pDr["ifb13"] = 0;//庫存數量
                }
                else
                {

                    pDr["ifb04"] = icaModel.ica02;//品名
                    pDr["ifb05"] = 0;//撥出數量
                    pDr["ifb06"] = icaModel.ica07;//撥出單位帶入庫存單位
                    pDr["ifb08"] = 0;//撥入數量
                    pDr["ifb09"] = icaModel.ica07;//撥入單位帶入庫存單位
                    pDr["ifb11"] = icaModel.ica07;//庫存單位
                    pDr["ifb12"] = 1;
                    pDr["ifb13"] = 0;
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
            ifa_tb ifaModel = null;
            ifb_tb ifbModel = null;

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

                ifaModel = DrMaster.ToItem<ifa_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                ifaModel.ifaconf = "Y";
                ifaModel.ifacond = Today;
                ifaModel.ifaconu = LoginInfo.UserNo;

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    ifbModel = dr.ToItem<ifb_tb>();
                    //更新料件庫存量 icc_tb 
                    if (BoInv.OfUpdIcc05("2", ifbModel.ifb03, ifbModel.ifb07, ifbModel.ifb13, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }

                    //更新料件庫存量 icc_tb 
                    if (BoInv.OfUpdIcc05("1", ifbModel.ifb03, ifbModel.ifb10, ifbModel.ifb13, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔
                    if (BoInv.OfStockPost("invt330", ifaModel, ifbModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }

                DrMaster["ifaconf"] = "Y";
                DrMaster["ifacond"] = Today;
                DrMaster["ifaconu"] = LoginInfo.UserNo;
                DrMaster["ifamodu"] = LoginInfo.UserNo;
                DrMaster["ifamodg"] = LoginInfo.DeptNo;
                DrMaster["ifamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                ifaModel = DrMaster.ToItem<ifa_tb>();
                WfSetDocPicture("", ifaModel.ifaconf, "", pbxDoc);
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
            vw_invt330 masterModel = null;
            vw_invt330s detailModel = null;
            List<vw_invt330s> detailList = null;
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt330>();
                if (masterModel.ifaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.ifa07), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_invt330s>();
                //檢查撥出倉別的數量是否足夠做出庫
                detailList = TabDetailList[0].DtSource.ToList<vw_invt330s>();
                var listSumIfb =                         //依料號及倉庫做加總
                        from ifb in detailList
                        where ifb.ifb13 > 0
                        group ifb by new { ifb.ifb03, ifb.ifb07 } into ifb_sum
                        select new
                        {
                            ifb03 = ifb_sum.Key.ifb03,
                            ifb07 = ifb_sum.Key.ifb07,
                            ifb13 = ifb_sum.Sum(p => p.ifb13)
                        }
                    ;
                foreach (var sumIfb in listSumIfb)
                {
                    icc05 = BoInv.OfGetIcc05(sumIfb.ifb03, sumIfb.ifb07);
                    if (icc05 < sumIfb.ifb13)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", sumIfb.ifb03, icc05));
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
            ifa_tb ifaModel = null;
            ifb_tb ifbModel = null;
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
                ifaModel = DrMaster.ToItem<ifa_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    ifbModel = dr.ToItem<ifb_tb>();
                    //更新料件庫存量 icc_tb  --撥出倉
                    if (BoInv.OfUpdIcc05("1", ifbModel.ifb03, ifbModel.ifb07, ifbModel.ifb13, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //更新料件庫存量 icc_tb --撥入倉
                    if (BoInv.OfUpdIcc05("2", ifbModel.ifb03, ifbModel.ifb10, ifbModel.ifb13, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //刪除庫存交易歷史檔
                    if (BoInv.OfDelIna(ifbModel.ifb01, ifbModel.ifb02, "1", this.StrFormID, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //刪除庫存交易歷史檔
                    if (BoInv.OfDelIna(ifbModel.ifb01, ifbModel.ifb02, "2", this.StrFormID, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                }

                DrMaster["ifaconf"] = "N";
                DrMaster["ifacond"] = DBNull.Value;
                DrMaster["ifaconu"] = "";
                DrMaster["ifamodu"] = LoginInfo.UserNo;
                DrMaster["ifamodg"] = LoginInfo.DeptNo;
                DrMaster["ifamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                ifaModel = DrMaster.ToItem<ifa_tb>();
                WfSetDocPicture("", ifaModel.ifaconf, "", pbxDoc);
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
            vw_invt330 masterModel = null;
            List<vw_invt330s> detailList = null;
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt330>();
                if (masterModel.ifaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }

                if (masterModel.ifa09 != "N")
                {
                    WfShowErrorMsg("單據已撥入!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.ifa07), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                //檢查在途倉別的數量是否足夠做出庫
                detailList = TabDetailList[0].DtSource.ToList<vw_invt330s>();
                var listSumIfb =                         //依料號及倉庫做加總
                        from ifb in detailList
                        where ifb.ifb13 > 0
                        group ifb by new { ifb.ifb03, ifb.ifb10 } into ifb_sum
                        select new
                        {
                            ifb03 = ifb_sum.Key.ifb03,
                            ifb10 = ifb_sum.Key.ifb10,
                            ifb13 = ifb_sum.Sum(p => p.ifb13)
                        }
                    ;
                foreach (var sumIfb in listSumIfb)
                {
                    icc05 = BoInv.OfGetIcc05(sumIfb.ifb03, sumIfb.ifb10);
                    if (icc05 < sumIfb.ifb13)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", sumIfb.ifb03, icc05));
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
            vw_invt330 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_invt330>();

                if (masterModel.ifaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認,不可作廢!");
                    WfRollback();
                    return;
                }

                if (masterModel.ifaconf == "N")//走作廢
                {

                    DrMaster["ifaconf"] = "X";
                    DrMaster["ifaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.ifaconf == "X")
                {
                    DrMaster["ifaconf"] = "N";
                    DrMaster["ifaconu"] = "";
                }
                DrMaster["ifamodu"] = LoginInfo.UserNo;
                DrMaster["ifamodg"] = LoginInfo.DeptNo;
                DrMaster["ifamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_invt330>();
                WfSetDocPicture("", masterModel.ifaconf, masterModel.ifaconf, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion
    }
}
