/* 程式名稱: 收付款條件資料維護作業
   系統代號: basi050
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
    public partial class FrmBasi050 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmBasi050()
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
            this.StrFormID = "basi050";
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
            List<SqlParameter> sqlParms;
            try
            {
                sqlParms = new List<SqlParameter>();
                sqlParms.Add(new SqlParameter("bef01", SqlDbType.NVarChar));
                sqlParms.Add(new SqlParameter("bef02", SqlDbType.NVarChar));
                TabMaster.PKParms = sqlParms;

                TabMaster.UserColumn = "befsecu";
                TabMaster.GroupColumn = "befsecg";
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

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                //付款方式分類 
                sourceList = BoBas.OfGetBef01KVPList(); 
                WfSetUcomboxDataSource(ute_bef01, sourceList);

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
                    WfSetControlReadonly(new List<Control> { ute_befcreu, ute_befcreg, udt_befcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_befmodu, ute_befmodg, udt_befmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_befsecu, ute_befsecg }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_bef01, true);
                        WfSetControlReadonly(ute_bef02, true);
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["befvali"] = "Y";
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
            try
            {
                #region 單頭-pick vw_basi020
                if (e.Row.Table.Prefix.ToLower() == "vw_basi050")
                {
                    switch (e.Column.ToLower())
                    {
                        #region bef02 付款條件編號check
                        case "bef02":
                            if (BoBas.OfChkBefPKValid(GlobalFn.isNullRet(e.Row["bef01"].ToString(), ""), GlobalFn.isNullRet(e.Value, "")) == true)
                            {
                                WfShowErrorMsg("付款條件編號已存在,請檢核!");
                                return false;
                            }
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_basi050 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_basi050>();
                #region 單頭資料檢查
                chkColName = "bef01";
                chkControl = ute_bef01;
                #region 付款條件分類
                if (GlobalFn.varIsNull(masterModel.bef01))
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

                chkColName = "bef02";
                chkControl = ute_bef02;
                #region 付款條件編號
                if (GlobalFn.varIsNull(masterModel.bef02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                if (FormEditMode == YREditType.新增 && BoBas.OfChkBefPKValid(GlobalFn.isNullRet(masterModel.bef01, ""), GlobalFn.isNullRet(masterModel.bef02, "")) == true)
                {
                    msg = "付款條件編號已存在,請檢核!";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion

                chkColName = "bef03";
                chkControl = ute_bef03;
                #region 付款條件編號說明
                if (GlobalFn.varIsNull(masterModel.bef03))
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
                        DrMaster["befsecu"] = LoginInfo.UserNo;
                        DrMaster["befsecg"] = LoginInfo.GroupNo;
                        DrMaster["befcreu"] = LoginInfo.UserNo;
                        DrMaster["befcreg"] = LoginInfo.DeptNo;
                        DrMaster["befcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["befmodu"] = LoginInfo.UserNo;
                        DrMaster["befmodg"] = LoginInfo.DeptNo;
                        DrMaster["befmodd"] = Now;
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
