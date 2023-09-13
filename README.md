# FlipIt
Forked from https://github.com/phaselden/FlipIt.
Flip Clock Screensaver. Inspired by [Fliqlo](http://fliqlo.com/). Fliqlo on Windows stopped working with a recent (Dec 2015?) Flash update which prompted this project. This project does NOT use Flash.

Improved and fixed some issues:
* fix the "_showSeconds" option's function and make it configurable.
* World time cities OK and the hour aligned in WorldTimesScreen.

![Screenshot](Screenshot.png)

![Screenshot](WorldTimes.png)

## Requirements

* Microsoft Windows
* .NET Framework 4.8

## Installation

To install without building with Visual Studio, copy the .scr file on the [Releases](https://github.com/phaselden/FlipIt/releases) page to:
    * C:\Windows\SysWOW64 on 64-bit Windows.
    * C:\Windows\System32 on 32-bit Windows

## Building with Visual Studio

Run in Release mode and Run as Administrator to have the build event copy the screensaver to the Windows SysWOW64 or System32 folder. Set the Command line arguments to `/s` to have the screensaver display full screen on F5/Start.

## Acknowledgements

Source code originally based on the article and code [Creating a Screen Saver with C#](http://www.harding.edu/fmccown/screensaver/screensaver.html) by Frank McCown.

This work is licensed under a [Creative Commons License](http://creativecommons.org/licenses/by-sa/2.0/).
