/* 程式名稱: 庫存盤點重計作業
   系統代號: invt501
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

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvb501 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        InvBLL BoInv = null;
        #endregion

        #region 建構子
        public FrmInvb501()
        {
            InitializeComponent();
        }

        public FrmInvb501(string pSourceForm, YR.ERP.Shared.UserInfo pUserInfo, string pWhere)
        {
            InitializeComponent();
            StrQueryWhereAppend = pWhere;
            this.LoginInfo = pUserInfo;
        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            this.StrFormID = "invb501";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "基本資料";
            uTab_Master.Tabs[1].Text = "狀態";
            uTab_Master.Tabs[2].Text = "資料瀏覽";

            IntTabDetailCount = 1;
            uTab_Detail.Tabs[0].Text = "明細資料";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ipa01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "ipasecu";
                TabMaster.GroupColumn = "ipasecg";

                TabMaster.CanAddMode = false;
                TabMaster.CanUpdateMode = false;
                TabMaster.CanCopyMode = false;
                TabMaster.CanDeleteMode = false;
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
                    BoInv.TRAN = BoMaster.TRAN;
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

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniTabDetailInfo(): 設定明細的資料來源
        protected override Boolean WfIniTabDetailInfo()
        {
            SqlParameter keyParm;

            this.TabDetailList[0].TargetTable = "ipb_tb";
            this.TabDetailList[0].ViewTable = "vw_invb501s";
            keyParm = new SqlParameter("ipb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "ipa01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfAddAction 新增action按鈕
        protected override List<ButtonTool> WfAddAction()
        {
            List<ButtonTool> buttonList = new List<ButtonTool>();
            ButtonTool bt;
            try
            {
                bt = new ButtonTool("ReCalculate");
                bt.SharedProps.Caption = "重計";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                return buttonList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfActionClick(string pActionName)
        protected override void WfActionClick(string pActionName)
        {
            vw_invb501 masterModel;
            try
            {
                switch (pActionName)
                {
                    case "ReCalculate":
                        WfRetrieveMaster();
                        masterModel = DrMaster.ToItem<vw_invb501>();
                        if (GlobalFn.isNullRet(masterModel.ipa05,"")=="Y")
                        {
                            WfShowBottomStatusMsg("已重計過!");
                            return;
                        }

                        WfReCalculate(masterModel.ipa01);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfReCalculate
        private bool WfReCalculate(string ipa01)
        {
            icc_tb iccModel;
            string selectSql;
            try
            {
                //lock master row 並取得transaction物件
                if (WfLockMasterRow() == false)
                {
                    WfShowErrorMsg("Lock ipa_file 失敗!");
                    return false;
                }
                WfRetrieveMaster();//更新主檔資料
                WfRetrieveDetail(); //更新明細資料
                WfSetBllTransaction();  //相關bll 註冊transaction
                TabDetailList[0].BoBasic.TRAN = BoMaster.TRAN;

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    var ipbModel = drDetail.ToItem<ipb_tb>();
                    iccModel = BoInv.OfGetIccModel(ipbModel.ipb03, ipbModel.ipb04);
                    drDetail["ipb06"] = iccModel.icc05;
                    drDetail["ipbmodu"] = LoginInfo.UserNo;
                    drDetail["ipbmodg"] = LoginInfo.DeptNo;
                    drDetail["ipbmodd"] = Now;
                }
                DrMaster["ipa05"] = "Y";
                DrMaster["ipa06"] = Today;
                DrMaster["ipamodu"] = LoginInfo.UserNo;
                DrMaster["ipamodg"] = LoginInfo.DeptNo;
                DrMaster["ipamodd"] = Now;

                BoMaster.OfUpdate(TabMaster.DtSource);
                TabDetailList[0].BoBasic.OfUpdate(TabDetailList[0].DtSource);

                if (WfCommit() == false)
                {
                    DrMaster.RejectChanges();
                    TabDetailList[0].DtSource.RejectChanges();
                    WfRollback();
                    WfShowErrorMsg("執行commit失敗!");
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                DrMaster.RejectChanges();
                TabDetailList[0].DtSource.RejectChanges();
                WfRollback();
                throw ex;
            }
        }
        #endregion
    }
}
