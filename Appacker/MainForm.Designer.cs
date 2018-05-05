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
            this.checkRepackable = new System.Windows.Forms.CheckBox();
            this.picAppIcon = new System.Windows.Forms.PictureBox();
            this.btnLanguage = new System.Windows.Forms.Button();
            this.flagsIamgeList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picAppIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // folderBrowserDialog
            // 
            resources.ApplyResources(this.folderBrowserDialog, "folderBrowserDialog");
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // txtAppFolderPath
            // 
            resources.ApplyResources(this.txtAppFolderPath, "txtAppFolderPath");
            this.txtAppFolderPath.AllowDrop = true;
            this.txtAppFolderPath.BackColor = System.Drawing.SystemColors.Window;
            this.txtAppFolderPath.Name = "txtAppFolderPath";
            this.txtAppFolderPath.ReadOnly = true;
            this.txtAppFolderPath.TabStop = false;
            this.txtAppFolderPath.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.txtAppFolderPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtAppFolderPath_DragDrop);
            this.txtAppFolderPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtAppFolderPath_DragEnter);
            // 
            // btnBrowseAppFolder
            // 
            resources.ApplyResources(this.btnBrowseAppFolder, "btnBrowseAppFolder");
            this.btnBrowseAppFolder.Name = "btnBrowseAppFolder";
            this.btnBrowseAppFolder.UseVisualStyleBackColor = true;
            this.btnBrowseAppFolder.Click += new System.EventHandler(this.btnBrowseAppFolder_Click);
            // 
            // labAppFolderPath
            // 
            resources.ApplyResources(this.labAppFolderPath, "labAppFolderPath");
            this.labAppFolderPath.Name = "labAppFolderPath";
            // 
            // labPackPath
            // 
            resources.ApplyResources(this.labPackPath, "labPackPath");
            this.labPackPath.Name = "labPackPath";
            // 
            // txtPackPath
            // 
            resources.ApplyResources(this.txtPackPath, "txtPackPath");
            this.txtPackPath.AllowDrop = true;
            this.txtPackPath.Name = "txtPackPath";
            this.txtPackPath.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.txtPackPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtPackPath_DragDrop);
            this.txtPackPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtAppFolderPath_DragEnter);
            // 
            // btnBrowsePackPath
            // 
            resources.ApplyResources(this.btnBrowsePackPath, "btnBrowsePackPath");
            this.btnBrowsePackPath.Name = "btnBrowsePackPath";
            this.btnBrowsePackPath.UseVisualStyleBackColor = true;
            this.btnBrowsePackPath.Click += new System.EventHandler(this.btnBrowsePackPath_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "exe";
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            // 
            // treeView
            // 
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.AllowDrop = true;
            this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            this.treeView.ImageList = this.treeViewIconsList;
            this.treeView.Name = "treeView";
            this.treeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeSelect);
            this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtAppFolderPath_DragDrop);
            this.treeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtAppFolderPath_DragEnter);
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
            resources.ApplyResources(this.txtMainExePath, "txtMainExePath");
            this.txtMainExePath.BackColor = System.Drawing.SystemColors.Window;
            this.txtMainExePath.Name = "txtMainExePath";
            this.txtMainExePath.ReadOnly = true;
            this.txtMainExePath.TabStop = false;
            this.txtMainExePath.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            // 
            // labMainExePath
            // 
            resources.ApplyResources(this.labMainExePath, "labMainExePath");
            this.labMainExePath.Name = "labMainExePath";
            // 
            // btnPack
            // 
            resources.ApplyResources(this.btnPack, "btnPack");
            this.btnPack.Name = "btnPack";
            this.btnPack.UseVisualStyleBackColor = true;
            this.btnPack.Click += new System.EventHandler(this.btnPack_Click);
            // 
            // checkRepackable
            // 
            resources.ApplyResources(this.checkRepackable, "checkRepackable");
            this.checkRepackable.Name = "checkRepackable";
            this.checkRepackable.UseVisualStyleBackColor = true;
            // 
            // picAppIcon
            // 
            resources.ApplyResources(this.picAppIcon, "picAppIcon");
            this.picAppIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picAppIcon.Name = "picAppIcon";
            this.picAppIcon.TabStop = false;
            // 
            // btnLanguage
            // 
            resources.ApplyResources(this.btnLanguage, "btnLanguage");
            this.btnLanguage.ImageList = this.flagsIamgeList;
            this.btnLanguage.Name = "btnLanguage";
            this.btnLanguage.TabStop = false;
            this.btnLanguage.UseVisualStyleBackColor = true;
            this.btnLanguage.Click += new System.EventHandler(this.btnLanguage_Click);
            // 
            // flagsIamgeList
            // 
            this.flagsIamgeList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("flagsIamgeList.ImageStream")));
            this.flagsIamgeList.TransparentColor = System.Drawing.Color.Transparent;
            this.flagsIamgeList.Images.SetKeyName(0, "russia-flag-icon-32.png");
            this.flagsIamgeList.Images.SetKeyName(1, "united-kingdom-flag-icon-32.png");
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnLanguage);
            this.Controls.Add(this.picAppIcon);
            this.Controls.Add(this.checkRepackable);
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
            ((System.ComponentModel.ISupportInitialize)(this.picAppIcon)).EndInit();
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
        private System.Windows.Forms.CheckBox checkRepackable;
        private System.Windows.Forms.PictureBox picAppIcon;
        private System.Windows.Forms.Button btnLanguage;
        private System.Windows.Forms.ImageList flagsIamgeList;
    }
}

