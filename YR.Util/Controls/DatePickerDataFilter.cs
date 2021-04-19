using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Infragistics.Win;

/*
 
    The Owner - The Owner is the back end value. In a grid cell, I think of the Owner value as the value that is stored in the grid's DataSource.
    The Editor - The Editor is the object that handles the value in the cell. I think of this as the value in the cell. The Editor Value has to be of a type that the cell's editor can handle. So, for example, if you are using a DateTimeEditor in a cell, the editor can only handle DateTime values. The Owner value can be any type, but the Editor Value always has to be a DateTime.
    The Display - This is what the user sees on the screen, so it's pretty much always a string. 

 */
namespace YR.Util.Controls
{
    public class DatePickerDataFilter : Infragistics.Win.IEditorDataFilter
    {
        object Infragistics.Win.IEditorDataFilter.Convert(Infragistics.Win.EditorDataFilterConvertArgs args)
        {
            object value;
            if (args.Value != null)
                value = args.Value.ToString();
            return args.Value;
            //switch (args.Direction)
            //{       
            //    //case ConversionDirection.EditorToOwner:
            //    //    args.Handled = true;
            //    //    if (args.Value == null) return "N";

            //    //    if (args.Value.ToString().ToUpper() == "CHECKED" || args.Value.ToString().ToUpper() == "TRUE" || args.Value.ToString().ToUpper() == "Y")
            //    //    {
            //    //        state = CheckState.Checked;
            //    //    }
            //    //    else
            //    //    {
            //    //        state = CheckState.Unchecked;
            //    //    }

            //    //    switch (state)
            //    //    {
            //    //        case CheckState.Checked:
            //    //            return "Y";
            //    //        case CheckState.Unchecked:
            //    //            return "N";
            //    //        case CheckState.Indeterminate:
            //    //            return "N";
            //    //    }
            //    //    break;
            //    case ConversionDirection.OwnerToEditor:
            //        args.Handled = true;
            //        return args.Value;
            //    case ConversionDirection.EditorToDisplay:
            //        break;

            //}
            //throw new Exception("Invalid value passed into CheckEditorDataFilter.Convert()");
        }
    }
}