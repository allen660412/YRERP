/* 程式名稱: 年度結轉作業
   系統代號: glab321
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
    public partial class FrmGlab321 : YR.ERP.Base.Forms.FrmBatchBase
    {

        #region Property
        Glab321BLL BoGlab321;
        #endregion

        #region 建構子
        public FrmGlab321()
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
            this.StrFormID = "glab321";

            return true;
        }
        #endregion



        #region WfSetMasterRowDefault(DataRow pDr) 設定MasterRow的初始值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                var dtToday = Today;

                pDr["gfa08"] = dtToday.Year;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            vw_glab321 glab321Model;
            try
            {
                glab321Model = DrMaster.ToItem<vw_glab321>();
                switch (e.Column.ToLower())
                {
                    case "gfa08":
                        if (GlobalFn.varIsNull(e.Value))
                        {
                            WfShowErrorMsg("會計年度不可為空白,請檢核!");
                            return false;
                        }

                        if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                        {
                            WfShowErrorMsg("會計年度應大於0,請檢核!");
                            return false;
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
        
        #region WfFormCheck() 執行報表前檢查
        protected override bool WfFormCheck()
        {
            vw_glab321 masterModel = null;
            string msg = "";
            try
            {
                masterModel = DrMaster.ToItem<vw_glab321>();
                if (GlobalFn.varIsNull(masterModel.gfa08))
                {
                    msg = "會計年度不可為空白!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_gfa08, msg);
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
            vw_glab321 glab312Model = null;
            try
            {
                glab312Model = DrMaster.ToItem<vw_glab321>();
                //取得交易物件
                BoMaster.TRAN = BoMaster.OfGetConntion().BeginTransaction();
                BoGlab321.TRAN = BoMaster.TRAN;

                var result = BoGlab321.OfGlab321Post(glab312Model, LoginInfo);
                if (result == null)
                {
                    WfShowBottomStatusMsg("查無可年結資料!");
                    BoMaster.TRAN.Rollback();
                    return true;
                }
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    BoMaster.TRAN.Rollback();
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
