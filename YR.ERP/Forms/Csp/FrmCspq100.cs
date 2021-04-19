using Infragistics.Win.UltraWinGrid;
/* 程式名稱: 成本明細查詢
   系統代號: cspq100
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
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YR.ERP.BLL.MSSQL;

namespace YR.ERP.Forms.Csp
{
    public partial class FrmCspq100 : YR.ERP.Base.Forms.FrmEntryBase
    {

        #region Property
        CspBLL BoCsp = null;
        #endregion


        #region 建構子
        public FrmCspq100()
        {
            InitializeComponent();
        } 
        #endregion

        #region FrmCspq100_Load
        private void FrmCspq100_Load(object sender, EventArgs e)
        {
            uGridMaster.InitializeRow += uGridMaster_InitializeRow;
        } 
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            this.StrFormID = "cspq100";
            this.IntTabCount = 1;
            this.IntMasterGridPos = 1;
            uTab_Master.Tabs[0].Text = "資料瀏覽";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("jja01", SqlDbType.NVarChar),
                                                new SqlParameter("jja02", SqlDbType.NVarChar),
                                                new SqlParameter("jja03", SqlDbType.Decimal)
                                });

                TabMaster.CanAddMode = false;
                TabMaster.CanCopyMode = false;
                TabMaster.CanDeleteMode = false;
                TabMaster.CanUpdateMode = false;
                TabMaster.CanUseRowLock = false;
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

            BoCsp = new CspBLL(BoMaster.OfGetConntion());
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
                    BoCsp.TRAN = BoMaster.TRAN;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示
        protected override Boolean WfDisplayMode()
        {
            //decimal sumIna10 = 0;
            //List<vw_invq220> masterList;
            //try
            //{
            //    WfSetControlsReadOnlyRecursion(this, true);

            //    if (TabMaster.DtSource == null || TabMaster.DtSource.Rows.Count == 0)
            //        sumIna10 = 0;
            //    else
            //    {
            //        masterList = TabMaster.DtSource.ToList<vw_invq220>();
            //        sumIna10 = (from o in masterList
            //                    select new
            //                    {
            //                        diff = o.ina03 == "1" ? o.ina10 : -1 * o.ina10
            //                    }
            //                    )
            //                    .Sum(p => p.diff)
            //                    ;
            //    }
            //    lbl_sum_in10.Text = sumIna10.ToString();


            return true;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        #endregion

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        
        #region uGridMaster_InitializeRow
        void uGridMaster_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            //if (e.Row.Cells["ina03"].Value.ToString() == "2")
            //    e.Row.Cells["ina10"].Appearance.ForeColor = Color.Red;
        }
        #endregion
    }
}
