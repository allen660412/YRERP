/* 程式名稱: FrmLogin.cs
   系統代號: 
   作　　者: Allen
   描　　述: 
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using YR.ERP.Shared;
using YR.Util;

namespace YR.ERP.Forms
{
    public partial class FrmLogin : YR.ERP.Base.Forms.FrmBase
    {
        #region Property
        int IntTryTimes = 0;//測試次數 
        #endregion

        #region 建構子
        public FrmLogin()
        {
            InitializeComponent();
        }
        #endregion

        #region FrmLogin_Load
        private void FrmLogin_Load(object sender, EventArgs e)
        {

            string ls_com;
            ls_com = Environment.GetEnvironmentVariable("COMPUTERNAME"); ;
            if (ls_com.ToLower() == "allen-nb")
            {
                txt_InputUserid.Text = "Y0474";
                txt_InputPassword.Text = "9999";
            }
        }
        #endregion

        #region Button_Click
        private void Button_Click(object sender, EventArgs e)
        {
            string senderKey;
            try
            {
                WfCleanBottomMsg();
                senderKey = WfGetControlName(sender);
                switch (senderKey.ToLower())
                {
                    case "ubtnok":
                        if (WfChkSecurity() == false)
                            return;
                        
                        this.DialogResult = DialogResult.OK;
                        break;
                        
                    case "ubtncancel":
                        this.DialogResult = DialogResult.Cancel;
                        break;
                }

            }
            catch (Exception ex)
            {
                WfShowBottomStatusMsg(ex.Message);
                this.Close();
            }
        }
        #endregion

        #region WfChkSecurity
        private bool WfChkSecurity()
        {
            // 輸入的帳號及密碼
            string userId, passWord;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                userId = txt_InputUserid.Text;
                passWord = txt_InputPassword.Text;
                if (GlobalFn.varIsNull(userId))
                {
                    WfShowErrorMsg("請輸入使用者 ID !");
                    return false;
                }
                if (GlobalFn.varIsNull(passWord))
                {
                    WfShowErrorMsg("請輸入使用者 密碼 !");
                    return false;
                }
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ada_tb ");
                sbSql.AppendLine("WHERE  ada01=@ada01");
                //sbSql.AppendLine("AND ada05=@ada05");
                sqlParmList = new List<SqlParameter>();

                sqlParmList.Add(new SqlParameter("ada01", userId));
                //sqlParmList.Add(new SqlParameter("ada05", passWord));

                //drAda = BoSecurity.OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                var adaModel = BoSecurity.OfGetAdaModel(userId);    //取得使用者資料
                if (adaModel == null)
                {
                    WfShowErrorMsg("無此帳號，請重新輸入!");
                    IntTryTimes++;

                    if (IntTryTimes >= 3)
                    {
                        WfShowErrorMsg("輸入錯誤三次以上,程式即將關閉...!");
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                    }
                    return false;
                }
                else
                {
                    var md5Hash = GlobalFn.genMd5Hash(passWord);
                    if (md5Hash != adaModel.ada05)
                    {
                        WfShowErrorMsg("帳號或密碼錯誤，請重新輸入!");
                        IntTryTimes++;

                        if (IntTryTimes >= 3)
                        {
                            WfShowErrorMsg("輸入錯誤三次以上,程式即將關 閉...!");
                            this.DialogResult = DialogResult.Cancel;
                            this.Close();
                        }
                        return false;
                    }
                }

                LoginInfo.UserNo = GlobalFn.isNullRet(adaModel.ada01, "");
                LoginInfo.UserName = GlobalFn.isNullRet(adaModel.ada02, "");
                LoginInfo.GroupNo = GlobalFn.isNullRet(adaModel.ada03, "");
                if (!GlobalFn.varIsNull(LoginInfo.GroupNo))
                {
                    var adeTbModel = BoSecurity.OfGetAdeModel(LoginInfo.GroupNo);
                    if (adeTbModel != null)
                        LoginInfo.GroupLevel = adeTbModel.ade03;
                }
                //LoginInfo.DeptNo = Global_Fn.isNullRet(ldr_ada["ada03"], "");     //移至開啟視窗中執行
                //LoginInfo.DeptName = "";                                            //移至開啟視窗中執行

                LoginInfo.UserRole = GlobalFn.isNullRet(adaModel.ada04, "");
                this.DialogResult = DialogResult.OK;
                return true;
                //this.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
