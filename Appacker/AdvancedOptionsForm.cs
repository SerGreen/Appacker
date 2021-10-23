using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appacker
{
    public partial class AdvancedOptionsForm : Form
    {
        MainForm mainForm = null;

        public AdvancedOptionsForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;

            checkRepackable.Checked = mainForm.isRepackable;
            checkOpenUnpackFolder.Checked = mainForm.openUnpackDirectory;
            if (mainForm.unpackDirectory == MainForm.UnpackDirectory.Desktop)
                comboUnpackDir.SelectedIndex = 1;
            else if (mainForm.unpackDirectory == MainForm.UnpackDirectory.NextToPackedExe)
                comboUnpackDir.SelectedIndex = 2;
            else if (mainForm.unpackDirectory == MainForm.UnpackDirectory.AskAtLaunch)
                comboUnpackDir.SelectedIndex = 3;
            else
                comboUnpackDir.SelectedIndex = 0;

            txtArguments.Text = mainForm.launchArguments;
            txtFileDescription.Text = mainForm.customFileDescription;

            SetRepackDescription();

            if (!MainForm.vcRuntime80Installed) 
            {
                txtFileDescription.Enabled = false;
                labFileDescription.Enabled = false;
                labFileDescriptionDescription.Enabled = false;
            }
        }

        private void SetRepackDescription() => labRepackableDescr.Text = checkRepackable.Checked ? Resources.Strings.repackOnDescr : Resources.Strings.repackOffDescr;

        // Save options to the main form before closing
        private void AdvancedOptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.isRepackable = checkRepackable.Checked;
            mainForm.openUnpackDirectory = checkOpenUnpackFolder.Checked;
            mainForm.unpackDirectory = comboUnpackDir.SelectedIndex == 1
                ? MainForm.UnpackDirectory.Desktop : comboUnpackDir.SelectedIndex == 2
                ? MainForm.UnpackDirectory.NextToPackedExe : comboUnpackDir.SelectedIndex == 3
                ? MainForm.UnpackDirectory.AskAtLaunch
                : MainForm.UnpackDirectory.Temp;
            mainForm.launchArguments = txtArguments.Text;
            mainForm.customFileDescription = txtFileDescription.Text;
        }
        
        private void checkRepackable_CheckedChanged(object sender, EventArgs e) => SetRepackDescription();
    }
}
