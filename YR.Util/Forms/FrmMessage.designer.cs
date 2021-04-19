namespace YR.Util.Forms
{
    partial class FrmMessage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnl_msg = new System.Windows.Forms.Panel();
            this.txt_message = new System.Windows.Forms.TextBox();
            this.btn_ok = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnl_msg.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl_msg
            // 
            this.pnl_msg.Controls.Add(this.txt_message);
            this.pnl_msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_msg.Location = new System.Drawing.Point(0, 0);
            this.pnl_msg.Name = "pnl_msg";
            this.pnl_msg.Padding = new System.Windows.Forms.Padding(5);
            this.pnl_msg.Size = new System.Drawing.Size(401, 86);
            this.pnl_msg.TabIndex = 0;
            // 
            // txt_message
            // 
            this.txt_message.BackColor = System.Drawing.Color.Linen;
            this.txt_message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_message.Font = new System.Drawing.Font("新細明體", 10F);
            this.txt_message.Location = new System.Drawing.Point(5, 5);
            this.txt_message.Multiline = true;
            this.txt_message.Name = "txt_message";
            this.txt_message.ReadOnly = true;
            this.txt_message.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_message.Size = new System.Drawing.Size(391, 76);
            this.txt_message.TabIndex = 0;
            // 
            // btn_ok
            // 
            this.btn_ok.Location = new System.Drawing.Point(157, 6);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(75, 31);
            this.btn_ok.TabIndex = 1;
            this.btn_ok.Text = "確定";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.button_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 131);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnl_msg);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(401, 131);
            this.splitContainer1.SplitterDistance = 86;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Linen;
            this.panel1.Controls.Add(this.btn_ok);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(401, 43);
            this.panel1.TabIndex = 4;
            // 
            // FrmMessage
            // 
            this.AcceptButton = this.btn_ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Linen;
            this.ClientSize = new System.Drawing.Size(404, 131);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.splitter1);
            this.Name = "FrmMessage";
            this.Text = "FrmMessage";
            this.Load += new System.EventHandler(this.FrmMessage_Load);
            this.pnl_msg.ResumeLayout(false);
            this.pnl_msg.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnl_msg;
        private System.Windows.Forms.TextBox txt_message;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
    }
}