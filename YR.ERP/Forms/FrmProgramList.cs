/* 程式名稱: FrmProgramList.cs
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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YR.ERP.Shared;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinTree;
using YR.Util;
using YR.ERP.DAL.YRModel;
using System.IO;
using YR.Util.Controls;

namespace YR.ERP.Forms
{
    public partial class FrmProgramList : YR.ERP.Base.Forms.FrmBase
    {
        #region Property
        private string m_strSecConString = GlobalVar.SQLCA_SecConSTR;
        ImageList ImgList = YR.Util.GlobalPictuer.LoadProgramListImage();

        private Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager ultraTabbedMdiManager1;

        #endregion

        #region 建構子
        public FrmProgramList()
        {
            InitializeComponent();
        }
        #endregion

        #region WfShowLogin 使用者登入
        private DialogResult WfShowLogin(YR.ERP.BLL.MSSQL.AdmBLL pBoAdm)
        {
            YR.ERP.Forms.FrmLogin frmLogin = new YR.ERP.Forms.FrmLogin();
            frmLogin.BoSecurity = pBoAdm;

            if (frmLogin == null)
            {
                WfShowErrorMsg("初始化視窗失敗!");
                return DialogResult.Cancel;
            }
            else
            {
                DialogResult = frmLogin.ShowDialog();
                this.LoginInfo = frmLogin.LoginInfo;
                //DialogResult = DialogResult.OK;
                return DialogResult;
            }
        }
        #endregion
        
        #region WfShowAdms801 公司別登入
        private DialogResult WfShowAdms801(YR.ERP.BLL.MSSQL.AdmBLL p_admbll)
        {
            using (YR.ERP.Forms.Adm.FrmAdms801 frmAdms801 = new YR.ERP.Forms.Adm.FrmAdms801())
            {
                frmAdms801.Owner = this;
                frmAdms801.BoSecurity = p_admbll;
                frmAdms801.LoginInfo = this.LoginInfo;          

                if (frmAdms801 == null)
                {
                    WfShowErrorMsg("初始化視窗失敗!");
                    return DialogResult.Cancel;
                }
                else
                {
                    DialogResult = frmAdms801.ShowDialog();
                    return DialogResult;
                }
            }
        }
        #endregion
        
        #region FrmProgramList_Load
        private void FrmProgramList_Load(object sender, EventArgs e)
        {
            SqlConnection sqlConnection;
            DAL.ERP_MSSQLDAL ladm_dao;
            sqlConnection = new SqlConnection(this.m_strSecConString);
            ladm_dao = YR.ERP.DAL.DALFactory.CreateDAO(m_strSecConString);
            BoSecurity = new YR.ERP.BLL.MSSQL.AdmBLL(ladm_dao);
            try
            {
                //this.BackColor = ColorTranslator.FromHtml("#FFFDE6");

                // 保存 Security 連線及admbll
                YR.ERP.Shared.GlobalVar.DbAdmConn = sqlConnection;
                YR.ERP.Shared.GlobalVar.PU_BUSOBJ = BoSecurity;
                YR.ERP.Shared.GlobalVar.Adm_DAO = ladm_dao;
                //WfSetAppearance(ultraSplitter1);

                DialogResult dialogResult = this.WfShowLogin(BoSecurity);
                if (dialogResult == DialogResult.OK)
                {
                    dialogResult = this.WfShowAdms801(BoSecurity);
                    if (dialogResult == DialogResult.OK)
                    {
                        WfIniUltraWinExplorBar(UebMenu);
                        WfLoadMenu();
                        this.WindowState = FormWindowState.Maximized;
                        WfSetFormTitle("Menu", "主選單", LoginInfo);
                        if (this.IsMdiChild)
                            this.MdiParent.Text = this.Text;
                        this.Show();
                        this.BringToFront();
                    }
                }
                else
                {
                    Application.Exit();
                }
                this.DialogResult = dialogResult;

                ute_ado01.KeyUp += Ute_ado01_KeyUp;
                ute_ado01.Focus();
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion
        
        #region WfOpenForm 開啟程式
        /// <summary>
        /// 檢查使用者是否有權限,並開啟視窗
        /// </summary>
        /// <param name="pAdo01"></param>
        private void WfOpenForm(string pAdo01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            ado_tb adoModel;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ado_tb");
                sbSql.AppendLine("WHERE ado01=@ado01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ado01", pAdo01));
                adoModel = BoSecurity.OfGetAdoModel(pAdo01);
                
                if (adoModel == null )
                {
                    WfShowErrorMsg("無此作業代號!");
                    return;
                }
                if (GlobalFn.isNullRet(adoModel.ado07, "") == "M" )
                {
                    //WfShowMsg("無此作業代號!");
                    return;
                }
                if (GlobalFn.isNullRet(adoModel.ado16,"").ToUpper() == "Y") //子程式不可由此進入
                {
                    WfShowErrorMsg("無此作業代號!");
                    return;
                }
                if (BoSecurity.OfChkAddPKExists(LoginInfo.UserRole, pAdo01) == false)
                {
                    WfShowErrorMsg("無此權限!");
                    return;
                }

                WfShowForm(adoModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region WfLoadMenu() 產生menu 僅產生menu下一階的目錄
        private void WfLoadMenu()
        {
            StringBuilder sbSql;
            DataTable dtAdm;
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl uebcc;
            Infragistics.Win.UltraWinTree.UltraTree ut;
            ImageList imgModules;
            string imgModuleKey;
            try
            {
                imgModules = YR.Util.GlobalPictuer.LoadModuleImage();

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM adm_tb");
                sbSql.AppendLine("LEFT JOIN ado_tb ON adm02=ado01");
                sbSql.AppendLine("WHERE adm01='menu'");
                sbSql.AppendLine("  AND ado07='M'");
                sbSql.AppendLine("ORDER BY adm03");
                dtAdm = BoSecurity.OfGetDataTable(sbSql.ToString(), null);
                if (dtAdm == null || dtAdm.Rows.Count == 0)
                    return;
                var i = 0;
                foreach (DataRow drAdm in dtAdm.Rows)
                {
                    UltraExplorerBarGroup uebg = new UltraExplorerBarGroup();
                    uebcc = new UltraExplorerBarContainerControl();

                    ut = new UltraTree();
                    ut.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                    ut.ShowLines = true;
                    //ut.Appearance.BackColor = GetStyleLibrary.TreeBackGroundColor;                    
                    ut.ImageList = ImgList;
                    //ut.Font = new Font(ut.Font.FontFamily, 11);
                    ut.Font = GetStyleLibrary.FontControlNormal;
                    
                    ut.DoubleClick += new EventHandler(UltraTree_DoubleClick);
                    ut.AfterActivate += ut_AfterActivate;
                    ut.KeyDown += new System.Windows.Forms.KeyEventHandler(UltraTree_KeyDown);

                    uebg.Key = drAdm["adm02"].ToString();
                    uebg.Text = drAdm["ado02"].ToString();
                    if (i == 0)//第一筆時要顯示流程圖
                        WfShowFlowByAdo01(uebg.Key);

                    uebg.Settings.Style = Infragistics.Win.UltraWinExplorerBar.GroupStyle.ControlContainer;

                    UebMenu.Groups.Add(uebg);
                    //設定header圖檔,先簡單處理
                    imgModuleKey = "module_" + uebg.Key + "_32";
                    try
                    {
                        uebg.Settings.AppearancesSmall.HeaderAppearance.Image = imgModules.Images[imgModuleKey];
                    }
                    catch
                    {
                    }
                    
                    WfloadTree(uebg.Key, ut, null);
                    if (ut != null)
                    {
                        uebg.Container = uebcc;

                        uebg.Settings.ItemAreaInnerMargins.Bottom = 0;
                        uebg.Settings.ItemAreaInnerMargins.Left = 0;
                        uebg.Settings.ItemAreaInnerMargins.Right = 0;
                        uebg.Settings.ItemAreaInnerMargins.Top = 0;

                        uebg.Settings.ItemAreaOuterMargins.Bottom = 5;
                        uebg.Settings.ItemAreaOuterMargins.Left = 5;
                        uebg.Settings.ItemAreaOuterMargins.Right = 5;
                        uebg.Settings.ItemAreaOuterMargins.Top = 5;
                        ut.Dock = DockStyle.Fill;
                        uebcc.Controls.Add(ut);
                        UebMenu.Controls.Add(uebcc);
                        UebMenu.Groups[0].ExplorerBar.Appearance.BackColor = Color.Red;
                    }
                    i++;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfloadTree 產生樹狀節點
        private void WfloadTree(string pAdm02, UltraTree pUltraTree, UltraTreeNode pUltraTreeNode)
        {
            StringBuilder sbSql;
            DataTable dtAdm;
            UltraTreeNode utn;
            string ado02, ado07, adm01, adm02;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM adm_tb");
                sbSql.AppendLine("LEFT JOIN ado_tb ON adm02=ado01");
                sbSql.AppendLine(string.Format("WHERE adm01='{0}'", pAdm02));
                //sbSql.AppendLine("  AND ado07='P'");
                sbSql.AppendLine("ORDER BY adm03");
                dtAdm = BoSecurity.OfGetDataTable(sbSql.ToString(), null);
                if (dtAdm == null || dtAdm.Rows.Count == 0)
                    return;

                foreach (DataRow drAdm in dtAdm.Rows)
                {
                    ado02 = GlobalFn.isNullRet(drAdm["ado02"], "");
                    ado07 = GlobalFn.isNullRet(drAdm["ado07"], "");
                    adm01 = GlobalFn.isNullRet(drAdm["adm01"], "");
                    adm02 = GlobalFn.isNullRet(drAdm["adm02"], "");
                    if (pUltraTreeNode == null)
                        utn = pUltraTree.Nodes.Add();
                    else
                    {
                        utn = pUltraTreeNode.Nodes.Add();
                    }

                    utn.Key = adm02;
                    if (ado07.ToLower() == "m")//menu
                    {
                        utn.Text = ado02;
                        utn.Override.ActiveNodeAppearance.Image = ImgList.Images[GlobalPictuer.MENU_TREE_FOLDER_ACTIVE];
                        utn.Override.NodeAppearance.Image = ImgList.Images[GlobalPictuer.MENU_TREE_FOLDER];
                        WfloadTree(adm02, pUltraTree, utn);
                    }
                    else
                    {
                        utn.Text = ado02 + " -" + adm02;
                        utn.Override.ActiveNodeAppearance.Image = ImgList.Images[GlobalPictuer.MENU_TREE_FORM_ACTIVE];
                        utn.Override.NodeAppearance.Image = ImgList.Images[GlobalPictuer.MENU_TREE_FORM];
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        
        #region UltraTree 相關event
        private void UltraTree_DoubleClick(object sender, EventArgs e)
        {
            string keyNode;
            UltraTreeNode utnActive;
            try
            {
                utnActive = (sender as UltraTree).ActiveNode;
                keyNode = utnActive.Key;
                WfOpenForm(keyNode);
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }

        void ut_AfterActivate(object sender, NodeEventArgs e)
        {
            string keyNode;
            UltraTreeNode utnActive;
            ado_tb adoModel;
            try
            {
                utnActive = (sender as UltraTree).ActiveNode;
                keyNode = utnActive.Key;
                //取得程式資料
                adoModel = BoSecurity.OfGetAdoModel(keyNode);
                if (adoModel == null || GlobalFn.isNullRet(adoModel.ado14, "") == "")
                    return;
                WfShowFlow(adoModel.ado14);
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }

        private void UltraTree_KeyDown(object sender, KeyEventArgs e)
        {
            string keyNode;
            UltraTreeNode utnActive;
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    utnActive = (sender as UltraTree).ActiveNode;
                    keyNode = utnActive.Key;
                    WfOpenForm(keyNode);
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region Button_Click 事件
        private void Button_Click(object sender, EventArgs e)
        {
            string senderName;
            DialogResult dialogResult;
            string ado01;
            try
            {
                senderName = WfGetControlName(sender);
                switch (senderName.ToLower())
                {
                    case "ubtngo":
                        ado01 = ute_ado01.Text;
                        ute_ado01.Text = "";
                        ute_ado01.Focus();
                        if (ado01.Trim() == "")
                        {
                            return;
                        }
                        WfOpenForm(ado01);
                        break;
                    case "ubtnchangecomp":
                        dialogResult = this.WfShowAdms801(BoSecurity);
                        if (dialogResult == DialogResult.OK)
                        {
                            this.WindowState = FormWindowState.Maximized;
                            WfSetFormTitle("Menu", "主選單", LoginInfo);
                            //WfSetFormTitle("", "", LoginInfo);
                            //WfSetFormTitle(AdoModel.ado01, AdoModel.ado02, LoginInfo);
                            this.Show();
                            this.BringToFront();
                            if (this.IsMdiChild)
                                this.MdiParent.Text=this.Text;
                        }
                        break;
                    case "ubtnnavigator":
                        WfOpenFrmNavigator();
                        break;
                    case "utbrefreshmenu":
                        WfLoadMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                WfShowBottomStatusMsg(ex.Message);
            }
        }
        #endregion

        #region UltraWinExplorBar 相關事件
        private void WfIniUltraWinExplorBar(UltraExplorerBar pUbe)
        {
            try
            {
                //設定 ultraexplorbar 外觀
                pUbe.NavigationPaneExpansionMode = NavigationPaneExpansionMode.OnButtonClick;
                pUbe.NavigationPaneFlyoutSize = new Size(175, 0);
                pUbe.GroupSettings.ExplorerBar.SelectedGroupChanged += ExplorerBar_SelectedGroupChanged;
                pUbe.NavigationOverflowButtonAreaVisible = false;   //下方功能按鈕隱藏
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void ExplorerBar_SelectedGroupChanged(object sender, GroupEventArgs e)
        {
            try
            {
                WfShowFlowByAdo01(e.Group.Key);
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region Ute_ado01_KeyUp
        void Ute_ado01_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == (Keys.Enter))
                    Button_Click(ubtnGo, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion

        #region 顯示流程圖
        void WfShowFlowByAdo01(string pAdo01)
        {
            ado_tb adoModel;
            try
            {
                adoModel = BoSecurity.OfGetAdoModel(pAdo01);
                if (adoModel == null || GlobalFn.varIsNull(adoModel.ado14))
                    return;
                WfShowFlow(adoModel.ado14);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        void WfShowFlow(string pAdf01)
        {
            adx_tb adxModel = null;
            List<ady_tb> adyList = null;
            try
            {
                adxModel = BoSecurity.OfGetAdxModel(pAdf01);
                adyList = BoSecurity.OfGetAdyList(pAdf01);

                if (adxModel.adx03 != null)
                    pbxFlow.Image = Image.FromStream(new MemoryStream(adxModel.adx03));


                //取得panel 中含有 UcTransparentPanel 型別物件,後再全部清除
                var ucPanelList = pnlFlow.Controls
                                         .Cast<Control>()
                                         .Where(p => p.GetType() == typeof(UcTransparentPanel))
                                         ;

                if (ucPanelList != null && ucPanelList.Count() > 0)
                {
                    //全部清除--要倒著刪
                    for (int i = ucPanelList.Count() - 1; i >= 0; i--)
                    {
                        pnlFlow.Controls[i].Dispose();
                    }
                }
                foreach (ady_tb adyModel in adyList)
                {
                    WfIniUcPanel(adyModel);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region panel 相關事件方法
        void WfIniUcPanel(ady_tb pAdyModel)
        {
            UcTransparentPanel panel = new UcTransparentPanel();
            panel.Name = pAdyModel.ady03.ToString();        //以adg03 程式代號做識別
            panel.BorderStyle = BorderStyle.None;

            panel.Location = new Point(pAdyModel.ady04, pAdyModel.ady05);
            panel.Size = new Size(pAdyModel.ady06, pAdyModel.ady07);
            panel.MouseLeave += panel_MouseLeave;
            panel.MouseEnter += panel_MouseEnter;
            panel.MouseClick += panel_MouseClick;
            panel.Cursor = Cursors.Hand;
            panel.AllowResize = false;

            //Label label = new Label();
            //label.Text = panel.Name;
            //label.BackColor = Color.White;
            //label.AutoSize = true;

            //panel.Controls.Add(label);
            pnlFlow.Controls.Add(panel);
            panel.BringToFront();

            panel.Refresh();
        }

        void panel_MouseEnter(object sender, EventArgs e)
        {
            (sender as UcTransparentPanel).BorderColor = Color.Coral;

        }

        void panel_MouseLeave(object sender, EventArgs e)
        {
            (sender as UcTransparentPanel).BorderColor = Color.White;
        }

        void panel_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    WfOpenForm((sender as Panel).Name);
                    (sender as Panel).Refresh();
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }

        //載入panel 屬性
        //void WfResetPanelPosition(UcTransparentPanel pPanel, vw_admi611s pDetailModel)
        //{
        //    try
        //    {
        //        pPanel.Location = new Point(pDetailModel.adg06, pDetailModel.adg07);
        //        pPanel.Size = new Size(pDetailModel.adg06, pDetailModel.adg07);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        #endregion

    }
}
