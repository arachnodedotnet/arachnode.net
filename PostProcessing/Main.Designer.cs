namespace Arachnode.PostProcessing
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
            this.gbProcessWebPages = new System.Windows.Forms.GroupBox();
            this.nudWebPageIDUpperBound = new System.Windows.Forms.NumericUpDown();
            this.nudWebPageIDLowerBound = new System.Windows.Forms.NumericUpDown();
            this.btnProcessWebPages = new System.Windows.Forms.Button();
            this.gpProcessFiles = new System.Windows.Forms.GroupBox();
            this.nudFileIDUpperBound = new System.Windows.Forms.NumericUpDown();
            this.nudFileIDLowerBound = new System.Windows.Forms.NumericUpDown();
            this.btnProcessFiles = new System.Windows.Forms.Button();
            this.gpProcessImages = new System.Windows.Forms.GroupBox();
            this.nudImagesIDUpperBound = new System.Windows.Forms.NumericUpDown();
            this.nudImagesIDLowerBound = new System.Windows.Forms.NumericUpDown();
            this.btnProcessImages = new System.Windows.Forms.Button();
            this.rtbPostProcessingStatus = new System.Windows.Forms.RichTextBox();
            this.gbProcessWebPages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWebPageIDUpperBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWebPageIDLowerBound)).BeginInit();
            this.gpProcessFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFileIDUpperBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFileIDLowerBound)).BeginInit();
            this.gpProcessImages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImagesIDUpperBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudImagesIDLowerBound)).BeginInit();
            this.SuspendLayout();
            // 
            // gbProcessWebPages
            // 
            this.gbProcessWebPages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProcessWebPages.Controls.Add(this.nudWebPageIDUpperBound);
            this.gbProcessWebPages.Controls.Add(this.nudWebPageIDLowerBound);
            this.gbProcessWebPages.Controls.Add(this.btnProcessWebPages);
            this.gbProcessWebPages.Location = new System.Drawing.Point(12, 13);
            this.gbProcessWebPages.Name = "gbProcessWebPages";
            this.gbProcessWebPages.Size = new System.Drawing.Size(1239, 57);
            this.gbProcessWebPages.TabIndex = 0;
            this.gbProcessWebPages.TabStop = false;
            this.gbProcessWebPages.Text = "WebPages";
            // 
            // nudWebPageIDUpperBound
            // 
            this.nudWebPageIDUpperBound.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudWebPageIDUpperBound.Location = new System.Drawing.Point(198, 21);
            this.nudWebPageIDUpperBound.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudWebPageIDUpperBound.Name = "nudWebPageIDUpperBound";
            this.nudWebPageIDUpperBound.Size = new System.Drawing.Size(105, 22);
            this.nudWebPageIDUpperBound.TabIndex = 2;
            this.nudWebPageIDUpperBound.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            // 
            // nudWebPageIDLowerBound
            // 
            this.nudWebPageIDLowerBound.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudWebPageIDLowerBound.Location = new System.Drawing.Point(87, 21);
            this.nudWebPageIDLowerBound.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudWebPageIDLowerBound.Name = "nudWebPageIDLowerBound";
            this.nudWebPageIDLowerBound.Size = new System.Drawing.Size(105, 22);
            this.nudWebPageIDLowerBound.TabIndex = 1;
            this.nudWebPageIDLowerBound.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnProcessWebPages
            // 
            this.btnProcessWebPages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcessWebPages.Location = new System.Drawing.Point(6, 20);
            this.btnProcessWebPages.Name = "btnProcessWebPages";
            this.btnProcessWebPages.Size = new System.Drawing.Size(75, 25);
            this.btnProcessWebPages.TabIndex = 0;
            this.btnProcessWebPages.Text = "Process";
            this.btnProcessWebPages.UseVisualStyleBackColor = true;
            this.btnProcessWebPages.Click += new System.EventHandler(this.btnProcessWebPages_Click);
            // 
            // gpProcessFiles
            // 
            this.gpProcessFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpProcessFiles.Controls.Add(this.nudFileIDUpperBound);
            this.gpProcessFiles.Controls.Add(this.nudFileIDLowerBound);
            this.gpProcessFiles.Controls.Add(this.btnProcessFiles);
            this.gpProcessFiles.Location = new System.Drawing.Point(12, 76);
            this.gpProcessFiles.Name = "gpProcessFiles";
            this.gpProcessFiles.Size = new System.Drawing.Size(1239, 57);
            this.gpProcessFiles.TabIndex = 3;
            this.gpProcessFiles.TabStop = false;
            this.gpProcessFiles.Text = "Files";
            // 
            // nudFileIDUpperBound
            // 
            this.nudFileIDUpperBound.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudFileIDUpperBound.Location = new System.Drawing.Point(198, 21);
            this.nudFileIDUpperBound.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudFileIDUpperBound.Name = "nudFileIDUpperBound";
            this.nudFileIDUpperBound.Size = new System.Drawing.Size(105, 22);
            this.nudFileIDUpperBound.TabIndex = 2;
            this.nudFileIDUpperBound.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            // 
            // nudFileIDLowerBound
            // 
            this.nudFileIDLowerBound.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudFileIDLowerBound.Location = new System.Drawing.Point(87, 21);
            this.nudFileIDLowerBound.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudFileIDLowerBound.Name = "nudFileIDLowerBound";
            this.nudFileIDLowerBound.Size = new System.Drawing.Size(105, 22);
            this.nudFileIDLowerBound.TabIndex = 1;
            this.nudFileIDLowerBound.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnProcessFiles
            // 
            this.btnProcessFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcessFiles.Location = new System.Drawing.Point(6, 20);
            this.btnProcessFiles.Name = "btnProcessFiles";
            this.btnProcessFiles.Size = new System.Drawing.Size(75, 25);
            this.btnProcessFiles.TabIndex = 0;
            this.btnProcessFiles.Text = "Process";
            this.btnProcessFiles.UseVisualStyleBackColor = true;
            this.btnProcessFiles.Click += new System.EventHandler(this.btnProcessFiles_Click);
            // 
            // gpProcessImages
            // 
            this.gpProcessImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpProcessImages.Controls.Add(this.nudImagesIDUpperBound);
            this.gpProcessImages.Controls.Add(this.nudImagesIDLowerBound);
            this.gpProcessImages.Controls.Add(this.btnProcessImages);
            this.gpProcessImages.Location = new System.Drawing.Point(12, 140);
            this.gpProcessImages.Name = "gpProcessImages";
            this.gpProcessImages.Size = new System.Drawing.Size(1239, 57);
            this.gpProcessImages.TabIndex = 4;
            this.gpProcessImages.TabStop = false;
            this.gpProcessImages.Text = "Images";
            // 
            // nudImagesIDUpperBound
            // 
            this.nudImagesIDUpperBound.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudImagesIDUpperBound.Location = new System.Drawing.Point(199, 21);
            this.nudImagesIDUpperBound.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudImagesIDUpperBound.Name = "nudImagesIDUpperBound";
            this.nudImagesIDUpperBound.Size = new System.Drawing.Size(105, 22);
            this.nudImagesIDUpperBound.TabIndex = 2;
            this.nudImagesIDUpperBound.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            // 
            // nudImagesIDLowerBound
            // 
            this.nudImagesIDLowerBound.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudImagesIDLowerBound.Location = new System.Drawing.Point(88, 21);
            this.nudImagesIDLowerBound.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudImagesIDLowerBound.Name = "nudImagesIDLowerBound";
            this.nudImagesIDLowerBound.Size = new System.Drawing.Size(105, 22);
            this.nudImagesIDLowerBound.TabIndex = 1;
            this.nudImagesIDLowerBound.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnProcessImages
            // 
            this.btnProcessImages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcessImages.Location = new System.Drawing.Point(6, 20);
            this.btnProcessImages.Name = "btnProcessImages";
            this.btnProcessImages.Size = new System.Drawing.Size(75, 25);
            this.btnProcessImages.TabIndex = 0;
            this.btnProcessImages.Text = "Process";
            this.btnProcessImages.UseVisualStyleBackColor = true;
            this.btnProcessImages.Click += new System.EventHandler(this.btnProcessImages_Click);
            // 
            // rtbPostProcessingStatus
            // 
            this.rtbPostProcessingStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbPostProcessingStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbPostProcessingStatus.Location = new System.Drawing.Point(12, 204);
            this.rtbPostProcessingStatus.Name = "rtbPostProcessingStatus";
            this.rtbPostProcessingStatus.Size = new System.Drawing.Size(1239, 730);
            this.rtbPostProcessingStatus.TabIndex = 5;
            this.rtbPostProcessingStatus.Text = "";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1263, 947);
            this.Controls.Add(this.rtbPostProcessingStatus);
            this.Controls.Add(this.gpProcessImages);
            this.Controls.Add(this.gpProcessFiles);
            this.Controls.Add(this.gbProcessWebPages);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Main";
            this.ShowIcon = false;
            this.Text = "arachnode.net | Post Processing";
            this.gbProcessWebPages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudWebPageIDUpperBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWebPageIDLowerBound)).EndInit();
            this.gpProcessFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudFileIDUpperBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFileIDLowerBound)).EndInit();
            this.gpProcessImages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudImagesIDUpperBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudImagesIDLowerBound)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbProcessWebPages;
        private System.Windows.Forms.NumericUpDown nudWebPageIDUpperBound;
        private System.Windows.Forms.NumericUpDown nudWebPageIDLowerBound;
        private System.Windows.Forms.Button btnProcessWebPages;
        private System.Windows.Forms.GroupBox gpProcessFiles;
        private System.Windows.Forms.NumericUpDown nudFileIDUpperBound;
        private System.Windows.Forms.NumericUpDown nudFileIDLowerBound;
        private System.Windows.Forms.Button btnProcessFiles;
        private System.Windows.Forms.GroupBox gpProcessImages;
        private System.Windows.Forms.NumericUpDown nudImagesIDUpperBound;
        private System.Windows.Forms.NumericUpDown nudImagesIDLowerBound;
        private System.Windows.Forms.Button btnProcessImages;
        private System.Windows.Forms.RichTextBox rtbPostProcessingStatus;
    }
}

