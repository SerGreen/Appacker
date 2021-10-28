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
            // AdvancedOptionsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labIniDescription);
            this.Controls.Add(this.btnSaveIni);
            this.Controls.Add(this.txtFileDescription);
            this.Controls.Add(this.txtArguments);
            this.Controls.Add(this.checkOpenUnpackFolder);
            this.Controls.Add(this.labFileDescription);
            this.Controls.Add(this.labArguments);
            this.Controls.Add(this.labUnpackDir);
            this.Controls.Add(this.comboUnpackDir);
            this.Controls.Add(this.labFileDescriptionDescription);
            this.Controls.Add(this.labRepackableDescr);
            this.Controls.Add(this.checkRepackable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedOptionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdvancedOptionsForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}