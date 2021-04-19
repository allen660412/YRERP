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

namespace YR.ERP.Forms.Stp
{
    public partial class FrmStpb400 : YR.ERP.Base.Forms.FrmBatchBase
    {

        #region Property
        StpBLL BoStp;
        #endregion

        #region 建構子
        public FrmStpb400()
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
            this.StrFormID = "stpb400";

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
            BoStp = new StpBLL(BoMaster.OfGetConntion());
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
                    case "sga01":       //出貨單號
                        messageModel.StrMultiColumn = "sga01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_sga1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            pDr[pColName] = messageModel.StrMultiRtn;
                        break;
                    case "sga03":       //客戶編號
                        messageModel.StrMultiColumn = "sga03";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_ica1", messageModel);
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

        #region WfExecute 批次執行開始
        protected override bool WfExecute()
        {
            vw_invb600 invb600Model;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            List<SqlParameter> sqlParmList;
            string strQueryRange, strWhere;
            DataTable dtMain;
            StringBuilder sbSql;
            int chkCnts = 0;
            try
            {

                //取得交易物件
                BoMaster.TRAN = BoMaster.OfGetConntion().BeginTransaction();
                BoStp.TRAN = BoMaster.TRAN;

                invb600Model = DrMaster.ToItem<vw_invb600>();
                #region range 查詢條件
                queryInfoList = new List<QueryInfo>();
                if (!GlobalFn.varIsNull(invb600Model.icc01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "icc_tb";
                    queryModel.ColumnName = "icc01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["icc01"].DataType.Name;
                    queryModel.Value = invb600Model.icc01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(invb600Model.icc02))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "icc_tb";
                    queryModel.ColumnName = "icc02";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["icc02"].DataType.Name;
                    queryModel.Value = invb600Model.icc02;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                strWhere = strQueryRange;
                #endregion

                var strSecurity = WfGetSecurityString();    //暫時無權限問題,先保留
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icc_tb");
                sbSql.AppendLine("WHERE 1=1");
                dtMain = BoMaster.OfGetDataTable(string.Concat(sbSql.ToString(), strWhere), sqlParmList.ToArray());
                dtMain.TableName = "Master";

                if (dtMain == null || dtMain.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    BoMaster.TRAN.Rollback();
                    return false;
                }

                foreach (DataRow drIcc in dtMain.Rows)
                {
                    var iccModel = drIcc.ToItem<vw_invb600>();
                    //取得ina_tb庫存歷史檔資料!
                    sbSql = new StringBuilder();
                    sbSql.AppendLine(string.Format("SELECT SUM(CASE WHEN ina03='1' THEN ina10 ELSE ina10*-1 END)"));
                    sbSql.AppendLine(string.Format("FROM ina_tb"));
                    sbSql.AppendLine(string.Format("WHERE ina05=@ina05"));
                    sbSql.AppendLine(string.Format("AND ina06=@ina06"));
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@ina05", iccModel.icc01));
                    sqlParmList.Add(new SqlParameter("@ina06", iccModel.icc02));
                    var compueQty = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);

                    sbSql = new StringBuilder();
                    sbSql.AppendLine(string.Format("UPDATE icc_tb"));
                    sbSql.AppendLine(string.Format("SET icc05=@icc05"));
                    sbSql.AppendLine(string.Format("WHERE icc01=@icc01"));
                    sbSql.AppendLine(string.Format("AND icc02=@icc02"));
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@icc01", iccModel.icc01));
                    sqlParmList.Add(new SqlParameter("@icc02", iccModel.icc02));
                    sqlParmList.Add(new SqlParameter("@icc05", compueQty));
                    chkCnts = BoStp.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray());
                    if (chkCnts <= 0)
                    {
                        WfShowErrorMsg("查無可異動icc_tb資料表,請檢核!");
                        BoMaster.TRAN.Rollback();

                        return false;
                    }
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
