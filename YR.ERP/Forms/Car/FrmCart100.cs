/* 程式名稱: 應收帳款維護作業
   系統代號: cart100
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
    public partial class FrmCart100 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        CarBLL BoCar = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;
        StpBLL BoStp = null;
        Glat200BLL BoGlat200 = null;

        //baa_tb BaaTbModel = null;
        bek_tb BekTbModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmCart100()
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
            this.StrFormID = "cart100";
            uTab_Header.Tabs[0].Text = "基本資料";
            uTab_Header.Tabs[1].Text = "狀態";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "應收明細";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("cea01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "ceasecu";
                TabMaster.GroupColumn = "ceasecg";
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
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT azf02,azf03 FROM azf_tb WHERE azf01='car' AND (azf02 LIKE '1%' OR azf02 LIKE '2%') ");
                sourceList = BoCar.OfGetKVP(sbSql.ToString());
                WfSetUcomboxDataSource(ucb_cea00, sourceList);

                //來源類型
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.轉入"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.人工輸入"));
                WfSetUcomboxDataSource(ucb_cea28, sourceList);

                //課稅別
                sourceList = BoCar.OfGetTaxTypeKVPList();
                WfSetUcomboxDataSource(ucb_cea06, sourceList);

                //發票聯數
                sourceList = BoCar.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_cea09, sourceList);

                ////單據確認
                sourceList = BoCar.OfGetCeaconfKVPList();
                WfSetUcomboxDataSource(ucb_ceaconf, sourceList);

                ////單據狀態
                sourceList = BoCar.OfGetCeastatKVPList();
                WfSetUcomboxDataSource(ucb_ceastat, sourceList);
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

            this.TabDetailList[0].TargetTable = "ceb_tb";
            this.TabDetailList[0].ViewTable = "vw_cart100s";
            keyParm = new SqlParameter("ceb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "cea01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_cart100 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_cart100>();
                    WfSetDocPicture("", masterModel.ceaconf, masterModel.ceastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.cea10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.cea10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.cea10));
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

                    WfSetControlReadonly(new List<Control> { ute_ceacreu, ute_ceacreg, udt_ceacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_ceamodu, ute_ceamodg, udt_ceamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_ceasecu, ute_ceasecg }, true);

                    if (FormEditMode == YREditType.新增)
                    {
                        WfSetControlReadonly(ucb_cea00, false);
                    }
                    else
                    {
                        WfSetControlReadonly(ucb_cea00, true);
                    }


                    if (GlobalFn.isNullRet(masterModel.cea01, "") != "")
                    {
                        WfSetControlReadonly(ute_cea01, true);
                        //WfSetSfa01RelReadonly(GlobalFn.isNullRet(masterModel.sfa01, ""));
                    }

                    WfSetControlReadonly(new List<Control> { ute_cea01_c, ute_cea03_c, ute_cea04_c, ute_cea05_c, ute_cea11_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_cea07, ucx_cea08 }, true);
                    WfSetControlReadonly(new List<Control> { ute_cea13, ute_cea13t, ute_cea13g, ute_cea14 }, true);
                    WfSetControlReadonly(new List<Control> { ute_cea15, ute_cea15t, ute_cea15g, ute_cea16 }, true);
                    WfSetControlReadonly(new List<Control> { ute_cea21_c, ute_cea22_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_cea27, ucb_cea28, ute_cea29 }, true);
                    WfSetControlReadonly(new List<Control> { ucb_ceaconf, udt_ceacond, ute_ceaconu, ucb_ceastat }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_cea01, true);
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
                MessageInfo messageModel = new MessageInfo();
                vw_cart100 masterModel = null;
                vw_cart100s detailModel = null;
                #region 單頭-pick vw_cart100
                if (pDr.Table.Prefix.ToLower() == "vw_cart100")
                {
                    masterModel = DrMaster.ToItem<vw_cart100>();
                    if (GlobalFn.varIsNull(masterModel.cea00))
                    {
                        WfShowErrorMsg("請先輸入來源別!");
                        this.uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                        ucb_cea00.Focus();
                        return false;
                    }

                    switch (pColName.ToLower())
                    {
                        case "cea01"://帳款編號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@cac03", "car"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@cac04", masterModel.cea00));
                            WfShowPickUtility("p_cac1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["cac01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "cea03"://客戶編號
                            WfShowPickUtility("p_sca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sca01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;
                        case "cea04"://業務人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;
                        case "cea05"://業務部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "cea10"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "cea11"://收款條件
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@bef01", "2"));
                            WfShowPickUtility("p_bef1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bef02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "cea21"://應收科目類別
                            WfShowPickUtility("p_cba1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["cba01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;


                        case "cea22"://會計科目
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

                        case "cea26"://發票客戶
                            WfShowPickUtility("p_sca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sca01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_cart100s
                if (pDr.Table.Prefix.ToLower() == "vw_cart100s")
                {
                    masterModel = DrMaster.ToItem<vw_cart100>();
                    detailModel = pDr.ToItem<vw_cart100s>();
                    switch (pColName.ToLower())
                    {
                        case "ceb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "ceb16"://單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        //case "sfb11"://報價單號
                        //    if (GlobalFn.isNullRet(masterModel.sfa03, "") == "")
                        //        WfShowPickUtility("p_seb1", messageModel);
                        //    else
                        //    {
                        //        messageModel.ParamSearchList = new List<SqlParameter>();
                        //        messageModel.ParamSearchList.Add(new SqlParameter("@sea03", masterModel.sfa03));
                        //        WfShowPickUtility("p_seb2", messageModel);
                        //    }
                        //    if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        //    {
                        //        if (messageModel.DataRowList.Count > 0)
                        //        {
                        //            pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["seb01"], "");
                        //            pDr["sfb12"] = GlobalFn.isNullRet(messageModel.DataRowList[0]["seb02"], "");
                        //        }
                        //        else
                        //        {
                        //            pDr[pColName] = "";
                        //            pDr["sfb12"] = "";
                        //        }
                        //    }
                        //    break;

                        case "ceb10"://會計科目
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
            vw_cart100 masterModel = null;
            vw_cart100s detalModel = null;
            List<vw_cart100s> detailList = null;
            bab_tb babModel = null;
            UltraGrid uGrid = null;
            gba_tb gbaModel = null;
            sga_tb sgaModel = null;
            sgb_tb sgbModel = null;
            cba_tb cbaModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_cart100>();
                if (e.Column.ToLower() != "cea00" && GlobalFn.varIsNull(DrMaster["cea00"]))
                {
                    WfShowErrorMsg("請先輸入帳款類別!");
                    return false;
                }
                
                #region 單頭 vw_cart100
                if (e.Row.Table.Prefix.ToLower() == "vw_cart100")
                {
                    switch (e.Column.ToLower())
                    {
                        case "cea01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;

                            //todo:這裡的單據性質要改成範圍查詢
                            if (BoCar.OfChkCacPKValid(GlobalFn.isNullRet(e.Value, ""),"car",masterModel.cea00) == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }

                            e.Row["cea01_c"] = BoCar.OfGetCac02(e.Value.ToString());
                            WfSetControlReadonly(ucb_cea00, true);
                            WfSetControlReadonly(ute_cea01, true);
                            break;
                        case "cea03"://客戶編號
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["cea03_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkScaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此客戶資料,請檢核!");
                                return false;
                            }
                            WfSetCea03Relation(GlobalFn.isNullRet(e.Value, ""));
                            //WfResetAmtBySfa06();
                            break;

                        case "cea04"://業務人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["cea04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["cea04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "cea05"://業務部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["cea05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["cea05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "cea06"://課稅別
                            WfSetCea06Relation(GlobalFn.isNullRet(e.Value, ""));
                            //WfResetAmtBySfa06();
                            break;

                        case "cea10"://幣別
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
                            
                        case "cea12":// 匯率
                            if (Convert.ToDecimal(e.Value) <= 0)
                            {
                                WfShowErrorMsg("匯率應大於0!");
                                return false;
                            }
                            break;

                        case "cea11"://收款條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["cea11_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("2", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此收款條件,請檢核!");
                                return false;
                            }
                            e.Row["cea11_c"] = BoBas.OfGetBef03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "cea21"://應收科目類別
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["cea21_c"] = "";
                                return true;
                            }
                            cbaModel = BoCar.OfGetCbaModel(e.Value.ToString());
                            if (cbaModel==null)
                            {
                                WfShowErrorMsg("無此應收科目類別,請檢核!");
                                return false;
                            }
                            e.Row["cea21_c"] = cbaModel.cba02;
                            if (!GlobalFn.varIsNull(cbaModel.cba03))
                            {
                                e.Row["cea22"] = cbaModel.cba03;
                                e.Row["cea22_c"] = BoGlat200.OfGetGba02(cbaModel.cba03);
                            }
                            break;

                        case "cea22"://會計科目
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["cea22_c"] = "";
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
                            e.Row["cea22_c"] = gbaModel.gba02;
                            break;

                    }
                }
                #endregion

                #region 單身 vw_cart100s
                if (e.Row.Table.Prefix.ToLower() == "vw_cart100s")
                {
                    uGrid = sender as UltraGrid;
                    detalModel = e.Row.ToItem<vw_cart100s>();

                    switch (e.Column.ToLower())
                    {
                        case "ceb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            detailList = e.Row.Table.ToList<vw_cart100s>();
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.ceb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "ceb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ceb04"] = "";//品名
                                e.Row["ceb05"] = 0;//數量
                                e.Row["ceb16"] = "";//單位
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }

                            if (WfSetCeb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;
                            break;
                        case "ceb05"://數量
                            if (GlobalFn.varIsNull(detalModel.ceb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["ceb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detalModel.ceb06))
                            {
                                WfShowErrorMsg("請先輸入單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["ceb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入數量!");
                                return false;
                            }
                            detalModel.ceb05 = BoBas.OfGetUnitRoundQty(detalModel.ceb16, detalModel.ceb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detalModel.ceb05;
                            //if (WfChkSfb05(pDr, listDetail) == false)
                            //    return false;
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "ceb16"://訂單單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detalModel.ceb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["ceb03"]);
                                return false;
                            }
                            if (WfChkCeb16(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;

                            break;

                        case "ceb06":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detalModel.ceb06 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detalModel.ceb06, BekTbModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "ceb10"://會計科目
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ceb10_c"] = "";
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
                            e.Row["ceb10_c"] = gbaModel.gba02;
                            break;

                        case "ceb11"://來源單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ceb12"] = DBNull.Value;  //項次
                                e.Row["ceb03"] = "";  //料號
                                e.Row["ceb04"] = "";  //品名
                                e.Row["ceb05"] = 0;  //數量 先以此帶入
                                e.Row["ceb06"] = 0;  //原幣單價
                                e.Row["ceb07"] = 0;  //原幣未稅金額
                                e.Row["ceb07t"] = 0;  //原幣含稅金額
                                //處理小計及總計
                                WfSetDetailAmt(e.Row);
                                WfSetTotalAmt();
                                e.Row["ceb10"] = "";  //會科
                                e.Row["ceb16"] = "";  //單位

                                return true;
                            }
                            if (masterModel.cea00 == "11")  //出貨
                            {
                                sgaModel = BoStp.OfGetSgaModel(e.Value.ToString());
                                if (sgaModel == null)
                                {
                                    WfShowErrorMsg("無此銷貨單號!");
                                    return false;
                                }
                                if (sgaModel.sgaconf != "Y")
                                {
                                    WfShowErrorMsg("銷貨單號,非確認狀態!");
                                    return false;
                                }
                                if (sgaModel.sga03!=masterModel.cea03)
                                {
                                    WfShowErrorMsg("客戶不同,請檢核!");
                                    return false;
                                }
                            }
                            break;

                        case "ceb12"://來源項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ceb03"] = "";  //料號
                                e.Row["ceb04"] = "";  //品名
                                e.Row["ceb05"] = 0;  //數量 先以此帶入
                                e.Row["ceb06"] = 0;  //原幣單價
                                e.Row["ceb07"] = 0;  //原幣未稅金額
                                e.Row["ceb07t"] = 0;  //原幣含稅金額
                                //處理小計及總計
                                WfSetDetailAmt(e.Row);
                                WfSetTotalAmt();
                                e.Row["ceb10"] = "";  //會科
                                e.Row["ceb16"] = "";  //單位
                                return true;
                            }
                            if (masterModel.cea00 == "11")  //出貨
                            {
                                sgbModel = BoStp.OfGetSgbModel(detalModel.ceb11, Convert.ToInt32(e.Value));
                                if (sgbModel == null)
                                {
                                    WfShowErrorMsg("無此來源項次!");
                                    return false;
                                }
                                //暫時先這樣子檢查,後續要考量減掉同單據項次,未寫入量
                                if ((sgbModel.sgb05 - sgbModel.sgb19) < 0)
                                {
                                    WfShowErrorMsg("該項次已無可轉應收量!");
                                    return false;
                                }
                                if (WfSetCeb12Relation(detalModel.ceb11, Convert.ToInt32(detalModel.ceb12), e.Row) == false)
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
                            if (columnName == "ceb02")
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
                                columnName == "ceb03" ||
                                columnName == "ceb05" ||
                                columnName == "ceb06" ||
                                columnName == "ceb10" ||
                                columnName == "ceb11" ||
                                columnName == "ceb12"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            //if (columnName == "sfb02")
                            //{
                            //    if (pDr.RowState == DataRowState.Added)//新增時
                            //        WfSetControlReadonly(ugc, false);
                            //    else    //修改時
                            //    {
                            //        WfSetControlReadonly(ugc, true);
                            //    }
                            //    continue;
                            //}

                            //if (columnName == "sfb03" ||
                            //    columnName == "sfb11" ||
                            //    columnName == "sfb12"
                            //    )
                            //{
                            //    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sfa01"], ""));
                            //    if (babModel == null)
                            //    {
                            //        WfShowErrorMsg("請先輸入訂單頭資料!");
                            //    }
                            //    if (GlobalFn.isNullRet(babModel.bab08, "") == "Y")    //有來源單據
                            //    {
                            //        if (columnName == "sfb03")//料號
                            //            WfSetControlReadonly(ugc, true);    //不可輸入
                            //        else
                            //            WfSetControlReadonly(ugc, false);
                            //    }
                            //    else
                            //    {
                            //        if (columnName == "sfb03")//料號
                            //            WfSetControlReadonly(ugc, false);    //可輸入
                            //        else
                            //            WfSetControlReadonly(ugc, true);
                            //    }
                            //    continue;
                            //}

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

        #region WfPreInInsertModeCheck 進新增模式前的檢查及清變數與設定變數
        protected override bool WfPreInInsertModeCheck()
        {
            try
            {
                //BaaTbModel = BoBas.OfGetBaaModel();

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
            vw_cart100 masterModel;
            try
            {
                masterModel = DrMaster.ToItem<vw_cart100>();

                if (masterModel.ceaconf != "N")
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
            vw_cart100 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_cart100>();
                if (masterModel.ceaconf != "N")
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["cea02"] = Today;
                pDr["cea04"] = LoginInfo.UserNo;
                pDr["cea04_c"] = LoginInfo.UserName;
                pDr["cea05"] = LoginInfo.DeptNo;
                pDr["cea05_c"] = LoginInfo.DeptName;
                pDr["cea07"] = 0;
                pDr["cea08"] = "N";
                pDr["cea10"] = BaaModel.baa04;
                pDr["cea12"] = 1;
                pDr["cea13"] = 0;
                pDr["cea13t"] = 0;
                pDr["cea13g"] = 0;
                pDr["cea14"] = 0;
                pDr["cea15"] = 0;
                pDr["cea15t"] = 0;
                pDr["cea15g"] = 0;
                pDr["cea16"] = 0;
                pDr["cea28"] = 2;   //1.轉入 2.人工
                pDr["ceaconf"] = "N";
                pDr["ceastat"] = "0";
                pDr["ceacomp"] = LoginInfo.CompNo;
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
            string cea21="";
            cba_tb cbaModel=null;
            try
            {

                switch (pCurTabDetail)
                {
                    case 0:
                        pDr["ceb02"] = WfGetMaxSeq(pDr.Table, "ceb02");
                        pDr["ceb05"] = 0;
                        pDr["ceb06"] = 0;
                        pDr["ceb07"] = 0;
                        pDr["ceb07t"] = 0;
                        pDr["ceb08"] = 0;
                        pDr["ceb09"] = 0;
                        pDr["ceb09t"] = 0;
                        cea21 = GlobalFn.isNullRet(DrMaster["cea21"], "");
                        if (cea21 !="")
                        {
                            cbaModel=BoCar.OfGetCbaModel(cea21);
                            if (cbaModel != null)
                                pDr["ceb10"] = cbaModel.cba04;
                        }
                        pDr["ceb13"] = 0;
                        pDr["ceb14"] = 0;
                        pDr["ceb15"] = 0;
                        pDr["cebcomp"] = LoginInfo.CompNo;
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
            vw_cart100 masterModel = null;
            vw_cart100s detailModel = null;
            List<vw_cart100s> detailList = null;
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
                masterModel = DrMaster.ToItem<vw_cart100>();
                if (!GlobalFn.varIsNull(masterModel.cea01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.cea01, ""));
                #region 單頭資料檢查
                chkColName = "cea00";       //帳款類別
                chkControl = ucb_cea00;
                if (GlobalFn.varIsNull(masterModel.cea00))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cea01";       //帳款編號
                chkControl = ute_cea01;
                if (GlobalFn.varIsNull(masterModel.cea01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cea02";       //日期
                chkControl = udt_cea02;
                if (GlobalFn.varIsNull(masterModel.cea02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cea03";       //客戶編號
                chkControl = ute_cea03;
                if (GlobalFn.varIsNull(masterModel.cea03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cea04";       //業務人員
                chkControl = ute_cea04;
                if (GlobalFn.varIsNull(masterModel.cea04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cea05";       //業務部門
                chkControl = ute_cea05;
                if (GlobalFn.varIsNull(masterModel.cea05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cea06";       //課稅別
                chkControl = ucb_cea06;
                if (GlobalFn.varIsNull(masterModel.cea06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cea09";       //發票聯數
                chkControl = ucb_cea09;
                if (GlobalFn.varIsNull(masterModel.cea09))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cea11";
                chkControl = ute_cea11;
                if (GlobalFn.varIsNull(masterModel.cea11))//無來源單據取價條件要輸入
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cea21";       //應收科目類別
                chkControl = ute_cea21;
                if (GlobalFn.varIsNull(masterModel.cea21))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cea22";       //會科編號
                chkControl = ute_cea22;
                if (GlobalFn.varIsNull(masterModel.cea22))
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
                detailList = TabDetailList[iChkDetailTab].DtSource.ToList<vw_cart100s>();
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    detailModel = drTemp.ToItem<vw_cart100s>();
                    chkColName = "ceb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.ceb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ceb11";   //出貨單號
                    if (GlobalFn.varIsNull(detailModel.ceb11) && masterModel.cea00 == "11")
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ceb12";   //出貨項次
                    if (GlobalFn.varIsNull(detailModel.ceb12) && masterModel.cea00 == "11")
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ceb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.ceb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ceb05";   //數量
                    #region ceb05 數量
                    if (GlobalFn.varIsNull(detailModel.ceb05) || detailModel.ceb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "ceb06";   //原幣未稅
                    if (GlobalFn.varIsNull(detailModel.ceb06) || detailModel.ceb06 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ceb10";   //會計科目
                    if (GlobalFn.varIsNull(detailModel.ceb10))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ceb16";   //單位
                    if (GlobalFn.varIsNull(detailModel.ceb16))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    if (GlobalFn.varIsNull(detailModel.ceb05) || detailModel.ceb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    
                    //這裡放全部都有資料的再做檢查
                    if (!GlobalFn.varIsNull(detailModel.ceb11) && !GlobalFn.varIsNull(detailModel.ceb12))
                    {
                        if (masterModel.cea00 == "11")
                        {
                            sgbModel = BoStp.OfGetSgbModel(detailModel.ceb11, Convert.ToInt32(detailModel.ceb12));
                            if (sgbModel == null)
                            {
                                this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                                msg = "查無此出貨單號!";
                                WfShowErrorMsg(msg);
                                WfFindErrUltraGridCell(uGrid, drTemp, "ceb11");
                                return false;
                            }

                            //這裡不管新增跟修改,抓其他單據量時,本身的單別至少有值 所以不會有問題
                            otherDocQty = 0; thisDocQty = 0;
                            sql = @"SELECT SUM(ceb05) FROM cea_tb
                                    INNER JOIN ceb_tb ON cea01=ceb01
                                WHERE ceaconf <>'X'
                                    AND cea01 <>@cea01
                                    AND ceb11=@ceb11 AND ceb12=@ceb12
                               ";
                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@cea01", masterModel.cea01));
                            sqlParmList.Add(new SqlParameter("@ceb11", detailModel.ceb11));
                            sqlParmList.Add(new SqlParameter("@ceb12", Convert.ToInt32(detailModel.ceb12)));
                            otherDocQty = GlobalFn.isNullRet(BoCar.OfGetFieldValue(sql, sqlParmList.ToArray()), 0m);

                            if (TabDetailList[iChkDetailTab].DtSource.Rows.Count > 0)
                            {
                                thisDocQty = detailList.Where(p => p.ceb11 == detailModel.ceb11 && p.ceb12 == detailModel.ceb12)
                                                   .Sum(p => p.ceb05);
                            }
                            if ((sgbModel.sgb05 - otherDocQty - thisDocQty) < 0)
                            {
                                msg = string.Format("出貨單號{0}-{1} 輸入數量{2} 已超過可輸轉應收量{3}"
                                                            , detailModel.ceb11
                                                            , detailModel.ceb12
                                                            , detailModel.ceb05
                                                            , sgbModel.sgb05 - otherDocQty
                                );
                                WfShowErrorMsg(msg);
                                WfFindErrUltraGridCell(uGrid, drTemp, "ceb05");
                                return false;
                            }

                        }
                    }
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
            string cea01New, errMsg;
            vw_cart100 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_cart100>();
                if (FormEditMode == YREditType.新增)
                {
                    cea01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.cea01, ModuleType.car, (DateTime)masterModel.cea02, out cea01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["cea01"] = cea01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["ceasecu"] = LoginInfo.UserNo;
                        DrMaster["ceasecg"] = LoginInfo.GroupNo;
                        DrMaster["ceacreu"] = LoginInfo.UserNo;
                        DrMaster["ceacreg"] = LoginInfo.DeptNo;
                        DrMaster["ceacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["ceamodu"] = LoginInfo.UserNo;
                        DrMaster["ceamodg"] = LoginInfo.DeptNo;
                        DrMaster["ceamodd"] = Now;
                    }
                }
                
                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    drDetail["ceb00"] = masterModel.cea00;
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["cebcreu"] = LoginInfo.UserNo;
                            drDetail["cebcreg"] = LoginInfo.DeptNo;
                            drDetail["cebcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["cebmodu"] = LoginInfo.UserNo;
                            drDetail["cebmodg"] = LoginInfo.DeptNo;
                            drDetail["cebmodd"] = Now;
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
            string sqlSelect,deleteGeaSql="",deleteGebSql = "";
            int chkCnts = 0;
            List<SqlParameter> sqlParmList = null;
            cea_tb masterModel=null;
            try
            {
                masterModel=DrMaster.ToItem<cea_tb>();
                if (masterModel.cea00 == "11") //出貨
                {
                    if (WfUpdSgb19() == false)
                        return false;
                    sqlSelect = @"SELECT COUNT(1) FROM gea_tb
                            WHERE gea01=@gea01 AND gea02='AR'
                                AND gea03=1
                            ";
                    deleteGeaSql = @"DELETE FROM gea_tb 
                                    WHERE gea01=@gea01 AND gea02='AR'  AND gea03=1
                                    ";
                    deleteGebSql = @"DELETE FROM geb_tb 
                                    WHERE geb01=@geb01 AND geb02='AR'  AND geb03=1
                                    ";

                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@gea01", masterModel.cea01));
                    chkCnts = GlobalFn.isNullRet(BoCar.OfGetFieldValue(sqlSelect, sqlParmList.ToArray()),0);
                    if (chkCnts>0)
                    {
                        if (WfShowConfirmMsg("分錄底稿已存在,是否重新新增?")==DialogResult.Yes)
                        {
                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@gea01", masterModel.cea01));
                            BoCar.OfExecuteNonquery(deleteGeaSql,sqlParmList.ToArray());

                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@geb01", masterModel.cea01));
                            BoCar.OfExecuteNonquery(deleteGebSql,sqlParmList.ToArray());
                            var resultList = BoGlat200.OfGenGeaByCea(masterModel.cea01, LoginInfo);
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
                            var resultList = BoGlat200.OfGenGeaByCea(masterModel.cea01, LoginInfo);
                            if (resultList != null && resultList.Count > 0)
                            {
                                WfShowErrorMsg(resultList[0].Message);
                                return false;
                            }
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
                vw_cart100 masterModel;
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
                    case "GenGea":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        WfGenGea(DrMaster["cea01"].ToString());
                        break;

                    case "glat200":
                        if (FormEditMode != YREditType.NA)
                            return; 
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_cart100>();
                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND gea01='{0}'", masterModel.cea01));
                        sbSql.AppendLine(string.Format(" AND gea02='AR' AND gea03=1 AND gea04=1 "));
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
                        
                        masterModel = DrMaster.ToItem<vw_cart100>();
                        if (GlobalFn.varIsNull(masterModel.cea29))
                            return;

                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND gfa01='{0}'", masterModel.cea29));
                        WfShowForm("glat300", false, new object[] { "cart100", this.LoginInfo, sbSql.ToString() });
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
        #region WfSetCea03Relation 設定客戶相關聯
        private void WfSetCea03Relation(string pCea03)
        {
            sca_tb scaModel;
            try
            {
                scaModel = BoStp.OfGetScaModel(pCea03);
                if (scaModel == null)
                    return;

                DrMaster["cea03_c"] = scaModel.sca03;
                DrMaster["cea06"] = scaModel.sca22;    //課稅別
                WfSetCea06Relation(scaModel.sca22);
                DrMaster["cea09"] = scaModel.sca23;    //發票聯數

                DrMaster["cea11"] = scaModel.sca21;    //收款條件
                DrMaster["cea11_c"] = BoBas.OfGetBef03("1", scaModel.sca21);
                DrMaster["cea26"] = pCea03;
                DrMaster["cea27"] = scaModel.sca12;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetCea06Relation 設定稅別關聯
        private void WfSetCea06Relation(string pCea06)
        {
            try
            {
                if (pCea06 == "1")
                {
                    DrMaster["cea07"] = BaaModel.baa05;
                    DrMaster["cea08"] = "Y";
                }
                else if (pCea06 == "2")
                {
                    DrMaster["cea07"] = BaaModel.baa05;
                    DrMaster["cea08"] = "N";
                }
                else
                {
                    DrMaster["cea07"] = 0;
                    DrMaster["cea08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetCeb03Relation 設定料號關聯
        private bool WfSetCeb03Relation(string pCeb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal seb08;
            try
            {
                icaModel = BoInv.OfGetIcaModel(pCeb03);
                seb08 = 0;
                if (icaModel == null)
                {
                    pDr["ceb04"] = "";//品名
                    pDr["ceb05"] = 0;//數量
                    pDr["ceb16"] = "";//單位
                }
                else
                {
                    pDr["ceb04"] = icaModel.ica02;//品名
                    pDr["ceb05"] = 0;//數量
                    pDr["ceb16"] = icaModel.ica09;//單位帶銷售單位
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetCeb12Relatrion
        private bool WfSetCeb12Relation(string pCeb11, int pCeb12, DataRow pDr)
        {
            sgb_tb sgbModel = null;
            cea_tb ceaModel = null;
            ceb_tb cebModel = null;
            List<ceb_tb> cebList = null;
            try
            {
                ceaModel = DrMaster.ToItem<cea_tb>();
                cebModel = pDr.ToItem<ceb_tb>();
                cebList = pDr.Table.ToList<ceb_tb>();
                sgbModel = BoStp.OfGetSgbModel(pCeb11, Convert.ToInt32(pCeb12));

                pDr["ceb03"] = sgbModel.sgb03;  //料號
                pDr["ceb04"] = sgbModel.sgb04;  //品名
                pDr["ceb05"] = sgbModel.sgb05 - sgbModel.sgb19;  //數量 先以此帶入
                pDr["ceb06"] = sgbModel.sgb09;  //原幣單價
                pDr["ceb07"] = sgbModel.sgb10;  //原幣未稅金額
                pDr["ceb07t"] = sgbModel.sgb10;  //原幣含稅金額
                //處理小計及總計
                WfSetDetailAmt(pDr);
                WfSetTotalAmt();

                //pDr["ceb10"] = "";  //會科
                pDr["ceb16"] = sgbModel.sgb06;  //單位

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkSgb19  檢查是否有超過出貨單數量
        //private bool WfChkSgb19(string pSgb01, decimal pSgb02)
        //{
        //    sgb_tb sgbModel = null;
        //    cea_tb ceaModel = null;
        //    ceb_tb cebModel = null;
        //    List<ceb_tb> cebList = null;
        //    try
        //    {
        //        List<ceb_tb> cebList = null;
        //        bek_tb bekLocalModel = null;

        

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        #region WfChkCeb16 檢查單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkCeb16(DataRow pDr, string pCeb16, string pBab08)
        {
            vw_cart100s detailModel;
            try
            {
                detailModel = pDr.ToItem<vw_cart100s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pCeb16, "")) == false)
                {
                    WfShowErrorMsg("無此訂單單位!請確認");
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
        private bool WfSetDetailAmt(DataRow drCeb)
        {
            cea_tb ceaModel;
            ceb_tb cebModel;
            decimal ceb07t = 0, ceb07 = 0;
            decimal ceb08 = 0, ceb09 = 0, ceb09t = 0;
            bek_tb bekLocalModel = null; //本幣
            try
            {
                ceaModel = DrMaster.ToItem<cea_tb>();
                cebModel = drCeb.ToItem<ceb_tb>();
                bekLocalModel = BoBas.OfGetBekModel(BaaModel.baa04);

                if (ceaModel.cea08 == "Y")//稅內含
                {
                    //原幣處理
                    ceb07t = cebModel.ceb05 * cebModel.ceb06;
                    ceb07t = GlobalFn.Round(ceb07t, BekTbModel.bek04);
                    ceb07 = ceb07t / (1 + (ceaModel.cea07 / 100));
                    ceb07 = GlobalFn.Round(ceb07, BekTbModel.bek04);
                    //本幣處理
                    ceb08 = cebModel.ceb06 * ceaModel.cea12;
                    ceb08 = GlobalFn.Round(ceb08, bekLocalModel.bek04);
                    ceb09t = cebModel.ceb05 * ceb08;
                    ceb09t = GlobalFn.Round(ceb09t, bekLocalModel.bek04);
                    ceb09 = ceb09t / (1 + (ceaModel.cea07 / 100));
                    ceb09 = GlobalFn.Round(ceb09, bekLocalModel.bek04);
                }
                else//稅外加
                {
                    ceb07 = cebModel.ceb05 * cebModel.ceb06;
                    ceb07 = GlobalFn.Round(ceb07, BekTbModel.bek04);
                    ceb07t = ceb07 * (1 + (ceaModel.cea07 / 100));
                    ceb07t = GlobalFn.Round(ceb07t, BekTbModel.bek04);
                    //本幣處理
                    ceb08 = cebModel.ceb06 * ceaModel.cea12;
                    ceb08 = GlobalFn.Round(ceb08, bekLocalModel.bek04);
                    ceb09 = cebModel.ceb05 * cebModel.ceb08;
                    ceb09 = GlobalFn.Round(ceb09, BekTbModel.bek04);
                    ceb09t = ceb09 * (1 + (ceaModel.cea07 / 100));
                    ceb09t = GlobalFn.Round(ceb09t, BekTbModel.bek04);
                }
                drCeb["ceb07"] = ceb07;
                drCeb["ceb07t"] = ceb07t;
                drCeb["ceb08"] = ceb08;
                drCeb["ceb09"] = ceb09;
                drCeb["ceb09t"] = ceb09t;

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
            cea_tb ceaModel;
            decimal cea13 = 0, cea13t = 0, cea13g;
            decimal cea15 = 0, cea15t = 0, cea15g;
            bek_tb bekLocalModel = null; //本幣
            bekLocalModel = BoBas.OfGetBekModel(BaaModel.baa04);
            try
            {
                ceaModel = DrMaster.ToItem<cea_tb>();
                if (ceaModel.cea08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (ceb_tb cebModel in TabDetailList[0].DtSource.ToList<ceb_tb>())
                    {
                        cea13t += cebModel.ceb07t;
                        cea15t += cebModel.ceb09t;
                    }
                    cea13t = GlobalFn.Round(cea13t, BekTbModel.bek04);
                    cea13 = cea13t / (1 + ceaModel.cea07 / 100);
                    cea13 = GlobalFn.Round(cea13, BekTbModel.bek04);
                    cea13g = cea13t - cea13;

                    cea15t = GlobalFn.Round(cea15t, bekLocalModel.bek04);
                    cea15 = cea15t / (1 + ceaModel.cea07 / 100);
                    cea15 = GlobalFn.Round(cea15, bekLocalModel.bek04);
                    cea15g = cea15t - cea15;
                }
                else//稅外加
                {
                    foreach (ceb_tb cebModel in TabDetailList[0].DtSource.ToList<ceb_tb>())
                    {
                        cea13 += cebModel.ceb07;
                        cea15 += cebModel.ceb09;
                    }
                    cea13 = GlobalFn.Round(cea13, BekTbModel.bek04);
                    cea13g = cea13 * (ceaModel.cea07 / 100);
                    cea13g = GlobalFn.Round(cea13g, BekTbModel.bek04);
                    cea13t = cea13 + cea13g;

                    cea15 = GlobalFn.Round(cea15, BekTbModel.bek04);
                    cea15g = cea15 * (ceaModel.cea07 / 100);
                    cea15g = GlobalFn.Round(cea15g, bekLocalModel.bek04);
                    cea15t = cea15 + cea15g;
                }

                DrMaster["cea13"] = cea13;
                DrMaster["cea13t"] = cea13t;
                DrMaster["cea13g"] = cea13g;

                DrMaster["cea15"] = cea15;
                DrMaster["cea15t"] = cea15t;
                DrMaster["cea15g"] = cea15g;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfUpdSgb19 更新出貨單已轉應收數量 存檔更新
        private bool WfUpdSgb19()
        {
            List<ceb_tb> cebList = null;
            List<SqlParameter> sqlParmList;
            decimal ceb05 = 0;
            string cea00;
            string sqlSumCeb = "", sqlUpdSgb17 = "";
            try
            {
                cea00 = GlobalFn.isNullRet(DrMaster["cea00"], "");
                cebList = TabDetailList[0].DtSource.ToList<ceb_tb>();

                sqlSumCeb = @"SELECT SUM(ceb05)  FROM cea_tb
                        INNER JOIN ceb_tb ON cea01=ceb01
                        WHERE ceb11=@ceb11 AND ceb12=@ceb12
                                AND ceaconf <>'X'
                    ";
                sqlUpdSgb17 = @"UPDATE sgb_tb SET sgb17=@sgb17
                        WHERE sgb01=@sgb01 AND sgb02=@sgb02
                    ";

                if (cea00 == "11")
                {
                    var distinctCeb = from o in cebList
                                      where !GlobalFn.varIsNull(o.ceb11)
                                      group o by new
                                      {
                                          ceb11 = o.ceb11,
                                          ceb12 = o.ceb12
                                      } into g
                                      orderby g.Key.ceb11, g.Key.ceb12
                                      select new
                                      {
                                          ceb11 = g.Key.ceb11,
                                          ceb12 = g.Key.ceb12
                                      };
                    foreach (var result in distinctCeb)
                    {
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@ceb11", result.ceb11));
                        sqlParmList.Add(new SqlParameter("@ceb12", result.ceb12));
                        ceb05 = GlobalFn.isNullRet(BoCar.OfGetFieldValue(sqlSumCeb, sqlParmList.ToArray()), 0m);

                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@sgb01", result.ceb11));
                        sqlParmList.Add(new SqlParameter("@sgb02", result.ceb12));
                        sqlParmList.Add(new SqlParameter("@sgb17", ceb05));
                        if (BoCar.OfExecuteNonquery(sqlUpdSgb17, sqlParmList.ToArray()) != 1)
                        {
                            WfShowErrorMsg("更新出貨單轉應收量失敗!");
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

        #region WfGenGea 產生分錄底稿
        private bool WfGenGea(string pCea01)
        {
            vw_cart100 masterModel = null;
            int chkCnts = 0;
            string sqlSelect, deleteGeaSql = "", deleteGebSql = "";
            List<SqlParameter> sqlParmList = null;
            try
            {
                WfRetrieveMaster();
                masterModel=DrMaster.ToItem<vw_cart100>();
                if (masterModel==null)
                {
                    WfShowErrorMsg("無資料可產生分錄底稿!");
                    return false;
                }
                if (masterModel.ceaconf!="N")
                {
                    WfShowErrorMsg("非未確認狀態,不可更新分錄底稿!");
                    return false;
                }
                if (masterModel.cea00=="21" ||masterModel.cea00=="22" )
                {
                    WfShowErrorMsg("待抵單不拋轉分錄!");
                    return false;
                }

                sqlSelect = @"SELECT COUNT(1) FROM gea_tb
                            WHERE gea01=@gea01 AND gea02='AR'
                                AND gea03=1
                            ";
                deleteGeaSql = @"DELETE FROM gea_tb 
                                    WHERE gea01=@gea01 AND gea02='AR'  AND gea03=1
                                    ";
                deleteGebSql = @"DELETE FROM geb_tb 
                                    WHERE geb01=@geb01 AND geb02='AR'  AND geb03=1
                                    ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gea01", masterModel.cea01));
                chkCnts = GlobalFn.isNullRet(BoCar.OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0);
                
                if (chkCnts>0)
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
                        sqlParmList.Add(new SqlParameter("@gea01", masterModel.cea01));
                        BoCar.OfExecuteNonquery(deleteGeaSql, sqlParmList.ToArray());

                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@geb01", masterModel.cea01));
                        BoCar.OfExecuteNonquery(deleteGebSql, sqlParmList.ToArray());
                        var resultDeleteList = BoGlat200.OfGenGeaByCea(masterModel.cea01, LoginInfo);
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

                    var resultGenList = BoGlat200.OfGenGeaByCea(pCea01, LoginInfo);
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

        #region WfConfirm 確認
        private void WfConfirm()
        {
            cea_tb ceaModel = null;

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

                ceaModel = DrMaster.ToItem<cea_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                ceaModel.ceaconf = "Y";
                ceaModel.ceacond = Today;
                ceaModel.ceaconu = LoginInfo.UserNo;

                DrMaster["ceastat"] = "1";
                DrMaster["ceaconf"] = "Y";
                DrMaster["ceacond"] = Today;
                DrMaster["ceaconu"] = LoginInfo.UserNo;
                DrMaster["ceamodu"] = LoginInfo.UserNo;
                DrMaster["ceamodg"] = LoginInfo.DeptNo;
                DrMaster["ceamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                ceaModel = DrMaster.ToItem<cea_tb>();
                WfSetDocPicture("", ceaModel.ceaconf, "", pbxDoc);
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
            vw_cart100 masterModel = null;
            vw_cart100s detailModel = null;
            List<vw_cart100s> detailList = null;
            StringBuilder sbSql;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_cart100>();
                if (masterModel.ceaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }

                if (masterModel.cea00 == "21" || masterModel.cea00 == "22")
                {
                    WfShowErrorMsg("待抵單不可取消確認!");
                    return false;
                }

                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.cea02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_cart100s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_cart100s>();
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
            cea_tb ceaModel = null;
            ceb_tb cebModel = null;
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
                ceaModel = DrMaster.ToItem<cea_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }
                

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    cebModel = dr.ToItem<ceb_tb>();
                }

                DrMaster["ceaconf"] = "N";
                DrMaster["ceacond"] = DBNull.Value;
                DrMaster["ceaconu"] = "";
                DrMaster["ceamodu"] = LoginInfo.UserNo;
                DrMaster["ceamodg"] = LoginInfo.DeptNo;
                DrMaster["ceamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                ceaModel = DrMaster.ToItem<cea_tb>();
                WfSetDocPicture("", ceaModel.ceaconf, "", pbxDoc);
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
            vw_cart100 masterModel = null;
            List<ceb_tb> cebList = null;
            decimal icc05;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_cart100>();
                if (masterModel.ceaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.cea02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                if (!GlobalFn.varIsNull(masterModel.cea29))
                {
                    WfShowErrorMsg("單據已拋轉傳票!");
                    return false;
                }
                if (masterModel.cea14>0 || masterModel.cea16>0)
                {
                    WfShowErrorMsg("已有沖帳記錄,不可還原!");
                    return false;
                }

                //檢查是否存在銷退單
                //if (WfChkShaExists(masterModel.sga01) == true)
                //{
                //    WfShowErrorMsg("已存在銷退單,不可取消確認!");
                //    return false;
                //}

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
            vw_cart100 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_cart100>();

                if (masterModel.ceaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.ceaconf == "N")//走作廢
                {

                    DrMaster["ceaconf"] = "X";
                    DrMaster["ceaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.ceaconf == "X")
                {
                    DrMaster["ceaconf"] = "N";
                    DrMaster["ceaconu"] = "";
                }
                DrMaster["ceamodu"] = LoginInfo.UserNo;
                DrMaster["ceamodg"] = LoginInfo.DeptNo;
                DrMaster["ceamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_cart100>();
                WfSetDocPicture("", masterModel.ceaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfGenVoucher 拋轉傳票
        private bool WfGenVoucher()
        {
            vw_cart100 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_cart100>();
                if (masterModel.ceaconf != "Y")
                {
                    WfShowBottomStatusMsg("應收帳款未確認!");
                    return false;
                }
                
                if (!GlobalFn.varIsNull(masterModel.cea29))
                {
                    WfShowBottomStatusMsg("已拋轉傳票,不可重覆拋轉!");
                    return false;
                }
                cacModel = BoCar.OfGetCacModel(masterModel.cea01);
                if (cacModel==null)
                {
                    WfShowBottomStatusMsg("取得應收單別資料錯誤!");
                    return false;
                }
                if (cacModel.cac08!="Y")
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
                carb350Model.gea01 = masterModel.cea01;
                carb350Model.gea03 = 1;                
                
                var resultList = boCarb350.OfGenVoucher(carb350Model,  "", LoginInfo);

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
                WfShowBottomStatusMsg("執行成功!");
                return true;
            }
            catch (Exception ex)
            {
                if (BoMaster.TRAN != null)
                    BoMaster.TRAN.Rollback();
                WfRetrieveMaster();
                WfShowErrorMsg(ex.Message);
                return false;
            }
        }
        #endregion

        #region WfUndoGenVoucher 拋轉傳票還原
        private bool WfUndoGenVoucher()
        {
            vw_cart100 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_cart100>();

                if (GlobalFn.varIsNull(masterModel.cea29))
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
                carb351Model.gfa01 = masterModel.cea29;
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
                WfShowBottomStatusMsg("執行成功!");
                return true;
            }
            catch (Exception ex)
            {
                if (BoMaster.TRAN != null)
                    BoMaster.TRAN.Rollback();
                WfRetrieveMaster();
                WfShowErrorMsg(ex.Message);
                throw ex;
            }
        } 
        #endregion

    }
}
