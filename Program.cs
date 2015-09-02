using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClockResIcon
{
    class Program
    {
        /* Refresh icon timer */
        private Timer timer = new Timer();
        /* Refresh timers (context menu) */
        private int[] refreshTimers = new int[] { 1, 10, 30 };
        /* Notification area icon */
        private NotifyIcon nfIcon = new NotifyIcon();

        public Program()
        {
            int resolution = GetTimerResolution();
            int lastTimerResolution = resolution;

            nfIcon.ContextMenuStrip = CreateContextMenu();
            nfIcon.Icon = GetIcon(resolution);
            nfIcon.Visible = true;
            /* Invoke a 'click' on the first item of the context menu */
            (nfIcon.ContextMenuStrip.Items[0] as ToolStripMenuItem).PerformClick();

            /* Timer tick: update nfIcon */
            timer.Tick += delegate
            {
                try
                {
                    resolution = GetTimerResolution();

                    /* Update icon on timer change */
                    if (resolution != lastTimerResolution)
                    {
                        /* Destroy the previous Icon Handle to avoid a Handle leak */
                        WinApiCalls.DestroyIcon(nfIcon.Icon.Handle);
                        /* Update icon */
                        nfIcon.Icon = GetIcon(resolution);
                        /* Save last timer */
                        lastTimerResolution = resolution;
                    }
                }
                catch (Exception e)
                {
                    nfIcon.ShowBalloonTip(5000, "Error", e.ToString(), ToolTipIcon.Error);
                };
            };
        }

        public void Run()
        {
            Application.Run();
            /* When application is done, hide nfIcon */
            nfIcon.Visible = false;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /* Run baby run! */
            new Program().Run();
        }

        /* Create the icon context menu */
        private ContextMenuStrip CreateContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            foreach (int interval in refreshTimers)
            {
                ToolStripMenuItem item = new ToolStripMenuItem("Refresh: " + interval.ToString() + "s");
                item.Name = "refresh";
                item.Click += RefreshItemClick;
                item.Tag = interval;
                menu.Items.Add(item);
            }
            menu.Items.Add("-");
            menu.Items.Add("Exit", null, delegate { Application.Exit(); });
            return menu;
        }

        private void RefreshItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            /* Update the timer interval */
            timer.Interval = (int) item.Tag * 1000;
            /* Uncheck menu items */
            foreach (ToolStripItem i in nfIcon.ContextMenuStrip.Items)
            {
                if (i.Name == "refresh") (i as ToolStripMenuItem).Checked = false;
            }
            /* Check me! */
            item.Checked = true;
            /* Start timer */
            if (!timer.Enabled) timer.Start();
        }

        private int GetTimerResolution()
        {
            /* Query the timer resolution from Windows API */
            TimerCaps caps = WinApiCalls.QueryTimerResolution();
            return (int)(caps.PeriodCurrent / 10000.0);
        }

        private Icon GetIcon(int resolution)
        {
            /* Icon creation */
            Bitmap bitmap = new Bitmap(32, 32);
            Graphics graphics = Graphics.FromImage(bitmap);

            Font font = new Font("Segoe UI", 18, FontStyle.Regular);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            SolidBrush fill = new SolidBrush(Color.Black);

            if (resolution < 5)
            {
                fill.Color = Color.Red;
                font = new Font("Segoe UI", 18, FontStyle.Bold);
            }
            else if (resolution < 10)
            {
                fill.Color = Color.DarkOrange;
                font = new Font("Segoe UI", 18, FontStyle.Bold);
            }
            else if (resolution < 15)
            {
                fill.Color = Color.Orange;
            }

            graphics.FillRectangle(fill, 0, 0, bitmap.Height, bitmap.Width);
            graphics.DrawString(resolution.ToString(), font, new SolidBrush(Color.White), 16, -2, format);
            Icon createdIcon = Icon.FromHandle(bitmap.GetHicon());

            /* Purge */
            fill.Dispose();
            font.Dispose();
            format.Dispose();
            graphics.Dispose();
            bitmap.Dispose();

            /* Icon */
            return createdIcon;
        }
    }
}
