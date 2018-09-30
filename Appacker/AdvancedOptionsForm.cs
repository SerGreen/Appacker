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

        public AdvancedOptionsForm(MainForm mainForm, bool isRepackable, bool openUnpackDirectory, MainForm.UnpackDirectory unpackDirectoryType)
        {
            InitializeComponent();
            this.mainForm = mainForm;

            checkRepackable.Checked = isRepackable;
            checkOpenUnpackFolder.Checked = openUnpackDirectory;
            if (unpackDirectoryType == MainForm.UnpackDirectory.Desktop)
                comboUnpackDir.SelectedIndex = 1;
            else if (unpackDirectoryType == MainForm.UnpackDirectory.NextToPackedExe)
                comboUnpackDir.SelectedIndex = 2;
            else if (unpackDirectoryType == MainForm.UnpackDirectory.AskAtLaunch)
                comboUnpackDir.SelectedIndex = 3;
            else
                comboUnpackDir.SelectedIndex = 0;

            SetRepackDescription();
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
        }
        
        private void checkRepackable_CheckedChanged(object sender, EventArgs e) => SetRepackDescription();
    }
}
