using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YR.Util.Forms
{
    public partial class FrmMessage : Form
    {

        #region 自訂變數
        private string _msg = "";
        public string ps_Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        private string _title = "Information";
        public string ps_Title
        {
            get { return _title; }
            set { _title = value; }
        } 
        #endregion

        #region 建構子
        public FrmMessage()
        {
            InitializeComponent();
        }

        public FrmMessage(string ps_message)
        {
            InitializeComponent();
            _msg = ps_message;
        }

        public FrmMessage(string ps_message,string ps_title)
        {
            InitializeComponent();
            _msg = ps_message;
            _title = ps_title;
        } 
        #endregion

        #region button_Click
        private void button_Click(object sender, EventArgs e)
        {
            string ls_ctrl_name = this.wf_get_control_name(sender);

            switch (ls_ctrl_name)
            {
                case "btn_ok":
                    this.Close();
                    break;
            }
        } 
        #endregion


        #region wf_get_control_name(object sender) : 獲取 windows control 控制項的名稱
        /// <summary>
        /// 獲取 windows control 控制項的名稱 
        /// </summary>
        /// <param name="sender">control 控制項</param>
        /// <returns>控制項的名稱 </returns>
        protected string wf_get_control_name(object sender)
        {
            try
            {
                string ls_name = "";
                try
                {
                    System.Windows.Forms.Control lcontrol = (System.Windows.Forms.Control)sender;
                    ls_name = lcontrol.Name.Trim();
                    return ls_name;
                }
                catch { }

                if (sender.GetType() == typeof(System.Windows.Forms.ToolStripItem) ||
                    sender.GetType() == typeof(System.Windows.Forms.ToolStripButton) ||
                    sender.GetType() == typeof(System.Windows.Forms.ToolStripDropDownButton) ||
                    sender.GetType() == typeof(System.Windows.Forms.ToolStripMenuItem)
                     )
                {
                    ls_name = ((ToolStripItem)sender).Name.Trim().ToLower();
                    return ls_name;
                }
                return ls_name;
            }
            catch
            {
                return "";
            }


        }
        #endregion 

        private void FrmMessage_Load(object sender, EventArgs e)
        {
            txt_message.Text = this._msg;
            this.Text = this._title;
        }

    }
}
