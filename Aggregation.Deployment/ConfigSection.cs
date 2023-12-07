using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aggregation.Deployment
{
    public class ConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("DestinationDirectory")]
        public ConfigElement DestinationDirectory
        {
            get { return this["DestinationDirectory"] as ConfigElement; }
            set { this["DestinationDirectory"] = value; }
        }

        [ConfigurationProperty("LogToConsole")]
        public ConfigElement LogToConsole
        {
            get { return this["LogToConsole"] as ConfigElement; }
            set { this["LogToConsole"] = value; }
        }

        [ConfigurationProperty("ProcessName")]
        public ConfigElement ProcessName
        {
            get { return this["ProcessName"] as ConfigElement; }
            set { this["ProcessName"] = value; }
        }

        [ConfigurationProperty("ServiceName")]
        public ConfigElement ServiceName
        {
            get { return this["ServiceName"] as ConfigElement; }
            set { this["ServiceName"] = value; }
        }

        [ConfigurationProperty("SiteName")]
        public ConfigElement SiteName
        {
            get { return this["SiteName"] as ConfigElement; }
            set { this["SiteName"] = value; }
        }

        [ConfigurationProperty("SourceDirectory")]
        public ConfigElement SourceDirectory
        {
            get { return this["SourceDirectory"] as ConfigElement; }
            set { this["SourceDirectory"] = value; }
        }

        [ConfigurationProperty("TimeoutMilliseconds")]
        public ConfigElement TimeoutMilliseconds
        {
            get { return this["TimeoutMilliseconds"] as ConfigElement; }
            set { this["TimeoutMilliseconds"] = value; }
        }

        public Config PopulateConfigObject(string[] args)
        {
            string machiName = Environment.MachineName.ToUpper();

            Dictionary<string, string> configurtions = GetConfigfiles();
            Dictionary<string, string> dbServers = GetDBServers();
            Dictionary<string, string> instances = GetInstances();

            Config configObject = new Config();
            configObject.ConfigNames = GetConfigfiles();

            if (configurtions.Count == 0)
            {
                configObject.Errors.Add("Invalid Configuration files.");
            }

            if (dbServers.Count == 0)
            {
                configObject.Errors.Add("Invalid DB Servers.");
            }

            if (instances.Count == 0)
            {
                configObject.Errors.Add("Invalid Instances.");
            }
            else
            {
                configObject.InstanceName = instances[machiName];
                if (string.IsNullOrEmpty(configObject.InstanceName))
                {
                    configObject.Errors.Add("Invalid configuration value: InstanceName");
                }
            }

            configObject.Apps = new List<string>(GetConfigfiles().Keys);
            configObject.ConfigNames = configurtions;
            configObject.DestinationDirectory = this.DestinationDirectory.InnerText;
            configObject.LogToConsole = Convert.ToBoolean(this.LogToConsole.InnerText);
            configObject.Errors = new List<string>();
            configObject.ProcessName = this.ProcessName.InnerText;
            configObject.ServiceName = this.ServiceName.InnerText;
            configObject.SiteName = this.SiteName.InnerText;
            configObject.SourceDirectory = (configObject.InstanceName == "DEV") ? @"C:\Deployment\ig-enms-dataaggregation" : this.SourceDirectory.InnerText;
            int timeoutMilliseconds = Convert.ToInt32(this.TimeoutMilliseconds.InnerText);
            configObject.TimeoutMilliseconds = (timeoutMilliseconds == 0) ? 60000 : timeoutMilliseconds;

            if (dbServers.ContainsKey(configObject.InstanceName))
            {
                string dbServer = dbServers[configObject.InstanceName];
                string[] dbInfo = dbServer.Split(",");
                if (dbInfo != null && dbInfo.Length > 3)
                {
                    configObject.DbServerName = dbInfo[0];
                    configObject.DbName = dbInfo[1];
                    configObject.DbUserId = dbInfo[2];
                    configObject.DbPassword = dbInfo[3];
                }
                else
                {
                    configObject.Errors.Add($"Invalid db configuration value: {dbServer}.");
                }
            }
            else
            {
                configObject.Errors.Add("Invalid configuration value: DB Server");
            }

            if (string.IsNullOrEmpty(configObject.ProcessName))
            {
                configObject.Errors.Add("Invalid configuration value: ProcessName");
            }

            if (string.IsNullOrEmpty(configObject.ServiceName))
            {
                configObject.Errors.Add("Invalid configuration value: ServiceName");
            }

            if (string.IsNullOrEmpty(configObject.SiteName))
            {
                configObject.Errors.Add("Invalid configuration value: SiteName");
            }

            if (string.IsNullOrEmpty(configObject.DestinationDirectory))
            {
                configObject.Errors.Add("Invalid configuration value: DestinationDirectory");
            }

            if (string.IsNullOrEmpty(configObject.SourceDirectory))
            {
                configObject.Errors.Add("Invalid configuration value: SourceDirectory");
            }

            configObject.IsValidConfigObject = (configObject.Errors.Count() == 0);

            return configObject;
        }

        Dictionary<string, string> GetInstances()
        {
            try
            {
                return (ConfigurationManager.GetSection("Config/InstanceNames") as System.Collections.Hashtable)
             .Cast<System.Collections.DictionaryEntry>()
             .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }

        Dictionary<string, string> GetConfigfiles()
        {
            try
            {
                return (ConfigurationManager.GetSection("Config/ConfigNames") as System.Collections.Hashtable)
                    .Cast<System.Collections.DictionaryEntry>()
                    .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }

        Dictionary<string, string> GetDBServers()
        {
            try
            {
                return (ConfigurationManager.GetSection("Config/DBServers") as System.Collections.Hashtable)
                 .Cast<System.Collections.DictionaryEntry>()
                 .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
