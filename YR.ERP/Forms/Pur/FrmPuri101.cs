/* 程式名稱: 送貨/帳單地址資料維護
   系統代號: puri101
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


namespace YR.ERP.Forms.Pur
{
    public partial class FrmPuri101 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region property
        PurBLL BoPur = null;
        //string Bga01 = "", Bgc03 = "", Pca01_new = "";          //新增時在存檔前用來重新取號使用
        #endregion

        #region 建構子
        public FrmPuri101()
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
            this.StrFormID = "puri101";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "基本資料";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("pcc01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "pccsecu";
                TabMaster.GroupColumn = "pccsecg";
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
                //票據寄領方式
                sourceList = BoPur.OfGetPcc02KVPList();
                WfSetUcomboxDataSource(ucb_pcc02, sourceList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            try
            {

                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false);        //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯

                    WfSetControlReadonly(new List<Control> { ute_pcccreu, ute_pcccreg, udt_pcccred }, true);
                    WfSetControlReadonly(new List<Control> { ute_pccmodu, ute_pccmodg, udt_pccmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_pccsecu, ute_pccsecg }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_pcc01, true);
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
                pDr["pccvali"] = "Y";
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
            pcc_tb pccModel;
            int ChkCnts = 0;
            try
            {
                pccModel = DrMaster.ToItem<pcc_tb>();
                #region 單頭-pick vw_puri101
                if (e.Row.Table.Prefix.ToLower() == "vw_puri101")
                {
                    switch (e.Column.ToLower())
                    {
                        case "pcc01":
                            if (GlobalFn.varIsNull(e.Value) == true)
                                return true;

                            if (FormEditMode==YREditType.新增)
                            {
                                ChkCnts = GlobalFn.isNullRet(BoPur.OfChkPccPkExists(GlobalFn.isNullRet(e.Value, "")), 0);
                                if (ChkCnts>0)
                                {
                                    WfShowErrorMsg("地址代號已存在!");
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            pcc_tb pccModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            int ChkCnts = 0;
            try
            {

                pccModel = DrMaster.ToItem<pcc_tb>();
                #region 單頭資料檢查
                chkColName = "pcc01";
                chkControl = ute_pcc01;
                if (GlobalFn.varIsNull(pccModel.pcc01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                if (FormEditMode == YREditType.新增)
                {
                    ChkCnts = GlobalFn.isNullRet(BoPur.OfChkPccPkExists(GlobalFn.isNullRet(pccModel.pcc01, "")), 0);
                    if (ChkCnts > 0)
                    {
                        this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                        chkControl.Focus();
                        msg = "地址代號已存在!";
                        WfShowErrorMsg(msg);
                        errorProvider.SetError(chkControl, msg);
                        return false;
                    }
                }

                chkColName = "pcc02";
                chkControl = ucb_pcc02;
                if (GlobalFn.varIsNull(pccModel.pcc02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pcc03";
                chkControl = ute_pcc03;
                if (GlobalFn.varIsNull(pccModel.pcc03))
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
                        DrMaster["pccsecu"] = LoginInfo.UserNo;
                        DrMaster["pccsecg"] = LoginInfo.GroupNo;
                        DrMaster["pcccreu"] = LoginInfo.UserNo;
                        DrMaster["pcccreg"] = LoginInfo.DeptNo;
                        DrMaster["pcccred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["pccmodu"] = LoginInfo.UserNo;
                        DrMaster["pccmodg"] = LoginInfo.DeptNo;
                        DrMaster["pccmodd"] = Now;
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

    }
}
