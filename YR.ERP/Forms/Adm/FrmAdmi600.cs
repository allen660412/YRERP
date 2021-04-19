/* 程式名稱: 程式建檔作業
   系統代號: admi600
   作　　者: Allen
   描　　述: 
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/

using Infragistics.Win.UltraWinToolbars;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.DAL.YRModel;
using YR.Util;

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi600 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        AdmBLL BoAdm = null;
        #endregion

        #region 建構子
        public FrmAdmi600()
        {
            InitializeComponent();
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
            this.StrFormID = "admi600";
            IntTabCount = 3;
            IntMasterGridPos = 3;
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ado01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "adosecu";
                TabMaster.GroupColumn = "adosecg";
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
            try
            {
                //程式類別
                sourceList = BoAdm.OfGetAdo07KVPList();
                WfSetUcomboxDataSource(ucb_ado07, sourceList);
                
                //模組別
                sourceList = BoAdm.OfGetAze01KVPList();
                WfSetUcomboxDataSource(ucb_ado12, sourceList);

                //報表類型
                sourceList = BoAdm.OfGetAdo13KVPList();
                sourceList.Insert(0, new KeyValuePair<string, string>("", ""));
                WfSetUcomboxDataSource(ucb_ado13, sourceList);
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
            vw_admi600 masterModel = null;
            try
            {
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else//新增與修改
                {
                    masterModel = DrMaster.ToItem<vw_admi600>();

                    WfSetControlsReadOnlyRecursion(this, false);//先全開
                    WfSetControlReadonly(uGridMaster, true);//grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_adocreu, ute_adocreg, udt_adocred }, true);
                    WfSetControlReadonly(new List<Control> { ute_adomodu, ute_adomodg, udt_adomodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_adosecu, ute_adosecg }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_ado01, true);
                    }
                    WfSetControlReadonly(new List<Control> { ute_ado10, ute_ado11 }, true);
                    WfSetAdo07RelReadonly(masterModel.ado07);
                    if (masterModel.ado09 == "Y" || masterModel.ado07=="R") //程式有自動報表功能及 程式類型為報表時都要可以改報表名稱
                        WfSetControlReadonly(ute_ado10, false);
                    else
                        WfSetControlReadonly(ute_ado10, true);

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
                pDr["ado11"] = 0;
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
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            vw_admi600 admi600Model;
            try
            {
                admi600Model = DrMaster.ToItem<vw_admi600>();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_admi600
                if (pDr.Table.Prefix.ToLower() == "vw_admi600")
                {
                    switch (pColName.ToLower())
                    {
                        case "ado14"://流程圖ID
                            WfShowPickUtility("p_adx", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["adx01"], "");
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
        //回傳值 true.通過驗證 false.未通過驗證,
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            vw_admi600 admi600Model;

            try
            {
                admi600Model = DrMaster.ToItem<vw_admi600>();
                #region 單頭-pick vw_admi600
                if (e.Row.Table.Prefix.ToLower() == "vw_admi600")
                {
                    switch (e.Column.ToLower())
                    {
                        case "ado01":   //程式代號
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (FormEditMode == YREditType.新增)
                            {
                                if (BoAdm.OfChkAdoPKExists(admi600Model.ado01) == true)
                                {
                                    WfShowErrorMsg("程式代號已存在,請檢查!");
                                    return false;
                                }

                            }
                            break;

                        case "ado07"://程式類別 P.程式 M.menu R.報表
                            if (GlobalFn.varIsNull(admi600Model.ado07) || admi600Model.ado07 == "M")
                            {
                                e.Row["ado03"] = "";
                                e.Row["ado04"] = "";
                                e.Row["ado05"] = "";
                                e.Row["ado06"] = "";
                                e.Row["ado08"] = "N";
                                e.Row["ado09"] = "N";
                                e.Row["ado10"] = "";
                                e.Row["ado11"] = 0;
                                e.Row["ado12"] = "";
                                e.Row["ado13"] = "";
                                e.Row["ado15"] = "";
                                e.Row["ado16"] = "N";
                            }
                            else if (admi600Model.ado07 == "R")
                            {
                                e.Row["ado08"] = "N";
                                e.Row["ado09"] = "N";
                                e.Row["ado13"] = "";
                                e.Row["ado15"] = "";
                                e.Row["ado16"] = "N";
                                WfItemChkForceFocus(ucb_ado13);
                            }
                            WfDisplayMode();
                            break;
                        case "ado09":   //自動化報表
                            if (GlobalFn.isNullRet(e.Value, "") == "Y")
                            {
                                WfSetControlReadonly(ute_ado10, false);
                                WfItemChkForceFocus(ute_ado10);
                            }
                            else
                            {
                                WfSetControlReadonly(ute_ado10, true);
                                e.Row["ado10"] = ""; //報表名稱
                                e.Row["ado13"] = "";
                            }
                            break;

                        case "ado14":   //流程圖ID
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoAdm.OfChkAdxPKExists(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此流程圖ID,請確認!");
                                return false;
                            }
                            break;
                        //case "ado15":   //Action功能
                        //    if (GlobalFn.varIsNull(e.Value))
                        //        return true;
                        //    if (BoAdm.OfChkAtc02Exists(e.Value.ToString()) == false)
                        //    {
                        //        WfShowMsg("無此欄位,請確認!");
                        //        return false;
                        //    }
                        //    break;
                        //case "ado16":   //是否為子程式
                        //    if (GlobalFn.varIsNull(e.Value))
                        //        return true;
                        //    if (BoAdm.OfChkAtc02Exists(e.Value.ToString()) == false)
                        //    {
                        //        WfShowMsg("無此欄位,請確認!");
                        //        return false;
                        //    }
                        //    break;
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
            vw_admi600 admi600Model = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {

                admi600Model = DrMaster.ToItem<vw_admi600>();
                #region 單頭資料檢查
                #region ado01 程式代號
                chkColName = "ado01";
                chkControl = ute_ado01;
                if (GlobalFn.varIsNull(admi600Model.ado01))
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
                    if (BoAdm.OfChkAdoPKExists(admi600Model.ado01) == true)
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

                chkColName = "ado02";       //程式名稱
                chkControl = ute_ado02;
                if (GlobalFn.varIsNull(admi600Model.ado02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ado07";       //程式類別 P.程式 M.menu
                chkControl = ucb_ado07;
                if (GlobalFn.varIsNull(admi600Model.ado07))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ado03";       //組件dll
                chkControl = ute_ado03;
                if (admi600Model.ado07 == "P" && GlobalFn.varIsNull(admi600Model.ado03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ado04";       //CLASS名稱
                chkControl = ute_ado04;
                if ((admi600Model.ado07 == "P" || admi600Model.ado07 == "R") && GlobalFn.varIsNull(admi600Model.ado04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ado05";       //CRUD資料表
                chkControl = ute_ado05;
                if ((admi600Model.ado07 == "P" || admi600Model.ado07 == "R") && GlobalFn.varIsNull(admi600Model.ado05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ado06";       //使用view名稱
                chkControl = ute_ado06;
                if ((admi600Model.ado07 == "P" || admi600Model.ado07 == "R") && GlobalFn.varIsNull(admi600Model.ado06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ado10";       //報表名稱
                chkControl = ute_ado06;
                if ((admi600Model.ado09 == "Y") && GlobalFn.varIsNull(admi600Model.ado09))
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
                        DrMaster["adosecu"] = LoginInfo.UserNo;
                        DrMaster["adosecg"] = LoginInfo.GroupNo;
                        DrMaster["adocreu"] = LoginInfo.UserNo;
                        DrMaster["adocreg"] = LoginInfo.DeptNo;
                        DrMaster["adocred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["adomodu"] = LoginInfo.UserNo;
                        DrMaster["adomodg"] = LoginInfo.DeptNo;
                        DrMaster["adomodd"] = Now;
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

        #region WfAddAction 新增action按鈕
        protected override List<ButtonTool> WfAddAction()
        {
            List<ButtonTool> listBt = new List<ButtonTool>();
            ButtonTool bt;
            try
            {
                bt = new ButtonTool("admi601");
                bt.SharedProps.Caption = "對應報表設定作業";
                bt.SharedProps.Category = "action";
                listBt.Add(bt);

                bt = new ButtonTool("admi602");
                bt.SharedProps.Caption = "對應功能設定作業";
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
            vw_admi600 masterModel;
            StringBuilder sbSql;
            YR.ERP.Base.Forms.FrmBase frmActive = null;
            try
            {
                switch (pActionName.ToLower())
                {
                    case "admi601":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_admi600>();
                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND ado01='{0}'", masterModel.ado01));
                        WfShowForm("admi601", false, new object[] { "admi600", this.LoginInfo, sbSql.ToString() });
                        break;

                    case "admi602":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_admi600>();
                        sbSql = new StringBuilder();
                        sbSql.AppendLine(string.Format(" AND ado01='{0}'", masterModel.ado01));
                        WfShowForm("admi602", false, new object[] { "admi600", this.LoginInfo, sbSql.ToString() });
                        break;

                    #region genaction
                    case "genaction":
                        if (DrMaster == null)
                            return;
                        if (FormEditMode != YREditType.NA)
                            return;
                        masterModel = DrMaster.ToItem<vw_admi600>();

                        var adoModle = BoAdm.OfGetAdoModel(masterModel.ado01);
                        if (AdoModel == null)
                        {
                            WfShowErrorMsg("無此程式代號");
                            return;
                        }

                        //先檢查是否有transction
                        if (BoMaster.TRAN != null)
                            WfCommit();

                        if (TabMaster.CanUseRowLock == true)
                            WfLockMasterRow();  //Lock row 並且產生transaction
                        else
                            WfBeginTran();
                        try
                        {

                            var assembly = System.Reflection.Assembly.LoadFile(Path.Combine(Application.StartupPath, adoModle.ado03));
                            var type = assembly.GetType(adoModle.ado04);
                            frmActive = Activator.CreateInstance(type) as YR.ERP.Base.Forms.FrmBase;
                            frmActive.LoginInfo = this.LoginInfo;
                            frmActive.WindowState = FormWindowState.Minimized;
                            frmActive.Show();
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            var actionDic = frmActive.ActionDic;
                            var reportDic = frmActive.ReportDic;
                            if (actionDic != null)
                            {
                                foreach (KeyValuePair<string, string> keyValue in actionDic)
                                {
                                    dic.Add(keyValue.Key, keyValue.Value);
                                }
                            }

                            if (reportDic != null)
                            {
                                foreach (KeyValuePair<string, string> keyValue in reportDic)
                                {
                                    dic.Add(keyValue.Key, keyValue.Value);
                                }
                            }

                            StringBuilder sb = new StringBuilder();
                            if (dic != null)
                            {
                                var i = 0;
                                foreach (KeyValuePair<string, string> keyValue in dic)
                                {
                                    i++;
                                    sb.Append(keyValue.Key);
                                    if (i < dic.Count)
                                        sb.Append(",");
                                }
                                DrMaster["ado15"] = sb.ToString();
                                //WfShowMsg(sb.ToString());
                            }
                            frmActive.Close();
                            BoMaster.OfUpdate(DrMaster.Table);
                            WfCommit();
                            DrMaster.Table.AcceptChanges();
                        }
                        catch (Exception ex)
                        {
                            WfRollback();
                            DrMaster.Table.RejectChanges();
                            throw ex;
                        }
                        finally
                        {
                            if (frmActive != null)
                                frmActive.Close();
                        }
                        break;
                    #endregion

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAppendUpdate 存檔後處理額外的資料表 新增、修改.....
        //檢查登入角色是否有執行該程式的權限
        protected override bool WfAppendUpdate()
        {
            DataTable dtAddTb;
            AdmBLL boAppend;
            StringBuilder sbSql;
            vw_admi600 masterModel = null;
            List<SqlParameter> sqlParmList = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_admi600>();

                boAppend = new AdmBLL(BoMaster.OfGetConntion()); 
                boAppend.TRAN = BoMaster.TRAN;
                boAppend.OfCreateDao("add_tb", "*", "");
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM add_tb");
                sbSql.AppendLine("WHERE add01=@add01");
                sbSql.AppendLine("AND add02=@add02");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@add01", LoginInfo.UserRole));
                sqlParmList.Add(new SqlParameter("@add02", masterModel.ado01));
                dtAddTb = boAppend.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                if (masterModel.ado07.ToUpper() == "M")//menus 時
                {
                    if (dtAddTb != null && dtAddTb.Rows.Count > 0)
                    {
                        dtAddTb.Rows[0].Delete();
                        if (boAppend.OfUpdate(dtAddTb) < 1)
                        {
                            WfShowErrorMsg("刪除權限資料檔(add_tb)失敗!");
                            return false;
                        }
                    }
                }
                else  //報表或程式時
                {
                    if (dtAddTb.Rows.Count == 0 && GlobalFn.isNullRet(masterModel.ado16,"") !="Y")
                    {
                        var result = WfShowConfirmMsg("無此程式的權限,請問是否自動新增 ?");
                        //var i = WfShowConfirmMsg("無此程式的權限,請問是否自動新增");
                        //if (i == 1)
                        if (result == DialogResult.Yes)
                        {
                            var drNew = dtAddTb.NewRow();
                            drNew["add01"] = LoginInfo.UserRole;
                            drNew["add02"] = masterModel.ado01;
                            drNew["add03"] = "Y";
                            drNew["add04"] = "Y";
                            drNew["add05"] = "Y";
                            drNew["add06"] = "Y";
                            drNew["add07"] = "Y";
                            drNew["add08"] = "Y";
                            drNew["add09"] = "Y";
                            drNew["add10"] = "Y";
                            drNew["add11"] = "Y";
                            drNew["add12"] = "Y";
                            dtAddTb.Rows.Add(drNew);

                            if (boAppend.OfUpdate(dtAddTb) < 1)
                            {
                                WfShowErrorMsg("刪除權限資料檔(add_tb)失敗!");
                                return false;
                            }
                        }
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

        #region WfDeleteAppenUpdate 刪除時使用,若需單身資料,要先在查此查詢資料庫並且異動
        /* 刪除時
         * 1.刪除目錄代碼(adm_tb),遞迴考慮
         * 2.權限(add_tb)
         * 3.流程圖(ady_tb)
         * 4.程式畫面(aza_tb) 先不刪,有可能共用
         * 
         */
        protected override bool WfDeleteAppenUpdate(DataRow pDr)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int chkCnts = 0;
            vw_admi600 admi600Model;

            try
            {
                admi600Model = pDr.ToItem<vw_admi600>();
                var sbMsg = new StringBuilder();
                sbMsg.AppendLine("是否同步刪除以下資料?");
                sbMsg.AppendLine("1.目錄代碼(adm_tb)");
                sbMsg.AppendLine("2.權限(add_tb)");
                sbMsg.AppendLine("3.流程圖(ady_tb)");
                var result = WfShowConfirmMsg(sbMsg.ToString(), MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                    return false;
                if (result == DialogResult.No)
                    return true;

                #region 刪除目錄權限--需處理遞迴
                //先處理 對應到adm01的
                var admList = new List<adm_tb>();
                WfGetAdmListRecursive(admi600Model.ado01, admList);

                if (admList != null && admList.Count > 0)
                {
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("DELETE FROM adm_tb");
                    sbSql.AppendLine("WHERE adm01=@adm01");
                    sbSql.AppendLine("  AND adm02=@adm02");
                    foreach (adm_tb admModel in admList)
                    {
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@adm01", admModel.adm01));
                        sqlParmList.Add(new SqlParameter("@adm02", admModel.adm02));

                        BoAdm.OfExecuteNonquery(sbSql.ToString(),sqlParmList.ToArray());
                    }
                }
                //再處理對應到adm02的
                sbSql = new StringBuilder();
                sbSql.AppendLine("DELETE FROM adm_tb");
                sbSql.AppendLine("WHERE adm02=@adm02");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@adm02", admi600Model.ado01));
                BoAdm.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray());
                #endregion

                #region 刪除角色權限
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM add_tb");
                sbSql.AppendLine("WHERE add02=@add02");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@add02", admi600Model.ado01));
                chkCnts = GlobalFn.isNullRet(BoAdm.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (chkCnts > 0)
                {
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("DELETE FROM add_tb");
                    sbSql.AppendLine("WHERE add02=@add02");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@add02", admi600Model.ado01));
                    if (BoAdm.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray()) < 0)
                    {
                        WfShowErrorMsg("刪除權限資料檔(add_tb)失敗!");
                        return false;
                    }
                } 
                #endregion

                #region 刪除流程圖
                sbSql = new StringBuilder();
                sbSql.AppendLine("DELETE FROM ady_tb");
                sbSql.AppendLine("WHERE ady03=@ady03");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ady03",AdoModel.ado01));
                BoAdm.OfExecuteNonquery(sbSql.ToString(),sqlParmList.ToArray());
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        //*****************************表單自訂Fuction****************************************

        #region WfSetAdo07RelReadonly 依程式類別設定相關控制項 readonly
        private void WfSetAdo07RelReadonly(string pAdo07)
        {
            try
            {
                if (pAdo07 == "M" || GlobalFn.varIsNull(pAdo07))   //程式類別---MENU類型時
                {
                    WfSetControlReadonly(new List<Control> { ute_ado03, ute_ado04, ute_ado05, ute_ado06, ucx_ado08, ucx_ado09, ucb_ado12, ucb_ado13 }, true);
                    WfSetControlReadonly(new List<Control> { ute_ado15, ucx_ado16 }, true);
                }
                else if (pAdo07 == "R")
                {
                    WfSetControlReadonly(new List<Control> { ute_ado03, ute_ado04, ute_ado05, ute_ado06, ucx_ado08, ucb_ado12, ucb_ado13 }, false);
                    WfSetControlReadonly(new List<Control> { ute_ado15, ucx_ado16 }, false);
                }
                else
                {
                    WfSetControlReadonly(new List<Control> { ute_ado03, ute_ado04, ute_ado05, ute_ado06, ucx_ado08, ucx_ado09, ucb_ado12, ucb_ado13 }, false);
                    WfSetControlReadonly(new List<Control> { ute_ado15, ucx_ado16 }, false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfGetAdmTb 取得menu設定檔及子目錄
        private void WfGetAdmListRecursive(string pAdm01, List<adm_tb> admList)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;

            sbSql = new StringBuilder();
            sbSql.AppendLine("SELECT * FROM adm_tb");
            sbSql.AppendLine("WHERE adm01=@adm01");
            sqlParmList = new List<SqlParameter>();
            sqlParmList.Add(new SqlParameter("@adm01", pAdm01));
            var dtTemp = BoAdm.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
            if (dtTemp.Rows.Count > 0)
            {
                foreach (DataRow drTemp in dtTemp.Rows)
                {
                    var admModel = drTemp.ToItem<adm_tb>();
                    admList.Add(admModel);
                    WfGetAdmListRecursive(admModel.adm02, admList);
                }
            }
        }
        #endregion
    }
}
