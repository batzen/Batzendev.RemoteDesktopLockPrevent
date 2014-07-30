namespace Batzendev.RemoteDesktopLockPrevent
{
    using System.ServiceProcess;

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            var servicesToRun = new ServiceBase[] 
            { 
                new RemoteDesktopLockPreventService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}