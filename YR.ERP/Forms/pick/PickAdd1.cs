/*
  程式名稱: PickAdd1.cs
  系統代號: 
  作　　者: Allen
  描　　述: 權限挑選-for admi110
  異動記錄:
  Date        PG         Memo
  ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using YR.ERP.Base.Forms;
using YR.ERP.DAL.YRModel;
using YR.ERP.Shared;
using YR.Util;
using System.Linq;

namespace YR.ERP.Forms.pick
{
    public partial class PickAdd1 : FrmPickBase
    {
        #region property
        List<adq_tb> _actAllList;
        string _actChoose; 
        #endregion

        #region 建構子
        public PickAdd1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 權限挑選-for admi110
        /// </summary>
        /// <param name="pActAllList">傳入程式所具有的所有action</param>
        /// <param name="pActChoose">傳入角色該程式原本已具有的action</param>
        /// <param name="pUserInfoModel"></param>
        public PickAdd1(List<adq_tb> pActAllList, string pActChoose, UserInfo pUserInfoModel)
            : base("pickAdd1", pUserInfoModel)
        {
            InitializeComponent();
            _actAllList = pActAllList;
            _actChoose = pActChoose;              
        } 
        #endregion

        #region WfIniSqlBody 初始化sql字串
        protected override void WfIniSqlBody()
        {
            StringBuilder sbSql;
            try
            {
                if (MsgInfoReturned == null)
                    throw new Exception("未實體化msgInfoReturned");

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT CONVERT(NVARCHAR(1),'N') is_pick,");
                sbSql.AppendLine("CONVERT(NVARCHAR(100),'') AS action,");
                sbSql.AppendLine("CONVERT(NVARCHAR(100),'') AS action_c");
                StrSqlBody = sbSql.ToString();
                DtMaster = BoMaster.OfGetDataTable(sbSql.ToString());

                Admi650Model = new vw_admi650()
                {
                    azp01 = StrPickNo,
                    azp02 = "action功能挑選",
                    azp03 = "",
                    azp03_c = "",
                    azp04 = "",
                    azp05 = "",
                    azp06 = "N",
                    azp07 = "",
                    azp08 = "Y",
                };
                Admi650sList = new List<vw_admi650s>()
                {
                    new vw_admi650s(){
                        azq01=Admi650Model.azp01,
                        azq02=1,
                        azq03="",
                        azq04="action",
                        azq04_c="action",
                        azq06="1",
                        azq08=100
                    },
                    new vw_admi650s(){
                        azq01=Admi650Model.azp01,
                        azq02=2,
                        azq03="",
                        azq04="action_c",
                        azq04_c="action名稱",
                        azq06="1",
                        azq08=200
                    }
                };


                //修改column 型別為string 並將實際的型別丟到　column.prefix
                foreach (DataColumn dcTempColumn in DtMaster.Columns)
                {
                    if (dcTempColumn.Prefix != "")
                        continue;
                    dcTempColumn.Prefix = dcTempColumn.DataType.Name;
                    dcTempColumn.DataType = typeof(string);
                }
                BindingMaster.DataSource = DtMaster;
                uGrid_Master.DataSource = BindingMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfQuery() 按下查詢按鈕時
        protected override bool WfQuery()
        {
            DataRow drNew;
            try
            {
                //DtMaster = BoMaster.OfGetDataTable(StrSqlBody + " AND 1<>1 ");
                DtMaster = BoMaster.OfGetDataTable(StrSqlBody);
                ////修改column 型別為string 並將實際的型別丟到　column.prefix
                //foreach (DataColumn ldc_temp in DtMaster.Columns)
                //{
                //    if (ldc_temp.Prefix != "")
                //        continue;
                //    ldc_temp.Prefix = ldc_temp.DataType.Name;
                //    ldc_temp.DataType = typeof(string);
                //}
                BindingMaster.DataSource = DtMaster;
                uGrid_Master.DataSource = BindingMaster;

                drNew = DtMaster.NewRow();
                DtMaster.Rows.Add(drNew);

                uGrid_Master.ActiveCell = uGrid_Master.Rows[0].Cells[1];

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfQueryOk() 查詢後按下OK按鈕
        protected override bool WfQueryOk()
        {

            //List<string> actAllArray = null;
            List<string> actChooseList = null;
            try
            {
                DtMaster.Rows.Clear();
                if (!GlobalFn.varIsNull(_actAllList))
                {
                    //actAllArray = _actAllList.Split(new char[] { ',' })
                    //                     .ToList<string>();
                    if (!GlobalFn.varIsNull(_actChoose))
                    {
                        actChooseList = _actChoose.Split(new char[] { ',' })
                                     .ToList<string>();

                    }
                    foreach (adq_tb adqModel in _actAllList)
                    {
                        var drNew = DtMaster.NewRow();
                        drNew["is_pick"] = "N";
                        if (actChooseList != null)
                        {
                            var chkCnts = actChooseList.Where(p => p.ToLower() == adqModel.adq02.ToLower()).Count();
                            if (chkCnts > 0)
                                drNew["is_pick"] = "Y";
                        }
                        drNew["action"] = adqModel.adq02;
                        drNew["action_c"] = adqModel.adq03;
                        DtMaster.Rows.Add(drNew);
                    }
                    //DtMaster = BoMaster.OfGetDataTable(StrSqlBody + extendSqlWhere, SqlParmTotalList.ToArray());
                }


                BindingMaster.DataSource = DtMaster;

                //改取sort後的第一筆
                if (DtMaster != null && DtMaster.Rows.Count > 0)
                    uGrid_Master.Rows.GetRowWithListIndex(0).Selected = true;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfReturnOk() 選擇後按下OK按鈕
        protected override bool WfReturnOk()
        {
            DataRow[] returnRowCollection;
            try
            {
                MsgInfoReturned.DataRowList.Clear();
                MsgInfoReturned.Result = System.Windows.Forms.DialogResult.OK;
                returnRowCollection = DtMaster.Select(" is_pick ='Y' ");
                if (returnRowCollection != null)
                    MsgInfoReturned.DataRowList.AddRange(returnRowCollection);
                MsgInfoReturned.StrMultiRtn = WfGetStrMultiRtrn(returnRowCollection, ',');
                this.Close();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
