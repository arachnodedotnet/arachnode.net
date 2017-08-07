namespace Arachnode.Browser
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
            this.gpViewImages = new System.Windows.Forms.GroupBox();
            this.llImageDiscoveryPathDirectory = new System.Windows.Forms.LinkLabel();
            this.nudImageID = new System.Windows.Forms.NumericUpDown();
            this.btnViewImage = new System.Windows.Forms.Button();
            this.gpViewFiles = new System.Windows.Forms.GroupBox();
            this.llFileDiscoveryPathDirectory = new System.Windows.Forms.LinkLabel();
            this.nudFileID = new System.Windows.Forms.NumericUpDown();
            this.btnViewFile = new System.Windows.Forms.Button();
            this.gbViewWebPages = new System.Windows.Forms.GroupBox();
            this.llWebPageDiscoveryPathDirectory = new System.Windows.Forms.LinkLabel();
            this.nudWebPageID = new System.Windows.Forms.NumericUpDown();
            this.btnViewWebPage = new System.Windows.Forms.Button();
            this.wbMain = new System.Windows.Forms.WebBrowser();
            this.cbRemoveScriptsFromWebPages = new System.Windows.Forms.CheckBox();
            this.cbAutoView = new System.Windows.Forms.CheckBox();
            this.tbFileName = new System.Windows.Forms.Label();
            this.gpViewImages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageID)).BeginInit();
            this.gpViewFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFileID)).BeginInit();
            this.gbViewWebPages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWebPageID)).BeginInit();
            this.SuspendLayout();
            // 
            // gpViewImages
            // 
            this.gpViewImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpViewImages.Controls.Add(this.llImageDiscoveryPathDirectory);
            this.gpViewImages.Controls.Add(this.nudImageID);
            this.gpViewImages.Controls.Add(this.btnViewImage);
            this.gpViewImages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gpViewImages.Location = new System.Drawing.Point(6, 140);
            this.gpViewImages.Name = "gpViewImages";
            this.gpViewImages.Size = new System.Drawing.Size(1252, 57);
            this.gpViewImages.TabIndex = 7;
            this.gpViewImages.TabStop = false;
            this.gpViewImages.Text = "Images";
            // 
            // llImageDiscoveryPathDirectory
            // 
            this.llImageDiscoveryPathDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.llImageDiscoveryPathDirectory.AutoEllipsis = true;
            this.llImageDiscoveryPathDirectory.Location = new System.Drawing.Point(199, 26);
            this.llImageDiscoveryPathDirectory.Name = "llImageDiscoveryPathDirectory";
            this.llImageDiscoveryPathDirectory.Size = new System.Drawing.Size(1047, 14);
            this.llImageDiscoveryPathDirectory.TabIndex = 10;
            this.llImageDiscoveryPathDirectory.TabStop = true;
            this.llImageDiscoveryPathDirectory.Text = "linkLabel1";
            this.llImageDiscoveryPathDirectory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llImageDiscoveryPathDirectory_LinkClicked);
            // 
            // nudImageID
            // 
            this.nudImageID.Location = new System.Drawing.Point(88, 23);
            this.nudImageID.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nudImageID.Name = "nudImageID";
            this.nudImageID.Size = new System.Drawing.Size(105, 20);
            this.nudImageID.TabIndex = 1;
            this.nudImageID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudImageID.ValueChanged += new System.EventHandler(this.nudImageID_ValueChanged);
            // 
            // btnViewImage
            // 
            this.btnViewImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewImage.Location = new System.Drawing.Point(6, 20);
            this.btnViewImage.Name = "btnViewImage";
            this.btnViewImage.Size = new System.Drawing.Size(75, 25);
            this.btnViewImage.TabIndex = 0;
            this.btnViewImage.Text = "View";
            this.btnViewImage.UseVisualStyleBackColor = true;
            this.btnViewImage.Click += new System.EventHandler(this.btnViewImage_Click);
            // 
            // gpViewFiles
            // 
            this.gpViewFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpViewFiles.Controls.Add(this.llFileDiscoveryPathDirectory);
            this.gpViewFiles.Controls.Add(this.nudFileID);
            this.gpViewFiles.Controls.Add(this.btnViewFile);
            this.gpViewFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gpViewFiles.Location = new System.Drawing.Point(6, 76);
            this.gpViewFiles.Name = "gpViewFiles";
            this.gpViewFiles.Size = new System.Drawing.Size(1252, 57);
            this.gpViewFiles.TabIndex = 6;
            this.gpViewFiles.TabStop = false;
            this.gpViewFiles.Text = "Files";
            // 
            // llFileDiscoveryPathDirectory
            // 
            this.llFileDiscoveryPathDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.llFileDiscoveryPathDirectory.AutoEllipsis = true;
            this.llFileDiscoveryPathDirectory.Location = new System.Drawing.Point(199, 26);
            this.llFileDiscoveryPathDirectory.Name = "llFileDiscoveryPathDirectory";
            this.llFileDiscoveryPathDirectory.Size = new System.Drawing.Size(1047, 14);
            this.llFileDiscoveryPathDirectory.TabIndex = 10;
            this.llFileDiscoveryPathDirectory.TabStop = true;
            this.llFileDiscoveryPathDirectory.Text = "linkLabel1";
            this.llFileDiscoveryPathDirectory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llFileDiscoveryPathDirectory_LinkClicked);
            // 
            // nudFileID
            // 
            this.nudFileID.Location = new System.Drawing.Point(88, 23);
            this.nudFileID.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nudFileID.Name = "nudFileID";
            this.nudFileID.Size = new System.Drawing.Size(105, 20);
            this.nudFileID.TabIndex = 1;
            this.nudFileID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFileID.ValueChanged += new System.EventHandler(this.nudFileID_ValueChanged);
            // 
            // btnViewFile
            // 
            this.btnViewFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewFile.Location = new System.Drawing.Point(6, 20);
            this.btnViewFile.Name = "btnViewFile";
            this.btnViewFile.Size = new System.Drawing.Size(75, 25);
            this.btnViewFile.TabIndex = 0;
            this.btnViewFile.Text = "View";
            this.btnViewFile.UseVisualStyleBackColor = true;
            this.btnViewFile.Click += new System.EventHandler(this.btnViewFile_Click);
            // 
            // gbViewWebPages
            // 
            this.gbViewWebPages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbViewWebPages.Controls.Add(this.llWebPageDiscoveryPathDirectory);
            this.gbViewWebPages.Controls.Add(this.nudWebPageID);
            this.gbViewWebPages.Controls.Add(this.btnViewWebPage);
            this.gbViewWebPages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gbViewWebPages.Location = new System.Drawing.Point(6, 13);
            this.gbViewWebPages.Name = "gbViewWebPages";
            this.gbViewWebPages.Size = new System.Drawing.Size(1252, 57);
            this.gbViewWebPages.TabIndex = 5;
            this.gbViewWebPages.TabStop = false;
            this.gbViewWebPages.Text = "WebPages";
            // 
            // llWebPageDiscoveryPathDirectory
            // 
            this.llWebPageDiscoveryPathDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.llWebPageDiscoveryPathDirectory.AutoEllipsis = true;
            this.llWebPageDiscoveryPathDirectory.Location = new System.Drawing.Point(199, 26);
            this.llWebPageDiscoveryPathDirectory.Name = "llWebPageDiscoveryPathDirectory";
            this.llWebPageDiscoveryPathDirectory.Size = new System.Drawing.Size(1047, 14);
            this.llWebPageDiscoveryPathDirectory.TabIndex = 9;
            this.llWebPageDiscoveryPathDirectory.TabStop = true;
            this.llWebPageDiscoveryPathDirectory.Text = "linkLabel1";
            this.llWebPageDiscoveryPathDirectory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llWebPageDiscoveryPathDirectory_LinkClicked);
            // 
            // nudWebPageID
            // 
            this.nudWebPageID.Location = new System.Drawing.Point(88, 23);
            this.nudWebPageID.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nudWebPageID.Name = "nudWebPageID";
            this.nudWebPageID.Size = new System.Drawing.Size(105, 20);
            this.nudWebPageID.TabIndex = 1;
            this.nudWebPageID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudWebPageID.ValueChanged += new System.EventHandler(this.nudWebPageID_ValueChanged);
            // 
            // btnViewWebPage
            // 
            this.btnViewWebPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewWebPage.Location = new System.Drawing.Point(6, 20);
            this.btnViewWebPage.Name = "btnViewWebPage";
            this.btnViewWebPage.Size = new System.Drawing.Size(75, 25);
            this.btnViewWebPage.TabIndex = 0;
            this.btnViewWebPage.Text = "View";
            this.btnViewWebPage.UseVisualStyleBackColor = true;
            this.btnViewWebPage.Click += new System.EventHandler(this.btnViewWebPage_Click);
            // 
            // wbMain
            // 
            this.wbMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wbMain.Location = new System.Drawing.Point(6, 229);
            this.wbMain.MinimumSize = new System.Drawing.Size(20, 22);
            this.wbMain.Name = "wbMain";
            this.wbMain.Size = new System.Drawing.Size(1252, 711);
            this.wbMain.TabIndex = 8;
            // 
            // cbRemoveScriptsFromWebPages
            // 
            this.cbRemoveScriptsFromWebPages.AutoSize = true;
            this.cbRemoveScriptsFromWebPages.Checked = true;
            this.cbRemoveScriptsFromWebPages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRemoveScriptsFromWebPages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbRemoveScriptsFromWebPages.Location = new System.Drawing.Point(6, 204);
            this.cbRemoveScriptsFromWebPages.Name = "cbRemoveScriptsFromWebPages";
            this.cbRemoveScriptsFromWebPages.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbRemoveScriptsFromWebPages.Size = new System.Drawing.Size(181, 18);
            this.cbRemoveScriptsFromWebPages.TabIndex = 9;
            this.cbRemoveScriptsFromWebPages.Text = "Remove Scripts From WebPages";
            this.cbRemoveScriptsFromWebPages.UseVisualStyleBackColor = true;
            // 
            // cbAutoView
            // 
            this.cbAutoView.AutoSize = true;
            this.cbAutoView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbAutoView.Location = new System.Drawing.Point(192, 204);
            this.cbAutoView.Name = "cbAutoView";
            this.cbAutoView.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbAutoView.Size = new System.Drawing.Size(72, 18);
            this.cbAutoView.TabIndex = 10;
            this.cbAutoView.Text = "AutoView";
            this.cbAutoView.UseVisualStyleBackColor = true;
            // 
            // tbFileName
            // 
            this.tbFileName.AutoSize = true;
            this.tbFileName.Location = new System.Drawing.Point(269, 205);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(36, 14);
            this.tbFileName.TabIndex = 11;
            this.tbFileName.Text = "[ N/A ]";
            this.tbFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1263, 947);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.cbAutoView);
            this.Controls.Add(this.cbRemoveScriptsFromWebPages);
            this.Controls.Add(this.wbMain);
            this.Controls.Add(this.gpViewImages);
            this.Controls.Add(this.gpViewFiles);
            this.Controls.Add(this.gbViewWebPages);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Main";
            this.ShowIcon = false;
            this.Text = "arachnode.net | Browser";
            this.Load += new System.EventHandler(this.Main_Load);
            this.gpViewImages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudImageID)).EndInit();
            this.gpViewFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudFileID)).EndInit();
            this.gbViewWebPages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudWebPageID)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gpViewImages;
        private System.Windows.Forms.NumericUpDown nudImageID;
        private System.Windows.Forms.Button btnViewImage;
        private System.Windows.Forms.GroupBox gpViewFiles;
        private System.Windows.Forms.NumericUpDown nudFileID;
        private System.Windows.Forms.Button btnViewFile;
        private System.Windows.Forms.GroupBox gbViewWebPages;
        private System.Windows.Forms.NumericUpDown nudWebPageID;
        private System.Windows.Forms.Button btnViewWebPage;
        private System.Windows.Forms.WebBrowser wbMain;
        private System.Windows.Forms.LinkLabel llImageDiscoveryPathDirectory;
        private System.Windows.Forms.LinkLabel llFileDiscoveryPathDirectory;
        private System.Windows.Forms.LinkLabel llWebPageDiscoveryPathDirectory;
        private System.Windows.Forms.CheckBox cbRemoveScriptsFromWebPages;
        private System.Windows.Forms.CheckBox cbAutoView;
        private System.Windows.Forms.Label tbFileName;
    }
}

