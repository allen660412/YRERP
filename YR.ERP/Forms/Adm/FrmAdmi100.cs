/* 程式名稱: 使用者建立作業
   系統代號: admi100
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
using Infragistics.Win.UltraWinGrid;
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.BLL.Model;

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi100 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region property
        AdmBLL BoAdm = null;
        #endregion

        #region 建構子
        public FrmAdmi100()
        {
            InitializeComponent();
            this.Load += FrmAdmi100_Load;
        }

        void FrmAdmi100_Load(object sender, EventArgs e)
        {
            ute_ada05.Enter += ute_ada05_Enter;
        }

        void ute_ada05_Enter(object sender, EventArgs e)
        {
            try
            {
                if (FormEditMode == YREditType.修改 || FormEditMode == YREditType.新增)
                {
                    ute_ada05.Value = "";
                }
            }
            catch (Exception ex)
            {

                throw ex;
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
            this.StrFormID = "admi100";
            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "資料內容";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ada01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "adasecu";
                TabMaster.GroupColumn = "adasecg";
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
                pDr["ada06"] = "Y";
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
                    WfSetControlReadonly(new List<Control> { ute_adacreu, ute_adacreg, udt_adacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_adamodu, ute_adamodg, udt_adamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_adasecu, ute_adasecg }, true);

                    WfSetControlReadonly(new List<Control> { ute_ada04_c, ute_ada03_c }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_ada01, true);
                    }

                    WfSetControlReadonly(uGridMaster, true);//grid不可編輯
                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "adb02_c" }, true);
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
            SqlParameter lParm;
            // 設定 Detail tab1 資料 :
            this.TabDetailList[0].TargetTable = "adb_tb";
            this.TabDetailList[0].ViewTable = "vw_admi100s";
            lParm = new SqlParameter("adb01", SqlDbType.NVarChar);
            lParm.SourceColumn = "ada01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { lParm };
            return true;
        }
        #endregion

        #region WfItemCheck
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            List<vw_admi100s> detailList = null;
            int chkCnts = 0;
            try
            {
                #region 單頭 vw_admi100
                if (e.Row.Table.Prefix.ToLower() == "vw_admi100")
                {
                    switch (e.Column.ToLower())
                    {
                        case "ada01":   //使用者ID
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (FormEditMode==YREditType.新增)
                            {
                                if (BoAdm.OfChkAdaPKExists(e.Value.ToString()))
                                {
                                    WfShowErrorMsg("使用者ID已存在!");
                                    return false;
                                }
                            }
                            break;

                        case "ada03":   //使用者部門
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ada03_c"] = "";
                                break;
                            }
                            if (BoAdm.OfChkAdePKExists(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此使用者部門代號,請確認!");
                                return false;
                            }
                            e.Row["ada03_c"] = BoAdm.OfGetAde02(e.Value.ToString());

                            break;
                        case "ada04":
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ada04_c"] = "";
                                break;
                            }
                            if (BoAdm.OfChkAdcPKExists(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此角色代號,請確認!");
                                return false;
                            }
                            e.Row["ada04_c"] = BoAdm.OfGetAdc02(e.Value.ToString());
                            break;
                        case "ada05":   //登入密碼 要處理編碼方式
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                if (!GlobalFn.varIsNull(OldValue))
                                {
                                    e.Row["ada05"] = OldValue;
                                    return true;
                                }
                            }
                            e.Row["ada05"] = GlobalFn.genMd5Hash(e.Value.ToString());
                            e.Row["ada06"] = "Y";
                            break;

                        case "ada08":   //預設營運公司
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoAdm.OfChkAtaPKExists(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此公司別,請確認!");
                                return false;
                            }
                            //檢查單身是否有此登入公司
                            detailList = TabDetailList[0].DtSource.ToList<vw_admi100s>();
                            if (detailList!=null)
                            {
                                chkCnts = detailList.Where(p => p.adb02 == e.Value.ToString())
                                                  .Count();                                
                            }
                            if (detailList==null||chkCnts==0)
                            {
                                
                                var result = WfShowConfirmMsg("無此登入公司別權限，是否要新增？");
                                if (result==DialogResult.Yes)
                                {
                                    var newRow = TabDetailList[0].DtSource.NewRow();
                                    WfSetDetailRowDefault(0, newRow);
                                    newRow["adb02"] = e.Value;
                                    var ataModel = BoAdm.OfGetAtaModel(e.Value.ToString());
                                    newRow["adb02_c"] = ataModel.ata02;
                                    TabDetailList[0].DtSource.Rows.Add(newRow);
                                }
                            }
                            break;
                    }
                }
                #endregion

                #region 單身 vw_admi100s
                if (e.Row.Table.Prefix.ToLower() == "vw_admi100s")
                {
                    switch (e.Column.ToLower())
                    {
                        case "adb02":   //可登入公司
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["adb02_c"] = "";
                                return true;
                            }

                            if (BoAdm.OfChkAtaPKExists(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此公司代號,請確認!");
                                return false;
                            }
                            detailList = TabDetailList[0].DtSource.ToList<vw_admi100s>();
                            chkCnts = detailList.Where(p => p.adb02.ToUpper() == e.Value.ToString().ToUpper()).Count();
                            if (chkCnts > 1)
                            {
                                WfShowErrorMsg("公司代碼重覆,請確認!");
                                return false;
                            }

                            e.Row["adb02_c"] = BoAdm.OfGetAta02(e.Value.ToString());
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
            vw_admi100s detailModel = null;
            UltraGrid uGrid;
            try
            {
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_admi100
                if (pDr.Table.Prefix.ToLower() == "vw_admi100")
                {
                    switch (pColName.ToLower())
                    {
                        case "ada03"://使用者部門
                            WfShowPickUtility("p_ade1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ade01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ade01"], "");
                            //}
                            break;

                        case "ada04"://使用者角色
                            WfShowPickUtility("p_adc1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["adc01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "ada08"://預設登入公司
                            WfShowPickUtility("p_ata", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ata01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_admi100s
                if (pDr.Table.Prefix.ToLower() == "vw_admi100s")
                {
                    detailModel = pDr.ToItem<vw_admi100s>();
                    uGrid = sender as UltraGrid;
                    switch (pColName.ToLower())
                    {
                        case "adb02"://資料表名稱
                            WfShowPickUtility("p_ata", messageModel);
                            if (messageModel != null && messageModel.DataRowList.Count > 0)
                            {
                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ata01"], "");
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
            vw_admi100 masterModel = null;
            vw_admi100s detailModel = null;
            List<vw_admi100s> detailList = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {
                masterModel = DrMaster.ToItem<vw_admi100>();
                #region 單頭資料檢查
                #region ada01-使用者ID
                chkControl = ute_ada01;
                if (GlobalFn.varIsNull(masterModel.ada01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == "ada01").Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                if (FormEditMode == YREditType.新增)
                {
                    if (BoAdm.OfChkAdaPKExists(masterModel.ada01))
                    {
                        msg = TabMaster.AzaTbList.Where(p => p.aza03 == "ada01").Select(p => p.aza04).FirstOrDefault();
                        msg += "已存在!";
                        errorProvider.SetError(chkControl, msg);
                        WfShowErrorMsg(msg);
                        return false;
                    }
                }
                #endregion

                #region ada02 -使用者姓名
                if (GlobalFn.varIsNull(masterModel.ada02))
                {
                    chkControl = ute_ada02;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == "ada02").Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion

                #region ada03 -使用者部門
                if (GlobalFn.varIsNull(masterModel.ada03))
                {
                    chkControl = ute_ada03;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == "ada03").Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion

                #region ada04 -使用者角色
                if (GlobalFn.varIsNull(masterModel.ada04))
                {
                    chkControl = ute_ada04;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == "ada04").Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion

                #region ada05 -使用者密碼
                if (GlobalFn.varIsNull(masterModel.ada05))
                {
                    chkControl = ute_ada05;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == "ada05").Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion

                #region ada08 -使用者預設公司別
                if (GlobalFn.varIsNull(masterModel.ada08))
                {
                    chkControl = ute_ada08;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == "ada08").Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                detailList = TabDetailList[0].DtSource.ToList<vw_admi100s>();
                if (detailList == null || detailList.Where(p => p.adb02 == masterModel.ada08).Count() == 0)
                { 
                    chkControl = ute_ada08;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = "未設定該預設公司的可登入權限，請檢核!";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }


                #endregion
                #endregion

                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    detailModel = drTemp.ToItem<vw_admi100s>();
                    #region adb02-登入公司
                    chkColName = "adb02";
                    if (GlobalFn.varIsNull(detailModel.adb02))
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
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["adasecu"] = LoginInfo.UserNo;
                        DrMaster["adasecg"] = LoginInfo.GroupNo;
                        DrMaster["adacreu"] = LoginInfo.UserNo;
                        DrMaster["adacreg"] = LoginInfo.DeptNo;
                        DrMaster["adacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["adamodu"] = LoginInfo.UserNo;
                        DrMaster["adamodg"] = LoginInfo.DeptNo;
                        DrMaster["adamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["adbcreu"] = LoginInfo.UserNo;
                            drDetail["adbcreg"] = LoginInfo.DeptNo;
                            drDetail["adbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["adbmodu"] = LoginInfo.UserNo;
                            drDetail["adbmodg"] = LoginInfo.DeptNo;
                            drDetail["adbmodd"] = Now;
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

    }
}
