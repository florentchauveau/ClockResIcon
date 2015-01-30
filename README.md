# ClockResIcon
Shows the resolution of the system clock in the notification area. Tested on Windows 8.1, but should work on all NT based kernels (Vista, 7, 8, 8.1 and Windows 10 as well).

Inspired by ClockRes from sysinternals (https://technet.microsoft.com/en-us/sysinternals/bb897568.aspx).

It queries the resolution timer with `NtQueryTimerResolution` every 10 seconds.

The icon background changes from black (15 ms), to orange (< 15 ms), to dark orange (<= 5ms), to red (< 5 ms).

Click on the icon to exit the program.

Improvements welcome!
