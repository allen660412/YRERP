/* 程式名稱: 員工建立作業
   系統代號: basi030
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
using System.Windows.Forms;
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;

namespace YR.ERP.Forms.Bas
{
    public partial class FrmBasi030 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmBasi030()
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
            this.StrFormID = "basi030";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("bec01", SqlDbType.NVarChar) });
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
                else//新增與修改
                {
                    WfSetControlsReadOnlyRecursion(this, false);//先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(ute_bec03_c, true);

                    WfSetControlReadonly(new List<Control> { ute_beccreu, ute_beccreg, udt_beccred }, true);
                    WfSetControlReadonly(new List<Control> { ute_becmodu, ute_becmodg, udt_becmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_becsecu, ute_becsecg }, true);

                    if (FormEditMode == YREditType.修改)
                        WfSetControlReadonly(ute_bec01, true);
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["becvali"] = "Y";
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfItemCheck
        //回傳值 false未通過驗證,還原輸入的值 true.未通過驗證,保留原值
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            beb_tb bebModel;
            try
            {
                #region 單頭-pick vw_basi030
                if (e.Row.Table.Prefix.ToLower() == "vw_basi030")
                {
                    switch (e.Column.ToLower())
                    {
                        case "bec01"://員工編號
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == true)
                            {
                                WfShowErrorMsg("員工編號已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "bec03"://部門
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["bec03_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("查無此部門,請檢核!");
                                return false;
                            }
                            bebModel = BoBas.OfGetBebModel(GlobalFn.isNullRet(e.Value, ""));
                            e.Row["bec03_c"] = GlobalFn.isNullRet(bebModel.beb03, "");
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
            try
            {
                MessageInfo messageModel = new MessageInfo();
                //this.MsgInfoReturned = new MessageInfo();
                #region 單頭-pick vw_basi030
                if (pDr.Table.Prefix.ToLower() == "vw_basi030")
                {
                    switch (pColName.ToLower())
                    {
                        case "bec03"://部門
                            WfShowPickUtility("p_beb", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                            //}
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
            vw_basi030 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_basi030>();
                #region 單頭資料檢查
                chkColName = "bec01";
                chkControl = ute_bec01;
                #region 員工編號
                if (GlobalFn.varIsNull(masterModel.bec01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                if (FormEditMode == YREditType.新增 && BoBas.OfChkBecPKValid(GlobalFn.isNullRet(masterModel.bec01, "")) == true)
                {
                    msg = "員工編號已存在,請檢核!";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion

                chkColName = "bec02";
                chkControl = ute_bec02;
                #region 姓名
                if (GlobalFn.varIsNull(masterModel.bec02))
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

                chkColName = "bec03";
                chkControl = ute_bec03;
                #region 部門
                if (GlobalFn.varIsNull(masterModel.bec03))
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
                        DrMaster["becsecu"] = LoginInfo.UserNo;
                        DrMaster["becsecg"] = LoginInfo.GroupNo;
                        DrMaster["beccreu"] = LoginInfo.UserNo;
                        DrMaster["beccreg"] = LoginInfo.DeptNo;
                        DrMaster["beccred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["becmodu"] = LoginInfo.UserNo;
                        DrMaster["becmodg"] = LoginInfo.DeptNo;
                        DrMaster["becmodd"] = Now;
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
