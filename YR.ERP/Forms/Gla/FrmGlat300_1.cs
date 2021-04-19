/* 程式名稱: 沖帳維護子程式
   系統代號: glat300_1
   作　　者: Allen
   描　　述: 

   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.DAL.YRModel;
using YR.ERP.Shared;
using YR.Util;
using Infragistics.Win;

namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlat300_1 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        GlaBLL BoGla = null;
        vw_glat300_1 _glat300_1Model = null;
        DataTable _dtGlat300a = null;
        #endregion

        #region 建構子
        public FrmGlat300_1()
        {
            InitializeComponent();
        }

        public FrmGlat300_1(YR.ERP.Shared.UserInfo pUserInfo, vw_glat300s pGlat300Model, DataTable pDtGlat300a)
        {
            InitializeComponent();
            _glat300_1Model = WfModelMapping(pGlat300Model);
            _dtGlat300a = pDtGlat300a;
            LoginInfo = pUserInfo;

            this.Shown += FrmGalt300_1_Shown;
        }

        #endregion

        #region FrmGalt300_1_Shown
        void FrmGalt300_1_Shown(object sender, EventArgs e)
        {
            //資料入後 模擬 update
            WfLoadInitialData();
        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            this.StrFormID = "glat300_1";

            this.IntTabCount = 1;
            this.IntMasterGridPos = 0;
            uTab_Master.Tabs[0].Text = "基本資料";
            //uTab_Master.Tabs[1].Text = "test";

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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("gfb01", SqlDbType.NVarChar) });

                TabMaster.CanCopyMode = false;
                TabMaster.CanAddMode = false;
                TabMaster.CanUseRowLock = false;

                TabMaster.IsReadOnly = true;
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
            BoGla = new GlaBLL(BoMaster.OfGetConntion());
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
                    BoGla.TRAN = BoMaster.TRAN;
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
                ////單據確認
                //sourceList = BoStp.OfGetSgaconfKVPList();
                //WfSetUcomboxDataSource(ucb_sgaconf, sourceList);
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

            this.TabDetailList[0].TargetTable = "gfg_tb";
            this.TabDetailList[0].ViewTable = "vw_glat300_1s";
            keyParm = new SqlParameter("gfg03", SqlDbType.NVarChar);
            keyParm.SourceColumn = "gfb03";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            TabDetailList[0].IsReadOnly = true;

            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            try
            {
                Infragistics.Win.UltraWinToolbars.RibbonTab rbt;
                rbt = UtbmMain.Ribbon.Tabs[0];
                rbt.Groups["RibgDetail"].Visible = false;   //關閉明細新增刪除,功能

                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false); //先全開
                    WfSetControlsReadOnlyRecursion(ute_gfb01.Parent, true);
                    WfSetControlReadonly(ute_sum_gfh09, true);
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
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
            vw_glat300_1s glat300_1 = null;
            try
            {
                switch (pCurTabDetail)
                {
                    case 0:
                        glat300_1 = pDr.ToItem<vw_glat300_1s>();
                        foreach (UltraGridCell ugc in pUgr.Cells)
                        {
                            columnName = ugc.Column.Key.ToLower();
                            //先控可以輸入的
                            if (
                                columnName == "is_pick"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "gfh09"
                                )
                            {
                                if (glat300_1.is_pick == "Y")
                                    WfSetControlReadonly(ugc, false);
                                else
                                    WfSetControlReadonly(ugc, true);
                                continue;
                            }

                            WfSetControlReadonly(ugc, true);    //其餘的都關閉
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

        #region WfSeDetailGridLayout
        protected override void WfSeDetailGridLayout(UltraGrid pUgrid)
        {
            Infragistics.Win.UltraWinGrid.UltraGridColumn lugc;
            try
            {
                if (pUgrid.DisplayLayout.Bands[0].Columns.Exists("is_pick"))
                {
                    lugc = pUgrid.DisplayLayout.Bands[0].Columns["is_pick"];
                    WfSetUgridCheckBox(lugc);
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
            vw_glat300_1 masterModel = null;
            vw_glat300_1s detailModel = null;
            List<vw_glat300_1s> detailList = null;
            gba_tb gbaModel = null;
            UltraGrid uGrid = null;
            UltraGridRow uGridRow = null;
            int ChkCnts = 0;
            string sql = "";
            List<SqlParameter> sqlParmList = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_glat300_1>();
                #region 單身-vw_glat300_1s
                if (e.Row.Table.Prefix.ToLower() == "vw_glat300_1s")
                {
                    uGrid = sender as UltraGrid;
                    uGridRow = uGrid.ActiveRow;
                    detailModel = e.Row.ToItem<vw_glat300_1s>();
                    detailList = e.Row.Table.ToList<vw_glat300_1s>();
                    var bb = TabDetailList[0].DtSource.ToList<vw_glat300_1s>();
                    switch (e.Column.ToLower())
                    {
                        case "is_pick"://項次
                            e.Row["gfh09"] = 0;
                            if (e.Value.ToString() == "Y")
                            {
                                WfSetControlReadonly(uGridRow.Cells["gfh09"], false);
                            }
                            else
                            {
                                WfSetControlReadonly(uGridRow.Cells["gfh09"], true);
                                WfSetSumGfb09();
                            }
                            break;
                        case "gfh09":   //沖帳金額
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入數字!");
                                return false;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) < 0)
                            {
                                WfShowErrorMsg("金額不可小於0!");
                                return false;
                            }
                            var sumGfh09 = detailList.Where(p => p.is_pick == "Y" && p.gfh09 >= 0)
                                                   .Sum(p => p.gfh09);
                            if (masterModel.gfb07 < sumGfh09)
                            {
                                var bekModel = BoBas.OfGetBekModel(BaaModel.baa04);
                                WfShowErrorMsg(string.Format("輸入金額加總已超過{0},請檢核!",
                                                GlobalFn.Round(masterModel.gfb07, bekModel.bek03).ToString())
                                    );
                                return false;
                            }
                            //檢查 已沖+暫沖(DB)+輸入金額 是否有超過立帳金額
                            var availableMaxAmt = 0m;    //可輸入最高金額
                            var glat300aList = _dtGlat300a.ToList<vw_glat300a>();
                            //可輸入金額=資料庫(立帳金額-暫沖)
                            availableMaxAmt = detailModel.gfg07 - detailModel.gfg08;
                            //可輸人金額 再減掉此傳票但不屬於該項次的金額加總
                            availableMaxAmt -= glat300aList.Where(p => p.gfh01 == detailModel.gfg01 && p.gfh02 == detailModel.gfg02
                                                                    && p.gfh04 != masterModel.gfb02)
                                                            .Sum(p => p.gfh09)
                                                            ;
                            //可輸入金額 再減掉資料庫中不屬於該傳票未過帳的金額加總
                            if (!GlobalFn.varIsNull(masterModel.gfb01))
                            {
                                sql = @"SELECT SUM(gfh09) FROM gfh_tb
                                        WHERE gfhconf='N' AND gfh03<>@gfh03
                                          AND gfh01=@gfh01 AND gfh02=@gfh02
                                    ";
                                sqlParmList = new List<SqlParameter>();
                                sqlParmList.Add(new SqlParameter("@gfh01", detailModel.gfg01));
                                sqlParmList.Add(new SqlParameter("@gfh02", detailModel.gfg02));
                                sqlParmList.Add(new SqlParameter("@gfh03", masterModel.gfb01));
                                var sumTempGfh09 = GlobalFn.isNullRet(BoGla.OfGetFieldValue(sql, sqlParmList.ToArray()), 0m);
                                availableMaxAmt -= sumTempGfh09;
                            }

                            if (Convert.ToDecimal(e.Value) > availableMaxAmt)
                            {
                                var bekModel = BoBas.OfGetBekModel(BaaModel.baa04);
                                WfShowErrorMsg(string.Format("可輸入最大沖帳金額為{0} !",
                                                GlobalFn.Round(availableMaxAmt, bekModel.bek03).ToString())
                                            );
                                return false;
                            }
                            DrMaster["sum_gfh09"] = sumGfh09;
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
            vw_glat300_1 masterModel = null;
            vw_glat300_1s detailModel = null;
            List<vw_glat300_1s> detailList = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {

                masterModel = DrMaster.ToItem<vw_glat300_1>();
                detailList = TabDetailList[0].DtSource.ToList<vw_glat300_1s>();
                #region 單頭資料檢查
                chkColName = "gfb07";       //本幣金額
                chkControl = ute_gfb07;
                var sumDetailGfb07 = detailList.Sum(p => p.gfh09);
                if (masterModel.gfb07 != sumDetailGfb07)
                {
                    msg = "沖帳金額加總不相同,請檢核!";
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

        //*****************************非正常ovveride*************************************
        
        #region WfPressToolBarSave 按下存檔時
        //將明細資料回寫至 glat300的vw_glat300a
        protected override bool WfPressToolBarSave()
        {
            vw_glat300_1 masterModel = null;
            string sqlWhere = "";
            try
            {
                var pickList = (TabDetailList[0].UGrid.DataSource as DataTable).ToList<vw_glat300_1s>();
                WfFireControlValidated(this.ActiveControl);
                //各驗證控製項會將 isItemchkValid設為true
                if (IsItemchkValid == false)
                    return true;

                if (WfToolbarSave() == false)
                    return false;

                pickList = TabDetailList[0].DtSource.ToList<vw_glat300_1s>();
                //if (WfDisplayMode() == false)
                //    return false;

                //先刪掉同項次的資料再補入
                masterModel = DrMaster.ToItem<vw_glat300_1>();
                sqlWhere = string.Format("gfh04={0}", Convert.ToInt32(masterModel.gfb02).ToString());
                var deleteRows = _dtGlat300a.Select(sqlWhere);
                if (deleteRows != null)
                {
                    for (int i = deleteRows.Count() - 1; i >= 0; i--)
                    {
                        _dtGlat300a.Rows.Remove(deleteRows[i]);
                    }
                }

                pickList = TabDetailList[0].DtSource.ToList<vw_glat300_1s>();

                var pickRows = TabDetailList[0].DtSource.Select("is_pick='Y'");
                if (pickRows != null)
                {
                    foreach (DataRow drGlat300_1s in pickRows)
                    {
                        var glat300_1sModel = drGlat300_1s.ToItem<vw_glat300_1s>();
                        var drNew = _dtGlat300a.NewRow();
                        drNew["gfh01"] = glat300_1sModel.gfg01;     //立帳傳票編號
                        drNew["gfh02"] = glat300_1sModel.gfg02;     //立帳傳票項次
                        drNew["gfh03"] = masterModel.gfb01;         //沖帳傳票編號
                        drNew["gfh04"] = masterModel.gfb02;         //沖帳傳票項次
                        drNew["gfh05"] = DBNull.Value;              //沖帳傳票日期--之後回寫
                        drNew["gfh06"] = masterModel.gfb03;         //會計科目
                        drNew["gfh07"] = DBNull.Value;              //摘要       --再確認要放那裡的
                        drNew["gfh08"] = masterModel.gfb05;         //部門
                        drNew["gfh09"] = glat300_1sModel.gfh09;     //沖帳金額
                        drNew["gfh10"] = DBNull.Value;
                        drNew["gfh11"] = DBNull.Value;
                        drNew["gfh12"] = DBNull.Value;
                        drNew["gfh13"] = DBNull.Value;
                        drNew["gfh14"] = DBNull.Value;
                        drNew["gfh15"] = DBNull.Value;
                        drNew["gfhconf"] = "N";
                        drNew["gfhcomp"] = LoginInfo.CompNo;

                        _dtGlat300a.Rows.Add(drNew);
                    }

                }
                this.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPressSaveCancel 存檔取消時
        protected override bool WfPressToolBarSaveCancel()
        {
            try
            {
                //if (this.WfRowChangeCheck(1) == true)
                //{
                //    IsInSaveCancle = true;
                //    WfSaveCancel();
                //    if (WfRetrieveDetail() == false)
                //        return false;
                //}

                //if (WfDisplayMode() == false)
                //    return false;
                
                this.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreInUpdateMode
        protected override bool WfPreInUpdateMode()
        {
            try
            {
                //進修改模式時,要重新查詢master資料,避免資料dirty
                //if (WfRetrieveMaster() == false)
                //    return false;

                if (TabMaster.CanUseRowLock == true)
                    if (WfLockMasterRow() == false)
                        return false;

                WfSetBllTransaction();

                if (WfRetrieveDetail() == false)
                    return false;

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

        #region WfLoadData
        private void WfLoadInitialData()
        {
            vw_glat300_1 glat300_1Model = null;
            List<vw_glat300a> glat300aList = null;
            List<SqlParameter> sqlParmList = null;
            int chkCnts = 0;
            string sql = "";
            //DataTable dtPickSource = null;
            try
            {

                //資料入後 模擬 update
                var drMaster = TabMaster.DtSource.NewRow();
                WfSetMasterRow(drMaster, _glat300_1Model);
                TabMaster.DtSource.Rows.Add(drMaster);
                WfSetActiveRowToNewRow(uGridMaster, 0);
                ToolClickEventArgs toolClickEventArg = new ToolClickEventArgs(UtbmMain.Tools["btupdate"], null);
                UtbmMain_ToolClick(UtbmMain, toolClickEventArg);    //這裡會觸發 reterivedetail--但需考量前端來源,所以要再更新一次
                glat300_1Model = DrMaster.ToItem<vw_glat300_1>();
                glat300aList = _dtGlat300a.ToList<vw_glat300a>();
                //勾選已載入資料
                if (glat300aList != null && glat300aList.Count > 0)
                {
                    if (TabDetailList[0].DtSource != null && TabDetailList[0].DtSource.Rows.Count > 0)
                    {
                        foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                        {
                            var glat300_1sModel = drDetail.ToItem<vw_glat300_1s>();
                            var glat300aModel = glat300aList.Where(p => p.gfh01 == glat300_1sModel.gfg01 && p.gfh02 == glat300_1sModel.gfg02
                                                                    && p.gfh04 == glat300_1Model.gfb02)
                                                          .FirstOrDefault();
                            if (glat300aModel != null && GlobalFn.isNullRet(glat300aModel.gfh01, "") != "")
                            {
                                drDetail["is_pick"] = "Y";
                                drDetail["gfh09"] = glat300aModel.gfh09;
                            }
                        }
                        WfSetSumGfb09();
                        if (TabDetailList[0].UGrid.ActiveRow!=null)
                        {
                        var activeDataRow=WfGetUgridDatarow(TabDetailList[0].UGrid.ActiveRow);
                        WfSetDetailDisplayMode(0, TabDetailList[0].UGrid.ActiveRow, activeDataRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region WfModelMapping
        private vw_glat300_1 WfModelMapping(vw_glat300s pGlat300sSouce)
        {
            vw_glat300_1 glat300_1Target = new vw_glat300_1();
            glat300_1Target.gfb01 = pGlat300sSouce.gfb01;
            glat300_1Target.gfb02 = pGlat300sSouce.gfb02;
            glat300_1Target.gfb03 = pGlat300sSouce.gfb03;
            glat300_1Target.gfb04 = pGlat300sSouce.gfb04;
            glat300_1Target.gfb05 = pGlat300sSouce.gfb05;
            glat300_1Target.gfb06 = pGlat300sSouce.gfb06;
            glat300_1Target.gfb07 = pGlat300sSouce.gfb07;
            glat300_1Target.gfb08 = pGlat300sSouce.gfb08;
            glat300_1Target.gfb09 = pGlat300sSouce.gfb09;
            glat300_1Target.gfb10 = pGlat300sSouce.gfb10;
            glat300_1Target.gfb11 = pGlat300sSouce.gfb11;
            glat300_1Target.gfb12 = pGlat300sSouce.gfb12;
            glat300_1Target.gfb13 = pGlat300sSouce.gfb13;
            glat300_1Target.gfb14 = pGlat300sSouce.gfb14;
            glat300_1Target.gfb15 = pGlat300sSouce.gfb15;
            glat300_1Target.gfb16 = pGlat300sSouce.gfb16;
            glat300_1Target.gfb17 = pGlat300sSouce.gfb17;
            glat300_1Target.gfb18 = pGlat300sSouce.gfb18;
            glat300_1Target.gfb19 = pGlat300sSouce.gfb19;
            glat300_1Target.gfb20 = pGlat300sSouce.gfb20;

            return glat300_1Target;
        }
        #endregion

        #region WfSetMasterRow
        private void WfSetMasterRow(DataRow pdr, vw_glat300_1 pGlat300_1)
        {
            pdr["gfb01"] = pGlat300_1.gfb01;
            pdr["gfb02"] = pGlat300_1.gfb02;
            pdr["gfb03"] = pGlat300_1.gfb03;
            pdr["gfb04"] = pGlat300_1.gfb04;
            pdr["gfb05"] = pGlat300_1.gfb05;
            pdr["gfb06"] = pGlat300_1.gfb06;
            pdr["gfb07"] = pGlat300_1.gfb07;
            pdr["gfb08"] = pGlat300_1.gfb08;
            pdr["gfb09"] = pGlat300_1.gfb09;
            pdr["gfb10"] = pGlat300_1.gfb10;
            pdr["gfb11"] = pGlat300_1.gfb11;
            pdr["gfb12"] = pGlat300_1.gfb12;
            pdr["gfb13"] = pGlat300_1.gfb13;
            pdr["gfb14"] = pGlat300_1.gfb14;
            pdr["gfb15"] = pGlat300_1.gfb15;
            pdr["gfb16"] = pGlat300_1.gfb16;
            pdr["gfb17"] = pGlat300_1.gfb17;
            pdr["gfb18"] = pGlat300_1.gfb18;
            pdr["gfb19"] = pGlat300_1.gfb19;
            pdr["gfb20"] = pGlat300_1.gfb20;
        }
        #endregion

        #region WfSetSumGfb09
        private void WfSetSumGfb09()
        {
            List<vw_glat300_1s> glat300_1sList = null;
            try
            {
                glat300_1sList=TabDetailList[0].DtSource.ToList<vw_glat300_1s>();
                var sumGfh09 = glat300_1sList.Where(p => p.is_pick == "Y")
                                           .Sum(p=>p.gfh09);
                DrMaster["sum_gfh09"] = sumGfh09;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion

    }
}
