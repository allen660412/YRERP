/* 程式名稱: 程式對應功能設定作業
   系統代號: admi601
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
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.BLL.Model;
using Infragistics.Win.UltraWinToolbars;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi602 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        AdmBLL BoAdm = null;
        #endregion

        #region 建構子
        public FrmAdmi602()
        {
            InitializeComponent();
        }

        public FrmAdmi602(string pSourceForm, YR.ERP.Shared.UserInfo pUserInfo, string pWhere)
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
            this.StrFormID = "admi602";
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
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ado01", SqlDbType.NVarChar) });

                TabMaster.CanCopyMode = false;
                TabMaster.CanAddMode = false;
                TabMaster.CanDeleteMode = false;
                TabMaster.IsReadOnly = true;
                TabMaster.CanUseRowLock = false;//不lock ado_tb
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
            List<SqlParameter> keyParms;
            SqlParameter keyParm;
            // 設定 Detail tab1 資料 : 終端印件
            this.TabDetailList[0].TargetTable = "adq_tb";
            this.TabDetailList[0].ViewTable = "vw_admi602s";
            this.TabDetailList[0].CanAddMode = false;
            this.TabDetailList[0].CanDeleteMode = false;

            keyParms = new List<SqlParameter>();
            keyParm = new SqlParameter("adq01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "ado01";
            keyParms.Add(keyParm);
            this.TabDetailList[0].RelationParams = keyParms;
            return true;
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

        #region WfDisplayMode 新增修改刪除後的readonly顯示
        protected override Boolean WfDisplayMode()
        {
            vw_admi602 masterModel = null;
            try
            {
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else//新增與修改
                {
                    masterModel = DrMaster.ToItem<vw_admi602>();

                    WfSetControlsReadOnlyRecursion(this, true);//先全關
                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "adq03", "adq04" }, false);
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
                SelectNextControl(this.uTab_Detail, true, true, true, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniDetailComboSource 處理grid下拉選單
        protected override void WfIniDetailComboSource(int pTabIndex)
        {
            UltraGrid uGrid;
            UltraGridColumn ugc;
            try
            {
                switch (pTabIndex)
                {
                    case 0:
                        uGrid = TabDetailList[pTabIndex].UGrid;
                        ugc = uGrid.DisplayLayout.Bands[0].Columns["adq04"];    //功能類型 1.action 2.report
                        WfSetGridValueList(ugc, BoAdm.OfGetAdq04KVPList());
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
            int iChkCnts = 0;
            vw_admi602s detailModel = null;
            List<vw_admi602s> detailList = null;
            UltraGrid uGrid;
            try
            {
                #region 單身-vw_admi602s
                if (e.Row.Table.Prefix.ToLower() == "vw_admi602s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_admi602s>();
                    switch (e.Column.ToLower())
                    {
                        case "adq02"://功能代號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            detailList = e.Row.Table.ToList<vw_admi602s>();
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.adq02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("功能代號已存在,請檢核!");
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
            vw_admi602 masterModel = null;
            vw_admi602s detailModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {
                masterModel = DrMaster.ToItem<vw_admi602>();
                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)  //未異動的資料就不檢查了
                        continue;

                    detailModel = drTemp.ToItem<vw_admi602s>();
                    #region adq02-功能代號
                    chkColName = "adq02";
                    if (GlobalFn.varIsNull(detailModel.adq02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region adq03-功能名稱
                    chkColName = "adq03";
                    if (GlobalFn.varIsNull(detailModel.adq03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region adq04-類別
                    chkColName = "adq04";
                    if (GlobalFn.varIsNull(detailModel.adq04))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
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
                            drDetail["adqcreu"] = LoginInfo.UserNo;
                            drDetail["adqcreg"] = LoginInfo.DeptNo;
                            drDetail["adqcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["adqmodu"] = LoginInfo.UserNo;
                            drDetail["adqmodg"] = LoginInfo.DeptNo;
                            drDetail["adqmodd"] = Now;
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

        /********************************ovveride*************************/
        #region WfToolbarModify() : 主表修改 function
        protected override Boolean WfToolbarModify()
        {
            vw_admi602 masterModel;
            StringBuilder sbSql;
            YR.ERP.Base.Forms.FrmBase frmActive = null;
            int chkCnts = 0;
            DataTable dtAdq = null;
            try
            {
                if (base.WfToolbarModify() == true)
                {
                    masterModel = DrMaster.ToItem<vw_admi602>();

                    var adoModle = BoAdm.OfGetAdoModel(masterModel.ado01);
                    if (AdoModel == null)
                    {
                        WfShowErrorMsg("無此程式代號");
                        return false;
                    }

                    var result = WfShowConfirmMsg("是否需要重新擷取功能選單 ?");

                    //if (WfShowConfirmMsg("是否需要重新擷取功能選單") != 1)
                    if (result != DialogResult.Yes)
                        return true;

                    //先檢查是否有transction
                    if (BoMaster.TRAN != null)
                        WfCommit();

                    if (TabMaster.CanUseRowLock == true)
                    {
                        if (WfLockMasterRow() == false)//Lock row 並且產生transaction
                            return false;
                    }
                    else
                    {
                        if (WfBeginTran() == false)
                            return false;
                    }

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

                        frmActive.Close();
                        //以目前的action與擷取的資料來比對,只處理新增與刪除
                        dtAdq = TabDetailList[0].DtSource;
                        //先處理刪除
                        for (int i = dtAdq.Rows.Count - 1; i >= 0; i--)
                        {
                            var adqModel = dtAdq.Rows[i].ToItem<adq_tb>();
                            //先刪掉不在程式中的
                            if (!dic.ContainsKey(adqModel.adq02))
                            {
                                dtAdq.Rows[i].Delete();

                                if (TabDetailList[0].BoBasic.OfUpdate(BoMaster.TRAN, dtAdq.GetChanges()) < 0)
                                {
                                    { throw new Exception("儲存明細時失敗，請檢核 !"); }
                                }
                                dtAdq.AcceptChanges();
                            }

                        }

                        //foreach (DataRow drAdq in dtAdq.Rows)
                        //{
                        //    var adqModel = drAdq.ToItem<adq_tb>();
                        //    //先刪掉不在程式中的
                        //    if (!dic.ContainsKey(adqModel.adq02))
                        //    {
                        //        drAdq.Delete();

                        //        if (TabDetailList[0].BoBasic.OfUpdate(BoMaster.TRAN, dtAdq.GetChanges()) < 0)
                        //        {
                        //            { throw new Exception("儲存明細時失敗，請檢核 !"); }
                        //        }
                        //        dtAdq.AcceptChanges();
                        //    }
                        //}

                        //再處理新增
                        var adqList = dtAdq.ToList<adq_tb>();
                        foreach (KeyValuePair<string, string> keyValue in dic)
                        {
                            chkCnts = adqList.Where(p => p.adq02 == keyValue.Key).Count();
                            if (chkCnts == 0)
                            {
                                var drNew = dtAdq.NewRow();
                                drNew["adq01"] = masterModel.ado01;
                                drNew["adq02"] = keyValue.Key;
                                drNew["adq03"] = keyValue.Value;
                                drNew["adq04"] = "1";   //預設為action
                                dtAdq.Rows.Add(drNew);
                            }
                        }

                        if (dtAdq.GetChanges() != null)
                        {
                            if (TabDetailList[0].BoBasic.OfUpdate(BoMaster.TRAN, dtAdq.GetChanges()) < 0)
                            {
                                { throw new Exception("儲存明細時失敗，請檢核 !"); }
                            }
                        }

                        WfCommit();
                        //dtAdq.AcceptChanges();
                    }
                    catch (Exception ex)
                    {
                        WfRollback();
                        //dtAdq.RejectChanges();
                        throw ex;
                    }
                    finally
                    {
                        if (frmActive != null)
                            frmActive.Close();
                        WfRetrieveDetail();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        #endregion 主表修改 function
    }
}
