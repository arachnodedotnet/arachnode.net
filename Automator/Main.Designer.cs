namespace Arachnode.Automator
{
    partial class Main
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
            this.tbVariables = new System.Windows.Forms.TextBox();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnClearIE = new System.Windows.Forms.Button();
            this.tbProxyServersOutput = new System.Windows.Forms.TextBox();
            this.btnValidateProxyServers = new System.Windows.Forms.Button();
            this.cbProxyServers = new Arachnode.Automator.ComboBox2();
            this.tbProxyServers = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.wbWhatIsMyIP = new System.Windows.Forms.WebBrowser();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbCookieValueTabOne = new System.Windows.Forms.TextBox();
            this.wbTabOne = new System.Windows.Forms.WebBrowser();
            this.btnResetWbOne = new System.Windows.Forms.Button();
            this.tbAbsoluteUriTabOne = new Arachnode.Automator.TextBox2();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbCookieValueTabTwo = new System.Windows.Forms.TextBox();
            this.btnResetWbTwo = new System.Windows.Forms.Button();
            this.wbTabTwo = new System.Windows.Forms.WebBrowser();
            this.tbAbsoluteUriTabTwo = new Arachnode.Automator.TextBox2();
            this.btnPopulate = new System.Windows.Forms.Button();
            this.btnUnmark = new System.Windows.Forms.Button();
            this.btnMark = new System.Windows.Forms.Button();
            this.dgvOne = new System.Windows.Forms.DataGridView();
            this.dgvtbcIdOne = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcValueOne = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcLinkOne = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcbcSaveOne = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvTwo = new System.Windows.Forms.DataGridView();
            this.dgvtbcIdTwo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcValueTwo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcLinkTwo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcbcSaveTwo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.tcMain.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOne)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTwo)).BeginInit();
            this.SuspendLayout();
            // 
            // tbVariables
            // 
            this.tbVariables.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbVariables.Location = new System.Drawing.Point(1035, 516);
            this.tbVariables.Multiline = true;
            this.tbVariables.Name = "tbVariables";
            this.tbVariables.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbVariables.Size = new System.Drawing.Size(354, 425);
            this.tbVariables.TabIndex = 3;
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tabPage3);
            this.tcMain.Controls.Add(this.tabPage4);
            this.tcMain.Controls.Add(this.tabPage1);
            this.tcMain.Controls.Add(this.tabPage2);
            this.tcMain.Location = new System.Drawing.Point(3, 3);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(1026, 942);
            this.tcMain.TabIndex = 4;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnClearIE);
            this.tabPage3.Controls.Add(this.tbProxyServersOutput);
            this.tabPage3.Controls.Add(this.btnValidateProxyServers);
            this.tabPage3.Controls.Add(this.cbProxyServers);
            this.tabPage3.Controls.Add(this.tbProxyServers);
            this.tabPage3.Location = new System.Drawing.Point(4, 23);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1018, 915);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Proxy Servers";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnClearIE
            // 
            this.btnClearIE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearIE.Location = new System.Drawing.Point(932, 6);
            this.btnClearIE.Name = "btnClearIE";
            this.btnClearIE.Size = new System.Drawing.Size(80, 25);
            this.btnClearIE.TabIndex = 5;
            this.btnClearIE.Text = "Clear IE";
            this.btnClearIE.UseVisualStyleBackColor = true;
            this.btnClearIE.Click += new System.EventHandler(this.btnClearIE_Click);
            // 
            // tbProxyServersOutput
            // 
            this.tbProxyServersOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProxyServersOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbProxyServersOutput.Location = new System.Drawing.Point(210, 37);
            this.tbProxyServersOutput.Multiline = true;
            this.tbProxyServersOutput.Name = "tbProxyServersOutput";
            this.tbProxyServersOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbProxyServersOutput.Size = new System.Drawing.Size(802, 872);
            this.tbProxyServersOutput.TabIndex = 3;
            // 
            // btnValidateProxyServers
            // 
            this.btnValidateProxyServers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnValidateProxyServers.Location = new System.Drawing.Point(846, 6);
            this.btnValidateProxyServers.Name = "btnValidateProxyServers";
            this.btnValidateProxyServers.Size = new System.Drawing.Size(80, 25);
            this.btnValidateProxyServers.TabIndex = 2;
            this.btnValidateProxyServers.Text = "Validate";
            this.btnValidateProxyServers.UseVisualStyleBackColor = true;
            this.btnValidateProxyServers.Click += new System.EventHandler(this.btnValidateProxyServers_Click);
            // 
            // cbProxyServers
            // 
            this.cbProxyServers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProxyServers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbProxyServers.Font = new System.Drawing.Font("Arial", 11.25F);
            this.cbProxyServers.FormattingEnabled = true;
            this.cbProxyServers.Location = new System.Drawing.Point(637, 6);
            this.cbProxyServers.Name = "cbProxyServers";
            this.cbProxyServers.Size = new System.Drawing.Size(203, 25);
            this.cbProxyServers.TabIndex = 1;
            this.cbProxyServers.SelectedIndexChanged += new System.EventHandler(this.cbProxyServers_SelectedIndexChanged);
            // 
            // tbProxyServers
            // 
            this.tbProxyServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tbProxyServers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbProxyServers.Location = new System.Drawing.Point(6, 6);
            this.tbProxyServers.Multiline = true;
            this.tbProxyServers.Name = "tbProxyServers";
            this.tbProxyServers.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbProxyServers.Size = new System.Drawing.Size(198, 903);
            this.tbProxyServers.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.wbWhatIsMyIP);
            this.tabPage4.Location = new System.Drawing.Point(4, 23);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1018, 915);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "IPAddress";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // wbWhatIsMyIP
            // 
            this.wbWhatIsMyIP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbWhatIsMyIP.Location = new System.Drawing.Point(0, 0);
            this.wbWhatIsMyIP.MinimumSize = new System.Drawing.Size(20, 22);
            this.wbWhatIsMyIP.Name = "wbWhatIsMyIP";
            this.wbWhatIsMyIP.ScriptErrorsSuppressed = true;
            this.wbWhatIsMyIP.Size = new System.Drawing.Size(1018, 915);
            this.wbWhatIsMyIP.TabIndex = 0;
            this.wbWhatIsMyIP.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.tbCookieValueTabOne);
            this.tabPage1.Controls.Add(this.wbTabOne);
            this.tabPage1.Controls.Add(this.btnResetWbOne);
            this.tabPage1.Controls.Add(this.tbAbsoluteUriTabOne);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1018, 915);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "One";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbCookieValueTabOne
            // 
            this.tbCookieValueTabOne.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCookieValueTabOne.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCookieValueTabOne.Location = new System.Drawing.Point(6, 889);
            this.tbCookieValueTabOne.Name = "tbCookieValueTabOne";
            this.tbCookieValueTabOne.Size = new System.Drawing.Size(1006, 20);
            this.tbCookieValueTabOne.TabIndex = 19;
            // 
            // wbTabOne
            // 
            this.wbTabOne.Location = new System.Drawing.Point(6, 37);
            this.wbTabOne.MinimumSize = new System.Drawing.Size(20, 22);
            this.wbTabOne.Name = "wbTabOne";
            this.wbTabOne.ScriptErrorsSuppressed = true;
            this.wbTabOne.Size = new System.Drawing.Size(1006, 846);
            this.wbTabOne.TabIndex = 18;
            this.wbTabOne.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wbTabOne_DocumentCompleted);
            // 
            // btnResetWbOne
            // 
            this.btnResetWbOne.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetWbOne.Location = new System.Drawing.Point(932, 6);
            this.btnResetWbOne.Name = "btnResetWbOne";
            this.btnResetWbOne.Size = new System.Drawing.Size(80, 25);
            this.btnResetWbOne.TabIndex = 10;
            this.btnResetWbOne.Text = "Reset";
            this.btnResetWbOne.UseVisualStyleBackColor = true;
            this.btnResetWbOne.Click += new System.EventHandler(this.btnResetWbOne_Click);
            // 
            // tbAbsoluteUriTabOne
            // 
            this.tbAbsoluteUriTabOne.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbAbsoluteUriTabOne.Font = new System.Drawing.Font("Arial", 13.25F);
            this.tbAbsoluteUriTabOne.Location = new System.Drawing.Point(6, 6);
            this.tbAbsoluteUriTabOne.Name = "tbAbsoluteUriTabOne";
            this.tbAbsoluteUriTabOne.Size = new System.Drawing.Size(920, 25);
            this.tbAbsoluteUriTabOne.TabIndex = 2;
            this.tbAbsoluteUriTabOne.Text = "https://shortmail.com/home/domains/shortmail.me/registration/new?src=button";
            this.tbAbsoluteUriTabOne.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbAbsoluteUriTabOne_KeyUp);
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.tbCookieValueTabTwo);
            this.tabPage2.Controls.Add(this.btnResetWbTwo);
            this.tabPage2.Controls.Add(this.wbTabTwo);
            this.tabPage2.Controls.Add(this.tbAbsoluteUriTabTwo);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1018, 915);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Two";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbCookieValueTabTwo
            // 
            this.tbCookieValueTabTwo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCookieValueTabTwo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCookieValueTabTwo.Location = new System.Drawing.Point(6, 889);
            this.tbCookieValueTabTwo.Name = "tbCookieValueTabTwo";
            this.tbCookieValueTabTwo.Size = new System.Drawing.Size(1006, 20);
            this.tbCookieValueTabTwo.TabIndex = 20;
            // 
            // btnResetWbTwo
            // 
            this.btnResetWbTwo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetWbTwo.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnResetWbTwo.Location = new System.Drawing.Point(932, 6);
            this.btnResetWbTwo.Name = "btnResetWbTwo";
            this.btnResetWbTwo.Size = new System.Drawing.Size(80, 25);
            this.btnResetWbTwo.TabIndex = 10;
            this.btnResetWbTwo.Text = "Reset";
            this.btnResetWbTwo.UseVisualStyleBackColor = true;
            this.btnResetWbTwo.Click += new System.EventHandler(this.btnResetWbTwo_Click);
            // 
            // wbTabTwo
            // 
            this.wbTabTwo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wbTabTwo.Location = new System.Drawing.Point(6, 37);
            this.wbTabTwo.MinimumSize = new System.Drawing.Size(20, 22);
            this.wbTabTwo.Name = "wbTabTwo";
            this.wbTabTwo.ScriptErrorsSuppressed = true;
            this.wbTabTwo.Size = new System.Drawing.Size(1006, 846);
            this.wbTabTwo.TabIndex = 3;
            this.wbTabTwo.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wbTabTwo_DocumentCompleted);
            // 
            // tbAbsoluteUriTabTwo
            // 
            this.tbAbsoluteUriTabTwo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbAbsoluteUriTabTwo.Font = new System.Drawing.Font("Arial", 13.25F);
            this.tbAbsoluteUriTabTwo.Location = new System.Drawing.Point(6, 6);
            this.tbAbsoluteUriTabTwo.Name = "tbAbsoluteUriTabTwo";
            this.tbAbsoluteUriTabTwo.Size = new System.Drawing.Size(920, 25);
            this.tbAbsoluteUriTabTwo.TabIndex = 4;
            this.tbAbsoluteUriTabTwo.Text = "http://facebook.com";
            this.tbAbsoluteUriTabTwo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbAbsoluteUriTabTwo_KeyUp);
            // 
            // btnPopulate
            // 
            this.btnPopulate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPopulate.Location = new System.Drawing.Point(1215, 26);
            this.btnPopulate.Name = "btnPopulate";
            this.btnPopulate.Size = new System.Drawing.Size(84, 25);
            this.btnPopulate.TabIndex = 5;
            this.btnPopulate.Text = "Populate";
            this.btnPopulate.UseVisualStyleBackColor = true;
            this.btnPopulate.Click += new System.EventHandler(this.btnPopulateOne_Click);
            // 
            // btnUnmark
            // 
            this.btnUnmark.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnmark.Location = new System.Drawing.Point(1125, 26);
            this.btnUnmark.Name = "btnUnmark";
            this.btnUnmark.Size = new System.Drawing.Size(84, 25);
            this.btnUnmark.TabIndex = 10;
            this.btnUnmark.Text = "Unmark";
            this.btnUnmark.UseVisualStyleBackColor = true;
            this.btnUnmark.Click += new System.EventHandler(this.btnUnmark_Click);
            // 
            // btnMark
            // 
            this.btnMark.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMark.Location = new System.Drawing.Point(1035, 26);
            this.btnMark.Name = "btnMark";
            this.btnMark.Size = new System.Drawing.Size(84, 25);
            this.btnMark.TabIndex = 9;
            this.btnMark.Text = "Mark";
            this.btnMark.UseVisualStyleBackColor = true;
            this.btnMark.Click += new System.EventHandler(this.btnMark_Click);
            // 
            // dgvOne
            // 
            this.dgvOne.AllowUserToAddRows = false;
            this.dgvOne.AllowUserToResizeRows = false;
            this.dgvOne.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvOne.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOne.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvtbcIdOne,
            this.dgvtbcValueOne,
            this.dgvtbcLinkOne,
            this.dgvcbcSaveOne});
            this.dgvOne.EnableHeadersVisualStyles = false;
            this.dgvOne.GridColor = System.Drawing.SystemColors.Control;
            this.dgvOne.Location = new System.Drawing.Point(1035, 57);
            this.dgvOne.Name = "dgvOne";
            this.dgvOne.RowHeadersVisible = false;
            this.dgvOne.RowHeadersWidth = 4;
            this.dgvOne.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvOne.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvOne.Size = new System.Drawing.Size(354, 223);
            this.dgvOne.TabIndex = 13;
            this.dgvOne.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOne_CellValueChanged);
            this.dgvOne.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvOne_UserDeletingRow);
            this.dgvOne.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvOne_RowsAdded);
            // 
            // dgvtbcIdOne
            // 
            this.dgvtbcIdOne.HeaderText = "Id";
            this.dgvtbcIdOne.Name = "dgvtbcIdOne";
            // 
            // dgvtbcValueOne
            // 
            this.dgvtbcValueOne.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcValueOne.HeaderText = "Value";
            this.dgvtbcValueOne.Name = "dgvtbcValueOne";
            // 
            // dgvtbcLinkOne
            // 
            this.dgvtbcLinkOne.HeaderText = "Link";
            this.dgvtbcLinkOne.Name = "dgvtbcLinkOne";
            // 
            // dgvcbcSaveOne
            // 
            this.dgvcbcSaveOne.HeaderText = "Save";
            this.dgvcbcSaveOne.Name = "dgvcbcSaveOne";
            this.dgvcbcSaveOne.Width = 40;
            // 
            // dgvTwo
            // 
            this.dgvTwo.AllowUserToAddRows = false;
            this.dgvTwo.AllowUserToResizeRows = false;
            this.dgvTwo.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvTwo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTwo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvtbcIdTwo,
            this.dgvtbcValueTwo,
            this.dgvtbcLinkTwo,
            this.dgvcbcSaveTwo});
            this.dgvTwo.EnableHeadersVisualStyles = false;
            this.dgvTwo.GridColor = System.Drawing.SystemColors.Control;
            this.dgvTwo.Location = new System.Drawing.Point(1035, 286);
            this.dgvTwo.Name = "dgvTwo";
            this.dgvTwo.RowHeadersVisible = false;
            this.dgvTwo.RowHeadersWidth = 4;
            this.dgvTwo.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvTwo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvTwo.Size = new System.Drawing.Size(354, 223);
            this.dgvTwo.TabIndex = 14;
            this.dgvTwo.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTwo_CellValueChanged);
            this.dgvTwo.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvTwo_UserDeletingRow);
            this.dgvTwo.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvTwo_RowsAdded);
            // 
            // dgvtbcIdTwo
            // 
            this.dgvtbcIdTwo.HeaderText = "Id";
            this.dgvtbcIdTwo.Name = "dgvtbcIdTwo";
            // 
            // dgvtbcValueTwo
            // 
            this.dgvtbcValueTwo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcValueTwo.HeaderText = "Value";
            this.dgvtbcValueTwo.Name = "dgvtbcValueTwo";
            // 
            // dgvtbcLinkTwo
            // 
            this.dgvtbcLinkTwo.HeaderText = "Link";
            this.dgvtbcLinkTwo.Name = "dgvtbcLinkTwo";
            // 
            // dgvcbcSaveTwo
            // 
            this.dgvcbcSaveTwo.HeaderText = "Save";
            this.dgvcbcSaveTwo.Name = "dgvcbcSaveTwo";
            this.dgvcbcSaveTwo.Width = 40;
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(1305, 26);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(84, 25);
            this.btnSave.TabIndex = 16;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1394, 947);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvTwo);
            this.Controls.Add(this.dgvOne);
            this.Controls.Add(this.btnUnmark);
            this.Controls.Add(this.btnMark);
            this.Controls.Add(this.btnPopulate);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.tbVariables);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Main";
            this.ShowIcon = false;
            this.Text = "arachnode.net | Automator";
            this.tcMain.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOne)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTwo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox2 tbAbsoluteUriTabOne;
        private System.Windows.Forms.TextBox tbVariables;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private TextBox2 tbAbsoluteUriTabTwo;
        private System.Windows.Forms.WebBrowser wbTabTwo;
        private System.Windows.Forms.Button btnPopulate;
        private System.Windows.Forms.Button btnUnmark;
        private System.Windows.Forms.Button btnMark;
        private System.Windows.Forms.DataGridView dgvOne;
        private System.Windows.Forms.DataGridView dgvTwo;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnResetWbOne;
        private System.Windows.Forms.Button btnResetWbTwo;
        private System.Windows.Forms.WebBrowser wbTabOne;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox tbProxyServers;
        private System.Windows.Forms.Button btnValidateProxyServers;
        private ComboBox2 cbProxyServers;
        private System.Windows.Forms.TextBox tbProxyServersOutput;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.WebBrowser wbWhatIsMyIP;
        private System.Windows.Forms.TextBox tbCookieValueTabOne;
        private System.Windows.Forms.TextBox tbCookieValueTabTwo;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcIdOne;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcValueOne;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcLinkOne;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvcbcSaveOne;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcIdTwo;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcValueTwo;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcLinkTwo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvcbcSaveTwo;
        private System.Windows.Forms.Button btnClearIE;
    }
}

