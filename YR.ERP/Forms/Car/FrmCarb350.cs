/* 程式名稱: 應收傳票拋轉總帳作業
   系統代號: carb350
   作　　者: Allen
   描　　述: 

   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.BLL.MSSQL.Gla;
using YR.ERP.DAL.YRModel;
using YR.Util;
using System.Linq;
using YR.ERP.BLL.MSSQL.Car;

namespace YR.ERP.Forms.Car
{
    public partial class FrmCarb350 : YR.ERP.Base.Forms.FrmBatchBase
    {

        #region Property
        Carb350BLL BoCarb350;
        #endregion

        #region 建構子
        public FrmCarb350()
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
            // 繼承的表單要覆寫, 更改參數值            
            /*
            this.strFormID = "XXX";
             */
            this.StrFormID = "carb350";

            return true;
        }
        #endregion
        
        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "ceasecu";
            TabMaster.GroupColumn = "ceasecg";
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            BoCarb350 = new Carb350BLL(BoMaster.OfGetConntion());
            return;
        }
        #endregion

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {

                //排序
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.應收"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.收款"));
                WfSetUcomboxDataSource(ucb_gea03, sourceList);
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
            vw_carb350 masterModel = null;
            try
            {
                MessageInfo messageModel = new MessageInfo();
                switch (pColName.ToLower())
                {
                    case "gea01":       //底稿單號
                        masterModel=pDr.ToItem<vw_carb350>();
                        if (masterModel.gea03==0)
                        {
                            WfShowErrorMsg("請先輸入類別資料");
                            ucb_gea03.Focus();
                            return false;
                        }
                        messageModel.StrMultiColumn = "cea01";
                        messageModel.IntMaxRow = 999;
                        //messageModel.StrWhereAppend = " AND gfapost='N'";
                        WfShowPickUtility("p_cea3", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            pDr[pColName] = messageModel.StrMultiRtn;
                        break;
                    case "gfa01"://拋轉傳票單別
                        WfShowPickUtility("p_gac1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            if (messageModel.DataRowList.Count > 0)
                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["gac01"], "");
                            else
                                pDr[pColName] = "";
                        }
                        break;
                    case "ceasecg"://群組
                        messageModel.StrMultiColumn = "beb01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_beb1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            pDr[pColName] = messageModel.StrMultiRtn;
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

        #region WfFormCheck() 執行批次前檢查
        protected override bool WfFormCheck()
        {
            vw_carb350 masterModel = null;
            string msg = "";
            try
            {
                masterModel = DrMaster.ToItem<vw_carb350>();
                if (GlobalFn.varIsNull(masterModel.gfa01))
                {
                    msg = "傳票傳票單別不可為空白!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_gfa01, msg);
                    return false;
                }

                if (GlobalFn.varIsNull(masterModel.gea03))
                {
                    msg = "類別不可為空白!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(udt_gfa05_e, msg);
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

        #region WfExecute 批次執行開始
        protected override bool WfExecute()
        {
            vw_carb350 masterModel = null;
            int chkCnts = 0;
            StringBuilder sbResult = null;
            try
            {
                //取得交易物件
                BoMaster.TRAN = BoMaster.OfGetConntion().BeginTransaction();
                BoCarb350.TRAN = BoMaster.TRAN;

                masterModel = DrMaster.ToItem<vw_carb350>();
                var securityString = WfGetSecurityString();
                var resultList = BoCarb350.OfGenVoucher(masterModel,  securityString, LoginInfo);

                if (resultList == null || resultList.Count == 0)
                {
                    BoCarb350.TRAN.Rollback();
                    WfShowBottomStatusMsg("無可拋轉傳票資料!");
                    return false;
                }

                chkCnts = resultList.Where(p => p.Success == false).Count();
                if (chkCnts > 0)
                {
                    BoCarb350.TRAN.Rollback();
                    sbResult = new StringBuilder();
                    sbResult.AppendLine(string.Format("執行失敗!"));
                    sbResult.AppendLine(string.Format("拋轉傳票筆數【{0}】 成功:【{1}】  失敗【{2}】",
                                                            resultList.Count,
                                                            resultList.Count - chkCnts,
                                                            chkCnts
                                                            ));
                    sbResult.AppendLine();
                    sbResult.AppendLine(string.Format("錯誤內容如下"));
                    sbResult.AppendLine("====================================");
                    foreach (Result result in resultList.Where(p => p.Success == false))
                    {
                        sbResult.AppendLine(string.Format("key1:【{0}】 錯誤訊息:【{1}】", result.Key1, result.Message));
                    }
                    WfShowErrorMsg(sbResult.ToString());
                    return false;
                }
                BoMaster.TRAN.Commit();
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
