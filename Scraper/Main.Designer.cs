namespace Arachnode.Scraper
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sQLServerManagementStudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notepadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taskManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCurrentDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tpCrawler = new System.Windows.Forms.TabPage();
            this.tpScrapeSettings = new System.Windows.Forms.TabPage();
            this.tpViewSource = new System.Windows.Forms.TabPage();
            this.rtbViewSource = new System.Windows.Forms.RichTextBox();
            this.tpBrowser = new System.Windows.Forms.TabPage();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tvBrowser = new System.Windows.Forms.TreeView();
            this.wbBrowser = new System.Windows.Forms.WebBrowser();
            this.tbXPath = new System.Windows.Forms.TextBox();
            this.tbAbsoluteUri = new System.Windows.Forms.TextBox();
            this.btnEvaluateXPath = new System.Windows.Forms.Button();
            this.btnAddXPath = new System.Windows.Forms.Button();
            this.tpPathFilter = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.tbGetPathsFrom = new System.Windows.Forms.TextBox();
            this.btnGetPaths = new System.Windows.Forms.Button();
            this.dgvPathFilter = new System.Windows.Forms.DataGridView();
            this.dgvtbcPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcbcCrawlAction = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgvcbcScrapeAction = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.menuStrip1.SuspendLayout();
            this.tpViewSource.SuspendLayout();
            this.tpBrowser.SuspendLayout();
            this.tpPathFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPathFilter)).BeginInit();
            this.tcMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1263, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStripMenuItem.Text = "Open...";
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sQLServerManagementStudioToolStripMenuItem,
            this.notepadToolStripMenuItem,
            this.taskManagerToolStripMenuItem,
            this.openCurrentDirectoryToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // sQLServerManagementStudioToolStripMenuItem
            // 
            this.sQLServerManagementStudioToolStripMenuItem.Name = "sQLServerManagementStudioToolStripMenuItem";
            this.sQLServerManagementStudioToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.sQLServerManagementStudioToolStripMenuItem.Text = "SQL Server Management Studio";
            // 
            // notepadToolStripMenuItem
            // 
            this.notepadToolStripMenuItem.Name = "notepadToolStripMenuItem";
            this.notepadToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.notepadToolStripMenuItem.Text = "Notepad";
            // 
            // taskManagerToolStripMenuItem
            // 
            this.taskManagerToolStripMenuItem.Name = "taskManagerToolStripMenuItem";
            this.taskManagerToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.taskManagerToolStripMenuItem.Text = "Task Manager";
            // 
            // openCurrentDirectoryToolStripMenuItem
            // 
            this.openCurrentDirectoryToolStripMenuItem.Name = "openCurrentDirectoryToolStripMenuItem";
            this.openCurrentDirectoryToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.openCurrentDirectoryToolStripMenuItem.Text = "Open Current Directory";
            // 
            // tpCrawler
            // 
            this.tpCrawler.Location = new System.Drawing.Point(4, 23);
            this.tpCrawler.Name = "tpCrawler";
            this.tpCrawler.Padding = new System.Windows.Forms.Padding(3);
            this.tpCrawler.Size = new System.Drawing.Size(1249, 890);
            this.tpCrawler.TabIndex = 3;
            this.tpCrawler.Text = "Crawler";
            this.tpCrawler.UseVisualStyleBackColor = true;
            // 
            // tpScrapeSettings
            // 
            this.tpScrapeSettings.Location = new System.Drawing.Point(4, 23);
            this.tpScrapeSettings.Name = "tpScrapeSettings";
            this.tpScrapeSettings.Size = new System.Drawing.Size(1249, 890);
            this.tpScrapeSettings.TabIndex = 6;
            this.tpScrapeSettings.Text = "Scrape Settings";
            this.tpScrapeSettings.UseVisualStyleBackColor = true;
            // 
            // tpViewSource
            // 
            this.tpViewSource.Controls.Add(this.rtbViewSource);
            this.tpViewSource.Location = new System.Drawing.Point(4, 23);
            this.tpViewSource.Name = "tpViewSource";
            this.tpViewSource.Padding = new System.Windows.Forms.Padding(3);
            this.tpViewSource.Size = new System.Drawing.Size(1249, 890);
            this.tpViewSource.TabIndex = 4;
            this.tpViewSource.Text = "View Source";
            this.tpViewSource.UseVisualStyleBackColor = true;
            // 
            // rtbViewSource
            // 
            this.rtbViewSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbViewSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbViewSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbViewSource.Location = new System.Drawing.Point(3, 3);
            this.rtbViewSource.Name = "rtbViewSource";
            this.rtbViewSource.Size = new System.Drawing.Size(1243, 884);
            this.rtbViewSource.TabIndex = 0;
            this.rtbViewSource.Text = "";
            // 
            // tpBrowser
            // 
            this.tpBrowser.Controls.Add(this.tbResult);
            this.tpBrowser.Controls.Add(this.button2);
            this.tpBrowser.Controls.Add(this.button3);
            this.tpBrowser.Controls.Add(this.tvBrowser);
            this.tpBrowser.Controls.Add(this.wbBrowser);
            this.tpBrowser.Controls.Add(this.tbXPath);
            this.tpBrowser.Controls.Add(this.tbAbsoluteUri);
            this.tpBrowser.Controls.Add(this.btnEvaluateXPath);
            this.tpBrowser.Controls.Add(this.btnAddXPath);
            this.tpBrowser.Location = new System.Drawing.Point(4, 23);
            this.tpBrowser.Name = "tpBrowser";
            this.tpBrowser.Padding = new System.Windows.Forms.Padding(3);
            this.tpBrowser.Size = new System.Drawing.Size(1249, 890);
            this.tpBrowser.TabIndex = 0;
            this.tpBrowser.Text = "Browser";
            this.tpBrowser.UseVisualStyleBackColor = true;
            // 
            // tbResult
            // 
            this.tbResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbResult.Font = new System.Drawing.Font("Arial", 11.25F);
            this.tbResult.Location = new System.Drawing.Point(3, 859);
            this.tbResult.Name = "tbResult";
            this.tbResult.Size = new System.Drawing.Size(1240, 25);
            this.tbResult.TabIndex = 19;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(1168, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 25);
            this.button2.TabIndex = 18;
            this.button2.Text = "Browse";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(1087, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 25);
            this.button3.TabIndex = 17;
            this.button3.Text = "Get Paths";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // tvBrowser
            // 
            this.tvBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvBrowser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvBrowser.HotTracking = true;
            this.tvBrowser.Location = new System.Drawing.Point(997, 37);
            this.tvBrowser.Name = "tvBrowser";
            this.tvBrowser.ShowNodeToolTips = true;
            this.tvBrowser.Size = new System.Drawing.Size(246, 786);
            this.tvBrowser.TabIndex = 15;
            this.tvBrowser.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvBrowser_AfterSelect);
            // 
            // wbBrowser
            // 
            this.wbBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wbBrowser.Location = new System.Drawing.Point(6, 37);
            this.wbBrowser.MinimumSize = new System.Drawing.Size(20, 21);
            this.wbBrowser.Name = "wbBrowser";
            this.wbBrowser.ScriptErrorsSuppressed = true;
            this.wbBrowser.Size = new System.Drawing.Size(985, 784);
            this.wbBrowser.TabIndex = 1;
            this.wbBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.wbBrowser_Navigating);
            this.wbBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wbBrowser_DocumentCompleted);
            this.wbBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.wbBrowser_Navigated);
            // 
            // tbXPath
            // 
            this.tbXPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbXPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbXPath.Font = new System.Drawing.Font("Arial", 11.25F);
            this.tbXPath.Location = new System.Drawing.Point(3, 829);
            this.tbXPath.Name = "tbXPath";
            this.tbXPath.Size = new System.Drawing.Size(1078, 25);
            this.tbXPath.TabIndex = 12;
            // 
            // tbAbsoluteUri
            // 
            this.tbAbsoluteUri.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAbsoluteUri.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbAbsoluteUri.Font = new System.Drawing.Font("Arial", 11.25F);
            this.tbAbsoluteUri.Location = new System.Drawing.Point(6, 6);
            this.tbAbsoluteUri.Name = "tbAbsoluteUri";
            this.tbAbsoluteUri.Size = new System.Drawing.Size(1075, 25);
            this.tbAbsoluteUri.TabIndex = 0;
            this.tbAbsoluteUri.Text = "http://direct.arachnode.net";
            this.tbAbsoluteUri.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbAbsoluteUri_KeyUp);
            // 
            // btnEvaluateXPath
            // 
            this.btnEvaluateXPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEvaluateXPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEvaluateXPath.Location = new System.Drawing.Point(1087, 829);
            this.btnEvaluateXPath.Name = "btnEvaluateXPath";
            this.btnEvaluateXPath.Size = new System.Drawing.Size(75, 25);
            this.btnEvaluateXPath.TabIndex = 13;
            this.btnEvaluateXPath.Text = "Evaluate";
            this.btnEvaluateXPath.UseVisualStyleBackColor = true;
            this.btnEvaluateXPath.Click += new System.EventHandler(this.btnEvaluateXPath_Click);
            // 
            // btnAddXPath
            // 
            this.btnAddXPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddXPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddXPath.Location = new System.Drawing.Point(1168, 829);
            this.btnAddXPath.Name = "btnAddXPath";
            this.btnAddXPath.Size = new System.Drawing.Size(75, 25);
            this.btnAddXPath.TabIndex = 11;
            this.btnAddXPath.Text = "Add";
            this.btnAddXPath.UseVisualStyleBackColor = true;
            // 
            // tpPathFilter
            // 
            this.tpPathFilter.Controls.Add(this.button1);
            this.tpPathFilter.Controls.Add(this.tbGetPathsFrom);
            this.tpPathFilter.Controls.Add(this.btnGetPaths);
            this.tpPathFilter.Controls.Add(this.dgvPathFilter);
            this.tpPathFilter.Location = new System.Drawing.Point(4, 23);
            this.tpPathFilter.Name = "tpPathFilter";
            this.tpPathFilter.Size = new System.Drawing.Size(1249, 890);
            this.tpPathFilter.TabIndex = 5;
            this.tpPathFilter.Text = "Path Filter";
            this.tpPathFilter.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(1169, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 25);
            this.button1.TabIndex = 16;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tbGetPathsFrom
            // 
            this.tbGetPathsFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGetPathsFrom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbGetPathsFrom.Font = new System.Drawing.Font("Arial", 11.25F);
            this.tbGetPathsFrom.Location = new System.Drawing.Point(6, 6);
            this.tbGetPathsFrom.Name = "tbGetPathsFrom";
            this.tbGetPathsFrom.Size = new System.Drawing.Size(1076, 25);
            this.tbGetPathsFrom.TabIndex = 15;
            this.tbGetPathsFrom.Text = "http://direct.arachnode.net";
            // 
            // btnGetPaths
            // 
            this.btnGetPaths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetPaths.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetPaths.Location = new System.Drawing.Point(1088, 6);
            this.btnGetPaths.Name = "btnGetPaths";
            this.btnGetPaths.Size = new System.Drawing.Size(75, 25);
            this.btnGetPaths.TabIndex = 14;
            this.btnGetPaths.Text = "Get Paths";
            this.btnGetPaths.UseVisualStyleBackColor = true;
            this.btnGetPaths.Click += new System.EventHandler(this.btnGetPaths_Click);
            // 
            // dgvPathFilter
            // 
            this.dgvPathFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPathFilter.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPathFilter.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvPathFilter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPathFilter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvtbcPath,
            this.dgvcbcCrawlAction,
            this.dgvcbcScrapeAction});
            this.dgvPathFilter.EnableHeadersVisualStyles = false;
            this.dgvPathFilter.Location = new System.Drawing.Point(6, 37);
            this.dgvPathFilter.Name = "dgvPathFilter";
            this.dgvPathFilter.Size = new System.Drawing.Size(1238, 850);
            this.dgvPathFilter.TabIndex = 13;
            // 
            // dgvtbcPath
            // 
            this.dgvtbcPath.FillWeight = 73.85786F;
            this.dgvtbcPath.HeaderText = "Path";
            this.dgvtbcPath.Name = "dgvtbcPath";
            // 
            // dgvcbcCrawlAction
            // 
            this.dgvcbcCrawlAction.FillWeight = 12.5F;
            this.dgvcbcCrawlAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dgvcbcCrawlAction.HeaderText = "Crawl Action";
            this.dgvcbcCrawlAction.Items.AddRange(new object[] {
            "Crawl",
            "Do Not Crawl"});
            this.dgvcbcCrawlAction.Name = "dgvcbcCrawlAction";
            this.dgvcbcCrawlAction.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvcbcCrawlAction.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dgvcbcScrapeAction
            // 
            this.dgvcbcScrapeAction.FillWeight = 12.5F;
            this.dgvcbcScrapeAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dgvcbcScrapeAction.HeaderText = "Scrape Action";
            this.dgvcbcScrapeAction.Items.AddRange(new object[] {
            "Scrape",
            "Do Not Scrape"});
            this.dgvcbcScrapeAction.Name = "dgvcbcScrapeAction";
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tpBrowser);
            this.tcMain.Controls.Add(this.tpPathFilter);
            this.tcMain.Controls.Add(this.tpViewSource);
            this.tcMain.Controls.Add(this.tpScrapeSettings);
            this.tcMain.Controls.Add(this.tpCrawler);
            this.tcMain.Location = new System.Drawing.Point(3, 27);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(1257, 917);
            this.tcMain.TabIndex = 2;
            this.tcMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tcMain_MouseUp);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1263, 947);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.ShowIcon = false;
            this.Text = "arachnode.net | scraper";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tpViewSource.ResumeLayout(false);
            this.tpBrowser.ResumeLayout(false);
            this.tpBrowser.PerformLayout();
            this.tpPathFilter.ResumeLayout(false);
            this.tpPathFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPathFilter)).EndInit();
            this.tcMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.TabPage tpCrawler;
        private System.Windows.Forms.TabPage tpScrapeSettings;
        private System.Windows.Forms.TabPage tpViewSource;
        private System.Windows.Forms.RichTextBox rtbViewSource;
        private System.Windows.Forms.TabPage tpBrowser;
        private System.Windows.Forms.WebBrowser wbBrowser;
        private System.Windows.Forms.TextBox tbXPath;
        private System.Windows.Forms.TextBox tbAbsoluteUri;
        private System.Windows.Forms.Button btnEvaluateXPath;
        private System.Windows.Forms.Button btnAddXPath;
        private System.Windows.Forms.TabPage tpPathFilter;
        private System.Windows.Forms.TextBox tbGetPathsFrom;
        private System.Windows.Forms.Button btnGetPaths;
        private System.Windows.Forms.DataGridView dgvPathFilter;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sQLServerManagementStudioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notepadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem taskManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCurrentDirectoryToolStripMenuItem;
        private System.Windows.Forms.TreeView tvBrowser;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcPath;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvcbcCrawlAction;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvcbcScrapeAction;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbResult;
    }
}

