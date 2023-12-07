using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregation_Library
{
    public class Helper
    {
        public const string Logs = "logs";
        public const string ErrorLogs = "ErrorLogs";

        public static Process[] GetProcessesByName(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes;
        }


        public static bool IsProcessRunning(string processName)
        {
            Process[] processes = Helper.GetProcessesByName(processName);

            return !(processes.Length == 0);
        }

        public static Int32 GetIntegerValue(object value)
        {
            Int32 i = 0;

            if (object.Equals(value, null))
            {
            }
            else
            {
                i = Convert.ToInt32(value);
            }
            return i;
        }

        public static Double GetDoubleValue(object value)
        {
            Double i = 0;

            if (object.Equals(value, null))
            {
            }
            else
            {
                i = Convert.ToDouble(value);
            }
            return i;
        }

        public static Decimal GetDecimalValue(object value)
        {
            Decimal decValue = 0;
            if (object.Equals(value, null))
            {
            }
            else
            {
                decValue = Convert.ToDecimal(value);
            }
            return decValue;

        }

        public static string GetExecutingDirectoryName()
        {
            var location = new Uri(System.Reflection.Assembly.GetEntryAssembly().GetName().CodeBase);
            return new System.IO.FileInfo(location.AbsolutePath).Directory.FullName;
        }
    }

}
