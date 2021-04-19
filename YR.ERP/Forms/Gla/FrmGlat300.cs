/* 程式名稱: 一般傳票維護作業
   系統代號: glat300
   作　　者: Allen
   描　　述: 

   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using Infragistics.Win;
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
using YR.ERP.BLL.MSSQL.Gla;
using YR.ERP.DAL.YRModel;
using YR.ERP.Shared;
using YR.Util;

namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlat300 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        GlaBLL BoGla = null;
        AdmBLL BoAdm = null;
        #endregion

        #region 建構子
        public FrmGlat300()
        {
            InitializeComponent();
        }
        
        public FrmGlat300(string pSourceForm, YR.ERP.Shared.UserInfo pUserInfo, string pWhere)
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
            this.StrFormID = "glat300";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "基本資料";
            uTab_Master.Tabs[1].Text = "狀態";
            uTab_Master.Tabs[2].Text = "資料瀏覽";

            IntTabDetailCount = 2;
            uTab_Detail.Tabs[0].Text = "明細資料";
            uTab_Detail.Tabs[1].Text = "沖帳資料";  //借用--後續要關掉不顯示
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("gfa01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "gfasecu";
                TabMaster.GroupColumn = "gfasecg";

                TabMaster.CanCopyMode = false;
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
                    BoBas.TRAN = BoMaster.TRAN;
                    BoGla.TRAN = BoMaster.TRAN;
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

            this.TabDetailList[0].TargetTable = "gfb_tb";
            this.TabDetailList[0].ViewTable = "vw_glat300s";
            keyParm = new SqlParameter("gfb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "gfa01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };

            //沖帳資料--借用明細,但實際資料以vw_glat300s 做為連結,以下key資料僅供參考不使用
            this.TabDetailList[1].TargetTable = "gfh_tb";
            this.TabDetailList[1].ViewTable = "vw_glat300a";
            keyParm = new SqlParameter("gfh03", SqlDbType.NVarChar);
            keyParm.SourceColumn = "gfa01";
            this.TabDetailList[1].RelationParams = new List<SqlParameter>() { keyParm };
            this.TabDetailList[1].IsReadOnly = true;    //改在append update中 先刪後新增
            return true;
        }
        #endregion

        #region WfIniDetailComboSource 處理grid下拉選單
        protected override void WfIniDetailComboSource(int pTabIndex)
        {
            UltraGrid uGrid;
            UltraGridColumn ugc;
            ImageList ilLarge;
            try
            {
                switch (pTabIndex)
                {
                    case 0:
                        ilLarge = GlobalPictuer.LoadToolBarImage();

                        uGrid = TabDetailList[pTabIndex].UGrid;
                        ugc = uGrid.DisplayLayout.Bands[0].Columns["gfb06"];//借貸方
                        WfSetGridValueList(ugc, BoGla.OfGetGfb06KVPList());

                        ////處理立沖欄位
                        //ugc = uGrid.DisplayLayout.Bands[0].Columns["gba08"];
                        //ugc.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;

                        //if (ilLarge != null)
                        //{
                        //    if (ilLarge.Images.ContainsKey("pick_32"))
                        //    {
                        //        ugc.CellButtonAppearance.Image = ilLarge.Images["pick_32"];
                        //    }
                        //    ugc.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                        //}
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
                if (pUgrid.DisplayLayout.Bands[0].Columns.Exists("gba08"))
                {
                    lugc = pUgrid.DisplayLayout.Bands[0].Columns["gba08"];
                    WfSetUgridCheckBox(lugc);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        
        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_glat300 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_glat300>();
                    WfSetDocPicture("", masterModel.gfaconf, null, pbxDoc);
                }
                else
                {
                    WfSetDocPicture("", "", "", pbxDoc);
                }

                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false); //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    
                    WfSetControlReadonly(new List<Control> { ute_gfa03, ute_gfa04 }, true);
                    WfSetControlReadonly(new List<Control> { ute_gfa06, ute_gfa07 }, true);
                    WfSetControlReadonly(new List<Control> { ute_gfa08, ute_gfa09 }, true);
                    WfSetControlReadonly(new List<Control> { ute_gfaconf, ute_gfaconu, udt_gfacond }, true);
                    WfSetControlReadonly(new List<Control> { ucx_gfapost, ute_gfaposu, udt_gfaposd }, true);

                    WfSetControlReadonly(new List<Control> { ute_gfacreu, ute_gfacreg, udt_gfacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_gfamodu, ute_gfamodg, udt_gfamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_gfasecu, ute_gfasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.gfa01, "") != "")
                    {
                        WfSetControlReadonly(ute_gfa01, true);
                        //WfSetgfa01RelReadonly(GlobalFn.isNullRet(masterModel.gfa01, ""));
                    }
                    
                    WfSetControlReadonly(new List<Control> { ute_gfa01_c }, true);
                    //WfSetControlReadonly(new List<Control> { ucb_gfaconf, udt_gfacond, ute_gfaconu, ute_gfastat }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_gfa01, true);
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
                                columnName == "gfb03" ||
                                columnName == "gfb04" ||
                                columnName == "gfb06" ||
                                columnName == "gfb10"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }
                            
                            if (columnName == "gfb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }
                            
                            if (columnName == "gfb05")    //部門
                            {
                                if (GlobalFn.isNullRet(pDr["gba09"], "") == "Y")
                                    WfSetControlReadonly(ugc, false);
                                else
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
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
        
        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                vw_glat300 masterModel = null;
                vw_glat300s detailModel = null;
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_glat300
                if (pDr.Table.Prefix.ToLower() == "vw_glat300")
                {
                    masterModel = DrMaster.ToItem<vw_glat300>();
                    switch (pColName.ToLower())
                    {
                        case "gfa01"://傳票單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            //messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "stp"));
                            //messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "30"));
                            WfShowPickUtility("p_gac1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["gac01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
                }
                #endregion
                
                #region 單身-pick vw_glat300s
                if (pDr.Table.Prefix.ToLower() == "vw_glat300s")
                {
                    masterModel = DrMaster.ToItem<vw_glat300>();
                    detailModel = pDr.ToItem<vw_glat300s>();
                    switch (pColName.ToLower())
                    {
                        case "gfb03"://會計科目
                            messageModel.StrWhereAppend = " AND gba03='2' AND gba06 in ('2','3')";
                            WfShowPickUtility("p_gba1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["gba01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "gfb05"://會計部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                {
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                }
                                else
                                {
                                    pDr[pColName] = "";
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
        
        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            int year, period;
            try
            {
                pDr["gfa02"] = Today;
                pDr["gfa03"] = 0;
                pDr["gfa04"] = 0;
                pDr["gfa06"] = "GL";
                pDr["gfa07"] = "";
                if (BoGla.OfGetGlaYearPeriod(Today, out year, out period) == false)
                {
                    WfShowErrorMsg("取得會計年度失敗!");
                    return false;
                }
                pDr["gfa08"] = year;
                pDr["gfa09"] = period;
                pDr["gfaprno"] = 0;
                pDr["gfaconf"] = "N";
                pDr["gfapost"] = "N";
                pDr["gfacomp"] = LoginInfo.CompNo;
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

                switch (pCurTabDetail)
                {
                    case 0:
                        pDr["gfb02"] = WfGetMaxSeq(pDr.Table, "gfb02");
                        pDr["gfb06"] = "1";             //借方
                        pDr["gfb07"] = 0;
                        pDr["gfb08"] = BaaModel.baa04;   //原幣幣別
                        pDr["gfb09"] = 1;
                        pDr["gfb10"] = 0;
                        pDr["gfbcomp"] = LoginInfo.CompNo;
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

        #region WfPreInInsertModeCheck 進新增模式前的檢查及清變數與設定變數
        protected override bool WfPreInInsertModeCheck()
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

        #region WfPreInsertDetailCheck() :新增明細資料前檢查
        protected override bool WfPreInsertDetailCheck(int pCurTabDetail)
        {
            vw_glat300 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_glat300>();
                if (WfFormCheck() == false)
                    return false;

                if (masterModel.gfa06.ToLower() != "gla")
                {
                    WfShowErrorMsg("拋轉後傳票,不可新增明細!");
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

        #region WfPreInUpdateModeCheck() 進存檔模式前檢查,及設定變數
        protected override bool WfPreInUpdateModeCheck()
        {
            vw_glat300 masterModel;
            try
            {
                masterModel = DrMaster.ToItem<vw_glat300>();
                if (masterModel.gfaconf != "N")
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreDeleteCheck 進主檔刪除前檢查
        protected override bool WfPreDeleteCheck(DataRow pDr)
        {
            vw_glat300 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_glat300>();
                if (masterModel.gfa06.ToLower() != "gla")
                {
                    WfShowErrorMsg("拋轉後傳票,不可刪除!");
                    return false;
                }
                if (masterModel.gfaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可刪除!");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfAfterDetailDelete() :刪除明細後調用 pdr為clone出來的舊資料
        protected override bool WfAfterDetailDelete(int pCurTabDetail, DataRow pDr)
        {
            vw_glat300s glat300sModel = null;
            vw_glat300a glat300aModel = null;
            string deleteSql = "", updateSql = "";
            List<SqlParameter> sqlParmsList = null;
            try
            {
                switch (pCurTabDetail)
                {
                    case 0:
                        glat300sModel = pDr.ToItem<vw_glat300s>();
                        if (glat300sModel.gba08 == "Y" && glat300sModel.gba05 != glat300sModel.gfb06)
                        {
                            //該資料列非新增時,需刪除背後沖帳資料
                            if (pDr.RowState != DataRowState.Added)
                            {
                                deleteSql = @"DELETE FROM gfh_tb WHERE gfh01=@gfh01
                                                   AND gfh02=@gfh02 AND gfh03=@gfh03 AND gfh04=@gfh04
                                        ";
                                updateSql = @"UPDATE gfg_tb
                                                SET gfg08=(SELECT ISNULL(SUM(gfh09),0) FROM gfh_tb WHERE gfg01=gfh01 AND gfg02=gfh02 AND ISNULL(gfhconf,'')='Y'),
	                                                gfg09=(SELECT ISNULL(SUM(gfh09),0) FROM gfh_tb WHERE gfg01=gfh01 AND gfg02=gfh02 AND ISNULL(gfhconf,'')='N')
                                                WHERE gfg01=@gfg01 AND gfg02=@gfg02
                                               ";
                            }

                            for (int i = TabDetailList[1].DtSource.Rows.Count - 1; i >= 0; i--)
                            {
                                var drTemp = TabDetailList[1].DtSource.Rows[i];
                                glat300aModel = drTemp.ToItem<vw_glat300a>();
                                if (Convert.ToInt16(drTemp["gfh04"]) == Convert.ToInt16(glat300sModel.gfb02))
                                {
                                    TabDetailList[1].DtSource.Rows.Remove(drTemp);
                                    //該資料列非新增時,需刪除背後沖帳資料
                                    if (pDr.RowState != DataRowState.Added)
                                    {
                                        sqlParmsList = new List<SqlParameter>();
                                        sqlParmsList.Add(new SqlParameter("@gfh01", glat300aModel.gfh01));
                                        sqlParmsList.Add(new SqlParameter("@gfh02", glat300aModel.gfh02));
                                        sqlParmsList.Add(new SqlParameter("@gfh03", glat300aModel.gfh03));
                                        sqlParmsList.Add(new SqlParameter("@gfh04", glat300aModel.gfh04));
                                        BoGla.OfExecuteNonquery(deleteSql, sqlParmsList.ToArray());

                                        sqlParmsList = new List<SqlParameter>();
                                        sqlParmsList.Add(new SqlParameter("@gfg01", glat300aModel.gfh01));
                                        sqlParmsList.Add(new SqlParameter("@gfg02", glat300aModel.gfh02));
                                        BoGla.OfExecuteNonquery(updateSql, sqlParmsList.ToArray());
                                    }
                                }
                            }
                        }
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int chkCnts = 0;
            vw_glat300 masterModel = null;
            vw_glat300s glat300sModel = null;
            List<vw_glat300s> glat300sList = null;
            List<vw_glat300a> glat300aList = null;
            gba_tb gbaModel = null;
            UltraGrid uGrid = null;
            UltraGridRow uGridRow = null;
            int year, period;
            try
            {
                masterModel = DrMaster.ToItem<vw_glat300>();
                if (e.Column.ToLower() != "gfa01" && GlobalFn.isNullRet(DrMaster["gfa01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false;
                }
                #region 單頭-vw_glat300
                if (e.Row.Table.Prefix.ToLower() == "vw_glat300")
                {
                    switch (e.Column.ToLower())
                    {
                        case "gfa01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoGla.OfChkGacPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["gfa01_c"] = BoGla.OfGetGac02(e.Value.ToString());
                            break;

                        case "gfa02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;

                            year = 0;period=0;
                            if (BoGla.OfGetGlaYearPeriod(Convert.ToDateTime(e.Value) , out year, out period) == false)
                            {
                                WfShowErrorMsg("取得會計年度失敗!");
                                return false;
                            }
                            e.Row["gfa08"] = year;
                            e.Row["gfa09"] = period;
                            break;
                    }
                }
                #endregion

                #region 單身-vw_glat300s
                if (e.Row.Table.Prefix.ToLower() == "vw_glat300s")
                {
                    uGrid = sender as UltraGrid;
                    uGridRow = uGrid.ActiveRow;
                    glat300sModel = e.Row.ToItem<vw_glat300s>();
                    glat300sList = e.Row.Table.ToList<vw_glat300s>();
                    glat300aList = TabDetailList[1].DtSource.ToList<vw_glat300a>();
                    switch (e.Column.ToLower())
                    {
                        case "gfb02":
                            #region 項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            glat300sList = e.Row.Table.ToList<vw_glat300s>();
                            chkCnts = glat300sList.Where(p => GlobalFn.isNullRet(p.gfb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (chkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            //立沖處理
                            //沖帳傳票,需修改項次
                            if (glat300sModel.gba08 == "Y" && !GlobalFn.varIsNull(glat300sModel.gfb03) && glat300sModel.gba05 != glat300sModel.gfb06)
                            {
                                foreach (DataRow drTemp in TabDetailList[1].DtSource.Rows)
                                {
                                    if (GlobalFn.isNullRet(drTemp["gfh04"], 0) == Convert.ToInt16(this.OldValue))
                                    {
                                        drTemp["gfh04"] = e.Value;
                                    }                                    
                                }
                            }
                            break;
                            #endregion
                        case "gfb03":
                            #region 會計科目
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                if (!GlobalFn.varIsNull(glat300sModel.gba05) && glat300sModel.gba08 == "Y" && glat300sModel.gba05 != glat300sModel.gfb06)
                                {
                                    chkCnts = glat300aList.Where(p => p.gfh04 == glat300sModel.gfb02).Count();
                                    if (chkCnts > 0)
                                    {
                                        var dialogResult = WfShowConfirmMsg("修改會科將刪除已挑選沖帳傳票,請問是否繼續?");
                                        if (dialogResult == DialogResult.Yes)
                                        {
                                            for (int i = TabDetailList[1].DtSource.Rows.Count - 1; i >= 0; i--)
                                            {
                                                var drTemp = TabDetailList[1].DtSource.Rows[i];
                                                if (GlobalFn.isNullRet(drTemp["gfh04"], 0m) == glat300sModel.gfb02)
                                                {
                                                    TabDetailList[1].DtSource.Rows.Remove(drTemp);
                                                }
                                            }
                                        } 
                                        else
                                            return false;
                                    }
                                }
                                e.Row["gfb03_c"] = "";
                                e.Row["gba05"] = "";
                                e.Row["gba08"] = "";
                                e.Row["gba09"] = "";
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
                                WfShowErrorMsg("非有效會計科目,請檢核!");
                                return false;
                            }
                            if (gbaModel.gba03 != "2")
                            {
                                WfShowErrorMsg("只能挑選帳戶型會科,請檢核!");
                                return false;
                            }
                            if (gbaModel.gba06 != "2" && gbaModel.gba06 != "3")
                            {
                                WfShowErrorMsg("只能挑選明細或獨立帳戶科目,請檢核!");
                                return false;
                            }
                            if (!GlobalFn.varIsNull(glat300sModel.gba05) && glat300sModel.gba08 == "Y" && glat300sModel.gba05 != glat300sModel.gfb06)
                            {
                                chkCnts = glat300aList.Where(p => p.gfh04 == glat300sModel.gfb02).Count();
                                if (chkCnts > 0)
                                {
                                    var dialogResult = WfShowConfirmMsg("修改會科將刪除已挑選沖帳傳票,請問是否繼續?");
                                    if (dialogResult == DialogResult.Yes)
                                    {
                                        for (int i = TabDetailList[1].DtSource.Rows.Count - 1; i >= 0; i--)
                                        {
                                            var drTemp = TabDetailList[1].DtSource.Rows[i];
                                            if (GlobalFn.isNullRet(drTemp["gfh04"], 0m) == glat300sModel.gfb02)
                                            {
                                                TabDetailList[1].DtSource.Rows.Remove(drTemp);
                                            }
                                        }
                                    }
                                    else
                                        return false;
                                }
                            }
                            
                            var ugcGfb05 = TabDetailList[0].UGrid.ActiveRow.Cells["gfb05"];
                            if (gbaModel.gba09 == "Y")    //屬於部門管理時
                            {
                                WfSetControlReadonly(ugcGfb05, false);
                            }
                            else
                            {
                                e.Row["gfb05"] = "";
                                WfSetControlReadonly(ugcGfb05, true);
                            }
                            e.Row["gfb03_c"] = gbaModel.gba02;
                            e.Row["gba05"] = gbaModel.gba05;
                            e.Row["gba08"] = gbaModel.gba08;
                            e.Row["gba09"] = gbaModel.gba09;
                            break;
                            #endregion
                        case "gfb06":
                            #region 借貸方
                            //立沖處理
                            //原為沖帳改立帳,要刪除明細的沖帳資料
                            if (glat300sModel.gba08 == "Y" && OldValue.ToString() != glat300sModel.gba05)
                            {
                                for (int i = TabDetailList[1].DtSource.Rows.Count - 1; i >= 0; i--)
                                {
                                    var drTemp = TabDetailList[1].DtSource.Rows[i];
                                    if (GlobalFn.isNullRet(drTemp["gfh04"], 0m) == glat300sModel.gfb02)
                                    {
                                        TabDetailList[1].DtSource.Rows.Remove(drTemp);
                                    }
                                }
                            }
                            WfSetTotalAmt();
                            break;
                            
                            #endregion
                        case "gfb10":
                            #region 原幣金額
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (glat300sModel.gfb10 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            if (glat300sModel.gfb03 == null)
                            {
                                WfShowErrorMsg("請先輸入會計科目!");

                                WfItemChkForceFocus(uGrid, uGridRow.Cells["gfb03"]);
                                return false;
                            }
                            //依幣別檔設定單價小數
                            var bekTempModel = BoBas.OfGetBekModel(glat300sModel.gfb08);
                            if (bekTempModel == null)
                            {
                                WfShowErrorMsg("查無此幣別檔資料,請先輸入!");
                                return false;
                            }
                            
                            e.Row[e.Column] = GlobalFn.Round(Convert.ToDecimal(e.Value), bekTempModel.bek04);
                            WfSetGfb07(e.Row);
                            WfSetTotalAmt();
                            if (glat300sModel.gba08 == "Y" && glat300sModel.gba05 != glat300sModel.gfb06)
                                WfOpenGlat300_1();
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
        
        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_glat300 masterModel = null;
            vw_glat300s detailModel = null;
            //bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_glat300>();
                //if (!GlobalFn.varIsNull(masterModel.gfa01))
                //    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.gfa01, ""));
                #region 單頭資料檢查
                chkColName = "gfa01";       //傳票單號
                chkControl = ute_gfa01;
                if (GlobalFn.varIsNull(masterModel.gfa01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "gfa02";       //傳票日期
                chkControl = udt_gfa02;
                if (GlobalFn.varIsNull(masterModel.gfa02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.gfa02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                //移到確認做檢查
                //chkColName = "gfa03";       //借方總金額
                //chkControl = ute_gfa03;
                //if (masterModel.gfa03 != masterModel.gfa04)
                //{
                //    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                //    chkControl.Focus();
                //    msg = "借貸方金額不相同,請檢核!";
                //    errorProvider.SetError(chkControl, msg);
                //    errorProvider.SetError(ute_gfa04, msg);
                //    WfShowErrorMsg(msg);                
                //    return false;
                //}
                #endregion

                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    detailModel = drTemp.ToItem<vw_glat300s>();
                    chkColName = "gfb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.gfb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "gfb03";   //科目編號
                    if (GlobalFn.varIsNull(detailModel.gfb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "gfb06";   //借貸別
                    if (GlobalFn.varIsNull(detailModel.gfb06))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "gfb05";   //部門
                    if (GlobalFn.isNullRet(detailModel.gba09, "") == "Y" && GlobalFn.varIsNull(detailModel.gfb05))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "gfb08";   //原幣幣別
                    if (GlobalFn.varIsNull(detailModel.gfb08))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
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
            string gfa01New, errMsg;
            vw_glat300 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_glat300>();
                if (FormEditMode == YREditType.新增)
                {
                    gfa01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.gfa01, ModuleType.gla, (DateTime)masterModel.gfa02, out gfa01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["gfa01"] = gfa01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["gfasecu"] = LoginInfo.UserNo;
                        DrMaster["gfasecg"] = LoginInfo.GroupNo;
                        DrMaster["gfacreu"] = LoginInfo.UserNo;
                        DrMaster["gfacreg"] = LoginInfo.DeptNo;
                        DrMaster["gfacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["gfamodu"] = LoginInfo.UserNo;
                        DrMaster["gfamodg"] = LoginInfo.DeptNo;
                        DrMaster["gfamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["gfbcreu"] = LoginInfo.UserNo;
                            drDetail["gfbcreg"] = LoginInfo.DeptNo;
                            drDetail["gfbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["gfbmodu"] = LoginInfo.UserNo;
                            drDetail["gfbmodg"] = LoginInfo.DeptNo;
                            drDetail["gfbmodd"] = Now;
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
            List<ButtonTool> buttonList = new List<ButtonTool>();
            ButtonTool bt;
            ado_tb adoModel;
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

                bt = new ButtonTool("Glat300_1");
                bt.SharedProps.Caption = "沖帳維護";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("Glab311");
                bt.SharedProps.Caption = "傳票過帳";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("Glab313");
                bt.SharedProps.Caption = "過帳還原";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("Glar300");
                adoModel = BoAdm.OfGetAdoModel("Glar300");
                bt.SharedProps.Caption = adoModel.ado02;
                bt.SharedProps.Category = "Report";
                bt.Tag = adoModel;
                buttonList.Add(bt);

                bt = new ButtonTool("Glar301");
                adoModel = BoAdm.OfGetAdoModel("Glar301");
                bt.SharedProps.Caption = adoModel.ado02;
                bt.SharedProps.Category = "Report";
                bt.Tag = adoModel;
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
            try
            {
                vw_glat300 masterModel;
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
                    case "Glat300_1":
                        if (FormEditMode == YREditType.NA)
                            return;
                        WfOpenGlat300_1();
                        break;
                    case "Glab311":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (WfGlab311() == false)
                            return;
                        break;
                    case "Glab313":
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (WfGlab313() == false)
                            return;
                        break;

                    case "Glar300":
                        vw_glar300 glar300Model;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_glat300>();
                        glar300Model = new vw_glar300();
                        glar300Model.gfa01 = masterModel.gfa01;
                        glar300Model.gfaconf = "";
                        glar300Model.gfapost = "";
                        glar300Model.gfaprno = 0;

                        FrmGlar300 frmGlar300 = new FrmGlar300(this.LoginInfo, glar300Model, true, true);
                        frmGlar300.WindowState = FormWindowState.Minimized;
                        frmGlar300.ShowInTaskbar = false;
                        frmGlar300.Show();
                        break;

                    case "Glar301":
                        vw_glar301 glar301Model;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_glat300>();
                        glar301Model = new vw_glar301();
                        glar301Model.gfa01 = masterModel.gfa01;
                        glar301Model.gfaconf = "";
                        glar301Model.gfapost = "";
                        glar301Model.gfaprno = 0;

                        FrmGlar301 frmGlar301 = new FrmGlar301(this.LoginInfo, glar301Model, true, true);
                        frmGlar301.WindowState = FormWindowState.Minimized;
                        frmGlar301.ShowInTaskbar = false;
                        frmGlar301.Show();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAppendUpdate 存檔後處理額外的資料表 新增、修改、刪除...
        protected override bool WfAppendUpdate()
        {
            vw_glat300 glat300Model = null;
            vw_glat300a glat300aModel = null;
            string sql = "";
            List<SqlParameter> sqlParmList = null;
            DataTable dtGfhTb = null;
            GlaBLL boAppend;
            try
            {
                glat300Model = DrMaster.ToItem<vw_glat300>();

                //立沖-沖帳處理 先刪後新增
                if (FormEditMode == YREditType.修改)
                {
                    //先記錄要刪除的立帳傳票編號,之後要更新
                    sql = @"SELECT * FROM gfh_tb
                            WHERE gfh03=@gfh03
                          ";
                    
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@gfh03", glat300Model.gfa01));
                    var dtDeleteGfh = BoGla.OfGetDataTable(sql, sqlParmList.ToArray());
                    if (dtDeleteGfh != null && dtDeleteGfh.Rows.Count > 0)
                    {
                        var gfhDeleteList = dtDeleteGfh.ToList<gfh_tb>();
                        var gfhDistinctList = from o in gfhDeleteList
                                              group o by new { o.gfh01, o.gfh02 } into g
                                              select new
                                              {
                                                  gfh01 = g.Key.gfh01,
                                                  gfh02 = g.Key.gfh02
                                              }
                                               ;
                        sql = @"DELETE FROM gfh_tb
                            WHERE gfh03=@gfh03
                            ";
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gfh03", glat300Model.gfa01));
                        BoGla.OfExecuteNonquery(sql, sqlParmList.ToArray());
                        
                        //更新暫沖金額
                        sql = @"UPDATE gfg_tb
                                SET gfg08=(SELECT ISNULL(SUM(gfh09),0) FROM gfh_tb WHERE gfg01=gfh01 AND gfg02=gfh02 AND ISNULL(gfhconf,'')='Y'),
	                                gfg09=(SELECT ISNULL(SUM(gfh09),0) FROM gfh_tb WHERE gfg01=gfh01 AND gfg02=gfh02 AND ISNULL(gfhconf,'')='N')
                                WHERE gfg01=@gfg01 AND gfg02=@gfg02
                               ";
                        foreach (var gfhTempModel in gfhDistinctList)
                        {
                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@gfg01", gfhTempModel.gfh01));
                            sqlParmList.Add(new SqlParameter("@gfg02", gfhTempModel.gfh02));
                            BoGla.OfExecuteNonquery(sql, sqlParmList.ToArray());
                        }
                    }
                }
                
                if (TabDetailList[1].DtSource.Rows.Count > 0)
                {
                    boAppend = new GlaBLL(BoMaster.OfGetConntion());
                    boAppend.TRAN = BoMaster.TRAN;
                    boAppend.OfCreateDao("gfh_tb", "*", "");
                    dtGfhTb = boAppend.OfSelect("WHERE 1<>1 ");
                    foreach (DataRow drGlat300a in TabDetailList[1].DtSource.Rows)
                    {
                        glat300aModel = drGlat300a.ToItem<vw_glat300a>();
                        var drNew = dtGfhTb.NewRow();
                        drNew["gfh01"] = glat300aModel.gfh01;   //立帳傳票編號
                        drNew["gfh02"] = glat300aModel.gfh02;   //立帳傳票項次
                        drNew["gfh03"] = glat300Model.gfa01;   //沖帳傳票編號 --取單頭傳票編號
                        drNew["gfh04"] = glat300aModel.gfh04;   //沖帳傳票項
                        //drNew["gfh05"] = glat300aModel.gfh05;
                        drNew["gfh05"] = glat300Model.gfa02;    //取單頭傳票日期
                        drNew["gfh06"] = glat300aModel.gfh06;   //會計科目
                        drNew["gfh07"] = glat300aModel.gfh07;   //摘要
                        drNew["gfh08"] = glat300aModel.gfh08;   //部門
                        drNew["gfh09"] = glat300aModel.gfh09;   //沖帳金額
                        drNew["gfh10"] = glat300aModel.gfh10;
                        drNew["gfh11"] = glat300aModel.gfh11;
                        drNew["gfh12"] = glat300aModel.gfh12;
                        drNew["gfh13"] = glat300aModel.gfh13;
                        drNew["gfh14"] = glat300aModel.gfh14;
                        drNew["gfh15"] = glat300aModel.gfh15;
                        drNew["gfhconf"] = "N";
                        drNew["gfhcomp"] = LoginInfo.CompNo;
                        dtGfhTb.Rows.Add(drNew);
                    }
                    boAppend.OfUpdate(dtGfhTb);
                }
                //這裡要再重刷一次暫沖金額,跟已沖金額
                if (TabDetailList[1].DtSource.Rows.Count > 0)
                {
                    //先記錄要更新的立帳傳票編號
                    sql = @"SELECT * FROM gfh_tb
                            WHERE gfh03=@gfh03
                          ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@gfh03", glat300Model.gfa01));
                    var dtRefreshGfh = BoGla.OfGetDataTable(sql, sqlParmList.ToArray());
                    if (dtRefreshGfh != null && dtRefreshGfh.Rows.Count > 0)
                    {
                        var gfhRefreshList = dtRefreshGfh.ToList<gfh_tb>();
                        var gfhDistinctList = from o in gfhRefreshList
                                              group o by new { o.gfh01, o.gfh02 } into g
                                              select new
                                              {
                                                  gfh01 = g.Key.gfh01,
                                                  gfh02 = g.Key.gfh02
                                              }
                                               ;

                        //更新暫沖金額
                        sql = @"UPDATE gfg_tb
                                SET gfg08=(SELECT ISNULL(SUM(gfh09),0) FROM gfh_tb WHERE gfg01=gfh01 AND gfg02=gfh02 AND ISNULL(gfhconf,'')='Y'),
	                                gfg09=(SELECT ISNULL(SUM(gfh09),0) FROM gfh_tb WHERE gfg01=gfh01 AND gfg02=gfh02 AND ISNULL(gfhconf,'')='N')
                                WHERE gfg01=@gfg01 AND gfg02=@gfg02
                               ";
                        foreach (var gfhTempModel in gfhDistinctList)
                        {
                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@gfg01", gfhTempModel.gfh01));
                            sqlParmList.Add(new SqlParameter("@gfg02", gfhTempModel.gfh02));
                            BoGla.OfExecuteNonquery(sql, sqlParmList.ToArray());                            
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
        protected override bool WfDeleteAppenUpdate(DataRow pDr)
        {
            string deleteSql = "", updateSql = "";
            List<SqlParameter> sqlParmList = null;
            try
            {
                //處理立沖-暫沖資料
                if (TabDetailList[1].DtSource != null && TabDetailList[1].DtSource.Rows.Count > 0)
                {
                    deleteSql = @"DELETE FROM gfh_tb WHERE gfh01=@gfh01
                                                   AND gfh02=@gfh02 AND gfh03=@gfh03 AND gfh04=@gfh04
                                        ";
                    updateSql = @"UPDATE gfg_tb
                                                SET gfg08=(SELECT ISNULL(SUM(gfh09),0) FROM gfh_tb WHERE gfg01=gfh01 AND gfg02=gfh02 AND ISNULL(gfhconf,'')='Y'),
	                                                gfg09=(SELECT ISNULL(SUM(gfh09),0) FROM gfh_tb WHERE gfg01=gfh01 AND gfg02=gfh02 AND ISNULL(gfhconf,'')='N')
                                                WHERE gfg01=@gfg01 AND gfg02=@gfg02
                                               ";
                    foreach (DataRow drTemp in TabDetailList[1].DtSource.Rows)
                    {
                        var glat300aModel = drTemp.ToItem<vw_glat300a>();
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gfh01", glat300aModel.gfh01));
                        sqlParmList.Add(new SqlParameter("@gfh02", glat300aModel.gfh02));
                        sqlParmList.Add(new SqlParameter("@gfh03", glat300aModel.gfh03));
                        sqlParmList.Add(new SqlParameter("@gfh04", glat300aModel.gfh04));
                        BoGla.OfExecuteNonquery(deleteSql, sqlParmList.ToArray());

                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gfg01", glat300aModel.gfh01));
                        sqlParmList.Add(new SqlParameter("@gfg02", glat300aModel.gfh02));
                        BoGla.OfExecuteNonquery(updateSql, sqlParmList.ToArray());
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

        //*****************************非正常ovveride*************************************

        #region WfIniDetail : 依定義的 itabsCount 設定 uTab_Detail
        /// <summary>
        /// 依定義的 itabsCount 設定 uTab_Detail
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfIniDetail()
        {
            try
            {                
                if (base.WfIniDetail() == false)
                    return false;
                //關掉明細第二個頁籤
                uTab_Detail.Tabs[1].Visible = false;                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
        #endregion
        
        //*****************************表單自訂Fuction****************************************	

        #region WfSetGfb07 填入本幣金額
        private bool WfSetGfb07(DataRow pDr)
        {            
            vw_glat300s detailModel;
            bek_tb bekModel = null;
            try
            {
                detailModel = pDr.ToItem<vw_glat300s>();
                //取得本幣資料
                bekModel = BoBas.OfGetBekModel(BaaModel.baa04);
                if (bekModel == null)
                {
                    WfShowErrorMsg("未設定本幣幣別資料,請確認!");
                    return false;
                }
                pDr["gfb07"] = GlobalFn.Round(detailModel.gfb10 * detailModel.gfb09, bekModel.bek04);    //取小計/總計的小數位數來四捨伍入
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetTotalAmt 更新借貸方,總金額
        private bool WfSetTotalAmt()
        {
            List<vw_glat300s> glat300List = null;
            decimal gfa03 = 0, gfa04 = 0;
            try
            {
                glat300List = TabDetailList[0].DtSource.ToList<vw_glat300s>();
                gfa03 = glat300List.Where(p => p.gfb06 == "1").Sum(p => p.gfb07);
                gfa04 = glat300List.Where(p => p.gfb06 == "2").Sum(p => p.gfb07);
                DrMaster["gfa03"] = gfa03;
                DrMaster["gfa04"] = gfa04;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfConfirm 確認
        private void WfConfirm()
        {
            vw_glat300 masterModel = null;
            vw_glat300s glat300sModel = null;
            vw_glat300a glat300aModel = null;
            CommonBLL boGfg, boGfh;
            DataTable dtGfg;
            StringBuilder sbSql;
            string updateGfgSql = "", updateGfhSql = "";
            string errMsg;
            List<SqlParameter> sqlParmList = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                if (WfRetrieveDetail() == false)
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_glat300>();
                
                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                masterModel.gfaconf = "Y";
                masterModel.gfacond = Today;
                masterModel.gfaconu = LoginInfo.UserNo;

                boGfg = new InvBLL(BoMaster.OfGetConntion());
                boGfg.TRAN = BoMaster.TRAN;
                boGfg.OfCreateDao("gfg_tb", "*", "");
                boGfh = new InvBLL(BoMaster.OfGetConntion());
                boGfh.TRAN = BoMaster.TRAN;
                boGfh.OfCreateDao("gfh_tb", "*", "");
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    glat300sModel = dr.ToItem<vw_glat300s>();
                    //處理立沖資料
                    if (glat300sModel.gba08 == "Y")
                    {
                        //立帳
                        if (glat300sModel.gfb06 == glat300sModel.gba05)
                        {
                            sbSql = new StringBuilder();
                            sbSql.AppendLine("SELECT * FROM gfg_tb");
                            sbSql.AppendLine("WHERE 1<>1");
                            dtGfg = boGfg.OfGetDataTable(sbSql.ToString());
                            var drNewGfg = dtGfg.NewRow();
                            drNewGfg["gfg01"] = glat300sModel.gfb01;
                            drNewGfg["gfg02"] = glat300sModel.gfb02;
                            drNewGfg["gfg03"] = glat300sModel.gfb03;
                            drNewGfg["gfg04"] = glat300sModel.gfb04;
                            drNewGfg["gfg05"] = glat300sModel.gfb05;
                            drNewGfg["gfg06"] = masterModel.gfa02;
                            drNewGfg["gfg07"] = glat300sModel.gfb07;
                            drNewGfg["gfg08"] = 0;
                            drNewGfg["gfg09"] = 0;
                            drNewGfg["gfgcomp"] = masterModel.gfacomp;
                            dtGfg.Rows.Add(drNewGfg);
                            if (boGfg.OfUpdate(dtGfg) <= 0)
                            {
                                throw new Exception("新增傳票立帳資料(gfg_tb)失敗!");
                            }
                        }
                    }
                }
                
                if (TabDetailList[1].DtSource != null && TabDetailList[1].DtSource.Rows.Count > 0)
                {
                    updateGfhSql = @"UPDATE gfh_tb SET gfhconf='Y' 
                                     WHERE gfh01=@gfh01 AND gfh02=@gfh02 AND gfh03=@gfh03 AND gfh04=@gfh04";
                    updateGfgSql = @"UPDATE gfg_tb
                                                SET gfg08=(SELECT ISNULL(SUM(gfh09),0) FROM gfh_tb WHERE gfg01=gfh01 AND gfg02=gfh02 AND ISNULL(gfhconf,'')='Y'),
	                                                gfg09=(SELECT ISNULL(SUM(gfh09),0) FROM gfh_tb WHERE gfg01=gfh01 AND gfg02=gfh02 AND ISNULL(gfhconf,'')='N')
                                                WHERE gfg01=@gfg01 AND gfg02=@gfg02
                                               ";
                    foreach (DataRow drTemp in TabDetailList[1].DtSource.Rows)
                    {
                        glat300aModel = drTemp.ToItem<vw_glat300a>();
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gfh01", glat300aModel.gfh01));
                        sqlParmList.Add(new SqlParameter("@gfh02", glat300aModel.gfh02));
                        sqlParmList.Add(new SqlParameter("@gfh03", glat300aModel.gfh03));
                        sqlParmList.Add(new SqlParameter("@gfh04", glat300aModel.gfh04));
                        BoGla.OfExecuteNonquery(updateGfhSql, sqlParmList.ToArray());

                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gfg01", glat300aModel.gfh01));
                        sqlParmList.Add(new SqlParameter("@gfg02", glat300aModel.gfh02));
                        BoGla.OfExecuteNonquery(updateGfgSql, sqlParmList.ToArray());
                    }
                }
                
                DrMaster["gfaconf"] = "Y";
                DrMaster["gfacond"] = Today;
                DrMaster["gfaconu"] = LoginInfo.UserNo;
                DrMaster["gfamodu"] = LoginInfo.UserNo;
                DrMaster["gfamodg"] = LoginInfo.DeptNo;
                DrMaster["gfamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_glat300>();
                WfSetDocPicture("", masterModel.gfaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfConfirmChk 確認前檢查
        private bool WfConfirmChk()
        {
            vw_glat300 masterModel = null;
            vw_glat300s glat300sModel = null;
            string selectSql = "";
            List<SqlParameter> sqlParmList = null;
            string errMsg = "";
            bek_tb bekModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_glat300>();
                if (masterModel.gfaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }
                if (masterModel.gfa03 != masterModel.gfa04)
                {
                    WfShowErrorMsg("借貸方金額不相同,請檢核!");
                    return false;
                }
                bekModel = BoBas.OfGetBekModel(BaaModel.baa04);
                selectSql = @"SELECT SUM(gfh09) FROM gfh_tb WHERE gfh03=@gfh03 AND gfh04=@gfh04 AND gfhconf='N'";
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    glat300sModel = dr.ToItem<vw_glat300s>();
                    if (glat300sModel.gfb10 == 0)
                    {
                        WfShowErrorMsg("金額不可為0,請檢核!");
                        return false;
                    }

                    if (glat300sModel.gba08 == "Y" && glat300sModel.gba05 != glat300sModel.gfb06)
                    {
                        //若為立沖-沖帳傳票檢查金額是否相同
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gfh03", glat300sModel.gfb01));
                        sqlParmList.Add(new SqlParameter("@gfh04", glat300sModel.gfb02));
                        var sumGfh09 = GlobalFn.isNullRet(BoGla.OfGetFieldValue(selectSql, sqlParmList.ToArray()), 0m);

                        if (sumGfh09 != glat300sModel.gfb07)
                        {
                            errMsg = string.Format("項次{0}金額【{1}】,不等同沖帳傳票金額【{2}】 !",
                                                                glat300sModel.gfb02,
                                                                string.Format("{0:N" + bekModel.bek04 + "}", glat300sModel.gfb07),
                                                                string.Format("{0:N" + bekModel.bek04 + "}", sumGfh09)
                                                                                );
                            WfShowErrorMsg(errMsg);
                            return false;
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

        #region WfCancelConfirm 取消確認
        private void WfCancelConfirm()
        {
            vw_glat300 glat300Model = null;
            vw_glat300s glat300sModel = null;
            string sqlNonQuery;
            List<SqlParameter> sqlParmList = null;
            int chkCnts = 0;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)        //這裡會LOCK資料
                    return;
                if (WfLockMasterRow() == false)         //這裡會LOCK資料
                    return;

                WfSetBllTransaction();
                glat300Model = DrMaster.ToItem<vw_glat300>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    glat300sModel = dr.ToItem<vw_glat300s>();
                    //處理立沖資料
                    if (glat300sModel.gba08 == "Y")
                    {
                        if (glat300sModel.gfb06 == glat300sModel.gba05)
                        {
                            sqlNonQuery = @"DELETE FROM gfg_tb WHERE gfg01=@gfg01 AND gfg02=@gfg02";
                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@gfg01", glat300sModel.gfb01));
                            sqlParmList.Add(new SqlParameter("@gfg02", glat300sModel.gfb02));
                            chkCnts = BoGla.OfExecuteNonquery(sqlNonQuery, sqlParmList.ToArray());
                            if (chkCnts < 0)
                            {
                                WfShowErrorMsg("刪除立帳料失敗!");
                                WfRollback();
                                DrMaster.RejectChanges();
                                return;
                            }
                        }
                    }
                }

                DrMaster["gfaconf"] = "N";
                DrMaster["gfacond"] = DBNull.Value;
                DrMaster["gfaconu"] = "";
                DrMaster["gfamodu"] = LoginInfo.UserNo;
                DrMaster["gfamodg"] = LoginInfo.DeptNo;
                DrMaster["gfamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                glat300Model = DrMaster.ToItem<vw_glat300>();
                WfSetDocPicture("", glat300Model.gfaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion
        
        #region WfCancelConfirmChk 取消確認前檢查
        private bool WfCancelConfirmChk()
        {
            vw_glat300 masterModel = null;
            vw_glat300s glat300sModel = null;
            Result result;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            int chkCnts = 0;

            try
            {
                masterModel = DrMaster.ToItem<vw_glat300>();
                if (masterModel.gfaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                if (masterModel.gfapost == "Y")
                {
                    WfShowErrorMsg("傳票已過帳,不可還原!");
                    return false;
                }

                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.gfa02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    glat300sModel = dr.ToItem<vw_glat300s>();
                    if (glat300sModel.gba08 == "Y")   //立沖科目
                    {
                        //立帳
                        if (glat300sModel.gfb06 == glat300sModel.gba05)
                        {
                            //檢查是否存在沖帳資料
                            sqlSelect = @"SELECT COUNT(1) FROM gfh_tb
                                          WHERE gfh01=@gfh01
                                                AND gfh02=@gfh02
                                    ";
                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@gfh01", glat300sModel.gfb01));
                            sqlParmList.Add(new SqlParameter("@gfh02", glat300sModel.gfb02));
                            chkCnts = GlobalFn.isNullRet(BoGla.OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0);
                            if (chkCnts > 0)
                            {
                                WfShowErrorMsg(string.Format("項次{0},已有沖帳資料,不可取消確認!", glat300sModel.gfb02.ToString()));
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

        #region WfInvalid 作廢/作廢還原
        private void WfInvalid()
        {
            vw_glat300 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_glat300>();

                if (masterModel.gfaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.gfaconf == "N")//走作廢
                {

                    DrMaster["gfaconf"] = "X";
                    DrMaster["gfaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.gfaconf == "X")
                {
                    DrMaster["gfaconf"] = "N";
                    DrMaster["gfaconu"] = "";
                }

                DrMaster["gfamodu"] = LoginInfo.UserNo;
                DrMaster["gfamodg"] = LoginInfo.DeptNo;
                DrMaster["gfamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_glat300>();
                WfSetDocPicture("", masterModel.gfaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfOpenGlat300_1
        private void WfOpenGlat300_1()
        {
            if (TabDetailList[0].UGrid.ActiveRow == null)
                return;
            var drDetail = WfGetUgridDatarow(TabDetailList[0].UGrid.ActiveRow);
            var detailModel = drDetail.ToItem<vw_glat300s>();
            if (detailModel.gba08 == "Y" && detailModel.gfb06 != detailModel.gba05)
            {
                FrmGlat300_1 rpt = new FrmGlat300_1(this.LoginInfo, detailModel, TabDetailList[1].DtSource);
                rpt.ShowDialog();
            }
        }
        #endregion

        #region WfGlab311 傳票過帳
        private bool WfGlab311()
        {
            vw_glat300 masterModel = null;
            vw_glab311 glab311Model = null;
            int chkCnts = 0;
            StringBuilder sbResult = null;
            Glab311BLL boGlab311;
            try
            {
                if (DrMaster == null)
                {
                    WfShowBottomStatusMsg("無資料可過帳");
                    return false;
                }
                WfRetrieveMaster();
                masterModel = DrMaster.ToItem<vw_glat300>();
                if (masterModel.gfaconf != "Y")
                {
                    WfShowBottomStatusMsg("傳票未確認!");
                    return false;
                }
                if (masterModel.gfapost == "Y")
                {
                    WfShowBottomStatusMsg("傳票已過帳!");
                    return false;
                }

                //取得交易物件
                BoMaster.TRAN = BoMaster.OfGetConntion().BeginTransaction();
                boGlab311 = new Glab311BLL(BoMaster.OfGetConntion());
                boGlab311.TRAN = BoMaster.TRAN;

                glab311Model = new vw_glab311();
                glab311Model.gfa01 = masterModel.gfa01;

                var resultList = boGlab311.OfGlab311Post(glab311Model, "N", "", "", LoginInfo);

                if (resultList == null || resultList.Count == 0)
                {
                    WfShowBottomStatusMsg("無可過帳資料!");
                    boGlab311.TRAN.Rollback();
                    return true;
                }

                chkCnts = resultList.Where(p => p.Success == false).Count();
                if (chkCnts > 0)
                {
                    boGlab311.TRAN.Rollback();
                    sbResult = new StringBuilder();
                    sbResult.AppendLine(string.Format("執行失敗!"));
                    sbResult.AppendLine();
                    sbResult.AppendLine(string.Format("錯誤內容如下"));
                    sbResult.AppendLine("====================================");
                    foreach (Result result in resultList.Where(p => p.Success == false))
                    {
                        sbResult.AppendLine(string.Format("key1:【{0}】 錯誤訊息:【{1}】", result.Key1, result.Message));
                    }
                    WfShowErrorMsg(sbResult.ToString());
                    boGlab311.TRAN.Rollback();
                    return false;
                }
                BoMaster.TRAN.Commit();
                WfRetrieveMaster();
                WfShowBottomStatusMsg("執行成功!");
                return true;
            }
            catch (Exception ex)
            {
                if (BoMaster.TRAN != null)
                    BoMaster.TRAN.Rollback();
                WfShowErrorMsg(ex.Message);
                return false;
            }
        }
        #endregion

        #region WfGlab313 過帳還原
        private bool WfGlab313()
        {
            vw_glat300 masterModel = null;
            vw_glab313 glab313Model = null;
            int chkCnts = 0;
            StringBuilder sbResult = null;
            Glab313BLL boGlab313 = null;
            try
            {
                if (DrMaster == null)
                {
                    WfShowBottomStatusMsg("無資料可過帳還原");
                    return false;
                }
                WfRetrieveMaster();
                masterModel = DrMaster.ToItem<vw_glat300>();
                if (masterModel.gfapost != "Y")
                {
                    WfShowBottomStatusMsg("傳票非過帳狀態!");
                    return false;
                }
                //取得交易物件
                BoMaster.TRAN = BoMaster.OfGetConntion().BeginTransaction();
                boGlab313 = new Glab313BLL(BoMaster.OfGetConntion());
                boGlab313.TRAN = BoMaster.TRAN;

                glab313Model = new vw_glab313();
                glab313Model.gfa01 = masterModel.gfa01;

                var resultList = boGlab313.OfGlab313Post(glab313Model, "", "", LoginInfo);

                if (resultList == null || resultList.Count == 0)
                {
                    WfShowBottomStatusMsg("無可過帳還原資料!");
                    boGlab313.TRAN.Rollback();
                    return true;
                }
                
                chkCnts = resultList.Where(p => p.Success == false).Count();
                if (chkCnts > 0)
                {
                    boGlab313.TRAN.Rollback();
                    sbResult = new StringBuilder();
                    sbResult.AppendLine(string.Format("執行失敗!"));
                    sbResult.AppendLine();
                    sbResult.AppendLine(string.Format("錯誤內容如下"));
                    sbResult.AppendLine("====================================");
                    foreach (Result result in resultList.Where(p => p.Success == false))
                    {
                        sbResult.AppendLine(string.Format("key1:【{0}】 錯誤訊息:【{1}】", result.Key1, result.Message));
                    }
                    WfShowErrorMsg(sbResult.ToString());
                    boGlab313.TRAN.Rollback();
                    return false;
                }
                BoMaster.TRAN.Commit();
                WfRetrieveMaster();
                WfShowBottomStatusMsg("執行成功!");
                return true;
            }
            catch (Exception ex)
            {
                if (BoMaster.TRAN != null)
                    BoMaster.TRAN.Rollback();
                throw ex;
            }
        }
        #endregion
    }
}
