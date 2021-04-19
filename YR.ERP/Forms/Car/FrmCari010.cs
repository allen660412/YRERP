/* 程式名稱: 應收帳款單別設定作業
   系統代號: cari010
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


namespace YR.ERP.Forms.Car
{
    public partial class FrmCari010 : YR.ERP.Base.Forms.FrmEntryBase
    {

        #region Property
        BasBLL BoBas = null;
        CarBLL BoCar = null;
        AdmBLL BoAdm = null;
        GlaBLL BoGla = null;
        #endregion

        #region 建構子
        public FrmCari010()
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
            this.StrFormID = "cari010";
            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;            
            uTab_Master.Tabs[0].Text = "資料內容";
            uTab_Master.Tabs[1].Text = "狀態";
            uTab_Master.Tabs[2].Text = "資料瀏覽";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("cac01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "cacsecu";
                TabMaster.GroupColumn = "cacsecg";
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

            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
            BoCar = new CarBLL(BoMaster.OfGetConntion());
            BoBas = new BasBLL(BoMaster.OfGetConntion());
            BoGla = new GlaBLL(BoMaster.OfGetConntion());
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
                    BoCar.TRAN = BoMaster.TRAN;
                    BoAdm.TRAN = BoMaster.TRAN;
                    BoGla.TRAN = BoMaster.TRAN;
                }
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
            vw_cari010 masterModel;
            try
            {
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else
                {
                    masterModel = DrMaster.ToItem<vw_cari010>();
                    
                    WfSetControlsReadOnlyRecursion(this, false);
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_caccreu, ute_caccreg, udt_caccred }, true);
                    WfSetControlReadonly(new List<Control> { ute_cacmodu, ute_cacmodg, udt_cacmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_cacsecu, ute_cacsecg }, true);
                    
                    WfSetControlReadonly(ucb_cac03, true);
                    WfSetControlReadonly(ute_cac04_c, true);

                    if (GlobalFn.varIsNull(masterModel.cac03))
                        WfSetControlReadonly(ute_cac04, true);
                    else
                        WfSetControlReadonly(ute_cac04, false);

                    if (GlobalFn.isNullRet(masterModel.cac08,"")=="Y")
                    {
                        WfSetControlReadonly(ucx_cac09, false);
                        WfSetControlReadonly(ute_cac10, false);
                    }
                    else
                    {
                        WfSetControlReadonly(ucx_cac09, true);
                        WfSetControlReadonly(ute_cac10, true);
                    }

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_cac01, true);
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
                uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                SelectNextControl(this.uTab_Master, true, true, true, false);
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
                //模組代號
                sourceList = BoAdm.OfGetAze01KVPList();
                WfSetUcomboxDataSource(ucb_cac03, sourceList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreInInsertModeCheck 進新增模式前的檢查
        protected override bool WfPreInInsertModeCheck()
        {
            int iChkCnts = 0;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM baa_tb");
                iChkCnts = GlobalFn.isNullRet(BoMaster.OfGetFieldValue(sbSql.ToString()), 0);
                if (iChkCnts == 0)
                {
                    WfShowErrorMsg("請先設定共用參數作業(basi001)!");
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["cac03"] = "car";   //模組別預設 Car
                pDr["cac05"] = "Y";     //自動取號
                pDr["cac06"] = "N";     //自動確認
                pDr["cac07"] = "N";     //自動列印
                pDr["cac08"] = "N";     //拋轉傳票
                pDr["cac09"] = "N";     //拋轉直接拋轉總帳
                pDr["cacvali"] = "Y";
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
            baa_tb baaModel;
            vw_cari010 masterModel;            
            string errMsg;
            try
            {
                baaModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_cari010>();
                #region 單頭-pick vw_cari010
                if (e.Row.Table.Prefix.ToLower() == "vw_cari010")
                {
                    switch (e.Column.ToLower())
                    {
                        case "cac01"://單別
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (GlobalFn.isNullRet(baaModel.baa06.Value, 0) != GlobalFn.isNullRet(e.Value, "").Length)
                            {
                                errMsg = string.Format("單別限定長度為{0}碼", GlobalFn.isNullRet(baaModel.baa06, ""));
                                WfShowErrorMsg(errMsg);
                                return false;
                            }
                            if (BoCar.OfChkCacPKValid(GlobalFn.isNullRet(e.Value, "")) == true)
                            {
                                WfShowErrorMsg("單別已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "cac03"://模組別
                            e.Row["cac04"] = "";
                            e.Row["cac04_c"] = "";
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            WfDisplayMode();
                            ute_cac04.Focus();
                            break;
                            
                        case "cac04"://單據性質
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["cac04_c"] = "";
                                return true;
                            }
                            if (GlobalFn.varIsNull(masterModel.cac03))
                            {
                                WfShowErrorMsg("請先輸入模組別!");
                                return true;
                            }
                            if (BoAdm.OfChkAzf02Exists(masterModel.cac03, GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此模組+單據性質!");
                                return false;
                            }
                            e.Row["cac04_c"] = BoAdm.OfGetAzf02(masterModel.cac03, GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "cac08":
                            if (e.Value.ToString()=="Y")
                            {
                                WfSetControlReadonly(ucx_cac09, false);
                                WfSetControlReadonly(ute_cac10, false);
                            }
                            else
                            {
                                WfSetControlReadonly(ucx_cac09, true);
                                WfSetControlReadonly(ute_cac10, true);
                                e.Row["cac09"] = "N";
                                e.Row["cac10"] = "";
                            }
                            break;
                        case "cac10":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoGla.OfChkGacPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
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
        
        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            vw_cari010 masterModel;
            try
            {
                masterModel = DrMaster.ToItem<vw_cari010>();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_cari010
                if (pDr.Table.Prefix.ToLower() == "vw_cari010")
                {
                    switch (pColName.ToLower())
                    {
                        case "cac04"://單據性質
                            if (GlobalFn.varIsNull(masterModel.cac03))
                            {
                                WfShowErrorMsg("請先輸入模組別!");
                                return false;
                            }
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@azf01", masterModel.cac03));
                            WfShowPickUtility("p_azf1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["azf02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "cac10":       //傳票單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            WfShowPickUtility("p_gac1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["gac01"], "");
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_cari010 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_cari010>();
                #region 單頭資料檢查
                chkColName = "cac01";
                chkControl = ute_cac01;
                if (GlobalFn.varIsNull(masterModel.cac01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cac02";
                chkControl = ute_cac02;
                if (GlobalFn.varIsNull(masterModel.cac02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cac03";
                chkControl = ucb_cac03;
                if (GlobalFn.varIsNull(masterModel.cac03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cac04";
                chkControl = ute_cac04;
                if (GlobalFn.varIsNull(masterModel.cac04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cac05";
                chkControl = ucx_cac05;
                if (GlobalFn.varIsNull(masterModel.cac05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                
                chkColName = "cac06";
                chkControl = ucx_cac06;
                if (GlobalFn.varIsNull(masterModel.cac06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "cac07";
                chkControl = ucx_cac07;
                if (GlobalFn.varIsNull(masterModel.cac07))
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
            try
            {
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["cacsecu"] = LoginInfo.UserNo;
                        DrMaster["cacsecg"] = LoginInfo.GroupNo;
                        DrMaster["caccreu"] = LoginInfo.UserNo;
                        DrMaster["caccreg"] = LoginInfo.DeptNo;
                        DrMaster["caccred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["cacmodu"] = LoginInfo.UserNo;
                        DrMaster["cacmodg"] = LoginInfo.DeptNo;
                        DrMaster["cacmodd"] = Now;
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
