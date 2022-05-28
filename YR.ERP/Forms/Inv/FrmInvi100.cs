/* 程式名稱: 料號基本資料建立作業
   系統代號: invi100
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
using Infragistics.Win.UltraWinToolbars;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;
using System.Drawing;
using System.IO;
using bpac;
using YR.Util;


namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvi100 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        PurBLL BoPur = null;
        string Bga01 = "", Bgc03 = "", Ica01New = "";          //新增時在存檔前用來重新取號使用
        #endregion

        #region 建構子
        public FrmInvi100()
        {
            InitializeComponent();
        }


        public FrmInvi100(string pSourceForm, YR.ERP.Shared.UserInfo pUserInfo, string pWhere)
        {
            InitializeComponent();
            StrQueryWhereAppend = pWhere;
            this.LoginInfo = pUserInfo;
            this.WindowState = FormWindowState.Maximized;
        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            this.StrFormID = "invi100";
            uTab_Header.Tabs[0].Text = "基本資料";
            uTab_Header.Tabs[1].Text = "狀態";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "一般資料";
            uTab_Master.Tabs[1].Text = "採購資料";
            uTab_Master.Tabs[2].Text = "資料瀏覽";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ica01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "icasecu";
                TabMaster.GroupColumn = "icasecg";
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

            BoInv = new InvBLL(BoMaster.OfGetConntion());
            BoBas = new BasBLL(BoMaster.OfGetConntion());
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
                    BoInv.TRAN = BoMaster.TRAN;
                    BoBas.TRAN = BoMaster.TRAN;
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
                //課稅別
                sourceList = BoInv.OfGetIca17KVPList();
                WfSetUcomboxDataSource(ucb_ica17, sourceList);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示
        protected override Boolean WfDisplayMode()
        {
            vw_invi100 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_invi100>();
                    WfSetDocPicture(masterModel.icavali, masterModel.icaconf, "", pbxDoc);
                    WfLoadIcp03(masterModel.ica01);
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
                    WfSetControlsReadOnlyRecursion(this, false);
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_icacreu, ute_icacreg, udt_icacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_icamodu, ute_icamodg, udt_icamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_icasecu, ute_icasecg }, true);

                    WfSetControlReadonly(ute_icc05_tot, true);
                    WfSetControlReadonly(ute_ica04_c, true);
                    WfSetControlReadonly(ute_ica05_c, true);
                    WfSetControlReadonly(ute_ica06_c, true);
                    WfSetControlReadonly(ute_ica16, true);
                    WfSetControlReadonly(ute_ica22_c, true);
                    WfSetControlReadonly(ute_ica23_c, true);
                    WfSetControlReadonly(ute_icavali, true);
                    WfSetControlReadonly(ucx_icaconf, true);
                    WfSetControlReadonly(new List<Control> { ute_ica18, udt_ica20 }, true);


                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_ica01, true);
                        if (masterModel.icaconf == "Y")
                        {
                            WfSetControlReadonly(new List<Control> { ute_ica07, ute_ica08, ute_ica09 }, true);
                            WfSetControlReadonly(ucb_ica17, true);
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
                pDr["ica07"] = "PCS";//庫存單位
                pDr["ica08"] = "PCS";//採購單位
                pDr["ica09"] = "PCS";//銷售單位
                pDr["ica12"] = 0;
                pDr["ica13"] = 0;
                pDr["ica14"] = 0;
                pDr["ica15"] = 0;
                pDr["ica16"] = 0;
                pDr["ica17"] = "P"; //預設為採購件
                pDr["ica18"] = 0;
                pDr["ica19"] = 0;
                pDr["ica23"] = 0;
                pDr["ica24"] = 0;
                pDr["ica26"] = 0;
                pDr["ica29"] = "Y"; //預設都是要計算成本的
                pDr["ica35"] = "N"; //預設是不做多原廠料件的
                pDr["icaconf"] = "N";
                pDr["icavali"] = "W";//待確認
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreInInsertModeCheck 進新增模式前的檢查及清變數
        protected override bool WfPreInInsertModeCheck()
        {
            try
            {
                Ica01New = "";
                Bga01 = "";
                Bgc03 = "";
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
            string bga01 = "", bgc03 = "";
            string ica01New;
            int iChkCnts = 0;
            string errMsg;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_invi100
                if (pDr.Table.Prefix.ToLower() == "vw_invi100")
                {
                    switch (pColName.ToLower())
                    {
                        case "ica01":
                            #region ica01 料件編號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bga03", "3"));
                            WfShowPickUtility("p_bga1", messageModel);
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel != null && messageModel.DataRowList.Count > 0)
                                {
                                    bga01 = GlobalFn.isNullRet(messageModel.DataRowList[0]["bga01"], "");
                                    //檢查是否有使用分類編碼
                                    sbSql = new StringBuilder();
                                    sbSql.AppendLine("SELECT COUNT(1) FROM bgb_tb");
                                    sbSql.AppendLine("WHERE bgb01=@bgb01 AND bgb05='3'");
                                    sqlParmList = new List<SqlParameter>();
                                    sqlParmList.Add(new SqlParameter("@bgb01", bga01));
                                    iChkCnts = GlobalFn.isNullRet(BoMaster.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                                    if (iChkCnts > 0)
                                    {
                                        messageModel = new MessageInfo();
                                        messageModel.IsAutoQuery = true;
                                        messageModel.ParamSearchList = new List<SqlParameter>();
                                        messageModel.ParamSearchList.Add(new SqlParameter("@bgc01", bga01));

                                        WfShowPickUtility("p_bgc1", messageModel);
                                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                        {
                                            if (messageModel.DataRowList.Count > 0)
                                                bgc03 = GlobalFn.isNullRet(messageModel.DataRowList[0]["bgc03"], "");
                                            else
                                                bgc03 = "";
                                        }
                                        else
                                            break;
                                    }
                                    if (BoBas.OfGetBga01AutoNo(bga01, bgc03, out ica01New, out errMsg) == false)
                                    {
                                        WfShowErrorMsg(errMsg);
                                        break;
                                    }
                                    Ica01New = ica01New;
                                    Bga01 = bga01;
                                    Bgc03 = bgc03;
                                    pDr["ica01"] = ica01New;
                                    pDr["ica04"] = ica01New.Substring(0, 1);
                                    pDr["ica05"] = ica01New.Substring(0, 3);
                                    pDr["ica06"] = ica01New.Substring(0, 6);

                                }
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                            #endregion
                        case "ica04"://大分類
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("icx01", "1"));
                            WfShowPickUtility("p_icx1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icx02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ica05"://中分類
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("icx01", "2"));
                            WfShowPickUtility("p_icx1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icx02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ica06"://中分類
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("icx01", "3"));
                            WfShowPickUtility("p_icx1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icx02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ica07"://庫存單位
                            WfShowPickUtility("p_bej", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ica08"://採購單位
                            WfShowPickUtility("p_bej", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ica09"://銷售單位
                            WfShowPickUtility("p_bej", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "ica22"://主要供應商
                            WfShowPickUtility("p_pca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pca01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;

                        case "ica23"://代理商
                            WfShowPickUtility("p_pca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pca01"], "");
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
        //回傳值 true.通過驗證 false.未通過驗證,
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            vw_invi100 masterModel = null;
            try
            {
                #region 單頭-pick vw_invi020
                if (e.Row.Table.Prefix.ToLower() == "vw_invi100")
                {
                    masterModel = DrMaster.ToItem<vw_invi100>();
                    switch (e.Column.ToLower())
                    {
                        case "ica04"://大分類
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ica04_c"] = "";
                                return true;
                            }
                            if (BoInv.OfChkIcxPKExists("1", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此大分類存在,請檢核!");
                                return false;
                            }
                            e.Row["ica04_c"] = BoInv.OfGetIcx03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ica05"://中分類
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ica05_c"] = "";
                                return true;
                            }
                            if (BoInv.OfChkIcxPKExists("2", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此中分類存在,請檢核!");
                                return false;
                            }
                            e.Row["ica05_c"] = BoInv.OfGetIcx03("2", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ica06"://小分類
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ica06_c"] = "";
                                return true;
                            }
                            if (BoInv.OfChkIcxPKExists("3", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此小分類存在,請檢核!");
                                return false;
                            }
                            e.Row["ica06_c"] = BoInv.OfGetIcx03("3", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ica07"://庫存單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此單位存在,請檢核!");
                                return false;
                            }

                            if (GlobalFn.isNullRet(masterModel.ica08, "") == "")
                                e.Row["ica08"] = e.Value;
                            if (GlobalFn.isNullRet(masterModel.ica09, "") == "")
                                e.Row["ica09"] = e.Value;
                            break;

                        case "ica08"://採購單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此單位存在,請檢核!");
                                return false;
                            }

                            break;
                        case "ica09"://銷售單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此單位存在,請檢核!");
                                return false;
                            }
                            break;
                        case "ica10"://未稅訂價
                            //if (BaaModel != null && GlobalFn.isNullRet(BaaModel.baa05, 0) != 0)
                            //{
                            //    e.Row["ica11"] = GlobalFn.isNullRet(e.Value, 0) * (1 + Convert.ToDouble(BaaModel.baa05) * 0.01);
                            //}
                            break;
                        case "ica12"://材料成本
                            WfSetIca16();
                            break;
                        case "ica13"://人工成本
                            WfSetIca16();
                            break;
                        case "ica14"://製造成本
                            WfSetIca16();
                            break;
                        case "ica15"://管銷成本
                            WfSetIca16();
                            break;

                        case "ica19"://市價
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入數字!");
                                return false;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0m) < 0)
                            {
                                WfShowErrorMsg("金額需大於0!");
                                return false;
                            }
                            e.Row["ica20"] = Today;
                            break;
                        case "ica22"://主要供應商
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ica22_c"] = "";
                                return true;
                            }
                            if (BoPur.OfChkPcaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此廠商資料,請檢核!");
                                return false;
                            }
                            DrMaster["ica22_c"] = BoPur.OfGetPca02(e.Value.ToString());
                            break;

                        case "ica23"://代理商
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ica23_c"] = "";
                                return true;
                            }
                            if (BoPur.OfChkPcaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此廠商資料,請檢核!");
                                return false;
                            }
                            DrMaster["ica23_c"] = BoPur.OfGetPca02(e.Value.ToString());
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
            vw_invi100 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_invi100>();
                #region 單頭資料檢查
                chkColName = "ica01";//料號
                chkControl = ute_ica01;
                if (GlobalFn.varIsNull(masterModel.ica01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ica02";//品名
                chkControl = ute_ica02;
                if (GlobalFn.varIsNull(masterModel.ica02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ica07";//庫存單位
                chkControl = ute_ica07;
                if (GlobalFn.varIsNull(masterModel.ica07))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ica08";//採購單位
                chkControl = ute_ica08;
                if (GlobalFn.varIsNull(masterModel.ica08))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ica09";//銷售單位
                chkControl = ute_ica09;
                if (GlobalFn.varIsNull(masterModel.ica09))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ica17";//料件屬性
                chkControl = ucb_ica17;
                if (GlobalFn.varIsNull(masterModel.ica17))
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
            string ica01New, errMsg;
            try
            {
                if (FormEditMode == YREditType.新增 && Ica01New == GlobalFn.isNullRet(DrMaster["ica01"], ""))     //避免搶號,重新取號
                {
                    if (BoBas.OfGetBga01AutoNo(Bga01, Bgc03, out ica01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["ica01"] = ica01New;
                }
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["icasecu"] = LoginInfo.UserNo;
                        DrMaster["icasecg"] = LoginInfo.GroupNo;
                        DrMaster["icacreu"] = LoginInfo.UserNo;
                        DrMaster["icacreg"] = LoginInfo.DeptNo;
                        DrMaster["icacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["icamodu"] = LoginInfo.UserNo;
                        DrMaster["icamodg"] = LoginInfo.DeptNo;
                        DrMaster["icamodd"] = Now;
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

                bt = new ButtonTool("invi101");
                bt.SharedProps.Caption = "料件單位換算設定";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("invi102");
                bt.SharedProps.Caption = "料件圖檔檢視";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("invi103");
                bt.SharedProps.Caption = "多原廠料件設定";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("stpi031");
                bt.SharedProps.Caption = "料件客戶價格設定";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("PrintLabel");
                bt.SharedProps.Caption = "列印標籤";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);


                bt = new ButtonTool("PrintLabelMoney");
                bt.SharedProps.Caption = "列印標籤(含價格)";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("PrintLabelShelf");
                bt.SharedProps.Caption = "列印貨架商品標籤";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);


                bt = new ButtonTool("PrintLabelShelfQR");
                bt.SharedProps.Caption = "列印貨架商品標籤QR";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("Line1");
                bt.SharedProps.Caption = "=================================";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);


                bt = new ButtonTool("GenOldMateriel");
                bt.SharedProps.Caption = "新增B料號";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("Line2");
                bt.SharedProps.Caption = "=================================";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("invq210");
                bt.SharedProps.Caption = "各倉庫存明細查詢";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("invi100_1");
                bt.SharedProps.Caption = "進貨歷史單價查詢";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("invi100_2");
                bt.SharedProps.Caption = "報價單歷史查詢";
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
            vw_invi100 masterModel;
            StringBuilder sbSql;
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

                    case "invi101":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_invi100>();
                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND ica01='{0}'", masterModel.ica01));
                        WfShowForm("invi101", false, new object[] { "invi100", this.LoginInfo, sbSql.ToString() });
                        break;

                    case "invi102":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_invi100>();
                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND icp01='{0}'", masterModel.ica01));
                        WfShowForm("invi102", false, new object[] { "invi100", this.LoginInfo, sbSql.ToString() });
                        break;

                    case "invi103"://多原廠料件管理
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_invi100>();
                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND ica01='{0}'", masterModel.ica01));
                        WfShowForm("invi103", false, new object[] { "invi100", this.LoginInfo, sbSql.ToString() });
                        break;

                    case "stpi031"://料件客戶價格設定
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_invi100>();
                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND ica01='{0}' ", masterModel.ica01));
                        WfShowForm("stpi031", false, new object[] { "invi100", this.LoginInfo, sbSql.ToString() });
                        break;

                    case "PrintLabel":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_invi100>();
                        WfPrintLabel(masterModel);
                        break;
                    case "PrintLabelMoney":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_invi100>();
                        WfPrintLabelMoney(masterModel);
                        break;

                    case "PrintLabelShelf":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_invi100>();
                        WfPrintLabelShelf(masterModel);
                        break;


                    case "PrintLabelShelfQR":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_invi100>();
                        WfPrintLabelShelfQR(masterModel);
                        break;

                    case "GenOldMateriel":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        var icaModel = DrMaster.ToItem<ica_tb>();
                        WFGenOldMateriel(icaModel);
                        break;

                    case "invq210":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_invi100>();
                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND ica01='{0}' ", masterModel.ica01));
                        WfShowForm("invq210", false, new object[] { "invi100", this.LoginInfo, sbSql.ToString() });
                        break;

                    case "invi100_1":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_invi100>();
                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND pgb03='{0}' ", masterModel.ica01));
                        WfShowForm("invi100_1", false, new object[] { "invi100", this.LoginInfo, sbSql.ToString() });
                        break;

                    case "invi100_2":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_invi100>();
                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND pfb03='{0}' ", masterModel.ica01));
                        WfShowForm("invi100_2", false, new object[] { "invi100", this.LoginInfo, sbSql.ToString() });
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreDeleteCheck 進主檔刪除前檢查
        //todo:料號刪除檢查需再補入不同公司別處理
        protected override bool WfPreDeleteCheck(DataRow pDr)
        {
            vw_invi100 masterModel;
            string sqlSelect;
            int chkCnts;
            List<SqlParameter> sqlParmsList;
            try
            {
                masterModel = DrMaster.ToItem<vw_invi100>();
                if (masterModel.icaconf == "Y")
                {
                    WfShowBottomStatusMsg("料件已確認,不可刪除!");
                    return false;
                }
                //領發料+收料
                sqlSelect = @"SELECT COUNT(1) FROM igb_tb WHERE igb03=@igb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@igb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在領發料單據,不可刪除!");
                    return false;
                }
                //調撥單
                sqlSelect = @"SELECT COUNT(1) FROM ifb_tb WHERE ifb03=@ifb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@ifb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在調撥單據,不可刪除!");
                    return false;
                }
                //借出單
                sqlSelect = @"SELECT COUNT(1) FROM ilb_tb WHERE ilb03=@ilb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@ilb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在借出單據,不可刪除!");
                    return false;
                }
                //借出歸還單
                sqlSelect = @"SELECT COUNT(1) FROM imb_tb WHERE imb03=@imb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@imb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在借出歸還單據,不可刪除!");
                    return false;
                }
                //盤點單
                sqlSelect = @"SELECT COUNT(1) FROM ipb_tb WHERE ipb03=@ipb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@ipb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在盤點單,不可刪除!");
                    return false;
                }

                //報價單
                sqlSelect = @"SELECT COUNT(1) FROM seb_tb WHERE seb03=@seb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@seb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在報價單,不可刪除!");
                    return false;
                }
                //訂單
                sqlSelect = @"SELECT COUNT(1) FROM sfb_tb WHERE sfb03=@sfb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@sfb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在客戶訂單,不可刪除!");
                    return false;
                }
                //出貨
                sqlSelect = @"SELECT COUNT(1) FROM sgb_tb WHERE sgb03=@sgb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@sgb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在銷貨單,不可刪除!");
                    return false;
                }
                //銷退單
                sqlSelect = @"SELECT COUNT(1) FROM shb_tb WHERE shb03=@shb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@shb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在銷退單,不可刪除!");
                    return false;
                }

                //請購單
                sqlSelect = @"SELECT COUNT(1) FROM peb_tb WHERE peb03=@peb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@peb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在請購單,不可刪除!");
                    return false;
                }

                //採購單
                sqlSelect = @"SELECT COUNT(1) FROM pfb_tb WHERE pfb03=@pfb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@pfb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在採購單,不可刪除!");
                    return false;
                }

                //進貨單
                sqlSelect = @"SELECT COUNT(1) FROM pgb_tb WHERE pgb03=@pgb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@pgb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在進貨單,不可刪除!");
                    return false;
                }

                //退貨單
                sqlSelect = @"SELECT COUNT(1) FROM phb_tb WHERE phb03=@phb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@phb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在退貨單,不可刪除!");
                    return false;
                }

                //退貨單
                sqlSelect = @"SELECT COUNT(1) FROM phb_tb WHERE phb03=@phb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@phb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在退貨單,不可刪除!");
                    return false;
                }

                //託工單單頭
                sqlSelect = @"SELECT COUNT(1) FROM mea_tb WHERE mea20=@mea20";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@mea20", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在託工單主件,不可刪除!");
                    return false;
                }

                //託工單單身
                sqlSelect = @"SELECT COUNT(1) FROM meb_tb WHERE meb03=@meb03";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@meb03", masterModel.ica01));
                chkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParmsList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    WfShowErrorMsg("料件已存在託工單子件,不可刪除!");
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

        #region WfDeleteAppenUpdate 刪除時使用,若需單身資料,要先在查此查詢資料庫並且異動
        protected override bool WfDeleteAppenUpdate(DataRow pDr)
        {
            vw_invi100 masterModel;
            string deleteSql;
            List<SqlParameter> sqlParmsList;
            int chkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_invi100>();
                //刪除料號圖檔
                deleteSql = @"
                           DELETE FROM icp_tb
                           WHERE icp01=@icp01
                           ";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@icp01", masterModel.ica01));
                chkCnts = BoMaster.OfExecuteNonquery(deleteSql, sqlParmsList.ToArray());
                if (chkCnts < 0)
                {
                    WfShowErrorMsg("刪除料號圖檔失敗(icp_tb)");
                    return false;
                }

                //刪除料號單位設定
                deleteSql = @"
                           DELETE FROM icn_tb
                           WHERE icn01=@icn01
                           ";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@icn01", masterModel.ica01));
                chkCnts = BoMaster.OfExecuteNonquery(deleteSql, sqlParmsList.ToArray());
                if (chkCnts < 0)
                {
                    WfShowErrorMsg("刪除料號圖檔失敗(icn_tb)");
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

        //*****************************表單自訂Fuction****************************************

        #region WfSetIca16 設定ica16的值
        private void WfSetIca16()
        {
            vw_invi100 masterModel;
            try
            {
                if (DrMaster == null)
                    return;
                masterModel = DrMaster.ToItem<vw_invi100>();

                masterModel.ica16 = masterModel.ica12 + masterModel.ica13 + masterModel.ica14 + masterModel.ica15;
                DrMaster["ica16"] = masterModel.ica16;
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
            vw_invi100 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)        //這裡會LOCK資料
                    return;
                if (WfLockMasterRow() == false)     //這裡會LOCK資料,並且做交易
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_invi100>();

                if (masterModel.icavali == "N")
                {
                    WfShowErrorMsg("料號已失效!");
                    WfRollback();
                    return;
                }

                if (masterModel.icaconf != "N")
                {
                    WfShowErrorMsg("料號非未確認狀態!");
                    WfRollback();
                    return;
                }

                DrMaster["icaconf"] = "Y";
                DrMaster["icavali"] = "Y";
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_invi100>();
                WfSetDocPicture(masterModel.icavali, masterModel.icaconf, "", pbxDoc);
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
            vw_invi100 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false)     //這裡會LOCK資料,並且做交易
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_invi100>();

                if (masterModel.icavali == "N")
                {
                    WfShowErrorMsg("料號已失效!");
                    WfRollback();
                    return;
                }

                if (masterModel.icaconf != "Y")
                {
                    WfShowErrorMsg("料號非已確認狀態!");
                    WfRollback();
                    return;
                }

                DrMaster["icaconf"] = "N";
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_invi100>();
                WfSetDocPicture(masterModel.icavali, masterModel.icaconf, "", pbxDoc);
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
            vw_invi100 masterModel = null;
            string msg;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)        //這裡會LOCK資料
                    return;
                if (WfLockMasterRow() == false)     //這裡會LOCK資料,並且做交易
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_invi100>();


                if (masterModel.icavali == "Y")
                    msg = "是否要作廢料號?";
                else
                    msg = "是否要作廢還原料號?";

                var result = WfShowConfirmMsg(msg);
                //if (WfShowConfirmMsg(msg) != 1)
                if (result != DialogResult.Yes)
                    return;


                if (masterModel.icavali == "Y" || masterModel.icavali == "W")//走作廢
                {
                    DrMaster["icavali"] = "N";
                }
                else
                {
                    DrMaster["icavali"] = "Y";
                }

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_invi100>();
                WfSetDocPicture(masterModel.icavali, masterModel.icaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region 取得料件圖檔
        private void WfLoadIcp03(string ica01)
        {
            string selectSql;
            object icp03;
            List<SqlParameter> sqlParmsList;
            try
            {
                selectSql = @"
                            SELECT TOP 1 icp03
                            FROM icp_tb
                            WHERE icp01=@icp01
                            ORDER BY ISNULL(icp06,'N') DESC,
                                     icp05
                           ";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@icp01", ica01));
                icp03 = BoInv.OfGetFieldValue(selectSql, sqlParmsList.ToArray());
                if (icp03 == null)
                {
                    pbx_icp03.Image = null;
                    pbxUsed.Image = null;
                    return;
                }

                pbx_icp03.Image = Image.FromStream(new MemoryStream((byte[])icp03));
                WfSetUsedPicture(ica01);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPrintLabel 列印標籤
        private void WfPrintLabel(vw_invi100 masterModel)
        {
            try
            {
                const string TEMPLATE_DIRECTORY = @"Label\";	// Template file path
                const string TEMPLATE_SIMPLE = "barcode.LBX";	// Template file name

                string templatePath = TEMPLATE_DIRECTORY;
                // None decoration frame
                //if (cmbTemplate.SelectedIndex == 0)
                //{
                //    templatePath += TEMPLATE_SIMPLE;
                //}
                //// Decoration frame
                //else
                //{
                //    templatePath += TEMPLATE_FRAME;
                //}
                templatePath = TEMPLATE_DIRECTORY + TEMPLATE_SIMPLE;
                bpac.DocumentClass doc = new DocumentClass();

                if (doc.Open(templatePath) != false)
                {
                    doc.GetObject("ica01").Text = masterModel.ica01;
                    doc.GetObject("ica02").Text = masterModel.ica02;

                    // doc.SetMediaById(doc.Printer.GetMediaId(), true);
                    doc.StartPrint("", PrintOptionConstants.bpoDefault);
                    doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                    doc.EndPrint();
                    doc.Close();
                }
                else
                {
                    MessageBox.Show("Open() Error: " + doc.ErrorCode);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        #region WfPrintLabelMoney 列印標籤
        private void WfPrintLabelMoney(vw_invi100 masterModel)
        {
            try
            {
                const string TEMPLATE_DIRECTORY = @"Label\";	// Template file path
                string TEMPLATE_SIMPLE = "barcode_money.LBX";	// Template file name

                string templatePath = TEMPLATE_DIRECTORY;
                templatePath = TEMPLATE_DIRECTORY + TEMPLATE_SIMPLE;
                bpac.DocumentClass doc = new DocumentClass();




                if (masterModel.ica01.Length == 11 && masterModel.ica01.Substring(10, 1) == "B")
                {
                    TEMPLATE_SIMPLE = "barcode_money_used.LBX";
                    templatePath = TEMPLATE_DIRECTORY + TEMPLATE_SIMPLE;
                    if (doc.Open(templatePath) == false)
                    {
                        MessageBox.Show("Open() Error: " + doc.ErrorCode);
                        return;
                    }
                }
                else
                {
                    if (doc.Open(templatePath) == false)
                    {
                        MessageBox.Show("Open() Error: " + doc.ErrorCode);
                        return;
                    }
                }
                doc.GetObject("ica01").Text = masterModel.ica01;
                doc.GetObject("ica02").Text = masterModel.ica02;
                doc.GetObject("ica11").Text = string.Format("{0:N0}", masterModel.ica11);

                doc.StartPrint("", PrintOptionConstants.bpoDefault);
                doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                doc.EndPrint();
                doc.Close();


                //if (doc.Open(templatePath) != false)
                //{
                //    if (masterModel.ica01.Length == 11 && masterModel.ica01.Substring(10, 1) == "B")
                //    {
                //        doc.GetObject("label1").Text = "二手$";
                //        doc.GetObject("ica01").Width = 32;
                //        doc.GetObject("ica01").Height = 7;
                //    }


                //    doc.GetObject("ica11").Text = string.Format("{0:N0}", masterModel.ica11);
                //    // doc.SetMediaById(doc.Printer.GetMediaId(), true);
                //    doc.StartPrint("", PrintOptionConstants.bpoDefault);
                //    doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                //    doc.EndPrint();
                //    doc.Close();
                //}
                //else
                //{
                //    MessageBox.Show("Open() Error: " + doc.ErrorCode);
                //}
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion

        #region WfPrintLabelShelf 列印貨架標籤
        private void WfPrintLabelShelf(vw_invi100 masterModel)
        {
            try
            {
                const string TEMPLATE_DIRECTORY = @"Label\";	// Template file path
                string TEMPLATE_SIMPLE = "barcode_shelf.LBX";	// Template file name

                string templatePath = TEMPLATE_DIRECTORY;
                templatePath = TEMPLATE_DIRECTORY + TEMPLATE_SIMPLE;
                bpac.DocumentClass doc = new DocumentClass();

                if (doc.Open(templatePath) == false)
                {
                    MessageBox.Show("Open() Error: " + doc.ErrorCode);
                    return;
                }
                doc.GetObject("ica01").Text = masterModel.ica01;
                doc.GetObject("ica02").Text = masterModel.ica02;
                //doc.GetObject("ica11").Text = string.Format("{0:N0}", masterModel.ica11);

                doc.StartPrint("", PrintOptionConstants.bpoDefault);
                doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                doc.EndPrint();
                doc.Close();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion

        #region WfPrintLabelMoney 列印貨架標籤QR
        private void WfPrintLabelShelfQR(vw_invi100 masterModel)
        {
            try
            {
                const string TEMPLATE_DIRECTORY = @"Label\";	// Template file path
                string TEMPLATE_SIMPLE = "barcode_shelfQR.LBX";	// Template file name

                string templatePath = TEMPLATE_DIRECTORY;
                templatePath = TEMPLATE_DIRECTORY + TEMPLATE_SIMPLE;
                bpac.DocumentClass doc = new DocumentClass();

                if (doc.Open(templatePath) == false)
                {
                    MessageBox.Show("Open() Error: " + doc.ErrorCode);
                    return;
                }
                //doc.GetObject("ica01").Text = masterModel.ica01;
                doc.GetObject("ica02").Text = masterModel.ica02;
                doc.GetObject("ica28").Text = masterModel.ica28;
                //doc.GetObject("ica11").Text = string.Format("{0:N0}", masterModel.ica11);

                doc.StartPrint("", PrintOptionConstants.bpoDefault);
                doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                doc.EndPrint();
                doc.Close();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion
        #endregion

        #region WFGenOldMateriel 產生B料號
        private void WFGenOldMateriel(ica_tb icaModel)
        {
            string ica01_new;
            DataTable dtIca = null;
            DataRow drIca = null;
            InvBLL boIca = null;
            string selectSql = "";
            try
            {
                if (icaModel.ica01.Length != 10)
                {
                    WfShowErrorMsg("已是B料號!");
                    return;
                }
                ica01_new = icaModel.ica01 + "B";
                if (BoInv.OfChkIcaPKExists(ica01_new))
                {
                    WfShowErrorMsg("已經有B料號!");
                    return;
                }
                boIca = new InvBLL(BoMaster.OfGetConntion());
                boIca.OfCreateDao("ica_tb", "*", "");
                selectSql = "SELECT * FROM ica_tb WHERE 1<>1 ";
                dtIca = boIca.OfGetDataTable(selectSql);
                drIca = dtIca.NewRow();

                drIca["ica01"] = ica01_new;
                drIca["ica02"] = icaModel.ica02;
                drIca["ica03"] = icaModel.ica03;
                drIca["ica04"] = icaModel.ica04;
                drIca["ica05"] = icaModel.ica05;
                drIca["ica06"] = icaModel.ica06;
                drIca["ica07"] = icaModel.ica07;
                drIca["ica08"] = icaModel.ica08;
                drIca["ica09"] = icaModel.ica09;
                drIca["ica10"] = 0; //未稅訂價
                drIca["ica11"] = 0; //含稅訂價
                drIca["ica12"] = 0; //材料成本
                drIca["ica13"] = 0; //人工成本
                drIca["ica14"] = 0; //製造費用
                drIca["ica15"] = 0; //管銷成本
                drIca["ica16"] = 0; //標準成本合計
                drIca["ica17"] = icaModel.ica17;
                drIca["ica18"] = 0; //最近採購單價
                drIca["ica19"] = 0; //市價
                drIca["ica20"] = DBNull.Value; //市價最近更新日
                drIca["ica21"] = "";    //原廠料號
                drIca["ica22"] = icaModel.ica22;
                drIca["ica23"] = icaModel.ica23;
                drIca["ica24"] = 0; //安全庫存量

                if (icaModel.ica25 != null)
                    drIca["ica25"] = icaModel.ica25;
                else
                    drIca["ica25"] = DBNull.Value;

                drIca["ica26"] = icaModel.ica26;
                drIca["ica27"] = icaModel.ica27;
                drIca["ica28"] = icaModel.ica28;
                drIca["ica29"] = icaModel.ica29;
                drIca["ica30"] = icaModel.ica30;
                drIca["ica31"] = icaModel.ica31;
                drIca["ica32"] = icaModel.ica32;
                drIca["ica33"] = icaModel.ica33;

                drIca["icaconf"] = icaModel.icaconf;
                drIca["icavali"] = icaModel.icavali;
                drIca["icacreu"] = LoginInfo.UserNo;
                drIca["icacreg"] = LoginInfo.DeptNo;
                drIca["icacred"] = Today;
                drIca["icamodu"] = LoginInfo.UserNo;
                drIca["icamodg"] = icaModel.icamodg;
                drIca["icamodd"] = DBNull.Value;
                drIca["icasecu"] = LoginInfo.UserNo;
                drIca["icasecg"] = LoginInfo.DeptNo;

                dtIca.Rows.Add(drIca);

                if (boIca.OfUpdate(dtIca) != 1)
                {
                    WfShowErrorMsg("新增B料號失敗!");
                    return;
                }
                WfShowBottomStatusMsg("新增B料號成功!");
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region WfSetDocPicture 設定單據顯示圖片
        protected void WfSetUsedPicture(string ica01)
        {
            ImageList imgList = null;
            try
            {
                pbxUsed.Visible = false;
                imgList = GlobalPictuer.LoadUsedImage();
                if (imgList == null)
                    return;

                if (imgList == null)
                    return;
                pbxUsed.Image = imgList.Images["old_goods"];
                if (ica01.Length == 11 && ica01.Substring(10, 1).ToUpper() == "B")
                {
                    pbxUsed.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region pbx_icp03_DoubleClick 複製圖片
        private void pbx_icp03_DoubleClick(object sender, EventArgs e)
        {
            if (pbx_icp03.Image != null)
            {
                Image img = pbx_icp03.Image;
                Clipboard.SetImage(img);
            }
        } 
        #endregion


    }
}
