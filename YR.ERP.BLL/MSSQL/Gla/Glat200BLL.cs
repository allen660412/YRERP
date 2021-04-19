/* 程式名稱: 分錄底稿產生相關程式
   系統代號: glat200
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
using System.Data.Common;

namespace YR.ERP.BLL.MSSQL.Gla
{
    public class Glat200BLL : YR.ERP.BLL.MSSQL.GlaBLL
    {
        #region Glat200BLL() :　建構子
        public Glat200BLL()
            : base()
        {
        }

        public Glat200BLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfGenGeaByCea 分錄底稿產生作業 應收帳款--先處理一般出貨
        public List<Result> OfGenGeaByCea(string pCea01, UserInfo pLoginInfo)
        {
            List<Result> rtnResultList = null;
            Result result = null;
            cea_tb ceaModel = null;
            List<ceb_tb> cebList = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            CarBLL boCar = null;
            StpBLL boStp = null;
            BasBLL boBas = null;
            DataTable dtGea = null, dtGeb = null;
            DataRow drGea = null, drGeb = null;
            decimal gea08 = 0, gea09 = 0;
            int i;
            baa_tb baaModel = null;
            gba_tb gbaModel = null;
            cba_tb cbaModel = null;
            try
            {
                rtnResultList = new List<Result>();
                boBas = new BasBLL(this.OfGetConntion());
                boBas.TRAN = this.TRAN;
                boCar = new CarBLL(this.OfGetConntion());
                boCar.TRAN = this.TRAN;
                boStp = new StpBLL(this.OfGetConntion());
                boStp.TRAN = this.TRAN;

                baaModel = boBas.OfGetBaaModel();
                if (baaModel==null || GlobalFn.varIsNull(baaModel.baa04))
                {
                    result = new Result();
                    result.Key1 = pCea01;
                    result.Message = "查無本國幣別!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }
                
                ceaModel = boCar.OfGetCeaModel(pCea01);
                if (ceaModel == null)
                {
                    result = new Result();
                    result.Key1 = pCea01;
                    result.Message = "查無此應收單號!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }
                if (ceaModel.ceaconf != "N")
                {
                    result = new Result();
                    result.Key1 = pCea01;
                    result.Message = "應收帳款非未確認狀態!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }
                cbaModel = boCar.OfGetCbaModel(ceaModel.cea21);
                if (cbaModel==null)
                {
                    result = new Result();
                    result.Key1 = pCea01;
                    result.Message = "查無應收科目類別資料!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }
                if (GlobalFn.varIsNull(cbaModel.cba05))
                {
                    result = new Result();
                    result.Key1 = pCea01;
                    result.Message = "查無銷項稅額科目!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }

                cebList = boCar.OfGetCebList(pCea01);
                if (cebList == null || cebList.Count == 0)
                {
                    result = new Result();
                    result.Key1 = pCea01;
                    result.Message = "應收帳款無單身資料!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }
                //新增底稿單頭
                this.OfCreateDao("gea_tb", "*", "");
                sqlSelect = "SELECT * FROM gea_tb WHERE 1<>1 ";
                dtGea = this.OfGetDataTable(sqlSelect);
                drGea = dtGea.NewRow();
                drGea["gea01"] = ceaModel.cea01;    //底稿單號
                drGea["gea02"] = "AR";  //系統別
                drGea["gea03"] = 1;     //1.應收 2.收款
                drGea["gea04"] = 1;     //AR 固定為1
                drGea["gea05"] = ceaModel.cea02;    //同帳款日期
                drGea["gea06"] = DBNull.Value;    //傳票號碼
                drGea["gea07"] = DBNull.Value;    //傳票日期
                drGea["gea08"] = ceaModel.cea15t;               //借方金額
                var ceb09t_tot = cebList.Sum(p => p.ceb09t);
                drGea["gea09"] = ceb09t_tot;                 //貸方金額
                drGea["gea10"] = DBNull.Value;      //保留
                drGea["gea11"] = DBNull.Value;      //保留
                drGea["gea12"] = DBNull.Value;      //保留
                drGea["geaprno"] = 0;
                drGea["geacomp"] = pLoginInfo.CompNo;
                drGea["geacreu"] = pLoginInfo.UserNo;
                drGea["geacreg"] = pLoginInfo.DeptNo;
                drGea["geacred"] = OfGetNow();
                drGea["geamodu"] = DBNull.Value;
                drGea["geamodg"] = DBNull.Value;
                drGea["geamodd"] = DBNull.Value;
                drGea["geasecu"] = pLoginInfo.UserNo;
                drGea["geasecg"] = pLoginInfo.GroupNo;
                dtGea.Rows.Add(drGea);
                if (this.OfUpdate(dtGea) == -1)
                {
                    result = new Result();
                    result.Key1 = pCea01;
                    result.Message = "新增分錄底稿單頭失敗!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }

                //新增底稿單身
                this.OfCreateDao("geb_tb", "*", "");
                sqlSelect = "SELECT * FROM geb_tb WHERE 1<>1 ";
                dtGeb = this.OfGetDataTable(sqlSelect);

                //借方帶單頭應收帳款
                //用本幣匯總成一筆來加總
                drGeb = dtGeb.NewRow();
                i=1;
                drGeb = dtGeb.NewRow();
                drGeb["geb01"] = ceaModel.cea01;        //底稿單頭
                drGeb["geb02"] = "AR";
                drGeb["geb03"] = 1;                     //1.應收 2.收款
                drGeb["geb04"] = 1;                     //AR 固定為1
                drGeb["geb05"] = i;                     //項次
                drGeb["geb06"] = ceaModel.cea22;        //會計科目-待補入
                drGeb["geb07"] = "";                    //摘要
                drGeb["geb08"] = DBNull.Value;          //部門
                if (!GlobalFn.varIsNull(ceaModel.cea22))
                {
                    gbaModel = OfGetGbaModel(ceaModel.cea22);
                    if (gbaModel != null && gbaModel.gba09 == "Y")
                        drGeb["geb08"] = ceaModel.cea05;
                }
                drGeb["geb09"] = 1;                     //借貸 1.借 2.貸
                drGeb["geb10"] = ceaModel.cea15t;       //本幣金額(稅)
                //幣別均以本幣處理
                drGeb["geb11"] = baaModel.baa04;        //原幣幣別(帶入本幣幣別)
                drGeb["geb12"] = ceaModel.cea12;        //匯率
                drGeb["geb13"] = ceaModel.cea15t;       //原幣金額(稅)
                drGeb["geb14"] = ceaModel.cea03;        //客戶編號
                drGeb["geb15"] = boStp.OfGetSca03(ceaModel.cea03);                    //客戶簡稱
                drGeb["geb16"] = DBNull.Value;          //保留
                drGeb["geb17"] = DBNull.Value;          //保留
                drGeb["geb18"] = DBNull.Value;          //保留
                drGeb["geb19"] = DBNull.Value;          //保留
                drGeb["geb20"] = DBNull.Value;          //保留
                drGeb["geb21"] = DBNull.Value;          //保留
                drGeb["gebcomp"] = pLoginInfo.CompNo;
                drGeb["gebcreu"] = pLoginInfo.UserNo;
                drGeb["gebcreg"] = pLoginInfo.DeptNo;
                drGeb["gebcred"] = OfGetNow();
                drGeb["gebmodu"] = DBNull.Value;
                drGeb["gebmodg"] = DBNull.Value;
                drGeb["gebmodd"] = DBNull.Value;
                dtGeb.Rows.Add(drGeb);

                //處理貸方未稅明細資料
                foreach (ceb_tb cebModel in cebList)
                {
                    i++;
                    drGeb = dtGeb.NewRow();
                    drGeb["geb01"] = cebModel.ceb01;        //底稿單頭
                    drGeb["geb02"] = "AR";
                    drGeb["geb03"] = 1;                     //1.應收 2.收款
                    drGeb["geb04"] = 1;                     //AR 固定為1
                    drGeb["geb05"] = i;                     //項次
                    drGeb["geb06"] = cebModel.ceb10;        //會計科目
                    drGeb["geb07"] = "";                    //摘要
                    drGeb["geb08"] = DBNull.Value;          //部門
                    if (!GlobalFn.varIsNull(cebModel.ceb10))
                    {
                        gbaModel = OfGetGbaModel(cebModel.ceb10);
                        if (gbaModel != null && gbaModel.gba09 == "Y")
                            drGeb["geb08"] = ceaModel.cea05;
                    }
                    drGeb["geb09"] = 2;                     //借貸 1.借 2.貸
                    drGeb["geb10"] = cebModel.ceb09;        //本幣金額(未稅)
                    drGeb["geb11"] = ceaModel.cea10;        //原幣幣別
                    drGeb["geb12"] = ceaModel.cea12;        //匯率
                    drGeb["geb13"] = cebModel.ceb07;        //原幣金額
                    drGeb["geb14"] = ceaModel.cea03;        //客戶編號
                    drGeb["geb15"] = boStp.OfGetSca03(ceaModel.cea03);                    //客戶簡稱
                    drGeb["geb16"] = DBNull.Value;          //保留
                    drGeb["geb17"] = DBNull.Value;          //保留
                    drGeb["geb18"] = DBNull.Value;          //保留
                    drGeb["geb19"] = DBNull.Value;          //保留
                    drGeb["geb20"] = DBNull.Value;          //保留
                    drGeb["geb21"] = DBNull.Value;          //保留
                    drGeb["gebcomp"] = pLoginInfo.CompNo;
                    drGeb["gebcreu"] = pLoginInfo.UserNo;
                    drGeb["gebcreg"] = pLoginInfo.DeptNo;
                    drGeb["gebcred"] = OfGetNow();
                    drGeb["gebmodu"] = DBNull.Value;
                    drGeb["gebmodg"] = DBNull.Value;
                    drGeb["gebmodd"] = DBNull.Value;
                    dtGeb.Rows.Add(drGeb);
                }
                
                //貸方再補一筆稅額
                //用本幣匯總成一筆來加總
                drGeb = dtGeb.NewRow();
                i++;
                drGeb = dtGeb.NewRow();
                drGeb["geb01"] = ceaModel.cea01;        //底稿單頭
                drGeb["geb02"] = "AR";
                drGeb["geb03"] = 1;                     //1.應收 2.收款
                drGeb["geb04"] = 1;                     //AR 固定為1
                drGeb["geb05"] = i;                     //項次



                drGeb["geb06"] = cbaModel.cba05;        //會計科目
                drGeb["geb07"] = "";                    //摘要
                drGeb["geb08"] = DBNull.Value;          //部門
                if (!GlobalFn.varIsNull(cbaModel.cba05))
                {
                    gbaModel = OfGetGbaModel(cbaModel.cba05);
                    if (gbaModel != null && gbaModel.gba09 == "Y")
                        drGeb["geb08"] = ceaModel.cea05;
                }
                drGeb["geb09"] = 2;                     //借貸 1.借 2.貸

                var taxTotal = cebList.Sum(x => x.ceb09t - x.ceb09);
                drGeb["geb10"] = taxTotal;        //本幣金額(稅)
                //幣別均以本幣處理
                drGeb["geb11"] = baaModel.baa04;        //原幣幣別(帶入本幣幣別)
                drGeb["geb12"] = ceaModel.cea12;        //匯率
                drGeb["geb13"] = taxTotal;             //原幣金額(稅)
                drGeb["geb14"] = ceaModel.cea03;        //客戶編號
                drGeb["geb15"] = boStp.OfGetSca03(ceaModel.cea03);                    //客戶簡稱
                drGeb["geb16"] = DBNull.Value;          //保留
                drGeb["geb17"] = DBNull.Value;          //保留
                drGeb["geb18"] = DBNull.Value;          //保留
                drGeb["geb19"] = DBNull.Value;          //保留
                drGeb["geb20"] = DBNull.Value;          //保留
                drGeb["geb21"] = DBNull.Value;          //保留
                drGeb["gebcomp"] = pLoginInfo.CompNo;
                drGeb["gebcreu"] = pLoginInfo.UserNo;
                drGeb["gebcreg"] = pLoginInfo.DeptNo;
                drGeb["gebcred"] = OfGetNow();
                drGeb["gebmodu"] = DBNull.Value;
                drGeb["gebmodg"] = DBNull.Value;
                drGeb["gebmodd"] = DBNull.Value;
                dtGeb.Rows.Add(drGeb);

                if (this.OfUpdate(dtGeb) == -1)
                {
                    result = new Result();
                    result.Key1 = pCea01;
                    result.Message = "新增分錄底稿單身失敗!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }

                return rtnResultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGenGeaByCfa 分錄底稿產生作業--收款沖帳單
        public List<Result> OfGenGeaByCfa(string pCfa01, UserInfo pLoginInfo)
        {
            List<Result> rtnResultList = null;
            Result result = null;
            cfa_tb cfaModel = null;
            List<cfb_tb> cfbList = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            CarBLL boCar = null;
            StpBLL boStp = null;
            BasBLL boBas = null;
            DataTable dtGea = null, dtGeb = null;
            DataRow drGea = null, drGeb = null;
            baa_tb baaModel = null;
            try
            {
                rtnResultList = new List<Result>();
                boBas = new BasBLL(this.OfGetConntion());
                boBas.TRAN = this.TRAN;
                boCar = new CarBLL(this.OfGetConntion());
                boCar.TRAN = this.TRAN;
                boStp = new StpBLL(this.OfGetConntion());
                boStp.TRAN = this.TRAN;

                baaModel = boBas.OfGetBaaModel();
                if (baaModel == null || GlobalFn.varIsNull(baaModel.baa04))
                {
                    result = new Result();
                    result.Key1 = pCfa01;
                    result.Message = "查無本國幣別!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }

                cfaModel = boCar.OfGetCfaModel(pCfa01);
                if (cfaModel == null)
                {
                    result = new Result();
                    result.Key1 = pCfa01;
                    result.Message = "查無此收款沖帳單號!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }
                if (cfaModel.cfaconf != "N")
                {
                    result = new Result();
                    result.Key1 = pCfa01;
                    result.Message = "收款沖帳單非未確認狀態!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }

                cfbList = boCar.OfGetCfbList(pCfa01);
                if (cfbList == null || cfbList.Count == 0)
                {
                    result = new Result();
                    result.Key1 = pCfa01;
                    result.Message = "收款沖帳單無單身資料!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }
                //新增底稿單頭
                this.OfCreateDao("gea_tb", "*", "");
                sqlSelect = "SELECT * FROM gea_tb WHERE 1<>1 ";
                dtGea = this.OfGetDataTable(sqlSelect);
                drGea = dtGea.NewRow();
                drGea["gea01"] = cfaModel.cfa01;    //底稿單號
                drGea["gea02"] = "AR";  //系統別
                drGea["gea03"] = 2;     //1.應收 2.收款
                drGea["gea04"] = 1;     //AR 固定為1
                drGea["gea05"] = cfaModel.cfa02;    //同帳款日期
                drGea["gea06"] = DBNull.Value;    //傳票號碼
                drGea["gea07"] = DBNull.Value;    //傳票日期
                drGea["gea08"] = cfaModel.cfa10;               //借方金額--本幣
                drGea["gea09"] = cfaModel.cfa11;                 //貸方金額--本幣
                drGea["gea10"] = DBNull.Value;      //保留
                drGea["gea11"] = DBNull.Value;      //保留
                drGea["gea12"] = DBNull.Value;      //保留
                drGea["geaprno"] = 0;
                drGea["geacomp"] = pLoginInfo.CompNo;
                drGea["geacreu"] = pLoginInfo.UserNo;
                drGea["geacreg"] = pLoginInfo.DeptNo;
                drGea["geacred"] = OfGetNow();
                drGea["geamodu"] = DBNull.Value;
                drGea["geamodg"] = DBNull.Value;
                drGea["geamodd"] = DBNull.Value;
                drGea["geasecu"] = pLoginInfo.UserNo;
                drGea["geasecg"] = pLoginInfo.GroupNo;
                dtGea.Rows.Add(drGea);
                if (this.OfUpdate(dtGea) == -1)
                {
                    result = new Result();
                    result.Key1 = pCfa01;
                    result.Message = "新增分錄底稿單頭失敗!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }

                //新增底稿單身
                this.OfCreateDao("geb_tb", "*", "");
                sqlSelect = "SELECT * FROM geb_tb WHERE 1<>1 ";
                dtGeb = this.OfGetDataTable(sqlSelect);

                //處理貸方資料
                foreach (cfb_tb cfbModel in cfbList)
                {
                    drGeb = dtGeb.NewRow();
                    drGeb["geb01"] = cfbModel.cfb01;        //底稿單頭
                    drGeb["geb02"] = "AR";
                    drGeb["geb03"] = 2;                     //1.應收 2.收款
                    drGeb["geb04"] = 1;                     //AR 固定為1
                    drGeb["geb05"] = cfbModel.cfb02;        //項次
                    drGeb["geb06"] = cfbModel.cfb11;        //會計科目
                    drGeb["geb07"] = "";                    //摘要
                    drGeb["geb08"] = cfbModel.cfb13;        //部門
                    drGeb["geb09"] = cfbModel.cfb03;        //借貸 1.借 2.貸
                    drGeb["geb10"] = cfbModel.cfb03;        //本幣金額(未稅)
                    drGeb["geb11"] = cfbModel.cfb07;        //原幣幣別
                    drGeb["geb12"] = cfbModel.cfb08;        //匯率
                    drGeb["geb13"] = cfbModel.cfb09;        //原幣金額
                    drGeb["geb14"] = cfaModel.cfa03;        //客戶編號
                    drGeb["geb15"] = boStp.OfGetSca03(cfaModel.cfa03);                    //客戶簡稱
                    drGeb["geb16"] = DBNull.Value;          //保留
                    drGeb["geb17"] = DBNull.Value;          //保留
                    drGeb["geb18"] = DBNull.Value;          //保留
                    drGeb["geb19"] = DBNull.Value;          //保留
                    drGeb["geb20"] = DBNull.Value;          //保留
                    drGeb["geb21"] = DBNull.Value;          //保留
                    drGeb["gebcomp"] = pLoginInfo.CompNo;
                    drGeb["gebcreu"] = pLoginInfo.UserNo;
                    drGeb["gebcreg"] = pLoginInfo.DeptNo;
                    drGeb["gebcred"] = OfGetNow();
                    drGeb["gebmodu"] = DBNull.Value;
                    drGeb["gebmodg"] = DBNull.Value;
                    drGeb["gebmodd"] = DBNull.Value;
                    dtGeb.Rows.Add(drGeb);
                }

                if (this.OfUpdate(dtGeb) == -1)
                {
                    result = new Result();
                    result.Key1 = pCfa01;
                    result.Message = "新增分錄底稿單身失敗!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }

                return rtnResultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
