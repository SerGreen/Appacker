using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appacker {
    public partial class VCRuntimeWarningForm : Form {
        public VCRuntimeWarningForm () {
            InitializeComponent();
        }

        private void btnOk_Click (object sender, EventArgs e) {
            Close();
        }

        private void linkLabel1_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("https://www.microsoft.com/download/details.aspx?id=26347");
        }
    }
}
