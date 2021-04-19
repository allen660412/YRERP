/* 程式名稱: 製令結構展開
   系統代號: mant210_1
   作　　者: Allen
   描　　述: 此為展開結構變形
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
using System.Threading.Tasks;
using System.Windows.Forms;
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;
using Infragistics.Win.UltraWinTree;
using YR.ERP.Shared;
using Infragistics.Win;

namespace YR.ERP.Forms.Man
{
    public partial class FrmMant210_1 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        ManBLL BoMan = null;

        //YREditType _srcFormState = new YREditType();//表單傳入要執行的狀態
        DataSet _DsMaster = null;
        #endregion

        #region 結構子
        public FrmMant210_1()
        {
            InitializeComponent();
        }

        public FrmMant210_1(YR.ERP.Shared.UserInfo pUserInfo, DataSet pDsMaster, UltraTreeNode pUtn)
        {
            InitializeComponent();
            //this._srcFormState = pYREditType;
            this._DsMaster = pDsMaster;
            this.LoginInfo = pUserInfo;

        }
        #endregion

        #region FrmMant210_1_Load
        private void FrmMant210_1_Load(object sender, EventArgs e)
        {
            WfIniUltraTree();
        }
        #endregion
        
        #region WfIniUltraTree 樹狀資料初始化
        private void WfIniUltraTree()
        {
            try
            {
                ultraTree1.InitializeDataNode += new Infragistics.Win.UltraWinTree.InitializeDataNodeEventHandler(this.UltraTree1InitializeDataNode);
                ultraTree1.ColumnSetGenerated += ultraTree1_ColumnSetGenerated;
                ultraTree1.BeforeActivate += ultraTree1_BeforeActivate;
                ultraTree1.AfterActivate += ultraTree1_AfterActivate;

                ultraTree1.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                ultraTree1.ShowLines = true;
                ultraTree1.Font = GetStyleLibrary.FontControlNormal;
                //ultraTree1.SetDataBinding(_DsMaster, "mant210_1");
                ultraTree1.SetDataBinding(BindingMaster.DataSource, "");

                //this.WfSetMasterDatasource(_DsMaster.Tables[0]);
                //ultraTree1.DataSource = BindingMaster.DataSource;
                //ultraTree1.SetDataBinding(_DsMaster.Tables[0], "mant210_1");

                //this.ultraTree1.Nodes.Add(pUtn);
                this.ultraTree1.ExpandAll();
                ultraTree1.ViewStyle = Infragistics.Win.UltraWinTree.ViewStyle.OutlookExpress;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void ultraTree1_BeforeActivate(object sender, CancelableNodeEventArgs e)
        {

            //try
            //{
            //    var activeNode = e.TreeNode;
            //    if (!GlobalFn.varIsNull(e.TreeNode.Cells["keySelf"].Value))
            //    {
            //        int position = BindingMaster.Find("keySelf", e.TreeNode.Cells["keySelf"].Value.ToString());
            //        if (position >= 0)
            //        {
            //            BindingMaster.Position = position;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            this.StrFormID = "mant210_1";
            IntTabCount = 1;
            IntMasterGridPos = 0;
            uTab_Master.Tabs[0].Text = "資料內容";

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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("mea01", SqlDbType.NVarChar) });
                //TabMaster.UserColumn = "seasecu";
                //TabMaster.GroupColumn = "seasecg";

                TabMaster.CanAddMode = false;
                TabMaster.CanCopyMode = false;
                TabMaster.CanDeleteMode = false;
                TabMaster.CanUpdateMode = false;
                TabMaster.CanQueryMode = false;
                TabMaster.CanNavigator = false;
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

            BoMan = new ManBLL(BoMaster.OfGetConntion());
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
                    BoMan.TRAN = BoMaster.TRAN;
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
            this.TabDetailList[0].TargetTable = "meb_tb";
            this.TabDetailList[0].ViewTable = "vw_mant210s";
            keyParm = new SqlParameter("meb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "mea01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region 樹狀清單相關事件
        #region ultraTree1_AfterActivate
        void ultraTree1_AfterActivate(object sender, NodeEventArgs e)
        {
            try
            {
                var activeNode = e.TreeNode;
                if (!GlobalFn.varIsNull(e.TreeNode.Cells["keySelf"].Value))
                {
                    int position = BindingMaster.Find("keySelf", e.TreeNode.Cells["keySelf"].Value.ToString());
                    if (position >= 0)
                    {
                        BindingMaster.Position = position;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ultraTree1_InitializeDataNode
        private void UltraTree1InitializeDataNode(object sender, InitializeDataNodeEventArgs e)
        {
            // This event fires every time a node is added to the tree from 
            // a data source. 

            // Make sure that only the highest-level employees show up at
            // the root level of the tree.
            if (e.Node.Parent == null &&
                GlobalFn.isNullRet(e.Node.Cells["keyParents"].Value, "") != "")
            {
                e.Node.Visible = false;
                return;
            }

            // Put a space before each parent node and color the parent
            // nodes so the the tree is a little easier to read. 
            if (e.Node.Nodes.Count > 0)
            {
                // No point in putting a space before the root node.
                if (e.Node.Parent != null)
                    e.Node.Override.NodeSpacingBefore = 0;

                e.Node.Override.NodeAppearance.BackColor = Color.FromArgb(238, 238, 238);
                e.Node.Override.NodeAppearance.BackColor2 = Color.White;
                e.Node.Override.NodeAppearance.BackGradientStyle = GradientStyle.Vertical;
                e.Node.Override.NodeAppearance.BorderColor = Color.FromArgb(204, 204, 204);
            }

            // If this is the root node (CEO) then expand it. 
            //if (e.Node.Parent == null)
            //    e.Node.Expanded = true;
        }
        #endregion ultraTree1_InitializeDataNode

        #region ultraTree1_ColumnSetGenerated
        void ultraTree1_ColumnSetGenerated(object sender, ColumnSetGeneratedEventArgs e)
        {
            try
            {
                string columnName;
                aza_tb azaModel;

                foreach (Infragistics.Win.UltraWinTree.UltraTreeNodeColumn utnc in e.ColumnSet.Columns)
                {

                    columnName = utnc.Key;
                    azaModel = (from o in TabMaster.AzaTbList
                                where o.aza03 == columnName
                                select o
                        ).FirstOrDefault<aza_tb>();
                    if (azaModel == null)
                    {
                        //utnc.Visible = false;     //註解不可拿掉
                        continue;
                    }
                    else
                    {
                        utnc.Text = azaModel.aza04;
                        if (azaModel.aza07 < 30)
                            utnc.LayoutInfo.PreferredLabelSize = new Size(30, 0);
                        else
                            utnc.LayoutInfo.PreferredLabelSize = new Size(Convert.ToInt16(azaModel.aza07), 0);

                        if (azaModel.aza05 != "Y")
                            utnc.Visible = false;

                        //處理欄位類型
                        switch (azaModel.aza08.ToLower())
                        {
                            case "nvarchar":
                                utnc.CellAppearance.TextHAlign = HAlign.Left;
                                break;
                            case "numeric":
                                utnc.CellAppearance.TextHAlign = HAlign.Right;
                                utnc.Format = "#,0.########";
                                break;
                            case "date":
                                utnc.CellAppearance.TextHAlign = HAlign.Center;
                                utnc.Format = this.DateFormat;
                                //utnc.MaskInput = pDefaultDateFormat.ToLower();
                                break;
                            case "datetime":
                                utnc.CellAppearance.TextHAlign = HAlign.Center;
                                if (azaModel.aza14 == "Y")   //是否顯示時間
                                {
                                    utnc.Format = this.DateFormat + " hh:mm";
                                    //ugc.MaskInput = pDefaultDateFormat.ToLower() + " hh:mm";

                                    //ugc.Format = "yyyy/MM/dd";  //時間預設顯示格式至日期
                                    //ugc.MaskInput = "yyyy/mm/dd";
                                }
                                else
                                {
                                    utnc.Format = this.DateFormat; ;
                                    //ugc.MaskInput = pDefaultDateFormat.ToLower();
                                }
                                break;
                            case "datetime2":
                                utnc.CellAppearance.TextHAlign = HAlign.Center;
                                if (azaModel.aza14 == "Y")   //是否顯示時間
                                {
                                    utnc.Format = this.DateFormat + " hh:mm";
                                    //utnc.MaskInput = pDefaultDateFormat.ToLower() + " hh:mm";

                                    //ugc.Format = "yyyy/MM/dd";  //時間預設顯示格式至日期
                                    //ugc.MaskInput = "yyyy/mm/dd";
                                }
                                else
                                {
                                    utnc.Format = this.DateFormat; ;
                                    //ugc.MaskInput = pDefaultDateFormat.ToLower();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                #region old 不使用
                // Normally, we would check the Key of the ColmumnSet here.
                // In this sample, there is only one table in the DataSet, 
                // and only one relation, and they both have the same name. 
                // So there will only be one ColumnSet - "Employees".

                // The advantage to having only one ColumnSet is that properties
                // applied to the ColumnSet will affect the entire tree. For
                // example, when the user clicks on a Column Header to sort
                // a column, it will affect that column in all levels of the
                // tree. This differs from the behavior of Outlook Express, 
                // where sorting the root nodes does not affect the order
                // of the child nodes. To acheive that effect, simply give the 
                // Relationship in the DataSet a different name from the table.
                // The tree will then create two different, independent ColumnSets.

                // Initialize properties of the ColumnSet.

                // Hide the EmployeeID and SupervisorID columns, since
                // they don't make much sense to a user. 



                //e.ColumnSet.Columns["mea01"].Visible = false;
                //e.ColumnSet.Columns["keySelf"].Visible = false;
                //e.ColumnSet.Columns["keyParents"].Visible = false;
                //e.ColumnSet.Columns["mea22"].Visible = false;
                //e.ColumnSet.Columns["mea23"].Visible = false;
                //e.ColumnSet.Columns["mea26"].Visible = false;
                //e.ColumnSet.Columns["mea27"].Visible = false;

                //e.ColumnSet.Columns["level"].LayoutInfo.PreferredLabelSize = new Size(60, 0);
                //e.ColumnSet.Columns["mea20"].LayoutInfo.PreferredLabelSize = new Size(180, 0);
                //e.ColumnSet.Columns["mea21"].LayoutInfo.PreferredLabelSize = new Size(250, 0);


                //e.ColumnSet.Columns["mea20"].Text = "abc";


                // Format the Salary column as currency
                //e.ColumnSet.Columns["Salary"].Format = "c";

                // Format the StartingDate column as a date with no time.
                //e.ColumnSet.Columns["Start Date"].Format = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

                // Center the text in the Status Column
                //e.ColumnSet.Columns["Status"].CellAppearance.TextHAlign = HAlign.Center;

                // Set the NodeTextColumn to the Last Name column. 
                // This has 2 effects: 
                // First, it tells the tree which column to use for Keyboard searching. 
                // Second, it tells the tree which column to show on each 
                // node when the ViewStyle is set to Standard. 
                //e.ColumnSet.NodeTextColumn = e.ColumnSet.Columns["Last Name"]; 
                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        #endregion

        /********************************ovveride*************************/

        #region WfQueryByFirstLoad 表單第一次進入時載入空資料,或被引用時可傳查詢條件或變形資料來源
        protected override bool WfQueryByFirstLoad()
        {
            try
            {
                ////若是由作為他窗引用時
                //if (GlobalFn.isNullRet(this.StrQueryWhereAppend, "")
                //            .Trim().Length == 0)
                //    this.TabMaster.DtSource = this.TabMaster.BoBasic.OfSelect(" WHERE 1=2 ");
                //else
                //    this.TabMaster.DtSource = this.TabMaster.BoBasic.OfSelect(this.StrQueryWhereAppend);


                this.TabMaster.DtSource = _DsMaster.Tables[0];
                this.WfSetMasterDatasource(this.TabMaster.DtSource);

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion        
    }
}
