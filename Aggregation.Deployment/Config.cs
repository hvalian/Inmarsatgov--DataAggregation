
namespace Aggregation.Deployment
{
    public class Config
    {
        public List<string> Apps { get; set; }
        public Dictionary<string, string> ConfigNames { get; set; }
        public string DbName { get; set; }
        public string DbPassword { get; set; }
        public string DbServerName { get; set; }
        public string DbUserId { get; set; }
        public string DestinationDirectory { get; set; }
        public List<string> Errors { get; set; }
        public string InstanceName { get; set; }
        public bool LogToConsole { get; set; }
        public string ProcessName { get; set; }
        public string ServiceName { get; set; }
        public string SiteName { get; set; }
        public string SourceDirectory { get; set; }
        public int TimeoutMilliseconds { get; set; }
        public bool IsValidConfigObject { get; set; }
    }
}
