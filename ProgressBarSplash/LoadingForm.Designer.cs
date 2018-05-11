namespace ProgressBarSplash
{
    partial class LoadingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingForm));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labPacking = new System.Windows.Forms.Label();
            this.labUnpacking = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.Name = "progressBar";
            this.progressBar.UseWaitCursor = true;
            // 
            // labPacking
            // 
            resources.ApplyResources(this.labPacking, "labPacking");
            this.labPacking.BackColor = System.Drawing.Color.Transparent;
            this.labPacking.Name = "labPacking";
            this.labPacking.UseWaitCursor = true;
            // 
            // labUnpacking
            // 
            resources.ApplyResources(this.labUnpacking, "labUnpacking");
            this.labUnpacking.BackColor = System.Drawing.Color.Transparent;
            this.labUnpacking.Name = "labUnpacking";
            this.labUnpacking.UseWaitCursor = true;
            // 
            // LoadingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.labUnpacking);
            this.Controls.Add(this.labPacking);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoadingForm";
            this.UseWaitCursor = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labPacking;
        private System.Windows.Forms.Label labUnpacking;
    }
}

