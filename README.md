# ClockResIcon
Shows the resolution of the system clock in the notification area. Tested on Windows 8.1 and Windows 10, but should work on all NT kernels (Vista, 7, 8, 8.1 and Windows 10).

**Very useful when you are using a laptop to detect if a program is killing your battery!**

**Download binary (x64) here:** https://github.com/florentchauveau/ClockResIcon/releases/download/v1.5-x64/ClockResIcon.exe

This blog post inspired me to create this tool: https://randomascii.wordpress.com/2013/07/08/windows-timer-resolution-megawatts-wasted/

Also inspired by ClockRes from sysinternals (https://technet.microsoft.com/en-us/sysinternals/bb897568.aspx).

It queries the resolution timer with `NtQueryTimerResolution` every 1s (default), 10s, or 30s (right click to change).

The icon background changes from black (15 ms), to orange (< 15 ms), to dark orange (< 10 ms), to red (< 5 ms).

Improvements welcome!
