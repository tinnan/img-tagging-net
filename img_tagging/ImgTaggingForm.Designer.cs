namespace img_tagging
{
    partial class ImgTaggingForm
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
            this.btnTag = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.grpProgress = new System.Windows.Forms.GroupBox();
            this.tagProgress = new System.Windows.Forms.ProgressBar();
            this.pathBrowseDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.txtTaskLogs = new System.Windows.Forms.TextBox();
            this.grpProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnTag
            // 
            this.btnTag.Location = new System.Drawing.Point(12, 45);
            this.btnTag.Name = "btnTag";
            this.btnTag.Size = new System.Drawing.Size(75, 23);
            this.btnTag.TabIndex = 0;
            this.btnTag.Text = "Tag";
            this.btnTag.UseVisualStyleBackColor = true;
            this.btnTag.Click += new System.EventHandler(this.btnTag_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(12, 13);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(400, 20);
            this.txtPath.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(418, 11);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 22);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // grpProgress
            // 
            this.grpProgress.Controls.Add(this.txtTaskLogs);
            this.grpProgress.Controls.Add(this.tagProgress);
            this.grpProgress.Location = new System.Drawing.Point(13, 89);
            this.grpProgress.Name = "grpProgress";
            this.grpProgress.Size = new System.Drawing.Size(664, 360);
            this.grpProgress.TabIndex = 3;
            this.grpProgress.TabStop = false;
            this.grpProgress.Text = "Progress";
            // 
            // tagProgress
            // 
            this.tagProgress.Location = new System.Drawing.Point(7, 20);
            this.tagProgress.Name = "tagProgress";
            this.tagProgress.Size = new System.Drawing.Size(651, 23);
            this.tagProgress.TabIndex = 0;
            // 
            // pathBrowseDialog
            // 
            this.pathBrowseDialog.Description = "Select root folder to start tagging.";
            this.pathBrowseDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.pathBrowseDialog.ShowNewFolderButton = false;
            this.pathBrowseDialog.HelpRequest += new System.EventHandler(this.pathBrowseDialog_HelpRequest);
            // 
            // txtTaskLogs
            // 
            this.txtTaskLogs.Location = new System.Drawing.Point(7, 50);
            this.txtTaskLogs.Multiline = true;
            this.txtTaskLogs.Name = "txtTaskLogs";
            this.txtTaskLogs.ReadOnly = true;
            this.txtTaskLogs.Size = new System.Drawing.Size(651, 304);
            this.txtTaskLogs.TabIndex = 1;
            // 
            // ImgTaggingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 461);
            this.Controls.Add(this.grpProgress);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.btnTag);
            this.Name = "ImgTaggingForm";
            this.Text = "Image tagging";
            this.grpProgress.ResumeLayout(false);
            this.grpProgress.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTag;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox grpProgress;
        private System.Windows.Forms.ProgressBar tagProgress;
        private System.Windows.Forms.FolderBrowserDialog pathBrowseDialog;
        private System.Windows.Forms.TextBox txtTaskLogs;
    }
}

