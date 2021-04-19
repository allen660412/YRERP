/* 程式名稱: 會科各期餘額查詢
   系統代號: glat401
   作　　者: Allen
   描　　述: 

   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.BLL.MSSQL.Gla;
using YR.ERP.DAL.YRModel;
using YR.ERP.Shared;
using YR.Util;

namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlaq401 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region 建構子
        public FrmGlaq401()
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
            this.StrFormID = "glaq401";

            this.IntTabCount = 2;
            this.IntMasterGridPos = 2;
            uTab_Master.Tabs[0].Text = "基本資料";
            uTab_Master.Tabs[1].Text = "資料瀏覽";
            
            IntTabDetailCount = 1;
            uTab_Detail.Tabs[0].Text = "明細資料";
            return true;
        }
        #endregion
        
        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { 
                                                new SqlParameter("gbh01", SqlDbType.NVarChar) ,
                                                new SqlParameter("gbh02", SqlDbType.Int) 
                });

                TabMaster.CanCopyMode = false;
                TabMaster.CanAddMode = false;
                TabMaster.CanDeleteMode = false;
                TabMaster.CanUpdateMode = false;
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
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniTabDetailInfo(): 設定明細的資料來源
        protected override Boolean WfIniTabDetailInfo()
        {
            SqlParameter keyParm;
            this.TabDetailList[0].CanUgridQuery = false;
            this.TabDetailList[0].CanAdvanceQuery = false;
            
            this.TabDetailList[0].TargetTable = "gbh_tb";
            this.TabDetailList[0].ViewTable = "vw_glaq401s";
            keyParm = new SqlParameter("gbh01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "gbh01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            keyParm = new SqlParameter("gbh02", SqlDbType.Int);
            keyParm.SourceColumn = "gbh02";
            this.TabDetailList[0].RelationParams.Add(keyParm);
            this.TabDetailList[0].SelectSql = @"
                    SELECT 
	                    gbh_tb.*,
	                    SUM(CASE WHEN gba05='1' THEN gbh04-gbh05 ELSE gbh05-gbh04 END) over(ORDER BY gbh01,gbh02,gbh03) AS sum_blance
                    FROM dbo.gbh_tb
	                    LEFT JOIN dbo.gba_tb  ON gbh01=gba01
                    ";
            return true;
        }
        #endregion
        
        #region WfAfterfDisplayMode  新增修改刪除查詢後的 focus調整
        protected override void WfAfterfDisplayMode()
        {                        
            try
            {
                uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                SelectNextControl(this.uTab_Master, true, true, true, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
