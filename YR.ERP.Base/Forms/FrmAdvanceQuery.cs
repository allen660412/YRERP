/* 程式名稱: 進階查詢底層視窗
   系統代號: FrmAdvanceQuery
   作　　者: Allen
   描　　述: 
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/

using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
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
using YR.ERP.BLL.Model;
using YR.ERP.DAL.YRModel;
using YR.ERP.Shared;
using YR.Util;

namespace YR.ERP.Base.Forms
{
    public partial class FrmAdvanceQuery : FrmBase
    {
        #region Property & Field
        private AdvanceQueryInfo tabMaster;
        public AdvanceQueryInfo TabMaster
        {
            get { return tabMaster; }
        }


        private List<AdvanceQueryInfo> tabDetailList;
        public List<AdvanceQueryInfo> TabDetailList
        {
            get { return tabDetailList; }
        }

        private int intCurTab = 0;                                            //設定停留在那個頁面
        private int intTabCount = 1;                                          //tab的數量 主檔0 如果有明細由1開始
        protected YR.ERP.BLL.MSSQL.AdmBLL BoMaster;                           //由使用者登入公司,取得連線Common Objoct
        private bool firstInit = true;                                        //是否為第一次啟動
        #endregion

        #region 建構式
        public FrmAdvanceQuery()    //基礎建構式不使用
        {
            InitializeComponent();
        }

        public FrmAdvanceQuery(UserInfo pUserInfoModel, AdvanceQueryInfo pTabMaster, List<AdvanceQueryInfo> pTabDetailList)
        {
            InitializeComponent();
            tabMaster = pTabMaster;
            tabDetailList = pTabDetailList;


            this.LoginInfo = pUserInfoModel;
        }
        #endregion

        #region FrmAdvanceQuery_Load
        private void FrmAdvanceQuery_Load(object sender, EventArgs e)
        {
            try
            {
                if (firstInit)
                {
                    this.Shown += FrmAdvanceQuery_Shown;

                    WfIniVar();
                    WfIniToolBarUI();
                    if (WfIniForm() == false)
                        return;

                    if (WfBindingDataSource(tabMaster) == false)
                        return;
                    if (tabDetailList != null)
                    {
                        foreach (AdvanceQueryInfo advInfo in tabDetailList)
                        {
                            if (WfBindingDataSource(advInfo) == false)
                                return;
                        }
                    }
                    WfDisplayMode();
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
            finally
            {
            }
        }

        void FrmAdvanceQuery_Shown(object sender, EventArgs e)
        {
            try
            {
                refreshCombo();
                //if (firstInit)
                //{
                //    //combobox 有無法定位到第一筆的問題,先暫時這樣處理
                //    tabMaster.UComboColumn.PerformAction(UltraComboAction.FirstRow);
                //    tabMaster.UComboCondition.PerformAction(UltraComboAction.FirstRow);

                //    if (tabDetailList!=null)
                //    {
                //        foreach (AdvanceQueryInfo advQueryInfo in tabDetailList)
                //        {
                //            advQueryInfo.UComboColumn.PerformAction(UltraComboAction.FirstRow);
                //            advQueryInfo.UComboCondition.PerformAction(UltraComboAction.FirstRow);
                //        }
                //    }

                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                firstInit = false;
            }

        }
        #endregion

        #region  WfIniVar時設定變數
        private void WfIniVar()
        {
            if (BoMaster == null)
            {
                BoMaster = new YR.ERP.BLL.MSSQL.AdmBLL(LoginInfo.CompNo, "", "", "");
            }
            if (tabDetailList == null)
            {
                intTabCount = 1;
            }
            else
            {
                intTabCount = 1 + tabDetailList.Count;
            }
        }
        #endregion

        #region WfIniForm 初始化表單相關物件
        private bool WfIniForm()
        {
            try
            {
                if (WfIniTab() == false)
                    return false;

                WfIniDataSource(tabMaster);
                if (tabDetailList != null)
                {
                    foreach (AdvanceQueryInfo advQueryInfo in tabDetailList)
                    {
                        WfIniDataSource(advQueryInfo);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniTab
        private bool WfIniTab()
        {
            try
            {
                var uTab = uTabControl.Tabs.Add(tabMaster.Key);
                uTab.Text = "主檔";
                if (WfIniTabPageUI(uTab, tabMaster) == false)
                    return false;
                uTabControl.SelectedTabChanged += uTabControl_SelectedTabChanged;

                if (tabDetailList != null)
                {
                    foreach (AdvanceQueryInfo advInfo in tabDetailList)
                    {
                        var uTabDetail = uTabControl.Tabs.Add(advInfo.Key);
                        var i = 1;
                        uTabDetail.Text = string.Format("明細{0}", i);

                        if (WfIniTabPageUI(uTabDetail, advInfo) == false)
                            return false;
                        i++;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region uTabControl_SelectedTabChanged
        void uTabControl_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            try
            {
                this.intCurTab = e.Tab.Index;
                refreshCombo();
            }
            catch (Exception ex)
            {
                var errMsg = new StringBuilder();
                errMsg.AppendLine("uTabControl_SelectedTabChanged Eror");
                errMsg.AppendLine(string.Format(ex.Message));
                WfShowErrorMsg(errMsg.ToString());
            }
        }
        #endregion

        #region WfIniTabPageUI()
        private bool WfIniTabPageUI(UltraTab pUTab, AdvanceQueryInfo pTabInfo)
        {
            Panel panel;
            UltraGroupBox UGroupBox;
            RadioButton RadioAnd;
            RadioButton RadioOr;
            UltraCombo UComboColumn;
            UltraCombo UComboCondition;
            UltraTextEditor UTextValue;
            UltraGrid UGrid;
            try
            {
                //var pUTab = uTabControl.Tabs.Add("11");
                //pUTab.Text = "測試";
                panel = new Panel();
                panel.Height = 100;
                panel.Dock = DockStyle.Top;

                UGroupBox = new UltraGroupBox();
                UGroupBox.Size = new Size(160, 45);
                UGroupBox.Location = new Point(15, 10);
                UGroupBox.Text = "且/或";
                if (GetStyleLibrary.FontControlNormal != null)
                {
                    UGroupBox.Font = GetStyleLibrary.FontControlNormal;

                }

                RadioAnd = new RadioButton();
                RadioAnd.Checked = true;
                RadioAnd.Size = new Size(60, 20);
                RadioAnd.Location = new Point(20, 18);
                RadioAnd.Text = "AND";
                if (GetStyleLibrary.FontControlNormal != null)
                {
                    RadioAnd.Font = GetStyleLibrary.FontControlNormal;

                }
                UGroupBox.Controls.Add(RadioAnd);

                RadioOr = new RadioButton();
                RadioOr.Checked = false;
                RadioOr.Size = new Size(60, 20);
                RadioOr.Location = new Point(80, 18);
                RadioOr.Text = "OR";
                if (GetStyleLibrary.FontControlNormal != null)
                {
                    RadioOr.Font = GetStyleLibrary.FontControlNormal;

                }
                UGroupBox.Controls.Add(RadioOr);
                panel.Controls.Add(UGroupBox);

                UComboColumn = new UltraCombo();
                UComboColumn.Size = new Size(160, 22);
                UComboColumn.Location = new Point(20, 60);
                if (GetStyleLibrary.FontControlNormal != null)
                {
                    UComboColumn.Font = GetStyleLibrary.FontControlNormal;

                }
                panel.Controls.Add(UComboColumn);

                UComboCondition = new UltraCombo();
                UComboCondition.Size = new Size(80, 22);
                UComboCondition.Location = new Point(190, 60);
                if (GetStyleLibrary.FontControlNormal != null)
                {
                    UComboCondition.Font = GetStyleLibrary.FontControlNormal;

                }
                panel.Controls.Add(UComboCondition);

                UTextValue = new UltraTextEditor();
                UTextValue.Size = new Size(260, 22);
                UTextValue.Location = new Point(280, 60);
                if (GetStyleLibrary.FontControlNormal != null)
                {
                    UTextValue.Font = GetStyleLibrary.FontControlNormal;

                }
                panel.Controls.Add(UTextValue);

                UGrid = new UltraGrid();
                UGrid.Dock = DockStyle.Fill;

                pUTab.TabPage.Controls.Add(UGrid);
                pUTab.TabPage.Controls.Add(panel);

                //ref 到tab
                pTabInfo.UGroupBox = UGroupBox;
                pTabInfo.RadioAnd = RadioAnd;
                pTabInfo.RadioOr = RadioOr;
                pTabInfo.UComboColumn = UComboColumn;
                pTabInfo.UComboCondition = UComboCondition;
                pTabInfo.UTextValue = UTextValue;
                pTabInfo.UGrid = UGrid;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region WfIniToolBarUI 初始化表單工具列--設定圖案及熱鍵
        protected virtual void WfIniToolBarUI()
        {
            ImageList ilLarge = new ImageList();
            string lsBtKey;
            try
            {
                UtbmMain.Style = GetStyleLibrary.UltraWinToolBarDiplayStyle;

                UtbmMain.UseAppStyling = false;
                UtbmMain.Office2007UICompatibility = false;
                UtbmMain.UseOsThemes = DefaultableBoolean.True;
                UtbmMain.Style = ToolbarStyle.Office2013;
                UtbmMain.Ribbon.QuickAccessToolbar.Visible = false;

                UtbmMain.Ribbon.FileMenuStyle = FileMenuStyle.None;
                UtbmMain.Ribbon.CaptionAreaAppearance.BackColor = Color.FromArgb(210, 210, 210);
                UtbmMain.Ribbon.FileMenuButtonAppearance.BackColor = Color.FromArgb(67, 168, 152);
                UtbmMain.Ribbon.FileMenuButtonAppearance.BackGradientStyle = GradientStyle.None;

                UtbmMain.Ribbon.GroupSettings.CaptionAppearance.FontData.SizeInPoints = 8.5f;
                UtbmMain.Ribbon.GroupSettings.CaptionAppearance.ForeColor = Color.White;

                UtbmMain.Ribbon.GroupSettings.HotTrackAppearance.BackColor = Color.FromArgb(66, 126, 123);
                UtbmMain.Ribbon.GroupSettings.HotTrackAppearance.BackGradientStyle = GradientStyle.None;
                UtbmMain.Ribbon.GroupSettings.HotTrackAppearance.BorderAlpha = Alpha.Opaque;
                UtbmMain.Ribbon.GroupSettings.HotTrackAppearance.BorderColor = Color.FromArgb(67, 168, 152);
                UtbmMain.Ribbon.GroupSettings.HotTrackAppearance.ForeColor = Color.White;
                UtbmMain.Ribbon.GroupSettings.ToolAppearance.ForeColor = Color.FromArgb(179, 187, 191);

                UtbmMain.Ribbon.TabAreaAppearance.BackColor = Color.FromArgb(67, 168, 152);

                UtbmMain.Ribbon.TabSettings.Appearance.BackColor = Color.FromArgb(66, 84, 94);
                UtbmMain.Ribbon.TabSettings.Appearance.BackGradientStyle = GradientStyle.None;
                UtbmMain.Ribbon.TabSettings.Appearance.BorderAlpha = Alpha.Transparent;
                UtbmMain.Ribbon.TabSettings.Appearance.ForeColor = Color.White;

                UtbmMain.Ribbon.TabSettings.SelectedAppearance.BackColor = Color.FromArgb(66, 84, 94);
                UtbmMain.Ribbon.TabSettings.SelectedAppearance.BorderAlpha = Alpha.Transparent;

                UtbmMain.Ribbon.TabSettings.TabItemAppearance.BackColor = Color.FromArgb(67, 168, 152);

                UtbmMain.MdiMergeable = false;
                ilLarge = GlobalPictuer.LoadToolBarImage();
                if (ilLarge == null)
                    return;
                UtbmMain.ImageListLarge = ilLarge;

                #region 產生RibbonTab/及Group
                RibbonTab RtData = new RibbonTab("RtData", "資料");
                UtbmMain.Ribbon.Tabs.AddRange(new RibbonTab[] { RtData });

                RibbonGroup RibgCrud = new RibbonGroup("RibgCrud", "資料存取");
                RibbonGroup RibgDecide = new RibbonGroup("RibgDecide", "處理");
                RibbonGroup RibgNav = new RibbonGroup("RibgNav", "導覽");
                RtData.Groups.AddRange(new RibbonGroup[] { RibgCrud, RibgDecide, RibgNav });
                #endregion

                #region RtData/RibgCrud 相關按鈕
                lsBtKey = "BtAdd";
                var BtAdd = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtAdd);
                RibgCrud.Tools.AddTool(lsBtKey);
                RibgCrud.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtAdd.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_INSERT_DETAIL];
                //BtAll.SharedPropsInternal.Shortcut = Shortcut.CtrlQ;
                BtAdd.SharedProps.Caption = "加入條件";

                lsBtKey = "BtDelete";
                var BtDelete = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtDelete);
                RibgCrud.Tools.AddTool(lsBtKey);
                RibgCrud.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtDelete.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_DEL_DETAIL];
                //BtNone.SharedPropsInternal.Shortcut = Shortcut.CtrlQ;
                BtDelete.SharedProps.Caption = "刪除條件";

                lsBtKey = "BtCleanAll";
                var BtCleanAll = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtCleanAll);
                RibgCrud.Tools.AddTool(lsBtKey);
                RibgCrud.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtCleanAll.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_ERASER];
                //BtNone.SharedPropsInternal.Shortcut = Shortcut.CtrlQ;
                BtCleanAll.SharedProps.Caption = "全部清除";
                #endregion

                #region RtData/RibgDecide 相關按鈕
                lsBtKey = "BtOk";
                var BtOk = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtOk);
                RibgDecide.Tools.AddTool(lsBtKey);
                RibgDecide.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtOk.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["ok_32"];
                BtOk.SharedProps.Caption = "確 認";

                lsBtKey = "BtCancel";
                var BtCancel = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtCancel);
                RibgDecide.Tools.AddTool(lsBtKey);
                RibgDecide.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtCancel.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["cancel_32"];
                BtCancel.SharedProps.Caption = "取 消";

                #endregion

                #region RtData/RibgNav 相關按鈕
                lsBtKey = "BtFirst";
                var BtFirst = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtFirst);
                RibgNav.Tools.AddTool(lsBtKey);
                RibgNav.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtFirst.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_FIRST];
                BtFirst.SharedPropsInternal.Shortcut = Shortcut.CtrlF;
                BtFirst.SharedProps.Caption = "首 筆";

                lsBtKey = "BtPrev";
                var BtPrev = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtPrev);
                RibgNav.Tools.AddTool(lsBtKey);
                RibgNav.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtPrev.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_PREVIOUS];
                BtPrev.SharedPropsInternal.Shortcut = Shortcut.CtrlP;
                BtPrev.SharedProps.Caption = "上一筆";

                lsBtKey = "BtNext";
                var BtNext = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtNext);
                RibgNav.Tools.AddTool(lsBtKey);
                RibgNav.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtNext.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_NEXT];
                BtNext.SharedPropsInternal.Shortcut = Shortcut.CtrlN;
                BtNext.SharedProps.Caption = "下一筆";

                lsBtKey = "BtEnd";
                var BtEnd = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtEnd);
                RibgNav.Tools.AddTool(lsBtKey);
                RibgNav.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtEnd.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_END];
                BtEnd.SharedPropsInternal.Shortcut = Shortcut.CtrlL;
                BtEnd.SharedProps.Caption = "末 筆";
                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region UtbmMain_ToolClick 工具列 click 事件
        protected virtual void UtbmMain_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            AdvanceQueryInfo activeTabInfo;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                if (intCurTab == 0)
                    activeTabInfo = tabMaster;
                else
                    activeTabInfo = tabDetailList[intCurTab - 1];

                WfCleanBottomMsg();
                switch (e.Tool.Key.ToLower())
                {
                    case "btfirst":

                        var sss = activeTabInfo.UComboColumn;

                        activeTabInfo.UGrid.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.FirstRowInGrid);
                        break;
                    case "btprev":
                        activeTabInfo.UGrid.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.PrevRow);
                        break;
                    case "btnext":
                        activeTabInfo.UGrid.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.NextRow);
                        break;
                    case "btend":
                        activeTabInfo.UGrid.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.LastRowInBand);
                        break;

                    case "btok":
                        activeTabInfo.UGrid.UpdateData();
                        //uGridMaster.UpdateData();
                        WfReturnOk();
                        break;

                    case "btcancel":
                        tabMaster.Result = DialogResult.Cancel;
                        this.Hide();
                        break;
                    case "btadd":
                        WfAdd(activeTabInfo);
                        break;
                    case "btdelete":
                        if (activeTabInfo.UGrid.ActiveRow == null)
                            break;

                        var activeRow = activeTabInfo.UGrid.ActiveRow;
                        var activeIndex = activeRow.VisibleIndex;
                        activeRow.Delete(false);
                        if (activeTabInfo.UGrid.Rows.Count == 0)
                            break;

                        //考慮最後一筆的情形時
                        if (activeIndex + 1 > activeTabInfo.UGrid.Rows.Count)
                            activeIndex = activeIndex - 1;

                        activeTabInfo.UGrid.ActiveRow = activeTabInfo.UGrid.Rows.GetRowAtVisibleIndex(activeIndex);
                        break;
                    case "btcleanall":
                        WfCleanAll(activeTabInfo);

                        break;

                }

            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region WfIniDataSource
        private bool WfIniDataSource(AdvanceQueryInfo pAdvQueryInfo)
        {
            UltraGrid uGrid;
            try
            {
                uGrid = pAdvQueryInfo.UGrid;

                if (GlobalFn.varIsNull(pAdvQueryInfo.Key))
                {
                    WfShowErrorMsg("此查詢代碼,請檢核");
                    return false;
                }
                var masterAzaList = BoMaster.OfGetAzaModels(pAdvQueryInfo.Key)
                                        .Where(p => p.aza05.ToUpper() == "Y")
                                        .ToList();
                if (masterAzaList == null)
                {
                    WfShowErrorMsg("無此查詢代碼,請檢核");
                    return false;
                }
                else
                {
                    pAdvQueryInfo.ConditionList = new List<ConditionInfo>();
                    var detailAzaList = BoMaster.OfGetAzaModels(pAdvQueryInfo.Key)
                                            .Where(p => p.aza05.ToUpper() == "Y")
                                            .ToList();
                    foreach (aza_tb azaModel in detailAzaList)
                    {
                        pAdvQueryInfo.ConditionList.Add(new ConditionInfo()
                        {
                            TableName = azaModel.aza01,
                            Condition = "",
                            ColumnName = azaModel.aza03,
                            ColumnDesc = azaModel.aza04,
                            ColumnType = azaModel.aza08,
                            Value = null,
                            orderSeq = azaModel.aza06
                        }
                            );
                    }
                }
                //初始化主檔GRID
                uGrid.DataSource = new List<ConditionInfo>();
                uGrid.DataBind();
                WfSetGridHeader(uGrid);
                WfSetAppearance(uGrid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        #endregion

        #region WfBindingDataSource 繫結相關元件的資料來源
        private bool WfBindingDataSource(AdvanceQueryInfo pAdvQueryInfo)
        {
            UltraGridColumn ugc;
            List<KeyValuePair<string, string>> columnList;
            List<KeyValuePair<string, string>> conditionList;
            List<KeyValuePair<string, string>> andOrList;
            try
            {

                columnList = new List<KeyValuePair<string, string>>();
                foreach (ConditionInfo conditionInfo in pAdvQueryInfo.ConditionList.OrderBy(p => p.orderSeq))
                {
                    columnList.Add(new KeyValuePair<string, string>(
                        conditionInfo.ColumnName,
                        conditionInfo.ColumnDesc
                        ));
                }
                conditionList = WfGetConditionList();
                andOrList = WfGetAndOrList();

                WfSetUcomboxDataSource(pAdvQueryInfo.UComboColumn, columnList);
                
                WfSetUcomboxDataSource(pAdvQueryInfo.UComboCondition, conditionList);
                //pAdvQueryInfo.UComboCondition.DropDownWidth = 80;
                pAdvQueryInfo.UComboCondition.DisplayLayout.Bands[0].Columns[1].Width = 80;
                pAdvQueryInfo.UComboCondition.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                pAdvQueryInfo.UComboCondition.PerformAction(UltraComboAction.FirstRow);

                #region Grid column binding
                ugc = pAdvQueryInfo.UGrid.DisplayLayout.Bands[0].Columns["condition"];
                WfSetGridValueList(ugc, conditionList);
                (ugc.ValueList as UltraDropDown).DisplayLayout.Bands[0].Columns[0].Hidden = true;
                (ugc.ValueList as UltraDropDown).DropDownWidth = 100;

                ugc = pAdvQueryInfo.UGrid.DisplayLayout.Bands[0].Columns["AndOr"];
                WfSetGridValueList(ugc, andOrList);
                (ugc.ValueList as UltraDropDown).DisplayLayout.Bands[0].Columns[0].Hidden = true;
                (ugc.ValueList as UltraDropDown).DropDownWidth = 100;
                #endregion
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfDisplayMode
        protected override bool WfDisplayMode()
        {
            UltraGrid uGrid;
            try
            {
                //主表gridreadonly處理
                uGrid = tabMaster.UGrid;
                foreach (UltraGridColumn ugc in uGrid.DisplayLayout.Bands[0].Columns)
                {
                    if (ugc.Key.ToLower() == "andor" ||
                        ugc.Key.ToLower() == "condition" ||
                        ugc.Key.ToLower() == "value"
                        )
                    {
                        WfSetControlReadonly(ugc, false);
                        continue;
                    }

                    WfSetControlReadonly(ugc, true);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region WfAdd 加入查詢條件
        private void WfAdd(AdvanceQueryInfo pAdvQueryInfo)
        {
            UltraGrid uGrid;
            try
            {
                //lugrid = uGridMaster;
                uGrid = pAdvQueryInfo.UGrid;
                var addConditionInfo = new ConditionInfo();
                var idMax = (from o in (uGrid.DataSource as List<ConditionInfo>)
                             orderby o.id descending
                             select o.id
                            ).FirstOrDefault()
                            ;

                addConditionInfo.id = idMax + 1;
                addConditionInfo.TableName = pAdvQueryInfo.ConditionList.FirstOrDefault().TableName;
                addConditionInfo.ColumnName = pAdvQueryInfo.UComboColumn.Value.ToString();
                addConditionInfo.ColumnDesc = pAdvQueryInfo.UComboColumn.Text;
                addConditionInfo.Condition = pAdvQueryInfo.UComboCondition.Value.ToString();
                addConditionInfo.Value = pAdvQueryInfo.UTextValue.Value;
                addConditionInfo.ColumnType = "";
                if (pAdvQueryInfo.RadioAnd.Checked == true)
                    addConditionInfo.AndOr = "AND";
                else
                    addConditionInfo.AndOr = "OR";

                (uGrid.DataSource as List<ConditionInfo>).Add(addConditionInfo);
                uGrid.DataBind();
                //直接定位在最後一列

                // 檢查要設定的 active 的 row 編號不可大於全部可見的 row 筆數
                if (uGrid.Rows.VisibleRowCount > 0)
                {
                    //lugrid.ActiveRow = lugrid.Rows.GetRowAtVisibleIndex(lugrid.Rows.VisibleRowCount - 1);
                    uGrid.ActiveRow = uGrid.Rows.GetRowWithListIndex(uGrid.Rows.VisibleRowCount - 1);
                }
                pAdvQueryInfo.UTextValue.Value = "";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfCleanAll() 清除所有的查詢條件
        private void WfCleanAll(AdvanceQueryInfo pAdvQueryInfo)
        {
            (pAdvQueryInfo.UGrid.DataSource as List<ConditionInfo>).Clear();
            pAdvQueryInfo.UGrid.DataBind();
        }
        #endregion

        #region WfReturnOk
        private void WfReturnOk()
        {
            try
            {
                StringBuilder sbWhere;
                List<SqlParameter> sqlParmList;
                var masterConditionList = (tabMaster.UGrid.DataSource as List<ConditionInfo>);
                sbWhere = new StringBuilder();
                sqlParmList = new List<SqlParameter>();
                if (masterConditionList != null)
                {
                    foreach (ConditionInfo c in masterConditionList)
                    {
                        var parmName = string.Format("@{0}", c.ColumnName + "_" + c.id.ToString());

                        sbWhere.AppendLine(string.Format("{0} {1}.{2} {3} {4}",
                                                                c.AndOr,
                                                                c.TableName,
                                                                c.ColumnName,
                                                                c.Condition,
                                                                parmName));
                        sqlParmList.Add(new SqlParameter(parmName, c.Value));
                    }
                    tabMaster.StrWhereAppend = sbWhere.ToString();
                    tabMaster.sqlParmsList = sqlParmList;
                }
                                
                if (tabDetailList!=null)
                {
                    foreach(AdvanceQueryInfo advQueryInfo in tabDetailList)
                    {
                        var uGridDatasource = advQueryInfo.UGrid.DataSource;
                        if (uGridDatasource==null)
                            continue;
                        var detailConditionList=uGridDatasource as List<ConditionInfo>;

                        if (detailConditionList.Count == 0)
                            continue;

                        sbWhere = new StringBuilder();
                        sqlParmList = new List<SqlParameter>();

                        //條件統一寫至tabMaster裡     
                        sbWhere.AppendLine(string.Format("AND EXISTS ("));
                        sbWhere.AppendLine(string.Format("SELECT 1 FROM {0}", advQueryInfo.Key));
                        sbWhere.AppendLine(string.Format("WHERE 1=1 "));
                        foreach (SqlParameter sp in advQueryInfo.RelationParams)
                        {
                            sbWhere.AppendLine(string.Format("AND {0}.{1} ={2}.{3} ", TabMaster.Key, sp.SourceColumn, advQueryInfo.Key, sp.ParameterName));
                        }

                        foreach (ConditionInfo c in detailConditionList)
                        {
                            var parmName = string.Format("@{0}", c.ColumnName + "_" + c.id.ToString());

                            sbWhere.AppendLine(string.Format("{0} {1}.{2} {3} {4}",
                                                                    c.AndOr,
                                                                    c.TableName,
                                                                    c.ColumnName,
                                                                    c.Condition,
                                                                    parmName));
                            sqlParmList.Add(new SqlParameter(parmName, c.Value));
                        }

                        sbWhere.AppendLine(string.Format(")"));
                        
                        tabMaster.StrWhereAppend += sbWhere.ToString();
                        tabMaster.sqlParmsList.AddRange(sqlParmList);
                    }
                }

                tabMaster.Result = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region WfSetGridHeader
        private void WfSetGridHeader(UltraGrid pUgrid)
        {
            try
            {
                if (pUgrid.DataSource == null)
                    return;
                if (pUgrid.DataSource.GetType() != typeof(List<ConditionInfo>))
                    return;
                var conditionInfo = (pUgrid.DataSource as List<ConditionInfo>);
                //先排順序
                pUgrid.DisplayLayout.Bands[0].Columns["id"].Header.VisiblePosition = 1;
                pUgrid.DisplayLayout.Bands[0].Columns["AndOr"].Header.VisiblePosition = 2;
                pUgrid.DisplayLayout.Bands[0].Columns["TableName"].Header.VisiblePosition = 3;
                pUgrid.DisplayLayout.Bands[0].Columns["ColumnName"].Header.VisiblePosition = 4;
                pUgrid.DisplayLayout.Bands[0].Columns["ColumnDesc"].Header.VisiblePosition = 5;
                pUgrid.DisplayLayout.Bands[0].Columns["Condition"].Header.VisiblePosition = 6;
                pUgrid.DisplayLayout.Bands[0].Columns["Value"].Header.VisiblePosition = 7;
                pUgrid.DisplayLayout.Bands[0].Columns["ColumnType"].Header.VisiblePosition = 8;
                pUgrid.DisplayLayout.Bands[0].Columns["orderSeq"].Header.VisiblePosition = 9;

                //grid頭
                pUgrid.DisplayLayout.Bands[0].Columns["id"].Header.Caption = "ID";
                pUgrid.DisplayLayout.Bands[0].Columns["AndOr"].Header.Caption = "是/或";
                pUgrid.DisplayLayout.Bands[0].Columns["TableName"].Header.Caption = "表名";
                pUgrid.DisplayLayout.Bands[0].Columns["ColumnName"].Header.Caption = "欄位";
                pUgrid.DisplayLayout.Bands[0].Columns["ColumnDesc"].Header.Caption = "欄位說明";
                pUgrid.DisplayLayout.Bands[0].Columns["Condition"].Header.Caption = "判斷式";
                pUgrid.DisplayLayout.Bands[0].Columns["Value"].Header.Caption = "值";
                pUgrid.DisplayLayout.Bands[0].Columns["ColumnType"].Header.Caption = "型態";
                pUgrid.DisplayLayout.Bands[0].Columns["orderSeq"].Header.Caption = "順序";

                pUgrid.DisplayLayout.Bands[0].Columns["AndOr"].Width = 80;
                pUgrid.DisplayLayout.Bands[0].Columns["ColumnName"].Width = 80;
                pUgrid.DisplayLayout.Bands[0].Columns["ColumnDesc"].Width = 120;
                pUgrid.DisplayLayout.Bands[0].Columns["Condition"].Width = 80;
                pUgrid.DisplayLayout.Bands[0].Columns["Value"].Width = 200;

                pUgrid.DisplayLayout.Bands[0].Columns["id"].Hidden = true;
                pUgrid.DisplayLayout.Bands[0].Columns["TableName"].Hidden = true;
                pUgrid.DisplayLayout.Bands[0].Columns["orderSeq"].Hidden = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private List<KeyValuePair<string, string>> WfGetConditionList()
        {

            var conditionLinst = new List<KeyValuePair<string, string>>();
            conditionLinst.Add(new KeyValuePair<string, string>("=", "="));
            conditionLinst.Add(new KeyValuePair<string, string>(">=", ">="));
            conditionLinst.Add(new KeyValuePair<string, string>("<=", "<="));
            conditionLinst.Add(new KeyValuePair<string, string>(">", ">"));
            conditionLinst.Add(new KeyValuePair<string, string>("<", "<"));
            conditionLinst.Add(new KeyValuePair<string, string>("<>", "<>"));
            conditionLinst.Add(new KeyValuePair<string, string>("LIKE", "LIKE"));

            return conditionLinst;
        }

        private List<KeyValuePair<string, string>> WfGetAndOrList()
        {

            var conditionLinst = new List<KeyValuePair<string, string>>();
            conditionLinst.Add(new KeyValuePair<string, string>("AND", "且"));
            conditionLinst.Add(new KeyValuePair<string, string>("OR", "或"));

            return conditionLinst;
        }

        #region refreshCombo combobox 無法keep資料,暫時這樣處理
        private void refreshCombo()
        {
            AdvanceQueryInfo advQueryInfo;
            if (intCurTab == 0)
                advQueryInfo = tabMaster;
            else
            {
                advQueryInfo = tabDetailList[intCurTab - 1];
            }

            if (advQueryInfo.UComboColumn.Rows.Count > 0 && advQueryInfo.UComboColumn.Value == null)
                advQueryInfo.UComboColumn.PerformAction(UltraComboAction.FirstRow);

            if (advQueryInfo.UComboCondition.Rows.Count > 0 && advQueryInfo.UComboCondition.Value == null)
                advQueryInfo.UComboCondition.PerformAction(UltraComboAction.FirstRow);

        } 
        #endregion
    }


}
