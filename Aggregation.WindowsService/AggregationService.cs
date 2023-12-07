using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Timers;
using Aggregation_Library;

namespace WindowsService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    public partial class AggregationService : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        const int statusWaitTime = 1000;

        System.Timers.Timer timer = new System.Timers.Timer();

        public AggregationService()
        {
            InitializeComponent();

            timer.Enabled = false;
        }

        protected override void OnStart(string[] args)
        {

            Aggregation.WriteToFile(null, "Staring Service", false);

            Aggregation.KillAllProcesses();

            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = statusWaitTime;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 1000;
            timer.Enabled = true;

            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            timer.Enabled = false;

            Aggregation.WriteToFile(null, "Stoping Service", false);

            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = statusWaitTime;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnContinue()
        {
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            StartProcess();
        }

        private void StartProcess()
        {
            timer.Enabled = false;

            if (Aggregation.IsEnabled())
            {
                ProcessStartInfo processInfo;
                Process process;

                processInfo = new ProcessStartInfo(Aggregation.GetExecutablePath + "AggregationApp.exe");
                processInfo.Arguments = "STARTJOB";
                processInfo.CreateNoWindow = false;
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";
                process = Process.Start(processInfo);
                process.WaitForExit();

                int exitCode = process.ExitCode;
                int processId = process.Id;
                process.Close();
            }

            if (Aggregation.IsEnabled())
            {
                timer.Interval = Aggregation.GetNextJobDelay();
                timer.Enabled = true;
            }
            else
            {
                string messageBody = "Job processing is disabled." + '\n' + '\n';
                messageBody += "Auto recovery will be performed. If recovery is successful, job processing will be resumed." + '\n' + '\n';
                Aggregation.SendNotification(messageBody);
                Aggregation.WriteToFile(null, messageBody, true);

                performRecovery();
            }
        }

        private void performRecovery()
        {
            timer.Enabled = false;

            Aggregation.KillAllProcesses();

            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo(Aggregation.GetExecutablePath + "AggregationApp.exe");
            processInfo.Arguments = "PERFORMRECOVERY";
            processInfo.CreateNoWindow = false;
            processInfo.UseShellExecute = true;
            processInfo.Verb = "runas";
            process = Process.Start(processInfo);
            process.WaitForExit(60000);
            process.Close();

            timer.Interval = 1000;
            timer.Enabled = true;
        }
    }
}