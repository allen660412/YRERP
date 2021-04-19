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
using YR.ERP.BLL.MSSQL;

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvi100_1 : YR.ERP.Base.Forms.FrmEntryBase
    {

        #region Property
        PurBLL BoPur = null;

        //YREditType _srcFormState = new YREditType();//表單傳入要執行的狀態
        //vw_admi611s _srcVwAdmi611s = null;
        #endregion


        #region 建構子
        public FrmInvi100_1()
        {
            InitializeComponent();
        }

        public FrmInvi100_1(string pSourceForm, YR.ERP.Shared.UserInfo pUserInfo, string pWhere)
        {
            InitializeComponent();
            StrQueryWhereAppend = pWhere;            
            this.LoginInfo = pUserInfo;
        }
        #endregion


        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            this.StrFormID = "invi100_1";

            this.IntTabCount = 1;
            this.IntMasterGridPos = 1;
            uTab_Master.Tabs[0].Text = "資料內容";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("pga01", SqlDbType.NVarChar)
                                                                                });

                TabMaster.CanCopyMode = false;
                TabMaster.CanDeleteMode = false;
                TabMaster.CanQueryMode = false;
                TabMaster.CanUseRowLock = false;    //不使用lock處理
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);

            BoPur = new PurBLL(BoMaster.OfGetConntion());
            return;
        }
        #endregion

        #region WfSetBllTransaction 以bomaster 註冊transaction至其他 bll
        protected override void WfSetBllTransaction()
        {
            try
            {
                if (BoMaster.TRAN != null)
                {
                    BoPur.TRAN = BoMaster.TRAN;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
