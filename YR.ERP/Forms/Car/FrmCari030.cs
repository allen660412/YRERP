/* 程式名稱: 應收科目類別維護作業
   系統代號: cari030
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

namespace YR.ERP.Forms.Car
{
    public partial class FrmCari030 : YR.ERP.Base.Forms.FrmEntryBase
    {

        #region Property
        BasBLL BoBas = null;
        CarBLL BoCar = null;
        GlaBLL BoGla = null;
        #endregion

        #region 建構子
        public FrmCari030()
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
            this.StrFormID = "cari030";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "資料內容";
            uTab_Master.Tabs[1].Text = "其他資料";
            uTab_Master.Tabs[2].Text = "資料瀏覽";

            return true;
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);

            BoBas = new BasBLL(BoMaster.OfGetConntion());
            BoCar = new CarBLL(BoMaster.OfGetConntion());
            BoGla = new GlaBLL(BoMaster.OfGetConntion());
            return;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("cba01", SqlDbType.NVarChar) });
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
                    BoCar.TRAN = BoMaster.TRAN;
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
            vw_cari030 masterModel = null;
            try
            {
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else//新增與修改
                {
                    masterModel = DrMaster.ToItem<vw_cari030>();

                    WfSetControlsReadOnlyRecursion(this, false);//先全開
                    WfSetControlReadonly(uGridMaster, true);//grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_cbacreu, ute_cbacreg, udt_cbacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_cbamodu, ute_cbamodg, udt_cbamodd }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_cba01, true);
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
            int iChkCnts = 0;
            vw_cari030 cari030Model;
            try
            {
                cari030Model = DrMaster.ToItem<vw_cari030>();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_cari030
                if (pDr.Table.Prefix.ToLower() == "vw_cari030")
                {
                    switch (pColName.ToLower())
                    {
                        case "cba03":       //應收帳款科目
                            messageModel.StrWhereAppend = " AND gba06 in ('2','3')";
                            WfShowPickUtility("p_gba1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["gba01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "cba04":       //銷貨收入科目
                            messageModel.StrWhereAppend = " AND gba06 in ('2','3')";
                            WfShowPickUtility("p_gba1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["gba01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "cba05":       //銷項稅額科目
                            messageModel.StrWhereAppend = " AND gba06 in ('2','3')";
                            WfShowPickUtility("p_gba1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["gba01"], "");
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_cari030 masterModel = null;
            gba_tb gbaModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_cari030>();

                #region 單頭 vw_cart100
                if (e.Row.Table.Prefix.ToLower() == "vw_cari030")
                {
                    switch (e.Column.ToLower())
                    {
                        case "cba01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (FormEditMode == YREditType.新增)
                            {
                                if (BoCar.OfChkCbaPKExists(masterModel.cba01) == true)
                                {
                                    WfShowErrorMsg("此科目分類碼已存在,請檢查!");
                                    return false;
                                }

                            }
                            break;

                        case "cba03"://應收帳款
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                return true;
                            }
                            gbaModel = BoGla.OfGetGbaModel(e.Value.ToString());
                            if (gbaModel == null)
                            {
                                WfShowErrorMsg("無此會計科目,請檢核!");
                                return false;
                            }
                            if (gbaModel.gbavali != "Y")
                            {
                                WfShowErrorMsg("此會計科目已失效,請檢核!");
                                return false;
                            }
                            if (gbaModel.gba06 != "2" && gbaModel.gba06 != "3")
                            {
                                WfShowErrorMsg("會計科目非明細或獨立科目,請檢核!");
                                return false;
                            }
                            break;

                        case "cba04"://銷貨收入
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                return true;
                            }
                            gbaModel = BoGla.OfGetGbaModel(e.Value.ToString());
                            if (gbaModel == null)
                            {
                                WfShowErrorMsg("無此會計科目,請檢核!");
                                return false;
                            }
                            if (gbaModel.gbavali != "Y")
                            {
                                WfShowErrorMsg("此會計科目已失效,請檢核!");
                                return false;
                            }
                            if (gbaModel.gba06 != "2" && gbaModel.gba06 != "3")
                            {
                                WfShowErrorMsg("會計科目非明細或獨立科目,請檢核!");
                                return false;
                            }
                            break;

                        case "cba05"://銷項稅額
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                return true;
                            }
                            gbaModel = BoGla.OfGetGbaModel(e.Value.ToString());
                            if (gbaModel == null)
                            {
                                WfShowErrorMsg("無此會計科目,請檢核!");
                                return false;
                            }
                            if (gbaModel.gbavali != "Y")
                            {
                                WfShowErrorMsg("此會計科目已失效,請檢核!");
                                return false;
                            }
                            if (gbaModel.gba06 != "2" && gbaModel.gba06 != "3")
                            {
                                WfShowErrorMsg("會計科目非明細或獨立科目,請檢核!");
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
            vw_cari030 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {

                masterModel = DrMaster.ToItem<vw_cari030>();
                #region 單頭資料檢查
                #region ado01 程式代號
                chkColName = "cba01";
                chkControl = ute_cba01;
                if (GlobalFn.varIsNull(masterModel.cba01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                //檢查是否重覆
                if (FormEditMode == YREditType.新增)
                {
                    if (BoCar.OfChkCbaPKExists(masterModel.cba01) == true)
                    {
                        this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                        chkControl.Focus();
                        msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "已存在,請檢查!";
                        errorProvider.SetError(chkControl, msg);
                        WfShowErrorMsg(msg);
                        return false;
                    }
                }

                #endregion

                chkColName = "cba02";       //科目分類說明
                chkControl = ute_cba02;
                if (GlobalFn.varIsNull(masterModel.cba02))
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
                        DrMaster["cbacreu"] = LoginInfo.UserNo;
                        DrMaster["cbacreg"] = LoginInfo.DeptNo;
                        DrMaster["cbacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["cbamodu"] = LoginInfo.UserNo;
                        DrMaster["cbamodg"] = LoginInfo.DeptNo;
                        DrMaster["cbamodd"] = Now;
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
