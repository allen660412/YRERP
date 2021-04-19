using Infragistics.Win;
using Infragistics.Win.UltraWinSchedule;
using Infragistics.Win.UltraWinSchedule.CalendarCombo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YR.Util.Controls
{
    public partial class UcDatePicker : Infragistics.Win.UltraWinSchedule.UltraCalendarCombo
    {

        private bool checkedValue = true;

        //[DefaultValue(true), Description("設定true時,檢查輸入值是否符合日期格式")]
        //public bool CheckedValue
        //{
        //    get { return this.checkedValue; }
        //    set { this.checkedValue = value; }
        //}

        public UcDatePicker()
        {
            InitializeComponent();

            //this.Value = DBNull.Value;
            //this.NullDateLabel = "";
            //this.DateButtons.Clear();//避免加了兩次
        }

        public UcDatePicker(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            //this.DateButtons.Clear();//避免加了兩次
        }

        //protected override void OnLeave(EventArgs e)
        //{
        //    if (checkedValue)
        //        base.OnLeave(e);
        //}

        //protected override void OnEditorKeyDown(KeyEventArgs args)
        //{

        //    if (checkedValue)//執行原程式
        //        base.OnEditorKeyDown(args);
        //    else
        //    {
        //        bool isTabKey = args.KeyData == Keys.Tab ||
        //                        args.KeyData == (Keys.Tab | Keys.Shift);

        //        if (args.KeyData == Keys.Tab)
        //        {
        //            //this.Value = this.ConvertDisplayValueToOwnerValue(this.Text);



        //            //this.Editor.GetDataFilteredDestinationValue(
        //            //    editorValue,
        //            //    ConversionDirection.EditorToOwner,
        //            //    out isValidConversion,
        //            //    this.embeddableOwner,
        //            //    this);

        //            //this.PerformAction(CalendarComboAction.NextControl);
        //            this.SetFocusToNextControl(true);
        //        }
        //        else if (args.KeyData == (Keys.Tab | Keys.Shift))
        //        {
        //            //this.PerformAction(CalendarComboAction.NextControl);
        //            this.SetFocusToNextControl(false);

        //        }

        //    }
        //}

    }
}
