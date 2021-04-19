/* 程式名稱: 公司資料建立作業
   系統代號: basi010
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
using YR.Util;
using YR.ERP.DAL.YRModel;


namespace YR.ERP.Forms.Bas
{
    public partial class FrmBasi010 : YR.ERP.Base.Forms.FrmEntryBase
    {

        #region 建構子
        public FrmBasi010()
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
            this.StrFormID = "basi010";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("beacomp", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "beasecu";
                TabMaster.GroupColumn = "beasecg";

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
            return;
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
                    //WfSetControlReadonly(ute_beacomp, true);
                    WfSetControlReadonly(new List<Control> { ute_beacreu, ute_beacreg, udt_beacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_beamodu, ute_beamodg, udt_beamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_beasecu, ute_beasecg }, true);
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(new List<Control> { ute_beacomp}, true);
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

        #region WfPreInInsertModeCheck 進新增模式前的檢查
        protected override bool WfPreInInsertModeCheck()
        {
            int iChkCnts = 0;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bea_tb");
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
                pDr["beacomp"] = LoginInfo.CompNo;
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
            vw_basi010 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_basi010>();
                #region 單頭資料檢查
                chkColName = "beacomp";//公司別
                if (GlobalFn.varIsNull(masterModel.beacomp))
                {
                    chkControl = ute_beacomp;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bea01";//公司全名
                if (GlobalFn.varIsNull(masterModel.bea01))
                {
                    chkControl = ute_bea01;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bea02";//公司簡稱
                if (GlobalFn.varIsNull(masterModel.bea02))
                {
                    chkControl = ute_bea02;
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
                        DrMaster["beasecu"] = LoginInfo.UserNo;
                        DrMaster["beasecg"] = LoginInfo.GroupNo;
                        DrMaster["beacreu"] = LoginInfo.UserNo;
                        DrMaster["beacreg"] = LoginInfo.DeptNo;
                        DrMaster["beacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["beamodu"] = LoginInfo.UserNo;
                        DrMaster["beamodg"] = LoginInfo.DeptNo;
                        DrMaster["beamodd"] = Now;
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
