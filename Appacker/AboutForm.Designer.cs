namespace Appacker
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.btnDismiss = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.linkIconsCredit = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labVersion = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.linkGithub = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.linkIconsCredit2 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDismiss
            // 
            resources.ApplyResources(this.btnDismiss, "btnDismiss");
            this.btnDismiss.Name = "btnDismiss";
            this.btnDismiss.UseVisualStyleBackColor = true;
            this.btnDismiss.Click += new System.EventHandler(this.btnDismiss_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // linkIconsCredit
            // 
            resources.ApplyResources(this.linkIconsCredit, "linkIconsCredit");
            this.linkIconsCredit.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkIconsCredit.Name = "linkIconsCredit";
            this.linkIconsCredit.TabStop = true;
            this.linkIconsCredit.VisitedLinkColor = System.Drawing.Color.Indigo;
            this.linkIconsCredit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkIconsCredit_LinkClicked);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::Appacker.Properties.Resources.open_box_icon;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // labVersion
            // 
            resources.ApplyResources(this.labVersion, "labVersion");
            this.labVersion.Name = "labVersion";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // linkGithub
            // 
            resources.ApplyResources(this.linkGithub, "linkGithub");
            this.linkGithub.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkGithub.Name = "linkGithub";
            this.linkGithub.TabStop = true;
            this.linkGithub.VisitedLinkColor = System.Drawing.Color.Indigo;
            this.linkGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGithub_LinkClicked);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // linkIconsCredit2
            // 
            resources.ApplyResources(this.linkIconsCredit2, "linkIconsCredit2");
            this.linkIconsCredit2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkIconsCredit2.Name = "linkIconsCredit2";
            this.linkIconsCredit2.TabStop = true;
            this.linkIconsCredit2.VisitedLinkColor = System.Drawing.Color.Indigo;
            this.linkIconsCredit2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkIconsCredit_LinkClicked);
            // 
            // AboutForm
            // 
            this.AcceptButton = this.btnDismiss;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.linkGithub);
            this.Controls.Add(this.linkIconsCredit2);
            this.Controls.Add(this.linkIconsCredit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labVersion);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDismiss);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDismiss;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkIconsCredit;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labVersion;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.LinkLabel linkGithub;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel linkIconsCredit2;
    }
}