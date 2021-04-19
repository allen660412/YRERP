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
    public class CheckEditorDataFilter : Infragistics.Win.IEditorDataFilter
    {
        CheckState state;
        object Infragistics.Win.IEditorDataFilter.Convert(Infragistics.Win.EditorDataFilterConvertArgs args)
        {
            switch (args.Direction)
            {       
                case ConversionDirection.EditorToOwner:
                    args.Handled = true;
                    if (args.Value == null) return "N";

                    if (args.Value.ToString().ToUpper() == "CHECKED" || args.Value.ToString().ToUpper() == "TRUE" || args.Value.ToString().ToUpper() == "Y")
                    {
                        state = CheckState.Checked;
                    }
                    else
                    {
                        state = CheckState.Unchecked;
                    }

                    switch (state)
                    {
                        case CheckState.Checked:
                            return "Y";
                        case CheckState.Unchecked:
                            return "N";
                        case CheckState.Indeterminate:
                            return "N";
                    }
                    break;
                case ConversionDirection.OwnerToEditor:
                    args.Handled = true;

                    if (args.Value == null) return CheckState.Unchecked;

                    if (args.Value.ToString() == "Y" || args.Value.ToString().ToUpper() == "TRUE")
                        return CheckState.Checked;
                    else
                        return CheckState.Unchecked;

                //使用三相時
                //if (args.Value.ToString() == "Y" || args.Value.ToString().ToUpper() == "TRUE")
                //    return CheckState.Checked;
                //else if (args.Value.ToString() == "N" || args.Value.ToString().ToUpper() == "False")
                //    return CheckState.Unchecked;
                //else
                //    return CheckState.Indeterminate;

                //else if (args.Value.ToString() == "N" || args.Value.ToString().ToUpper() == "FALSE")
                //    return CheckState.Unchecked;
                //else
                //    return CheckState.Indeterminate;
            }
            throw new Exception("Invalid value passed into CheckEditorDataFilter.Convert()");
        }
    }

    //public class CheckEditorDataFilter1 : Infragistics.Win.IEditorDataFilter
    //{
    //    object Infragistics.Win.IEditorDataFilter.Convert(Infragistics.Win.EditorDataFilterConvertArgs args)
    //    {
    //        switch (args.Direction)
    //        {
    //            case ConversionDirection.EditorToOwner:
    //                args.Handled = true;
    //                CheckState state = (CheckState)args.Value;
    //                switch (state)
    //                {
    //                    case CheckState.Checked:
    //                        return "YES";
    //                    case CheckState.Unchecked:
    //                        return "NO";
    //                    case CheckState.Indeterminate:
    //                        return String.Empty;
    //                }
    //                break;
    //            case ConversionDirection.OwnerToEditor:
    //                args.Handled = true;
    //                if (args.Value.ToString() == "YES")
    //                    return CheckState.Checked;
    //                else if (args.Value.ToString() == "NO")
    //                    return CheckState.Unchecked;
    //                else
    //                    return CheckState.Indeterminate;
    //        }
    //        throw new Exception("Invalid value passed into CheckEditorDataFilter.Convert()");
    //    }
    //}


}