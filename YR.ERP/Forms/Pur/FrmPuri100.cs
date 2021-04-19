/* 程式名稱: 廠商基本資料建立
   系統代號: puri100
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


namespace YR.ERP.Forms.Pur
{
    public partial class FrmPuri100 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region property
        PurBLL BoPur = null;
        BasBLL BoBas = null;
        string Bga01 = "", Bgc03 = "", Pca01New = "";          //新增時在存檔前用來重新取號使用
        #endregion

        #region 建構子
        public FrmPuri100()
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
            this.StrFormID = "puri100";
            uTab_Header.Tabs[0].Text = "基本資料";
            uTab_Header.Tabs[1].Text = "狀態";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "廠商資料";
            uTab_Master.Tabs[1].Text = "交易資料";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("pca01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "pcasecu";
                TabMaster.GroupColumn = "pcasecg";
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

            BoPur = new PurBLL(BoMaster.OfGetConntion());
            BoBas = new BasBLL(BoMaster.OfGetConntion());
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
                    BoPur.TRAN = BoMaster.TRAN;
                    BoBas.TRAN = BoMaster.TRAN;
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
                //票據寄領方式
                sourceList = BoPur.OfGetPca20KVPList();
                WfSetUcomboxDataSource(ucb_pca20, sourceList);

                //課稅別
                sourceList = BoPur.OfGetTaxTypeKVPList();
                WfSetUcomboxDataSource(ucb_pca22, sourceList);

                //發票聯數
                sourceList = BoPur.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_pca23, sourceList);

                //採購單發送方式
                sourceList = BoPur.OfGetPca28KVPList();
                WfSetUcomboxDataSource(ucb_pca28, sourceList);
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
            this.TabDetailList[0].TargetTable = "pcb_tb";
            this.TabDetailList[0].ViewTable = "vw_puri100s";
            keyParm = new SqlParameter("pcb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "pca01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_puri100 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_puri100>();
                    WfSetDocPicture(masterModel.pcavali, masterModel.pcaconf, "", pbxDoc);
                }
                else
                {
                    WfSetDocPicture("", "", "", pbxDoc);
                }

                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false);        //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_pcacreu, ute_pcacreg, udt_pcacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_pcamodu, ute_pcamodg, udt_pcamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_pcasecu, ute_pcasecg }, true);

                    WfSetControlReadonly(new List<Control> { ute_pca05_c, ute_pca21_c, ute_pca24_c, ucb_pca30, ucb_pca31 }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_pca01, true);
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

        #region WfSetDetailDisplayMode 新增修改時的明細列可輸入處理,需要每筆資料列微調整時再使用
        protected override void WfSetDetailDisplayMode(int pCurTabDetail, UltraGridRow pUgr, DataRow pDr)
        {
            string columnName;
            try
            {
                switch (pCurTabDetail)
                {
                    case 0:
                        foreach (UltraGridCell ugc in pUgr.Cells)
                        {
                            columnName = ugc.Column.Key.ToLower();
                            //先控可以輸入的
                            if (
                                columnName == "pcb03" ||
                                columnName == "pcb04" ||
                                columnName == "pcb05" ||
                                columnName == "pcb06" ||
                                columnName == "pcb07" ||
                                columnName == "pcb08"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "pcb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }
                            WfSetControlReadonly(ugc, true);
                        }
                        break;
                }
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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            string bga01 = "", bgc03 = "";
            string pca01New;
            int iChkCnts = 0;
            string errMsg;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_puri100
                if (pDr.Table.Prefix.ToLower() == "vw_puri100")
                {
                    switch (pColName.ToLower())
                    {
                        case "pca01":
                            #region pca01 廠商編號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bga03", "2"));
                            WfShowPickUtility("p_bga1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                {
                                    bga01 = GlobalFn.isNullRet(messageModel.DataRowList[0]["bga01"], "");
                                    //檢查是否有使用分類編碼
                                    sbSql = new StringBuilder();
                                    sbSql.AppendLine("SELECT COUNT(1) FROM bgb_tb");
                                    sbSql.AppendLine("WHERE bgb01=@bgb01 AND bgb05='3'");
                                    sqlParmList = new List<SqlParameter>();
                                    sqlParmList.Add(new SqlParameter("@bgb01", bga01));
                                    iChkCnts = GlobalFn.isNullRet(BoMaster.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                                    if (iChkCnts > 0)
                                    {
                                        messageModel = new MessageInfo();
                                        messageModel.IsAutoQuery = true;
                                        messageModel.ParamSearchList = new List<SqlParameter>();
                                        messageModel.ParamSearchList.Add(new SqlParameter("@bgc01", bga01));

                                        WfShowPickUtility("p_bgc1", messageModel);
                                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                        {
                                            if (messageModel.DataRowList.Count > 0)
                                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bgc03"], "");
                                            else
                                                pDr[pColName] = "";
                                        }
                                        else
                                            break;
                                        //if (messageModel != null && messageModel.DataRowList.Count > 0)
                                        //{
                                        //    bgc03 = GlobalFn.isNullRet(messageModel.DataRowList[0]["bgc03"], "");
                                        //}
                                        //else
                                        //    break;
                                    }
                                    if (BoBas.OfGetBga01AutoNo(bga01, bgc03, out pca01New, out errMsg) == false)
                                    {
                                        WfShowErrorMsg(errMsg);
                                        break;
                                    }
                                    Pca01New = pca01New;
                                    Bga01 = bga01;
                                    Bgc03 = bgc03;
                                    pDr["pca01"] = pca01New;
                                }
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                            #endregion

                        case "pca04"://廠商分類
                            WfShowPickUtility("p_pba", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pba01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["pba01"];
                            //}
                            break;

                        case "pca05"://負責業務人員
                            WfShowPickUtility("p_bec", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                            //}
                            break;

                        case "pca21"://付款方式
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bef01", "1"));
                            WfShowPickUtility("p_bef1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bef02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["bef02"];
                            //}
                            break;

                        case "pca24"://取價原則
                            WfShowPickUtility("p_pbb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pbb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["pbb01"];
                            //}
                            break;

                        case "pca26"://金融機構
                            WfShowPickUtility("p_beg", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beg01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["beg01"];
                            //}
                            break;

                        case "pca35"://送貨地址
                            messageModel.IsAutoQuery = true;
                            WfShowPickUtility("p_pcc1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pcc01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["pcc01"];
                            //}
                            break;

                        case "pca36"://帳單地址
                            messageModel.IsAutoQuery = true;
                            WfShowPickUtility("p_pcc2", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pcc01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["pcc01"];
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["pcaconf"] = "N";
                pDr["pcavali"] = "";
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr) 設定明細資料列的初始值
        protected override bool WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr)
        {
            try
            {
                switch (pDr.Table.Prefix.ToLower())
                {
                    case "vw_puri100s":
                        //pDr["pcb02"] = WfGetMaxSeq(pDr.Table, "pcb02");//不預帶,由使用者自行輸入
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

        #region WfPreInInsertModeCheck 進新增模式前的檢查及清變數
        protected override bool WfPreInInsertModeCheck()
        {
            try
            {
                Pca01New = "";
                Bga01 = "";
                Bgc03 = "";
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
            List<ButtonTool> buttonList = new List<ButtonTool>();
            ButtonTool bt;
            try
            {
                bt = new ButtonTool("Confirm");
                bt.SharedProps.Caption = "確認";
                bt.SharedProps.Category = "Confirm";
                buttonList.Add(bt);

                bt = new ButtonTool("CancelConfirm");
                bt.SharedProps.Caption = "取消確認";
                bt.SharedProps.Category = "CancelConfirm";
                buttonList.Add(bt);

                bt = new ButtonTool("Invalid");
                bt.SharedProps.Caption = "作廢";
                bt.SharedProps.Category = "Invalid";
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
            vw_puri100 masterModel;
            try
            {
                masterModel = DrMaster.ToItem<vw_puri100>();
                switch (pActionName)
                {
                    case "Confirm":
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfConfirm();
                        break;
                    case "CancelConfirm":
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfCancelConfirm();
                        break;
                    case "Invalid":
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfInvalid();
                        break;
                }
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
            List<vw_puri100s> detailList;
            vw_puri100s detailModel;
            int iChkCnts = 0;
            try
            {
                #region 單頭-pick vw_puri100
                if (e.Row.Table.Prefix.ToLower() == "vw_puri100")
                {
                    switch (e.Column.ToLower())
                    {

                        case "pca01"://廠商編號
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoPur.OfChkPcaPKExists(e.Value.ToString()) == true)
                            {
                                WfShowErrorMsg("廠商編號已存存,請檢核!");
                                return false;
                            }
                            break;

                        case "pca04"://廠商分類
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoPur.OfChkPbaPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此廠商分類,請檢核!");
                                return false;
                            }
                            break;

                        case "pca05"://負責採購
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["pca05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此負責採購,請檢核!");
                                return false;
                            }
                            e.Row["pca05_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pca21"://付款方式
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["pca21_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("1", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此付款方式,請檢核!");
                                return false;
                            }
                            e.Row["pca21_c"] = BoBas.OfGetBef03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pca24"://取價原則
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["pca24_c"] = "";
                                return true;
                            }
                            if (BoPur.OfChkPbbPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此取價原則,請檢核!");
                                return false;
                            }
                            e.Row["pca24_c"] = BoPur.OfGetPbb02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pca26"://金融機構
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBegPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此金融機構,請檢核!");
                                return false;
                            }
                            break;

                        case "pca35"://送貨地址
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoPur.OfChkPccPKValid(GlobalFn.isNullRet(e.Value, ""), "1") == false)
                            {
                                WfShowErrorMsg("無此送貨地址,請檢核!");
                                return false;
                            }
                            break;

                        case "pca36"://帳單地址
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoPur.OfChkPccPKValid(GlobalFn.isNullRet(e.Value, ""), "2") == false)
                            {
                                WfShowErrorMsg("無此帳單地址,請檢核!");
                                return false;
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_puri100
                if (e.Row.Table.Prefix.ToLower() == "vw_puri100s")
                {
                    detailModel = e.Row.ToItem<vw_puri100s>();
                    detailList = e.Row.Table.ToList<vw_puri100s>();
                    switch (e.Column.ToLower())
                    {
                        case "pcb02":
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            iChkCnts = detailList.Where(x => x.pcb02 == detailModel.pcb02).Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("地址碼已存在!");
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
            vw_puri100 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;

            try
            {
                masterModel = DrMaster.ToItem<vw_puri100>();
                #region 單頭資料檢查
                chkColName = "pca01";//廠商編號
                chkControl = ute_pca01;
                if (GlobalFn.varIsNull(masterModel.pca01))
                {
                    this.uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pca02";//廠商全名
                chkControl = ute_pca02;
                if (GlobalFn.varIsNull(masterModel.pca02))
                {
                    this.uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pca03";//廠商簡稱
                chkControl = ute_pca03;
                if (GlobalFn.varIsNull(masterModel.pca03))
                {
                    this.uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pca20";//票據寄領
                chkControl = ucb_pca20;
                if (GlobalFn.varIsNull(masterModel.pca20))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pca21";//付款方式
                chkControl = ute_pca21;
                if (GlobalFn.varIsNull(masterModel.pca21))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pca22";//課稅別
                chkControl = ucb_pca22;
                if (GlobalFn.varIsNull(masterModel.pca22))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pca23";//發票聯數
                chkControl = ucb_pca23;
                if (GlobalFn.varIsNull(masterModel.pca23))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pca24";//取價原則
                chkControl = ute_pca24;
                if (GlobalFn.varIsNull(masterModel.pca24))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
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
            string pca01New, errMsg;
            try
            {
                #region 廠商編號 避免搶號,重新取號
                if (FormEditMode == YREditType.新增 && Pca01New == GlobalFn.isNullRet(DrMaster["pca01"], ""))     //避免搶號,重新取號
                {
                    if (BoBas.OfGetBga01AutoNo(Bga01, Bgc03, out pca01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["pca01"] = pca01New;
                }
                #endregion

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["pcasecu"] = LoginInfo.UserNo;
                        DrMaster["pcasecg"] = LoginInfo.GroupNo;
                        DrMaster["pcacreu"] = LoginInfo.UserNo;
                        DrMaster["pcacreg"] = LoginInfo.DeptNo;
                        DrMaster["pcacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["pcamodu"] = LoginInfo.UserNo;
                        DrMaster["pcamodg"] = LoginInfo.DeptNo;
                        DrMaster["pcamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["pcbcreu"] = LoginInfo.UserNo;
                            drDetail["pcbcreg"] = LoginInfo.DeptNo;
                            drDetail["pcbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["pcbmodu"] = LoginInfo.UserNo;
                            drDetail["pcbmodg"] = LoginInfo.DeptNo;
                            drDetail["pcbmodd"] = Now;
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

        //*****************************表單自訂Fuction****************************************
        #region WfConfirm 確認
        private void WfConfirm()
        {
            vw_puri100 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)       
                    return;

                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_puri100>();

                if (masterModel.pcavali == "N")
                {
                    WfShowErrorMsg("廠商已失效!");
                    WfRollback();
                    return;
                }

                if (GlobalFn.isNullRet(masterModel.pcaconf,"N") != "N")
                {
                    WfShowErrorMsg("廠商非未確認狀態!");
                    WfRollback();
                    return;
                }

                DrMaster["pcaconf"] = "Y";
                DrMaster["pcavali"] = "Y";
                DrMaster["pcamodu"] = LoginInfo.UserNo;
                DrMaster["pcamodg"] = LoginInfo.DeptNo;
                DrMaster["pcamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_puri100>();
                WfSetDocPicture(masterModel.pcavali, masterModel.pcaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfCancelConfirm 取消確認
        private void WfCancelConfirm()
        {
            vw_puri100 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)        
                    return;

                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;

                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_puri100>();

                if (masterModel.pcavali == "N")
                {
                    WfShowErrorMsg("廠商已失效!");
                    WfRollback();
                    return;
                }

                if (masterModel.pcaconf != "Y")
                {
                    WfShowErrorMsg("廠商非已確認狀態!");
                    WfRollback();
                    return;
                }

                DrMaster["pcaconf"] = "N";
                DrMaster["pcamodu"] = LoginInfo.UserNo;
                DrMaster["pcamodg"] = LoginInfo.DeptNo;
                DrMaster["pcamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_puri100>();
                WfSetDocPicture(masterModel.pcavali, masterModel.pcaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfInvalid 作廢/作廢還原
        private void WfInvalid()
        {
            vw_puri100 masterModel = null;
            string msg;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)        //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_puri100>();


                if (masterModel.pcavali == "Y")
                    msg = "是否要作廢廠商?";
                else
                    msg = "是否要作廢還原廠商?";

                var result = WfShowConfirmMsg(msg);

                //if (WfShowConfirmMsg(msg) != 1)
                if (result != DialogResult.Yes)
                    return;


                if (masterModel.pcavali == "Y" || masterModel.pcavali == "W")//走作廢
                {
                    DrMaster["pcavali"] = "N";
                }
                else
                {
                    DrMaster["pcavali"] = "Y";
                }
                DrMaster["pcamodu"] = LoginInfo.UserNo;
                DrMaster["pcamodg"] = LoginInfo.DeptNo;
                DrMaster["pcamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_puri100>();
                WfSetDocPicture(masterModel.pcavali, masterModel.pcaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

    }
}
