Batzendev.RemoteDesktopLockPrevent
==================================

Windows service which prevents a workstation/virtual machine from being locked when an RDP (remote desktop) connection is disconnected.

You should only use this service when you really know why you need it and never on a physically accessible machine as it renders such a machine unprotected after your RDP connection is disconnected.


My use case for this service is a third party QA UI automation application which, sadly, works with SendKeys/SendWait and is used as part of CI (continuous integration) builds.

As SendKeys/SendWait does NOT work when the machine is locked, but people are allowed to connect via RDP, i have to ensure that the virtual machine stays unlocked.
