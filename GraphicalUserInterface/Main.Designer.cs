namespace GraphicalUserInterface
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            this.dgvPerformanceCounters = new System.Windows.Forms.DataGridView();
            this.dgvtbcCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcAverage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcMaximum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpConfiguration = new System.Windows.Forms.TabPage();
            this.dgvTableName = new System.Windows.Forms.DataGridView();
            this.dgvtbcTableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvConfiguration = new System.Windows.Forms.DataGridView();
            this.tpApplicationSettings = new System.Windows.Forms.TabPage();
            this.btnSaveApplicationSettings = new System.Windows.Forms.Button();
            this.btnLoadApplicationsSettings = new System.Windows.Forms.Button();
            this.dgvApplicationSettings = new System.Windows.Forms.DataGridView();
            this.dgvtbcApplicationSettingsName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcApplicationSettingsValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpWebSettings = new System.Windows.Forms.TabPage();
            this.btnSaveWebSettings = new System.Windows.Forms.Button();
            this.btnLoadWebSettings = new System.Windows.Forms.Button();
            this.dgvWebSettings = new System.Windows.Forms.DataGridView();
            this.dgvtbcWebSettingsName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcWebSettingsValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpCredentialCache = new System.Windows.Forms.TabPage();
            this.btnSaveCredentialCache = new System.Windows.Forms.Button();
            this.btnLoadCredentialCache = new System.Windows.Forms.Button();
            this.dgvCredentialCache = new System.Windows.Forms.DataGridView();
            this.dgvcbcSchemeCredentialCache = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgvtbcDomainCredentialCache = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcUserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcPassword = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcbcIsEnabledCredentialCache = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tpCookieContainer = new System.Windows.Forms.TabPage();
            this.btnSaveCookieContainer = new System.Windows.Forms.Button();
            this.btnLoadCookieContainer = new System.Windows.Forms.Button();
            this.dgvCookieContainer = new System.Windows.Forms.DataGridView();
            this.dgvcbcSchemeCookieContainer = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgvtbcDomainCookieContainer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgctbcValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcbcIsEnabledCookieContainer = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tpProxyServers = new System.Windows.Forms.TabPage();
            this.btnVerify = new System.Windows.Forms.Button();
            this.btnSaveProxyServers = new System.Windows.Forms.Button();
            this.btnLoadProxyServers = new System.Windows.Forms.Button();
            this.dgvProxyServers = new System.Windows.Forms.DataGridView();
            this.dgvcbcSchemeProxyServers = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgvtbcIPAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcAbsoluteUriToVerify = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcValueToVerify = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcTimeoutInMilliseconds = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcStatusCodeProxyServers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcbcIsEnabledProxyServers = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tpCrawlRequests = new System.Windows.Forms.TabPage();
            this.btnSaveCrawlRequests = new System.Windows.Forms.Button();
            this.btnLoadCrawlRequests = new System.Windows.Forms.Button();
            this.dgvCrawlRequests = new System.Windows.Forms.DataGridView();
            this.dgvtbcCRAbsoluteUri = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcCRDepth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcRestrictCrawlTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcRestrictDiscoveriesTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcCRPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcRenderType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcRenderTypeForChildren = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcCRAdded = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpCrawler = new System.Windows.Forms.TabPage();
            this.btnSaveCrawler = new System.Windows.Forms.Button();
            this.btnLoadCrawler = new System.Windows.Forms.Button();
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.dgvCrawler = new System.Windows.Forms.DataGridView();
            this.dgvtbcThreadNumberCrawler = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcAbsoluteUri = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcStatusCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcDiscoveryType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcHyperLinkDiscoveries = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcFileAndImageDiscoveries = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcCurrentDepth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcMaximumDepth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.cmsMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.tss1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCut = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.tpCrawlInfo = new System.Windows.Forms.TabPage();
            this.dgvCrawlInfo = new System.Windows.Forms.DataGridView();
            this.dgvtbcThreadNumberCrawlInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcParentDiscovery = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcCurrentDiscovery = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcCrawlState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcTotalCrawlFedCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcTotalCrawlRequestsAssigned = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcTotalCrawlStarvedCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcTotalHttpWebResponseTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSaveCrawlInfo = new System.Windows.Forms.Button();
            this.btnLoadCrawlInfo = new System.Windows.Forms.Button();
            this.tpPerformanceCounters = new System.Windows.Forms.TabPage();
            this.btnSavePerformanceCounters = new System.Windows.Forms.Button();
            this.btnLoadPerformanceCounters = new System.Windows.Forms.Button();
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCrawlerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetDirectoriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetIISToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startPerfmonexeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.resetGUIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.breadthFirstByPriorityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.depthFirstByPriorityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetDatabaseOnStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCrawlerOnStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.automatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.postProcessingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scraperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notepadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCurrentDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performanceMonitorMSCtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sQLServerManagementStudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taskManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sfdEntityRows = new System.Windows.Forms.SaveFileDialog();
            this.ssMain = new System.Windows.Forms.StatusStrip();
            this.tsslEngineState = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslElapasedTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslApplicationTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.ofdEntityRows = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerformanceCounters)).BeginInit();
            this.tcMain.SuspendLayout();
            this.tpConfiguration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConfiguration)).BeginInit();
            this.tpApplicationSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvApplicationSettings)).BeginInit();
            this.tpWebSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWebSettings)).BeginInit();
            this.tpCredentialCache.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCredentialCache)).BeginInit();
            this.tpCookieContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCookieContainer)).BeginInit();
            this.tpProxyServers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProxyServers)).BeginInit();
            this.tpCrawlRequests.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCrawlRequests)).BeginInit();
            this.tpCrawler.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCrawler)).BeginInit();
            this.cmsMain.SuspendLayout();
            this.tpCrawlInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCrawlInfo)).BeginInit();
            this.tpPerformanceCounters.SuspendLayout();
            this.msMain.SuspendLayout();
            this.ssMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvPerformanceCounters
            // 
            this.dgvPerformanceCounters.AllowUserToAddRows = false;
            this.dgvPerformanceCounters.AllowUserToDeleteRows = false;
            this.dgvPerformanceCounters.AllowUserToResizeColumns = false;
            this.dgvPerformanceCounters.AllowUserToResizeRows = false;
            this.dgvPerformanceCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPerformanceCounters.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvPerformanceCounters.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPerformanceCounters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPerformanceCounters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvtbcCategory,
            this.dgvtbcName,
            this.dgvtbcValue,
            this.dgvtbcAverage,
            this.dgvtbcMaximum,
            this.dgvtbcTotal});
            this.dgvPerformanceCounters.EnableHeadersVisualStyles = false;
            this.dgvPerformanceCounters.Location = new System.Drawing.Point(6, 37);
            this.dgvPerformanceCounters.Name = "dgvPerformanceCounters";
            this.dgvPerformanceCounters.ReadOnly = true;
            this.dgvPerformanceCounters.RowHeadersVisible = false;
            this.dgvPerformanceCounters.Size = new System.Drawing.Size(1239, 827);
            this.dgvPerformanceCounters.TabIndex = 0;
            // 
            // dgvtbcCategory
            // 
            this.dgvtbcCategory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcCategory.FillWeight = 50F;
            this.dgvtbcCategory.HeaderText = "Category";
            this.dgvtbcCategory.Name = "dgvtbcCategory";
            this.dgvtbcCategory.ReadOnly = true;
            this.dgvtbcCategory.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcName
            // 
            this.dgvtbcName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcName.HeaderText = "Name";
            this.dgvtbcName.Name = "dgvtbcName";
            this.dgvtbcName.ReadOnly = true;
            this.dgvtbcName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcValue
            // 
            this.dgvtbcValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcValue.FillWeight = 20F;
            this.dgvtbcValue.HeaderText = "Value";
            this.dgvtbcValue.Name = "dgvtbcValue";
            this.dgvtbcValue.ReadOnly = true;
            this.dgvtbcValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcAverage
            // 
            this.dgvtbcAverage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcAverage.FillWeight = 20F;
            this.dgvtbcAverage.HeaderText = "Average";
            this.dgvtbcAverage.Name = "dgvtbcAverage";
            this.dgvtbcAverage.ReadOnly = true;
            this.dgvtbcAverage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcMaximum
            // 
            this.dgvtbcMaximum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcMaximum.FillWeight = 20F;
            this.dgvtbcMaximum.HeaderText = "Maximum";
            this.dgvtbcMaximum.Name = "dgvtbcMaximum";
            this.dgvtbcMaximum.ReadOnly = true;
            this.dgvtbcMaximum.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcTotal
            // 
            this.dgvtbcTotal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcTotal.FillWeight = 20F;
            this.dgvtbcTotal.HeaderText = "Total";
            this.dgvtbcTotal.Name = "dgvtbcTotal";
            this.dgvtbcTotal.ReadOnly = true;
            this.dgvtbcTotal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpConfiguration);
            this.tcMain.Controls.Add(this.tpApplicationSettings);
            this.tcMain.Controls.Add(this.tpWebSettings);
            this.tcMain.Controls.Add(this.tpCredentialCache);
            this.tcMain.Controls.Add(this.tpCookieContainer);
            this.tcMain.Controls.Add(this.tpProxyServers);
            this.tcMain.Controls.Add(this.tpCrawlRequests);
            this.tcMain.Controls.Add(this.tpCrawler);
            this.tcMain.Controls.Add(this.tpCrawlInfo);
            this.tcMain.Controls.Add(this.tpPerformanceCounters);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(3, 27);
            this.tcMain.Name = "tcMain";
            this.tcMain.Padding = new System.Drawing.Point(3, 3);
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(1257, 917);
            this.tcMain.TabIndex = 1;
            // 
            // tpConfiguration
            // 
            this.tpConfiguration.BackColor = System.Drawing.Color.Transparent;
            this.tpConfiguration.Controls.Add(this.dgvTableName);
            this.tpConfiguration.Controls.Add(this.dgvConfiguration);
            this.tpConfiguration.Location = new System.Drawing.Point(4, 23);
            this.tpConfiguration.Name = "tpConfiguration";
            this.tpConfiguration.Padding = new System.Windows.Forms.Padding(3);
            this.tpConfiguration.Size = new System.Drawing.Size(1249, 890);
            this.tpConfiguration.TabIndex = 2;
            this.tpConfiguration.Text = " Configuration ";
            this.tpConfiguration.UseVisualStyleBackColor = true;
            // 
            // dgvTableName
            // 
            this.dgvTableName.AllowUserToAddRows = false;
            this.dgvTableName.AllowUserToDeleteRows = false;
            this.dgvTableName.AllowUserToResizeColumns = false;
            this.dgvTableName.AllowUserToResizeRows = false;
            this.dgvTableName.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvTableName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTableName.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTableName.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvtbcTableName});
            this.dgvTableName.EnableHeadersVisualStyles = false;
            this.dgvTableName.Location = new System.Drawing.Point(6, 6);
            this.dgvTableName.MultiSelect = false;
            this.dgvTableName.Name = "dgvTableName";
            this.dgvTableName.RowHeadersVisible = false;
            this.dgvTableName.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTableName.Size = new System.Drawing.Size(156, 858);
            this.dgvTableName.TabIndex = 1;
            this.dgvTableName.SelectionChanged += new System.EventHandler(this.dgvTableName_SelectionChanged);
            // 
            // dgvtbcTableName
            // 
            this.dgvtbcTableName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcTableName.HeaderText = "Table Name";
            this.dgvtbcTableName.Name = "dgvtbcTableName";
            this.dgvtbcTableName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvConfiguration
            // 
            this.dgvConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvConfiguration.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvConfiguration.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvConfiguration.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConfiguration.EnableHeadersVisualStyles = false;
            this.dgvConfiguration.Location = new System.Drawing.Point(167, 6);
            this.dgvConfiguration.Name = "dgvConfiguration";
            this.dgvConfiguration.Size = new System.Drawing.Size(1077, 858);
            this.dgvConfiguration.TabIndex = 0;
            // 
            // tpApplicationSettings
            // 
            this.tpApplicationSettings.Controls.Add(this.btnSaveApplicationSettings);
            this.tpApplicationSettings.Controls.Add(this.btnLoadApplicationsSettings);
            this.tpApplicationSettings.Controls.Add(this.dgvApplicationSettings);
            this.tpApplicationSettings.Location = new System.Drawing.Point(4, 23);
            this.tpApplicationSettings.Name = "tpApplicationSettings";
            this.tpApplicationSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tpApplicationSettings.Size = new System.Drawing.Size(1249, 890);
            this.tpApplicationSettings.TabIndex = 4;
            this.tpApplicationSettings.Text = " Application Settings ";
            this.tpApplicationSettings.UseVisualStyleBackColor = true;
            // 
            // btnSaveApplicationSettings
            // 
            this.btnSaveApplicationSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveApplicationSettings.Location = new System.Drawing.Point(87, 6);
            this.btnSaveApplicationSettings.Name = "btnSaveApplicationSettings";
            this.btnSaveApplicationSettings.Size = new System.Drawing.Size(75, 25);
            this.btnSaveApplicationSettings.TabIndex = 4;
            this.btnSaveApplicationSettings.Text = "Save";
            this.btnSaveApplicationSettings.UseVisualStyleBackColor = true;
            this.btnSaveApplicationSettings.Click += new System.EventHandler(this.btnSaveApplicationSettings_Click);
            // 
            // btnLoadApplicationsSettings
            // 
            this.btnLoadApplicationsSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadApplicationsSettings.Location = new System.Drawing.Point(6, 6);
            this.btnLoadApplicationsSettings.Name = "btnLoadApplicationsSettings";
            this.btnLoadApplicationsSettings.Size = new System.Drawing.Size(75, 25);
            this.btnLoadApplicationsSettings.TabIndex = 3;
            this.btnLoadApplicationsSettings.Text = "Load";
            this.btnLoadApplicationsSettings.UseVisualStyleBackColor = true;
            this.btnLoadApplicationsSettings.Click += new System.EventHandler(this.btnLoadApplicationsSettings_Click);
            // 
            // dgvApplicationSettings
            // 
            this.dgvApplicationSettings.AllowUserToAddRows = false;
            this.dgvApplicationSettings.AllowUserToDeleteRows = false;
            this.dgvApplicationSettings.AllowUserToResizeColumns = false;
            this.dgvApplicationSettings.AllowUserToResizeRows = false;
            this.dgvApplicationSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvApplicationSettings.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvApplicationSettings.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvApplicationSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvApplicationSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvtbcApplicationSettingsName,
            this.dgvtbcApplicationSettingsValue});
            this.dgvApplicationSettings.EnableHeadersVisualStyles = false;
            this.dgvApplicationSettings.Location = new System.Drawing.Point(6, 37);
            this.dgvApplicationSettings.Name = "dgvApplicationSettings";
            this.dgvApplicationSettings.RowHeadersVisible = false;
            this.dgvApplicationSettings.Size = new System.Drawing.Size(1239, 827);
            this.dgvApplicationSettings.TabIndex = 0;
            this.dgvApplicationSettings.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvApplicationSettings_CellValueChanged);
            this.dgvApplicationSettings.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvApplicationSettings_DataError);
            // 
            // dgvtbcApplicationSettingsName
            // 
            this.dgvtbcApplicationSettingsName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcApplicationSettingsName.HeaderText = "Name";
            this.dgvtbcApplicationSettingsName.Name = "dgvtbcApplicationSettingsName";
            this.dgvtbcApplicationSettingsName.ReadOnly = true;
            this.dgvtbcApplicationSettingsName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcApplicationSettingsValue
            // 
            this.dgvtbcApplicationSettingsValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcApplicationSettingsValue.FillWeight = 50F;
            this.dgvtbcApplicationSettingsValue.HeaderText = "Value";
            this.dgvtbcApplicationSettingsValue.Name = "dgvtbcApplicationSettingsValue";
            this.dgvtbcApplicationSettingsValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tpWebSettings
            // 
            this.tpWebSettings.Controls.Add(this.btnSaveWebSettings);
            this.tpWebSettings.Controls.Add(this.btnLoadWebSettings);
            this.tpWebSettings.Controls.Add(this.dgvWebSettings);
            this.tpWebSettings.Location = new System.Drawing.Point(4, 23);
            this.tpWebSettings.Name = "tpWebSettings";
            this.tpWebSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tpWebSettings.Size = new System.Drawing.Size(1249, 890);
            this.tpWebSettings.TabIndex = 5;
            this.tpWebSettings.Text = " Web Settings ";
            this.tpWebSettings.UseVisualStyleBackColor = true;
            // 
            // btnSaveWebSettings
            // 
            this.btnSaveWebSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveWebSettings.Location = new System.Drawing.Point(87, 6);
            this.btnSaveWebSettings.Name = "btnSaveWebSettings";
            this.btnSaveWebSettings.Size = new System.Drawing.Size(75, 25);
            this.btnSaveWebSettings.TabIndex = 6;
            this.btnSaveWebSettings.Text = "Save";
            this.btnSaveWebSettings.UseVisualStyleBackColor = true;
            this.btnSaveWebSettings.Click += new System.EventHandler(this.btnSaveWebSettings_Click);
            // 
            // btnLoadWebSettings
            // 
            this.btnLoadWebSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadWebSettings.Location = new System.Drawing.Point(6, 6);
            this.btnLoadWebSettings.Name = "btnLoadWebSettings";
            this.btnLoadWebSettings.Size = new System.Drawing.Size(75, 25);
            this.btnLoadWebSettings.TabIndex = 5;
            this.btnLoadWebSettings.Text = "Load";
            this.btnLoadWebSettings.UseVisualStyleBackColor = true;
            this.btnLoadWebSettings.Click += new System.EventHandler(this.btnLoadWebSettings_Click);
            // 
            // dgvWebSettings
            // 
            this.dgvWebSettings.AllowUserToAddRows = false;
            this.dgvWebSettings.AllowUserToDeleteRows = false;
            this.dgvWebSettings.AllowUserToResizeColumns = false;
            this.dgvWebSettings.AllowUserToResizeRows = false;
            this.dgvWebSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvWebSettings.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvWebSettings.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvWebSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWebSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvtbcWebSettingsName,
            this.dgvtbcWebSettingsValue});
            this.dgvWebSettings.EnableHeadersVisualStyles = false;
            this.dgvWebSettings.Location = new System.Drawing.Point(6, 37);
            this.dgvWebSettings.Name = "dgvWebSettings";
            this.dgvWebSettings.RowHeadersVisible = false;
            this.dgvWebSettings.Size = new System.Drawing.Size(1239, 827);
            this.dgvWebSettings.TabIndex = 1;
            this.dgvWebSettings.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvWebSettings_CellValueChanged);
            this.dgvWebSettings.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvWebSettings_DataError);
            // 
            // dgvtbcWebSettingsName
            // 
            this.dgvtbcWebSettingsName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcWebSettingsName.HeaderText = "Name";
            this.dgvtbcWebSettingsName.Name = "dgvtbcWebSettingsName";
            this.dgvtbcWebSettingsName.ReadOnly = true;
            this.dgvtbcWebSettingsName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcWebSettingsValue
            // 
            this.dgvtbcWebSettingsValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcWebSettingsValue.FillWeight = 50F;
            this.dgvtbcWebSettingsValue.HeaderText = "Value";
            this.dgvtbcWebSettingsValue.Name = "dgvtbcWebSettingsValue";
            this.dgvtbcWebSettingsValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tpCredentialCache
            // 
            this.tpCredentialCache.Controls.Add(this.btnSaveCredentialCache);
            this.tpCredentialCache.Controls.Add(this.btnLoadCredentialCache);
            this.tpCredentialCache.Controls.Add(this.dgvCredentialCache);
            this.tpCredentialCache.Location = new System.Drawing.Point(4, 23);
            this.tpCredentialCache.Name = "tpCredentialCache";
            this.tpCredentialCache.Size = new System.Drawing.Size(1249, 890);
            this.tpCredentialCache.TabIndex = 9;
            this.tpCredentialCache.Text = " Credential Cache ";
            this.tpCredentialCache.UseVisualStyleBackColor = true;
            // 
            // btnSaveCredentialCache
            // 
            this.btnSaveCredentialCache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCredentialCache.Location = new System.Drawing.Point(87, 6);
            this.btnSaveCredentialCache.Name = "btnSaveCredentialCache";
            this.btnSaveCredentialCache.Size = new System.Drawing.Size(75, 25);
            this.btnSaveCredentialCache.TabIndex = 7;
            this.btnSaveCredentialCache.Text = "Save";
            this.btnSaveCredentialCache.UseVisualStyleBackColor = true;
            this.btnSaveCredentialCache.Click += new System.EventHandler(this.btnSaveCredentialCache_Click);
            // 
            // btnLoadCredentialCache
            // 
            this.btnLoadCredentialCache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadCredentialCache.Location = new System.Drawing.Point(6, 6);
            this.btnLoadCredentialCache.Name = "btnLoadCredentialCache";
            this.btnLoadCredentialCache.Size = new System.Drawing.Size(75, 25);
            this.btnLoadCredentialCache.TabIndex = 6;
            this.btnLoadCredentialCache.Text = "Load";
            this.btnLoadCredentialCache.UseVisualStyleBackColor = true;
            this.btnLoadCredentialCache.Click += new System.EventHandler(this.btnLoadCredentialCache_Click);
            // 
            // dgvCredentialCache
            // 
            this.dgvCredentialCache.AllowUserToResizeColumns = false;
            this.dgvCredentialCache.AllowUserToResizeRows = false;
            this.dgvCredentialCache.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCredentialCache.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvCredentialCache.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCredentialCache.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCredentialCache.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvcbcSchemeCredentialCache,
            this.dgvtbcDomainCredentialCache,
            this.dgvtbcUserName,
            this.dgvtbcPassword,
            this.dgvcbcIsEnabledCredentialCache});
            this.dgvCredentialCache.EnableHeadersVisualStyles = false;
            this.dgvCredentialCache.Location = new System.Drawing.Point(6, 37);
            this.dgvCredentialCache.Name = "dgvCredentialCache";
            this.dgvCredentialCache.Size = new System.Drawing.Size(1239, 827);
            this.dgvCredentialCache.TabIndex = 5;
            // 
            // dgvcbcSchemeCredentialCache
            // 
            this.dgvcbcSchemeCredentialCache.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvcbcSchemeCredentialCache.FillWeight = 25F;
            this.dgvcbcSchemeCredentialCache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dgvcbcSchemeCredentialCache.HeaderText = "Scheme";
            this.dgvcbcSchemeCredentialCache.Items.AddRange(new object[] {
            "http://",
            "https://"});
            this.dgvcbcSchemeCredentialCache.Name = "dgvcbcSchemeCredentialCache";
            this.dgvcbcSchemeCredentialCache.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dgvtbcDomainCredentialCache
            // 
            this.dgvtbcDomainCredentialCache.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcDomainCredentialCache.FillWeight = 50F;
            this.dgvtbcDomainCredentialCache.HeaderText = "Domain";
            this.dgvtbcDomainCredentialCache.Name = "dgvtbcDomainCredentialCache";
            this.dgvtbcDomainCredentialCache.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcUserName
            // 
            this.dgvtbcUserName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcUserName.HeaderText = "User Name";
            this.dgvtbcUserName.Name = "dgvtbcUserName";
            this.dgvtbcUserName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcPassword
            // 
            this.dgvtbcPassword.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcPassword.HeaderText = "Password";
            this.dgvtbcPassword.Name = "dgvtbcPassword";
            // 
            // dgvcbcIsEnabledCredentialCache
            // 
            this.dgvcbcIsEnabledCredentialCache.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvcbcIsEnabledCredentialCache.FillWeight = 25F;
            this.dgvcbcIsEnabledCredentialCache.HeaderText = "Is Enabled";
            this.dgvcbcIsEnabledCredentialCache.Name = "dgvcbcIsEnabledCredentialCache";
            // 
            // tpCookieContainer
            // 
            this.tpCookieContainer.Controls.Add(this.btnSaveCookieContainer);
            this.tpCookieContainer.Controls.Add(this.btnLoadCookieContainer);
            this.tpCookieContainer.Controls.Add(this.dgvCookieContainer);
            this.tpCookieContainer.Location = new System.Drawing.Point(4, 23);
            this.tpCookieContainer.Name = "tpCookieContainer";
            this.tpCookieContainer.Size = new System.Drawing.Size(1249, 890);
            this.tpCookieContainer.TabIndex = 8;
            this.tpCookieContainer.Text = " Cookie Container ";
            this.tpCookieContainer.UseVisualStyleBackColor = true;
            // 
            // btnSaveCookieContainer
            // 
            this.btnSaveCookieContainer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCookieContainer.Location = new System.Drawing.Point(87, 6);
            this.btnSaveCookieContainer.Name = "btnSaveCookieContainer";
            this.btnSaveCookieContainer.Size = new System.Drawing.Size(75, 25);
            this.btnSaveCookieContainer.TabIndex = 10;
            this.btnSaveCookieContainer.Text = "Save";
            this.btnSaveCookieContainer.UseVisualStyleBackColor = true;
            this.btnSaveCookieContainer.Click += new System.EventHandler(this.btnSaveCookieContainer_Click);
            // 
            // btnLoadCookieContainer
            // 
            this.btnLoadCookieContainer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadCookieContainer.Location = new System.Drawing.Point(6, 6);
            this.btnLoadCookieContainer.Name = "btnLoadCookieContainer";
            this.btnLoadCookieContainer.Size = new System.Drawing.Size(75, 25);
            this.btnLoadCookieContainer.TabIndex = 9;
            this.btnLoadCookieContainer.Text = "Load";
            this.btnLoadCookieContainer.UseVisualStyleBackColor = true;
            this.btnLoadCookieContainer.Click += new System.EventHandler(this.btnLoadCookieContainer_Click);
            // 
            // dgvCookieContainer
            // 
            this.dgvCookieContainer.AllowUserToResizeColumns = false;
            this.dgvCookieContainer.AllowUserToResizeRows = false;
            this.dgvCookieContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCookieContainer.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvCookieContainer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCookieContainer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCookieContainer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvcbcSchemeCookieContainer,
            this.dgvtbcDomainCookieContainer,
            this.dgctbcValue,
            this.dgvcbcIsEnabledCookieContainer});
            this.dgvCookieContainer.EnableHeadersVisualStyles = false;
            this.dgvCookieContainer.Location = new System.Drawing.Point(6, 37);
            this.dgvCookieContainer.Name = "dgvCookieContainer";
            this.dgvCookieContainer.Size = new System.Drawing.Size(1239, 827);
            this.dgvCookieContainer.TabIndex = 8;
            // 
            // dgvcbcSchemeCookieContainer
            // 
            this.dgvcbcSchemeCookieContainer.FillWeight = 25F;
            this.dgvcbcSchemeCookieContainer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dgvcbcSchemeCookieContainer.HeaderText = "Scheme";
            this.dgvcbcSchemeCookieContainer.Items.AddRange(new object[] {
            "http://",
            "https://"});
            this.dgvcbcSchemeCookieContainer.Name = "dgvcbcSchemeCookieContainer";
            this.dgvcbcSchemeCookieContainer.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dgvtbcDomainCookieContainer
            // 
            this.dgvtbcDomainCookieContainer.FillWeight = 50F;
            this.dgvtbcDomainCookieContainer.HeaderText = "Domain";
            this.dgvtbcDomainCookieContainer.Name = "dgvtbcDomainCookieContainer";
            // 
            // dgctbcValue
            // 
            this.dgctbcValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgctbcValue.HeaderText = "Value";
            this.dgctbcValue.Name = "dgctbcValue";
            this.dgctbcValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvcbcIsEnabledCookieContainer
            // 
            this.dgvcbcIsEnabledCookieContainer.FillWeight = 25F;
            this.dgvcbcIsEnabledCookieContainer.HeaderText = "Is Enabled";
            this.dgvcbcIsEnabledCookieContainer.Name = "dgvcbcIsEnabledCookieContainer";
            // 
            // tpProxyServers
            // 
            this.tpProxyServers.Controls.Add(this.btnVerify);
            this.tpProxyServers.Controls.Add(this.btnSaveProxyServers);
            this.tpProxyServers.Controls.Add(this.btnLoadProxyServers);
            this.tpProxyServers.Controls.Add(this.dgvProxyServers);
            this.tpProxyServers.Location = new System.Drawing.Point(4, 23);
            this.tpProxyServers.Name = "tpProxyServers";
            this.tpProxyServers.Size = new System.Drawing.Size(1249, 890);
            this.tpProxyServers.TabIndex = 7;
            this.tpProxyServers.Text = " Proxy Servers ";
            this.tpProxyServers.UseVisualStyleBackColor = true;
            // 
            // btnVerify
            // 
            this.btnVerify.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerify.Location = new System.Drawing.Point(1170, 6);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(75, 25);
            this.btnVerify.TabIndex = 14;
            this.btnVerify.Text = "Verify";
            this.btnVerify.UseVisualStyleBackColor = true;
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // btnSaveProxyServers
            // 
            this.btnSaveProxyServers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveProxyServers.Location = new System.Drawing.Point(87, 6);
            this.btnSaveProxyServers.Name = "btnSaveProxyServers";
            this.btnSaveProxyServers.Size = new System.Drawing.Size(75, 25);
            this.btnSaveProxyServers.TabIndex = 13;
            this.btnSaveProxyServers.Text = "Save";
            this.btnSaveProxyServers.UseVisualStyleBackColor = true;
            this.btnSaveProxyServers.Click += new System.EventHandler(this.btnSaveProxyServers_Click);
            // 
            // btnLoadProxyServers
            // 
            this.btnLoadProxyServers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadProxyServers.Location = new System.Drawing.Point(6, 6);
            this.btnLoadProxyServers.Name = "btnLoadProxyServers";
            this.btnLoadProxyServers.Size = new System.Drawing.Size(75, 25);
            this.btnLoadProxyServers.TabIndex = 12;
            this.btnLoadProxyServers.Text = "Load";
            this.btnLoadProxyServers.UseVisualStyleBackColor = true;
            this.btnLoadProxyServers.Click += new System.EventHandler(this.btnLoadProxyServers_Click);
            // 
            // dgvProxyServers
            // 
            this.dgvProxyServers.AllowUserToResizeColumns = false;
            this.dgvProxyServers.AllowUserToResizeRows = false;
            this.dgvProxyServers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProxyServers.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvProxyServers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvProxyServers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProxyServers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvcbcSchemeProxyServers,
            this.dgvtbcIPAddress,
            this.dgvtbcPort,
            this.dgvtbcAbsoluteUriToVerify,
            this.dgvtbcValueToVerify,
            this.dgvtbcTimeoutInMilliseconds,
            this.dgvtbcStatusCodeProxyServers,
            this.dgvcbcIsEnabledProxyServers});
            this.dgvProxyServers.EnableHeadersVisualStyles = false;
            this.dgvProxyServers.Location = new System.Drawing.Point(6, 37);
            this.dgvProxyServers.Name = "dgvProxyServers";
            this.dgvProxyServers.Size = new System.Drawing.Size(1239, 827);
            this.dgvProxyServers.TabIndex = 11;
            // 
            // dgvcbcSchemeProxyServers
            // 
            this.dgvcbcSchemeProxyServers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvcbcSchemeProxyServers.FillWeight = 25F;
            this.dgvcbcSchemeProxyServers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dgvcbcSchemeProxyServers.HeaderText = "Scheme";
            this.dgvcbcSchemeProxyServers.Items.AddRange(new object[] {
            "http://",
            "https://"});
            this.dgvcbcSchemeProxyServers.Name = "dgvcbcSchemeProxyServers";
            // 
            // dgvtbcIPAddress
            // 
            this.dgvtbcIPAddress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcIPAddress.HeaderText = "IP Address";
            this.dgvtbcIPAddress.Name = "dgvtbcIPAddress";
            this.dgvtbcIPAddress.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvtbcIPAddress.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcPort
            // 
            this.dgvtbcPort.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcPort.FillWeight = 50F;
            this.dgvtbcPort.HeaderText = "Port";
            this.dgvtbcPort.Name = "dgvtbcPort";
            // 
            // dgvtbcAbsoluteUriToVerify
            // 
            this.dgvtbcAbsoluteUriToVerify.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcAbsoluteUriToVerify.FillWeight = 50F;
            this.dgvtbcAbsoluteUriToVerify.HeaderText = "AbsoluteUri To Verify";
            this.dgvtbcAbsoluteUriToVerify.Name = "dgvtbcAbsoluteUriToVerify";
            // 
            // dgvtbcValueToVerify
            // 
            this.dgvtbcValueToVerify.HeaderText = "Value To Verify";
            this.dgvtbcValueToVerify.Name = "dgvtbcValueToVerify";
            // 
            // dgvtbcTimeoutInMilliseconds
            // 
            this.dgvtbcTimeoutInMilliseconds.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcTimeoutInMilliseconds.FillWeight = 25F;
            this.dgvtbcTimeoutInMilliseconds.HeaderText = "Timeout In Milliseconds";
            this.dgvtbcTimeoutInMilliseconds.Name = "dgvtbcTimeoutInMilliseconds";
            // 
            // dgvtbcStatusCodeProxyServers
            // 
            this.dgvtbcStatusCodeProxyServers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcStatusCodeProxyServers.FillWeight = 25F;
            this.dgvtbcStatusCodeProxyServers.HeaderText = "Status Code";
            this.dgvtbcStatusCodeProxyServers.Name = "dgvtbcStatusCodeProxyServers";
            this.dgvtbcStatusCodeProxyServers.ReadOnly = true;
            // 
            // dgvcbcIsEnabledProxyServers
            // 
            this.dgvcbcIsEnabledProxyServers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvcbcIsEnabledProxyServers.FillWeight = 25F;
            this.dgvcbcIsEnabledProxyServers.HeaderText = "Is Enabled";
            this.dgvcbcIsEnabledProxyServers.Name = "dgvcbcIsEnabledProxyServers";
            // 
            // tpCrawlRequests
            // 
            this.tpCrawlRequests.Controls.Add(this.btnSaveCrawlRequests);
            this.tpCrawlRequests.Controls.Add(this.btnLoadCrawlRequests);
            this.tpCrawlRequests.Controls.Add(this.dgvCrawlRequests);
            this.tpCrawlRequests.Location = new System.Drawing.Point(4, 23);
            this.tpCrawlRequests.Name = "tpCrawlRequests";
            this.tpCrawlRequests.Padding = new System.Windows.Forms.Padding(3);
            this.tpCrawlRequests.Size = new System.Drawing.Size(1249, 890);
            this.tpCrawlRequests.TabIndex = 3;
            this.tpCrawlRequests.Text = " Crawl Requests ";
            this.tpCrawlRequests.UseVisualStyleBackColor = true;
            // 
            // btnSaveCrawlRequests
            // 
            this.btnSaveCrawlRequests.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCrawlRequests.Location = new System.Drawing.Point(87, 6);
            this.btnSaveCrawlRequests.Name = "btnSaveCrawlRequests";
            this.btnSaveCrawlRequests.Size = new System.Drawing.Size(75, 25);
            this.btnSaveCrawlRequests.TabIndex = 2;
            this.btnSaveCrawlRequests.Text = "Save";
            this.btnSaveCrawlRequests.UseVisualStyleBackColor = true;
            this.btnSaveCrawlRequests.Click += new System.EventHandler(this.btnSaveCrawlRequests_Click);
            // 
            // btnLoadCrawlRequests
            // 
            this.btnLoadCrawlRequests.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadCrawlRequests.Location = new System.Drawing.Point(6, 6);
            this.btnLoadCrawlRequests.Name = "btnLoadCrawlRequests";
            this.btnLoadCrawlRequests.Size = new System.Drawing.Size(75, 25);
            this.btnLoadCrawlRequests.TabIndex = 1;
            this.btnLoadCrawlRequests.Text = "Load";
            this.btnLoadCrawlRequests.UseVisualStyleBackColor = true;
            this.btnLoadCrawlRequests.Click += new System.EventHandler(this.btnLoadCrawlRequests_Click);
            // 
            // dgvCrawlRequests
            // 
            this.dgvCrawlRequests.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCrawlRequests.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvCrawlRequests.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCrawlRequests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCrawlRequests.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvtbcCRAbsoluteUri,
            this.dgvtbcCRDepth,
            this.dgvtbcRestrictCrawlTo,
            this.dgvtbcRestrictDiscoveriesTo,
            this.dgvtbcCRPriority,
            this.dgvtbcRenderType,
            this.dgvtbcRenderTypeForChildren,
            this.dgvtbcCRAdded});
            this.dgvCrawlRequests.EnableHeadersVisualStyles = false;
            this.dgvCrawlRequests.Location = new System.Drawing.Point(6, 37);
            this.dgvCrawlRequests.Name = "dgvCrawlRequests";
            this.dgvCrawlRequests.Size = new System.Drawing.Size(1239, 827);
            this.dgvCrawlRequests.TabIndex = 0;
            // 
            // dgvtbcCRAbsoluteUri
            // 
            this.dgvtbcCRAbsoluteUri.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcCRAbsoluteUri.HeaderText = "AbsoluteUri";
            this.dgvtbcCRAbsoluteUri.Name = "dgvtbcCRAbsoluteUri";
            // 
            // dgvtbcCRDepth
            // 
            this.dgvtbcCRDepth.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcCRDepth.FillWeight = 20F;
            this.dgvtbcCRDepth.HeaderText = "Depth";
            this.dgvtbcCRDepth.Name = "dgvtbcCRDepth";
            // 
            // dgvtbcRestrictCrawlTo
            // 
            this.dgvtbcRestrictCrawlTo.HeaderText = "Restrict Crawl To";
            this.dgvtbcRestrictCrawlTo.Name = "dgvtbcRestrictCrawlTo";
            // 
            // dgvtbcRestrictDiscoveriesTo
            // 
            this.dgvtbcRestrictDiscoveriesTo.HeaderText = "Restrict Discoveries To";
            this.dgvtbcRestrictDiscoveriesTo.Name = "dgvtbcRestrictDiscoveriesTo";
            // 
            // dgvtbcCRPriority
            // 
            this.dgvtbcCRPriority.FillWeight = 20F;
            this.dgvtbcCRPriority.HeaderText = "Priority";
            this.dgvtbcCRPriority.Name = "dgvtbcCRPriority";
            // 
            // dgvtbcRenderType
            // 
            this.dgvtbcRenderType.HeaderText = "Render Type";
            this.dgvtbcRenderType.Name = "dgvtbcRenderType";
            // 
            // dgvtbcRenderTypeForChildren
            // 
            this.dgvtbcRenderTypeForChildren.HeaderText = "Render Type For Children";
            this.dgvtbcRenderTypeForChildren.Name = "dgvtbcRenderTypeForChildren";
            // 
            // dgvtbcCRAdded
            // 
            this.dgvtbcCRAdded.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcCRAdded.FillWeight = 20F;
            this.dgvtbcCRAdded.HeaderText = "Added";
            this.dgvtbcCRAdded.Name = "dgvtbcCRAdded";
            // 
            // tpCrawler
            // 
            this.tpCrawler.Controls.Add(this.btnSaveCrawler);
            this.tpCrawler.Controls.Add(this.btnLoadCrawler);
            this.tpCrawler.Controls.Add(this.scMain);
            this.tpCrawler.Controls.Add(this.btnResume);
            this.tpCrawler.Controls.Add(this.btnPause);
            this.tpCrawler.Controls.Add(this.btnStop);
            this.tpCrawler.Controls.Add(this.btnStart);
            this.tpCrawler.Location = new System.Drawing.Point(4, 23);
            this.tpCrawler.Name = "tpCrawler";
            this.tpCrawler.Padding = new System.Windows.Forms.Padding(3);
            this.tpCrawler.Size = new System.Drawing.Size(1249, 890);
            this.tpCrawler.TabIndex = 1;
            this.tpCrawler.Text = " Crawler ";
            this.tpCrawler.UseVisualStyleBackColor = true;
            // 
            // btnSaveCrawler
            // 
            this.btnSaveCrawler.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCrawler.Location = new System.Drawing.Point(87, 6);
            this.btnSaveCrawler.Name = "btnSaveCrawler";
            this.btnSaveCrawler.Size = new System.Drawing.Size(75, 25);
            this.btnSaveCrawler.TabIndex = 8;
            this.btnSaveCrawler.Text = "Save";
            this.btnSaveCrawler.UseVisualStyleBackColor = true;
            this.btnSaveCrawler.Click += new System.EventHandler(this.btnSaveCrawler_Click);
            // 
            // btnLoadCrawler
            // 
            this.btnLoadCrawler.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadCrawler.Location = new System.Drawing.Point(6, 6);
            this.btnLoadCrawler.Name = "btnLoadCrawler";
            this.btnLoadCrawler.Size = new System.Drawing.Size(75, 25);
            this.btnLoadCrawler.TabIndex = 7;
            this.btnLoadCrawler.Text = "Load";
            this.btnLoadCrawler.UseVisualStyleBackColor = true;
            this.btnLoadCrawler.Click += new System.EventHandler(this.btnLoadCrawler_Click);
            // 
            // scMain
            // 
            this.scMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scMain.Location = new System.Drawing.Point(6, 37);
            this.scMain.Name = "scMain";
            this.scMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.dgvCrawler);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.rtbOutput);
            this.scMain.Size = new System.Drawing.Size(1239, 825);
            this.scMain.SplitterDistance = 412;
            this.scMain.TabIndex = 6;
            // 
            // dgvCrawler
            // 
            this.dgvCrawler.AllowUserToAddRows = false;
            this.dgvCrawler.AllowUserToDeleteRows = false;
            this.dgvCrawler.AllowUserToOrderColumns = true;
            this.dgvCrawler.AllowUserToResizeColumns = false;
            this.dgvCrawler.AllowUserToResizeRows = false;
            this.dgvCrawler.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvCrawler.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCrawler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCrawler.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvtbcThreadNumberCrawler,
            this.dgvtbcAbsoluteUri,
            this.dgvtbcStatusCode,
            this.dgvtbcDiscoveryType,
            this.dgvtbcHyperLinkDiscoveries,
            this.dgvtbcFileAndImageDiscoveries,
            this.dgvtbcCurrentDepth,
            this.dgvtbcMaximumDepth});
            this.dgvCrawler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCrawler.EnableHeadersVisualStyles = false;
            this.dgvCrawler.Location = new System.Drawing.Point(0, 0);
            this.dgvCrawler.Name = "dgvCrawler";
            this.dgvCrawler.ReadOnly = true;
            this.dgvCrawler.RowHeadersVisible = false;
            this.dgvCrawler.Size = new System.Drawing.Size(1239, 412);
            this.dgvCrawler.TabIndex = 0;
            // 
            // dgvtbcThreadNumberCrawler
            // 
            this.dgvtbcThreadNumberCrawler.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcThreadNumberCrawler.FillWeight = 20F;
            this.dgvtbcThreadNumberCrawler.HeaderText = "Thread Number";
            this.dgvtbcThreadNumberCrawler.Name = "dgvtbcThreadNumberCrawler";
            this.dgvtbcThreadNumberCrawler.ReadOnly = true;
            this.dgvtbcThreadNumberCrawler.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcAbsoluteUri
            // 
            this.dgvtbcAbsoluteUri.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcAbsoluteUri.HeaderText = "Absolute Uri";
            this.dgvtbcAbsoluteUri.Name = "dgvtbcAbsoluteUri";
            this.dgvtbcAbsoluteUri.ReadOnly = true;
            this.dgvtbcAbsoluteUri.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcStatusCode
            // 
            this.dgvtbcStatusCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcStatusCode.FillWeight = 20F;
            this.dgvtbcStatusCode.HeaderText = "Status Code";
            this.dgvtbcStatusCode.Name = "dgvtbcStatusCode";
            this.dgvtbcStatusCode.ReadOnly = true;
            this.dgvtbcStatusCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcDiscoveryType
            // 
            this.dgvtbcDiscoveryType.FillWeight = 20F;
            this.dgvtbcDiscoveryType.HeaderText = "Discovery Type";
            this.dgvtbcDiscoveryType.Name = "dgvtbcDiscoveryType";
            this.dgvtbcDiscoveryType.ReadOnly = true;
            this.dgvtbcDiscoveryType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcHyperLinkDiscoveries
            // 
            this.dgvtbcHyperLinkDiscoveries.FillWeight = 20F;
            this.dgvtbcHyperLinkDiscoveries.HeaderText = "HyperLink Discoveries";
            this.dgvtbcHyperLinkDiscoveries.Name = "dgvtbcHyperLinkDiscoveries";
            this.dgvtbcHyperLinkDiscoveries.ReadOnly = true;
            this.dgvtbcHyperLinkDiscoveries.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcFileAndImageDiscoveries
            // 
            this.dgvtbcFileAndImageDiscoveries.FillWeight = 20F;
            this.dgvtbcFileAndImageDiscoveries.HeaderText = "File/Image Discoveries";
            this.dgvtbcFileAndImageDiscoveries.Name = "dgvtbcFileAndImageDiscoveries";
            this.dgvtbcFileAndImageDiscoveries.ReadOnly = true;
            this.dgvtbcFileAndImageDiscoveries.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcCurrentDepth
            // 
            this.dgvtbcCurrentDepth.FillWeight = 10F;
            this.dgvtbcCurrentDepth.HeaderText = "Current Depth";
            this.dgvtbcCurrentDepth.Name = "dgvtbcCurrentDepth";
            this.dgvtbcCurrentDepth.ReadOnly = true;
            this.dgvtbcCurrentDepth.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcMaximumDepth
            // 
            this.dgvtbcMaximumDepth.FillWeight = 10F;
            this.dgvtbcMaximumDepth.HeaderText = "Maximum Depth";
            this.dgvtbcMaximumDepth.Name = "dgvtbcMaximumDepth";
            this.dgvtbcMaximumDepth.ReadOnly = true;
            this.dgvtbcMaximumDepth.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // rtbOutput
            // 
            this.rtbOutput.ContextMenuStrip = this.cmsMain;
            this.rtbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbOutput.Location = new System.Drawing.Point(0, 0);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ShowSelectionMargin = true;
            this.rtbOutput.Size = new System.Drawing.Size(1239, 409);
            this.rtbOutput.TabIndex = 0;
            this.rtbOutput.Text = "";
            this.rtbOutput.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbOutput_LinkClicked);
            this.rtbOutput.SelectionChanged += new System.EventHandler(this.rtbOutput_SelectionChanged);
            // 
            // cmsMain
            // 
            this.cmsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiUndo,
            this.tss1,
            this.tsmiCut,
            this.tsmiCopy,
            this.tsmiPaste,
            this.tsmiDelete,
            this.toolStripSeparator2,
            this.tsmiSelectAll});
            this.cmsMain.Name = "cmsMain";
            this.cmsMain.Size = new System.Drawing.Size(123, 148);
            // 
            // tsmiUndo
            // 
            this.tsmiUndo.Name = "tsmiUndo";
            this.tsmiUndo.Size = new System.Drawing.Size(122, 22);
            this.tsmiUndo.Text = "Undo";
            this.tsmiUndo.Click += new System.EventHandler(this.tsmiUndo_Click);
            // 
            // tss1
            // 
            this.tss1.Name = "tss1";
            this.tss1.Size = new System.Drawing.Size(119, 6);
            // 
            // tsmiCut
            // 
            this.tsmiCut.Name = "tsmiCut";
            this.tsmiCut.Size = new System.Drawing.Size(122, 22);
            this.tsmiCut.Text = "Cut";
            this.tsmiCut.Click += new System.EventHandler(this.tsmiCut_Click);
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.Size = new System.Drawing.Size(122, 22);
            this.tsmiCopy.Text = "Copy";
            this.tsmiCopy.Click += new System.EventHandler(this.tsmiCopy_Click);
            // 
            // tsmiPaste
            // 
            this.tsmiPaste.Name = "tsmiPaste";
            this.tsmiPaste.Size = new System.Drawing.Size(122, 22);
            this.tsmiPaste.Text = "Paste";
            this.tsmiPaste.Click += new System.EventHandler(this.tsmiPaste_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(122, 22);
            this.tsmiDelete.Text = "Delete";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(119, 6);
            // 
            // tsmiSelectAll
            // 
            this.tsmiSelectAll.Name = "tsmiSelectAll";
            this.tsmiSelectAll.Size = new System.Drawing.Size(122, 22);
            this.tsmiSelectAll.Text = "Select All";
            this.tsmiSelectAll.Click += new System.EventHandler(this.tsmiSelectAll_Click);
            // 
            // btnResume
            // 
            this.btnResume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResume.Enabled = false;
            this.btnResume.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResume.Location = new System.Drawing.Point(1170, 6);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(75, 25);
            this.btnResume.TabIndex = 5;
            this.btnResume.Text = "Resume";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // btnPause
            // 
            this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPause.Enabled = false;
            this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPause.Location = new System.Drawing.Point(1089, 6);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 25);
            this.btnPause.TabIndex = 4;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Enabled = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Location = new System.Drawing.Point(1008, 6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 25);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Location = new System.Drawing.Point(927, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 25);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // tpCrawlInfo
            // 
            this.tpCrawlInfo.Controls.Add(this.dgvCrawlInfo);
            this.tpCrawlInfo.Controls.Add(this.btnSaveCrawlInfo);
            this.tpCrawlInfo.Controls.Add(this.btnLoadCrawlInfo);
            this.tpCrawlInfo.Location = new System.Drawing.Point(4, 23);
            this.tpCrawlInfo.Name = "tpCrawlInfo";
            this.tpCrawlInfo.Size = new System.Drawing.Size(1249, 890);
            this.tpCrawlInfo.TabIndex = 6;
            this.tpCrawlInfo.Text = " Crawl Info ";
            this.tpCrawlInfo.UseVisualStyleBackColor = true;
            // 
            // dgvCrawlInfo
            // 
            this.dgvCrawlInfo.AllowUserToAddRows = false;
            this.dgvCrawlInfo.AllowUserToDeleteRows = false;
            this.dgvCrawlInfo.AllowUserToOrderColumns = true;
            this.dgvCrawlInfo.AllowUserToResizeColumns = false;
            this.dgvCrawlInfo.AllowUserToResizeRows = false;
            this.dgvCrawlInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCrawlInfo.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvCrawlInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCrawlInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCrawlInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvtbcThreadNumberCrawlInfo,
            this.dgvtbcParentDiscovery,
            this.dgvtbcCurrentDiscovery,
            this.dgvtbcCrawlState,
            this.dgvtbcTotalCrawlFedCount,
            this.dgvtbcTotalCrawlRequestsAssigned,
            this.dgvtbcTotalCrawlStarvedCount,
            this.dgvtbcTotalHttpWebResponseTime});
            this.dgvCrawlInfo.EnableHeadersVisualStyles = false;
            this.dgvCrawlInfo.Location = new System.Drawing.Point(6, 37);
            this.dgvCrawlInfo.Name = "dgvCrawlInfo";
            this.dgvCrawlInfo.ReadOnly = true;
            this.dgvCrawlInfo.RowHeadersVisible = false;
            this.dgvCrawlInfo.Size = new System.Drawing.Size(1239, 827);
            this.dgvCrawlInfo.TabIndex = 6;
            // 
            // dgvtbcThreadNumberCrawlInfo
            // 
            this.dgvtbcThreadNumberCrawlInfo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcThreadNumberCrawlInfo.FillWeight = 20F;
            this.dgvtbcThreadNumberCrawlInfo.HeaderText = "Thread Number";
            this.dgvtbcThreadNumberCrawlInfo.Name = "dgvtbcThreadNumberCrawlInfo";
            this.dgvtbcThreadNumberCrawlInfo.ReadOnly = true;
            this.dgvtbcThreadNumberCrawlInfo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcParentDiscovery
            // 
            this.dgvtbcParentDiscovery.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcParentDiscovery.HeaderText = "Parent Discovery";
            this.dgvtbcParentDiscovery.Name = "dgvtbcParentDiscovery";
            this.dgvtbcParentDiscovery.ReadOnly = true;
            // 
            // dgvtbcCurrentDiscovery
            // 
            this.dgvtbcCurrentDiscovery.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcCurrentDiscovery.HeaderText = "Current Discovery";
            this.dgvtbcCurrentDiscovery.Name = "dgvtbcCurrentDiscovery";
            this.dgvtbcCurrentDiscovery.ReadOnly = true;
            this.dgvtbcCurrentDiscovery.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcCrawlState
            // 
            this.dgvtbcCrawlState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvtbcCrawlState.FillWeight = 20F;
            this.dgvtbcCrawlState.HeaderText = "Crawl State";
            this.dgvtbcCrawlState.Name = "dgvtbcCrawlState";
            this.dgvtbcCrawlState.ReadOnly = true;
            this.dgvtbcCrawlState.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcTotalCrawlFedCount
            // 
            this.dgvtbcTotalCrawlFedCount.FillWeight = 20F;
            this.dgvtbcTotalCrawlFedCount.HeaderText = "Total Crawl Fed Count";
            this.dgvtbcTotalCrawlFedCount.Name = "dgvtbcTotalCrawlFedCount";
            this.dgvtbcTotalCrawlFedCount.ReadOnly = true;
            this.dgvtbcTotalCrawlFedCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcTotalCrawlRequestsAssigned
            // 
            this.dgvtbcTotalCrawlRequestsAssigned.FillWeight = 20F;
            this.dgvtbcTotalCrawlRequestsAssigned.HeaderText = "Total Crawl Requests Assigned";
            this.dgvtbcTotalCrawlRequestsAssigned.Name = "dgvtbcTotalCrawlRequestsAssigned";
            this.dgvtbcTotalCrawlRequestsAssigned.ReadOnly = true;
            this.dgvtbcTotalCrawlRequestsAssigned.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcTotalCrawlStarvedCount
            // 
            this.dgvtbcTotalCrawlStarvedCount.FillWeight = 10F;
            this.dgvtbcTotalCrawlStarvedCount.HeaderText = "Total Crawl Starved Count";
            this.dgvtbcTotalCrawlStarvedCount.Name = "dgvtbcTotalCrawlStarvedCount";
            this.dgvtbcTotalCrawlStarvedCount.ReadOnly = true;
            this.dgvtbcTotalCrawlStarvedCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvtbcTotalHttpWebResponseTime
            // 
            this.dgvtbcTotalHttpWebResponseTime.FillWeight = 10F;
            this.dgvtbcTotalHttpWebResponseTime.HeaderText = "Total HttpWebResponse Time";
            this.dgvtbcTotalHttpWebResponseTime.Name = "dgvtbcTotalHttpWebResponseTime";
            this.dgvtbcTotalHttpWebResponseTime.ReadOnly = true;
            this.dgvtbcTotalHttpWebResponseTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnSaveCrawlInfo
            // 
            this.btnSaveCrawlInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCrawlInfo.Location = new System.Drawing.Point(87, 6);
            this.btnSaveCrawlInfo.Name = "btnSaveCrawlInfo";
            this.btnSaveCrawlInfo.Size = new System.Drawing.Size(75, 25);
            this.btnSaveCrawlInfo.TabIndex = 5;
            this.btnSaveCrawlInfo.Text = "Save";
            this.btnSaveCrawlInfo.UseVisualStyleBackColor = true;
            this.btnSaveCrawlInfo.Click += new System.EventHandler(this.btnSaveCrawlInfo_Click);
            // 
            // btnLoadCrawlInfo
            // 
            this.btnLoadCrawlInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadCrawlInfo.Location = new System.Drawing.Point(6, 6);
            this.btnLoadCrawlInfo.Name = "btnLoadCrawlInfo";
            this.btnLoadCrawlInfo.Size = new System.Drawing.Size(75, 25);
            this.btnLoadCrawlInfo.TabIndex = 4;
            this.btnLoadCrawlInfo.Text = "Load";
            this.btnLoadCrawlInfo.UseVisualStyleBackColor = true;
            this.btnLoadCrawlInfo.Click += new System.EventHandler(this.btnLoadCrawlInfo_Click);
            // 
            // tpPerformanceCounters
            // 
            this.tpPerformanceCounters.Controls.Add(this.btnSavePerformanceCounters);
            this.tpPerformanceCounters.Controls.Add(this.btnLoadPerformanceCounters);
            this.tpPerformanceCounters.Controls.Add(this.dgvPerformanceCounters);
            this.tpPerformanceCounters.Location = new System.Drawing.Point(4, 23);
            this.tpPerformanceCounters.Name = "tpPerformanceCounters";
            this.tpPerformanceCounters.Padding = new System.Windows.Forms.Padding(3);
            this.tpPerformanceCounters.Size = new System.Drawing.Size(1249, 890);
            this.tpPerformanceCounters.TabIndex = 0;
            this.tpPerformanceCounters.Text = " Performance Counters ";
            this.tpPerformanceCounters.UseVisualStyleBackColor = true;
            // 
            // btnSavePerformanceCounters
            // 
            this.btnSavePerformanceCounters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePerformanceCounters.Location = new System.Drawing.Point(87, 6);
            this.btnSavePerformanceCounters.Name = "btnSavePerformanceCounters";
            this.btnSavePerformanceCounters.Size = new System.Drawing.Size(75, 25);
            this.btnSavePerformanceCounters.TabIndex = 10;
            this.btnSavePerformanceCounters.Text = "Save";
            this.btnSavePerformanceCounters.UseVisualStyleBackColor = true;
            this.btnSavePerformanceCounters.Click += new System.EventHandler(this.btnSavePerformanceCounters_Click);
            // 
            // btnLoadPerformanceCounters
            // 
            this.btnLoadPerformanceCounters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadPerformanceCounters.Location = new System.Drawing.Point(6, 6);
            this.btnLoadPerformanceCounters.Name = "btnLoadPerformanceCounters";
            this.btnLoadPerformanceCounters.Size = new System.Drawing.Size(75, 25);
            this.btnLoadPerformanceCounters.TabIndex = 9;
            this.btnLoadPerformanceCounters.Text = "Load";
            this.btnLoadPerformanceCounters.UseVisualStyleBackColor = true;
            this.btnLoadPerformanceCounters.Click += new System.EventHandler(this.btnLoadPerformanceCounters_Click);
            // 
            // msMain
            // 
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.msMain.Location = new System.Drawing.Point(3, 3);
            this.msMain.Name = "msMain";
            this.msMain.Size = new System.Drawing.Size(1257, 24);
            this.msMain.TabIndex = 2;
            this.msMain.Text = "menuStrip1";
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
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetDatabaseToolStripMenuItem,
            this.resetCrawlerToolStripMenuItem,
            this.resetDirectoriesToolStripMenuItem,
            this.resetIISToolStripMenuItem,
            this.startPerfmonexeToolStripMenuItem,
            this.toolStripSeparator1,
            this.resetGUIToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.actionsToolStripMenuItem.Text = "Actions";
            // 
            // resetDatabaseToolStripMenuItem
            // 
            this.resetDatabaseToolStripMenuItem.Name = "resetDatabaseToolStripMenuItem";
            this.resetDatabaseToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.resetDatabaseToolStripMenuItem.Text = "Reset Database";
            this.resetDatabaseToolStripMenuItem.Click += new System.EventHandler(this.resetDatabaseToolStripMenuItem_Click);
            // 
            // resetCrawlerToolStripMenuItem
            // 
            this.resetCrawlerToolStripMenuItem.Name = "resetCrawlerToolStripMenuItem";
            this.resetCrawlerToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.resetCrawlerToolStripMenuItem.Text = "Reset Crawler";
            this.resetCrawlerToolStripMenuItem.Click += new System.EventHandler(this.resetCrawlerToolStripMenuItem_Click);
            // 
            // resetDirectoriesToolStripMenuItem
            // 
            this.resetDirectoriesToolStripMenuItem.Name = "resetDirectoriesToolStripMenuItem";
            this.resetDirectoriesToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.resetDirectoriesToolStripMenuItem.Text = "Reset Directories";
            this.resetDirectoriesToolStripMenuItem.Click += new System.EventHandler(this.resetDirectoriesToolStripMenuItem_Click);
            // 
            // resetIISToolStripMenuItem
            // 
            this.resetIISToolStripMenuItem.Name = "resetIISToolStripMenuItem";
            this.resetIISToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.resetIISToolStripMenuItem.Text = "Reset IIS";
            // 
            // startPerfmonexeToolStripMenuItem
            // 
            this.startPerfmonexeToolStripMenuItem.Name = "startPerfmonexeToolStripMenuItem";
            this.startPerfmonexeToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.startPerfmonexeToolStripMenuItem.Text = "Start perfmon.exe";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // resetGUIToolStripMenuItem
            // 
            this.resetGUIToolStripMenuItem.Name = "resetGUIToolStripMenuItem";
            this.resetGUIToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.resetGUIToolStripMenuItem.Text = "Reset GUI";
            this.resetGUIToolStripMenuItem.Click += new System.EventHandler(this.resetGUIToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.resetDatabaseOnStartToolStripMenuItem,
            this.resetCrawlerOnStartToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.breadthFirstByPriorityToolStripMenuItem,
            this.depthFirstByPriorityToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(197, 22);
            this.toolStripMenuItem1.Text = "CrawlMode";
            // 
            // breadthFirstByPriorityToolStripMenuItem
            // 
            this.breadthFirstByPriorityToolStripMenuItem.Checked = true;
            this.breadthFirstByPriorityToolStripMenuItem.CheckOnClick = true;
            this.breadthFirstByPriorityToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.breadthFirstByPriorityToolStripMenuItem.Name = "breadthFirstByPriorityToolStripMenuItem";
            this.breadthFirstByPriorityToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.breadthFirstByPriorityToolStripMenuItem.Text = "BreadthFirstByPriority";
            // 
            // depthFirstByPriorityToolStripMenuItem
            // 
            this.depthFirstByPriorityToolStripMenuItem.CheckOnClick = true;
            this.depthFirstByPriorityToolStripMenuItem.Name = "depthFirstByPriorityToolStripMenuItem";
            this.depthFirstByPriorityToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.depthFirstByPriorityToolStripMenuItem.Text = "DepthFirstByPriority";
            // 
            // resetDatabaseOnStartToolStripMenuItem
            // 
            this.resetDatabaseOnStartToolStripMenuItem.CheckOnClick = true;
            this.resetDatabaseOnStartToolStripMenuItem.Name = "resetDatabaseOnStartToolStripMenuItem";
            this.resetDatabaseOnStartToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.resetDatabaseOnStartToolStripMenuItem.Text = "Reset Database on Start";
            // 
            // resetCrawlerOnStartToolStripMenuItem
            // 
            this.resetCrawlerOnStartToolStripMenuItem.CheckOnClick = true;
            this.resetCrawlerOnStartToolStripMenuItem.Name = "resetCrawlerOnStartToolStripMenuItem";
            this.resetCrawlerOnStartToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.resetCrawlerOnStartToolStripMenuItem.Text = "Reset Crawler on Start";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.notepadToolStripMenuItem,
            this.openCurrentDirectoryToolStripMenuItem,
            this.performanceMonitorMSCtToolStripMenuItem,
            this.sQLServerManagementStudioToolStripMenuItem,
            this.taskManagerToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.automatorToolStripMenuItem,
            this.browserToolStripMenuItem,
            this.postProcessingToolStripMenuItem,
            this.scraperToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(241, 22);
            this.toolStripMenuItem2.Text = "arachnode.net Utilities";
            // 
            // automatorToolStripMenuItem
            // 
            this.automatorToolStripMenuItem.Name = "automatorToolStripMenuItem";
            this.automatorToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.automatorToolStripMenuItem.Text = "Automator";
            this.automatorToolStripMenuItem.Click += new System.EventHandler(this.automatorToolStripMenuItem_Click);
            // 
            // browserToolStripMenuItem
            // 
            this.browserToolStripMenuItem.Name = "browserToolStripMenuItem";
            this.browserToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.browserToolStripMenuItem.Text = "Browser";
            this.browserToolStripMenuItem.Click += new System.EventHandler(this.browserToolStripMenuItem_Click);
            // 
            // postProcessingToolStripMenuItem
            // 
            this.postProcessingToolStripMenuItem.Name = "postProcessingToolStripMenuItem";
            this.postProcessingToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.postProcessingToolStripMenuItem.Text = "Post Processing";
            this.postProcessingToolStripMenuItem.Click += new System.EventHandler(this.postProcessingToolStripMenuItem_Click);
            // 
            // scraperToolStripMenuItem
            // 
            this.scraperToolStripMenuItem.Name = "scraperToolStripMenuItem";
            this.scraperToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.scraperToolStripMenuItem.Text = "Scraper";
            this.scraperToolStripMenuItem.Click += new System.EventHandler(this.scraperToolStripMenuItem_Click);
            // 
            // notepadToolStripMenuItem
            // 
            this.notepadToolStripMenuItem.Name = "notepadToolStripMenuItem";
            this.notepadToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.notepadToolStripMenuItem.Text = "Notepad";
            this.notepadToolStripMenuItem.Click += new System.EventHandler(this.notepadToolStripMenuItem_Click);
            // 
            // openCurrentDirectoryToolStripMenuItem
            // 
            this.openCurrentDirectoryToolStripMenuItem.Name = "openCurrentDirectoryToolStripMenuItem";
            this.openCurrentDirectoryToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.openCurrentDirectoryToolStripMenuItem.Text = "Open Current Directory";
            this.openCurrentDirectoryToolStripMenuItem.Click += new System.EventHandler(this.openCurrentDirectoryToolStripMenuItem_Click);
            // 
            // performanceMonitorMSCtToolStripMenuItem
            // 
            this.performanceMonitorMSCtToolStripMenuItem.Name = "performanceMonitorMSCtToolStripMenuItem";
            this.performanceMonitorMSCtToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.performanceMonitorMSCtToolStripMenuItem.Text = "PerformanceMonitor.msc";
            this.performanceMonitorMSCtToolStripMenuItem.Click += new System.EventHandler(this.performanceMonitorMSCtToolStripMenuItem_Click);
            // 
            // sQLServerManagementStudioToolStripMenuItem
            // 
            this.sQLServerManagementStudioToolStripMenuItem.Name = "sQLServerManagementStudioToolStripMenuItem";
            this.sQLServerManagementStudioToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.sQLServerManagementStudioToolStripMenuItem.Text = "SQL Server Management Studio";
            this.sQLServerManagementStudioToolStripMenuItem.Click += new System.EventHandler(this.sQLServerManagementStudioToolStripMenuItem_Click);
            // 
            // taskManagerToolStripMenuItem
            // 
            this.taskManagerToolStripMenuItem.Name = "taskManagerToolStripMenuItem";
            this.taskManagerToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.taskManagerToolStripMenuItem.Text = "Task Manager";
            this.taskManagerToolStripMenuItem.Click += new System.EventHandler(this.taskManagerToolStripMenuItem_Click);
            // 
            // ssMain
            // 
            this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslEngineState,
            this.tsslElapasedTime,
            this.tsslApplicationTime});
            this.ssMain.Location = new System.Drawing.Point(3, 922);
            this.ssMain.Name = "ssMain";
            this.ssMain.Padding = new System.Windows.Forms.Padding(1, 0, 15, 0);
            this.ssMain.Size = new System.Drawing.Size(1257, 22);
            this.ssMain.TabIndex = 4;
            this.ssMain.Text = "Ready";
            // 
            // tsslEngineState
            // 
            this.tsslEngineState.Name = "tsslEngineState";
            this.tsslEngineState.Size = new System.Drawing.Size(122, 17);
            this.tsslEngineState.Text = "Engine State: Stopped";
            // 
            // tsslElapasedTime
            // 
            this.tsslElapasedTime.Name = "tsslElapasedTime";
            this.tsslElapasedTime.Size = new System.Drawing.Size(119, 17);
            this.tsslElapasedTime.Text = "/ Elapased Time: N/A";
            // 
            // tsslApplicationTime
            // 
            this.tsslApplicationTime.Name = "tsslApplicationTime";
            this.tsslApplicationTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tsslApplicationTime.Size = new System.Drawing.Size(1000, 17);
            this.tsslApplicationTime.Spring = true;
            this.tsslApplicationTime.Text = "Application Time: N/A";
            this.tsslApplicationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1263, 947);
            this.Controls.Add(this.ssMain);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.msMain);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.msMain;
            this.Name = "frmMain";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.ShowIcon = false;
            this.Text = "arachnode.net | graphical user interface";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerformanceCounters)).EndInit();
            this.tcMain.ResumeLayout(false);
            this.tpConfiguration.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConfiguration)).EndInit();
            this.tpApplicationSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvApplicationSettings)).EndInit();
            this.tpWebSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWebSettings)).EndInit();
            this.tpCredentialCache.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCredentialCache)).EndInit();
            this.tpCookieContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCookieContainer)).EndInit();
            this.tpProxyServers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProxyServers)).EndInit();
            this.tpCrawlRequests.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCrawlRequests)).EndInit();
            this.tpCrawler.ResumeLayout(false);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCrawler)).EndInit();
            this.cmsMain.ResumeLayout(false);
            this.tpCrawlInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCrawlInfo)).EndInit();
            this.tpPerformanceCounters.ResumeLayout(false);
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            this.ssMain.ResumeLayout(false);
            this.ssMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPerformanceCounters;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpPerformanceCounters;
        private System.Windows.Forms.TabPage tpCrawler;
        private System.Windows.Forms.TabPage tpConfiguration;
        private System.Windows.Forms.DataGridView dgvConfiguration;
        private System.Windows.Forms.DataGridView dgvTableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcTableName;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.MenuStrip msMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetDatabaseOnStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem breadthFirstByPriorityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem depthFirstByPriorityToolStripMenuItem;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.TabPage tpCrawlRequests;
        private System.Windows.Forms.Button btnLoadCrawlRequests;
        private System.Windows.Forms.DataGridView dgvCrawlRequests;
        private System.Windows.Forms.Button btnSaveCrawlRequests;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCrawlerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetDirectoriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetIISToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startPerfmonexeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCrawlerOnStartToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog sfdEntityRows;
        private System.Windows.Forms.StatusStrip ssMain;
        private System.Windows.Forms.ToolStripStatusLabel tsslEngineState;
        private System.Windows.Forms.ToolStripStatusLabel tsslElapasedTime;
        private System.Windows.Forms.TabPage tpApplicationSettings;
        private System.Windows.Forms.DataGridView dgvApplicationSettings;
        private System.Windows.Forms.TabPage tpWebSettings;
        private System.Windows.Forms.DataGridView dgvWebSettings;
        private System.Windows.Forms.OpenFileDialog ofdEntityRows;
        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcApplicationSettingsName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcApplicationSettingsValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcWebSettingsName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcWebSettingsValue;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem resetGUIToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcAverage;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcMaximum;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcTotal;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcCRAbsoluteUri;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcCRDepth;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcRestrictCrawlTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcRestrictDiscoveriesTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcCRPriority;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcRenderType;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcRenderTypeForChildren;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcCRAdded;
        private System.Windows.Forms.Button btnSaveApplicationSettings;
        private System.Windows.Forms.Button btnLoadApplicationsSettings;
        private System.Windows.Forms.Button btnSaveWebSettings;
        private System.Windows.Forms.Button btnLoadWebSettings;
        private System.Windows.Forms.Button btnSaveCrawler;
        private System.Windows.Forms.Button btnLoadCrawler;
        private System.Windows.Forms.Button btnSavePerformanceCounters;
        private System.Windows.Forms.Button btnLoadPerformanceCounters;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sQLServerManagementStudioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem taskManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notepadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCurrentDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel tsslApplicationTime;
        private System.Windows.Forms.ContextMenuStrip cmsMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiUndo;
        private System.Windows.Forms.ToolStripMenuItem tsmiCut;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
        private System.Windows.Forms.ToolStripSeparator tss1;
        private System.Windows.Forms.ToolStripMenuItem tsmiPaste;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectAll;
        private System.Windows.Forms.TabPage tpCrawlInfo;
        private System.Windows.Forms.DataGridView dgvCrawler;
        private System.Windows.Forms.DataGridView dgvCrawlInfo;
        private System.Windows.Forms.Button btnSaveCrawlInfo;
        private System.Windows.Forms.Button btnLoadCrawlInfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcThreadNumberCrawler;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcAbsoluteUri;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcStatusCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcDiscoveryType;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcHyperLinkDiscoveries;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcFileAndImageDiscoveries;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcCurrentDepth;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcMaximumDepth;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcThreadNumberCrawlInfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcParentDiscovery;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcCurrentDiscovery;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcCrawlState;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcTotalCrawlFedCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcTotalCrawlRequestsAssigned;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcTotalCrawlStarvedCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcTotalHttpWebResponseTime;
        private System.Windows.Forms.TabPage tpCredentialCache;
        private System.Windows.Forms.TabPage tpCookieContainer;
        private System.Windows.Forms.TabPage tpProxyServers;
        private System.Windows.Forms.Button btnSaveCredentialCache;
        private System.Windows.Forms.Button btnLoadCredentialCache;
        private System.Windows.Forms.DataGridView dgvCredentialCache;
        private System.Windows.Forms.Button btnSaveCookieContainer;
        private System.Windows.Forms.Button btnLoadCookieContainer;
        private System.Windows.Forms.DataGridView dgvCookieContainer;
        private System.Windows.Forms.Button btnSaveProxyServers;
        private System.Windows.Forms.Button btnLoadProxyServers;
        private System.Windows.Forms.DataGridView dgvProxyServers;
        private System.Windows.Forms.Button btnVerify;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvcbcSchemeCookieContainer;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcDomainCookieContainer;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgctbcValue;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvcbcIsEnabledCookieContainer;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvcbcSchemeProxyServers;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcIPAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcPort;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcAbsoluteUriToVerify;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcValueToVerify;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcTimeoutInMilliseconds;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcStatusCodeProxyServers;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvcbcIsEnabledProxyServers;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem automatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem postProcessingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scraperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem performanceMonitorMSCtToolStripMenuItem;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvcbcSchemeCredentialCache;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcDomainCredentialCache;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcUserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvtbcPassword;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvcbcIsEnabledCredentialCache;

    }
}

