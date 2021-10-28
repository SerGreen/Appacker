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
            this.txtPackExePath = new System.Windows.Forms.TextBox();
            this.btnBrowsePackPath = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.treeView = new System.Windows.Forms.TreeView();
            this.treeVieIconsList = new System.Windows.Forms.ImageList(this.components);
            this.labMainExePath = new System.Windows.Forms.Label();
            this.btnPack = new System.Windows.Forms.Button();
            this.flagsIamgeList = new System.Windows.Forms.ImageList(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.russianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labTreeViewCaption = new System.Windows.Forms.Label();
            this.btnChangeIcon = new System.Windows.Forms.LinkLabel();
            this.comboMainExePath = new System.Windows.Forms.ComboBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labOverride = new System.Windows.Forms.Label();
            this.labSize = new System.Windows.Forms.Label();
            this.cultureManager = new Infralution.Localization.CultureManager(this.components);
            this.openIconDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnAdvancedOptions = new System.Windows.Forms.Button();
            this.indMainExe = new System.Windows.Forms.PictureBox();
            this.indPackExePath = new System.Windows.Forms.PictureBox();
            this.indAppFolder = new System.Windows.Forms.PictureBox();
            this.btnIconReset = new System.Windows.Forms.Button();
            this.picAppIcon = new System.Windows.Forms.PictureBox();
            this.picVCRWarning = new System.Windows.Forms.PictureBox();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indMainExe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.indPackExePath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.indAppFolder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAppIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVCRWarning)).BeginInit();
            this.SuspendLayout();
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // txtAppFolderPath
            // 
            this.txtAppFolderPath.AllowDrop = true;
            resources.ApplyResources(this.txtAppFolderPath, "txtAppFolderPath");
            this.txtAppFolderPath.BackColor = System.Drawing.SystemColors.Window;
            this.txtAppFolderPath.Name = "txtAppFolderPath";
            this.txtAppFolderPath.ShortcutsEnabled = false;
            this.txtAppFolderPath.TabStop = false;
            this.txtAppFolderPath.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.txtAppFolderPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtAppFolderPath_DragDrop);
            this.txtAppFolderPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtAppFolderPath_DragEnter);
            this.txtAppFolderPath.Enter += new System.EventHandler(this.txtAppFolderPath_Enter);
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
            // txtPackExePath
            // 
            this.txtPackExePath.AllowDrop = true;
            resources.ApplyResources(this.txtPackExePath, "txtPackExePath");
            this.txtPackExePath.Name = "txtPackExePath";
            this.txtPackExePath.TextChanged += new System.EventHandler(this.txtPackPath_TextChanged);
            this.txtPackExePath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtPackPath_DragDrop);
            this.txtPackExePath.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtPackPath_DragEnter);
            this.txtPackExePath.Leave += new System.EventHandler(this.txtPackExePath_Leave);
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
            this.treeView.AllowDrop = true;
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            this.treeView.ImageList = this.treeVieIconsList;
            this.treeView.Name = "treeView";
            this.treeView.ShowRootLines = false;
            this.treeView.TabStop = false;
            this.treeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeSelect);
            this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtAppFolderPath_DragDrop);
            this.treeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtAppFolderPath_DragEnter);
            // 
            // treeVieIconsList
            // 
            this.treeVieIconsList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeVieIconsList.ImageStream")));
            this.treeVieIconsList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeVieIconsList.Images.SetKeyName(0, "folder-closed-16.png");
            this.treeVieIconsList.Images.SetKeyName(1, "folder-open-16.png");
            this.treeVieIconsList.Images.SetKeyName(2, "file-16.png");
            this.treeVieIconsList.Images.SetKeyName(3, "file-exe-16.png");
            this.treeVieIconsList.Images.SetKeyName(4, "package-16.png");
            this.treeVieIconsList.Images.SetKeyName(5, "file-lnk-16.png");
            this.treeVieIconsList.Images.SetKeyName(6, "file-bin-16.png");
            this.treeVieIconsList.Images.SetKeyName(7, "file-bat-16.png");
            this.treeVieIconsList.Images.SetKeyName(8, "file-cmd-16.png");
            this.treeVieIconsList.Images.SetKeyName(9, "box-16.png");
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
            // flagsIamgeList
            // 
            this.flagsIamgeList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("flagsIamgeList.ImageStream")));
            this.flagsIamgeList.TransparentColor = System.Drawing.Color.Transparent;
            this.flagsIamgeList.Images.SetKeyName(0, "united-kingdom-flag-icon-32.png");
            this.flagsIamgeList.Images.SetKeyName(1, "russia-flag-icon-32.png");
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.languageToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.packToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // packToolStripMenuItem
            // 
            resources.ApplyResources(this.packToolStripMenuItem, "packToolStripMenuItem");
            this.packToolStripMenuItem.Name = "packToolStripMenuItem";
            this.packToolStripMenuItem.Click += new System.EventHandler(this.btnPack_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.russianToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            resources.ApplyResources(this.languageToolStripMenuItem, "languageToolStripMenuItem");
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Image = global::Appacker.Properties.Resources.flag_great_britain_30;
            resources.ApplyResources(this.englishToolStripMenuItem, "englishToolStripMenuItem");
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // russianToolStripMenuItem
            // 
            this.russianToolStripMenuItem.Image = global::Appacker.Properties.Resources.flag_russian_federation_30;
            resources.ApplyResources(this.russianToolStripMenuItem, "russianToolStripMenuItem");
            this.russianToolStripMenuItem.Name = "russianToolStripMenuItem";
            this.russianToolStripMenuItem.Click += new System.EventHandler(this.russianToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::Appacker.Properties.Resources.info_16;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // labTreeViewCaption
            // 
            resources.ApplyResources(this.labTreeViewCaption, "labTreeViewCaption");
            this.labTreeViewCaption.Name = "labTreeViewCaption";
            // 
            // btnChangeIcon
            // 
            resources.ApplyResources(this.btnChangeIcon, "btnChangeIcon");
            this.btnChangeIcon.Name = "btnChangeIcon";
            this.btnChangeIcon.TabStop = true;
            this.btnChangeIcon.UseCompatibleTextRendering = true;
            this.btnChangeIcon.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnChangeIcon_LinkClicked);
            // 
            // comboMainExePath
            // 
            resources.ApplyResources(this.comboMainExePath, "comboMainExePath");
            this.comboMainExePath.BackColor = System.Drawing.SystemColors.Window;
            this.comboMainExePath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMainExePath.Name = "comboMainExePath";
            this.comboMainExePath.SelectedIndexChanged += new System.EventHandler(this.comboMainExePath_SelectedIndexChanged);
            // 
            // progressBar
            // 
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.Name = "progressBar";
            // 
            // labOverride
            // 
            resources.ApplyResources(this.labOverride, "labOverride");
            this.labOverride.ForeColor = System.Drawing.Color.Red;
            this.labOverride.Name = "labOverride";
            // 
            // labSize
            // 
            resources.ApplyResources(this.labSize, "labSize");
            this.labSize.Name = "labSize";
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // openIconDialog
            // 
            resources.ApplyResources(this.openIconDialog, "openIconDialog");
            // 
            // btnAdvancedOptions
            // 
            resources.ApplyResources(this.btnAdvancedOptions, "btnAdvancedOptions");
            this.btnAdvancedOptions.Image = global::Appacker.Properties.Resources.tune_16;
            this.btnAdvancedOptions.Name = "btnAdvancedOptions";
            this.btnAdvancedOptions.UseVisualStyleBackColor = true;
            this.btnAdvancedOptions.Click += new System.EventHandler(this.btnAdvancedOptions_Click);
            // 
            // indMainExe
            // 
            resources.ApplyResources(this.indMainExe, "indMainExe");
            this.indMainExe.BackColor = System.Drawing.Color.Red;
            this.indMainExe.Name = "indMainExe";
            this.indMainExe.TabStop = false;
            // 
            // indPackExePath
            // 
            this.indPackExePath.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.indPackExePath, "indPackExePath");
            this.indPackExePath.Name = "indPackExePath";
            this.indPackExePath.TabStop = false;
            // 
            // indAppFolder
            // 
            this.indAppFolder.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.indAppFolder, "indAppFolder");
            this.indAppFolder.Name = "indAppFolder";
            this.indAppFolder.TabStop = false;
            // 
            // btnIconReset
            // 
            resources.ApplyResources(this.btnIconReset, "btnIconReset");
            this.btnIconReset.BackColor = System.Drawing.Color.Transparent;
            this.btnIconReset.BackgroundImage = global::Appacker.Properties.Resources.close_icon;
            this.btnIconReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIconReset.FlatAppearance.BorderSize = 0;
            this.btnIconReset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnIconReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnIconReset.ForeColor = System.Drawing.Color.Red;
            this.btnIconReset.Name = "btnIconReset";
            this.btnIconReset.UseVisualStyleBackColor = false;
            this.btnIconReset.Click += new System.EventHandler(this.btnIconReset_Click);
            // 
            // picAppIcon
            // 
            resources.ApplyResources(this.picAppIcon, "picAppIcon");
            this.picAppIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picAppIcon.Name = "picAppIcon";
            this.picAppIcon.TabStop = false;
            this.picAppIcon.DragDrop += new System.Windows.Forms.DragEventHandler(this.picAppIcon_DragDrop);
            this.picAppIcon.DragEnter += new System.Windows.Forms.DragEventHandler(this.picAppIcon_DragEnter);
            // 
            // picVCRWarning
            // 
            resources.ApplyResources(this.picVCRWarning, "picVCRWarning");
            this.picVCRWarning.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picVCRWarning.Image = global::Appacker.Properties.Resources.red_warning_16;
            this.picVCRWarning.Name = "picVCRWarning";
            this.picVCRWarning.TabStop = false;
            this.picVCRWarning.Click += new System.EventHandler(this.picVCRWarning_Click);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picVCRWarning);
            this.Controls.Add(this.btnAdvancedOptions);
            this.Controls.Add(this.labSize);
            this.Controls.Add(this.indMainExe);
            this.Controls.Add(this.indPackExePath);
            this.Controls.Add(this.indAppFolder);
            this.Controls.Add(this.labOverride);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnIconReset);
            this.Controls.Add(this.comboMainExePath);
            this.Controls.Add(this.btnChangeIcon);
            this.Controls.Add(this.labTreeViewCaption);
            this.Controls.Add(this.picAppIcon);
            this.Controls.Add(this.btnPack);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.labMainExePath);
            this.Controls.Add(this.labPackPath);
            this.Controls.Add(this.labAppFolderPath);
            this.Controls.Add(this.btnBrowsePackPath);
            this.Controls.Add(this.btnBrowseAppFolder);
            this.Controls.Add(this.txtPackExePath);
            this.Controls.Add(this.txtAppFolderPath);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indMainExe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.indPackExePath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.indAppFolder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAppIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVCRWarning)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox txtAppFolderPath;
        private System.Windows.Forms.Button btnBrowseAppFolder;
        private System.Windows.Forms.Label labAppFolderPath;
        private System.Windows.Forms.Label labPackPath;
        private System.Windows.Forms.TextBox txtPackExePath;
        private System.Windows.Forms.Button btnBrowsePackPath;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Label labMainExePath;
        private System.Windows.Forms.Button btnPack;
        private System.Windows.Forms.PictureBox picAppIcon;
        private System.Windows.Forms.ImageList flagsIamgeList;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem packToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem russianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private Infralution.Localization.CultureManager cultureManager;
        private System.Windows.Forms.Label labTreeViewCaption;
        private System.Windows.Forms.LinkLabel btnChangeIcon;
        private System.Windows.Forms.ComboBox comboMainExePath;
        private System.Windows.Forms.Button btnIconReset;
        private System.Windows.Forms.OpenFileDialog openIconDialog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labOverride;
        private System.Windows.Forms.PictureBox indAppFolder;
        private System.Windows.Forms.PictureBox indMainExe;
        private System.Windows.Forms.PictureBox indPackExePath;
        private System.Windows.Forms.Label labSize;
        private System.Windows.Forms.Button btnAdvancedOptions;
        private System.Windows.Forms.ImageList treeVieIconsList;
        private System.Windows.Forms.PictureBox picVCRWarning;
    }
}

