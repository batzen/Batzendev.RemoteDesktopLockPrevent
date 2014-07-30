Batzendev.RemoteDesktopLockPrevent
==================================

Windows service which prevents a workstation/virtual machine from being locked when an RDP (remote desktop protocol) connection is disconnected.

You should only use this service when you really know why you need it and never on a physically accessible machine as it renders such a machine unprotected after your RDP connection is disconnected.

# Motivation to build this
My use case for this service is a third party QA (Quality assurance) UI (User interface) automation application which, sadly, works with SendKeys/SendWait and is used as part of CI (continuous integration) builds.

As SendKeys/SendWait does NOT work when the machine is locked, but people are allowed to connect via RDP, i have to ensure that the virtual machine stays unlocked.

# Installation instructions
- Clone the repository
- Compile
- Grab the binary
- InstallUtil.exe LOCATION_OF_BINARY\Batzendev.RemoteDesktopLockPrevent.exe

## How to find InstallUtil
> This tool is installed with the .NET Framework to the folder %WINDIR%\Microsoft.NET\Framework[64]\framework_version. For example, the default path for the 32-bit version of the .NET Framework 4, 4.5, 4.5.1, and 4.5.2 is C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe.

Taken from http://msdn.microsoft.com/en-us/library/zt39148a(v=vs.110).aspx which also describes how to uinstall the service.
