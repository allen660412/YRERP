/* 程式名稱: 幣別資料維護作業
   系統代號: basi080
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
    public partial class FrmBasi080 : YR.ERP.Base.Forms.FrmEntryBase
    {

        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmBasi080()
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
            this.StrFormID = "basi080";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("bek01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "beksecu";
                TabMaster.GroupColumn = "beksecg";
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
                    WfSetControlReadonly(new List<Control> { ute_bekcreu, ute_bekcreg, udt_bekcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_bekmodu, ute_bekmodg, udt_bekmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_beksecu, ute_beksecg }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_bek01, true);
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
                pDr["bek03"] = 0;
                pDr["bek04"] = 0;
                pDr["bek05"] = 0;
                pDr["bekvali"] = "Y";
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
            int iTemp=0;
            try
            {
                #region 單頭-pick vw_basi080
                if (e.Row.Table.Prefix.ToLower() == "vw_basi080")
                {
                    switch (e.Column.ToLower())
                    {
                        #region bek01 幣別資料check
                        case "bek01":
                            if (BoBas.OfChkBekPKExists(GlobalFn.isNullRet(e.Value.ToString(), "")) == true)
                            {
                                WfShowErrorMsg("幣別代碼已存在,請檢核!");
                                return false;
                            }
                            break;
                        #endregion

                        #region bek03 單價小數位數
                        case "bek03":
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (int.TryParse(GlobalFn.isNullRet(e.Value, ""), out iTemp) == false)
                            {
                                WfShowErrorMsg("請輸入整數值");
                                return false;
                            }
                            if (iTemp>5||iTemp<0)
                            {
                                WfShowErrorMsg("需輸入範圍為0-5的整數!");
                                return false;
                            }
                            break;
                        #endregion

                        #region bek03 小計/總計位數
                        case "bek04":
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (int.TryParse(GlobalFn.isNullRet(e.Value, ""), out iTemp) == false)
                            {
                                WfShowErrorMsg("請輸入整數值");
                                return false;
                            }
                            if (iTemp > 5 || iTemp < 0)
                            {
                                WfShowErrorMsg("需輸入範圍為0-5的整數!");
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
            vw_basi080 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            int iTemp = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_basi080>();
                chkColName = "bek01";
                chkControl = ute_bek01;
                #region 幣別
                if (GlobalFn.varIsNull(masterModel.bek01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                if (FormEditMode == YREditType.新增 && BoBas.OfChkBekPKExists(GlobalFn.isNullRet(masterModel.bek01, "")) == true)
                {
                    msg = "幣別已存在,請檢核!";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion

                chkColName = "bek02";
                chkControl = ute_bek02;
                #region 幣別名稱
                if (GlobalFn.varIsNull(masterModel.bek02))
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

                #region 單價小數位數
                chkColName = "bek03";
                chkControl = ute_bek03;
                iTemp = 0;
                if (GlobalFn.varIsNull(masterModel.bek03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                if (int.TryParse(GlobalFn.isNullRet(masterModel.bek03, ""), out iTemp) == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = "請輸入整數值";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(chkControl, msg);
                    return false;
                }
                if (iTemp > 5 || iTemp < 0)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus(); ;
                    msg = "需輸入範圍為0-5的整數";
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion

                #region 小計/總計 小數位數
                chkColName = "bek04";
                chkControl = ute_bek04;
                iTemp = 0;
                if (GlobalFn.varIsNull(masterModel.bek04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                if (int.TryParse(GlobalFn.isNullRet(masterModel.bek04, ""), out iTemp) == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = "請輸入整數值";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(chkControl, msg);
                    return false;
                }
                if (iTemp > 5 || iTemp < 0)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus(); ;
                    msg = "需輸入範圍為0-5的整數";
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
                        DrMaster["beksecu"] = LoginInfo.UserNo;
                        DrMaster["beksecg"] = LoginInfo.GroupNo;
                        DrMaster["bekcreu"] = LoginInfo.UserNo;
                        DrMaster["bekcreg"] = LoginInfo.DeptNo;
                        DrMaster["bekcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["bekmodu"] = LoginInfo.UserNo;
                        DrMaster["bekmodg"] = LoginInfo.DeptNo;
                        DrMaster["bekmodd"] = Now;
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
