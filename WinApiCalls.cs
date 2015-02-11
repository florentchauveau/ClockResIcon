/*
 * This is based on https://github.com/tebjan/TimerTool (Tebjan Halm)
 * Thank you :)
 */

using System;
using System.Runtime.InteropServices;


namespace ClockResIcon
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TimerCaps
    {
        public uint PeriodMin;
        public uint PeriodMax;
        public uint PeriodCurrent;
    };

    /// <summary>
    /// Description of WinApiCalls.
    /// </summary>
    public static class WinApiCalls
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtQueryTimerResolution(out uint MinimumResolution, out uint MaximumResolution, out uint ActualResolution);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool DestroyIcon(IntPtr handle);

        public static TimerCaps QueryTimerResolution()
        {
            var caps = new TimerCaps();
            NtQueryTimerResolution(out caps.PeriodMin, out caps.PeriodMax, out caps.PeriodCurrent);
            return caps;
        }
    }
}
