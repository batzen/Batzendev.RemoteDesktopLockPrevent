namespace Batzendev.RemoteDesktopLockPrevent
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.ServiceProcess;
    using System.Threading;
    using System.Threading.Tasks;

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
            this.EventLog.WriteEntry(string.Format("SessionChange for session \"{0}\" with reason \"{1}\".", changeDescription.SessionId, changeDescription.Reason));

            if (changeDescription.Reason == SessionChangeReason.RemoteDisconnect)
            {
                this.TryTransferSessionToConsole(changeDescription.SessionId);
            }

            base.OnSessionChange(changeDescription);
        }

        private void TryTransferSessionToConsole(int sessionId)
        {
            this.EventLog.WriteEntry(string.Format("Trying to disconnect session \"{0}\"...", sessionId));

            try
            {
                Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    this.TransferSessionToConsole(sessionId);
                }).ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {
                            this.EventLog.WriteEntry(t.Exception.ToString(), EventLogEntryType.Error);
                        }
                        else
                        {
                            this.EventLog.WriteEntry("Unkown error in task.", EventLogEntryType.Error);
                        }
                    }, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception exception)
            {
                this.EventLog.WriteEntry(exception.ToString(), EventLogEntryType.Error);
            }
        }

        private void TransferSessionToConsole(int sessionId)
        {
            var system = Environment.ExpandEnvironmentVariables(@"%systemroot%\Sysnative");
            var startInfo = new ProcessStartInfo(Path.Combine(system, "tscon.exe"), string.Format("{0} /dest:console /v", sessionId))
            {
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            using (var process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    if (process.WaitForExit((int)TimeSpan.FromSeconds(10).TotalMilliseconds))
                    {
                        if (process.ExitCode == 0)
                        {
                            this.EventLog.WriteEntry(string.Format("Disconnected session \"{0}\".", sessionId));
                        }
                        else
                        {
                            var message =
                                string.Format(
                                    "Unable to disconnect session \"{0}\". ExitCode: {1}\r\nOutput:\r\n{2}\r\nErrors:\r\n{3}",
                                    sessionId, process.ExitCode, process.StandardOutput.ReadToEnd(), process.StandardError.ReadToEnd());
                            this.EventLog.WriteEntry(message, EventLogEntryType.Warning);
                        }
                    }
                    else
                    {
                        var message =
                            string.Format(
                                "Unable to disconnect session \"{0}\". Timeout while waiting for process. \r\nOutput:\r\n{1}\r\nErrors:\r\n{2}",
                                sessionId, process.StandardOutput.ReadToEnd(), process.StandardError.ReadToEnd());
                        this.EventLog.WriteEntry(message, EventLogEntryType.Warning);
                    }
                }
                else
                {
                    this.EventLog.WriteEntry(string.Format("Unable to disconnect session \"{0}\". Called to tscon failed.", sessionId), EventLogEntryType.Warning);
                }
            }
        }
    }
}