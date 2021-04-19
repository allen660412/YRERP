/* 程式名稱: 料號圖檔建立作業
   系統代號: invi102
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
using Infragistics.Win.UltraWinToolbars;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;
using System.IO;
using System.Drawing;
using Infragistics.Win.UltraWinGrid;

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvi102 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        #endregion

        #region 建構子
        public FrmInvi102()
        {
            InitializeComponent();
        }

        public FrmInvi102(string pSourceForm, YR.ERP.Shared.UserInfo pUserInfo, string pWhere)
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
            this.StrFormID = "invi102";

            this.IntTabCount = 2;
            this.IntMasterGridPos = 2;
            uTab_Master.Tabs[0].Text = "一般資料";
            uTab_Master.Tabs[1].Text = "基本資料";
            uTab_Master.Tabs[2].Text = "資料瀏覽";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] 
                                            { new SqlParameter("icp01", SqlDbType.NVarChar) ,
                                              new SqlParameter("icp02", SqlDbType.Decimal) ,
                                            });
                TabMaster.UserColumn = "icpsecu";
                TabMaster.GroupColumn = "icpsecg";
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

            BoInv = new InvBLL(BoMaster.OfGetConntion());
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

                    BoInv.TRAN = BoMaster.TRAN;
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMasterGridLayout
        protected override void WfSetMasterGridLayout()
        {
            UltraGrid uGrid;
            UltraGridColumn uColumn ;
            try
            {
                uGrid = TabMaster.UGrid;
                uGrid.DisplayLayout.Override.DefaultRowHeight = 100;


                uColumn = uGrid.DisplayLayout.Bands[0].Columns["icp03"];
                uColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
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
            vw_invi102 masterModel;
            try
            {
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else
                {
                    masterModel = DrMaster.ToItem<vw_invi102>();
                    WfSetControlsReadOnlyRecursion(this, false);
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(ute_icp01_c, true);

                    if (GlobalFn.isNullRet(masterModel.icp06, "") == "Y")
                        WfSetControlReadonly(ute_icp05, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_icp01, true);
                        WfSetControlReadonly(ute_icp02, true);
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
                pDr["icp02"] = 0;
                pDr["icp05"] = 999;
                pDr["icp06"] = "N";
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
            string errMsg;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_invi100
                if (pDr.Table.Prefix.ToLower() == "vw_invi102")
                {
                    switch (pColName.ToLower())
                    {
                        case "icp01"://料號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
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
            vw_invi102 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_invi102>();
                #region 單頭-pick vw_invi102
                if (e.Row.Table.Prefix.ToLower() == "vw_invi102")
                {
                    switch (e.Column.ToLower())
                    {
                        case "icp01"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["icp01_c"] = "";
                                return true;
                            }
                            var icaModel = BoInv.OfGetIcaModel(e.Value.ToString());
                            if (icaModel == null)
                            {
                                WfShowErrorMsg("查無此料號!");
                                return false;
                            }
                            if (icaModel.icaconf != "Y")
                            {
                                WfShowErrorMsg("料號未確認!");
                                return false;
                            }
                            if (icaModel.icavali == "N")
                            {
                                WfShowErrorMsg("此為無效料號!");
                                return false;
                            }
                            e.Row["icp01_c"] = icaModel.ica02;
                            if (masterModel.icp02 == 0)
                                WfSetIcp02(e.Value.ToString());
                            break;
                        case "icp02"://項次
                            if (GlobalFn.varIsNull(masterModel.icp01))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                return false;
                            }
                            if (!GlobalFn.isNumeric(e.Value.ToString()))
                            {
                                WfShowErrorMsg("請輸入數字!");
                                return false;
                            }
                            if (FormEditMode == YREditType.新增)
                            {
                                if (BoInv.OfChkIcpPKExists(masterModel.icp01, Convert.ToInt32(e.Value)))
                                {
                                    WfShowErrorMsg("此項次已存在!");
                                    return false;
                                }
                            }
                            break;

                        case "icp06":   //預設圖片
                            if (GlobalFn.isNullRet(e.Value, "") == "Y")
                            {
                                e.Row["icp05"] = 0;
                                WfSetControlReadonly(ute_icp05, true);
                            }
                            else
                            {
                                e.Row["icp05"] = 999;
                                WfSetControlReadonly(ute_icp05, false);
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

        #region WfAppendUpdate 存檔後處理額外的資料表 新增、修改、刪除...
        protected override bool WfAppendUpdate()
        {
            string updateSql;
            vw_invi102 masterModel;
            List<SqlParameter> sqlParmsList;
            try
            {
                masterModel=DrMaster.ToItem<vw_invi102>();
                if (masterModel.icp06=="Y")
                {
                    updateSql = @"
                                UPDATE icp_tb
                                SET icp06='N'                                        
                                WHERE icp01=@icp01
                                      AND icp02<>@icp02
                               ";
                    sqlParmsList = new List<SqlParameter>();
                    sqlParmsList.Add(new SqlParameter("icp01", masterModel.icp01));
                    sqlParmsList.Add(new SqlParameter("icp02", masterModel.icp02));

                    if (BoInv.OfExecuteNonquery(updateSql,sqlParmsList.ToArray())<0)
                    {
                        WfShowErrorMsg("WfAppendUpdate 更新 icp_tb 失敗!");
                        return false;
                    }

                    updateSql = @"
                                UPDATE icp_tb
                                SET icp05=999                                     
                                WHERE icp01=@icp01
                                      AND icp02<>@icp02
                                      AND icp05=0
                               ";
                    sqlParmsList = new List<SqlParameter>();
                    sqlParmsList.Add(new SqlParameter("icp01", masterModel.icp01));
                    sqlParmsList.Add(new SqlParameter("icp02", masterModel.icp02));

                    if (BoInv.OfExecuteNonquery(updateSql, sqlParmsList.ToArray()) < 0)
                    {
                        WfShowErrorMsg("WfAppendUpdate 更新 icp_tb 失敗!");
                        return false;
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

        //*****************************表單自訂Fuction****************************************

        #region btn_getPicure_Click 按下取得圖檔
        private void btn_getPicure_Click(object sender, EventArgs e)
        {
            if (FormEditMode == YREditType.NA)
                return;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "請選擇圖檔";
            dialog.InitialDirectory = ".\\";
            dialog.Filter = "Image Files (*.jpg, *.bmp, *.png)|*.jpg; *.bmp;*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                WfLoadImage(dialog.FileName);
            }
        }
        #endregion

        #region WfLoadImage 產生圖檔,並寫入至datarow
        private void WfLoadImage(string pImagePath)
        {
            try
            {
                if (pImagePath != "")
                {
                    if (System.IO.File.Exists(pImagePath))
                    {
                        Image img = Image.FromFile(pImagePath);
                        DrMaster["icp03"] = GlobalPictuer.GetBytesFromImage(img);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfSetIcp02 設定icp02
        private void WfSetIcp02(string icp01)
        {
            string sqlSelect;
            List<SqlParameter> sqlParms = null;
            int maxIcp02;
            try
            {
                sqlSelect = @"SELECT MAX(icp02) FROM icp_tb 
                            WHERE icp01=@icp01 
                           ";
                sqlParms = new List<SqlParameter>();
                sqlParms.Add(new SqlParameter("@icp01", icp01));
                maxIcp02 = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sqlSelect, sqlParms.ToArray()), 0);
                maxIcp02 += 1;
                DrMaster["icp02"] = maxIcp02;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
