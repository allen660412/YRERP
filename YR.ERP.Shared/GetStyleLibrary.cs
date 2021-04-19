using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace YR.ERP.Shared
{
    public class GetStyleLibrary
    {
        public static Color FormBackGRoundColor = Color.White;
        //public static Color FormBackGRoundColor = Color.FromKnownColor(KnownColor.Control);    
        //public static Color FormBackGRoundColor = ColorTranslator.FromHtml("#FFFDE6");    //鵝黃色

        public static readonly Infragistics.Win.EmbeddableElementDisplayStyle ControlDisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2013;
        public static readonly Infragistics.Win.UltraWinToolbars.ToolbarStyle UltraWinToolBarDiplayStyle = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2010;

        //public static Color BackGround_required = ColorTranslator.FromHtml("#E8E8D0");    //暗黃色
        //public static Color BackGround_required = ColorTranslator.FromHtml("#EDF5FD");    //淺藍色
        public static readonly Color BackGround_required = ColorTranslator.FromHtml("#D8E6D1");      //淺綠
        public static readonly Color BackGround_readonly = ColorTranslator.FromHtml("#E5E9ED"); //淺藍色
        public static readonly Color BackGround_edit = Color.White;

        public static readonly Color Font_readonly = Color.Black;
        public static readonly Color Font_edit = Color.Black;

        //public static Font FontNormal = null;
        public static readonly Font FontControlNormal = new System.Drawing.Font("新細明體", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(1)), false);
        public static readonly Font FontGrid = new System.Drawing.Font("新細明體", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(1)), false);

        //menu 樹狀式圖
        public static readonly Color TreeBackGroundColor = Color.FromArgb(67, 168, 152);
        //public static readonly Color TreeBackGroundColor = ColorTranslator.FromHtml("#FFFDE6");    //鵝黃色

        //summary 摘要
        public static readonly Color SummaryBackGroundColor = ColorTranslator.FromHtml("#FFFDE6");    //鵝黃色
    }
}
