using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using YR.Util;

namespace YR.ERP.Shared
{

    public class UserInfo
    {
        public string CompNo{get;set;}
        public string DeptNo{get;set;}
        public string UserNo { get; set; }
        public string GroupNo { get; set; }          //角色群組--ada_tb.ada03
        public string GroupLevel { get; set; }       //群組階層--ade_tb.ade03

        public string CompNameA { get; set; }
        //public string User_CompName_B{get;set;}
        public string UserName{get;set;}
        public string DeptName{get;set;}
        public string UserRole { get; set; }        //角色權限

        public UserInfo(){}

        /// <summary>
        /// 以傳入的公司別、帳號建立使用者資訊
        /// </summary>
        /// <param name="ps_comp_no"></param>
        /// <param name="ps_usrid"></param>
        public UserInfo(string psComp, string psUser)
        {
            //設定使用者資訊                
            this.CompNo = psComp;
            this.UserNo = psUser;
        }



    }
}

