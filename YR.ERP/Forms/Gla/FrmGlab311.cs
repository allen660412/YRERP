/* 程式名稱: 未過帳傳票整批過帳作業
   系統代號: glab311
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

namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlab311 : YR.ERP.Base.Forms.FrmBatchBase
    {

        #region Property
        Glab311BLL BoGlab311;
        #endregion

        #region 建構子
        public FrmGlab311()
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
            this.StrFormID = "glab311";
            
            return true;
        }
        #endregion
        
        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "gfasecu";
            TabMaster.GroupColumn = "gfasecg";
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            BoGlab311 = new Glab311BLL(BoMaster.OfGetConntion());
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
                    case "gfa01":       //傳票單號
                        messageModel.StrMultiColumn = "gfa01";
                        messageModel.IntMaxRow = 999;
                        messageModel.StrWhereAppend = " AND gfapost='N'";
                        WfShowPickUtility("p_gfa1", messageModel);
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

        #region WfFormCheck() 執行報表前檢查
        protected override bool WfFormCheck()
        {
            vw_glab311 masterModel = null;
            string msg = "";
            try
            {
                masterModel = DrMaster.ToItem<vw_glab311>();
                if (GlobalFn.varIsNull(masterModel.gfa02_s))
                {
                    msg = "傳票起日不可為空白!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(udt_gfa02_s, msg);
                    return false;
                }

                if (GlobalFn.varIsNull(masterModel.gfa02_s))
                {
                    msg = "傳票迄日不可為空白!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(udt_gfa02_e, msg);
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
            vw_glab311 masterModel = null;
            int chkCnts = 0;
            StringBuilder sbResult = null;
            try
            {
                //取得交易物件
                BoMaster.TRAN = BoMaster.OfGetConntion().BeginTransaction();
                BoGlab311.TRAN = BoMaster.TRAN;

                masterModel = DrMaster.ToItem<vw_glab311>();
                var securityString = WfGetSecurityString();
                var resultList = BoGlab311.OfGlab311Post(masterModel, "N", securityString,"",LoginInfo);

                if (resultList == null || resultList.Count == 0)
                {
                    BoGlab311.TRAN.Rollback();
                    WfShowBottomStatusMsg("無可過帳資料!");
                    return true;
                }

                chkCnts = resultList.Where(p => p.Success == false).Count();
                if (chkCnts > 0)
                {
                    BoGlab311.TRAN.Rollback();
                    sbResult = new StringBuilder();
                    sbResult.AppendLine(string.Format("執行失敗!"));
                    sbResult.AppendLine(string.Format("過帳傳票筆數【{0}】 成功:【{1}】  失敗【{2}】", 
                                                            resultList.Count,
                                                            resultList.Count - chkCnts,
                                                            chkCnts
                                                            ));
                    sbResult.AppendLine();
                    sbResult.AppendLine(string.Format("錯誤內容如下"));
                    sbResult.AppendLine("====================================");
                    foreach(Result result in resultList.Where(p=>p.Success==false))
                    {
                        sbResult.AppendLine(string.Format("key1:【{0}】 錯誤訊息:【{1}】", result.Key1,result.Message));
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
