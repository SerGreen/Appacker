using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appacker
{
    public partial class MainForm : Form
    {
        private readonly CultureInfo CULTURE_RU = CultureInfo.GetCultureInfo("ru-RU");
        private readonly CultureInfo CULTURE_EN = CultureInfo.GetCultureInfo("en-US");

        public MainForm()
        {
            InitializeComponent();
            SetLanguage(Properties.Settings.Default.Language);
        }
        
        // Open folder dialog box
        private void btnBrowseAppFolder_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                SetAppFolderPath(folderBrowserDialog.SelectedPath);
            }
        }

        private void SetAppFolderPath(string path)
        {
            txtAppFolderPath.Text = path;
            RebuildTree();
            txtMainExePath.Text = string.Empty;
        }

        // Open save file dialog box
        private void btnBrowsePackPath_Click(object sender, EventArgs e)
        {
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SetPackPath(saveFileDialog.FileName);
            }
        }

        private void SetPackPath(string path)
        {
            txtPackPath.Text = path;

            // If treeView is build, make the root node name equal to name of the pack name
            if (treeView.Nodes.Count > 0)
                treeView.Nodes[0].Text = Path.GetFileName(path);
        }

        // Clear tree view and build new tree using path from txtAppFolderPath
        private void RebuildTree()
        {
            treeView.Nodes.Clear();
            BuildTree(new DirectoryInfo(txtAppFolderPath.Text), treeView.Nodes);

            if (treeView.Nodes.Count > 0)
            {
                // Set icon for root node
                var rootNode = treeView.Nodes[0];
                rootNode.ImageIndex = rootNode.ImageIndex = 4;
                rootNode.Expand();

                // If there's defined path to package save file, then set pack file name as root node name
                if (!string.IsNullOrWhiteSpace(txtPackPath.Text))
                {
                    try
                    {
                        rootNode.Text = Path.GetFileName(txtPackPath.Text);
                    }
                    catch (ArgumentException) { /* Fucked up pack save path */ }
                }
            }
        }

        // Recursively build hierarchical tree
        private void BuildTree(DirectoryInfo directoryInfo, TreeNodeCollection addInMe)
        {
            TreeNode curNode = addInMe.Add(directoryInfo.Name);
            curNode.ImageIndex = 0;
            curNode.SelectedImageIndex = 1;

            foreach (DirectoryInfo subdir in directoryInfo.GetDirectories())
            {
                BuildTree(subdir, curNode.Nodes);
            }
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                TreeNode fileNode = curNode.Nodes.Add(file.FullName, file.Name);
                if (file.Extension.ToLowerInvariant() == ".exe")
                    fileNode.ImageIndex = fileNode.SelectedImageIndex = 3;
                else
                    fileNode.ImageIndex = fileNode.SelectedImageIndex = 2;
            }
        }

        // Only alow selection of .exe files
        private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if(!e.Node.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;
                return;
            }

            txtMainExePath.Text = e.Node.FullPath.Substring(e.Node.FullPath.IndexOf(Path.DirectorySeparatorChar) + 1);
        }

        private void SetAppIconPreview()
        {
            if (string.IsNullOrWhiteSpace(txtMainExePath.Text))
                picAppIcon.Image = null;
            else
            {
                Bitmap appIcon = Icon.ExtractAssociatedIcon(Path.Combine(txtAppFolderPath.Text, txtMainExePath.Text)).ToBitmap();
                picAppIcon.Image = appIcon;
            }
        }

        // When user specified path to app directory, path to save package and local path to main exe
        // Button 'Pack' becomes active
        private void CheckIfReadyToPack()
        {
            if (string.IsNullOrWhiteSpace(txtAppFolderPath.Text) ||
                 string.IsNullOrWhiteSpace(txtMainExePath.Text) ||
                 string.IsNullOrWhiteSpace(txtPackPath.Text) ||
                 !txtPackPath.Text.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                packToolStripMenuItem.Enabled = btnPack.Enabled = false;
            else
                packToolStripMenuItem.Enabled = btnPack.Enabled = true;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            CheckIfReadyToPack();
            SetAppIconPreview();
        }

        // Launch packer.exe with needed arguments
        private void btnPack_Click(object sender, EventArgs e)
        {
            packToolStripMenuItem.Enabled = false;
            btnPack.Text = Strings.btnPackTextPacking1 + Environment.NewLine + Strings.btnPackTextPacking2;
            btnPack.Update();

            // Copy packer and unpacker into temp directory
            string tempDir = null;
            while (tempDir == null || Directory.Exists(tempDir))
                tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            File.WriteAllBytes(Path.Combine(tempDir, "packer.exe"), Resource.Packer);
            File.WriteAllBytes(Path.Combine(tempDir, "unpacker.exe"), Resource.Unpacker);

            // Inject new icon into unpacker.exe (take the icon from the main executable of unpacked app)
            IconSwapper.ChangeIcon(Path.Combine(tempDir, "unpacker.exe"), Path.Combine(txtAppFolderPath.Text, txtMainExePath.Text));

            // Launch packer.exe with arguments:
            // 1. Path to unpacker.exe
            // 2. Path where to save packed app
            // 3. Relative path to main executable inside app directory
            // 4. Path to app directory
            // 5. Whether app is self-repackable, True or False
            ProcessStartInfo packProcInfo = new ProcessStartInfo(Path.Combine(tempDir, "packer.exe"));
            packProcInfo.Arguments = $@"""{Path.Combine(tempDir, "unpacker.exe")}"" ""{txtPackPath.Text.TrimEnd(Path.DirectorySeparatorChar)}"" ""{txtMainExePath.Text}"" ""{txtAppFolderPath.Text}"" {checkRepackable.Checked}";
#if (!DEBUG)
            packProcInfo.CreateNoWindow = true;
            packProcInfo.WindowStyle = ProcessWindowStyle.Hidden;
#endif

            Process packProc = Process.Start(packProcInfo);
            packProc.WaitForExit();

            // Show error message if return code is abnormal
            if (packProc.ExitCode != 0)
                ShowPackingFailMessage(packProc.ExitCode);
            else
                System.Media.SystemSounds.Exclamation.Play();

            packProc.Dispose();
            
            // Delete temp directory
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            
            btnPack.Text = Strings.btnPackText;
            packToolStripMenuItem.Enabled = true;
        }

        // Display message box with error explanation
        private void ShowPackingFailMessage(int exitCode)
        {
            string message;
            if (exitCode >= 1 && exitCode <= 6)
                message = Strings.ResourceManager.GetString($"errorCode{exitCode}");
            else
                message = Strings.errorCodeUnknown;

            MessageBox.Show($"{Strings.errorText} 0x{exitCode:X3}.\n{message}", Strings.errorCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void txtAppFolderPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void txtAppFolderPath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);

                if (Directory.Exists(files[0]))
                {
                    SetAppFolderPath(files[0]);
                }
            }
        }

        private void txtPackPath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (File.Exists(files[0]) && files[0].EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    SetPackPath(files[0]);
                }
            }
        }
        
        private void englishToolStripMenuItem_Click(object sender, EventArgs e) => SetLanguage(CULTURE_EN);
        private void russianToolStripMenuItem_Click(object sender, EventArgs e) => SetLanguage(CULTURE_RU);
        
        private void SetLanguage(CultureInfo language)
        {
            englishToolStripMenuItem.Checked = language.Equals(CULTURE_EN);
            russianToolStripMenuItem.Checked = language.Equals(CULTURE_RU);
            cultureManager.UICulture = 
                System.Threading.Thread.CurrentThread.CurrentCulture =
                System.Threading.Thread.CurrentThread.CurrentUICulture = language;
            Properties.Settings.Default.Language = cultureManager.UICulture;
            Properties.Settings.Default.Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog(this);
        }
    }
}
