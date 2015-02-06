using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClockResIcon
{
    static class Program
    {
        private static Timer timer = new Timer();
        private static int[] timers = new int[] { 1, 10, 30 };
        private static NotifyIcon nfIcon = new NotifyIcon();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            nfIcon.Icon = GetIcon();
            nfIcon.Visible = true;
            nfIcon.ContextMenuStrip = new ContextMenuStrip();
            foreach (int interval in timers)
            {
                ToolStripMenuItem item = new ToolStripMenuItem("Timer: " + interval.ToString() + "s");
                item.Name = "timer";
                item.Click += TimerItemClick;
                item.Tag = interval;
                nfIcon.ContextMenuStrip.Items.Add(item);
            }
            nfIcon.ContextMenuStrip.Items.Add("-");
            nfIcon.ContextMenuStrip.Items.Add("Exit", null, delegate { Application.Exit(); });

            (nfIcon.ContextMenuStrip.Items[0] as ToolStripMenuItem).PerformClick();

            timer.Tick += delegate { nfIcon.Icon = GetIcon(); };

            Application.Run();

            nfIcon.Visible = false;
        }

        private static void TimerItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            timer.Interval = (int) item.Tag * 1000;
            foreach (ToolStripItem i in nfIcon.ContextMenuStrip.Items)
            {
                if (i.Name == "timer") (i as ToolStripMenuItem).Checked = false;
            }
            item.Checked = true;
            if (!timer.Enabled) timer.Start();
        }

        public static Icon GetIcon()
        {
            var caps = WinApiCalls.QueryTimerResolution();
            var number = (int)(caps.PeriodCurrent / 10000.0);

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

            fill.Dispose();
            font.Dispose();
            graphics.Dispose();
            bitmap.Dispose();

            return createdIcon;
        }
    }
}
