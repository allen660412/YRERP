/* 程式名稱: 程式對應報表設定作業
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

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi601 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        AdmBLL BoAdm = null;
        #endregion

        #region 建構子
        public FrmAdmi601()
        {
            InitializeComponent();
        }

        public FrmAdmi601(string pSourceForm, YR.ERP.Shared.UserInfo pUserInfo, string pWhere)
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
            // 繼承的表單要覆寫, 更改參數值            
            /*
            this.strFormID = "XXX";
            this.isDirectEdit = false;
            this.isMultiRowEdit = false;
             */
            this.StrFormID = "admi601";
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
            //tabMaster.strBLLClassName = "YR.ERP.BLL.MSSQL.CommonBLL";
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);

            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
            return;
        }
        #endregion

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                //程式類別
                sourceList = (BoMaster as AdmBLL).OfGetAdo07KVPList();
                WfSetUcomboxDataSource(ucb_ado07, sourceList);

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

        #region WfDisplayMode 新增修改刪除後的readonly顯示
        protected override Boolean WfDisplayMode()
        {
            vw_admi601 masterModel = null;
            try
            {
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else//新增與修改
                {
                    masterModel = DrMaster.ToItem<vw_admi601>();

                    WfSetControlsReadOnlyRecursion(this, true);//先全關
                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "adp02", "adp03", "adp04" }, false);
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
                if (FormEditMode == YREditType.查詢)
                {
                    uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    SelectNextControl(this.uTab_Master, true, true, true, false);
                }
                else
                {
                    uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    SelectNextControl(this.uTab_Detail, true, true, true, false);
                    WfSetFirstVisibelCellFocus(TabDetailList[0].UGrid);
                }
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
            this.TabDetailList[0].TargetTable = "adp_tb";
            this.TabDetailList[0].ViewTable = "vw_admi601s";

            keyParms = new List<SqlParameter>();
            keyParm = new SqlParameter("adp01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "ado01";
            keyParms.Add(keyParm);
            this.TabDetailList[0].RelationParams = keyParms;
            return true;
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
                        //uGrid = TabDetailList[pTabIndex].UGrid;
                        //ugc = uGrid.DisplayLayout.Bands[0].Columns["aza12"];//元件類型
                        //WfSetGridValueList(ugc, BoAdm.OfGetAza12KVPList());
                        break;
                }
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
                switch (pCurTabDetail)
                {
                    case 0:
                        pDr["adp03"] = WfGetMaxSeq(pDr.Table, "adp03");
                        pDr["adp04"] = 2; ;  //預設2公分
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
            vw_admi601 admi601Model = null;
            vw_admi601s admi601sModel = null;
            UltraGrid uGrid;
            try
            {
                MessageInfo messageModel = new MessageInfo();
                #region 單身-pick vw_admi650s
                if (pDr.Table.Prefix.ToLower() == "vw_admi601s")
                {
                    admi601Model = DrMaster.ToItem<vw_admi601>();
                    admi601sModel = pDr.ToItem<vw_admi601s>();

                    uGrid = sender as UltraGrid;
                    switch (pColName.ToLower())
                    {
                        case "adp02":   //報表欄位
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IntMaxRow = -1;
                            messageModel.ParamSearchList.Add(new SqlParameter("@aza01", admi601Model.ado06));
                            WfShowPickUtility("p_aza1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count == 1)
                                    pDr[pColName] = "";
                                else if (messageModel.DataRowList.Count == 1)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["aza03"], "");
                                else
                                {
                                    var drReturns = messageModel.DataRowList;
                                    for (int i = 0; i < drReturns.Count; i++)
                                    {
                                        if (i == 0)
                                            pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["aza03"], "");
                                        else
                                        {
                                            var drInsert = TabDetailList[0].DtSource.NewRow();
                                            WfSetDetailRowDefault(0, drInsert);
                                            drInsert[pColName] = GlobalFn.isNullRet(drReturns[i]["aza03"], "");
                                            TabDetailList[0].DtSource.Rows.Add(drInsert);
                                        }
                                    }
                                }
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
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            vw_admi601 admi601Model = null;
            vw_admi601s admi601sModel = null;
            List<vw_admi601s> admi601List = null;
            aza_tb l_aza;
            int iChkCnts = 0;
            try
            {
                if (e.Row.Table.Prefix.ToLower() == "vw_admi601s")
                {
                    admi601Model = DrMaster.ToItem<vw_admi601>();
                    admi601List = e.Row.Table.ToList<vw_admi601s>();
                    admi601sModel = e.Row.ToItem<vw_admi601s>();
                    switch (e.Column.ToLower())
                    {
                        case "adp02":
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["aza04"] = "";
                                e.Row["aza08"] = "";
                                e.Row["aza09"] = DBNull.Value;
                                e.Row["aza10"] = DBNull.Value;
                                e.Row["aza11"] = DBNull.Value;
                                return true;
                            }
                            iChkCnts = admi601List.Where(p => p.adp02 == e.Value.ToString()).Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("不可重覆輸入報表欄位!");
                                return false;
                            }

                            if (BoAdm.OfChkAzaPKExists(admi601Model.ado06, e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此欄位代碼,請檢核!");
                                return false;
                            }
                            l_aza = BoAdm.OfGetAzaModel(admi601Model.ado06, e.Value.ToString());
                            e.Row["aza04"] = l_aza.aza04;
                            e.Row["aza08"] = l_aza.aza08;
                            e.Row["aza09"] = l_aza.aza09;
                            e.Row["aza10"] = l_aza.aza10;
                            e.Row["aza11"] = l_aza.aza11;
                            break;
                        case "adp03":   //排列順序
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("輸入欄位不可空白!");
                                return false;
                            }
                            break;

                        case "adp04":   //報表寬度
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("輸入欄位不可空白!");
                                return false;
                            }
                            break;
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_admi601s detailModel = null;
            UltraGrid uGrid;
            string msg;
            string chkColName;
            int iChkDetailTab;
            try
            {
                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)  //未異動的資料就不檢查了
                        continue;

                    detailModel = drTemp.ToItem<vw_admi601s>();
                    #region adp02-報表欄位
                    chkColName = "adp02";
                    if (GlobalFn.varIsNull(detailModel.adp02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region adp04-報表寬度
                    chkColName = "adp04";
                    if (GlobalFn.varIsNull(detailModel.adp04))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region adp03-排列順序
                    chkColName = "adp03";
                    if (GlobalFn.varIsNull(detailModel.adp03))
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
                            drDetail["adpcreu"] = LoginInfo.UserNo;
                            drDetail["adpcreg"] = LoginInfo.DeptNo;
                            drDetail["adpcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["adpmodu"] = LoginInfo.UserNo;
                            drDetail["adpmodg"] = LoginInfo.DeptNo;
                            drDetail["adpmodd"] = Now;
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
