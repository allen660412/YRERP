/* 程式名稱: 料件多原廠料號管理
   系統代號: invi103
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
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvi103 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        #endregion

        #region 建構子
        public FrmInvi103()
        {
            InitializeComponent();
        }

        public FrmInvi103(string pSourceForm, YR.ERP.Shared.UserInfo pUserInfo, string pWhere)
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
            this.StrFormID = "invi103";
            this.IntTabCount = 2;
            this.IntMasterGridPos = 2;
            TabMaster.IsReadOnly = true;    //單頭不做存檔
            uTab_Master.Tabs[0].Text = "資料內容";
            //uTab_Master.Tabs[1].Text = "狀態";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ica01", SqlDbType.NVarChar)
                                });

                TabMaster.UserColumn = "icasecu";
                TabMaster.GroupColumn = "icasecg";
                TabMaster.CanAddMode = false;
                TabMaster.CanCopyMode = false;
                TabMaster.CanDeleteMode = false;
                TabMaster.CanUseRowLock = false;    //不Lock主表
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
            BoInv = new InvBLL(BoMaster.OfGetConntion());
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
                    BoInv.TRAN = BoMaster.TRAN;
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
            try
            {

                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(uTab_Master, true);
                    WfSetControlsReadOnlyRecursion(uTab_Detail, false);  //先全開其餘的由WfSetDetailDisplayMode處理
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
                            ////先控可以輸入的
                            //if (columnName == "icn04" ||
                            //    columnName == "icn05" ||
                            //    columnName == "icn06" ||
                            //    columnName == "icnvali"
                            //    )
                            //{
                            //    WfSetControlReadonly(ugc, false);
                            //    continue;
                            //}

                            //if (columnName == "icn02" || columnName == "icn03")
                            //{
                            //    if (pDr.RowState == DataRowState.Added)//新增時
                            //        WfSetControlReadonly(ugc, false);
                            //    else    //修改時
                            //    {
                            //        WfSetControlReadonly(ugc, true);
                            //    }
                            //    continue;
                            //}
                            //WfSetControlReadonly(ugc, true);
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
            SqlParameter keyParm;

            this.TabDetailList[0].TargetTable = "icd_tb";
            this.TabDetailList[0].ViewTable = "vw_invi103s";
            keyParm = new SqlParameter("icd01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "ica01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
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
                        pDr["icd01"] = DrMaster["ica01"];
                        //pDr["icn04"] = 0;
                        //pDr["icn05"] = 0;
                        //pDr["icnvali"] = "Y";
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

        #region WfSeDetailGridLayout
        protected override void WfSeDetailGridLayout(UltraGrid pUgrid)
        {
            Infragistics.Win.UltraWinGrid.UltraGridColumn ugc;
            try
            {
                //if (pUgrid.DisplayLayout.Bands[0].Columns.Exists("icnvali"))
                //{
                //    ugc = pUgrid.DisplayLayout.Bands[0].Columns["icnvali"];
                //    WfSetUgridCheckBox(ugc);
                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
