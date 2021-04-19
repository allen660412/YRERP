/* 程式名稱: menu設定作業
   系統代號: admi610
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
using YR.ERP.DAL.YRModel;
using Infragistics.Win.UltraWinGrid;
using YR.Util;
using Infragistics.Win.UltraWinToolbars;
using YR.ERP.BLL.Model;

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi610 : YR.ERP.Base.Forms.FrmEntryMDBase
    {

        #region Property
        AdmBLL BoAdm = null;
        #endregion

        #region 建構子
        public FrmAdmi610()
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
            // 繼承的表單要覆寫, 更改參數值            
            /*
            this.strFormID = "XXX";
            this.isDirectEdit = false;
            this.isMultiRowEdit = false;
             */
            this.StrFormID = "admi610";
            IntTabCount = 2;
            IntMasterGridPos = 2;
            uTab_Master.Tabs[0].Text = "資料內容";
            uTab_Master.Tabs[1].Text = "資料瀏覽";

            IntTabDetailCount = 1;
            uTab_Detail.Tabs[0].Text = "明細資料";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.IsReadOnly = true; //存檔時不回存資料庫

            TabMaster.CanCopyMode = false;

            TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("adm01", SqlDbType.NVarChar) });   //假雙檔會咬一個集合
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
        }
        #endregion

        #region WfSetBllTransaction 以bomaster 註冊transaction至其他 bll
        protected override void WfSetBllTransaction()
        {
            try
            {
                if (BoMaster.TRAN != null)
                {
                    BoAdm.TRAN = BoMaster.TRAN;
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
                    WfSetControlsReadOnlyRecursion(this, false);    //先全開
                    WfSetControlReadonly(uGridMaster, true);//grid不可編輯

                    WfSetControlReadonly(ute_adm01_c, true);
                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "adm02_c" }, true);
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

        #region WfIniTabDetailInfo(): 設定明細的資料來源
        protected override Boolean WfIniTabDetailInfo()
        {
            SqlParameter keyParm;
            this.TabDetailList[0].TargetTable = "adm_tb";
            this.TabDetailList[0].ViewTable = "vw_admi610s";
            keyParm = new SqlParameter("adm01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "adm01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr) 設定明細資料列的初始值
        protected override bool WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr)
        {
            try
            {
                switch (pCurTabDetail)
                {
                    case 0:
                        pDr["adm03"] = WfGetMaxSeq(pDr.Table, "adm03");
                        break;
                }
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
            UltraGrid uGrid;
            try
            {
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_admi650
                if (pDr.Table.Prefix.ToLower() == "vw_admi610")
                {
                    switch (pColName.ToLower())
                    {
                        case "adm01"://目錄代碼
                            WfShowPickUtility("p_ado", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ado01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                    }
                }
                #endregion

                #region 單身-pick vw_admi650s
                if (pDr.Table.Prefix.ToLower() == "vw_admi610s")
                {
                    uGrid = sender as UltraGrid;
                    switch (pColName.ToLower())
                    {
                        case "adm02"://資料表名稱
                            WfShowPickUtility("p_ado", messageModel);
                            if (messageModel != null && messageModel.DataRowList.Count > 0)
                            {
                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ado01"], "");
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

        #region WfItemCheck
        //回傳值 true.通過驗證 flase.未通過驗證,還原原來的值
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            try
            {
                #region 單頭-pick vw_admi610
                if (e.Row.Table.Prefix.ToLower() == "vw_admi610")
                {
                    switch (e.Column.ToLower())
                    {
                        case "adm01"://目錄代碼
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["adm01_c"] = "";
                                break;
                            }
                            if (BoAdm.OfChkAdoPKExists(GlobalFn.isNullRet(e.Value, ""), "M") == false)
                            {
                                WfShowErrorMsg("無此目錄代碼");
                                return false;
                            }
                            e.Row["adm01_c"] = BoAdm.OfGetAdo02(GlobalFn.isNullRet(e.Value, ""));
                            break;
                    }
                }
                #endregion

                #region 單頭-pick vw_admi610s
                if (e.Row.Table.Prefix.ToLower() == "vw_admi610s")
                {
                    switch (e.Column.ToLower())
                    {
                        case "adm02"://程式代碼
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["adm02_c"] = "";
                                break;
                            }
                            if (BoAdm.OfChkAdoPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此程式設定資料");
                                return false;
                            }
                            e.Row["adm02_c"] = BoAdm.OfGetAdo02(GlobalFn.isNullRet(e.Value, ""));
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
            vw_admi610 masterModel = null;
            vw_admi610s detailModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {
                masterModel = DrMaster.ToItem<vw_admi610>();
                #region 單頭資料檢查
                if (GlobalFn.varIsNull(masterModel.adm01))
                {
                    chkControl = ute_adm01;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == "adm01").Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion

                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    detailModel = drTemp.ToItem<vw_admi610s>();
                    #region adm02-程式代碼
                    chkColName = "adm02";
                    if (GlobalFn.varIsNull(detailModel.adm02))
                    {
                        this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion
                    
                    #region azq04-序號
                    chkColName = "adm03";
                    if (GlobalFn.varIsNull(detailModel.adm03))
                    {
                        this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion
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
                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["admcreu"] = LoginInfo.UserNo;
                            drDetail["admcreg"] = LoginInfo.DeptNo;
                            drDetail["admcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {                            
                            drDetail["admmodu"] = LoginInfo.UserNo;
                            drDetail["admmodg"] = LoginInfo.DeptNo;
                            drDetail["admmodd"] = Now;
                        }
                    }
                }
                WfSetDetailPK();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAddAction 新增action按鈕
        protected override List<ButtonTool> WfAddAction()
        {
            List<ButtonTool> listBt = new List<ButtonTool>();
            ButtonTool bt;
            try
            {
                bt = new ButtonTool("ReOrderAdm03");
                bt.SharedProps.Caption = "重排序號";
                bt.SharedProps.Category = "action";
                listBt.Add(bt);

                return listBt;
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
            try
            {
                switch (pActionName)
                {
                    case "ReOrderAdm03":
                        if (FormEditMode == YREditType.NA || FormEditMode == YREditType.查詢)
                            return;
                        WfReOrderAdm03();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //*****************************表單自訂Fuction****************************************
        #region WfReOrderAdm03()
        private void WfReOrderAdm03()
        {
            int i = 0;
            try
            {
                foreach (DataRow dr in TabDetailList[0].DtSource.Select("", "adm03"))
                {
                    i++;
                    dr["adm03"] = i;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
