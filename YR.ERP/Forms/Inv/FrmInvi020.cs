/* 程式名稱: 單位基本換算功能設定
   系統代號: invi020
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


namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvi020 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        #endregion

        #region 建構子
        public FrmInvi020()
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
            this.StrFormID = "invi020";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("icm01", SqlDbType.NVarChar),
                                                                                new SqlParameter("icm02", SqlDbType.NVarChar)
                                });
                TabMaster.UserColumn = "icmsecu";
                TabMaster.GroupColumn = "icmsecg";
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
            BoInv = new InvBLL(BoMaster.OfGetConntion());
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
                    BoInv.TRAN = BoMaster.TRAN;
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
                    WfSetControlReadonly(new List<Control> { ute_icmcreu, ute_icmcreg, udt_icmcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_icmmodu, ute_icmmodg, udt_icmmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_icmsecu, ute_icmsecg }, true);

                    WfSetControlReadonly(ute_icm01_c, true);
                    WfSetControlReadonly(ute_icm02_c, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_icm01, true);
                        WfSetControlReadonly(ute_icm02, true);
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
                pDr["icm03"] = 0;
                pDr["icm04"] = 0;
                pDr["icmvali"] = "Y";
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
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_invi020
                if (pDr.Table.Prefix.ToLower() == "vw_invi020")
                {
                    switch (pColName.ToLower())
                    {
                        case "icm01"://來源單位
                            WfShowPickUtility("p_bej", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                            //}
                            break;
                        case "icm02"://目的單位
                            WfShowPickUtility("p_bej", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            vw_invi020 masterModel = null;
            try
            {
                #region 單頭-pick vw_invi020
                if (e.Row.Table.Prefix.ToLower() == "vw_invi020")
                {
                    masterModel = DrMaster.ToItem<vw_invi020>();
                    switch (e.Column.ToLower())
                    {
                        case "icm01"://來源單位
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此單位代碼存在,請檢核!");
                                return false;
                            }
                            if (!GlobalFn.varIsNull(masterModel.icm02) && !GlobalFn.varIsNull(e.Value)
                                && e.Value.ToString() == masterModel.icm02
                                )
                            {
                                WfShowErrorMsg("來源單位與目的單位不可相同,請檢核!");
                                return false;
                            }
                            e.Row["icm01_c"] = BoBas.OfGetBej02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "icm02"://目的單位
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此單位代碼存在,請檢核!");
                                return false;
                            }
                            if (!GlobalFn.varIsNull(masterModel.icm01) && !GlobalFn.varIsNull(e.Value)
                                && e.Value.ToString() == masterModel.icm01
                                )
                            {
                                WfShowErrorMsg("來源單位與目的單位不可相同,請檢核!");
                                return false;
                            }
                            e.Row["icm02_c"] = BoBas.OfGetBej02(GlobalFn.isNullRet(e.Value, ""));
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

        #region WfAppendUpdate 存檔後處理額外的資料表 新增、修改、刪除...
        protected override bool WfAppendUpdate()
        {
            icm_tb icmOrgModel;
            DataTable dtIcm;
            DataRow drIcm;
            CommonBLL boAppend;
            StringBuilder sbSql;
            try
            {
                icmOrgModel = DrMaster.ToItem<icm_tb>();

                boAppend = new CommonBLL(BoMaster.OfGetConntion());
                boAppend.TRAN = BoMaster.TRAN;
                boAppend.OfCreateDao("icm_tb", "*", "");

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icm_tb");
                sbSql.AppendLine(string.Format("WHERE icm01='{0}'", icmOrgModel.icm02));
                sbSql.AppendLine(string.Format("AND icm02='{0}'", icmOrgModel.icm01));

                dtIcm = boAppend.OfGetDataTable(sbSql.ToString());

                if (dtIcm.Rows.Count == 0)//新增
                {
                    drIcm = dtIcm.NewRow();
                    drIcm["icm01"] = icmOrgModel.icm02;
                    drIcm["icm02"] = icmOrgModel.icm01;
                    drIcm["icm03"] = icmOrgModel.icm04;
                    drIcm["icm04"] = icmOrgModel.icm03;
                    drIcm["icm05"] = "";
                    drIcm["icmvali"] = icmOrgModel.icmvali;
                    drIcm["icmsecu"] = LoginInfo.UserNo;
                    drIcm["icmsecg"] = LoginInfo.GroupNo;
                    drIcm["icmcreu"] = LoginInfo.UserNo;
                    drIcm["icmcreg"] = LoginInfo.DeptNo;
                    drIcm["icmcred"] = Now;
                    dtIcm.Rows.Add(drIcm);
                }
                else
                {
                    drIcm = dtIcm.Rows[0];
                    drIcm["icm03"] = icmOrgModel.icm04;
                    drIcm["icm04"] = icmOrgModel.icm03;
                    drIcm["icmvali"] = icmOrgModel.icmvali;

                    drIcm["icmmodu"] = LoginInfo.UserNo;
                    drIcm["icmmodg"] = LoginInfo.DeptNo;
                    drIcm["icmmodd"] = Now;
                }
                boAppend.OfUpdate(dtIcm);
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
            vw_invi020 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_invi020>();
                #region 單頭資料檢查
                chkColName = "icm01";
                chkControl = ute_icm01;
                if (GlobalFn.varIsNull(masterModel.icm01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "icm02";
                chkControl = ute_icm02;
                if (GlobalFn.varIsNull(masterModel.icm02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                //檢查PK是否存在
                if (FormEditMode == YREditType.新增)
                {
                    if (!GlobalFn.varIsNull(masterModel.icm01) && !GlobalFn.varIsNull(masterModel.icm02))
                    {
                        if (BoInv.OfChkIcmPKExists(masterModel.icm01,masterModel.icm02)==true)
                        {
                            this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                            ute_icm01.Focus();
                            msg = "資料已存在,不可重覆!";
                            errorProvider.SetError(ute_icm01, msg);
                            errorProvider.SetError(ute_icm02, msg);
                            WfShowErrorMsg(msg);
                            return false;
                        }
                    }
                }

                chkColName = "icm03";
                chkControl = ute_icm03;
                if (GlobalFn.varIsNull(masterModel.icm03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "icm04";
                chkControl = ute_icm04;
                if (GlobalFn.varIsNull(masterModel.icm04))
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
                        DrMaster["icmsecu"] = LoginInfo.UserNo;
                        DrMaster["icmsecg"] = LoginInfo.GroupNo;
                        DrMaster["icmcreu"] = LoginInfo.UserNo;
                        DrMaster["icmcreg"] = LoginInfo.DeptNo;
                        DrMaster["icmcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["icmmodu"] = LoginInfo.UserNo;
                        DrMaster["icmmodg"] = LoginInfo.DeptNo;
                        DrMaster["icmmodd"] = Now;
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
