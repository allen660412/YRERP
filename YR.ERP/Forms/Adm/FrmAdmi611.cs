/* 程式名稱: 流程設定作業
   系統代號: admi611
   作　　者: Allen
   描　　述: 繼承FrmEntryBase 但其實是 Master Detail 表單
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
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.Util.Controls;

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi611 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        AdmBLL BoAdm = null;
        Point MousePoint = new Point(0, 0);//記錄原始滑鼠點下的位值
        Point OldPoint = new Point(0, 0);//記錄原始panel位值
        bool isMove = false;
        bool isInDoubleClick = false;


        List<vw_admi611s> Admi611sList = null;  //初始化及清除 在UGrid_Master_AfterRowActivate、WfSaveCancel、
        #endregion

        #region 建構子
        public FrmAdmi611()
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
            this.StrFormID = "admi611";
            this.IntTabCount = 2;
            this.IntMasterGridPos = 2;
            uTab_Master.Tabs[0].Text = "資料內容";
            uTab_Master.Tabs[1].Text = "資料瀏覽";

            uTab_Header.Tabs[0].Text = "基本資料";
            uTab_Header.Tabs[1].Text = "狀態";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("adx01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "adxsecu";
                TabMaster.GroupColumn = "adxsecg";

                TabMaster.CanCopyMode = false;
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

            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
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
                    BoAdm.TRAN = BoMaster.TRAN;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                return true;
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
            try
            {
                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);

                    foreach (Control control in pnl_adx03.Controls)
                    {
                        if (control.GetType() == typeof(UcTransparentPanel))
                        {
                            (control as UcTransparentPanel).AllowResize = false;
                        }
                    }
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false); //先全開
                    WfSetControlReadonly(uGridMaster, true);//grid不可編輯

                    WfSetControlReadonly(new List<Control> { ute_adxcreu, ute_adxcreg, udt_adxcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_adxmodu, ute_adxmodg, udt_adxmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_adxsecu, ute_adxsecg }, true);

                    foreach (Control control in pnl_adx03.Controls)
                    {
                        if (control.GetType() == typeof(UcTransparentPanel))
                        {
                            (control as UcTransparentPanel).AllowResize = true;
                        }
                    }
                }
                //WfSetUcPanelToFront();
                pnl_adx03.Refresh();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterfDisplayMode  新增修改刪除查詢後的 focus調整
        protected override void WfAfterfDisplayMode()
        {
            try
            {
                uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                SelectNextControl(this.uTab_Header, true, true, true, false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterFormCheck() 存檔後處理,通常為放入Pk
        protected override bool WfAfterFormCheck()
        {
            string sea01New, errMsg;
            try
            {

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["adxsecu"] = LoginInfo.UserNo;
                        DrMaster["adxsecg"] = LoginInfo.GroupNo;
                        DrMaster["adxcreu"] = LoginInfo.UserNo;
                        DrMaster["adxcreg"] = LoginInfo.DeptNo;
                        DrMaster["adxcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["adxmodu"] = LoginInfo.UserNo;
                        DrMaster["adxmodg"] = LoginInfo.DeptNo;
                        DrMaster["adxmodd"] = Now;
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

        #region WfAppendUpdate 存檔後處理額外的資料表 新增、修改、刪除...
        protected override bool WfAppendUpdate()
        {
            vw_admi611 admi611Model = null;
            int chkCnts = 0;
            StringBuilder sbSql = null;
            List<SqlParameter> sqlParmsList;
            CommonBLL boAppend;
            DataTable dtady = null;
            try
            {
                boAppend = new InvBLL(BoMaster.OfGetConntion());
                boAppend.TRAN = BoMaster.TRAN;
                boAppend.OfCreateDao("ady_tb", "*", "");
                admi611Model = DrMaster.ToItem<vw_admi611>();

                //處理明細--均先刪後新增
                if (FormEditMode != YREditType.新增)
                {
                    sbSql = new StringBuilder();
                    sqlParmsList = new List<SqlParameter>();
                    sbSql.AppendLine("DELETE FROM ady_tb WHERE ady01=@ady01");
                    sqlParmsList.Add(new SqlParameter("@ady01", admi611Model.adx01));
                    chkCnts = boAppend.OfExecuteNonquery(sbSql.ToString(), sqlParmsList.ToArray());


                    sbSql = new StringBuilder();
                    sqlParmsList = new List<SqlParameter>();
                    sbSql.AppendLine("SELECT * FROM ady_tb");
                    sbSql.AppendLine("WHERE 1<>1");
                    dtady = boAppend.OfGetDataTable(sbSql.ToString());
                    foreach (vw_admi611s detailModel in Admi611sList)
                    {
                        var drady = dtady.NewRow();
                        drady["ady01"] = detailModel.ady01;
                        drady["ady02"] = detailModel.ady02;
                        drady["ady03"] = detailModel.ady03;
                        drady["ady04"] = detailModel.ady04;
                        drady["ady05"] = detailModel.ady05;
                        drady["ady06"] = detailModel.ady06;
                        drady["ady07"] = detailModel.ady07;
                        drady["adycreu"] = detailModel.adycreu;
                        drady["adycreg"] = detailModel.adycreg;
                        if (detailModel.adycred == null)
                            drady["adycred"] = DBNull.Value ;
                        else
                            drady["adycred"] = detailModel.adycred;

                        drady["adymodu"] = detailModel.adymodu;
                        drady["adymodg"] = detailModel.adymodg;

                        if (detailModel.adymodd == null)
                            drady["adymodd"] = DBNull.Value;
                        else
                            drady["adymodd"] = detailModel.adymodd;
                        dtady.Rows.Add(drady);
                    }
                    boAppend.OfUpdate(dtady);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfDeleteAppenUpdate 刪除時使用,若需單身資料,要先在查此查詢資料庫並且異動
        protected override bool WfDeleteAppenUpdate(DataRow pDr)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmsList;
            vw_admi611 admi611Model;
            try
            {
                admi611Model=DrMaster.ToItem<vw_admi611>();
                
                //刪除明細資料
                sbSql = new StringBuilder();
                sbSql.AppendLine("DELETE FROM ady_tb WHERE ady01=@ady01");
                sqlParmsList=new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@ady01",admi611Model.adx01));
                BoAdm.OfExecuteNonquery(sbSql.ToString(),sqlParmsList.ToArray());

                //清空有掛載在menu的料 ado_tb.ado14
                sbSql = new StringBuilder();
                sbSql.AppendLine("UPDATE ado_tb SET ado14=NULL ");
                sbSql.AppendLine("WHERE ado14=@ado14");
                sqlParmsList.Add(new SqlParameter("@ado14", admi611Model.adx01));
                BoAdm.OfExecuteNonquery(sbSql.ToString(), sqlParmsList.ToArray());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        //*****************************覆寫 功能****************************************
        #region WfToolbarModify() : 主表修改 function   --覆寫
        /// <summary>
        /// 主表修改 function
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfToolbarModify()
        {
            try
            {
                if (base.WfToolbarModify() == false)
                    return false;
                //Admi611sList = new List<vw_admi611s>();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
        #endregion 主表修改 function

        #region WfSaveCancel() 新增或修改後按下取消按鈕---額外覆寫
        protected override void WfSaveCancel()
        {
            try
            {
                base.WfSaveCancel();

                Admi611sList.Clear();

                if (DrMaster == null || DrMaster.RowState == DataRowState.Detached)
                    return;

                WfRetriveDetail(GlobalFn.isNullRet(DrMaster["adx01"], ""));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region uGrid_Master_AfterRowActivate override 事件 --覆寫
        protected override void UGrid_Master_AfterRowActivate(object sender, EventArgs e)
        {
            vw_admi611 vw_admi611Model;
            try
            {
                base.UGrid_Master_AfterRowActivate(sender, e);

                if (FormEditMode == YREditType.新增)
                    Admi611sList = new List<vw_admi611s>();
                else if (FormEditMode == YREditType.NA)
                {
                    vw_admi611Model = DrMaster.ToItem<vw_admi611>();
                    WfRetriveDetail(vw_admi611Model.adx01);
                }
                else if (FormEditMode == YREditType.修改)
                {
                    vw_admi611Model = DrMaster.ToItem<vw_admi611>();
                    WfRetriveDetail(vw_admi611Model.adx01);
                }

                if (Admi611sList == null)
                    Admi611sList = new List<vw_admi611s>();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //isInLoadRowDefault = false;
            }
        }
        #endregion

        //*****************************表單自訂Fuction****************************************

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                WfLoadImage(ultraTextEditor1.Text);
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            vw_admi611 masterModel = null;
            vw_admi611s detailModel = null;
            int maxNo = 0;
            DataRow drAdmi611s;
            try
            {
                if (Admi611sList.Count == 0)
                    maxNo = 1;
                else
                {
                    maxNo = Admi611sList.Max(x => x.ady02);
                    if (GlobalFn.isNullRet(maxNo, 0) == 0)
                        maxNo = 1;
                    else
                        maxNo += 1;

                }

                masterModel = DrMaster.ToItem<vw_admi611>();
                detailModel = new vw_admi611s();
                detailModel.ady01 = masterModel.adx01;
                detailModel.ady02 = maxNo;
                detailModel.ady03 = "";
                detailModel.ady04 = 0;
                detailModel.ady05 = 0;
                detailModel.ady06 = 100;
                detailModel.ady07 = 20;


                var result = WfOpenAdmi611s(YREditType.新增, detailModel,out drAdmi611s);
                if (result == DialogResult.Yes)
                {
                    WfIniUcPanel(drAdmi611s.ToItem<vw_admi611s>());
                    Admi611sList.Add(drAdmi611s.ToItem<vw_admi611s>());
                    //WfIniUcPanel(detailModel);
                    //Admi611sList.Add(detailModel);

                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }

        }

        private void btnShowPnl_Click(object sender, EventArgs e)
        {
            List<UcTransparentPanel> utpList = new List<UcTransparentPanel>();
            StringBuilder sbMsg = new StringBuilder();
            foreach (Control control in pnl_adx03.Controls)
            {
                if (!(control.GetType() == typeof(UcTransparentPanel)))
                    continue;

                utpList.Add(control as UcTransparentPanel);
                sbMsg.AppendLine(string.Format("panel名稱:{0} X軸:{1} Y軸:{2}", control.Name, control.Location.X, control.Location.Y));
            }
            WfShowErrorMsg(sbMsg.ToString());
        }

        #region panel 相關事件方法
        void WfIniUcPanel(vw_admi611s pDetailModel)
        {
            UcTransparentPanel panel = new UcTransparentPanel();
            panel.Name = pDetailModel.ady02.ToString();        //以序號做識別
            panel.BorderStyle = BorderStyle.Fixed3D;
            panel.BorderColor = Color.White;
            panel.Location = new Point(pDetailModel.ady04, pDetailModel.ady05);
            panel.Size = new Size(pDetailModel.ady06, pDetailModel.ady07);

            panel.MouseDown += panel_MouseDown;
            panel.MouseMove += panel_MouseMove;
            panel.MouseUp += panel_MouseUp;
            panel.MouseDoubleClick += panel_MouseDoubleClick;
            panel.Resize += panel_Resize;
            panel.MouseHover += panel_MouseHover;
            panel.MouseLeave += panel_MouseLeave;
            panel.AllowResize = true;

            Label label = new Label();
            label.Text = panel.Name;
            label.BackColor = Color.White;
            label.AutoSize = true;

            panel.Controls.Add(label);
            pnl_adx03.Controls.Add(panel);
            panel.BringToFront();

            panel.Refresh();
        }

        void panel_MouseHover(object sender, EventArgs e)
        {
            string msg;
            UcTransparentPanel panel;
            if (Admi611sList == null)
                return;

            panel=(sender as UcTransparentPanel);
            var admi611Model = Admi611sList.Where(p => p.ady02.ToString() == panel.Name)
                                         .FirstOrDefault();
            msg = string.Format("程式代號:{0} X:{1} Y:{2} WIDTH:{3} HEIGHT:{4}",
                                admi611Model.ady03,
                                admi611Model.ady04.ToString(),
                                admi611Model.ady05.ToString(),
                                admi611Model.ady06.ToString(),
                                admi611Model.ady07.ToString());
            WfShowBottomHelpMsg(msg);
        }

        void panel_MouseLeave(object sender, EventArgs e)
        {
            WfShowBottomHelpMsg("");
        }

        //載入panel 屬性
        void WfResetPanelPosition(UcTransparentPanel pPanel, vw_admi611s pDetailModel)
        {
            try
            {
                pPanel.Location = new Point(pDetailModel.ady04, pDetailModel.ady05);
                pPanel.Size = new Size(pDetailModel.ady06, pDetailModel.ady07);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void panel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataRow drAdmi611s = null;
            try
            {
                if (FormEditMode != YREditType.修改 && FormEditMode != YREditType.新增)
                    return;

                if (e.Button != MouseButtons.Left)
                    return;
                //isInDoubleClick = true;

                var panel = (sender as UcTransparentPanel);
                var panelName = panel.Name;
                var vw_admi611Model = Admi611sList.Where(p => p.ady02 == Int16.Parse(panelName))
                                                .FirstOrDefault();

                var result = WfOpenAdmi611s(YREditType.修改, vw_admi611Model, out drAdmi611s);
                if (result == DialogResult.Yes)
                {
                    //會引發 mouseUp事件,因此註解掉
                    WfResetPanelPosition(panel, drAdmi611s.ToItem<vw_admi611s>());
                    var returnAdmi611s = drAdmi611s.ToItem<vw_admi611s>();
                    var updAdmi611model = Admi611sList.Where(p => p.ady02 == Int16.Parse(panelName))
                                                .FirstOrDefault();
                    updAdmi611model.ady01 = returnAdmi611s.ady01;
                    updAdmi611model.ady02 = returnAdmi611s.ady02;
                    updAdmi611model.ady03 = returnAdmi611s.ady03;
                    updAdmi611model.ady04 = returnAdmi611s.ady04;
                    updAdmi611model.ady05 = returnAdmi611s.ady05;
                    updAdmi611model.ady06 = returnAdmi611s.ady06;
                    updAdmi611model.ady07 = returnAdmi611s.ady07;
                    updAdmi611model.adycreu = returnAdmi611s.adycreu;
                    updAdmi611model.adycreg = returnAdmi611s.adycreg;
                    updAdmi611model.adycred = returnAdmi611s.adycred;
                    updAdmi611model.adymodu = returnAdmi611s.adymodu;
                    updAdmi611model.adymodg = returnAdmi611s.adymodg;
                    updAdmi611model.adymodd = returnAdmi611s.adymodd;

                    WfResetPanelPosition(panel, vw_admi611Model);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                //isInDoubleClick = false;      //移至mouseup 
            }
        }

        void panel_MouseDown(object sender, MouseEventArgs e)
        {

            if (FormEditMode != YREditType.修改 && FormEditMode != YREditType.新增)
                return;

            if (e.Button != MouseButtons.Left)
                return;
            //把本panel置為最前
            ((Panel)sender).BringToFront();
            MousePoint = e.Location;
            OldPoint = ((Panel)sender).Location;
            isMove = true;

            WfShowBottomHelpMsg(string.Format("({0},{1})", OldPoint.X.ToString(), OldPoint.Y.ToString()));
        }

        void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (FormEditMode != YREditType.修改 && FormEditMode != YREditType.新增)
                return;

            if (e.Button != MouseButtons.Left)
                return;
            if (isInDoubleClick)
                return;

            if (isMove)
            {
                ((Panel)sender).Location = new Point(OldPoint.X + e.X - MousePoint.X, OldPoint.Y + e.Y - MousePoint.Y);
                OldPoint = ((Panel)sender).Location;
            }
        }

        void panel_MouseUp(object sender, MouseEventArgs e)
        {
            UcTransparentPanel panel = null;
            Point panelLocation;
            System.Drawing.Size panelSize;
            try
            {
                panel = ((UcTransparentPanel)sender);
                if (FormEditMode != YREditType.修改 && FormEditMode != YREditType.新增)
                    return;

                if (e.Button != MouseButtons.Left)
                    return;

                if (isMove)
                {
                    var admi611sModel = Admi611sList.Where(p => p.ady02 == int.Parse(panel.Name))
                                                  .FirstOrDefault();
                    panelSize = panel.Size;// = new Size(pDetailModel.ady06, pDetailModel.ady07);
                    panelLocation = panel.Location;// = new Point(pDetailModel.ady06, pDetailModel.ady07);
                    if (WfChkPanelInside(panel) == false)
                    {
                        var result = WfShowConfirmMsg("已超過邊界,是否要刪除?");
                        //if (WfShowConfirmMsg("已超過邊界,是否要刪除?") == 1)
                        if (result==DialogResult.Yes)
                        {
                            panel.Dispose();
                            Admi611sList.Remove(admi611sModel);
                        }
                        else
                        {
                            panel.Location = new Point(admi611sModel.ady04, admi611sModel.ady05);
                            panel.Refresh();
                        }
                    }
                    else
                    {
                        admi611sModel.ady04 = panelLocation.X;
                        admi611sModel.ady05 = panelLocation.Y;
                        admi611sModel.ady06 = panelSize.Width;
                        admi611sModel.ady07 = panelSize.Height;
                    }

                    panel.Refresh();
                    isMove = false;
                }

            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
            finally
            {
                isInDoubleClick = false;      //避免引發mousedoubleclick事件後 又再次處理 mouse up 
            }

        }

        void panel_Resize(object sender, EventArgs e)
        {
            string panelName;
            UcTransparentPanel panel = (sender as UcTransparentPanel);

            panelName = panel.Name;
            if (Admi611sList == null || Admi611sList.Count == 0)
                return;

            var admi611s = Admi611sList.Where(p => p.ady02.ToString() == panelName)
                                    .FirstOrDefault();
            if (admi611s == null)
                return;

            if (WfChkPanelInside(panel) == true)
            {
                admi611s.ady06 = panel.Size.Width;
                admi611s.ady07 = panel.Size.Height;
            }
            else
            {
                var result = WfShowConfirmMsg("已超過邊界,是否要刪除?");

                //if (WfShowConfirmMsg("已超過邊界,是否要刪除?")==1)
                if (result == DialogResult.Yes)
                {
                    panel.Dispose();
                    Admi611sList.Remove(admi611s);

                }
                else
                {
                    panel.Size = new Size(admi611s.ady06, admi611s.ady07);
                    panel.Refresh();
                }
            }


        }
        #endregion

        #region WfLoadImage 產生圖檔,並寫入至datarow
        private void WfLoadImage(string pImagePath)
        {
            try
            {
                if (pImagePath != "")
                {
                    if (System.IO.File.Exists(pImagePath))
                    {
                        Image img = Image.FromFile(pImagePath);
                        DrMaster["adx03"] = GlobalPictuer.GetBytesFromImage(img);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfOpenAdmi611s 閞啟元件位置視窗
        private DialogResult WfOpenAdmi611s(YREditType pEditType, vw_admi611s pAdmi611s,out DataRow pdrAdmi611s)
        {
            try
            {
                pdrAdmi611s=null;
                FrmAdmi611s frm = new FrmAdmi611s(pEditType, pAdmi611s);
                frm.LoginInfo = this.LoginInfo;
                frm.ShowDialog(this);
                DialogResult = frm.DialogResult;      
                if (DialogResult==DialogResult.Yes)
                    pdrAdmi611s = frm.TabMaster.DtSource.Rows[0];
                return DialogResult;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfRetriveDetail 取得明細資料
        private void WfRetriveDetail(string padx01)
        {
            DataTable dtady = null;
            List<SqlParameter> listSqlparms;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM vw_admi611s");
                sbSql.AppendLine("WHERE ady01=@ady01");
                listSqlparms = new List<SqlParameter>() { new SqlParameter("@ady01", padx01),
                                                          };
                dtady = BoAdm.OfGetDataTable(sbSql.ToString(), listSqlparms.ToArray());
                Admi611sList = dtady.ToList<vw_admi611s>();

                //取得panel 中含有 UcTransparentPanel 型別物件,後再全部清除
                var ucPanelList = pnl_adx03.Controls
                                         .Cast<Control>()
                                         .Where(p => p.GetType() == typeof(UcTransparentPanel))
                                         ;

                if (ucPanelList != null && ucPanelList.Count() > 0)
                {
                    //全部清除--要倒著刪
                    for (int i = ucPanelList.Count() - 1; i >= 0; i--)
                    {
                        pnl_adx03.Controls[i].Dispose();
                    }
                }

                //重取位置及大小
                foreach (vw_admi611s detailModel in Admi611sList)
                {
                    WfIniUcPanel(detailModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkPanelOutside 檢查 panel 是否超過邊界
        private bool WfChkPanelInside(UcTransparentPanel panel)
        {
            int panelLeft, panelRight, panelUp, panelDown;
            int pictureLeft, pictureRight, pictureUp, pictureDown;
            try
            {
                panelLeft = panel.Location.X;
                panelRight = panel.Location.X + panel.Size.Width;
                panelUp = panel.Location.Y;
                panelDown = panel.Location.Y + panel.Size.Height;

                pictureLeft = 0;
                pictureRight = pbx_adx03.Image.Size.Width;
                pictureUp = 0;
                pictureDown = pbx_adx03.Image.Size.Height;

                if (panelLeft < pictureLeft ||
                    panelRight > pictureRight ||
                    panelUp < pictureUp ||
                    panelDown > pictureDown
                    )
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //private void WfSetUcPanelToFront()
        //{
        //    foreach (Control control in pnl_adx03.Controls)
        //    {
        //        if (control.GetType() == typeof(UcTransparentPanel))
        //        {
        //            (control as UcTransparentPanel).BringToFront();
        //        }

        //    }
        //}

    }
}
