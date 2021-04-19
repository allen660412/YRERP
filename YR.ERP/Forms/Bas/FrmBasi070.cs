/* 程式名稱: 單位基本資料維護作業
   系統代號: basi070
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



namespace YR.ERP.Forms.Bas
{
    public partial class FrmBasi070 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmBasi070()
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
            this.StrFormID = "basi070";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("bej01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "bejsecu";
                TabMaster.GroupColumn = "bejsecg";
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
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_bejcreu, ute_bejcreg, udt_bejcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_bejmodu, ute_bejmodg, udt_bejmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_bejsecu, ute_bejsecg }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_bej01, true);
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
                pDr["bej03"] = 0;
                pDr["bejvali"] = 'Y';
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
            try
            {
                #region 單頭- vw_basi070
                if (e.Row.Table.Prefix.ToLower() == "vw_basi070")
                {
                    switch (e.Column.ToLower())
                    {
                        case "bej01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                return true;
                            }

                            if (BoBas.OfChkBejPkExists(GlobalFn.isNullRet(e.Value.ToString(), "")) == true)
                            {
                                WfShowErrorMsg("單位已存在,請檢核!");
                                return false;
                            }
                            break;


                        case "bej03"://小數位數
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (!GlobalFn.isNumeric(e.Value.ToString()))
                            {
                                WfShowErrorMsg("請輸入數字!");
                                return false;
                            }

                            if (GlobalFn.isNullRet(e.Value, 0.0M) < 0 || GlobalFn.isNullRet(e.Value, 0.0M) >5)
                            {
                                WfShowErrorMsg("請輸入區間數字0-5!");
                                return false;
                            }
                            DrMaster["bej03"] = Math.Round(Convert.ToDecimal(e.Value) ,0,MidpointRounding.AwayFromZero );//自動四捨伍入
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
            vw_basi070 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_basi070>();
                #region 單頭資料檢查
                chkColName = "bej01";       //單位
                chkControl = ute_bej01;
                if (GlobalFn.varIsNull(masterModel.bej01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bej02";       //單位名稱
                chkControl = ute_bej02;
                if (GlobalFn.varIsNull(masterModel.bej02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bej03";
                chkControl = ute_bej03;
                if (GlobalFn.varIsNull(masterModel.bej03))
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
                        DrMaster["bejsecu"] = LoginInfo.UserNo;
                        DrMaster["bejsecg"] = LoginInfo.GroupNo;
                        DrMaster["bejcreu"] = LoginInfo.UserNo;
                        DrMaster["bejcreg"] = LoginInfo.DeptNo;
                        DrMaster["bejcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["bejmodu"] = LoginInfo.UserNo;
                        DrMaster["bejmodg"] = LoginInfo.DeptNo;
                        DrMaster["bejmodd"] = Now;
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
