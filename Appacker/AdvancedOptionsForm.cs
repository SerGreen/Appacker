using RavSoft;
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
            txtPassword.Text = mainForm.password;
            checkWindowless.Checked = mainForm.isWindowlessUnpacker;
            checkUnpackProgressBar.Checked = mainForm.isUnpackProgressBarEnabled;

            SetRepackDescription();
            SetWindowlessDescription();
            SetCueBanners();
        }

        private void SetRepackDescription() => labRepackableDescr.Text = checkRepackable.Checked ? Resources.Strings.repackOnDescr : Resources.Strings.repackOffDescr;

        private void SetWindowlessDescription() => labWindowlessDescription.Text = checkWindowless.Checked ? Resources.Strings.windowlessOnDescr : Resources.Strings.windowlessOffDescr;

        private void SetCueBanners ()
        {
            CueProvider.SetCue(txtPassword, Resources.Strings.cuePassword);
            CueProvider.SetCue(txtFileDescription, Resources.Strings.cueFileDescription);
        }

        // Save options to the main form before closing
        private void AdvancedOptionsForm_FormClosing (object sender, FormClosingEventArgs e) => SaveSettings();

        private void SaveSettings ()
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
            mainForm.password = txtPassword.Text;
            mainForm.isWindowlessUnpacker = checkWindowless.Checked;
            mainForm.isUnpackProgressBarEnabled = checkUnpackProgressBar.Checked;
        }
        
        private void checkRepackable_CheckedChanged(object sender, EventArgs e) => SetRepackDescription();
        private void checkWindowless_CheckedChanged (object sender, EventArgs e) => SetWindowlessDescription();

        private void btnSaveIni_Click (object sender, EventArgs e)
        {
            SaveSettings();
            mainForm.SaveIniSettings();
        }

        private void btnClose_Click (object sender, EventArgs e) => this.Close();
    }
}
