using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YR.Util.Controls
{
    public partial class UcTransparentPanel : Panel
    {
        #region Property
        private Color _colorBorder = Color.Transparent;
        private Button _pbxResize = new Button();//用來顯示右下角可移動的圖案
        bool inResizeing = false;
        private bool _allowResize = false;
        public bool AllowResize
        {
            get { return _allowResize; }
            set
            {
                _allowResize = value;
                if (value)
                {
                    _pbxResize.Visible = true;
                    _pbxResize.Cursor = Cursors.SizeNWSE;
                }
                else
                {
                    _pbxResize.Visible = false;

                    _pbxResize.Cursor = this.Cursor;
                }

            }
        }
        #endregion

        #region 建構子
        public UcTransparentPanel()
        {
            InitializeComponent();
            IniControl();
        }
        public UcTransparentPanel(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            IniControl();
        }
        #endregion

        private void IniControl()
        {
            _pbxResize.MouseDown += pbxResize_MouseDown;
            _pbxResize.MouseMove += pbxResize_MouseMove;
            _pbxResize.MouseUp += pbxResize_MouseUp;
            _pbxResize.Size = new Size(12, 12);
            _pbxResize.BackColor = Color.Red;
            _pbxResize.Visible = _allowResize;

            _pbxResize.Location = new Point(this.Size.Width - _pbxResize.Size.Width, this.Size.Height - _pbxResize.Size.Height);
            _pbxResize.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _pbxResize.Parent = this;
        }

        private void pbxResize_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            inResizeing = true;
        }

        private void pbxResize_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            inResizeing = false;
        }

        private void pbxResize_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (inResizeing)
            {
                this.Height = _pbxResize.Top + e.Y;
                this.Width = _pbxResize.Left + e.X;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_colorBorder != Color.Transparent)
            {
                e.Graphics.DrawRectangle(
                    new Pen(
                        new SolidBrush(_colorBorder), 0),
                        e.ClipRectangle);

            }
            else
            {
                e.Graphics.DrawRectangle(
                    new Pen(
                        new SolidBrush(_colorBorder), 2),
                        e.ClipRectangle);
            }
        }

        public Color BorderColor
        {
            get
            {
                return _colorBorder;
            }
            set
            {
                _colorBorder = value;
            }
        }

        //protected override void OnPaint(PaintEventArgs e) {
        //    base.OnPaint(e);
        //    Graphics g = e.Graphics;
        //    Font font = new Font("Microsoft Sans Serif", 36F,
        //                   System.Drawing.FontStyle.Bold,
        //                   System.Drawing.GraphicsUnit.Point,
        //                   ((byte)(0)));
        //    SizeF stringSize = new SizeF();
        //    String str = "      www.whiteboardcoder.com         www.whiteboardcoder.com";
        //    int rowHeight;
        //    int textWidth;
        //    int xAdjust = 0;

        //    stringSize = e.Graphics.MeasureString(str, font, 800);
        //    rowHeight = (int)stringSize.Height +
        //                          (int)stringSize.Height;
        //    textWidth = ((int)stringSize.Width);

        //    g.RotateTransform(-20);
        //    for (int x = 0; x <
        //            (this.Width / textWidth) + 2; x++) {
        //        for (int y = 0; y < 2 *
        //                 (this.Height / rowHeight) + 2; y++) {
        //            xAdjust = textWidth / 2;
        //            g.DrawString(str, font,
        //                 System.Drawing.Brushes.Red,
        //               new Point(x * textWidth - xAdjust,
        //                     y * rowHeight));
        //        }
        //    }
        //}
    }
}
