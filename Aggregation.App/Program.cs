using System;
using System.Diagnostics;

using Aggregation_Library;

namespace Aggregation_App
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string paramValue = "";

                for (int i = 1; i <= args.Length - 1; i++)
                {
                    paramValue += args[i] + " ";
                }

                if (PriorProcess() != null)
                {
                    System.Environment.Exit(-100);
                }
                else
                {
                    int rc = Aggregation.Main(args[0].ToUpper(), paramValue.Trim());
                    System.Environment.Exit(rc);
                }
            }
        }

        public static Process PriorProcess()
        {
            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);
            foreach (Process p in procs)
            {
                if ((p.Id != curr.Id) &&
                    (p.MainModule.FileName == curr.MainModule.FileName))
                    return p;
            }
            return null;
        }
    }
}
