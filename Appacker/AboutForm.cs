using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appacker
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            labVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        private void linkIconsCredit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.flaticon.com/authors/good-ware");
        }

        private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/SerGreen/Appacker");
        }

        private void btnDismiss_Click(object sender, EventArgs e) => Close();
    }
}
