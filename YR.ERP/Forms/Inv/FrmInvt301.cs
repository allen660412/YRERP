/* 程式名稱: 庫存異動領發料處理作業
   系統代號: invt301
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
    public partial class FrmInvt301 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;

        baa_tb Baa_tb = null;
        bek_tb Bek = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmInvt301()
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
            this.StrFormID = "invt301";

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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("iga01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "igasecu";
                TabMaster.GroupColumn = "igasecg";

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
                sourceList = BoInv.OfGetIgaconfKVPList();
                WfSetUcomboxDataSource(ucb_igaconf, sourceList);
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

            this.TabDetailList[0].TargetTable = "igb_tb";
            this.TabDetailList[0].ViewTable = "vw_invt301s";
            keyParm = new SqlParameter("igb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "iga01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_invt301 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_invt301>();
                    WfSetDocPicture("", masterModel.igaconf, "", pbxDoc);
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

                    WfSetControlReadonly(new List<Control> { ute_igacreu, ute_igacreg, udt_igacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_igamodu, ute_igamodg, udt_igamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_igasecu, ute_igasecg }, true);

                    WfSetControlReadonly(new List<Control> { ute_iga01_c, ute_iga03_c, ute_iga04_c }, true);
                    WfSetControlReadonly(new List<Control> { ucb_igaconf, udt_igacond, ute_igaconu }, true);
                    //WfSetControlReadonly(new List<Control> { ute_igaconf,  ute_igaconu }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_iga01, true);                        
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
                                columnName == "igb03" ||
                                columnName == "igb05" ||
                                columnName == "igb06" ||
                                columnName == "igb09" ||
                                columnName == "igb10" 
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "igb02")
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
                vw_invt301 masterModel = null;
                vw_invt301s detailModel = null;
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_invt301
                if (pDr.Table.Prefix.ToLower() == "vw_invt301")
                {
                    switch (pColName.ToLower())
                    {
                        case "iga01"://異動單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "inv"));
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
                        case "iga03"://申請人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "iga04"://申請部門
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

                #region 單身-pick vw_invt301s
                if (pDr.Table.Prefix.ToLower() == "vw_invt301s")
                {
                    masterModel = DrMaster.ToItem<vw_invt301>();
                    detailModel = pDr.ToItem<vw_invt301s>();
                    switch (pColName.ToLower())
                    {
                        case "igb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "igb06"://異動單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "igb09"://倉庫
                            if (GlobalFn.isNullRet(detailModel.igb03, "") == "")
                                WfShowPickUtility("p_icb1", messageModel);
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.igb03));
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
                pDr["iga00"] = "1";         //領料
                pDr["iga02"] = Today;
                pDr["iga03"] = LoginInfo.UserNo;
                pDr["iga03_c"] = LoginInfo.UserName;
                pDr["iga04"] = LoginInfo.DeptNo;
                pDr["iga04_c"] = LoginInfo.DeptName;
                pDr["igaconf"] = "N";
                pDr["igacomp"] = LoginInfo.CompNo;
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
                        pDr["igb02"] = WfGetMaxSeq(pDr.Table, "igb02");
                        pDr["igb05"] = 0;
                        pDr["igb07"] = 0;
                        pDr["igb08"] = 0;
                        pDr["igb12"] = 0;
                        pDr["igb13"] = 0;
                        pDr["igbcomp"] = LoginInfo.CompNo;
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
                Baa_tb = BoBas.OfGetBaaModel();
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
            vw_invt301 masterModel;
            try
            {
                Baa_tb = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_invt301>();
                if (masterModel.igaconf != "N")
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
            vw_invt301 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_invt301>();
                if (masterModel.igaconf != "N")
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
            vw_invt301 masterModel = null;
            vw_invt301s detailModel = null;
            List<vw_invt301s> listDetails = null;
            bab_tb babModel = null;
            UltraGrid uGrid = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt301>();
                if (e.Column.ToLower() != "iga01" && GlobalFn.isNullRet(DrMaster["iga01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false;
                }
                #region 單頭-pick vw_invt301
                if (e.Row.Table.Prefix.ToLower() == "vw_invt301")
                {
                    switch (e.Column.ToLower())
                    {
                        case "iga01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "inv", "10") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["iga01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "iga02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            var result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;

                        case "iga03"://申請人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["iga03_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["iga03_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "iga04"://申請部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["iga04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["iga04_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;


                    }
                }
                #endregion

                #region 單身-pick vw_invt301s
                if (e.Row.Table.Prefix.ToLower() == "vw_invt301s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_invt301s>();
                    listDetails = e.Row.Table.ToList<vw_invt301s>();
                    babModel = BoBas.OfGetBabModel(masterModel.iga01);

                    switch (e.Column.ToLower())
                    {
                        case "igb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            listDetails = e.Row.Table.ToList<vw_invt301s>();
                            iChkCnts = listDetails.Where(p => GlobalFn.isNullRet(p.igb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "igb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["igb04"] = "";//品名
                                e.Row["igb05"] = 0;//異動數量
                                e.Row["igb06"] = "";//異動單位
                                e.Row["igb11"] = "";//庫存單位
                                e.Row["igb12"] = 0;//庫存轉換率
                                e.Row["igb13"] = 0;//庫存數量
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }

                            if (WfSetIgb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;
                            WfSetDetailAmt(e.Row);
                            break;
                        case "igb05"://出貨數量
                            if (GlobalFn.varIsNull(detailModel.igb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["igb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detailModel.igb06))
                            {
                                WfShowErrorMsg("請先輸入異動單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sgb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入異動數量!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["igb05"]);
                                return false;
                            }
                            detailModel.igb05 = BoBas.OfGetUnitRoundQty(detailModel.igb06, detailModel.igb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.igb05;
                            e.Row["igb13"] = BoBas.OfGetUnitRoundQty(detailModel.igb11, detailModel.igb05 * detailModel.igb12); //轉換庫存數量(並四拾伍入)
                            WfSetDetailAmt(e.Row);
                            break;
                        case "igb06"://異動單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.igb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["igb03"]);
                                return false;
                            }
                            if (WfChkIgb06(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;

                            if (WfSetIgb06Relation(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;
                            break;

                        case "igb07":   //異動成本
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.igb07 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.igb07, Bek.bek03);
                            WfSetDetailAmt(e.Row);
                            break;

                        case "igb09"://異動倉庫
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_invt301 masterModel = null;
            vw_invt301s detailModel = null;
            bab_tb l_bab = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {

                masterModel = DrMaster.ToItem<vw_invt301>();
                if (!GlobalFn.varIsNull(masterModel.iga01))
                    l_bab = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.iga01, ""));
                #region 單頭資料檢查
                chkColName = "iga01";       //異動單號
                chkControl = ute_iga01;
                if (GlobalFn.varIsNull(masterModel.iga01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "iga02";       //異動日期
                chkControl = udt_iga02;
                if (GlobalFn.varIsNull(masterModel.iga02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.iga02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "iga03";       //申請人員
                chkControl = ute_iga03;
                if (GlobalFn.varIsNull(masterModel.iga03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "iga04";       //部門
                chkControl = ute_iga04;
                if (GlobalFn.varIsNull(masterModel.iga04))
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

                    detailModel = drTemp.ToItem<vw_invt301s>();
                    chkColName = "igb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.igb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "igb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.igb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "igb05";   //異動數量
                    #region igb05 異動數量
                    if (GlobalFn.varIsNull(detailModel.igb05) || detailModel.igb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    #endregion

                    chkColName = "igb06";   //異動單位
                    if (GlobalFn.varIsNull(detailModel.igb06))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "igb09";   //倉庫別
                    if (GlobalFn.varIsNull(detailModel.igb09))
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
            string iga01New, errMsg;
            vw_invt301 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt301>();
                if (FormEditMode == YREditType.新增)
                {
                    iga01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.iga01, ModuleType.inv, (DateTime)masterModel.iga02, out iga01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["iga01"] = iga01New;
                }
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["igasecu"] = LoginInfo.UserNo;
                        DrMaster["igasecg"] = LoginInfo.GroupNo;
                        DrMaster["igacreu"] = LoginInfo.UserNo;
                        DrMaster["igacreg"] = LoginInfo.DeptNo;
                        DrMaster["igacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["igamodu"] = LoginInfo.UserNo;
                        DrMaster["igamodg"] = LoginInfo.DeptNo;
                        DrMaster["igamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["igbcreu"] = LoginInfo.UserNo;
                            drDetail["igbcreg"] = LoginInfo.DeptNo;
                            drDetail["igbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["igbmodu"] = LoginInfo.UserNo;
                            drDetail["igbmodg"] = LoginInfo.DeptNo;
                            drDetail["igbmodd"] = Now;
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

                bt = new ButtonTool("Invr301");
                adoModel = BoAdm.OfGetAdoModel("invr301");
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
            vw_invt301 masterModel;
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
                    case "Invr301":
                        vw_invr301 l_vw_invr301;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_invt301>();
                        l_vw_invr301 = new vw_invr301();
                        l_vw_invr301.iga01 = masterModel.iga01;
                        l_vw_invr301.jump_yn = "Y";

                        FrmInvr301 rpInvr401 = new FrmInvr301(this.LoginInfo, l_vw_invr301, true, true);
                        rpInvr401.WindowState = FormWindowState.Minimized;
                        rpInvr401.ShowInTaskbar = false;
                        rpInvr401.Show();
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
        #region WfChkIgb06 檢查銷貨單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkIgb06(DataRow pDr, string pIgb06, string pBab08)
        {
            vw_invt301s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_invt301s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pIgb06, "")) == false)
                {
                    WfShowErrorMsg("無此異動單位!請確認");
                    return false;
                }
                //檢查是否有銷售對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.igb03, pIgb06, detailModel.igb11, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定異動單位對庫存單位的轉換率,請先設定!");
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

        #region WfSetIgb03Relation 設定料號關聯
        private bool WfSetIgb03Relation(string pIgb03, DataRow pDr)
        {
            ica_tb icaModel;
            try
            {
                icaModel = BoInv.OfGetIcaModel(pIgb03);
                if (icaModel == null)
                {
                    pDr["igb04"] = "";//品名
                    pDr["igb05"] = 0;//異動數量
                    pDr["igb06"] = "";//異動單位
                    pDr["igb11"] = "";//庫存單位
                    pDr["igb12"] = 0;//庫存轉換率
                    pDr["igb13"] = 0;//庫存數量
                }
                else
                {
                    //if (BoInv.OfGetUnitCovert(pIgb03, l_ica.ica08, l_ica.ica07, out ld_igb12) == false)
                    //{
                    //    WfShowMsg("未設定料號轉換,請先設定!");
                    //    return false;
                    //}
                    pDr["igb04"] = icaModel.ica02;//品名
                    pDr["igb05"] = 0;//異動數量
                    pDr["igb06"] = icaModel.ica07;//異動單位帶入庫存單位
                    pDr["igb11"] = icaModel.ica07;//庫存單位
                    pDr["igb12"] = 1;
                    pDr["igb13"] = 0;
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
        private bool WfSetDetailAmt(DataRow drSeb)
        {
            igb_tb igbModel;
            try
            {
                igbModel = drSeb.ToItem<igb_tb>();

                drSeb["igb08"] = igbModel.igb07 * igbModel.igb05;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetIgb06Relation 設定異動單位關聯
        //檢查前要先確認料號是否已輸入
        private bool WfSetIgb06Relation(DataRow pDr, string pIgb06, string pBab08)
        {
            vw_invt301s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_invt301s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pIgb06, "")) == false)
                {
                    WfShowErrorMsg("無此異動單位!請確認");
                    return false;
                }
                //取得是否有異動對庫存的轉換率
                dConvert = 0;
                if (BoInv.OfGetUnitCovert(detailModel.igb03, pIgb06, detailModel.igb11, out dConvert) == true)
                {
                    pDr["igb12"] = dConvert;
                    pDr["igb13"] = BoBas.OfGetUnitRoundQty(detailModel.igb11, detailModel.igb12 * dConvert); //轉換庫存數量(並四拾伍入)
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
            iga_tb igaModel = null;
            igb_tb igbModel = null;

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

                igaModel = DrMaster.ToItem<iga_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                igaModel.igaconf = "Y";
                igaModel.igacond = Today;
                igaModel.igaconu = LoginInfo.UserNo;

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    igbModel = dr.ToItem<igb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("2", igbModel.igb03, igbModel.igb09, igbModel.igb13, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                    //更新庫存交易歷史檔
                    if (BoInv.OfStockPost("invt301", igaModel, igbModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }

                DrMaster["igaconf"] = "Y";
                DrMaster["igacond"] = Today;
                DrMaster["igaconu"] = LoginInfo.UserNo;
                DrMaster["igamodu"] = LoginInfo.UserNo;
                DrMaster["igamodg"] = LoginInfo.DeptNo;
                DrMaster["igamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                igaModel = DrMaster.ToItem<iga_tb>();
                WfSetDocPicture("", igaModel.igaconf, "", pbxDoc);
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
            vw_invt301 masterModel = null;
            vw_invt301s detailModel = null;
            List<vw_invt301s> detailList = null;
            StringBuilder sbSql;
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt301>();
                if (masterModel.igaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.iga02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_invt301s>();
                //foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                //{
                //    listDetail = drTemp.ToItem<vw_invt301s>();
                //    if (WfChkSgb05(drTemp, listDetail) == false)
                //    {
                //        return false;
                //    }
                //}

                //檢查銷貨單的數量是否足夠做出庫
                detailList = TabDetailList[0].DtSource.ToList<vw_invt301s>();
                var listSumIgb =                         //依銷貨單的料號及倉庫做加總
                        from sgb in detailList
                        where sgb.igb13 > 0
                        group sgb by new { sgb.igb03, sgb.igb09 } into pgb_sum
                        select new
                        {
                            igb03 = pgb_sum.Key.igb03,
                            igb09 = pgb_sum.Key.igb09,
                            igb13 = pgb_sum.Sum(p => p.igb13)
                        }
                    ;
                foreach (var sumIgb in listSumIgb)
                {
                    icc05 = BoInv.OfGetIcc05(sumIgb.igb03, sumIgb.igb09);
                    if (icc05 < sumIgb.igb13)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", sumIgb.igb03, icc05));
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
            iga_tb igaModel = null;
            igb_tb igbModel = null;
            string errMsg;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)        //這裡會LOCK資料
                    return;
                if (WfLockMasterRow() == false)         //這裡會LOCK資料
                    return;

                WfSetBllTransaction();
                igaModel = DrMaster.ToItem<iga_tb>();
                
                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    igbModel = dr.ToItem<igb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("1", igbModel.igb03, igbModel.igb09, igbModel.igb13, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔
                    if (BoInv.OfDelIna(igbModel.igb01, igbModel.igb02, "2", out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                }

                DrMaster["igaconf"] = "N";
                DrMaster["igacond"] = DBNull.Value;
                DrMaster["igaconu"] = "";
                DrMaster["igamodu"] = LoginInfo.UserNo;
                DrMaster["igamodg"] = LoginInfo.DeptNo;
                DrMaster["igamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                igaModel = DrMaster.ToItem<iga_tb>();
                WfSetDocPicture("", igaModel.igaconf, "", pbxDoc);
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
            vw_invt301 masterModel = null;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt301>();
                if (masterModel.igaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }

                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.iga02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
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

        #region WfInvalid 作廢/作廢還原
        private void WfInvalid()
        {
            vw_invt301 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_invt301>();

                if (masterModel.igaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.igaconf == "N")//走作廢
                {

                    DrMaster["igaconf"] = "X";
                    DrMaster["igaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.igaconf == "X")
                {
                    DrMaster["igaconf"] = "N";
                    DrMaster["igaconu"] = "";
                }
                DrMaster["igamodu"] = LoginInfo.UserNo;
                DrMaster["igamodg"] = LoginInfo.DeptNo;
                DrMaster["igamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_invt301>();
                WfSetDocPicture("", masterModel.igaconf, masterModel.igaconf, pbxDoc);
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
