/* 程式名稱: 應收/發票拋轉作業
   系統代號: carb110
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
    public partial class FrmCarb110 : YR.ERP.Base.Forms.FrmBatchBase
    {

        #region Property
        Carb110BLL BoCarb110;
        #endregion

        #region MyRegion
        public FrmCarb110()
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
            this.StrFormID = "carb110";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "sgasecu";
            TabMaster.GroupColumn = "sgasecg";
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            BoCarb110 = new Carb110BLL(BoMaster.OfGetConntion());
            return;
        }
        #endregion

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                MessageInfo messageModel = new MessageInfo();
                switch (pColName.ToLower())
                {
                    case "cea01":       //應收單號
                        messageModel.ParamSearchList = new List<SqlParameter>();
                        messageModel.IsAutoQuery = true;
                        messageModel.ParamSearchList.Add(new SqlParameter("@cac03", "car"));
                        messageModel.ParamSearchList.Add(new SqlParameter("@cac04", "11"));
                        WfShowPickUtility("p_cac1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            if (messageModel.DataRowList.Count > 0)
                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["cac01"], "");
                            else
                                pDr[pColName] = "";
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

        #region WfFormCheck() 執行批次前檢查
        protected override bool WfFormCheck()
        {
            vw_carb110 masterModel = null;
            string msg = "";
            try
            {
                masterModel = DrMaster.ToItem<vw_carb110>();
                if (GlobalFn.varIsNull(masterModel.sga01))
                {
                    msg = "出貨單不可為空白!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_sga01, msg);
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
            vw_carb110 masterModel = null;
            int chkCnts = 0;
            StringBuilder sbResult = null;
            try
            {
                //取得交易物件
                BoMaster.TRAN = BoMaster.OfGetConntion().BeginTransaction();
                BoCarb110.TRAN = BoMaster.TRAN;

                masterModel = DrMaster.ToItem<vw_carb110>();
                var securityString = WfGetSecurityString();
                var resultList = BoCarb110.OfGenAR(masterModel, securityString, LoginInfo);
                
                if (resultList == null || resultList.Count == 0)
                {
                    BoCarb110.TRAN.Rollback();
                    WfShowBottomStatusMsg("無可拋轉出貨單資料!");
                    return false;
                }
                
                chkCnts = resultList.Where(p => p.Success == false).Count();
                if (chkCnts > 0)
                {
                    BoCarb110.TRAN.Rollback();
                    sbResult = new StringBuilder();
                    sbResult.AppendLine(string.Format("執行失敗!"));
                    sbResult.AppendLine(string.Format("拋轉應收筆數【{0}】 成功:【{1}】  失敗【{2}】",
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
