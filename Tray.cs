using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AGQRecorder0
{
    public partial class MainWindow : Window
    {
        public System.Windows.Forms.NotifyIcon tray = new System.Windows.Forms.NotifyIcon();

        private void initTray()
        {
            tray.Visible = true;

            tray.Icon = AGQRecorder0.Properties.Resources.favicon;
            tray.Text = "AGQRecorder0";

            System.Windows.Forms.ContextMenuStrip ctxt = new System.Windows.Forms.ContextMenuStrip();
            System.Windows.Forms.ToolStripMenuItem cshutdown = new System.Windows.Forms.ToolStripMenuItem("Close");

            tray.DoubleClick += tray_DoubleClick;
            cshutdown.Click += cshutdown_Click;

            ctxt.Items.Add(cshutdown);
            tray.ContextMenuStrip = ctxt;

        }

        void cshutdown_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void tray_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized) {
                this.Hide();
            }

            base.OnStateChanged(e);
        }
    }
}
