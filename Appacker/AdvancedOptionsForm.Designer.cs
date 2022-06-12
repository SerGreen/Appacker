namespace Appacker
{
    partial class AdvancedOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedOptionsForm));
            this.checkRepackable = new System.Windows.Forms.CheckBox();
            this.labRepackableDescr = new System.Windows.Forms.Label();
            this.comboUnpackDir = new System.Windows.Forms.ComboBox();
            this.labUnpackDir = new System.Windows.Forms.Label();
            this.checkOpenUnpackFolder = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.txtArguments = new System.Windows.Forms.TextBox();
            this.labArguments = new System.Windows.Forms.Label();
            this.labFileDescription = new System.Windows.Forms.Label();
            this.txtFileDescription = new System.Windows.Forms.TextBox();
            this.labFileDescriptionDescription = new System.Windows.Forms.Label();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.labLocalIniDescription = new System.Windows.Forms.Label();
            this.labAppDataIniDescription = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.labLocalIniDetected = new System.Windows.Forms.Label();
            this.labAppDataIniDetected = new System.Windows.Forms.Label();
            this.labIniDescription = new System.Windows.Forms.Label();
            this.btnSaveIniAppData = new System.Windows.Forms.Button();
            this.btnSaveLocalIni = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnPassEye = new System.Windows.Forms.Button();
            this.labProgressBarDescription = new System.Windows.Forms.Label();
            this.checkUnpackProgressBar = new System.Windows.Forms.CheckBox();
            this.labWindowlessDescription = new System.Windows.Forms.Label();
            this.labPasswordDescription = new System.Windows.Forms.Label();
            this.labPassword = new System.Windows.Forms.Label();
            this.checkWindowless = new System.Windows.Forms.CheckBox();
            this.linkAppDataIniDelete = new System.Windows.Forms.LinkLabel();
            this.linkLocalIniDelete = new System.Windows.Forms.LinkLabel();
            this.panelContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkRepackable
            // 
            resources.ApplyResources(this.checkRepackable, "checkRepackable");
            this.checkRepackable.Checked = true;
            this.checkRepackable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkRepackable.Name = "checkRepackable";
            this.checkRepackable.UseVisualStyleBackColor = true;
            this.checkRepackable.CheckedChanged += new System.EventHandler(this.checkRepackable_CheckedChanged);
            // 
            // labRepackableDescr
            // 
            resources.ApplyResources(this.labRepackableDescr, "labRepackableDescr");
            this.labRepackableDescr.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labRepackableDescr.Name = "labRepackableDescr";
            // 
            // comboUnpackDir
            // 
            resources.ApplyResources(this.comboUnpackDir, "comboUnpackDir");
            this.comboUnpackDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUnpackDir.FormattingEnabled = true;
            this.comboUnpackDir.Items.AddRange(new object[] {
            resources.GetString("comboUnpackDir.Items"),
            resources.GetString("comboUnpackDir.Items1"),
            resources.GetString("comboUnpackDir.Items2"),
            resources.GetString("comboUnpackDir.Items3")});
            this.comboUnpackDir.Name = "comboUnpackDir";
            // 
            // labUnpackDir
            // 
            resources.ApplyResources(this.labUnpackDir, "labUnpackDir");
            this.labUnpackDir.Name = "labUnpackDir";
            // 
            // checkOpenUnpackFolder
            // 
            resources.ApplyResources(this.checkOpenUnpackFolder, "checkOpenUnpackFolder");
            this.checkOpenUnpackFolder.Name = "checkOpenUnpackFolder";
            this.checkOpenUnpackFolder.UseVisualStyleBackColor = true;
            // 
            // folderBrowserDialog
            // 
            resources.ApplyResources(this.folderBrowserDialog, "folderBrowserDialog");
            // 
            // txtArguments
            // 
            resources.ApplyResources(this.txtArguments, "txtArguments");
            this.txtArguments.Name = "txtArguments";
            // 
            // labArguments
            // 
            resources.ApplyResources(this.labArguments, "labArguments");
            this.labArguments.Name = "labArguments";
            // 
            // labFileDescription
            // 
            resources.ApplyResources(this.labFileDescription, "labFileDescription");
            this.labFileDescription.Name = "labFileDescription";
            // 
            // txtFileDescription
            // 
            resources.ApplyResources(this.txtFileDescription, "txtFileDescription");
            this.txtFileDescription.Name = "txtFileDescription";
            // 
            // labFileDescriptionDescription
            // 
            resources.ApplyResources(this.labFileDescriptionDescription, "labFileDescriptionDescription");
            this.labFileDescriptionDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labFileDescriptionDescription.Name = "labFileDescriptionDescription";
            // 
            // panelContainer
            // 
            resources.ApplyResources(this.panelContainer, "panelContainer");
            this.panelContainer.Controls.Add(this.labLocalIniDescription);
            this.panelContainer.Controls.Add(this.labAppDataIniDescription);
            this.panelContainer.Controls.Add(this.linkLocalIniDelete);
            this.panelContainer.Controls.Add(this.linkAppDataIniDelete);
            this.panelContainer.Controls.Add(this.btnClose);
            this.panelContainer.Controls.Add(this.labLocalIniDetected);
            this.panelContainer.Controls.Add(this.labAppDataIniDetected);
            this.panelContainer.Controls.Add(this.labIniDescription);
            this.panelContainer.Controls.Add(this.btnSaveIniAppData);
            this.panelContainer.Controls.Add(this.btnSaveLocalIni);
            this.panelContainer.Controls.Add(this.txtPassword);
            this.panelContainer.Controls.Add(this.btnPassEye);
            this.panelContainer.Controls.Add(this.labProgressBarDescription);
            this.panelContainer.Controls.Add(this.checkUnpackProgressBar);
            this.panelContainer.Controls.Add(this.labWindowlessDescription);
            this.panelContainer.Controls.Add(this.labPasswordDescription);
            this.panelContainer.Controls.Add(this.labPassword);
            this.panelContainer.Controls.Add(this.checkWindowless);
            this.panelContainer.Controls.Add(this.checkRepackable);
            this.panelContainer.Controls.Add(this.labRepackableDescr);
            this.panelContainer.Controls.Add(this.labFileDescriptionDescription);
            this.panelContainer.Controls.Add(this.txtFileDescription);
            this.panelContainer.Controls.Add(this.comboUnpackDir);
            this.panelContainer.Controls.Add(this.txtArguments);
            this.panelContainer.Controls.Add(this.labUnpackDir);
            this.panelContainer.Controls.Add(this.checkOpenUnpackFolder);
            this.panelContainer.Controls.Add(this.labArguments);
            this.panelContainer.Controls.Add(this.labFileDescription);
            this.panelContainer.Name = "panelContainer";
            // 
            // labLocalIniDescription
            // 
            resources.ApplyResources(this.labLocalIniDescription, "labLocalIniDescription");
            this.labLocalIniDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labLocalIniDescription.Name = "labLocalIniDescription";
            // 
            // labAppDataIniDescription
            // 
            resources.ApplyResources(this.labAppDataIniDescription, "labAppDataIniDescription");
            this.labAppDataIniDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labAppDataIniDescription.Name = "labAppDataIniDescription";
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // labLocalIniDetected
            // 
            resources.ApplyResources(this.labLocalIniDetected, "labLocalIniDetected");
            this.labLocalIniDetected.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labLocalIniDetected.Name = "labLocalIniDetected";
            // 
            // labAppDataIniDetected
            // 
            resources.ApplyResources(this.labAppDataIniDetected, "labAppDataIniDetected");
            this.labAppDataIniDetected.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labAppDataIniDetected.Name = "labAppDataIniDetected";
            // 
            // labIniDescription
            // 
            resources.ApplyResources(this.labIniDescription, "labIniDescription");
            this.labIniDescription.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labIniDescription.Name = "labIniDescription";
            // 
            // btnSaveIniAppData
            // 
            resources.ApplyResources(this.btnSaveIniAppData, "btnSaveIniAppData");
            this.btnSaveIniAppData.Name = "btnSaveIniAppData";
            this.btnSaveIniAppData.UseVisualStyleBackColor = true;
            this.btnSaveIniAppData.Click += new System.EventHandler(this.btnSaveIniAppData_Click);
            // 
            // btnSaveLocalIni
            // 
            resources.ApplyResources(this.btnSaveLocalIni, "btnSaveLocalIni");
            this.btnSaveLocalIni.Name = "btnSaveLocalIni";
            this.btnSaveLocalIni.UseVisualStyleBackColor = true;
            this.btnSaveLocalIni.Click += new System.EventHandler(this.btnSaveLocalIni_Click);
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // btnPassEye
            // 
            resources.ApplyResources(this.btnPassEye, "btnPassEye");
            this.btnPassEye.Image = global::Appacker.Properties.Resources.eye_14;
            this.btnPassEye.Name = "btnPassEye";
            this.btnPassEye.TabStop = false;
            this.btnPassEye.UseVisualStyleBackColor = true;
            this.btnPassEye.Click += new System.EventHandler(this.btnPassEye_Click);
            // 
            // labProgressBarDescription
            // 
            resources.ApplyResources(this.labProgressBarDescription, "labProgressBarDescription");
            this.labProgressBarDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labProgressBarDescription.Name = "labProgressBarDescription";
            // 
            // checkUnpackProgressBar
            // 
            resources.ApplyResources(this.checkUnpackProgressBar, "checkUnpackProgressBar");
            this.checkUnpackProgressBar.Checked = true;
            this.checkUnpackProgressBar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkUnpackProgressBar.Name = "checkUnpackProgressBar";
            this.checkUnpackProgressBar.UseVisualStyleBackColor = true;
            this.checkUnpackProgressBar.CheckedChanged += new System.EventHandler(this.checkUnpackProgressBar_CheckedChanged);
            // 
            // labWindowlessDescription
            // 
            resources.ApplyResources(this.labWindowlessDescription, "labWindowlessDescription");
            this.labWindowlessDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labWindowlessDescription.Name = "labWindowlessDescription";
            // 
            // labPasswordDescription
            // 
            resources.ApplyResources(this.labPasswordDescription, "labPasswordDescription");
            this.labPasswordDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labPasswordDescription.Name = "labPasswordDescription";
            // 
            // labPassword
            // 
            resources.ApplyResources(this.labPassword, "labPassword");
            this.labPassword.Name = "labPassword";
            // 
            // checkWindowless
            // 
            resources.ApplyResources(this.checkWindowless, "checkWindowless");
            this.checkWindowless.Checked = true;
            this.checkWindowless.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkWindowless.Name = "checkWindowless";
            this.checkWindowless.UseVisualStyleBackColor = true;
            this.checkWindowless.CheckedChanged += new System.EventHandler(this.checkWindowless_CheckedChanged);
            // 
            // linkAppDataIniDelete
            // 
            resources.ApplyResources(this.linkAppDataIniDelete, "linkAppDataIniDelete");
            this.linkAppDataIniDelete.Name = "linkAppDataIniDelete";
            this.linkAppDataIniDelete.TabStop = true;
            this.linkAppDataIniDelete.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAppDataIniDelete_LinkClicked);
            // 
            // linkLocalIniDelete
            // 
            resources.ApplyResources(this.linkLocalIniDelete, "linkLocalIniDelete");
            this.linkLocalIniDelete.Name = "linkLocalIniDelete";
            this.linkLocalIniDelete.TabStop = true;
            this.linkLocalIniDelete.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLocalIniDelete_LinkClicked);
            // 
            // AdvancedOptionsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedOptionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdvancedOptionsForm_FormClosing);
            this.panelContainer.ResumeLayout(false);
            this.panelContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkRepackable;
        private System.Windows.Forms.Label labRepackableDescr;
        private System.Windows.Forms.ComboBox comboUnpackDir;
        private System.Windows.Forms.Label labUnpackDir;
        private System.Windows.Forms.CheckBox checkOpenUnpackFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox txtArguments;
        private System.Windows.Forms.Label labArguments;
        private System.Windows.Forms.Label labFileDescription;
        private System.Windows.Forms.TextBox txtFileDescription;
        private System.Windows.Forms.Label labFileDescriptionDescription;
        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label labPassword;
        private System.Windows.Forms.Label labPasswordDescription;
        private System.Windows.Forms.Label labWindowlessDescription;
        private System.Windows.Forms.CheckBox checkWindowless;
        private System.Windows.Forms.Label labProgressBarDescription;
        private System.Windows.Forms.CheckBox checkUnpackProgressBar;
        private System.Windows.Forms.Button btnPassEye;
        private System.Windows.Forms.Label labLocalIniDescription;
        private System.Windows.Forms.Label labAppDataIniDescription;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label labLocalIniDetected;
        private System.Windows.Forms.Label labAppDataIniDetected;
        private System.Windows.Forms.Label labIniDescription;
        private System.Windows.Forms.Button btnSaveIniAppData;
        private System.Windows.Forms.Button btnSaveLocalIni;
        private System.Windows.Forms.LinkLabel linkLocalIniDelete;
        private System.Windows.Forms.LinkLabel linkAppDataIniDelete;
    }
}