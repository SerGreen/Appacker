using RavSoft;
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
using XDMessaging;

namespace Appacker
{
    public partial class MainForm : Form
    {
        private readonly CultureInfo CULTURE_RU = CultureInfo.GetCultureInfo("ru-RU");
        private readonly CultureInfo CULTURE_EN = CultureInfo.GetCultureInfo("en-US");

        private string pathToCustomIcon = null;

        public MainForm()
        {
            InitializeComponent();
            // Fix btnIconReset background transparency by setting pictureBox as its Parent (and recalculate position)
            Point pos = PointToScreen(btnIconReset.Location);
            pos = picAppIcon.PointToClient(pos);
            btnIconReset.Parent = picAppIcon;
            btnIconReset.Location = pos;

            // Load language from settings
            SetLanguage(RegistrySettingsProvider.Language);
        }

        #region GUI and controls stuff
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
            UpdateComboBoxMainExe(path);
            CheckIfReadyToPack();
            UpdateEstimatedSize();
        }

        private void txtAppFolderPath_Enter(object sender, EventArgs e) => btnBrowseAppFolder.Focus();

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
            txtPackExePath.Text = path;

            // If treeView is built, make the root node name equal to name of the pack name
            if (treeView.Nodes.Count > 0)
                treeView.Nodes[0].Text = Path.GetFileName(path);
        }

        // Add .exe extension to packExePath if missing
        private void txtPackExePath_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPackExePath.Text) && !txtPackExePath.Text.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                txtPackExePath.Text = txtPackExePath.Text.TrimEnd('.') + ".exe";
            }
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
                if (!string.IsNullOrWhiteSpace(txtPackExePath.Text))
                {
                    try
                    {
                        rootNode.Text = Path.GetFileName(txtPackExePath.Text);
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
                TreeNode fileNode = curNode.Nodes.Add(file.Name);
                fileNode.Name = fileNode.FullPath.Substring(fileNode.FullPath.IndexOf(Path.DirectorySeparatorChar) + 1);
                fileNode.Tag = file.Length;
                if (file.Extension.ToLowerInvariant() == ".exe")
                    fileNode.ImageIndex = fileNode.SelectedImageIndex = 3;
                else
                    fileNode.ImageIndex = fileNode.SelectedImageIndex = 2;
            }
        }

        // Updates label that displays the size of the package
        private void UpdateEstimatedSize()
        {
            long size = CountFolderSize(treeView.TopNode);

            if (size != 0)
                size += 204800; // + 200 KiB ~= appacker wrapper & tools

            string unit;
            int power = 0;
            if (size >= 1073741824)
            {
                unit = "GiB";
                power = 3;
            }
            else if (size >= 1048576)
            {
                unit = "MiB";
                power = 2;
            }
            else if (size >= 1024)
            {
                unit = "KiB";
                power = 1;
            }
            else
                unit = "B";

            labSize.Text = $"{Resources.Strings.labSizeText} {(size / Math.Pow(1024, power)).ToString(size == 0 ? "0" : "0.00")} {unit}";
            labSize.Left = this.ClientSize.Width - labSize.Width - (9 + labSize.Margin.Right); // magic 9  (∩ ͡° ͜ʖ ͡°)⊃━☆ﾟ. *
            // If file size is more than 200 MiB, then warn user using red color
            labSize.ForeColor = size > 209715200 ? Color.Red : Color.Black;

            if (size == 0)
                labSize.Enabled = false;
            else
                labSize.Enabled = true;
        }
        // Returns the size of all files in a folder, in bytes
        private long CountFolderSize(TreeNode folder)
        {
            long size = 0;
            if (folder != null)
            {
                foreach (TreeNode node in folder.Nodes)
                {
                    if (node.Tag != null)
                        size += (long)node.Tag;
                    if (node.Nodes.Count > 0)
                        size += CountFolderSize(node);
                }
            }
            return size;
        }

        // Find all .exe files and add them to the combobox
        private void UpdateComboBoxMainExe(string pathToAppFolder)
        {
            string[] exes = Directory.GetFiles(pathToAppFolder, "*.exe", SearchOption.AllDirectories);
            comboMainExePath.Items.Clear();
            foreach (string localPath in exes.Select(x => x.Replace(pathToAppFolder, "").TrimStart(Path.DirectorySeparatorChar)))
                comboMainExePath.Items.Add(localPath);
            comboMainExePath.Enabled = true;
            SetAppIconPreviewFromMainExeIfNoCustom();
        }

        // Only alow selection of .exe files
        private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if(e.Node == treeView.SelectedNode || !e.Node.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;
                return;
            }

            comboMainExePath.SelectedItem = e.Node.Name;
        }
        
        // Change selected .exe in treeView when comboBox selection changes
        private void comboMainExePath_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeNode node = treeView.Nodes.Find(comboMainExePath.Text, true).First();
            treeView.SelectedNode = node;

            TextBox_TextChanged(sender, e);
            SetAppIconPreviewFromMainExeIfNoCustom();
        }

        // If user chose existing exe file as a save destination, then display file override warning label
        private void txtPackPath_TextChanged(object sender, EventArgs e)
        {
            TextBox_TextChanged(sender, e);
            labOverride.Visible = File.Exists(txtPackExePath.Text);
        }

        private void TextBox_TextChanged(object sender, EventArgs e) => CheckIfReadyToPack();

        // When user have specified path to app directory, path to save package and local path to main exe, button 'Pack' becomes active
        private readonly Color colOk = Color.Green;
        private readonly Color colWrong = Color.FromArgb(200, 0, 0);
        private void CheckIfReadyToPack()
        {
            bool isReady = true;

            if (string.IsNullOrWhiteSpace(txtAppFolderPath.Text))
            {
                isReady = false;
                indAppFolder.BackColor = colWrong;
            }
            else
            {
                indAppFolder.BackColor = colOk;
            }

            if (string.IsNullOrWhiteSpace(txtPackExePath.Text)
                || !txtPackExePath.Text.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                isReady = false;
                indPackExePath.BackColor = colWrong;
            }
            else
            {
                indPackExePath.BackColor = colOk;
            }

            if (string.IsNullOrWhiteSpace(comboMainExePath.Text))
            {
                isReady = false;
                indMainExe.BackColor = colWrong;
            }
            else
            {
                indMainExe.BackColor = colOk;
            }

            packToolStripMenuItem.Enabled = btnPack.Enabled = isReady;
        }
        #endregion

        // ============== PACK METHOD ================
        // Launch the packer.exe with needed arguments
        // ===========================================
        private void btnPack_Click(object sender, EventArgs e)
        {
            btnPack.Enabled = packToolStripMenuItem.Enabled = false;
            btnPack.Text = Resources.Strings.btnPackTextPacking1 + Environment.NewLine + Resources.Strings.btnPackTextPacking2;
            btnPack.Update();

            string sourceAppFolder = txtAppFolderPath.Text;
            string mainExePath = comboMainExePath.Text;
            string destinationPath = txtPackExePath.Text;
            string customIconPath = pathToCustomIcon;
            bool selfRepackable = checkRepackable.Checked;

            PackingProgressUpdate += (o, progress) =>
            {
                progressBar.Maximum = progress.maxValue;
                progressBar.Value = progress.currentValue;
            };
            PackingFinished += (o, exitCode) => 
            {
                progressBar.Value = progressBar.Maximum;
                
                // Show error message if return code is abnormal
                if (exitCode != 0)
                    ShowPackingFailMessage(exitCode);
                else
                    System.Media.SystemSounds.Exclamation.Play();

                btnPack.Text = Resources.Strings.btnPackText;
                btnPack.Enabled = packToolStripMenuItem.Enabled = true;
            };

            progressBar.Value = 0;
            StartPacking(sourceAppFolder, mainExePath, destinationPath, customIconPath, selfRepackable);
        }

        internal static event EventHandler<(int maxValue, int currentValue)> PackingProgressUpdate;
        internal static event EventHandler<int> PackingFinished;
        /// <summary>
        /// Initiates packing process in the background thread. Subscribe to PackingProgressUpdate and PackingFinished for callbacks
        /// </summary>
        /// <param name="sourceAppFolder">Path to the folder with the target application</param>
        /// <param name="mainExePath">Local path to the main executable of the target app</param>
        /// <param name="destinationPath">Save location of the resulting packed .exe</param>
        /// <param name="customIconPath">Path to the icon file that will replace original app's icon</param>
        /// <param name="selfRepackable">True = packed app will repack itself after its execution so any changes are saved; Flase = changes are discarded</param>
        internal static void StartPacking( string sourceAppFolder, 
                                                 string mainExePath, 
                                                 string destinationPath, 
                                                 string customIconPath = null, 
                                                 bool   selfRepackable = true )
        {
            // Copy packer and unpacker into the temp directory
            string tempDir = null;
            while (tempDir == null || Directory.Exists(tempDir))
                tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            File.WriteAllBytes(Path.Combine(tempDir, "packer.exe"), ToolsStorage.Packer);
            File.WriteAllBytes(Path.Combine(tempDir, "unpacker.exe"), ToolsStorage.Unpacker);
            File.WriteAllBytes(Path.Combine(tempDir, "progressBarSplash.exe"), ToolsStorage.ProgressBarSplash);

            // Inject new icon into unpacker.exe (take the icon from the main executable of unpacked app if user did not provide a custom icon)
            string iconPath = customIconPath ?? Path.Combine(sourceAppFolder, mainExePath);
            IconSwapper.ChangeIcon(Path.Combine(tempDir, "unpacker.exe"), iconPath);

            // Launch packer.exe with arguments:
            // 1. Path to unpacker.exe
            // 2. Path where to save packed app
            // 3. Relative path to main executable inside app directory
            // 4. Path to app directory
            // 5. Whether app is self-repackable, True or False
            ProcessStartInfo packProcInfo = new ProcessStartInfo(Path.Combine(tempDir, "packer.exe"));
            packProcInfo.Arguments = $@"""{Path.Combine(tempDir, "unpacker.exe")}"" ""{destinationPath.TrimEnd(Path.DirectorySeparatorChar)}"" ""{mainExePath}"" ""{sourceAppFolder}"" {selfRepackable}";
#if (!DEBUG)
            packProcInfo.CreateNoWindow = true;
            packProcInfo.WindowStyle = ProcessWindowStyle.Hidden;
#endif

            Process packProc = new Process();
            packProc.StartInfo = packProcInfo;

            // Setup XDMessagingClient listener to receive packing progress updates
            XDMessagingClient client = new XDMessagingClient();
            IXDListener listener = client.Listeners.GetListenerForMode(XDTransportMode.HighPerformanceUI);
            listener.RegisterChannel("AppackerProgress");

            // Attach event handler for incoming messages
            listener.MessageReceived += (o, ea) =>
            {
                if (ea.DataGram.Channel == "AppackerProgress")
                {
                    // 'Done' is sent by Packer when it finished packing and is ready to quit
                    if (ea.DataGram.Message == "Done")
                    {
                        packProc.WaitForExit();
                        PackingFinished.Invoke(null, packProc.ExitCode);
                        packProc.Dispose();

                        // Delete temp directory
                        if (Directory.Exists(tempDir))
                            Directory.Delete(tempDir, true);

                        listener.UnRegisterChannel("AppackerProgress");
                        listener.Dispose();
                    }
                    else
                    {
                        string[] tokens = ea.DataGram.Message.Split(' ');
                        PackingProgressUpdate.Invoke(null, (int.Parse(tokens[1]), int.Parse(tokens[0])));
                    }
                }
            };

            packProc.Start();
        }

        // Display message box with an error explanation
        private void ShowPackingFailMessage(int exitCode)
        {
            string message = GetErrorMessage(exitCode);
            MessageBox.Show($"{Resources.Strings.errorText} 0x{exitCode:X3}.\n{message}", Resources.Strings.errorCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Translates numeric exit code to the human-readable message
        /// </summary>
        /// <param name="exitCode">The return code of the packer process from PackingFinished event</param>
        internal static string GetErrorMessage(int exitCode)
        {
            if (exitCode >= 1 && exitCode <= 6)
                return Resources.Strings.ResourceManager.GetString($"errorCode{exitCode}");
            else
                return Resources.Strings.errorCodeUnknown;
        }

        #region == Drag and drop stuff ==
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
        #endregion

        #region == Language stuff ==
        // Change the CultureInfo and save the language choice to the registry
        private void SetLanguage(CultureInfo language)
        {
            englishToolStripMenuItem.Checked = language.Equals(CULTURE_EN);
            russianToolStripMenuItem.Checked = language.Equals(CULTURE_RU);
            cultureManager.UICulture =
                System.Threading.Thread.CurrentThread.CurrentCulture =
                System.Threading.Thread.CurrentThread.CurrentUICulture = language;
            RegistrySettingsProvider.Language = cultureManager.UICulture;
            CheckIfReadyToPack();
            SetCueBanners();
            UpdateEstimatedSize();
            CrunchFixControlsVisibility();
        }
        
        // Changing language resets Enabled and Visible parameters of controls to default values
        // So this is rather lazy fix by manually re-checking specific controlls
        private void CrunchFixControlsVisibility()
        {
            if (!string.IsNullOrWhiteSpace(txtAppFolderPath.Text))
                comboMainExePath.Enabled = true;
            if (!string.IsNullOrWhiteSpace(pathToCustomIcon))
                btnIconReset.Visible = true;
            if (!string.IsNullOrWhiteSpace(txtPackExePath.Text) && File.Exists(txtPackExePath.Text))
                labOverride.Visible = true;
        }

        private void SetCueBanners()
        {
            CueProvider.SetCue(txtAppFolderPath, Resources.Strings.cueAppDirPath);
            CueProvider.SetCue(txtPackExePath, Resources.Strings.cuePackExePath);
        }
        #endregion

        #region == Menu strip items stuff ==
        private void englishToolStripMenuItem_Click(object sender, EventArgs e) => SetLanguage(CULTURE_EN);
        private void russianToolStripMenuItem_Click(object sender, EventArgs e) => SetLanguage(CULTURE_RU);

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog(this);
        }
        #endregion

        #region == Package icon stuff ==
        // Displays the icon of the main executable in pictureBox
        private void SetAppIconPreviewFromMainExe()
        {
            if (string.IsNullOrWhiteSpace(comboMainExePath.Text))
                picAppIcon.Image = null;
            else
            {
                Bitmap appIcon = IconSwapper.GetIconFromFile(Path.Combine(txtAppFolderPath.Text, comboMainExePath.Text));
                picAppIcon.Image = appIcon;
            }
        }

        private void SetAppIconPreviewFromMainExeIfNoCustom()
        {
            if (pathToCustomIcon == null)
                SetAppIconPreviewFromMainExe();
        }

        // Displays the custom icon in pictureBox
        private void btnChangeIcon_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(openIconDialog.ShowDialog() == DialogResult.OK)
            {
                pathToCustomIcon = openIconDialog.FileName;
                Bitmap icon = IconSwapper.GetIconFromFile(pathToCustomIcon);
                picAppIcon.Image = icon;
                btnIconReset.Visible = true;
            }
        }

        // Resets icon in pictureBox to the main executable icon
        private void btnIconReset_Click(object sender, EventArgs e)
        {
            pathToCustomIcon = null;
            SetAppIconPreviewFromMainExe();
            btnIconReset.Visible = false;
        }
        #endregion
    }
}
