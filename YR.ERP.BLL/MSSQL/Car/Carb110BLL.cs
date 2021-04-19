/* 程式名稱: 應收帳款產生作業
   系統代號: carb110
   作　　者: Allen
   描　　述: 
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using YR.ERP.DAL.YRModel;
using YR.Util;
using System.Data.Common;
using YR.Util;
using System.Globalization;
using YR.ERP.Shared;
using YR.ERP.BLL.Model;


namespace YR.ERP.BLL.MSSQL.Car
{

    public class Carb110BLL : YR.ERP.BLL.MSSQL.CarBLL
    {
        private UserInfo _loginInfo = null;
        
        #region Carb110BLL() :　建構子
        public Carb110BLL()
            : base()
        {
        }

        public Carb110BLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfGenAR
        public List<Result> OfGenAR(vw_carb110 pModel, string pSecurityString, UserInfo pLoginInfo)
        {
            List<Result> rtnResultList = null;
            Result result = null;
            string selectSql = "";
            string strQueryRange = "", strQuerySingle = "";
            List<SqlParameter> sqlParmList = null;
            DataTable dtSource = null;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            int chkCnts = 0;
            try
            {
                _loginInfo = pLoginInfo;
                rtnResultList = new List<Result>();

                sqlParmList = new List<SqlParameter>();
                queryInfoList = new List<QueryInfo>();
                selectSql = @"
                            SELECT sga_tb.* FROM sga_tb
                            WHERE sgaconf='Y'
                        ";

                #region range 處理
                if (!GlobalFn.varIsNull(pModel.sga01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "sga_tb";
                    queryModel.ColumnName = "sga01";
                    queryModel.ColumnType = "string";
                    queryModel.Value = pModel.sga01;
                    queryInfoList.Add(queryModel);
                }
                
                strQueryRange = WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion
                                

                #region single處理
                if (!GlobalFn.varIsNull(pModel.sga02_s))
                {
                    sqlParmList.Add(new SqlParameter("@sga02_s", pModel.sga02_s));
                    strQuerySingle += string.Format(" AND sga02>=@sga02_s");
                }
                if (!GlobalFn.varIsNull(pModel.sga02_e))
                {
                    sqlParmList.Add(new SqlParameter("@sga02_e", pModel.sga02_e));
                    strQuerySingle += string.Format(" AND sga02<=@sga02_e");
                }
                #endregion

                selectSql = string.Concat(selectSql, strQueryRange, strQuerySingle, pSecurityString);       //加入權限處理
                dtSource = OfGetDataTable(selectSql, sqlParmList.ToArray());
                if (dtSource == null || dtSource.Rows.Count == 0)
                    return null;
                
                foreach (DataRow drTemp in dtSource.Rows)
                {
                    result = new Result();
                    rtnResultList.Add(result);

                    result.Success = true;
                }
                return rtnResultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion
        
        private bool OfGenGea(vw_carb110 pCarb110Model, sga_tb pSgaModel, Result pResult)
        {
            string selectSql = "", updateSql = "";
            List<SqlParameter> sqlParmList = null;
            StpBLL boStp = null;
            CarBLL boCar = null;
            BasBLL boBas = null;
            DataTable dtCea = null, dtCeb = null;
            DataRow drCea = null, drCeb = null;
            string cea01 = "", cea01New = "";
            DateTime? cea02;
            string errMsg = "";
            List<sgb_tb> sgbList = null;
            cac_tb cacModel = null; //收款單別
            bab_tb babModel = null; //出貨單別
            int chkCnts = 0;
            try
            {
                boCar = new CarBLL(OfGetConntion());
                boCar.TRAN = this.TRAN;
                boStp = new StpBLL(OfGetConntion());
                boStp.TRAN = this.TRAN;
                boBas = new BasBLL(OfGetConntion());
                boBas.TRAN = this.TRAN;
                
                
                boCar.OfCreateDao("cea_tb", "*", "");
                selectSql = @"SELECT * FROM cea_tb WHERE 1<>1";
                dtCea = boCar.OfGetDataTable(selectSql);
                drCea = dtCea.NewRow();
                //已輸入的為主
                if (!GlobalFn.varIsNull(pCarb110Model.cea01))
                {
                    cea01 = pCarb110Model.cea01;
                }
                else
                {
                    //以出貨單別設定的來拋轉
                    babModel = boBas.OfGetBabModel(pSgaModel.sga01);
                    if (babModel == null)
                    {
                        pResult.Key1 = pSgaModel.sga01;
                        pResult.Message = "取得出貨單別失敗!";
                        return false;
                    }
                    if (GlobalFn.varIsNull(babModel.bab09))
                    {
                        pResult.Key1 = pSgaModel.sga01;
                        pResult.Message = "未設定出貨單拋轉應收帳款單別,請先至單別資料設定!";
                        return false;
                    }
                    cea01 = babModel.bab09;
                }
                if (!GlobalFn.varIsNull(pCarb110Model.cea02))
                    cea02 = pCarb110Model.cea02;
                else
                    cea02 = pSgaModel.sga02;

                errMsg = "";
                if (boBas.OfGetAutoNo(cea01, ModuleType.car, (DateTime)cea02, out cea01New, out errMsg) == false)
                {
                    pResult.Key1 = pSgaModel.sga01;
                    pResult.Message = errMsg;
                    return false;
                }


                drCea["cea00"] = "11";                  //帳款類別
                drCea["cea01"] = cea01New;
                drCea["cea02"] = cea02;
                drCea["cea03"] = pSgaModel.sga03;
                drCea["cea04"] = pSgaModel.sga04;
                drCea["cea05"] = pSgaModel.sga05;        //業務部門
                drCea["cea06"] = pSgaModel.sga06;        //課稅別
                drCea["cea07"] = pSgaModel.sga07;        //稅率
                drCea["cea08"] = pSgaModel.sga08;        //含稅否
                drCea["cea09"] = pSgaModel.sga09;        //發票聯數
                drCea["cea10"] = pSgaModel.sga10;        //幣別
                drCea["cea11"] = pSgaModel.sga11;        //收款條件
                drCea["cea12"] = pSgaModel.sga21;        //匯率
                drCea["cea13"] = pSgaModel.sga13;        //原幣未稅金額
                drCea["cea13t"] = pSgaModel.sga13t;      //原幣含稅金額
                drCea["cea13g"] = pSgaModel.sga13g;      //原幣稅額
                drCea["cea14"] = 0;                      //原幣沖帳金額
                drCea["cea15"] = 0;         //本幣未稅金額
                drCea["cea15t"] = 0;        //本幣含稅金額
                drCea["cea15g"] = 0;        //本幣稅額
                drCea["cea16"] = 0;        //本幣沖帳金額
                drCea["cea17"] = "";        //參考單號
                drCea["cea18"] = "";        //備註
                drCea["cea19"] = DBNull.Value;        //應收款日
                drCea["cea20"] = "";        //保留
                drCea["cea21"] = "";        //科目類別
                drCea["cea22"] = "";        //會科編號
                drCea["cea23"] = "";        //發票別
                drCea["cea24"] = "";        //發票日期
                drCea["cea25"] = "";        //發票號碼
                drCea["cea26"] = "";        //發票客戶
                drCea["cea27"] = "";        //申報統編
                drCea["cea28"] = "";        //來源別
                drCea["cea29"] = "";        //傳票編號
                drCea["ceaconf"] = "N";
                drCea["ceacomp"] = _loginInfo.CompNo;
                drCea["ceacreu"] = _loginInfo.UserNo;
                drCea["ceacreg"] = _loginInfo.DeptNo;
                drCea["ceacred"] = OfGetNow();
                drCea["ceamodu"] = DBNull.Value;
                drCea["ceamodg"] = DBNull.Value;
                drCea["ceamodd"] = DBNull.Value;
                drCea["ceasecu"] = _loginInfo.UserNo;
                drCea["ceasecg"] = _loginInfo.GroupNo;
                dtCea.Rows.Add(drCea);

                if (boCar.OfUpdate(dtCea) == -1)
                {
                    pResult.Key1 = cea01New;
                    pResult.Message = "新增應收單頭失敗!";
                    return false;
                }

                boCar.OfCreateDao("cfb_tb", "*", "");
                selectSql = @"SELECT * FROM ceb_tb WHERE 1<>1";
                dtCeb = boCar.OfGetDataTable(selectSql);
                foreach (sgb_tb sgbModel in sgbList.OrderBy(p => p.sgb02))
                {
                    drCeb = dtCeb.NewRow();
                    drCeb["ceb00"] = "11";              //帳款類別
                    drCeb["ceb01"] = cea01New;          //傳票編號
                    drCeb["ceb02"] = sgbModel.sgb02;    //項次
                    drCeb["ceb03"] = sgbModel.sgb03;    //料號
                    drCeb["ceb04"] = sgbModel.sgb04;    //品名
                    drCeb["ceb05"] = sgbModel.sgb05;    //數量
                    drCeb["ceb06"] = sgbModel.sgb09;    //原幣單價
                    drCeb["ceb07"] = sgbModel.sgb10;    //原幣未稅金額
                    drCeb["ceb07t"] = sgbModel.sgb10t;  //原幣含稅金額
                    drCeb["ceb08"] = 0;    //本幣單價
                    drCeb["ceb09"] = 0;    //本未稅金額
                    drCeb["ceb09t"] = 0;    //本幣含稅金額
                    drCeb["ceb10"] = "";    //會計科目
                    drCeb["ceb11"] = pSgaModel.sga01;      //出貨單號
                    drCeb["ceb12"] = sgbModel.sgb02;      //出貨項次
                    drCeb["ceb13"] = 0;      //原幣已沖金額
                    drCeb["ceb14"] = 0;      //本幣已沖金額
                    drCeb["ceb15"] = 0;      //本幣未沖金額
                    drCeb["ceb16"] = sgbModel.sgb06;      //單位
                    drCeb["cebcomp"] = _loginInfo.CompNo;
                    drCeb["cebcreu"] = _loginInfo.UserNo;
                    drCeb["cebcreg"] = _loginInfo.DeptNo;
                    drCeb["cebcred"] = OfGetNow();
                    drCeb["cebmodu"] = DBNull.Value;
                    drCeb["cebmodg"] = DBNull.Value;
                    drCeb["cebmodd"] = DBNull.Value;
                    dtCeb.Rows.Add(drCeb);
                }

                if (boCar.OfUpdate(dtCeb) == -1)
                {
                    pResult.Key1 = cea01New;
                    pResult.Message = "新增應收單身失敗!";
                    return false;
                }

                updateSql = @"UPDATE sga_tb 
                            SET sga22=@sga22
                            WHERE sga01=@sga01
                            ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sga22", cea01New));
                sqlParmList.Add(new SqlParameter("@sga01", pSgaModel.sga01));
                chkCnts = OfExecuteNonquery(updateSql, sqlParmList.ToArray());
                if (chkCnts != 1)
                {
                    pResult.Key1 = pSgaModel.sga01;
                    pResult.Message = "更新出貨單失敗!";
                    return false;
                }

//                updateSql = @"UPDATE gea_tb 
//                            SET gea06=@gea06,
//                                gea07=@gea07
//                            WHERE gea01=@gea01
//                                 AND gea02=@gea02
//                                AND gea03=@gea03
//                                AND gea04=@gea04
//                            ";
//                sqlParmList = new List<SqlParameter>();
//                sqlParmList.Add(new SqlParameter("@gea06", cea01New));
//                sqlParmList.Add(new SqlParameter("@gea07", cea02));
//                sqlParmList.Add(new SqlParameter("@gea01", pGeaModel.gea01));
//                sqlParmList.Add(new SqlParameter("@gea02", pGeaModel.gea02));
//                sqlParmList.Add(new SqlParameter("@gea03", pGeaModel.gea03));
//                sqlParmList.Add(new SqlParameter("@gea04", pGeaModel.gea04));
//                chkCnts = OfExecuteNonquery(updateSql, sqlParmList.ToArray());
//                if (chkCnts != 1)
//                {
//                    pResult.Key1 = pGeaModel.gea01;
//                    pResult.Message = "更新分稿底稿失敗!";
//                    return false;
//                }

                return true;
            }
            catch (Exception ex)
            {
                pResult.Key1 = pSgaModel.sga01;
                pResult.Message = ex.Message;
                pResult.Exception = ex;
                throw;
            }
        }
    }
}
