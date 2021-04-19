/* 程式名稱: 流程圖元件位置
   系統代號: admi611s
   作　　者: Allen
   描　　述: 僅做為 admi611呼叫使用,本身無CRUD功能
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi611s : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        AdmBLL BoAdm = null;

        YREditType _srcFormState = new YREditType();//表單傳入要執行的狀態
        vw_admi611s _srcVwAdmi611s = null;
        #endregion

        #region 建構子
        public FrmAdmi611s()
        {
            InitializeComponent();
        }

        public FrmAdmi611s(YREditType pYREditType,  vw_admi611s pVwAdmi611s)
        {
            InitializeComponent();
            this._srcFormState = pYREditType;
            this._srcVwAdmi611s = pVwAdmi611s;
        } 
        #endregion

        #region FrmAdmi611s_Load
        private void FrmAdmi611s_Load(object sender, EventArgs e)
        {
            ToolClickEventArgs eTool;
            ToolBase utb;
            if (_srcFormState == YREditType.新增)
            {
                utb = UtbmMain.Tools["btinsert"];
                eTool = new ToolClickEventArgs(utb, null);
                UtbmMain_ToolClick(UtbmMain, eTool);
            }
            else if (_srcFormState == YREditType.修改)
            {
                utb = UtbmMain.Tools["btupdate"];
                var drModify = TabMaster.DtSource.NewRow();
                drModify["ady01"] = _srcVwAdmi611s.ady01;
                drModify["ady02"] = _srcVwAdmi611s.ady02;
                drModify["ady03"] = _srcVwAdmi611s.ady03;
                drModify["ady03_c"] = BoAdm.OfGetAdo02(_srcVwAdmi611s.ady03);
                drModify["ady04"] = _srcVwAdmi611s.ady04;
                drModify["ady05"] = _srcVwAdmi611s.ady05;
                drModify["ady06"] = _srcVwAdmi611s.ady06;
                drModify["ady07"] = _srcVwAdmi611s.ady07;
                TabMaster.DtSource.Rows.Add(drModify);

                uGridMaster.ActiveRow = uGridMaster.Rows[0];
                //bindingMaster.MoveFirst();
                eTool = new ToolClickEventArgs(utb, null);
                //DRMASTER = drModify;
                UtbmMain_ToolClick(UtbmMain, eTool);
                TabMaster.DtSource.AcceptChanges();     //先接受方便後續還原為舊值
            }
        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            this.StrFormID = "admi611s";

            this.IntTabCount = 1;
            this.IntMasterGridPos = 0;
            uTab_Master.Tabs[0].Text = "資料內容";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ady01", SqlDbType.NVarChar),
                                                                                new SqlParameter("ady02", SqlDbType.Int)
                                                                                });

                TabMaster.CanCopyMode = false;
                TabMaster.CanDeleteMode = false;
                TabMaster.CanQueryMode = false;
                TabMaster.CanUseRowLock = false;    //不使用lock處理
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
                    BoAdm.TRAN = BoMaster.TRAN;
                }
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
                pDr["ady01"] = _srcVwAdmi611s.ady01;
                pDr["ady02"] = _srcVwAdmi611s.ady02;
                pDr["ady03"] = _srcVwAdmi611s.ady03;
                pDr["ady04"] = _srcVwAdmi611s.ady04;
                pDr["ady05"] = _srcVwAdmi611s.ady05;
                pDr["ady06"] = _srcVwAdmi611s.ady06;
                pDr["ady07"] = _srcVwAdmi611s.ady07;
                return true;
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
                    WfSetControlReadonly(uGridMaster, true);//grid不可編輯
                    WfSetControlReadonly(new List<Control>() { ute_ady01, ute_ady02, ute_ady03_c }, true);
                }

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
            vw_admi611s detailModel;

            try
            {
                detailModel = DrMaster.ToItem<vw_admi611s>();
                #region 單頭-pick vw_admi611s
                if (e.Row.Table.Prefix.ToLower() == "vw_admi611s")
                {
                    switch (e.Column.ToLower())
                    {
                        case "ady03":   //程式代號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                DrMaster["ady03_c"] = "";
                                return true;
                            }
                            var adoModel = BoAdm.OfGetAdoModel(e.Value.ToString());
                            if (adoModel==null)
                            {
                                WfShowErrorMsg("無此程式代號,請確認!");
                                return false;
                            }
                            if (adoModel.ado07.ToUpper()=="M")
                            {
                                WfShowErrorMsg("不可選擇目錄,請確認!");
                                return false;
                            }
                            DrMaster["ady03_c"] = adoModel.ado02;
                            break;
                        case "ady04":
                            if (e.Value == null)
                            {
                                WfShowErrorMsg("欄位不可空白,請確認!");
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) < 0)
                            {
                                WfShowErrorMsg("欄位不可為負數,請確認!");
                                return false;
                            }
                            break;
                        case "ady05":
                            if (e.Value == null)
                            {
                                WfShowErrorMsg("欄位不可空白,請確認!");
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) < 0)
                            {
                                WfShowErrorMsg("欄位不可為負數,請確認!");
                                return false;
                            }
                            break;
                        case "ady06":
                            if (e.Value == null)
                            {
                                WfShowErrorMsg("欄位不可空白,請確認!");
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) < 0)
                            {
                                WfShowErrorMsg("欄位不可為負數,請確認!");
                                return true;
                            }
                            break;
                        case "ady07":
                            if (e.Value == null)
                            {
                                WfShowErrorMsg("欄位不可空白,請確認!");
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) < 0)
                            {
                                WfShowErrorMsg("欄位不可為負數,請確認!");
                                return true;
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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_admi611s
                if (pDr.Table.Prefix.ToLower() == "vw_admi611s")
                {
                    switch (pColName.ToLower())
                    {
                        case "ady03"://程式代碼
                            WfShowPickUtility("p_ado1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ado01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ado01"], "");
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

        #region WfAfterFormCheck() 存檔後處理,通常為放入Pk
        protected override bool WfAfterFormCheck()
        {
            try
            {
                if (_srcFormState==YREditType.新增)
                {
                    DrMaster["adycreu"] = LoginInfo.UserNo;
                    DrMaster["adycreg"] = LoginInfo.DeptNo;
                    DrMaster["adycred"] = Now;
                    _srcVwAdmi611s.adycreu = LoginInfo.UserNo;
                    _srcVwAdmi611s.adycreg = LoginInfo.DeptNo;
                    _srcVwAdmi611s.adycred = Now;
                }
                else if (_srcFormState == YREditType.修改)
                {
                    DrMaster["adymodu"] = LoginInfo.UserNo;
                    DrMaster["adymodg"] = LoginInfo.DeptNo;
                    DrMaster["adymodd"] = Now;
                    _srcVwAdmi611s.adymodu = LoginInfo.UserNo;
                    _srcVwAdmi611s.adymodg = LoginInfo.DeptNo;
                    _srcVwAdmi611s.adymodd = Now;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //*****************************覆寫 功能****************************************

        #region WfToolbarSave() : 主表存檔 function --覆寫 存檔功能
        /// <summary>
        /// 主表存檔 function
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfToolbarSave()
        {
            uGridMaster.PerformAction(UltraGridAction.ExitEditMode);
            uGridMaster.UpdateData();
            BindingMaster.EndEdit();

            if (DrMaster == null)
            {
                WfShowErrorMsg("查無要存檔的資料!");
                return false;
            }
            try
            {
                //if (BOMASTER.TRAN==null)
                //{
                //    if (this.WfBeginTran() == false)
                //    { return false; }
                //}

                WfSetBllTransaction();

                this.errorProvider.Clear();
                if (this.WfFormCheck() == false)
                {
                    //WfRollback();
                    return false;
                }

                if (this.WfAfterFormCheck() == false)
                {
                    //WfRollback();
                    return false;
                }

                //if (this.TabMaster.DtSource.GetChanges() != null)
                //{
                //    if (this.TabMaster.BoBasic.OfUpdate(BOMASTER.TRAN, this.TabMaster.DtSource.GetChanges()) < 0)
                //    {
                //        { throw new Exception("儲存主表時失敗(boBasic.of_update)，請檢核 !"); }
                //    }
                //}

                if (!this.WfAppendUpdate())
                    throw (new Exception("同步資料時發生錯誤(wf_append_update)，請檢核 !"));

                //if (this.WfCommit() == false)
                //{ throw new Exception("確認 (commit) 交易時發生錯誤，請檢核 !"); }
            }
            catch (Exception ex)
            {
                //this.WfRollback();
                throw ex;
            }
            finally
            {
                //this.Cursor = Cursors.Default; //改到工具列處理
            }

            this.TabMaster.DtSource.AcceptChanges();
            this.FormEditMode = YREditType.NA;
            WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
            WfShowBottomStatusMsg("存檔成功!");
            this.IsChanged = false;
            this.DialogResult = DialogResult.Yes;
            this.Close();
            return true;
        }
        #endregion 刪除主表記錄 function

        #region WfSaveCancel() 新增或修改後按下取消按鈕 --覆寫 存檔功能
        protected override void WfSaveCancel()
        {
            try
            {
                //if (BOMASTER.TRAN != null)
                //    WfRollback();//TODO:先全部都ROLLBACK
                IsInSaveCancle = true;

                this.errorProvider.Clear();
                BindingMaster.CancelEdit();
                TabMaster.DtSource.RejectChanges();

                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.FormEditMode = YREditType.NA;
                WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                this.IsChanged = false;
                this.IsInSaveCancle = false;
            }
        }
        #endregion

        #region WfPreInUpdateMode--覆寫進修改時的預設處理
        protected override bool WfPreInUpdateMode()
        {
            try
            {
                //進修改模式時,要重新查詢master資料,避免資料dirty
                //if (WfRetrieveMaster() == false)
                //    return false;

                if (TabMaster.CanUseRowLock == true)
                {
                    if (WfLockMasterRow() == false)
                        return false;
                }
                else
                    if (WfBeginTran() == false)
                        return false;


                WfSetBllTransaction();

                if (WfPreInUpdateModeCheck() == false)
                {
                    if (BoMaster.TRAN != null)
                        WfRollback();
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

        //*****************************表單自訂Fuction****************************************

    }
}
