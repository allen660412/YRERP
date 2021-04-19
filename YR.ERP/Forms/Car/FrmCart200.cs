/* 程式名稱: 收款單維護作業
   系統代號: cart200
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
using Infragistics.Win.UltraWinTabControl;
using YR.ERP.BLL.MSSQL.Gla;
using YR.ERP.BLL.MSSQL.Car;


namespace YR.ERP.Forms.Car
{
    public partial class FrmCart200 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        CarBLL BoCar = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;
        StpBLL BoStp = null;
        Glat200BLL BoGlat200 = null;
        bek_tb bekOrignalModel = null;      //本幣資料
        List<KeyValuePair<string, string>> Cfb04cList = null;
        List<KeyValuePair<string, string>> Cfb04dList = null;
        #endregion

        #region 建構子
        public FrmCart200()
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
            this.StrFormID = "cart200";

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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("cfa01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "cfasecu";
                TabMaster.GroupColumn = "cfasecg";
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

            BoCar = new CarBLL(BoMaster.OfGetConntion());
            BoBas = new BasBLL(BoMaster.OfGetConntion());
            BoStp = new StpBLL(BoMaster.OfGetConntion());
            BoInv = new InvBLL(BoMaster.OfGetConntion());
            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
            BoGlat200 = new Glat200BLL(BoMaster.OfGetConntion());
            return;
        }
        #endregion

        #region WfAfterInitialForm BLL載入後,供後續載入一次性的model或資料
        protected override bool WfAfterInitialForm()
        {
            try
            {
                bekOrignalModel = BoBas.OfGetBekModel(BaaModel.baa04);

                TabDetailList[0].UGrid.InitializeRow += UGrid_InitializeRow;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void UGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (GlobalFn.isNullRet(e.Row.Cells["cfb03"].Value, "") == "")
                return;
            else if (GlobalFn.isNullRet(e.Row.Cells["cfb03"].Value, "") == "1")
                WfSetGridValueList(e.Row.Cells["cfb04"], Cfb04dList);
            else
                WfSetGridValueList(e.Row.Cells["cfb04"], Cfb04cList);
        }
        #endregion

        #region WfSetBllTransaction 以bomaster 註冊transaction至其他 bll
        protected override void WfSetBllTransaction()
        {
            try
            {
                if (BoMaster.TRAN != null)
                {
                    BoCar.TRAN = BoMaster.TRAN;
                    BoBas.TRAN = BoMaster.TRAN;
                    BoStp.TRAN = BoMaster.TRAN;
                    BoInv.TRAN = BoMaster.TRAN;
                    BoAdm.TRAN = BoMaster.TRAN;
                    BoGlat200.TRAN = BoMaster.TRAN;
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
            StringBuilder sbSql;
            try
            {

                ////單據確認
                sourceList = BoCar.OfGetCfaconfKVPList();
                WfSetUcomboxDataSource(ucb_cfaconf, sourceList);

                ////單據狀態
                sourceList = BoCar.OfGetCfastatKVPList();
                WfSetUcomboxDataSource(ucb_cfastat, sourceList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniDetailComboSource 處理grid下拉選單
        protected override void WfIniDetailComboSource(int pTabIndex)
        {
            UltraGrid uGrid;
            UltraGridColumn ugc;
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                switch (pTabIndex)
                {
                    case 0:
                        uGrid = TabDetailList[pTabIndex].UGrid;
                        //借/貸
                        sourceList = new List<KeyValuePair<string, string>>();
                        sourceList.Add(new KeyValuePair<string, string>("1", "1.借"));
                        sourceList.Add(new KeyValuePair<string, string>("2", "2.貸"));
                        ugc = uGrid.DisplayLayout.Bands[0].Columns["cfb03"];
                        WfSetGridValueList(ugc, sourceList);

                        Cfb04dList = new List<KeyValuePair<string, string>>();
                        Cfb04dList.Add(new KeyValuePair<string, string>("1", "1.一般"));
                        Cfb04dList.Add(new KeyValuePair<string, string>("2", "2.票據"));
                        Cfb04dList.Add(new KeyValuePair<string, string>("3", "3.待抵"));

                        Cfb04cList = new List<KeyValuePair<string, string>>();
                        Cfb04cList.Add(new KeyValuePair<string, string>("1", "1.應收"));
                        Cfb04cList.Add(new KeyValuePair<string, string>("2", "2.溢收"));

                        ugc = uGrid.DisplayLayout.Bands[0].Columns["cfb04"];//類型 預帶借方類型
                        WfSetGridValueList(ugc, Cfb04dList);
                        break;
                }
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

            this.TabDetailList[0].TargetTable = "cfb_tb";
            this.TabDetailList[0].ViewTable = "vw_cart200s";
            keyParm = new SqlParameter("cfb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "cfa01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_cart200 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_cart200>();
                    WfSetDocPicture("", masterModel.cfaconf, masterModel.cfastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.cfa07) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        //BekTbModel = BoBas.OfGetBekModel(masterModel.cfa07);
                        //if (BekTbModel == null)
                        //{
                        //    WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.cfa07));
                        //}
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

                    WfSetControlReadonly(new List<Control> { ute_cfacreu, ute_cfacreg, udt_cfacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_cfamodu, ute_cfamodg, udt_cfamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_cfasecu, ute_cfasecg }, true);

                    if (FormEditMode == YREditType.新增)
                    {
                        //WfSetControlReadonly(ucb_cea00, false);
                    }
                    else
                    {
                        //WfSetControlReadonly(ucb_cea00, true);
                    }


                    if (GlobalFn.isNullRet(masterModel.cfa01, "") != "")
                    {
                        WfSetControlReadonly(ute_cfa01, true);
                        //WfSetSfa01RelReadonly(GlobalFn.isNullRet(masterModel.sfa01, ""));
                    }

                    WfSetControlReadonly(new List<Control> { ute_cfa01_c, ute_cfa03_c, ute_cfa04_c, ute_cfa05_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_cfa08, ute_cfa09, ute_cfa10, ute_cfa11, ute_cfa12 }, true);
                    WfSetControlReadonly(new List<Control> { ucb_cfaconf, ucb_cfastat }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    //if (FormEditMode == YREditType.修改)
                    //{
                    //    WfSetControlReadonly(ute_cfa01, true);
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

        //#region WfQueryOkEnd 查詢完畢時--通常做為後續資料處理
        //protected override bool WfQueryOkEnd()
        //{
        //    UltraGridCell ugc = null;
        //    try
        //    {
        //        foreach (UltraGridRow ugr in TabDetailList[0].UGrid.Rows)
        //        {
        //            ugc = ugr.Cells["cfb04"];
        //            if (ugr.Cells["cfb03"].Value.ToString() == "1")
        //                WfSetGridValueList(ugc, Cfb04dList);
        //            else
        //                WfSetGridValueList(ugc, Cfb04cList);
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
        //#endregion

        #region WfSetDetailDisplayMode 新增修改時的明細列可輸入處理,需要每筆資料列微調整時再使用
        protected override void WfSetDetailDisplayMode(int pCurTabDetail, UltraGridRow pUgr, DataRow pDr)
        {
            string columnName;
            bab_tb babModel;
            vw_cart200s detailModel = null;
            gba_tb gbaModel = null;
            try
            {
                switch (pCurTabDetail)
                {
                    case 0:
                        detailModel = pDr.ToItem<vw_cart200s>();
                        foreach (UltraGridCell ugc in pUgr.Cells)
                        {
                            columnName = ugc.Column.Key.ToLower();
                            if (columnName == "cfb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }
                            //先控可以輸入的
                            if (
                                columnName == "cfb03" ||
                                columnName == "cfb04" ||
                                columnName == "cfb09" ||
                                columnName == "cfb10" ||
                                columnName == "cfb11"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (
                                columnName == "cfb05"
                                )
                            {

                                if (
                                    (detailModel.cfb03 == "1" && detailModel.cfb04 == "3") ||
                                    (detailModel.cfb03 == "2" && detailModel.cfb04 == "1") ||
                                    (detailModel.cfb03 == "2" && detailModel.cfb04 == "2")
                                    )
                                {
                                    WfSetControlReadonly(ugc, false);
                                    continue;
                                }
                            }

                            if (columnName == "cfb13" && !GlobalFn.varIsNull(detailModel.cfb11))
                            {
                                gbaModel = BoGlat200.OfGetGbaModel(detailModel.cfb11);
                                if (GlobalFn.isNullRet(gbaModel.gba09, "") == "Y")
                                {
                                    WfSetControlReadonly(ugc, false);
                                    continue;
                                }
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["cfa02"] = Today;
                pDr["cfa04"] = LoginInfo.UserNo;
                pDr["cfa04_c"] = LoginInfo.UserName;
                pDr["cfa05"] = LoginInfo.DeptNo;
                pDr["cfa05_c"] = LoginInfo.DeptName;
                pDr["cfa07"] = BaaModel.baa04;
                pDr["cfa08"] = 0;
                pDr["cfa09"] = 0;
                pDr["cfa10"] = 0;
                pDr["cfa11"] = 0;
                pDr["cfaconf"] = "N";
                pDr["cfastat"] = "0";
                pDr["cfaprno"] = "0";
                pDr["cfacomp"] = LoginInfo.CompNo;
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
            vw_cart200 masterModel = null;
            try
            {

                switch (pCurTabDetail)
                {
                    case 0:
                        masterModel = DrMaster.ToItem<vw_cart200>();
                        pDr["cfb02"] = WfGetMaxSeq(pDr.Table, "cfb02");
                        pDr["cfb03"] = "1"; //借
                        pDr["cfb04"] = "1"; //一般
                        pDr["cfb07"] = masterModel.cfa07;
                        pDr["cfb08"] = 1;
                        pDr["cfb09"] = 0;
                        pDr["cfb10"] = 0;
                        pDr["cfbcomp"] = LoginInfo.CompNo;
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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                MessageInfo messageModel = new MessageInfo();
                vw_cart200 masterModel = null;
                vw_cart200s detailModel = null;
                List<SqlParameter> sqlParmList = null;
                #region 單頭-pick vw_cart200
                if (pDr.Table.Prefix.ToLower() == "vw_cart200")
                {
                    masterModel = DrMaster.ToItem<vw_cart200>();

                    switch (pColName.ToLower())
                    {
                        case "cfa01"://收款單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@cac03", "car"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@cac04", "31"));
                            WfShowPickUtility("p_cac1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["cac01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "cfa03"://客戶編號
                            WfShowPickUtility("p_sca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sca01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;
                        case "cfa04"://人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;
                        case "cfa05"://部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "cfa07"://幣別
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
                
                #region 單身-pick vw_cart200s
                if (pDr.Table.Prefix.ToLower() == "vw_cart200s")
                {
                    masterModel = DrMaster.ToItem<vw_cart200>();
                    detailModel = pDr.ToItem<vw_cart200s>();
                    switch (pColName.ToLower())
                    {
                        case "cfb05":    //參考單號
                            if (detailModel.cfb03 == "1")        //借方
                            {
                                switch (detailModel.cfb04)
                                {
                                    case "1":

                                        break;

                                    case "3":   //待抵帳款
                                        messageModel.StrWhereAppend = string.Format(" AND cea03=@cea03");
                                        sqlParmList = new List<SqlParameter>();
                                        sqlParmList.Add(new SqlParameter("cea03", masterModel.cfa03));
                                        messageModel.ParamSearchList = sqlParmList;
                                        WfShowPickUtility("p_cea4", messageModel);
                                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                        {
                                            if (messageModel.DataRowList.Count > 0)
                                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["cea01"], "");
                                            else
                                                pDr[pColName] = "";
                                        }
                                        break;
                                }
                                
                            }
                            else if (detailModel.cfb03 == "2")   //貸方
                            {
                                switch (detailModel.cfb04)
                                {
                                    case "1":   //應收-沖應收帳款
                                        messageModel.StrWhereAppend = string.Format(" AND cea03=@cea03");
                                        sqlParmList = new List<SqlParameter>();
                                        sqlParmList.Add(new SqlParameter("cea03", masterModel.cfa03));
                                        messageModel.ParamSearchList = sqlParmList;
                                        WfShowPickUtility("p_cea2", messageModel);
                                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                        {
                                            if (messageModel.DataRowList.Count > 0)
                                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["cea01"], "");
                                            else
                                                pDr[pColName] = "";
                                        }
                                        break;
                                    case "2":   //溢收
                                        sqlParmList = new List<SqlParameter>();
                                        sqlParmList.Add(new SqlParameter("@cac03", "car"));
                                        sqlParmList.Add(new SqlParameter("@cac04", "22"));
                                        messageModel.ParamSearchList = sqlParmList;
                                        WfShowPickUtility("p_cac1", messageModel);
                                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                        {
                                            if (messageModel.DataRowList.Count > 0)
                                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["cac01"], "");
                                            else
                                                pDr[pColName] = "";
                                        }
                                        break;
                                }
                            }
                            break;
                        case "cfb07"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "cfb11"://會計科目
                            messageModel.StrWhereAppend = " AND gba06 in ('2','3')";
                            WfShowPickUtility("p_gba1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["gba01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "cfb13"://部門
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
            vw_cart200 masterModel;
            try
            {
                masterModel = DrMaster.ToItem<vw_cart200>();

                if (masterModel.cfaconf != "N")
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
            vw_cart200 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_cart200>();
                if (masterModel.cfaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可刪除!");
                    return false;
                }


                ////還需檢查出貨單
                //if (WfChkSgaExists(masterModel.sfa01) == true)
                //{
                //    WfShowErrorMsg("已有出貨資料!不可取消確認!");
                //    return false;
                //}

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
            int chkCnts = 0;
            vw_cart200 masterModel = null;
            vw_cart200s detailModel = null;
            bek_tb bekModel = null;
            List<vw_cart200s> detailList = null;
            UltraGrid uGrid = null;
            UltraGridRow uGridRowActive = null;
            gba_tb gbaModel = null;
            cac_tb cacModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_cart200>();

                #region 單頭 vw_cart100
                if (e.Row.Table.Prefix.ToLower() == "vw_cart200")
                {
                    switch (e.Column.ToLower())
                    {
                        case "cfa01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoCar.OfChkCacPKValid(GlobalFn.isNullRet(e.Value, ""), "car", "31") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["cfa01_c"] = BoCar.OfGetCac02(e.Value.ToString());
                            WfSetControlReadonly(ute_cfa01, true);
                            break;
                        case "cfa03"://客戶編號
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["cfa03_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkScaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此客戶資料,請檢核!");
                                return false;
                            }
                            //WfSetCea03Relation(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "cfa04"://業務人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["cfa04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["cfa04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "cfa05"://業務部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["cfa05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["cfa05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;


                        case "cfa07"://幣別
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBekPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此幣別");
                                return false;
                            }
                            //BekTbModel = BoBas.OfGetBekModel(e.Value.ToString());
                            break;


                        //case "cea21"://應收科目類別
                        //    if (GlobalFn.isNullRet(e.Value, "") == "")
                        //    {
                        //        e.Row["cea21_c"] = "";
                        //        return true;
                        //    }
                        //    cbaModel = BoCar.OfGetCbaModel(e.Value.ToString());
                        //    if (cbaModel == null)
                        //    {
                        //        WfShowErrorMsg("無此應收科目類別,請檢核!");
                        //        return false;
                        //    }
                        //    e.Row["cea21_c"] = cbaModel.cba02;
                        //    if (!GlobalFn.varIsNull(cbaModel.cba03))
                        //    {
                        //        e.Row["cea22"] = cbaModel.cba03;
                        //        e.Row["cea22_c"] = BoGlat200.OfGetGba02(cbaModel.cba03);
                        //    }
                        //    break;


                    }
                }
                #endregion

                #region 單身 vw_cart200s
                if (e.Row.Table.Prefix.ToLower() == "vw_cart200s")
                {
                    uGrid = sender as UltraGrid;
                    uGridRowActive = uGrid.ActiveRow;
                    detailModel = e.Row.ToItem<vw_cart200s>();
                    detailList = e.Row.Table.ToList<vw_cart200s>();
                    switch (e.Column.ToLower())
                    {
                        case "cfb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            detailList = e.Row.Table.ToList<vw_cart200s>();
                            chkCnts = detailList.Where(p => GlobalFn.isNullRet(p.cfb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (chkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;
                        case "cfb03":
                            if (e.Value.ToString() == "1") //借
                            {
                                WfSetGridValueList(uGridRowActive.Cells["cfb04"], Cfb04dList);
                            }
                            else//貸
                            {
                                WfSetGridValueList(uGridRowActive.Cells["cfb04"], Cfb04cList);
                            }
                            e.Row["cfb04"] = "1";
                            e.Row["cfb05"] = DBNull.Value;
                            e.Row["cfb06"] = DBNull.Value;
                            e.Row["cfb09"] = 0;
                            e.Row["cfb10"] = 0;
                            WfSetTotalAmt();
                            if ((detailModel.cfb03 == "1" && detailModel.cfb04 == "3") ||
                                (detailModel.cfb03 == "2" && detailModel.cfb04 == "1") ||
                                (detailModel.cfb03 == "2" && detailModel.cfb04 == "2")
                                )
                            {
                                WfSetControlReadonly(uGridRowActive.Cells["cfb05"], false);
                                WfSetControlReadonly(uGridRowActive.Cells["cfb06"], true);
                            }
                            else
                            {
                                WfSetControlReadonly(uGridRowActive.Cells["cfb05"], true);
                                WfSetControlReadonly(uGridRowActive.Cells["cfb06"], true);
                            }
                            break;
                        case "cfb04":   //類別
                            e.Row["cfb05"] = DBNull.Value;
                            e.Row["cfb06"] = DBNull.Value;
                            e.Row["cfb09"] = 0;
                            e.Row["cfb10"] = 0;
                            WfSetTotalAmt();
                            if ((detailModel.cfb03 == "1" && detailModel.cfb04 == "3") ||
                                (detailModel.cfb03 == "2" && detailModel.cfb04 == "1") ||
                                (detailModel.cfb03 == "2" && detailModel.cfb04 == "2")
                                )
                            {
                                WfSetControlReadonly(uGridRowActive.Cells["cfb05"], false);
                                WfSetControlReadonly(uGridRowActive.Cells["cfb06"], true);
                                WfItemChkForceFocus(uGrid, uGridRowActive.Cells["cfb05"]);
                            }
                            else
                            {
                                WfSetControlReadonly(uGridRowActive.Cells["cfb05"], true);
                                WfSetControlReadonly(uGridRowActive.Cells["cfb06"], true);
                            }
                            break;
                            
                        case "cfb05":
                            #region 參考單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["cfb09"] = 0;
                                e.Row["cfb10"] = 0;
                                WfSetTotalAmt();
                                return true;
                            }

                            if (detailModel.cfb03 == "1")  //借方
                            {
                                switch (detailModel.cfb04)
                                {
                                    case "1":   //一般
                                        break;
                                    case "2":   //票據
                                        break;
                                    case "3":   //待抵
                                        if (WfChkCfb04D3("1", masterModel, e.Row, detailModel, detailList) == false)
                                            return false;
                                        WfSetTotalAmt();
                                        break;
                                }
                            }
                            else        //貸方
                            {
                                switch (detailModel.cfb04)
                                {
                                    case "1":   //應收
                                        if (WfChkCfb04C1("1", masterModel, e.Row, detailModel, detailList) == false)
                                            return false;
                                        WfSetTotalAmt();
                                        break;
                                    case "2":   //溢收
                                        if (e.Value.ToString().Length != BaaModel.baa06)
                                        {
                                            WfShowErrorMsg("單別位數不符!");
                                            return false;
                                        }
                                        cacModel = BoCar.OfGetCacModel(e.Value.ToString());
                                        if (cacModel == null)
                                        {
                                            WfShowErrorMsg("查無此單別!");
                                            return false;
                                        }
                                        if (cacModel.cacvali != "Y")
                                        {
                                            WfShowErrorMsg("非有效單別!");
                                            return false;
                                        }
                                        if (cacModel.cac04 != "22")
                                        {
                                            WfShowErrorMsg("非溢收單別!");
                                            return false;
                                        }
                                        break;
                                }
                            }
                            break;
                            #endregion
                        case "cfb06":   //項次
                            if (!GlobalFn.varIsNull(detailModel.cfb05))
                            {
                                WfShowErrorMsg("請先輸入參考單號!");
                                return false;
                            }
                            break;


                        case "cfb07":   //幣別
                            if (GlobalFn.varIsNull(e.Value.ToString()))
                            {
                                WfShowErrorMsg("請輸入幣別!");
                                return false;
                            }
                            if (!GlobalFn.varIsNull(masterModel.cfa07) &&
                                    GlobalFn.isNullRet(masterModel.cfa07, "") != GlobalFn.isNullRet(detailModel.cfb07, ""))
                            {
                                WfShowErrorMsg("幣別需相同,請檢核!");
                                return false;
                            }
                            if (BoBas.OfChkBekPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此幣別,請檢核!");
                                return false;
                            }
                            break;
                        case "cfb08":   //匯率
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入金額!");
                                return false;
                            }
                            bekModel = BoBas.OfGetBekModel(detailModel.cfb07);
                            if (bekModel == null)
                            {
                                WfShowErrorMsg("無此幣別資料!");
                                return false;
                            }
                            detailModel.cfb09 = GlobalFn.Round(detailModel.cfb09, bekModel.bek04);
                            WfSetCfb10(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "cfb09":   //原幣金額                            
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入金額!");
                                return false;
                            }
                            if (GlobalFn.varIsNull(detailModel.cfb07))
                            {
                                WfShowErrorMsg("請輸入幣別!");
                                return false;
                            }

                            bekModel = BoBas.OfGetBekModel(detailModel.cfb07);
                            if (bekModel == null)
                            {
                                WfShowErrorMsg("查無幣別資料!");
                                return false;
                            }
                            e.Value = GlobalFn.Round(detailModel.cfb09, bekModel.bek04);
                            e.Row[e.Column] = e.Value;

                            #region 依參考單號做檢查
                            if (detailModel.cfb03 == "1")  //借方
                            {
                                switch (detailModel.cfb04)
                                {
                                    case "1":   //一般
                                        break;
                                    case "2":   //票據
                                        break;
                                    case "3":   //待抵
                                        break;
                                }
                            }
                            else        //貸方
                            {
                                switch (detailModel.cfb04)
                                {
                                    case "1":   //應收
                                        if (WfChkCfb09Cfb10C1(e.Column, masterModel, e.Row, detailModel, detailList) == false)
                                            return false;
                                        break;
                                    case "2":   //溢收
                                        break;
                                }
                            }
                            #endregion
                            WfSetCfb10(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "cfb10":   //原幣金額                            
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入金額!");
                                return false;
                            }
                            if (GlobalFn.varIsNull(detailModel.cfb07))
                            {
                                WfShowErrorMsg("請輸入幣別!");
                                return false;
                            }
                            e.Value = GlobalFn.Round(detailModel.cfb10, bekOrignalModel.bek04);
                            e.Row[e.Column] = e.Value;

                            #region 依參考單號做檢查
                            if (detailModel.cfb03 == "1")  //借方
                            {
                                switch (detailModel.cfb04)
                                {
                                    case "1":   //一般
                                        break;
                                    case "2":   //票據
                                        break;
                                    case "3":   //待抵
                                        break;
                                }
                            }
                            else        //貸方
                            {
                                switch (detailModel.cfb04)
                                {
                                    case "1":   //應收
                                        if (WfChkCfb09Cfb10C1(e.Column, masterModel, e.Row, detailModel, detailList) == false)
                                            return false;
                                        break;
                                    case "2":   //溢收
                                        break;
                                }
                            }
                            #endregion
                            WfSetTotalAmt();
                            break;

                        case "cfb11"://會計科目
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["cfb11_c"] = "";
                                return true;
                            }
                            gbaModel = BoGlat200.OfGetGbaModel(e.Value.ToString());
                            if (gbaModel == null)
                            {
                                WfShowErrorMsg("無此會計科目,請檢核!");
                                return false;
                            }
                            if (gbaModel.gbavali != "Y")
                            {
                                WfShowErrorMsg("此會計科目已失效,請檢核!");
                                return false;
                            }
                            if (gbaModel.gba06 != "2" && gbaModel.gba06 != "3")
                            {
                                WfShowErrorMsg("會計科目非明細或獨立科目,請檢核!");
                                return false;
                            }
                            e.Row["cfb11_c"] = gbaModel.gba02;
                            if (gbaModel.gba09 == "Y")
                            {
                                WfSetControlReadonly(uGridRowActive.Cells["cfb13"], false);
                            }
                            else
                            {
                                WfSetControlReadonly(uGridRowActive.Cells["cfb13"], true);
                                e.Row["cfb13"] = "";
                            }
                            break;

                        case "cfb13"://部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
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
            vw_cart200 masterModel = null;
            vw_cart200s detailModel = null;
            List<vw_cart200s> detailList = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            string sql = "";
            List<SqlParameter> sqlParmList = null;
            sgb_tb sgbModel = null;
            decimal otherDocQty = 0, thisDocQty = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_cart200>();
                if (!GlobalFn.varIsNull(masterModel.cfa01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.cfa01, ""));
                #region 單頭資料檢查
                chkColName = "cfa01";       //收款編號
                chkControl = ute_cfa01;
                if (GlobalFn.varIsNull(masterModel.cfa01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cfa02";       //日期
                chkControl = udt_cfa02;
                if (GlobalFn.varIsNull(masterModel.cfa02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cfa03";       //客戶編號
                chkControl = ute_cfa03;
                if (GlobalFn.varIsNull(masterModel.cfa03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cfa04";       //業務人員
                chkControl = ute_cfa04;
                if (GlobalFn.varIsNull(masterModel.cfa04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cfa05";       //業務部門
                chkControl = ute_cfa05;
                if (GlobalFn.varIsNull(masterModel.cfa05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
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
                detailList = TabDetailList[iChkDetailTab].DtSource.ToList<vw_cart200s>();
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    detailModel = drTemp.ToItem<vw_cart200s>();
                    chkColName = "cfb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.cfb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "cfb07";   //幣別
                    if (GlobalFn.varIsNull(detailModel.cfb07))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "cfb08";
                    #region cfb08 匯率
                    if (GlobalFn.varIsNull(detailModel.cfb08) || detailModel.cfb08 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    if (detailModel.cfb07 == BaaModel.baa04 && detailModel.cfb08 != 1)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = "匯率只能為1!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "cfb09";   //原幣金額
                    if (GlobalFn.varIsNull(detailModel.cfb09) || detailModel.cfb09 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "cfb10";   //本幣金額
                    if (GlobalFn.varIsNull(detailModel.cfb10) || detailModel.cfb10 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "cfb11";   //單位
                    if (GlobalFn.varIsNull(detailModel.cfb11))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    //這裡處理空白檢查後的相關邏輯檢查
                    chkColName = "cfb05";
                    #region cfb05 參考單號
                    if ((detailModel.cfb03 == "2" && detailModel.cfb04 == "1"))
                    {
                        if (GlobalFn.varIsNull(detailModel.cfb05))
                        {
                            this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                            msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                            msg += "不可為空白";
                            WfShowErrorMsg(msg);
                            WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                            return false;
                        }

                        if (WfChkCfb09Cfb10C1("cfb09", masterModel, drTemp, detailModel, detailList) == false)
                        {
                            WfFindErrUltraGridCell(uGrid, drTemp, "cfb09");
                            return false;
                        }

                        if (WfChkCfb09Cfb10C1("cfb09", masterModel, drTemp, detailModel, detailList) == false)
                        {
                            WfFindErrUltraGridCell(uGrid, drTemp, "cfb10");
                            return false;
                        }
                    }
                    if ((detailModel.cfb03 == "2" && detailModel.cfb04 == "2")) //貸-溢收 
                    {
                        if (GlobalFn.varIsNull(detailModel.cfb05))
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

                #endregion

                }
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
            string cfa01New, errMsg;
            vw_cart200 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_cart200>();
                if (FormEditMode == YREditType.新增)
                {
                    cfa01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.cfa01, ModuleType.car, (DateTime)masterModel.cfa02, out cfa01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["cfa01"] = cfa01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["cfasecu"] = LoginInfo.UserNo;
                        DrMaster["cfasecg"] = LoginInfo.GroupNo;
                        DrMaster["cfacreu"] = LoginInfo.UserNo;
                        DrMaster["cfacreg"] = LoginInfo.DeptNo;
                        DrMaster["cfacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["cfamodu"] = LoginInfo.UserNo;
                        DrMaster["cfamodg"] = LoginInfo.DeptNo;
                        DrMaster["cfamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["cfbcreu"] = LoginInfo.UserNo;
                            drDetail["cfbcreg"] = LoginInfo.DeptNo;
                            drDetail["cfbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["cfbmodu"] = LoginInfo.UserNo;
                            drDetail["cfbmodg"] = LoginInfo.DeptNo;
                            drDetail["cfbmodd"] = Now;
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
            string sqlSelect, deleteGeaSql = "", deleteGebSql = "";
            int chkCnts = 0;
            List<SqlParameter> sqlParmList = null;
            cfa_tb masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<cfa_tb>();

                sqlSelect = @"SELECT COUNT(1) FROM gea_tb
                            WHERE gea01=@gea01 AND gea02='AR'
                                AND gea03=2
                            ";
                deleteGeaSql = @"DELETE FROM gea_tb 
                                    WHERE gea01=@gea01 AND gea02='AR'  AND gea03=2
                                    ";
                deleteGebSql = @"DELETE FROM geb_tb 
                                    WHERE geb01=@geb01 AND geb02='AR'  AND geb03=2
                                    ";

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gea01", masterModel.cfa01));
                chkCnts = GlobalFn.isNullRet(BoCar.OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    if (WfShowConfirmMsg("分錄底稿已存在,是否重新新增?") == DialogResult.Yes)
                    {
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gea01", masterModel.cfa01));
                        BoCar.OfExecuteNonquery(deleteGeaSql, sqlParmList.ToArray());

                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@geb01", masterModel.cfa01));
                        BoCar.OfExecuteNonquery(deleteGebSql, sqlParmList.ToArray());
                        var resultList = BoGlat200.OfGenGeaByCfa(masterModel.cfa01, LoginInfo);
                        if (resultList != null && resultList.Count > 0)
                        {
                            WfShowErrorMsg(resultList[0].Message);
                            return false;
                        }
                    }
                }
                else
                {
                    if (WfShowConfirmMsg("是否新增分錄底稿?") == DialogResult.Yes)
                    {
                        var resultList = BoGlat200.OfGenGeaByCfa(masterModel.cfa01, LoginInfo);
                        if (resultList != null && resultList.Count > 0)
                        {
                            WfShowErrorMsg(resultList[0].Message);
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


                bt = new ButtonTool("GenGea");
                bt.SharedProps.Caption = "產生分錄底稿";
                bt.SharedProps.Category = "Action";
                buttonList.Add(bt);

                bt = new ButtonTool("glat200");
                bt.SharedProps.Caption = "分錄底稿維護作業";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("GenVoucher");
                bt.SharedProps.Caption = "拋轉傳票";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("UndoGenVoucher");
                bt.SharedProps.Caption = "傳票拋轉還原";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("glat300");
                bt.SharedProps.Caption = "傳票明細查詢";
                bt.SharedProps.Category = "action";
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
            StringBuilder sbSql;
            try
            {
                vw_cart200 masterModel;
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
                    //case "Invalid":
                    //    if (FormEditMode != YREditType.NA)
                    //        return;
                    //    WfInvalid();
                    //    break;
                    case "GenGea":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        WfGenGea(DrMaster["cfa01"].ToString());
                        break;

                    case "glat200":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_cart200>();
                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND gea01='{0}'", masterModel.cfa01));
                        sbSql.AppendLine(string.Format(" AND gea02='AR' AND gea03=2 AND gea04=1 "));
                        WfShowForm("glat200", false, new object[] { "glat200", this.LoginInfo, sbSql.ToString() });
                        break;

                    case "GenVoucher":  //拋轉傳票
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        if (WfGenVoucher() == false)
                            return;
                        break;

                    case "UndoGenVoucher":  //傳票拋轉還原
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        if (WfUndoGenVoucher() == false)
                            return;
                        break;

                    case "glat300":     //傳票明細查詢
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;

                        masterModel = DrMaster.ToItem<vw_cart200>();
                        if (GlobalFn.varIsNull(masterModel.cfa12))
                            return;

                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND gfa01='{0}'", masterModel.cfa12));
                        WfShowForm("glat300", false, new object[] { "cart200", this.LoginInfo, sbSql.ToString() });
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
        #region WfSetTotalAmt 更新 cfa08、cfa09、cfa10、cfa11
        private void WfSetTotalAmt()
        {
            decimal cfa08 = 0, cfa09 = 0, cfa10 = 0, cfa11 = 0;
            vw_cart200s detailModel = null;
            try
            {
                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drDetail.ToItem<vw_cart200s>();
                    if (detailModel.cfb03 == "1")//借方
                    {
                        cfa08 += detailModel.cfb09;
                        cfa10 += detailModel.cfb10;
                    }
                    else
                    {
                        cfa09 += detailModel.cfb09;
                        cfa11 += detailModel.cfb10;
                    }
                    DrMaster["cfa08"] = cfa08;
                    DrMaster["cfa09"] = cfa09;
                    DrMaster["cfa10"] = cfa10;
                    DrMaster["cfa11"] = cfa11;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetCfb10 更新本幣金額
        private void WfSetCfb10(DataRow pDrDetail)
        {
            vw_cart200s detailModel = null;
            try
            {
                detailModel = pDrDetail.ToItem<vw_cart200s>();
                detailModel.cfb10 = detailModel.cfb08 * detailModel.cfb09;
                if (BaaModel.baa04 != detailModel.cfb07)  //幣別不同再考慮小數點進位
                {
                    detailModel.cfb10 = GlobalFn.Round(detailModel.cfb10, bekOrignalModel.bek04);
                }
                pDrDetail["cfb10"] = detailModel.cfb10;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkCfb04D3 由參考單號檢查貸方/應收帳款
        /// <summary>
        /// 檢查 借方/待抵應收帳款 並視情況預帶資料
        /// </summary>
        /// <param name="pCheckType">1.itemchk 2.存檔或確認</param>
        /// <param name="pMasterModel"></param>
        /// <param name="pDrCart200s"></param>
        /// <param name="pDetailModel"></param>
        /// <param name="pDetailList"></param>
        /// <returns></returns>
        private bool WfChkCfb04D3(string pCheckType, vw_cart200 pMasterModel, DataRow pDrCart200s, vw_cart200s pDetailModel, List<vw_cart200s> pDetailList)
        {
            cea_tb ceaModel = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            DataRow drSum = null;
            decimal cfb09Sum = 0, cfb09ThisDoc = 0;
            decimal cfb10Sum = 0, cfb10ThisDoc = 0;
            decimal cfb09Max = 0, cfb10Max = 0;
            try
            {
                ceaModel = BoCar.OfGetCeaModel(pDetailModel.cfb05);
                if (ceaModel == null)
                {
                    WfShowErrorMsg("無此應收帳款,請檢核!");
                    return false;
                }
                if (ceaModel.cea03 != pMasterModel.cfa03)
                {
                    WfShowErrorMsg("客戶不相同,請檢核!");
                    return false;
                }

                if (ceaModel.ceaconf != "Y")
                {
                    WfShowErrorMsg("應收帳款非確認狀態,請檢核!");
                    return false;
                }
                if ((ceaModel.cea15t - ceaModel.cea16) <= 0)
                {
                    WfShowErrorMsg("無可沖帳金額,請檢核!");
                    return false;
                }
                if (FormEditMode == YREditType.新增)
                    {
                    sqlSelect = @"SELECT SUM(cfb09) AS cfb09,SUM(cfb10) AS cfb10
                               FROM cfa_tb 
                                    INNER JOIN cfb_tb ON cfa01=cfb01
                              WHERE cfaconf <>'X'
                                    AND cfb03='1' AND cfb04='3' 
                                    AND cfb05=@cfb05
                            ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@cfb05", pDetailModel.cfb05));
                    drSum = BoCar.OfGetDataRow(sqlSelect, sqlParmList.ToArray());
                }
                else //修改時
                {
                    sqlSelect = @"SELECT SUM(cfb09) AS cfb09,SUM(cfb10) AS cfb10
                               FROM cfa_tb 
                                    INNER JOIN cfb_tb ON cfa01=cfb01
                              WHERE cfaconf <>'X'
                                    AND cfb03='1' AND cfb04='3' 
                                    AND cfb05=@cfb05 
                                    AND cfa01 <> @cfa01
                            ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@cfb05", pDetailModel.cfb05));
                    sqlParmList.Add(new SqlParameter("@cfa01", pMasterModel.cfa01));
                    drSum = BoCar.OfGetDataRow(sqlSelect, sqlParmList.ToArray());
                }
                if (drSum != null)
                {
                    cfb09Sum = GlobalFn.isNullRet(drSum["cfb09"], 0m);
                    cfb10Sum = GlobalFn.isNullRet(drSum["cfb10"], 0m);
                }
                var sumThisDoc = from p in pDetailList
                                 where p.cfb05 == pDetailModel.cfb05 && p.cfb03 == "1" && p.cfb04 == "3"
                                       && p.cfb02 != pDetailModel.cfb02
                                 group p by p.cfb05 into g
                                 select new
                                 {
                                     cfb09 = g.Sum(x => x.cfb09),
                                     cfb10 = g.Sum(x => x.cfb10)
                                 };
                if (sumThisDoc != null && sumThisDoc.Count() > 0)
                {
                    cfb09Max = ceaModel.cea13t - cfb09Sum - sumThisDoc.FirstOrDefault().cfb09;
                    cfb10Max = ceaModel.cea13t - cfb09Sum - sumThisDoc.FirstOrDefault().cfb10;
                }
                else
                {
                    cfb09Max = ceaModel.cea13t - cfb09Sum;
                    cfb10Max = ceaModel.cea13t - cfb09Sum;
                }

                if (cfb09Max <= 0 || cfb10Max <= 0)
                {
                    WfShowErrorMsg("無可沖帳金額,請檢核!");
                    return false;
                }

                //itemcheck時要預帶金額
                if (pCheckType == "1")
                {
                    pDrCart200s["cfb09"] = cfb09Max;
                    pDrCart200s["cfb10"] = cfb10Max;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkCfb04C1 由參考單號檢查貸方/應收帳款
        /// <summary>
        /// 檢查 貸方/應收帳款 並視情況預帶資料
        /// </summary>
        /// <param name="pCheckType">1.itemchk 2.存檔或確認</param>
        /// <param name="pMasterModel"></param>
        /// <param name="pDrCart200s"></param>
        /// <param name="pDetailModel"></param>
        /// <param name="pDetailList"></param>
        /// <returns></returns>
        private bool WfChkCfb04C1(string pCheckType, vw_cart200 pMasterModel, DataRow pDrCart200s, vw_cart200s pDetailModel, List<vw_cart200s> pDetailList)
        {
            cea_tb ceaModel = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            DataRow drSum = null;
            decimal cfb09Sum = 0, cfb09ThisDoc = 0;
            decimal cfb10Sum = 0, cfb10ThisDoc = 0;
            decimal cfb09Max = 0, cfb10Max = 0;
            try
            {
                ceaModel = BoCar.OfGetCeaModel(pDetailModel.cfb05);
                if (ceaModel == null)
                {
                    WfShowErrorMsg("無此應收帳款,請檢核!");
                    return false;
                }
                if (ceaModel.cea03 != pMasterModel.cfa03)
                {
                    WfShowErrorMsg("客戶不相同,請檢核!");
                    return false;
                }

                if (ceaModel.ceaconf != "Y")
                {
                    WfShowErrorMsg("應收帳款非確認狀態,請檢核!");
                    return false;
                }
                if ((ceaModel.cea15t - ceaModel.cea16) <= 0)
                {
                    WfShowErrorMsg("無可沖帳金額,請檢核!");
                    return false;
                }
                if (FormEditMode == YREditType.新增)
                {
                    sqlSelect = @"SELECT SUM(cfb09) AS cfb09,SUM(cfb10) AS cfb10
                               FROM cfa_tb 
                                    INNER JOIN cfb_tb ON cfa01=cfb01
                              WHERE cfaconf <>'X'
                                    AND cfb03='2' AND cfb04='1' 
                                    AND cfb05=@cfb05
                            ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@cfb05", pDetailModel.cfb05));
                    drSum = BoCar.OfGetDataRow(sqlSelect, sqlParmList.ToArray());
                }
                else //修改時
                {
                    sqlSelect = @"SELECT SUM(cfb09) AS cfb09,SUM(cfb10) AS cfb10
                               FROM cfa_tb 
                                    INNER JOIN cfb_tb ON cfa01=cfb01
                              WHERE cfaconf <>'X'
                                    AND cfb03='2' AND cfb04='1' 
                                    AND cfb05=@cfb05 
                                    AND cfa01 <> @cfa01
                            ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@cfb05", pDetailModel.cfb05));
                    sqlParmList.Add(new SqlParameter("@cfa01", pMasterModel.cfa01));
                    drSum = BoCar.OfGetDataRow(sqlSelect, sqlParmList.ToArray());
                }
                if (drSum != null)
                {
                    cfb09Sum = GlobalFn.isNullRet(drSum["cfb09"], 0m);
                    cfb10Sum = GlobalFn.isNullRet(drSum["cfb10"], 0m);
                }
                var sumThisDoc = from p in pDetailList
                                 where p.cfb05 == pDetailModel.cfb05 && p.cfb03 == "2" && p.cfb04 == "1"
                                       && p.cfb02 != pDetailModel.cfb02
                                 group p by p.cfb05 into g
                                 select new
                                 {
                                     cfb09 = g.Sum(x => x.cfb09),
                                     cfb10 = g.Sum(x => x.cfb10)
                                 };
                if (sumThisDoc != null && sumThisDoc.Count() > 0)
                {
                    cfb09Max = ceaModel.cea13t - cfb09Sum - sumThisDoc.FirstOrDefault().cfb09;
                    cfb10Max = ceaModel.cea13t - cfb09Sum - sumThisDoc.FirstOrDefault().cfb10;
                }
                else
                {
                    cfb09Max = ceaModel.cea13t - cfb09Sum;
                    cfb10Max = ceaModel.cea13t - cfb09Sum;
                }

                if (cfb09Max <= 0 || cfb10Max <= 0)
                {
                    WfShowErrorMsg("無可沖帳金額,請檢核!");
                    return false;
                }

                //itemcheck時要預帶金額
                if (pCheckType == "1")
                {
                    pDrCart200s["cfb09"] = cfb09Max;
                    pDrCart200s["cfb10"] = cfb10Max;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkCfb09Cfb10D3 由金額檢查借方/待抵帳款
        private bool WfChkCfb09Cfb10D3(string pChkColumn, vw_cart200 pMasterModel, DataRow pDrCart200s, vw_cart200s pDetailModel, List<vw_cart200s> pDetailList)
        {
            cea_tb ceaModel = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            DataRow drSum = null;
            decimal cfb09Sum = 0;
            decimal cfb10Sum = 0;
            decimal cfb09Max = 0, cfb10Max = 0;
            try
            {
                ceaModel = BoCar.OfGetCeaModel(pDetailModel.cfb05);
                if (ceaModel == null)
                {
                    WfShowErrorMsg("無此應收帳款,請檢核!");
                    return false;
                }
                if (ceaModel.ceaconf != "Y")
                {
                    WfShowErrorMsg("應收帳款非確認狀態,請檢核!");
                    return false;
                }
                if ((ceaModel.cea15t - ceaModel.cea16) <= 0)
                {
                    WfShowErrorMsg("無可沖帳金額,請檢核!");
                    return false;
                }
                if (FormEditMode == YREditType.新增)
                {
                    sqlSelect = @"SELECT SUM(cfb09) AS cfb09,SUM(cfb10) AS cfb10
                               FROM cfa_tb 
                                    INNER JOIN cfb_tb ON cfa01=cfb01
                              WHERE cfaconf <>'X'
                                    AND cfb03='1' AND cfb04='3' 
                                    AND cfb05=@cfb05
                            ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@cfb05", pDetailModel.cfb05));
                    drSum = BoCar.OfGetDataRow(sqlSelect, sqlParmList.ToArray());
                }
                else //修改時
                {
                    sqlSelect = @"SELECT SUM(cfb09) AS cfb09,SUM(cfb10) AS cfb10
                               FROM cfa_tb 
                                    INNER JOIN cfb_tb ON cfa01=cfb01
                              WHERE cfaconf <>'X'
                                    AND cfb03='1' AND cfb04='3' 
                                    AND cfb05=@cfb05 
                                    AND cfa01 <> @cfa01
                            ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@cfb05", pDetailModel.cfb05));
                    sqlParmList.Add(new SqlParameter("@cfa01", pMasterModel.cfa01));
                    drSum = BoCar.OfGetDataRow(sqlSelect, sqlParmList.ToArray());
                }
                if (drSum != null)
                {
                    cfb09Sum = GlobalFn.isNullRet(drSum["cfb09"], 0m);
                    cfb10Sum = GlobalFn.isNullRet(drSum["cfb10"], 0m);
                }
                var sumThisDoc = from p in pDetailList
                                 where p.cfb05 == pDetailModel.cfb05 && p.cfb03 == "1" && p.cfb04 == "3"
                                       && p.cfb02 != pDetailModel.cfb02
                                 group p by p.cfb05 into g
                                 select new
                                 {
                                     cfb09 = g.Sum(x => x.cfb09),
                                     cfb10 = g.Sum(x => x.cfb10)
                                 };
                if (sumThisDoc != null && sumThisDoc.Count() > 0)
                {
                    cfb09Max = ceaModel.cea13t - cfb09Sum - sumThisDoc.FirstOrDefault().cfb09;
                    cfb10Max = ceaModel.cea13t - cfb09Sum - sumThisDoc.FirstOrDefault().cfb10;
                }
                else
                {
                    cfb09Max = ceaModel.cea13t - cfb09Sum;
                    cfb10Max = ceaModel.cea13t - cfb09Sum;
                }

                if (pChkColumn.ToLower() == "cfb09")
                {
                    if (cfb09Max < pDetailModel.cfb09)
                    {
                        WfShowErrorMsg(string.Format("項次{0} 原幣金額已超過應收可沖帳金額,請檢核!", pDetailModel.cfb02));
                        return false;
                    }
                }
                else if (pChkColumn.ToLower() == "cfb10")
                {
                    if (cfb09Max < pDetailModel.cfb10)
                    {
                        WfShowErrorMsg(string.Format("項次{0} 本幣金額已超過應收可沖帳金額,請檢核!", pDetailModel.cfb02));
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

        #region WfChkCfb09Cfb10C1 由金額檢查貸方/應收帳款
        private bool WfChkCfb09Cfb10C1(string pChkColumn, vw_cart200 pMasterModel, DataRow pDrCart200s, vw_cart200s pDetailModel, List<vw_cart200s> pDetailList)
        {
            cea_tb ceaModel = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            DataRow drSum = null;
            decimal cfb09Sum = 0;
            decimal cfb10Sum = 0;
            decimal cfb09Max = 0, cfb10Max = 0;
            try
            {
                ceaModel = BoCar.OfGetCeaModel(pDetailModel.cfb05);
                if (ceaModel == null)
                {
                    WfShowErrorMsg("無此應收帳款,請檢核!");
                    return false;
                }
                if (ceaModel.ceaconf != "Y")
                {
                    WfShowErrorMsg("應收帳款非確認狀態,請檢核!");
                    return false;
                }
                if ((ceaModel.cea15t - ceaModel.cea16) <= 0)
                {
                    WfShowErrorMsg("無可沖帳金額,請檢核!");
                    return false;
                }
                if (FormEditMode == YREditType.新增)
                {
                    sqlSelect = @"SELECT SUM(cfb09) AS cfb09,SUM(cfb10) AS cfb10
                               FROM cfa_tb 
                                    INNER JOIN cfb_tb ON cfa01=cfb01
                              WHERE cfaconf <>'X'
                                    AND cfb03='2' AND cfb04='1' 
                                    AND cfb05=@cfb05
                            ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@cfb05", pDetailModel.cfb05));
                    drSum = BoCar.OfGetDataRow(sqlSelect, sqlParmList.ToArray());
                }
                else //修改時
                {
                    sqlSelect = @"SELECT SUM(cfb09) AS cfb09,SUM(cfb10) AS cfb10
                               FROM cfa_tb 
                                    INNER JOIN cfb_tb ON cfa01=cfb01
                              WHERE cfaconf <>'X'
                                    AND cfb03='2' AND cfb04='1' 
                                    AND cfb05=@cfb05 
                                    AND cfa01 <> @cfa01
                            ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@cfb05", pDetailModel.cfb05));
                    sqlParmList.Add(new SqlParameter("@cfa01", pMasterModel.cfa01));
                    drSum = BoCar.OfGetDataRow(sqlSelect, sqlParmList.ToArray());
                }
                if (drSum != null)
                {
                    cfb09Sum = GlobalFn.isNullRet(drSum["cfb09"], 0m);
                    cfb10Sum = GlobalFn.isNullRet(drSum["cfb10"], 0m);
                }
                var sumThisDoc = from p in pDetailList
                                 where p.cfb05 == pDetailModel.cfb05 && p.cfb03 == "2" && p.cfb04 == "1"
                                       && p.cfb02 != pDetailModel.cfb02
                                 group p by p.cfb05 into g
                                 select new
                                 {
                                     cfb09 = g.Sum(x => x.cfb09),
                                     cfb10 = g.Sum(x => x.cfb10)
                                 };
                if (sumThisDoc != null && sumThisDoc.Count() > 0)
                {
                    cfb09Max = ceaModel.cea13t - cfb09Sum - sumThisDoc.FirstOrDefault().cfb09;
                    cfb10Max = ceaModel.cea13t - cfb09Sum - sumThisDoc.FirstOrDefault().cfb10;
                }
                else
                {
                    cfb09Max = ceaModel.cea13t - cfb09Sum;
                    cfb10Max = ceaModel.cea13t - cfb09Sum;
                }

                if (pChkColumn.ToLower() == "cfb09")
                {
                    if (cfb09Max < pDetailModel.cfb09)
                    {
                        WfShowErrorMsg(string.Format("項次{0} 原幣金額已超過應收可沖帳金額,請檢核!", pDetailModel.cfb02));
                        return false;
                    }
                }
                else if (pChkColumn.ToLower() == "cfb10")
                {
                    if (cfb09Max < pDetailModel.cfb10)
                    {
                        WfShowErrorMsg(string.Format("項次{0} 本幣金額已超過應收可沖帳金額,請檢核!", pDetailModel.cfb02));
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

        #region WfConfirm 確認
        private void WfConfirm()
        {
            cfa_tb cfaModel = null;
            vw_cart200s detailModel = null;
            
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

                cfaModel = DrMaster.ToItem<cfa_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                cfaModel.cfaconf = "Y";
                cfaModel.cfacond = Today;
                cfaModel.cfaconu = LoginInfo.UserNo;

                DrMaster["cfastat"] = "1";
                DrMaster["cfaconf"] = "Y";
                DrMaster["cfacond"] = Today;
                DrMaster["cfaconu"] = LoginInfo.UserNo;
                DrMaster["cfamodu"] = LoginInfo.UserNo;
                DrMaster["cfamodg"] = LoginInfo.DeptNo;
                DrMaster["cfamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drDetail.ToItem<vw_cart200s>();

                    if ((detailModel.cfb03 == "1" && detailModel.cfb04 == "3") 
                        )
                    {
                        if (WfUpdCea14Cea16("D3",detailModel.cfb05) == false)
                        {
                            WfRollback();
                            WfRetrieveMaster();
                            WfRetrieveDetail();
                            return;
                        }
                    }

                    if ((detailModel.cfb03 == "2" && detailModel.cfb04 == "1")
                        )
                    {
                        if (WfUpdCea14Cea16("C1", detailModel.cfb05) == false)
                        {
                            WfRollback();
                            WfRetrieveMaster();
                            WfRetrieveDetail();
                            return;
                        }
                    }
                    
                    if (detailModel.cfb03 == "2" && detailModel.cfb04 == "2")
                    {
                        if (WfGenCeaByC2( cfaModel, detailModel)==false)
                        {
                            WfRollback();
                            WfRetrieveMaster();
                            WfRetrieveDetail();
                            return;
                        }

                    }
                }

                WfCommit();
                //DrMaster.AcceptChanges();

                WfRetrieveMaster();
                WfRetrieveDetail();
                WfShowBottomStatusMsg("作業成功!");
                cfaModel = DrMaster.ToItem<cfa_tb>();
                WfSetDocPicture("", cfaModel.cfaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                //DrMaster.RejectChanges();
                WfRetrieveMaster();
                WfRetrieveDetail();
                //if (WfRetrieveMaster() == false)
                //    return;
                throw ex;
            }
        }
        #endregion
        
        #region WfConfirmChk 確認前檢查
        private bool WfConfirmChk()
        {
            vw_cart200 masterModel = null;
            vw_cart200s detailModel = null;
            List<vw_cart200s> detailList = null;
            Result result;
            gea_tb geaModel = null;
            cac_tb cacModel = null;
            string sqlSelect;
            List<SqlParameter> sqlParmList = null;
            int chkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_cart200>();
                if (masterModel.cfaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }

                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.cfa02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                if (masterModel.cfa08 != masterModel.cfa09 || masterModel.cfa10 != masterModel.cfa11)
                {
                    WfShowErrorMsg("借貸不平衡,請檢核!");
                    return false;
                }

                geaModel = BoGlat200.OfGetGeaModel(masterModel.cfa01, "AR", 2, 1);
                if (geaModel == null)
                {
                    WfShowErrorMsg("查無分錄底稿,請檢核!");
                    return false;
                }
                if (geaModel.gea08 != geaModel.gea08)
                {
                    WfShowErrorMsg("分錄底稿借貸不平衡,請檢核!");
                    return false;
                }
                if (geaModel.gea08 != masterModel.cfa10)
                {
                    WfShowErrorMsg("收款單與分錄底稿金額不同,請檢核!");
                    return false;
                }
                
                detailList = TabDetailList[0].DtSource.ToList<vw_cart200s>();
                if (detailList == null || detailList.Count == 0)
                {
                    WfShowErrorMsg("無收款明細,請檢核!");
                    return false;
                }
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_cart200s>();
                    //借方-待抵帳款
                    if ((detailModel.cfb03 == "1" && detailModel.cfb04 == "3"))
                    {
                        if (WfChkCfb09Cfb10D3("cfb09", masterModel, drTemp, detailModel, detailList) == false)
                        {
                            return false;
                        }

                        if (WfChkCfb09Cfb10D3("cfb10", masterModel, drTemp, detailModel, detailList) == false)
                        {
                            return false;
                        }
                    }

                    //貸方-應收帳款
                    if ((detailModel.cfb03 == "2" && detailModel.cfb04 == "1"))
                    {
                        if (WfChkCfb09Cfb10C1("cfb09", masterModel, drTemp, detailModel, detailList) == false)
                        {
                            return false;
                        }

                        if (WfChkCfb09Cfb10C1("cfb10", masterModel, drTemp, detailModel, detailList) == false)
                        {
                            return false;
                        }
                    }
                    
                    if ((detailModel.cfb03 == "2" && detailModel.cfb04 == "2"))
                    {
                        if (detailModel.cfb05.Length != BaaModel.baa06)
                        {
                            WfShowErrorMsg("單別位數不符!");
                            return false;
                        }
                        cacModel = BoCar.OfGetCacModel(detailModel.cfb05);
                        if (cacModel == null)
                        {
                            WfShowErrorMsg("查無此單別!");
                            return false;
                        }                        
                        if (cacModel.cacvali != "Y")
                        {
                            WfShowErrorMsg("非有效單別!");
                            return false;
                        }
                        if (cacModel.cac04 != "22")
                        {
                            WfShowErrorMsg("非溢收單別!");
                            return false;
                        }
                        
                        sqlSelect = @"SELECT COUNT(1) FROM cea_tb 
                                        WHERE cea01=@cea01 AND (cea14>0 OR cea16>0)";
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@ceae01",detailModel.cfb05));
                        chkCnts = GlobalFn.isNullRet(BoCar.OfGetFieldValue(sqlSelect, sqlParmList.ToArray()),0);
                        if (chkCnts>0)
                        {
                            WfShowErrorMsg(string.Concat("項次",detailModel.cfb02,"帳款已有沖帳記錄,不可取消確認!"));
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

        #region WfCancelConfirm 取消確認
        private void WfCancelConfirm()
        {
            cfa_tb cfaModel = null;
            vw_cart200s detailModel = null;
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
                cfaModel = DrMaster.ToItem<cfa_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                DrMaster["cfaconf"] = "N";
                DrMaster["cfacond"] = DBNull.Value;
                DrMaster["cfaconu"] = "";
                DrMaster["cfamodu"] = LoginInfo.UserNo;
                DrMaster["cfamodg"] = LoginInfo.DeptNo;
                DrMaster["cfamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drDetail.ToItem<vw_cart200s>();

                    if (detailModel.cfb03 == "1" && detailModel.cfb04 == "3")
                    {
                        if (WfUpdCea14Cea16("D3", detailModel.cfb05) == false)
                        {
                            WfRetrieveMaster();
                            WfRetrieveDetail();
                            WfSetDocPicture("", cfaModel.cfaconf, "", pbxDoc);
                            return;
                        }
                    }

                    if (detailModel.cfb03 == "2" && detailModel.cfb04 == "1")
                    {
                        if (WfUpdCea14Cea16("C1",detailModel.cfb05) == false)
                        {
                            WfRetrieveMaster();
                            WfRetrieveDetail();
                            WfSetDocPicture("", cfaModel.cfaconf, "", pbxDoc);
                            return;
                        }
                    }
                }

                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                WfRetrieveMaster();
                WfRetrieveDetail();
                cfaModel = DrMaster.ToItem<cfa_tb>();
                WfSetDocPicture("", cfaModel.cfaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                WfRetrieveMaster();
                WfRetrieveDetail();
                cfaModel = DrMaster.ToItem<cfa_tb>();
                WfSetDocPicture("", cfaModel.cfaconf, "", pbxDoc);
                //DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfCancelConfirmChk 取消確認前檢查
        private bool WfCancelConfirmChk()
        {
            vw_cart200 masterModel = null;
            List<cfb_tb> cebList = null;
            decimal icc05;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_cart200>();
                if (masterModel.cfaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.cfa02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                if (!GlobalFn.varIsNull(masterModel.cfa12))
                {
                    WfShowErrorMsg("單據已拋轉傳票!");
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

        #region WfGenGea 產生分錄底稿
        private bool WfGenGea(string pCfa01)
        {
            vw_cart200 masterModel = null;
            int chkCnts = 0;
            string sqlSelect, deleteGeaSql = "", deleteGebSql = "";
            List<SqlParameter> sqlParmList = null;
            try
            {
                WfRetrieveMaster();
                masterModel = DrMaster.ToItem<vw_cart200>();
                if (masterModel == null)
                {
                    WfShowErrorMsg("無資料可產生分錄底稿!");
                    return false;
                }
                if (masterModel.cfaconf != "N")
                {
                    WfShowErrorMsg("非未確認狀態,不可更新分錄底稿!");
                    return false;
                }

                sqlSelect = @"SELECT COUNT(1) FROM gea_tb
                            WHERE gea01=@gea01 AND gea02='AR'
                                AND gea03=2
                            ";
                deleteGeaSql = @"DELETE FROM gea_tb 
                                    WHERE gea01=@gea01 AND gea02='AR'  AND gea03=2
                                    ";
                deleteGebSql = @"DELETE FROM geb_tb 
                                    WHERE geb01=@geb01 AND geb02='AR'  AND geb03=2
                                    ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gea01", masterModel.cfa01));
                chkCnts = GlobalFn.isNullRet(BoCar.OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0);

                if (chkCnts > 0)
                {

                    if (WfShowConfirmMsg("分錄底稿已存在,是否重新新增?") == DialogResult.Yes)
                    {
                        if (WfLockMasterRow() == false) //這裡開始begin tran
                        {
                            WfRollback();
                            return false;
                        }
                        //BoGlat200.TRAN = BoMaster.TRAN;
                        WfSetBllTransaction();
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gea01", masterModel.cfa01));
                        BoCar.OfExecuteNonquery(deleteGeaSql, sqlParmList.ToArray());

                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@geb01", masterModel.cfa01));
                        BoCar.OfExecuteNonquery(deleteGebSql, sqlParmList.ToArray());
                        var resultDeleteList = BoGlat200.OfGenGeaByCfa(masterModel.cfa01, LoginInfo);
                        if (resultDeleteList != null && resultDeleteList.Count > 0)
                        {
                            WfShowErrorMsg(resultDeleteList[0].Message);
                            return false;
                        }
                        BoGlat200.TRAN.Commit();
                    }
                    else
                        return true;
                }
                else
                {
                    if (WfLockMasterRow() == false) //這裡開始begin tran
                    {
                        WfRollback();
                        return false;
                    }
                    BoGlat200.TRAN = BoMaster.TRAN;

                    var resultGenList = BoGlat200.OfGenGeaByCfa(pCfa01, LoginInfo);
                    if (resultGenList != null && resultGenList.Count > 0)
                    {
                        WfShowErrorMsg(resultGenList[0].Message);
                        WfRollback();
                        return false;
                    }
                    BoGlat200.TRAN.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (BoMaster.TRAN != null)
                    WfRollback();

                throw ex;
            }
        }
        #endregion

        #region WfUpdCea14Cea16 確認及取消確認時,更新帳款沖帳金額
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pType">D3.沖待抵 C1.沖應收</param>
        /// <param name="pCfb05"></param>
        /// <returns></returns>
        private bool WfUpdCea14Cea16(string pType,string pCfb05)
        {
            string sqlSelect, sqlUpdCea;
            List<SqlParameter> sqlParmList = null;
            DataRow drSum;
            decimal cfb09Sum, cfb10Sum;
            int chkCnts = 0;
            try
            {
                sqlSelect = @"SELECT SUM(cfb09) AS cfb09,SUM(cfb10) AS cfb10		
                              FROM cfa_tb									
                            	    INNER JOIN cfb_tb ON cfa01=cfb01			
                              WHERE cfaconf='Y'							
                            	    AND cfb05=@cfb05";
                if (pType=="D3")
                {
                    sqlSelect += " AND cfb03='1' AND cfb04='3' ";
                }
                else if (pType == "C1")
                    sqlSelect += " AND cfb03='2' AND cfb04='1' ";

                sqlUpdCea = @"UPDATE cea_tb
                              SET cea14=@cea14,
                                  cea16=@cea16
                              WHERE   cea01=@cea01
                            ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@cfb05", pCfb05));
                drSum = BoCar.OfGetDataRow(sqlSelect, sqlParmList.ToArray());
                cfb09Sum = GlobalFn.isNullRet(drSum["cfb09"], 0m);
                cfb10Sum = GlobalFn.isNullRet(drSum["cfb10"], 0m);

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@cea14", cfb09Sum));
                sqlParmList.Add(new SqlParameter("@cea16", cfb10Sum));
                sqlParmList.Add(new SqlParameter("@cea01", pCfb05));

                chkCnts = BoCar.OfExecuteNonquery(sqlUpdCea, sqlParmList);
                if (chkCnts <= 0)
                {
                    WfShowErrorMsg(string.Format("更新帳款 {0} 沖帳金額失敗,請檢核!", pCfb05));
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

        #region WfGenCeaByC2 產生溢收帳款
        private bool WfGenCeaByC2(cfa_tb pCfaModel,vw_cart200s pDetailModel)
        {
            CarBLL boCea = null;
            DataTable dtCea = null;
            DataRow drCeaNew = null;
            string sqlSelect = "",sqlUpdateCfb="";
            string cea01New = "", errMsg = "";
            List<SqlParameter> sqlParmList = null;
            int chkCnts = 0;
            try
            {
                boCea = new CarBLL(BoMaster.OfGetConntion());
                boCea.TRAN = BoMaster.TRAN;
                sqlUpdateCfb = @"UPDATE cfb_tb
                                SET cfb05=@cfb05
                                WHERE cfb01=@cfb01 AND cfb02=@cfb02
                            ";

                boCea.OfCreateDao("cea_tb", "*", "");
                sqlSelect = @"SELECT * FROM cea_tb WHERE 1<>1";
                dtCea = boCea.OfGetDataTable(sqlSelect);
                drCeaNew = dtCea.NewRow();

                cea01New = "";
                errMsg = "";
                if (BoBas.OfGetAutoNo(pDetailModel.cfb05, ModuleType.car, (DateTime)pCfaModel.cfa02, out cea01New, out errMsg) == false)
                {
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                drCeaNew["cea00"] = "22";   //帳款類別  22.溢收
                drCeaNew["cea01"] = cea01New;
                drCeaNew["cea02"] = pCfaModel.cfa02;    //帳款日期
                drCeaNew["cea03"] = pCfaModel.cfa03;    //帳款客戶編號
                drCeaNew["cea04"] = pCfaModel.cfa04;    //業務人員
                drCeaNew["cea05"] = pCfaModel.cfa05;    //部門編號
                drCeaNew["cea06"] = "";         //稅別
                drCeaNew["cea07"] = 0;          //稅率
                drCeaNew["cea08"] = "N";        //含稅否
                drCeaNew["cea09"] = "";         //發票聯數
                drCeaNew["cea10"] = pDetailModel.cfb07;         //幣別
                drCeaNew["cea11"] = "";     //收款條件
                drCeaNew["cea12"] = pDetailModel.cfb08;     //匯率
                drCeaNew["cea13"] = pDetailModel.cfb09;     //原幣未稅
                drCeaNew["cea13t"] = pDetailModel.cfb09;    //原幣含稅
                drCeaNew["cea13g"] = 0;    //原幣稅額
                drCeaNew["cea14"] = 0;     //原幣沖帳
                drCeaNew["cea15"] = pDetailModel.cfb10;     //本幣未稅
                drCeaNew["cea15t"] = pDetailModel.cfb10;    //本幣含稅金額
                drCeaNew["cea15g"] = 0;    //本幣稅額
                drCeaNew["cea16"] = 0;     //本幣沖帳
                drCeaNew["cea17"] = pCfaModel.cfa01;     //參考單號
                drCeaNew["cea18"] = "";     //備註
                drCeaNew["cea19"] = DBNull.Value;     //應收款日
                drCeaNew["cea20"] = DBNull.Value;     //保留
                drCeaNew["cea21"] = "";     //科目類別
                drCeaNew["cea22"] = pDetailModel.cfb11;     //會科編號
                drCeaNew["cea23"] = "";     //發票別
                drCeaNew["cea24"] = DBNull.Value;     //發票日期
                drCeaNew["cea25"] = "";     //發票號碼
                drCeaNew["cea26"] = "";     //發票客戶
                drCeaNew["cea27"] = "";     //申報統編
                drCeaNew["cea28"] = "1";     //來源別      1.轉入 2.人工輸入
                drCeaNew["cea29"] = "";      //傳票號碼
                drCeaNew["cea30"] = DBNull.Value;     //保留
                drCeaNew["cea31"] = DBNull.Value;     //保留
                drCeaNew["cea32"] = DBNull.Value;     //保留
                drCeaNew["cea33"] = DBNull.Value;     //保留
                drCeaNew["cea34"] = DBNull.Value;     //保留
                drCeaNew["cea35"] = DBNull.Value;     //保留
                drCeaNew["cea36"] = DBNull.Value;     //保留
                drCeaNew["cea37"] = DBNull.Value;     //保留
                drCeaNew["cea38"] = DBNull.Value;     //保留
                drCeaNew["cea39"] = DBNull.Value;     //保留
                drCeaNew["cea40"] = DBNull.Value;     //保留
                drCeaNew["ceaconf"] = "Y";
                drCeaNew["ceacond"] = Now;
                drCeaNew["ceaconu"] = LoginInfo.UserNo;
                drCeaNew["ceastat"] = "1";
                drCeaNew["ceacomp"] = LoginInfo.CompNo;
                drCeaNew["ceacreu"] = LoginInfo.UserNo;
                drCeaNew["ceacreg"] = LoginInfo.DeptNo;
                drCeaNew["ceacred"] = Now;
                drCeaNew["ceamodu"] = DBNull.Value;
                drCeaNew["ceamodg"] = DBNull.Value;
                drCeaNew["ceamodd"] = DBNull.Value;
                drCeaNew["ceasecu"] = LoginInfo.UserNo;
                drCeaNew["ceasecg"] = LoginInfo.GroupNo;    
                dtCea.Rows.Add(drCeaNew);

                if (boCea.OfUpdate(dtCea) <= 0)
                {
                    WfShowErrorMsg("新增溢收帳款失敗!");
                    return false;
                }

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@cfb01", pDetailModel.cfb01));
                sqlParmList.Add(new SqlParameter("@cfb02", pDetailModel.cfb02));
                sqlParmList.Add(new SqlParameter("@cfb05", cea01New));
                chkCnts = boCea.OfExecuteNonquery(sqlUpdateCfb, sqlParmList);

                if (chkCnts <= 0)
                {
                    WfShowErrorMsg(string.Concat(pDetailModel.cfb01,"-",pDetailModel.cfb02, " 寫入收款單溢收單號失敗!"));
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

        #region WfGenVoucher 拋轉傳票
        private bool WfGenVoucher()
        {
            vw_cart200 masterModel = null;
            vw_carb350 carb350Model = null;
            int chkCnts = 0;
            StringBuilder sbResult = null;
            Carb350BLL boCarb350;
            cac_tb cacModel = null;
            try
            {
                if (DrMaster == null)
                {
                    WfShowBottomStatusMsg("無資料可拋轉傳票!");
                    return false;
                }
                WfRetrieveMaster();
                masterModel = DrMaster.ToItem<vw_cart200>();
                if (masterModel.cfaconf != "Y")
                {
                    WfShowBottomStatusMsg("收款單未確認!");
                    return false;
                }

                if (!GlobalFn.varIsNull(masterModel.cfa12))
                {
                    WfShowBottomStatusMsg("已拋轉傳票,不可重覆拋轉!");
                    return false;
                }
                cacModel = BoCar.OfGetCacModel(masterModel.cfa01);
                if (cacModel == null)
                {
                    WfShowBottomStatusMsg("取得應收單別資料錯誤!");
                    return false;
                }
                if (cacModel.cac08 != "Y")
                {
                    WfShowBottomStatusMsg("此單別設定不可拋轉傳票!");
                    return false;
                }

                if (WfLockMasterRow() == false) //這裡開始begin tran
                {
                    WfRollback();
                    return false;
                }
                //BoMaster.TRAN = BoMaster.OfGetConntion().BeginTransaction();
                boCarb350 = new Carb350BLL(BoMaster.OfGetConntion());
                boCarb350.TRAN = BoMaster.TRAN;

                carb350Model = new vw_carb350();
                carb350Model.gea01 = masterModel.cfa01;
                carb350Model.gea03 = 2;//收款單


                var resultList = boCarb350.OfGenVoucher(carb350Model, "", LoginInfo);

                if (resultList == null || resultList.Count == 0)
                {
                    WfShowBottomStatusMsg("拋轉傳票失敗!");
                    boCarb350.TRAN.Rollback();
                    return true;
                }

                chkCnts = resultList.Where(p => p.Success == false).Count();
                if (chkCnts > 0)
                {
                    boCarb350.TRAN.Rollback();
                    sbResult = new StringBuilder();
                    sbResult.AppendLine(string.Format("執行失敗!"));
                    sbResult.AppendLine();
                    sbResult.AppendLine(string.Format("錯誤內容如下"));
                    sbResult.AppendLine("====================================");
                    foreach (Result result in resultList.Where(p => p.Success == false))
                    {
                        sbResult.AppendLine(string.Format("key1:【{0}】 錯誤訊息:【{1}】", result.Key1, result.Message));
                    }
                    WfShowErrorMsg(sbResult.ToString());
                    return false;
                }
                BoMaster.TRAN.Commit();
                WfRetrieveMaster();
                masterModel = DrMaster.ToItem<vw_cart200>();
                WfSetDocPicture("", masterModel.cfaconf, "", pbxDoc);
                WfShowBottomStatusMsg("執行成功!");
                return true;
            }
            catch (Exception ex)
            {
                if (BoMaster.TRAN != null)
                    BoMaster.TRAN.Rollback();
                WfRetrieveMaster();
                masterModel = DrMaster.ToItem<vw_cart200>();
                WfSetDocPicture("", masterModel.cfaconf, "", pbxDoc);
                WfShowErrorMsg(ex.Message);
                return false;
            }
        }
        #endregion
        
        #region WfUndoGenVoucher 拋轉傳票還原
        private bool WfUndoGenVoucher()
        {
            vw_cart200 masterModel = null;
            vw_carb351 carb351Model = null;
            int chkCnts = 0;
            StringBuilder sbResult = null;
            Carb351BLL boCarb351;
            cac_tb cacModel = null;
            try
            {
                if (DrMaster == null)
                {
                    WfShowBottomStatusMsg("無資料可拋轉傳票!");
                    return false;
                }
                WfRetrieveMaster();
                masterModel = DrMaster.ToItem<vw_cart200>();

                if (GlobalFn.varIsNull(masterModel.cfa12))
                {
                    WfShowBottomStatusMsg("傳票未拋轉!");
                    return false;
                }

                if (WfLockMasterRow() == false) //這裡開始begin tran
                {
                    WfRollback();
                    return false;
                }

                boCarb351 = new Carb351BLL(BoMaster.OfGetConntion());
                boCarb351.TRAN = BoMaster.TRAN;

                carb351Model = new vw_carb351();
                carb351Model.gfa01 = masterModel.cfa12;
                var resultList = boCarb351.OfUndoGenVoucher(carb351Model, "", LoginInfo);

                if (resultList == null || resultList.Count == 0)
                {
                    WfShowBottomStatusMsg("拋轉傳票還原失敗!");
                    boCarb351.TRAN.Rollback();
                    return true;
                }

                chkCnts = resultList.Where(p => p.Success == false).Count();
                if (chkCnts > 0)
                {
                    boCarb351.TRAN.Rollback();
                    sbResult = new StringBuilder();
                    sbResult.AppendLine(string.Format("執行失敗!"));
                    sbResult.AppendLine();
                    sbResult.AppendLine(string.Format("錯誤內容如下"));
                    sbResult.AppendLine("====================================");
                    foreach (Result result in resultList.Where(p => p.Success == false))
                    {
                        sbResult.AppendLine(string.Format("key1:【{0}】 錯誤訊息:【{1}】", result.Key1, result.Message));
                    }
                    WfShowErrorMsg(sbResult.ToString());
                    return false;
                }

                BoMaster.TRAN.Commit();
                WfRetrieveMaster();
                masterModel = DrMaster.ToItem<vw_cart200>();
                WfSetDocPicture("", masterModel.cfaconf, "", pbxDoc);
                WfShowBottomStatusMsg("執行成功!");
                return true;
            }
            catch (Exception ex)
            {
                if (BoMaster.TRAN != null)
                    BoMaster.TRAN.Rollback();
                WfRetrieveMaster();
                masterModel = DrMaster.ToItem<vw_cart200>();
                WfSetDocPicture("", masterModel.cfaconf, "", pbxDoc);
                WfShowErrorMsg(ex.Message);
                throw ex;
            }
        }
        #endregion
    }
}
