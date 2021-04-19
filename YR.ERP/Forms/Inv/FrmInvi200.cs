/* 程式名稱: 倉庫別資料維護作業
   系統代號: invi200
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

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvi200 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        #endregion

        #region 建構子
        public FrmInvi200()
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
            this.StrFormID = "invi200";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("icb01", SqlDbType.NVarChar)
                                });
                TabMaster.UserColumn = "icbsecu";
                TabMaster.GroupColumn = "icbsecg";
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

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                //程式類別
                sourceList = BoInv.OfGetIcb07KVPList();
                WfSetUcomboxDataSource(ucb_icb07, sourceList);

            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                    WfSetControlReadonly(new List<Control> { ute_icbcreu, ute_icbcreg, udt_icbcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_icbmodu, ute_icbmodg, udt_icbmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_icbsecu, ute_icbsecg }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_icb01, true);
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
                pDr["icb03"] = "Y";
                pDr["icb07"] = "1";
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
            vw_invi200 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_invi200>();
                #region 單頭資料檢查
                chkColName = "icb01";
                chkControl = ute_icb01;
                if (GlobalFn.varIsNull(masterModel.icb01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "icb02";
                chkControl = ute_icb02;
                if (GlobalFn.varIsNull(masterModel.icb02))
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
                        DrMaster["icbsecu"] = LoginInfo.UserNo;
                        DrMaster["icbsecg"] = LoginInfo.GroupNo;
                        DrMaster["icbcreu"] = LoginInfo.UserNo;
                        DrMaster["icbcreg"] = LoginInfo.DeptNo;
                        DrMaster["icbcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["icbmodu"] = LoginInfo.UserNo;
                        DrMaster["icbmodg"] = LoginInfo.DeptNo;
                        DrMaster["icbmodd"] = Now;
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
