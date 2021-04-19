/* 程式名稱: FrmReportBase.cs
   系統代號: 
   作    者: Allen
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using Infragistics.Win;
using Infragistics.Win.UltraWinLiveTileView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YR.ERP.DAL.YRModel;
using YR.Util;

namespace YR.ERP.Base.Forms
{
    public partial class FrmNavigator : Form
    {
        #region property
        private List<StaticTile> StaticTileList;
        private GroupByType TitleGroupByType = GroupByType.Moudle;
        private enum GroupByType
        {
            Moudle = 1,
            Company = 2
        }
        ImageList ImgList = YR.Util.GlobalPictuer.LoadNavigatorImage();
        #endregion

        #region 建構子
        public FrmNavigator()
        {
            InitializeComponent();
        }
        #endregion

        #region FrmNavigateor_Load
        private void FrmNavigateor_Load(object sender, EventArgs e)
        {
            //TileGroup tileGroup;
            try
            {
                this.ultraLiveTileView1.TileClick += new Infragistics.Win.UltraWinLiveTileView.TileClickHandler(this.UltraLiveTileView1TileClick);
                this.ultraLiveTileView1.Appearance.BackColor = Color.FromArgb(30, 30, 30);
                this.ultraLiveTileView1.Appearance.BackColor2 = Color.FromArgb(30, 30, 30);
                this.ultraLiveTileView1.ResolutionScale = ResolutionScale.Scale100Percent;
                this.ultraLiveTileView1.SelectionCheckmarkColor = Color.White;

                if (WfGenStaticTileList() == false)
                {
                    this.Close();
                }
                WfGenUltvContet();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
        }
        #region 舊的不使用
        //private void FrmNavigateor_Load(object sender, EventArgs e)
        //{
        //    TileGroup tileGroup;
        //    int i;
        //    Rectangle clientRectangle;
        //    try
        //    {
        //        this.ultraLiveTileView1.TileClick += new Infragistics.Win.UltraWinLiveTileView.TileClickHandler(this.UltraLiveTileView1TileClick);
        //        this.ultraLiveTileView1.Appearance.BackColor = Color.FromArgb(30, 30, 30);
        //        this.ultraLiveTileView1.Appearance.BackColor2 = Color.FromArgb(30, 30, 30);
        //        this.ultraLiveTileView1.ResolutionScale = ResolutionScale.Scale100Percent;
        //        this.ultraLiveTileView1.SelectionCheckmarkColor = Color.White;

        //        tileGroup = new TileGroup();
        //        tileGroup.Key = "Group1";
        //        tileGroup.Text = "視窗導覽";
        //        InitializeGroup(tileGroup);
        //        this.ultraLiveTileView1.Groups.Add(tileGroup);

        //        var actForms = Application.OpenForms.Cast<Form>()
        //                                .Where(p => p.Name != this.Name
        //                                        && p.Name.ToLower() != "frmmain"            //mdi parents
        //                                        && p.Name.ToLower() != "frmprogramlist"     //主選單
        //                                )
        //                                ;

        //        if (actForms == null)
        //            return;

        //        i = 0;
        //        foreach (Form openForm in actForms)
        //        {
        //            YR.ERP.Base.Forms.FrmBase form;
        //            try
        //            {
        //                form = openForm as YR.ERP.Base.Forms.FrmBase;                                                
        //            }
        //            catch (Exception)
        //            {
        //                continue;
        //            }

        //            if (form == null) continue; //有時會抓到null 先這樣處理

        //            Bitmap bm;
        //            StaticTile staticTile;

        //            clientRectangle = new Rectangle();
        //            if (form.WindowState != FormWindowState.Normal)
        //            {
        //                clientRectangle = form.RestoreBounds;
        //            }
        //            else
        //                clientRectangle = form.ClientRectangle;

        //            if (clientRectangle == null) continue; //有時會抓到null 先這樣處理
        //            if (clientRectangle.Width <= 0 || clientRectangle.Height <= 0 || clientRectangle.X < 0 || clientRectangle.Y < 0)
        //            {
        //                continue;
        //            }

        //            bm = new Bitmap(clientRectangle.Width, clientRectangle.Height);
        //            form.DrawToBitmap(bm, clientRectangle);

        //            i++;
        //            staticTile = tileGroup.Tiles.AddStaticTile(string.Format("frm{0}", i.ToString()));                    

        //            if (form.AdoModel != null)
        //            {
        //                staticTile.DefaultView.Text = form.AdoModel.ado02;
        //                staticTile.ToolTipText = form.AdoModel.ado02;
        //            }
        //            else if (form.Name.ToLower() == "frmprogramlist")
        //            {
        //                staticTile.DefaultView.Text = "主選單";
        //                staticTile.ToolTipText = "主選單";
        //            }
        //            else
        //            {
        //                staticTile.DefaultView.Text = "???";
        //                staticTile.ToolTipText = "???";
        //            }

        //            staticTile.Tag = form;
        //            WfAddStaticTilte(tileGroup, staticTile, bm);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        throw ex;
        //    }
        //} 
        #endregion
        #endregion

        #region WfGenStaticTileList
        private bool WfGenStaticTileList()
        {
            //Rectangle clientRectangle;
            StaticTileList = new List<StaticTile>();
            string imageType = "";
            try
            {
                var actForms = Application.OpenForms.Cast<Form>()
                                        .Where(p => p.Name != this.Name
                                                && p.Name.ToLower() != "frmmain"            //mdi parents
                                                && p.Name.ToLower() != "frmprogramlist"     //主選單
                                        )
                                        ;
                if (actForms == null)
                    return false;

                foreach (Form openForm in actForms)
                {
                    YR.ERP.Base.Forms.FrmBase form;
                    try
                    {
                        form = openForm as YR.ERP.Base.Forms.FrmBase;
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (form == null) continue; //有時會抓到null 先這樣處理

                    Image bm = null;
                    StaticTile staticTile;

                    //clientRectangle = new Rectangle();
                    //if (form.WindowState != FormWindowState.Normal)
                    //{
                    //    clientRectangle = form.RestoreBounds;
                    //}
                    //else
                    //    clientRectangle = form.ClientRectangle;

                    //if (clientRectangle == null) continue; //有時會抓到null 先這樣處理
                    //if (clientRectangle.Width <= 0 || clientRectangle.Height <= 0 || clientRectangle.X < 0 || clientRectangle.Y < 0)
                    //{
                    //    continue;
                    //}

                    //bm = new Bitmap(clientRectangle.Width, clientRectangle.Height);
                    //form.DrawToBitmap(bm, clientRectangle);

                    //i++;
                    //staticTile = tileGroup.Tiles.AddStaticTile(string.Format("frm{0}", i.ToString()));

                    staticTile = new StaticTile();

                    if (form.AdoModel != null)
                    {
                        staticTile.DefaultView.Text = form.AdoModel.ado02;
                        staticTile.ToolTipText = string.Concat(form.LoginInfo.CompNo, "-", form.AdoModel.ado02);

                        imageType = "";
                        switch (form.AdoModel.ado07)
                        {
                            case "M":
                                imageType = GlobalPictuer.NAVIGATOR_MENU;
                                break;
                            case "P":
                                var ado01 = form.AdoModel.ado01;
                                if (ado01.Length < 4)
                                    break;
                                var programType = ado01.Substring(ado01.Length - 4, 1);
                                switch (programType.ToLower())
                                {
                                    case "q":
                                        imageType = GlobalPictuer.NAVIGATOR_QUERY;
                                        break;
                                    case "s":
                                        imageType = GlobalPictuer.NAVIGATOR_PARAMETER;
                                        break;
                                    case "i":
                                        imageType = GlobalPictuer.NAVIGATOR_FILING;
                                        break;
                                    case "t":
                                        imageType = GlobalPictuer.NAVIGATOR_RECEIPT;
                                        break;
                                    case "b":
                                        imageType = GlobalPictuer.NAVIGATOR_BATCH;
                                        break;
                                }
                                break;
                            case "R":
                                imageType = GlobalPictuer.NAVIGATOR_REPORT;
                                break;
                        }
                    }
                    else
                    {
                        staticTile.DefaultView.Text = "???";
                        staticTile.ToolTipText = "???";
                    }

                    staticTile.Tag = form;
                    if (GlobalFn.varIsNull(imageType))
                        bm = null;
                    else
                        bm = ImgList.Images[imageType];
                    WfSetStaticTilteApperance(staticTile, bm);
                    StaticTileList.Add(staticTile);
                }
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfGenUltvContet
        private void WfGenUltvContet()
        {
            TileGroup tileGroup;
            try
            {
                foreach (TileGroup tg in this.ultraLiveTileView1.Groups)
                {
                    tg.Tiles.Clear();
                }
                this.ultraLiveTileView1.Groups.Clear();

                if (TitleGroupByType == GroupByType.Company)
                {
                    var query = from staticTile in StaticTileList
                                group staticTile by (staticTile.Tag as YR.ERP.Base.Forms.FrmBase).LoginInfo.CompNo;
                    ;
                    foreach (var q in query)
                    {
                        tileGroup = new TileGroup();
                        tileGroup.Key = q.Key;
                        tileGroup.Text = q.Key;
                        WfSetGroupApperance(tileGroup);
                        this.ultraLiveTileView1.Groups.Add(tileGroup);
                        foreach (var item in q)
                        {
                            tileGroup.Tiles.Add(item);
                        }
                    }
                }
                else
                {
                    var query = from staticTile in StaticTileList
                                group staticTile by (staticTile.Tag as YR.ERP.Base.Forms.FrmBase).AdoModel.ado12;
                    ;
                    foreach (var q in query)
                    {
                        tileGroup = new TileGroup();
                        tileGroup.Key = q.Key;
                        switch (q.Key.ToLower())
                        {
                            case "adm":
                                tileGroup.Text = "管理";
                                break;
                            case "bas":
                                tileGroup.Text = "基本";
                                break;
                            case "inv":
                                tileGroup.Text = "庫存";
                                break;
                            case "stp":
                                tileGroup.Text = "銷貨";
                                break;
                            case "pur":
                                tileGroup.Text = "採購";
                                break;
                            case "man":
                                tileGroup.Text = "生產";
                                break;
                            case "car":
                                tileGroup.Text = "應收";
                                break;
                            case "gla":
                                tileGroup.Text = "總帳";
                                break;
                            case "tax":
                                tileGroup.Text = "稅務";
                                break;
                            default:
                                tileGroup.Text = q.Key;
                                break;

                        }
                        //tileGroup.Text = q.Key;
                        WfSetGroupApperance(tileGroup);
                        this.ultraLiveTileView1.Groups.Add(tileGroup);
                        foreach (var item in q)
                        {
                            tileGroup.Tiles.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetGroupApperance
        public void WfSetGroupApperance(TileGroup currentGroup)
        {
            currentGroup.TextAreaAppearance.HotTracking.BorderColor = Color.FromArgb(58, 58, 58);
            currentGroup.Appearance.HotTracking.BorderColor = Color.FromArgb(58, 58, 58);
            currentGroup.Appearance.HotTracking.BackColor = Color.FromArgb(30, 30, 30);
            currentGroup.Appearance.HotTracking.BackGradientStyle = GradientStyle.None;
            currentGroup.Appearance.Normal.BackColor = Color.FromArgb(30, 30, 30);
            currentGroup.Appearance.Normal.BackGradientStyle = GradientStyle.None;
        }
        #endregion

        #region WfSetStaticTilteApperance
        private void WfSetStaticTilteApperance(StaticTile pStaticTitle, Image pImage)
        {
            pStaticTitle.Appearance.Normal.BackColor = Color.FromArgb(69, 69, 69);
            pStaticTitle.Appearance.Normal.BackColor2 = Color.FromArgb(85, 85, 85);
            pStaticTitle.Appearance.Normal.BorderColor = Color.FromArgb(125, 125, 125);
            pStaticTitle.Appearance.Selected.BorderColor = Color.FromArgb(38, 115, 236);
            pStaticTitle.Appearance.HotTracking.BorderColor = Color.FromArgb(58, 58, 58);
            pStaticTitle.CurrentSize = TileSize.Medium;

            pStaticTitle.DefaultView.Image.AllResolutions.Image = pImage;
        }
        #endregion

        //#region WfAddStaticTilte
        //private void WfAddStaticTilte(TileGroup pTg, StaticTile pStaticTitle, Bitmap pBitmap)
        //{
        //    pStaticTitle.Appearance.Normal.BackColor = Color.FromArgb(69, 69, 69);
        //    pStaticTitle.Appearance.Normal.BackColor2 = Color.FromArgb(85, 85, 85);
        //    pStaticTitle.Appearance.Normal.BorderColor = Color.FromArgb(125, 125, 125);
        //    pStaticTitle.Appearance.Selected.BorderColor = Color.FromArgb(38, 115, 236);
        //    pStaticTitle.Appearance.HotTracking.BorderColor = Color.FromArgb(58, 58, 58);
        //    pStaticTitle.CurrentSize = TileSize.Medium;

        //    pStaticTitle.DefaultView.Image.AllResolutions.Image = pBitmap;
        //}
        //#endregion

        #region UltraLiveTileView1TileClick
        private void UltraLiveTileView1TileClick(object sender, TileClickEventArgs e)
        {
            Form openForm;
            try
            {
                openForm = e.Tile.Tag as Form;
                this.Hide();
                openForm.Show();
                openForm.BringToFront();
                openForm.WindowState = FormWindowState.Maximized;
                this.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion UltraLiveTileView1TileClick

        #region override ProcessCmdKey 註冊表單熱鍵
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Escape))
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region btnChangeGroup_Click
        private void btnChangeGroup_Click(object sender, EventArgs e)
        {
            try
            {
                if (TitleGroupByType == GroupByType.Company)
                {
                    TitleGroupByType = GroupByType.Moudle;
                    btnChangeGroup.Text = "依公司分類";
                }
                else
                {
                    TitleGroupByType = GroupByType.Company;
                    btnChangeGroup.Text = "依模組分類";
                }
                WfGenUltvContet();
            }
            catch (Exception ex)
            {
                this.Close();
            }
        }
        #endregion

    }

    //public class ControlPainter
    //{
    //    private const int
    //        WM_PRINT = 0x317, PRF_CLIENT = 4,
    //        PRF_CHILDREN = 0x10, PRF_NON_CLIENT = 2,
    //        COMBINED_PRINTFLAGS = PRF_CLIENT | PRF_CHILDREN | PRF_NON_CLIENT;

    //    [DllImport("USER32.DLL")]
    //    private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, int lParam);

    //    public static void PaintControl(Graphics graphics, Control control)
    //    { // paint control onto graphics
    //        IntPtr hWnd = control.Handle;
    //        IntPtr hDC = graphics.GetHdc();
    //        SendMessage(hWnd, WM_PRINT, hDC, COMBINED_PRINTFLAGS);
    //        graphics.ReleaseHdc(hDC);
    //    }
    //}
}
