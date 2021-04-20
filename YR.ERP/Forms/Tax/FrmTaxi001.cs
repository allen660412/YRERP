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
using Infragistics.Win.UltraWinGrid;

namespace YR.ERP.Forms.Tax
{
    public partial class FrmTaxi001 : YR.ERP.Base.Forms.FrmGridEditBase
    {
        #region Property
        TaxBLL BoTax = null;
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmTaxi001()
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
            this.StrFormID = "taxi001";

            this.IntTabCount = 1;
            this.IntMasterGridPos = 1;
            uTab_Master.Tabs[0].Text = "基本資料";

            return true;
        }
        #endregion
        
        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("tba01", SqlDbType.NVarChar) });
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
            BoTax = new TaxBLL(BoMaster.OfGetConntion());
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
                    BoTax.TRAN = BoMaster.TRAN;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        
        #region WfDisplayMode 
        protected override bool WfDisplayMode()
        {
            try
            {
                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlReadonly(uGridMaster, true);
                    return true;
                }
                WfSetControlReadonly(uGridMaster, false);
                
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion
        
        #region WfBeforeCellActivate 進入GridCell前事件
        protected override bool WfBeforeCellActivate(BeforeEnterCellInfo e)
        {
            if (e.Column=="tba01")
            {
                if (FormEditMode == YREditType.新增)
                {
                    WfSetControlReadonly(e.Cell, false);
                }
                else
                    WfSetControlReadonly(e.Cell, true);
            }
            
            return true;
        }
        #endregion
        
        #region WfItemCheck
        //回傳值 false未通過驗證,還原輸入的值 true.未通過驗證,保留原值
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            Result resultModel = null;
            try
            {
                #region 單頭-pick vw_taxi001
                if (e.Row.Table.Prefix.ToLower() == "vw_taxi001")
                {
                    switch (e.Column.ToLower())
                    {
                        #region tba01 申報部門編號
                        case "tba01":
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            var bebModel = BoBas.OfGetBebModel(e.Value.ToString());
                            //if (bebModel==null)
                            //{
                            //    WfShowErrorMsg("查無此部門!");
                            //    return false;
                            //}
                            //if (bebModel.bebvali.ToUpper() !="Y")
                            //{
                            //    WfShowErrorMsg("該部門已失效!");
                            //    return false;
                            //}

                            if (BoTax.OfChkTbaPKExists(e.Value.ToString())==true)
                            {
                                WfShowErrorMsg("此申報部門已存在!");
                                return false;
                            }
                            break;
                        #endregion

                        #region tba02 統一編號
                        case "tba02":
                            if (GlobalFn.varIsNull(e.Value))                            
                            {                                
                                return true;
                            }
                            resultModel = BoTax.OfChkUniformNumber(e.Value.ToString());
                            if (resultModel.Success == false)
                            {
                                WfShowErrorMsg(resultModel.Message);
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
        
        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                vw_taxi001 masterModel = null;
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_stpt400
                if (pDr.Table.Prefix.ToLower() == "vw_taxi001")
                {
                    masterModel = DrMaster.ToItem<vw_taxi001>();
                    switch (pColName.ToLower())
                    {
                        case "tba01"://申報部門編號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
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
            vw_taxi001 masterModel = null;
            string msg;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_taxi001>();
                chkColName = "tba01";
                #region 申報部門編號
                if (GlobalFn.varIsNull(masterModel.tba01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    WfFindErrUltraGridCell(uGridMaster, DrMaster, chkColName);
                    WfShowErrorMsg(msg);
                    return false;
                }
                //if (FormEditMode == YREditType.新增 && BoPur.OfChkPbaPKExists(GlobalFn.isNullRet(masterModel.pba01, "")) == true)
                //{
                //    msg = "廠商分類編號已存在,請檢核!";
                //    errorProvider.SetError(chkControl, msg);
                //    WfShowErrorMsg(msg);
                //    return false;
                //}
                #endregion

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
