using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XDMessaging;

namespace ProgressBarSplash
{
    public partial class LoadingForm : Form
    {
        private IXDListener listener;

        public LoadingForm()
        {
            InitializeComponent();

            // Setup XDMessagingClient listener to receive progress updates
            XDMessagingClient client = new XDMessagingClient();
            listener = client.Listeners.GetListenerForMode(XDTransportMode.HighPerformanceUI);
            listener.RegisterChannel("AppackerProgress");
            
            // Attach event handler for incoming messages
            listener.MessageReceived += (o, ea) =>
            {
                if (ea.DataGram.Channel == "AppackerProgress")
                {
                    // 'Done' is sent when packing/unpacking is finished => close this splash
                    if (ea.DataGram.Message == "Done")
                    {
                        progressBar.Value = progressBar.Maximum;
                        Exit();
                    }
                    // Other messages are progress updates
                    else
                    {
                        string[] tokens = ea.DataGram.Message.Split(' ');
                        progressBar.Maximum = int.Parse(tokens[1]);
                        progressBar.Value = int.Parse(tokens[0]);
                    }
                }
            };
        }

        private void Exit()
        {
            listener?.UnRegisterChannel("AppackerProgress");
            listener?.Dispose();
            Application.Exit();
        }
    }
}
