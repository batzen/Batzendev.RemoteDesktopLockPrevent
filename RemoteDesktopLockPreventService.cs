namespace Batzendev.RemoteDesktopLockPrevent
{
    using System;
    using System.Diagnostics;
    using System.ServiceProcess;
    using System.IO;

    public partial class RemoteDesktopLockPreventService : ServiceBase
    {
        public RemoteDesktopLockPreventService()
        {
            this.InitializeComponent();

            this.CanHandleSessionChangeEvent = true;
        }

        protected override void OnStart(string[] args)
        {
                    
        }

        protected override void OnStop()
        {   
        }

        /// <summary>
        /// Executes when a change event is received from a Terminal Server session. 
        /// </summary>
        /// <param name="changeDescription">A <see cref="T:System.ServiceProcess.SessionChangeDescription"/> structure that identifies the change type.</param>
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            if (changeDescription.Reason == SessionChangeReason.RemoteDisconnect)
            {
                try
                {
                    TryTsCon();
                }
                catch
                {
                }
            }

            base.OnSessionChange(changeDescription);
        }

        private static void TryTsCon()
        {
            Console.WriteLine("Trying TsCon...");

            var system = Environment.ExpandEnvironmentVariables(@"%systemroot%\Sysnative");

            for (var i = 0; i < 10; i++)
            {
                Process.Start(Path.Combine(system, "tscon.exe"), string.Format("{0} /dest:console", i));
            }
        }
    }
}