/* 程式名稱: FrmAdms801.cs
   系統代號: 
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
using System.Windows.Forms;
using YR.ERP.BLL.MSSQL;
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.BLL.Model;
using Infragistics.Win.UltraWinEditors;

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdms801 : YR.ERP.Base.Forms.FrmBase
    {
        #region 建構子
        public FrmAdms801()
        {
            InitializeComponent();
        }
        #endregion
        
        #region FrmAdms801_Load
        private void FrmAdms801_Load(object sender, EventArgs e)
        {
            try
            {
                WfSetUltraTxtEditPick(ute_adb02);
                ute_adb02.UseAppStyling = false;
                ute_adb02.EditorButtonClick += new EditorButtonEventHandler(UltraTextEditor_EditorPickButtonClick);
                if (LoginInfo != null)
                {
                    if (GlobalFn.varIsNull(LoginInfo.CompNo))//表示第一次登入
                    {
                        var adaModel = BoSecurity.OfGetAdaModel(LoginInfo.UserNo);
                        ute_adb02.Text = adaModel.ada08;
                    }
                    else
                        ute_adb02.Text = LoginInfo.CompNo;
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Button_Click
        private void Button_Click(object sender, EventArgs e)
        {
            string ls_sender;
            try
            {
                ls_sender = WfGetControlName(sender);
                switch (ls_sender.ToLower())
                {
                    case "ubtnok":
                        WfChkAdb();
                        break;
                    case "ubtncancel":
                        break;
                }
            }
            catch (Exception ex)
            {
                WfShowBottomStatusMsg(ex.Message);
            }

        }
        #endregion

        #region WfChkAdb 檢查是否有登入的權限
        private void WfChkAdb()
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            DataRow drAta;
            string adb02;
            adb02 = ute_adb02.Text;
            BasBLL boBas;
            bec_tb becModel;
            try
            {
                if (GlobalFn.isNullRet(adb02, "") == "")
                {
                    WfShowErrorMsg("請輸入公司別");
                }
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT ata01,ata04 FROM adb_tb ");
                sbSql.AppendLine("  INNER JOIN ata_tb ON adb02=ata01 ");
                sbSql.AppendLine("WHERE adb01=@adb01 AND adb02=@adb02 ");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@adb01", LoginInfo.UserNo));
                sqlParmList.Add(new SqlParameter("@adb02", adb02));
                drAta = BoSecurity.OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                if (drAta == null || drAta.Table.Rows.Count == 0)
                {
                    WfShowErrorMsg("無此公司別,或無登入權限!");
                    return;
                }
                if (this.Owner != null)
                {
                    if (drAta["ata04"] != DBNull.Value)
                    {
                        (this.Owner as YR.ERP.Base.Forms.FrmBase).MainConnStr = drAta["ata04"].ToString();
                        //(this.Owner as YR.ERP.Base.Forms.FrmBase).LoginInfo.CompNo = adb02;
                        //改以ATA_TB.ATA01 帶入,大小寫問題
                        (this.Owner as YR.ERP.Base.Forms.FrmBase).LoginInfo.CompNo = drAta["ata01"].ToString();
                        boBas = new BasBLL(LoginInfo.CompNo, "", "", "");

                        (this.Owner as YR.ERP.Base.Forms.FrmBase).LoginInfo.CompNameA = boBas.OfGetBea01(adb02);
                        becModel = boBas.OfGetBecModel(LoginInfo.UserNo);
                        //if (l_bec==null)
                        //{
                        //    WfShowMsg(ls_adb02+"未設定員工資料,請確認!");
                        //    return;
                        //}

                        if (becModel != null)
                        {
                            (this.Owner as YR.ERP.Base.Forms.FrmBase).LoginInfo.DeptNo = becModel.bec03;
                            (this.Owner as YR.ERP.Base.Forms.FrmBase).LoginInfo.DeptName = boBas.OfGetBeb03(becModel.bec03);
                        }
                    }
                    this.DialogResult = DialogResult.OK;
                    //if (WfUpdateAda08(LoginInfo.UserNo, adb02) == false)
                    if (WfUpdateAda08(LoginInfo.UserNo, LoginInfo.CompNo) == false)
                        return;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        protected bool WfPickClickOnEditMode(object sender, string pColName)
        {
            MessageInfo messageModel = new MessageInfo();
            try
            {
                switch (pColName.ToLower())
                {
                    case "adb02":
                        messageModel = new MessageInfo();
                        messageModel.ParamSearchList = new List<SqlParameter>();
                        messageModel.ParamSearchList.Add(new SqlParameter("@adb01", LoginInfo.UserNo));
                        WfShowPickUtility("p_adb", messageModel, FormStartPosition.CenterScreen);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            if (messageModel.DataRowList.Count > 0)
                                ute_adb02.Text = GlobalFn.isNullRet(messageModel.DataRowList[0]["adb02"], "");
                            else
                                ute_adb02.Text = "";
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

        protected override void UltraTextEditor_EditorPickButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string pickName = "";
            try
            {
                if (e.Button.Key != null && e.Button.Key.ToLower() == "pick")
                {
                    pickName = e.Button.Tag.ToString();
                    WfPickClickOnEditMode(sender, pickName);
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.ToString());
            }
        }

        protected bool WfUpdateAda08(string pAda01, string pAda08)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmsList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("UPDATE ada_tb");
                sbSql.AppendLine("SET ada08=@ada08");
                sbSql.AppendLine("WHERE ada01=@ada01");
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("ada01", pAda01));
                sqlParmsList.Add(new SqlParameter("ada08", pAda08));
                BoSecurity.OfExecuteNonquery(sbSql.ToString(), sqlParmsList.ToArray());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
