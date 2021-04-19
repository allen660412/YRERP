/* 程式名稱: 共用參數設定作業
   系統代號: basi001
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
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;


namespace YR.ERP.Forms.Bas
{
    public partial class FrmBasi001 : YR.ERP.Base.Forms.FrmEntryBase
    {

        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmBasi001()
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
            this.StrFormID = "basi001";
            this.IntTabCount = 2;
            this.IntMasterGridPos = 0;
            uTab_Master.Tabs[0].Text = "資料內容";
            uTab_Master.Tabs[1].Text = "狀態";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("baacomp", SqlDbType.NVarChar) });

                TabMaster.CanCopyMode = false;
                TabMaster.CanDeleteMode = false;
                TabMaster.IsAutoQueryFistLoad = true;
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
            try
            {
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false);
                    WfSetControlReadonly(new List<Control> { ute_baacreu, ute_baacreg, udt_baacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_baamodu, ute_baamodg, udt_baamodd }, true);
                    //WfSetControlReadonly(new List<Control> { ute_baasecu, ute_baasecg }, true);

                    //WfSetControlReadonly(ute_baacomp, true);
                    WfSetControlReadonly(ute_result, true);
                }
                wfAutoCodeResult();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfQueryDisplayMode 查詢後的狀態處理 readonly
        protected override bool WfQueryDisplayMode()
        {
            try
            {
                WfSetControlsReadOnlyRecursion(this, false);
                WfSetControlReadonly(ute_result,true);
                ute_result.Value = "";
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
                //日期格式
                sourceList = BoBas.OfGetBaa01KVPList();
                WfSetUcomboxDataSource(ucb_baa01, sourceList);

                //單別設定位數
                sourceList = BoBas.OfGetBaa06KVPList();
                WfSetUcomboxDataSource(ucb_baa06, sourceList);

                //自動編號方式
                sourceList = BoBas.OfGetBaa07KVPList();
                WfSetUcomboxDataSource(ucb_baa07, sourceList);

                //單號設定位數
                sourceList = BoBas.OfGetBaa08KVPList();
                WfSetUcomboxDataSource(ucb_baa08, sourceList);
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
                if (iChkCnts > 0)
                {
                    WfShowErrorMsg("本作業僅可新增一筆資料!");
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
                pDr["baacomp"] = LoginInfo.CompNo;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfItemCheck
        //回傳值  false未通過驗證,還原輸入的值 true.未通過驗證,保留原值
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            vw_basi001 basi001Model;
            DateTime parseDatetime;
            try
            {
                basi001Model=DrMaster.ToItem<vw_basi001>();
                #region 單頭-pick vw_basi080
                if (e.Row.Table.Prefix.ToLower() == "vw_basi001")
                {
                    switch (e.Column.ToLower())
                    {
                        #region baa02 庫存現行年月
                        case "baa02":
                            if (GlobalFn.varIsNull(e.Value))
                                return true;

                            if (DateTime.TryParseExact(e.Value.ToString(),"yyyyMM",YR.ERP.Shared.GlobalVar.FormatProvider,
                                System.Globalization.DateTimeStyles.None,out parseDatetime)==false)
                            {
                                WfShowErrorMsg("輸入年月格式字串錯誤,請檢核!");
                                return false;
                            }

                            if (!GlobalFn.varIsNull(basi001Model.baa02) && !GlobalFn.varIsNull(basi001Model.baa03))
                            {
                                if (Convert.ToInt32(basi001Model.baa03) > Convert.ToInt32(basi001Model.baa02))
                                {
                                    WfShowErrorMsg("關帳年月不可大於現行年月,請檢核!");
                                    return false;
                                }
                            }
                            break;
                        #endregion

                        #region baa03 庫存關帳年月
                        case "baa03":
                            if (GlobalFn.varIsNull(e.Value))
                                return true;

                            if (DateTime.TryParseExact(e.Value.ToString(), "yyyyMM", YR.ERP.Shared.GlobalVar.FormatProvider,
                                System.Globalization.DateTimeStyles.None, out parseDatetime) == false)
                            {
                                WfShowErrorMsg("輸入年月格式字串錯誤,請檢核!");
                                return false;
                            }

                            if (!GlobalFn.varIsNull(basi001Model.baa02) && !GlobalFn.varIsNull(basi001Model.baa03))
                            {
                                if (Convert.ToInt32(basi001Model.baa03) > Convert.ToInt32(basi001Model.baa02))
                                {
                                    WfShowErrorMsg("關帳年月不可大於現行年月,請檢核!");
                                    return false;
                                }
                            }
                            break;
                        #endregion

                        #region baa04 幣別
                        case "baa04":
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoBas.OfChkBekPKValid(e.Value.ToString())==false)
                            {
                                WfShowErrorMsg("無此幣別資料,請檢核!");
                                return false;
                            }
                            break;
                        #endregion

                        #region baa06 單別設定位數
                        case "baa06":
                            wfAutoCodeResult();
                            break;
                        #endregion

                        #region baa07 自動編號方式
                        case "baa07":
                            wfAutoCodeResult();
                            break;
                        #endregion

                        #region baa08 單號設定位數
                        case "baa08":
                            wfAutoCodeResult();
                            break;
                        #endregion
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
            vw_basi001 masterModel;
            try
            {
                masterModel = DrMaster.ToItem<vw_basi001>();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_basi001
                if (pDr.Table.Prefix.ToLower() == "vw_basi001")
                {
                    switch (pColName.ToLower())
                    {
                        case "baa04"://單據性質
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
            vw_basi001 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_basi001>();
                #region 單頭資料檢查
                chkColName = "baacomp";//公司別
                chkControl = ute_baacomp;
                if (GlobalFn.varIsNull(masterModel.baacomp))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();                    
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "baa01";//日期格式
                chkControl = ucb_baa01;
                if (GlobalFn.varIsNull(masterModel.baa01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "baa02";//庫存現行年月
                chkControl = ute_baa02;
                if (GlobalFn.varIsNull(masterModel.baa02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "baa03";//庫存關帳年月
                chkControl = ute_baa03;
                if (GlobalFn.varIsNull(masterModel.baa03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "baa04";//本國幣別
                chkControl = ute_baa04;
                if (GlobalFn.varIsNull(masterModel.baa04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "baa05";//營業稅率
                chkControl = ute_baa05;
                if (GlobalFn.varIsNull(masterModel.baa05))
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
                        //DrMaster["baasecu"] = LoginInfo.UserNo;
                        //DrMaster["baasecg"] = LoginInfo.GroupNo;
                        DrMaster["baacreu"] = LoginInfo.UserNo;
                        DrMaster["baacreg"] = LoginInfo.DeptNo;
                        DrMaster["baacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["baamodu"] = LoginInfo.UserNo;
                        DrMaster["baamodg"] = LoginInfo.DeptNo;
                        DrMaster["baamodd"] = Now;
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

        //*****************************表單自訂Fuction****************************************
        #region wfAutoCodeResult 顯示編碼結果
        public void wfAutoCodeResult()
        {
            vw_basi001 masterModel;
            string autoCodeResult = "";
            string tempString = "";

            if (DrMaster == null)
                ute_result.Text = "";
            masterModel = DrMaster.ToItem<vw_basi001>();
            if (GlobalFn.varIsNull(masterModel.baa06) ||
                GlobalFn.varIsNull(masterModel.baa07) ||
                GlobalFn.varIsNull(masterModel.baa08)
                )
                ute_result.Text = "";
            //先處理單別位數
            tempString = "".PadLeft(GlobalFn.isNullRet(masterModel.baa06, 0), 'X');
            autoCodeResult += tempString + "-";
            //再處理自動編號方式
            tempString = "";
            switch (masterModel.baa07)
            {
                case "1":
                    tempString = "";
                    break;
                case "2":
                    tempString = "yyMM";
                    break;
                case "3":
                    tempString = "yyww";
                    break;
                case "4":
                    tempString = "yyMMdd";
                    break;
            }
            autoCodeResult += tempString;
            //最後處理流水編
            var flowLength = GlobalFn.isNullRet(masterModel.baa08 - tempString.Length, 0);
            tempString = "".PadLeft(flowLength, '9');
            autoCodeResult += tempString;

            ute_result.Text = autoCodeResult;
        } 
        #endregion
    }
}
