using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL.Csp;
using YR.ERP.BLL.MSSQL.Tax;

namespace YR.ERP.Forms.Tax
{
    public partial class FrmTaxb010 : YR.ERP.Base.Forms.FrmBatchBase
    {

        #region Property
        Taxb010BLL BoTaxb010;
        #endregion

        public FrmTaxb010()
        {
            InitializeComponent();
        }

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
            this.StrFormID = "taxb010";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            //todo:不處理執行範圍權限,要測試
            //TabMaster.UserColumn = "seasecu";
            //TabMaster.GroupColumn = "seasecg";
        }
        #endregion


        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            BoTaxb010 = new Taxb010BLL(BoMaster.OfGetConntion());
            return;

        }
        #endregion

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                //調撥類型
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("0", "0.全部"));
                sourceList.Add(new KeyValuePair<string, string>("1", "1.進項"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.銷項"));
                WfSetUcomboxDataSource(ucb_tca01, sourceList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMasterRowDefault(DataRow pDr) 設定MasterRow的初始值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            decimal dMonth=0;
            string year="";
            string tca03 = "", tca04 = "";
            try
            {
                pDr["tca01"] = "0";

                
                year = Today.Year.ToString();
                dMonth = Today.Month;
                if (dMonth % 2 ==1) //表示奇數月
                {

                    tca03 = year + dMonth.ToString().PadLeft(2, '0');
                    tca04 = year + (dMonth+1).ToString().PadLeft(2, '0');
                }
                else
                {
                    tca04 = year + dMonth.ToString().PadLeft(2, '0');
                    tca03 = year + (dMonth - 1).ToString().PadLeft(2, '0');
                }
                pDr["tca03"] = tca03;
                pDr["tca04"] = tca04;
                

                //pDr["jja06"] = Today;
                pDr["delete_yn"] = "N";
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
            //try
            //{
            //    MessageInfo messageModel = new MessageInfo();
            //    switch (pColName.ToLower())
            //    {
            //        case "icc01":       //料號
            //            messageModel.StrMultiColumn = "ica01";
            //            messageModel.IntMaxRow = 999;
            //            WfShowPickUtility("p_ica1", messageModel);
            //            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
            //                pDr[pColName] = messageModel.StrMultiRtn;
            //            break;
            //        case "icc02":       //倉庫
            //            messageModel.StrMultiColumn = "icb01";
            //            messageModel.IntMaxRow = 999;
            //            WfShowPickUtility("p_icb1", messageModel);
            //            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
            //                pDr[pColName] = messageModel.StrMultiRtn;
            //            break;
            //    }

            return true;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        #endregion

        #region WfExecute 批次執行開始
        protected override bool WfExecute()
        {
            vw_taxb010 masterModel = null;
            int chkCnts = 0;
            StringBuilder sbResult = null;
            try
            {
                //取得交易物件
                BoMaster.TRAN = BoMaster.OfGetConntion().BeginTransaction();
                BoTaxb010.TRAN = BoMaster.TRAN;

                masterModel = DrMaster.ToItem<vw_taxb010>();
                var securityString = WfGetSecurityString();
                var resultList = BoTaxb010.OfGenInv(masterModel, LoginInfo);

                //if (resultList == null || resultList.Count == 0)
                //{
                //    BoCspb200.TRAN.Rollback();
                //    WfShowBottomStatusMsg("無可產生成本資料!");
                //    return false;
                //}

                chkCnts = resultList.Where(p => p.Success == false).Count();
                if (chkCnts > 0)
                {
                    BoTaxb010.TRAN.Rollback();
                    sbResult = new StringBuilder();
                    sbResult.AppendLine(string.Format("執行失敗!"));
                    sbResult.AppendLine(string.Format("拋轉成本筆數【{0}】 成功:【{1}】  失敗【{2}】",
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
                BoMaster.TRAN.Rollback();
                throw ex;
            }
        }
        #endregion

    }
}
