using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClockResIcon
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            NotifyIcon nfIcon = new NotifyIcon();
            nfIcon.Icon = GetIcon();
            nfIcon.Visible = true;
            nfIcon.MouseClick += delegate { Application.Exit(); };
            
            Timer t = new Timer();
            t.Interval = 10000;
            t.Tick += delegate { nfIcon.Icon = GetIcon(); };
            t.Start();

            Application.Run();

            nfIcon.Visible = false;
        }

        public static Icon GetIcon()
        {
            var caps = WinApiCalls.QueryTimerResolution();
            var number = (int)(caps.PeriodCurrent / 10000.0);

            Bitmap bitmap = new Bitmap(32, 32);
            Graphics graphics = Graphics.FromImage(bitmap);

            Font font = new Font("Consolas", 16, FontStyle.Bold);
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
