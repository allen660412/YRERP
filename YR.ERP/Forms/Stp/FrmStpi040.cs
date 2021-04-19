/* 程式名稱: 運輸地點維護作業
   系統代號: stpi040
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


namespace YR.ERP.Forms.Stp
{
    public partial class FrmStpi040 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        StpBLL BoStp = null;
        #endregion

        #region 建構子
        public FrmStpi040()
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
            this.StrFormID = "stpi040";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("sbg01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "sbgsecu";
                TabMaster.GroupColumn = "sbgsecg";
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

            BoStp = new StpBLL(BoMaster.OfGetConntion());
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
                    BoStp.TRAN = BoMaster.TRAN;
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
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_sbgcreu, ute_sbgcreg, udt_sbgcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_sbgmodu, ute_sbgmodg, udt_sbgmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_sbgsecu, ute_sbgsecg }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_sbg01, true);
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

        #region WfItemCheck
        //回傳值  false未通過驗證,還原輸入的值 true.未通過驗證,保留原值
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            try
            {
                #region 單頭-pick vw_stpi040
                if (e.Row.Table.Prefix.ToLower() == "vw_stpi040")
                {
                    switch (e.Column.ToLower())
                    {
                        #region beg01 運輸地點check
                        case "sbg01":
                            if (BoStp.OfChkSbgPKExists(GlobalFn.isNullRet(e.Value, "")) == true)
                            {
                                WfShowErrorMsg("運輸地點編號已存在,請檢核!");
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
            vw_stpi040 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpi040>();
                #region 單頭資料檢查
                chkColName = "sbg01";//運輸地點
                chkControl = ute_sbg01;
                if (GlobalFn.varIsNull(masterModel.sbg01))
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
                        DrMaster["sbgsecu"] = LoginInfo.UserNo;
                        DrMaster["sbgsecg"] = LoginInfo.GroupNo;
                        DrMaster["sbgcreu"] = LoginInfo.UserNo;
                        DrMaster["sbgcreg"] = LoginInfo.DeptNo;
                        DrMaster["sbgcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["sbgmodu"] = LoginInfo.UserNo;
                        DrMaster["sbgmodg"] = LoginInfo.DeptNo;
                        DrMaster["sbgmodd"] = Now;
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
