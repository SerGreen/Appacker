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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // Open folder dialog box
        private void btnBrowseAppFolder_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtAppFolderPath.Text = folderBrowserDialog.SelectedPath;
                RebuildTree();
                txtMainExePath.Text = string.Empty;
            }

            CheckIfReadyToPack();
        }

        // Open save file dialog box
        private void btnBrowsePackPath_Click(object sender, EventArgs e)
        {
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtPackPath.Text = saveFileDialog.FileName;

                // If treeView is build, make the root node name equal to name of the pack name
                if (treeView.Nodes.Count > 0)
                    treeView.Nodes[0].Text = Path.GetFileName(saveFileDialog.FileName);
            }

            CheckIfReadyToPack();
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
            CheckIfReadyToPack();
        }

        // When user specified path to app directory, path to save package and local path to main exe
        // Button 'Pack' becomes active
        private void CheckIfReadyToPack()
        {
            if (string.IsNullOrWhiteSpace(txtAppFolderPath.Text) ||
                string.IsNullOrWhiteSpace(txtMainExePath.Text) ||
                string.IsNullOrWhiteSpace(txtPackPath.Text))
                btnPack.Enabled = false;
            else
                btnPack.Enabled = true;
        }

        // Launch packer.exe with needed arguments
        private void btnPack_Click(object sender, EventArgs e)
        {
            // TODO save unpacker and packer to disk, run packer with arguments, clean up
        }
    }
}
