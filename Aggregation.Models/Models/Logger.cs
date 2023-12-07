using System;
using Aggregation_DataModels.Enums;

namespace Aggregation_DataModels.Models
{
    public class Logger
    {
        public long elapsedTime { get; set; }
        public string exceptionMessage { get; set; }
        public string innerException { get; set; }
        public long jobId { get; set; }
        public Log_Type logType { get; set; }
        public string methodName { get; set; }
        public string parameters { get; set; }
        public int returnCode { get; set; }
        public long taskId { get; set; }
        public DateTime timestamp { get; set; }
        public string stackTrace { get; set; }

        public Logger(long jobId, string methodName, string parameters)
        {
            this.timestamp = System.DateTime.Now;
            this.jobId = jobId;
            this.methodName = methodName;
            this.parameters = parameters;
            this.logType = Log_Type.Acticity;
            Console.WriteLine("Method Name: " + methodName + " Job Id:" + jobId.ToString());
        }

        public void Save()
        {
            this.logType = Log_Type.Info;
            this.elapsedTime = (long)(System.DateTime.Now - timestamp).TotalSeconds;
        }
    }
}