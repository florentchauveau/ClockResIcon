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
            nfIcon.ContextMenuStrip = CreateContextMenu();
            nfIcon.Icon = GetIcon();
            nfIcon.Visible = true;
            /* Invoke a 'click' on the first item of the context menu */
            (nfIcon.ContextMenuStrip.Items[0] as ToolStripMenuItem).PerformClick();

            /* Timer tick: update nfIcon */
            timer.Tick += delegate {
                try
                {
                    nfIcon.Icon = GetIcon();
                }
                catch (Exception)
                {
                    /* occasionally, there is a GDI+ exception. For now, we ignore it. */
        }
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

        public Icon GetIcon()
        {
            /* Query the timer resolution from Windows API */
            TimerCaps caps = WinApiCalls.QueryTimerResolution();
            int number = (int)(caps.PeriodCurrent / 10000.0);

            /* Icon creation */
            Bitmap bitmap = new Bitmap(32, 32);
            Graphics graphics = Graphics.FromImage(bitmap);

            Font font = new Font("Consolas", 17, FontStyle.Bold);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            SolidBrush fill = new SolidBrush(Color.Black);
            if (number < 5)
            {
                fill.Color = Color.Red;
            } else if (number < 10)
            {
                fill.Color = Color.DarkOrange;
            } else if (number < 15)
            {
                fill.Color = Color.Orange;
            }

            graphics.FillRectangle(fill, 0, 0, bitmap.Height, bitmap.Width);
            graphics.DrawString(number.ToString(), font, new SolidBrush(Color.White), 16, 2, format);
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
