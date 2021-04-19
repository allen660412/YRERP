/* 程式名稱: 銷項發票維護作業
   系統代號: cart110
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
using YR.ERP.Shared;
using Infragistics.Win.UltraWinTabControl;
using YR.ERP.BLL.MSSQL.Gla;
using YR.ERP.BLL.MSSQL.Car;

namespace YR.ERP.Forms.Car
{
    public partial class FrmCart110 : YR.ERP.Base.Forms.FrmEntryBase
    {

        #region Property
        BasBLL BoBas = null;
        CarBLL BoCar = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;
        StpBLL BoStp = null;

        //baa_tb BaaTbModel = null;
        bek_tb BekTbModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmCart110()
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
            this.StrFormID = "cart110";
            uTab_Header.Tabs[0].Text = "基本資料";
            uTab_Header.Tabs[1].Text = "狀態";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "應收明細";
            uTab_Master.Tabs[1].Text = "其他資料";
            uTab_Master.Tabs[2].Text = "資料瀏覽";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("cee01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "ceesecu";
                TabMaster.GroupColumn = "ceesecg";
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

            BoCar = new CarBLL(BoMaster.OfGetConntion());
            BoBas = new BasBLL(BoMaster.OfGetConntion());
            BoStp = new StpBLL(BoMaster.OfGetConntion());
            BoInv = new InvBLL(BoMaster.OfGetConntion());
            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
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
                    BoCar.TRAN = BoMaster.TRAN;
                    BoBas.TRAN = BoMaster.TRAN;
                    BoStp.TRAN = BoMaster.TRAN;
                    BoInv.TRAN = BoMaster.TRAN;
                    BoAdm.TRAN = BoMaster.TRAN;
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
            StringBuilder sbSql;
            try
            {

                //課稅別
                sourceList = BoCar.OfGetTaxTypeKVPList();
                WfSetUcomboxDataSource(ucb_cee10, sourceList);

                ////發票聯數
                sourceList = BoCar.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_cee13, sourceList);

                //////單據確認
                //sourceList = BoCar.OfGetCeaconfKVPList();
                //WfSetUcomboxDataSource(ucb_ceaconf, sourceList);

                //////單據狀態
                //sourceList = BoCar.OfGetCeastatKVPList();
                //WfSetUcomboxDataSource(ucb_ceastat, sourceList);
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
            vw_cart110 masterModel = null;
            try
            {
                //if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                //{
                //    masterModel = DrMaster.ToItem<vw_cart110>();
                //    //WfSetDocPicture("", masterModel.ceaconf, masterModel.ceastat, pbxDoc);
                //    if (GlobalFn.varIsNull(masterModel.cea10) != true
                //            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                //    {
                //        BekTbModel = BoBas.OfGetBekModel(masterModel.cea10);
                //        if (BekTbModel == null)
                //        {
                //            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.cea10));
                //        }
                //    }
                //}
                //else
                //{
                //    WfSetDocPicture("", "", "", pbxDoc);
                //}

                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false); //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯

                    WfSetControlReadonly(new List<Control> { ute_ceecreu, ute_ceecreg, udt_ceecred }, true);
                    WfSetControlReadonly(new List<Control> { ute_ceemodu, ute_ceemodg, udt_ceemodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_ceesecu, ute_ceesecg }, true);
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
                uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                SelectNextControl(this.uTab_Header, true, true, true, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
