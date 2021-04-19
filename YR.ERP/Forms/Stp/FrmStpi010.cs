/* 程式名稱: 銷售單別資料維護作業
   系統代號: stpi010
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

namespace YR.ERP.Forms.Stp
{
    public partial class FrmStpi010 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        BasBLL BoBas = null;
        AdmBLL BoAdm = null;
        CarBLL BoCar = null;
        #endregion

        #region 建構子
        public FrmStpi010()
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
            this.StrFormID = "stpi010";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("bab01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "babsecu";
                TabMaster.GroupColumn = "babsecg";
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
            BoBas = new BasBLL(BoMaster.OfGetConntion());
            BoCar = new CarBLL(BoMaster.OfGetConntion());
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
                    BoAdm.TRAN = BoMaster.TRAN;
                    BoCar.TRAN = BoMaster.TRAN;
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
            vw_stpi010 masterModel;
            try
            {
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else
                {
                    masterModel = DrMaster.ToItem<vw_stpi010>();

                    WfSetControlsReadOnlyRecursion(this, false);
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_babcreu, ute_babcreg, udt_babcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_babmodu, ute_babmodg, udt_babmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_babsecu, ute_babsecg }, true);

                    WfSetControlReadonly(ute_bab04_c, true);

                    if (GlobalFn.varIsNull(masterModel.bab03))
                        WfSetControlReadonly(ute_bab04, true);
                    else
                        WfSetControlReadonly(ute_bab04, false);
                    ;

                    if (!GlobalFn.varIsNull(masterModel.bab04) &&
                        (masterModel.bab04 == "30" || masterModel.bab04 == "40")
                        )
                    {
                        WfSetControlReadonly(ute_bab09, false);
                    }
                    else
                        WfSetControlReadonly(ute_bab09, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_bab01, true);
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
                WfSetUcomboxDataSource(ucb_bab03, sourceList);

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
                pDr["bab05"] = "Y";     //自動取號
                pDr["bab06"] = "N";
                pDr["bab07"] = "N";
                pDr["bab08"] = "N";
                pDr["babvali"] = "Y";
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
            vw_stpi010 masterModel;
            string errMsg;
            try
            {
                baaModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_stpi010>();
                #region 單頭-pick vw_stpi010
                if (e.Row.Table.Prefix.ToLower() == "vw_stpi010")
                {
                    switch (e.Column.ToLower())
                    {
                        case "bab01"://單別
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (GlobalFn.isNullRet(baaModel.baa06.Value, 0) != GlobalFn.isNullRet(e.Value, "").Length)
                            {
                                errMsg = string.Format("單別限定長度為{0}碼", GlobalFn.isNullRet(baaModel.baa06, ""));
                                WfShowErrorMsg(errMsg);
                                return false;
                            }
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, "")) == true)
                            {
                                WfShowErrorMsg("單別已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "bab03"://模組別
                            e.Row["bab04"] = "";
                            e.Row["bab04_c"] = "";
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            WfDisplayMode();
                            ute_bab04.Focus();
                            break;

                        case "bab04"://單據性質
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["bab04_c"] = "";
                                e.Row["bab09"] = "";
                                WfSetControlReadonly(ute_bab09, true);
                                return true;
                            }
                            if (GlobalFn.varIsNull(masterModel.bab03))
                            {
                                WfShowErrorMsg("請先輸入模組別!");
                                return true;
                            }
                            if (BoAdm.OfChkAzf02Exists(masterModel.bab03, GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此模組+單據性質!");
                                return false;
                            }
                            if (e.Value.ToString() == "30" || e.Value.ToString() == "40")
                            {
                                WfSetControlReadonly(ute_bab09, false);
                                WfItemChkForceFocus(ute_bab09);
                            }
                            else
                                WfSetControlReadonly(ute_bab09, true);
                            
                            e.Row["bab09"] = "";
                            e.Row["bab04_c"] = BoAdm.OfGetAzf02(masterModel.bab03, GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "bab09"://轉應收單別
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.varIsNull(masterModel.bab04))
                            {
                                WfShowErrorMsg("請先輸入單據性質!");
                                return false;
                            }
                            if (masterModel.bab04=="30")
                            {
                                if (BoCar.OfChkCacPKValid(e.Value.ToString(),"car","11")==false)
                                {
                                    WfShowErrorMsg("無此單別!");
                                    return false;
                                }
                            }
                            else if (masterModel.bab04 == "40")
                            {
                                if (BoCar.OfChkCacPKValid(e.Value.ToString(), "car", "21") == false)
                                {
                                    WfShowErrorMsg("無此單別!");
                                    return false;
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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            vw_stpi010 masterModel;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpi010>();
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_stpi010
                if (pDr.Table.Prefix.ToLower() == "vw_stpi010")
                {
                    switch (pColName.ToLower())
                    {
                        case "bab04"://單據性質
                            if (GlobalFn.varIsNull(masterModel.bab03))
                            {
                                WfShowErrorMsg("請先輸入模組別!");
                                return false;
                            }
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@azf01", masterModel.bab03));
                            WfShowPickUtility("p_azf1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["azf02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["azf02"], "");
                            //}
                            break;

                        case "bab09"://帳款單別
                            if (GlobalFn.varIsNull(masterModel.bab04))
                            {
                                WfShowErrorMsg("請先輸入單據性質!");
                                return false;
                            }
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@cac03", "car"));
                            if (masterModel.bab04 == "30")
                                messageModel.ParamSearchList.Add(new SqlParameter("@cac04", "11")); //出貨
                            else if (masterModel.bab04=="40")
                                messageModel.ParamSearchList.Add(new SqlParameter("@cac04", "21")); //退貨折讓待抵

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
            vw_stpi010 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpi010>();
                #region 單頭資料檢查
                chkColName = "bab01";
                chkControl = ute_bab01;
                if (GlobalFn.varIsNull(masterModel.bab01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bab02";
                chkControl = ute_bab02;
                if (GlobalFn.varIsNull(masterModel.bab02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bab03";
                chkControl = ucb_bab03;
                if (GlobalFn.varIsNull(masterModel.bab03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bab04";
                chkControl = ute_bab04;
                if (GlobalFn.varIsNull(masterModel.bab04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bab05";
                chkControl = ucx_bab05;
                if (GlobalFn.varIsNull(masterModel.bab05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bab06";
                chkControl = ucx_bab06;
                if (GlobalFn.varIsNull(masterModel.bab06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bab07";
                chkControl = ucx_bab07;
                if (GlobalFn.varIsNull(masterModel.bab07))
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
                        DrMaster["babsecu"] = LoginInfo.UserNo;
                        DrMaster["babsecg"] = LoginInfo.GroupNo;
                        DrMaster["babcreu"] = LoginInfo.UserNo;
                        DrMaster["babcreg"] = LoginInfo.DeptNo;
                        DrMaster["babcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["babmodu"] = LoginInfo.UserNo;
                        DrMaster["babmodg"] = LoginInfo.DeptNo;
                        DrMaster["babmodd"] = Now;
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
