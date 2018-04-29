namespace Appacker
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.txtAppFolderPath = new System.Windows.Forms.TextBox();
            this.btnBrowseAppFolder = new System.Windows.Forms.Button();
            this.labAppFolderPath = new System.Windows.Forms.Label();
            this.labPackPath = new System.Windows.Forms.Label();
            this.txtPackPath = new System.Windows.Forms.TextBox();
            this.btnBrowsePackPath = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.treeView = new System.Windows.Forms.TreeView();
            this.treeViewIconsList = new System.Windows.Forms.ImageList(this.components);
            this.txtMainExePath = new System.Windows.Forms.TextBox();
            this.labMainExePath = new System.Windows.Forms.Label();
            this.btnPack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // txtAppFolderPath
            // 
            this.txtAppFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAppFolderPath.BackColor = System.Drawing.SystemColors.Window;
            this.txtAppFolderPath.Location = new System.Drawing.Point(12, 25);
            this.txtAppFolderPath.Name = "txtAppFolderPath";
            this.txtAppFolderPath.ReadOnly = true;
            this.txtAppFolderPath.Size = new System.Drawing.Size(357, 20);
            this.txtAppFolderPath.TabIndex = 0;
            // 
            // btnBrowseAppFolder
            // 
            this.btnBrowseAppFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseAppFolder.Location = new System.Drawing.Point(375, 24);
            this.btnBrowseAppFolder.Name = "btnBrowseAppFolder";
            this.btnBrowseAppFolder.Size = new System.Drawing.Size(75, 22);
            this.btnBrowseAppFolder.TabIndex = 1;
            this.btnBrowseAppFolder.Text = "Browse...";
            this.btnBrowseAppFolder.UseVisualStyleBackColor = true;
            this.btnBrowseAppFolder.Click += new System.EventHandler(this.btnBrowseAppFolder_Click);
            // 
            // labAppFolderPath
            // 
            this.labAppFolderPath.AutoSize = true;
            this.labAppFolderPath.Location = new System.Drawing.Point(12, 9);
            this.labAppFolderPath.Name = "labAppFolderPath";
            this.labAppFolderPath.Size = new System.Drawing.Size(194, 13);
            this.labAppFolderPath.TabIndex = 2;
            this.labAppFolderPath.Text = "Directory containing application to pack";
            // 
            // labPackPath
            // 
            this.labPackPath.AutoSize = true;
            this.labPackPath.Location = new System.Drawing.Point(12, 58);
            this.labPackPath.Name = "labPackPath";
            this.labPackPath.Size = new System.Drawing.Size(163, 13);
            this.labPackPath.TabIndex = 2;
            this.labPackPath.Text = "Where packed app will be saved";
            // 
            // txtPackPath
            // 
            this.txtPackPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPackPath.Location = new System.Drawing.Point(12, 74);
            this.txtPackPath.Name = "txtPackPath";
            this.txtPackPath.Size = new System.Drawing.Size(357, 20);
            this.txtPackPath.TabIndex = 0;
            // 
            // btnBrowsePackPath
            // 
            this.btnBrowsePackPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowsePackPath.Location = new System.Drawing.Point(375, 73);
            this.btnBrowsePackPath.Name = "btnBrowsePackPath";
            this.btnBrowsePackPath.Size = new System.Drawing.Size(75, 22);
            this.btnBrowsePackPath.TabIndex = 1;
            this.btnBrowsePackPath.Text = "Browse...";
            this.btnBrowsePackPath.UseVisualStyleBackColor = true;
            this.btnBrowsePackPath.Click += new System.EventHandler(this.btnBrowsePackPath_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "exe";
            this.saveFileDialog.Filter = "Executables|*.exe";
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.treeViewIconsList;
            this.treeView.Indent = 17;
            this.treeView.Location = new System.Drawing.Point(12, 149);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 1;
            this.treeView.Size = new System.Drawing.Size(357, 241);
            this.treeView.TabIndex = 3;
            this.treeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeSelect);
            // 
            // treeViewIconsList
            // 
            this.treeViewIconsList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeViewIconsList.ImageStream")));
            this.treeViewIconsList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeViewIconsList.Images.SetKeyName(0, "folder-16.png");
            this.treeViewIconsList.Images.SetKeyName(1, "folder-open-16.png");
            this.treeViewIconsList.Images.SetKeyName(2, "file-16.png");
            this.treeViewIconsList.Images.SetKeyName(3, "file-exe-16.png");
            this.treeViewIconsList.Images.SetKeyName(4, "package-16.png");
            // 
            // txtMainExePath
            // 
            this.txtMainExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMainExePath.BackColor = System.Drawing.SystemColors.Window;
            this.txtMainExePath.Location = new System.Drawing.Point(12, 123);
            this.txtMainExePath.Name = "txtMainExePath";
            this.txtMainExePath.ReadOnly = true;
            this.txtMainExePath.Size = new System.Drawing.Size(357, 20);
            this.txtMainExePath.TabIndex = 0;
            // 
            // labMainExePath
            // 
            this.labMainExePath.AutoSize = true;
            this.labMainExePath.Location = new System.Drawing.Point(12, 107);
            this.labMainExePath.Name = "labMainExePath";
            this.labMainExePath.Size = new System.Drawing.Size(117, 13);
            this.labMainExePath.TabIndex = 2;
            this.labMainExePath.Text = "Select main executable";
            // 
            // btnPack
            // 
            this.btnPack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPack.Enabled = false;
            this.btnPack.Location = new System.Drawing.Point(375, 341);
            this.btnPack.Name = "btnPack";
            this.btnPack.Size = new System.Drawing.Size(75, 49);
            this.btnPack.TabIndex = 4;
            this.btnPack.Text = "Pack!";
            this.btnPack.UseVisualStyleBackColor = true;
            this.btnPack.Click += new System.EventHandler(this.btnPack_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 402);
            this.Controls.Add(this.btnPack);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.labMainExePath);
            this.Controls.Add(this.labPackPath);
            this.Controls.Add(this.labAppFolderPath);
            this.Controls.Add(this.btnBrowsePackPath);
            this.Controls.Add(this.btnBrowseAppFolder);
            this.Controls.Add(this.txtMainExePath);
            this.Controls.Add(this.txtPackPath);
            this.Controls.Add(this.txtAppFolderPath);
            this.Name = "MainForm";
            this.Text = "Appacker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox txtAppFolderPath;
        private System.Windows.Forms.Button btnBrowseAppFolder;
        private System.Windows.Forms.Label labAppFolderPath;
        private System.Windows.Forms.Label labPackPath;
        private System.Windows.Forms.TextBox txtPackPath;
        private System.Windows.Forms.Button btnBrowsePackPath;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.TextBox txtMainExePath;
        private System.Windows.Forms.Label labMainExePath;
        private System.Windows.Forms.ImageList treeViewIconsList;
        private System.Windows.Forms.Button btnPack;
    }
}

