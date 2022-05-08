using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Unpacker
{
    public partial class PasswordForm : Form
    {
        private byte[] hash;
        private Timer shakeTimer = new Timer();
        private int shakeHeight = 3; // height in pixels that labWrongPassword jumps up on wrong password entry
        private int shakeTime = 35; //ms
        private int wrongTriesCount = 0;

        public PasswordForm (byte[] hash)
        {
            InitializeComponent();
            this.hash = hash;
            shakeTimer.Interval = shakeTime;
            shakeTimer.Tick += (s, e) => { 
                labWrongPassword.Location = new Point(labWrongPassword.Location.X, labWrongPassword.Location.Y + shakeHeight);
                shakeTimer.Stop();
            };
        }

        private void txtPassword_KeyDown (object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnOk.PerformClick();
            else if (e.KeyCode == Keys.Escape)
                btnCancel.PerformClick();
        }

        private void btnOk_Click (object sender, EventArgs e)
        {
            if (shakeTimer.Enabled)
                return;

            if (Appacker.Password.ComparePassword(txtPassword.Text, hash))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                txtPassword.Text = "";
                wrongTriesCount++;
                if (wrongTriesCount == 5)
                    labWrongPassword.Text = labStopIt.Text;
                else if (wrongTriesCount > 5 && (wrongTriesCount - 5) % 3 == 0)
                    labWrongPassword.Text += "!";

                if (wrongTriesCount < 5)
                    System.Media.SystemSounds.Exclamation.Play();
                else
                    System.Media.SystemSounds.Hand.Play();

                labWrongPassword.Visible = true;
                labWrongPassword.Location = new Point(labWrongPassword.Location.X, labWrongPassword.Location.Y - shakeHeight);
                shakeTimer.Start();
            }
        }

        private void btnCancel_Click (object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
