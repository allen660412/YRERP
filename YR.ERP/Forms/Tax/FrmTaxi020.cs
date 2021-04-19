/* 程式名稱: 每月發票建立作業
   系統代號: taxi020
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

namespace YR.ERP.Forms.Tax
{
    public partial class FrmTaxi020 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        TaxBLL BoTax = null;

        #endregion

        #region 建構子
        public FrmTaxi020()
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
            this.StrFormID = "taxi020";
            this.IntTabCount = 2;
            this.IntMasterGridPos = 2;
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] 
                    { new SqlParameter("tbe01", SqlDbType.Decimal),
                      new SqlParameter("tbe02", SqlDbType.Decimal),
                      new SqlParameter("tbe03", SqlDbType.Decimal)
                });

                TabMaster.CanCopyMode = false;
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
                    BoTax.TRAN = BoMaster.TRAN;
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

            this.TabDetailList[0].TargetTable = "tbe_tb";
            this.TabDetailList[0].ViewTable = "vw_taxi020s";
            keyParm = new SqlParameter("tbe01", SqlDbType.Decimal);
            keyParm.SourceColumn = "tbe01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            keyParm = new SqlParameter("tbe02", SqlDbType.Decimal);
            keyParm.SourceColumn = "tbe02";
            this.TabDetailList[0].RelationParams.Add(keyParm);
            keyParm = new SqlParameter("tbe03", SqlDbType.Decimal);
            keyParm.SourceColumn = "tbe03";
            this.TabDetailList[0].RelationParams.Add(keyParm);
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            try
            {
                
                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false); //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯

                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_tbe01, true);
                        WfSetControlReadonly(ute_tbe02, true);
                        WfSetControlReadonly(ute_tbe03, true);
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
            bab_tb babModel;
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
                                columnName == "tbe07" ||
                                columnName == "tbe08"
                                )
                            {
                                if (GlobalFn.varIsNull(pDr["tbe09"]))   //發票本未開立時,可編輯
                                    WfSetControlReadonly(ugc, false);
                                else
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }

                            if (columnName == "tbe04" || columnName == "tbe05" || columnName == "tbe06")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
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
                if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.查詢)
                {
                    SelectNextControl(this.uTab_Master, true, true, true, false);
                }
                else if (FormEditMode == YREditType.修改)
                {
                    WfSetFirstVisibelCellFocus(TabDetailList[0].UGrid);
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
                pDr["tbe01"] = Today.Year;
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
                        pDr["total_cnts"] = 0;
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
            vw_taxi020 masterModel = null;
            vw_taxi020s detailModel = null;
            List<vw_taxi020s> detailList = null;
            UltraGrid uGrid = null;
            int begNo, endNo,chkBegNo,chkEndNo;
            int ChkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_taxi020>();
                #region 單頭-vw_taxi020
                if (e.Row.Table.Prefix.ToLower() == "vw_taxi020")
                {
                    switch (e.Column.ToLower())
                    {
                        case "tbe01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (Convert.ToInt32(e.Value) <= 0)
                            {
                                WfShowErrorMsg("不可為0或負數!");
                                return false;
                            }
                            break;
                        case "tbe02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (Convert.ToInt32(e.Value) <= 0)
                            {
                                WfShowErrorMsg("不可為0或負數!");
                                return false;
                            }
                            if (masterModel.tbe03 != 0 && Convert.ToInt32(e.Value) > masterModel.tbe03)
                            {
                                WfShowErrorMsg("發票年月起不得大於迄月!");
                                return false;
                            }

                            break;
                        case "tbe03":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (Convert.ToInt32(e.Value) <= 0)
                            {
                                WfShowErrorMsg("不可為0或負數!");
                                return false;
                            }
                            if (masterModel.tbe02 != 0 && Convert.ToInt32(e.Value) < masterModel.tbe02)
                            {
                                WfShowErrorMsg("發票年月起不得大於迄月!");
                                return false;
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-vw_taxi020s
                if (e.Row.Table.Prefix.ToLower() == "vw_taxi020s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_taxi020s>();
                    detailList = e.Row.Table.ToList<vw_taxi020s>();

                    switch (e.Column.ToLower())
                    {
                        case "tbe04":   //發票別
                            if (GlobalFn.varIsNull(e.Value))
                                return true;

                            if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(detailModel.tbe05) && detailModel.tbe06 > 0
                                && detailList.Count > 1
                                )
                            {
                                if (WfChktbe04tbe05tbe06(e.Value.ToString(), detailModel.tbe05, detailModel.tbe06) == false)
                                    return false;
                            }
                            break;
                        case "tbe05":   //聯數
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (!GlobalFn.varIsNull(detailModel.tbe04) && !GlobalFn.varIsNull(e.Value) && detailModel.tbe06 > 0
                                && detailList.Count > 1
                                )
                            {
                                if (WfChktbe04tbe05tbe06(detailModel.tbe04, e.Value.ToString(), detailModel.tbe06) == false)
                                    return false;
                            }
                            break;
                        case "tbe06":   //簿號
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (!GlobalFn.varIsNull(detailModel.tbe04) && !GlobalFn.varIsNull(detailModel.tbe05) && Convert.ToInt32(detailModel.tbe06) > 0
                                && detailList.Count > 1
                                )
                            {
                                if (WfChktbe04tbe05tbe06(detailModel.tbe04, e.Value.ToString(), detailModel.tbe06) == false)
                                    return false;
                            }
                            
                            break;
                        case "tbe07"://起始發票號碼
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (GlobalFn.varIsNull(detailModel.tbe05))
                                return true;
                            if (e.Value.ToString().Length != 10)
                            {
                                WfShowErrorMsg("發票號碼應為10碼!");
                                return false;
                            }
                            if (!GlobalFn.isEnglishLettrs(e.Value.ToString().Substring(0, 2)))
                            {
                                WfShowErrorMsg("發票前兩碼僅能輸入英文字母!");
                                return false;
                            }
                            if (!GlobalFn.isNumberCharachter(e.Value.ToString().Substring(2, 8)))
                            {
                                WfShowErrorMsg("發票後八碼僅能輸入數字!");
                                return false;
                            }
                            if (!GlobalFn.varIsNull(detailModel.tbe08))
                            {
                                begNo = Convert.ToInt32(e.Value.ToString().Substring(2, 8));
                                endNo = Convert.ToInt32(detailModel.tbe08.Substring(2, 8));
                                if (e.Value.ToString().Substring(0,2) !=detailModel.tbe08.Substring(0,2))
                                {
                                    WfShowErrorMsg("發票字軌應相同,請檢核!");
                                    return false;
                                }

                                if (begNo > endNo)
                                {
                                    WfShowErrorMsg("發票號碼，起不得大於迄!");
                                    return false;
                                }

                                //檢查明細中是否有發票重疊
                                ChkCnts=detailList.Where(p=>!GlobalFn.varIsNull(p.tbe07)  && !GlobalFn.varIsNull(p.tbe08)
                                                        && p.tbe07.Substring(0,2).ToUpper()==detailModel.tbe07.Substring(0,2).ToUpper()
                                                        && !(endNo<Convert.ToInt32(p.tbe07.Substring(2,8)) || begNo>Convert.ToInt32(p.tbe08.Substring(2,8)) )
                                            ).Count();
                               if (ChkCnts>1)
                               {
                                   WfShowErrorMsg("輸入發票有重覆,請檢核!");
                                   return false;
                               }
                            }
                            WfSetTotalCnts(e.Row);
                            break;
                        case "tbe08"://截止發票號碼
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (GlobalFn.varIsNull(detailModel.tbe05))
                                return true;
                            if (e.Value.ToString().Length != 10)
                            {
                                WfShowErrorMsg("發票號碼應為10碼!");
                                return false;
                            }
                            if (!GlobalFn.isEnglishLettrs(e.Value.ToString().Substring(0, 2)))
                            {
                                WfShowErrorMsg("發票前兩碼僅能輸入英文字母!");
                                return false;
                            }
                            
                            if (!GlobalFn.isNumberCharachter(e.Value.ToString().Substring(2, 8)))
                            {
                                WfShowErrorMsg("發票後八碼僅能輸入數字!");
                                return false;
                            }
                            if (!GlobalFn.varIsNull(detailModel.tbe08))
                            {
                                begNo = Convert.ToInt32(detailModel.tbe08.Substring(2, 8));
                                endNo = Convert.ToInt32(e.Value.ToString().Substring(2, 8));
                                if (e.Value.ToString().Substring(0, 2) != detailModel.tbe07.Substring(0, 2))
                                {
                                    WfShowErrorMsg("發票字軌應相同,請檢核!");
                                    return false;
                                }

                                if (begNo > endNo)
                                {
                                    WfShowErrorMsg("發票號碼，起不得大於迄!");
                                    return false;
                                }
                                //檢查明細中是否有發票重疊
                                ChkCnts = detailList.Where(p => !GlobalFn.varIsNull(p.tbe07) && !GlobalFn.varIsNull(p.tbe08)
                                                        && p.tbe07.Substring(0, 2).ToUpper() == detailModel.tbe07.Substring(0, 2).ToUpper()
                                                        && !(endNo < Convert.ToInt32(p.tbe07.Substring(2, 8)) || begNo > Convert.ToInt32(p.tbe08.Substring(2, 8)))
                                            ).Count();
                                if (ChkCnts > 1)
                                {
                                    WfShowErrorMsg("輸入發票有重覆,請檢核!");
                                    return false;
                                }
                            }                                                        
                            WfSetTotalCnts(e.Row);
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

        #region WfPreInsertDetailCheck() :新增明細資料前檢查
        protected override bool WfPreInsertDetailCheck(int pCurTabDetail)
        {
            vw_taxi020 masterModel = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParms = null;
            int chkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_taxi020>();
                if (GlobalFn.varIsNull(masterModel.tbe01) || masterModel.tbe01 == 0 ||
                    GlobalFn.varIsNull(masterModel.tbe02) || masterModel.tbe02 == 0 ||
                    GlobalFn.varIsNull(masterModel.tbe03) || masterModel.tbe03 == 0
                )
                {
                    WfShowErrorMsg("請先新增單頭資料!");
                    return false;
                }

                if (TabDetailList[0].DtSource.Rows.Count == 0)
                {
                    //檢查單頭資料是否存在
                    sqlSelect = @"SELECT COUNT(1) FROM tbe_tb
                                  WHERE tbe01=@tbe01
                                      AND NOT (@tbe02>tbe03
                                         OR tbe03<@tbe02)
                                ";
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@tbe01", masterModel.tbe01));
                    sqlParms.Add(new SqlParameter("@tbe02", masterModel.tbe02));
                    sqlParms.Add(new SqlParameter("@tbe03", masterModel.tbe03));
                    chkCnts = GlobalFn.isNullRet(BoTax.OfGetFieldValue(sqlSelect, sqlParms.ToArray()), 0);
                    if (chkCnts > 1)
                    {
                        WfShowErrorMsg("該年月資料有重疊,請先查詢後再做修改!");
                        return false;
                    }

                    WfSetControlsReadOnlyRecursion(ute_tbe01.Parent, true);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreDeleteDetailCheck (): 刪除明細前檢查
        protected override bool WfPreDeleteDetailCheck(int pCurTabDetail, DataRow pDrDetail)
        {
            vw_taxi020s detailModel = null;
            try
            {
                detailModel = pDrDetail.ToItem<vw_taxi020s>();
                if (!GlobalFn.varIsNull(detailModel.tbe09))
                {
                    WfShowErrorMsg("已有開立發票,不可刪除!");
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
        #region WfAfterDetailDelete() :刪除明細後調用
        protected override bool WfAfterDetailDelete(int pCurTabDetail, DataRow pDr)
        {
            try
            {
                if (TabDetailList[0].DtSource.Rows.Count == 0)
                {
                    
                    WfSetControlsReadOnlyRecursion(ute_tbe01.Parent, false);
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
            vw_taxi020 masterModel = null;
            vw_taxi020s detailModel = null;
            List<vw_taxi020s> detailList = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab, chkCnts;
            try
            {
                masterModel = DrMaster.ToItem<vw_taxi020>();
                #region 單頭資料檢查
                chkColName = "tbe01";       //發票年度
                chkControl = ute_tbe01;
                if (GlobalFn.varIsNull(masterModel.tbe01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "tbe02";       //發票月份-起
                chkControl = ute_tbe02;
                if (GlobalFn.varIsNull(masterModel.tbe02))
                {                    
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "tbe03";       //發票月份-迄
                chkControl = ute_tbe03;
                if (GlobalFn.varIsNull(masterModel.tbe03))
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

                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                detailList = TabDetailList[iChkDetailTab].DtSource.ToList<vw_taxi020s>();
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    detailModel = drTemp.ToItem<vw_taxi020s>();
                    chkColName = "tbe04";   //發票別
                    if (GlobalFn.varIsNull(detailModel.tbe04))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "tbe05";   //發票別
                    if (GlobalFn.varIsNull(detailModel.tbe05))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }


                    chkColName = "tbe06";   //發票別
                    if (GlobalFn.varIsNull(detailModel.tbe06))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    //檢查發票別+聯數+簿號 在單身是否重覆
                    chkCnts = detailList.Where(p => GlobalFn.isNullRet(p.tbe04, "").ToUpper() == detailModel.tbe04.ToUpper()
                                                && GlobalFn.isNullRet(p.tbe05, "").ToUpper() == detailModel.tbe05.ToUpper()
                                                && p.tbe06 == detailModel.tbe06)
                                            .Count();
                    if (chkCnts > 1)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = string.Format("發票別+聯數+簿號有重覆,請檢核!");
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);                        
                        return false;
                    }
                    //檢查key值是否存在
                    if (drTemp.RowState == DataRowState.Added)
                    {
                        chkColName = "tbe04";
                        if (BoTax.OfChkTbePKExists(detailModel.tbe01, detailModel.tbe02, detailModel.tbe03,
                                        detailModel.tbe04, detailModel.tbe05, detailModel.tbe06) == true)
                        {
                            this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                            msg = string.Format("發票本已存在,請檢核!");
                            WfShowErrorMsg(msg);
                            WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        }
                    }

                    chkColName = "tbe07";   //起始發票號碼
                    if (GlobalFn.varIsNull(detailModel.tbe07))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "tbe08";   //截止發票號碼
                    if (GlobalFn.varIsNull(detailModel.tbe08))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    //檢查字軌是否相同
                    if (detailModel.tbe07.ToUpper().Substring(0, 2) !=
                            detailModel.tbe08.ToUpper().Substring(0, 2))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = "發票字軌不同，請檢核!";
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
            try
            {
                //填入系統資訊
                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["tbecreu"] = LoginInfo.UserNo;
                            drDetail["tbecreg"] = LoginInfo.DeptNo;
                            drDetail["tbecred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["tbemodu"] = LoginInfo.UserNo;
                            drDetail["tbemodg"] = LoginInfo.DeptNo;
                            drDetail["tbemodd"] = Now;
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

        #region WfChktbe04tbe05tbe06 檢查key值是否有重覆
        private bool WfChktbe04tbe05tbe06(string tbe04, string tbe05, decimal tbe06)
        {
            List<vw_taxi020s> detailList = null;
            int chkCnts = 0;
            try
            {
                detailList = TabDetailList[0].DtSource.ToList<vw_taxi020s>();
                chkCnts = detailList.Where(p => !GlobalFn.varIsNull(p.tbe04) && !GlobalFn.varIsNull(p.tbe05) && p.tbe06 > 0
                                            && p.tbe04 == tbe04 && p.tbe05 == tbe05 && p.tbe06 == tbe06
                                        ).Count();
                if (chkCnts > 1)
                {
                    WfShowErrorMsg("該發票別簿號已重覆,請檢核!");                    
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
        
        #region WfSetTotalCnts 設定發票總張數
        private void WfSetTotalCnts(DataRow drTbe)
        {
            vw_taxi020s detailModel = null;
            int begNo, endNo;
            try
            {
                detailModel = drTbe.ToItem<vw_taxi020s>();
                if (GlobalFn.varIsNull(detailModel.tbe07) || GlobalFn.varIsNull(detailModel.tbe08))
                {
                    drTbe["total_cnts"] = 0;
                    return;
                }
                begNo = Convert.ToInt32(detailModel.tbe07.Substring(2, 8));
                endNo = Convert.ToInt32(detailModel.tbe08.Substring(2, 8));
                drTbe["total_cnts"] = endNo - begNo+1;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
    }
}
