/* 程式名稱: 傳票單別設定作業
   系統代號: glai010
   作　　者: Allen
   描　　述: 

   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.DAL.YRModel;
using YR.ERP.Shared;
using YR.Util;

namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlai010 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        BasBLL BoBas = null;
        AdmBLL BoAdm = null;
        GlaBLL BoGla = null;
        #endregion

        #region 建構子
        public FrmGlai010()
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
            this.StrFormID = "glai010";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("gac01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "gacsecu";
                TabMaster.GroupColumn = "gacsecg";
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
            BoGla = new GlaBLL(BoMaster.OfGetConntion());
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
                    BoGla.TRAN = BoMaster.TRAN;
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
            vw_glai010 masterModel;
            try
            {
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else
                {
                    masterModel = DrMaster.ToItem<vw_glai010>();

                    WfSetControlsReadOnlyRecursion(this, false);
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_gaccreu, ute_gaccreg, udt_gaccred }, true);
                    WfSetControlReadonly(new List<Control> { ute_gacmodu, ute_gacmodg, udt_gacmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_gacsecu, ute_gacsecg }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_gac01, true);
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
                pDr["gacvali"] = "Y";
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
            vw_glai010 masterModel;
            string errMsg;
            try
            {
                baaModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_glai010>();
                #region 單頭-pick vw_invi010
                if (e.Row.Table.Prefix.ToLower() == "vw_glai010")
                {
                    switch (e.Column.ToLower())
                    {
                        case "gac01"://單別
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
            vw_glai010 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_glai010>();
                #region 單頭資料檢查
                chkColName = "gac01";
                chkControl = ute_gac01;
                if (GlobalFn.varIsNull(masterModel.gac01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "gac02";
                chkControl = ute_gac02;
                if (GlobalFn.varIsNull(masterModel.gac02))
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
                        DrMaster["gacsecu"] = LoginInfo.UserNo;
                        DrMaster["gacsecg"] = LoginInfo.GroupNo;
                        DrMaster["gaccreu"] = LoginInfo.UserNo;
                        DrMaster["gaccreg"] = LoginInfo.DeptNo;
                        DrMaster["gaccred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["gacmodu"] = LoginInfo.UserNo;
                        DrMaster["gacmodg"] = LoginInfo.DeptNo;
                        DrMaster["gacmodd"] = Now;
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
