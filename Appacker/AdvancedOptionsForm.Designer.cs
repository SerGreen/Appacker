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
            this.btnSaveIni = new System.Windows.Forms.Button();
            this.labIniDescription = new System.Windows.Forms.Label();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.labPasswordDescription = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.labPassword = new System.Windows.Forms.Label();
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
            // btnSaveIni
            // 
            resources.ApplyResources(this.btnSaveIni, "btnSaveIni");
            this.btnSaveIni.Name = "btnSaveIni";
            this.btnSaveIni.UseVisualStyleBackColor = true;
            this.btnSaveIni.Click += new System.EventHandler(this.btnSaveIni_Click);
            // 
            // labIniDescription
            // 
            resources.ApplyResources(this.labIniDescription, "labIniDescription");
            this.labIniDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labIniDescription.Name = "labIniDescription";
            // 
            // panelContainer
            // 
            resources.ApplyResources(this.panelContainer, "panelContainer");
            this.panelContainer.Controls.Add(this.btnClose);
            this.panelContainer.Controls.Add(this.labPasswordDescription);
            this.panelContainer.Controls.Add(this.txtPassword);
            this.panelContainer.Controls.Add(this.labPassword);
            this.panelContainer.Controls.Add(this.checkRepackable);
            this.panelContainer.Controls.Add(this.labIniDescription);
            this.panelContainer.Controls.Add(this.labRepackableDescr);
            this.panelContainer.Controls.Add(this.btnSaveIni);
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
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // labPasswordDescription
            // 
            resources.ApplyResources(this.labPasswordDescription, "labPasswordDescription");
            this.labPasswordDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labPasswordDescription.Name = "labPasswordDescription";
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // labPassword
            // 
            resources.ApplyResources(this.labPassword, "labPassword");
            this.labPassword.Name = "labPassword";
            // 
            // AdvancedOptionsForm
            // 
            this.AcceptButton = this.btnClose;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
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
        private System.Windows.Forms.Button btnSaveIni;
        private System.Windows.Forms.Label labIniDescription;
        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label labPassword;
        private System.Windows.Forms.Label labPasswordDescription;
        private System.Windows.Forms.Button btnClose;
    }
}