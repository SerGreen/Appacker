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
            SetProgressBarDescription();
            SetCueBanners();
            UpdateSettingsFileDescriptions();
        }

        private void SetRepackDescription() => labRepackableDescr.Text = checkRepackable.Checked ? Resources.Strings.repackOnDescr : Resources.Strings.repackOffDescr;
        private void SetWindowlessDescription() => labWindowlessDescription.Text = checkWindowless.Checked ? Resources.Strings.windowlessOnDescr : Resources.Strings.windowlessOffDescr;
        private void SetProgressBarDescription() => labProgressBarDescription.Text = checkUnpackProgressBar.Checked ? Resources.Strings.unpackProgressBarOnDescr : Resources.Strings.unpackProgressBarOffDescr;

        private void SetCueBanners ()
        {
            CueProvider.SetCue(txtPassword, Resources.Strings.cuePassword);
            CueProvider.SetCue(txtFileDescription, Resources.Strings.cueFileDescription);
            CueProvider.SetCue(txtFileDescription, Resources.Strings.cueFileDescription);
        }

        private void UpdateSettingsFileDescriptions()
        {
            if (IniSettingsProvider.isLocalIniFilePresent)
                labIniDescription.Text = Resources.Strings.iniDescriptionLocal;
            else if (IniSettingsProvider.isAppDataIniFilePresent)
                labIniDescription.Text = Resources.Strings.iniDescriptionAppData;
            else
                labIniDescription.Text = Resources.Strings.iniDescriptionDefault;

            labAppDataIniDetected.Visible = linkAppDataIniDelete.Visible = IniSettingsProvider.isAppDataIniFilePresent;
            btnSaveIniAppData.Enabled = labAppDataIniDescription.Visible = !IniSettingsProvider.isAppDataIniFilePresent;
            labLocalIniDetected.Visible = linkLocalIniDelete.Visible = IniSettingsProvider.isLocalIniFilePresent;
            btnSaveLocalIni.Enabled = labLocalIniDescription.Visible = !IniSettingsProvider.isLocalIniFilePresent;
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
        private void checkUnpackProgressBar_CheckedChanged (object sender, EventArgs e) => SetProgressBarDescription();

        private void btnSaveLocalIni_Click (object sender, EventArgs e)
        {
            SaveSettings();
            mainForm.SaveLocalIniSettings();
            UpdateSettingsFileDescriptions();
        }

        private void btnSaveIniAppData_Click (object sender, EventArgs e)
        {
            SaveSettings();
            mainForm.SaveAppDataIniSettings();
            UpdateSettingsFileDescriptions();
        }

        private void btnClose_Click (object sender, EventArgs e) => this.Close();

        private void btnPassEye_Click (object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            SetCueBanners();
            btnPassEye.Image = txtPassword.UseSystemPasswordChar ? Properties.Resources.eye_14 : Properties.Resources.eye_red_14;
        }

        private void linkAppDataIniDelete_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e)
        {
            IniSettingsProvider.DeleteIniFile(IniSettingsProvider.IniLocationFlags.AppData);
            UpdateSettingsFileDescriptions();
        }

        private void linkLocalIniDelete_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e)
        {
            IniSettingsProvider.DeleteIniFile(IniSettingsProvider.IniLocationFlags.Local);
            UpdateSettingsFileDescriptions();
        }
    }
}
