using Aggregation.Deployment;
using Microsoft.Data.SqlClient;
using Microsoft.Web.Administration;
using System.Configuration;
using System.Diagnostics;
using System.Security.Principal;
using System.ServiceProcess;

namespace Aggregation_Deployment
{
    static class Program
    {
        private static bool IsServiceinstalled = false;
        private static bool IsWebSiteExist = false;

        private static Config deployConfigObj;

        public static Config DeployConfigObj
        {
            get
            {
                return deployConfigObj;
            }
            set
            {
                deployConfigObj = value;
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Process started!");

            if (!IsAdministrator())
            {
                Console.WriteLine("You must run this application as administrator.");
                Terminate(-1);
            }

            CreateConfigObject(args);

            PriorProcessRunning();

            DeployApps();

            Terminate(0);
        }

        static void AggregationAppRunning()
        {
            bool isRunning = System.Diagnostics.Process.GetProcessesByName(DeployConfigObj.ProcessName).Count() > 0;

            if (isRunning)
            {
                Console.WriteLine($"({DeployConfigObj.ProcessName} is already running and waiting to be completed.");
                var process = System.Diagnostics.Process.GetProcessesByName(DeployConfigObj.ProcessName).FirstOrDefault();
                if (process != null)
                {
                    process.WaitForExit(DeployConfigObj.TimeoutMilliseconds);
                }

                isRunning = System.Diagnostics.Process.GetProcessesByName(DeployConfigObj.ProcessName).Count() > 0;
            }

            if (isRunning)
            {
                Console.WriteLine($"({DeployConfigObj.ProcessName} is already running and process can't continue.");
                Terminate(-1);
            }
        }

        static void CopyFile(string fileName, string sourceDirectory, string destinationDirectory)
        {
            try
            {
                File.Copy(sourceDirectory + @"\" + fileName, destinationDirectory + @"\" + fileName, true);
            }
            catch (IOException iox)
            {
                WriteToConsole($"Erroe while duing file copy {fileName} from {sourceDirectory} to {destinationDirectory}.");
                Console.WriteLine(iox.Message);
            }
        }

        static void CopyFiles(string foldername)
        {
            string sourceDirectory = DeployConfigObj.SourceDirectory + @"\" + foldername;
            string destFolderName = (foldername == "BuildPortal") ? "Portal" : foldername;
            string destinationDirectory = DeployConfigObj.DestinationDirectory + @"\" + destFolderName;

            DirectoryInfo place = new DirectoryInfo(sourceDirectory);

            FileInfo[] Files = place.GetFiles();

            foreach (FileInfo i in Files)
            {
                CopyFile(i.Name, sourceDirectory, destinationDirectory);
            }
        }

        static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        static void CreateConfigObject(string[] args)
        {
            try
            {
                ConfigSection config = (ConfigSection)ConfigurationManager.GetSection("Config/AppSettings");
                DeployConfigObj = config.PopulateConfigObject(args);


                if (!DeployConfigObj.IsValidConfigObject)
                {
                    Console.WriteLine("Config object continas invalid value and and process can't continue.");
                    foreach (string error in DeployConfigObj.Errors)
                    {
                        System.Console.WriteLine($"{error}.");
                    }
                    Terminate(-1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred while creating config object from app settings.");
                Terminate(-1);
            }
        }

        static bool CreateDirectory(string directoryName)
        {
            if (!Directory.Exists(directoryName))
            {
                WriteToConsole($"Create directory for {directoryName}.");
                Directory.CreateDirectory(directoryName);
            }

            return true;
        }

        static void DeployApps()
        {
            bool isNewSetup = false;

            if (DeployConfigObj.Apps != null && DeployConfigObj.Apps.Count > 0)
            {
                for (int i = 0; i < DeployConfigObj.Apps.Count; i++)
                {
                    if (!Directory.Exists(DeployConfigObj.DestinationDirectory + @"\" + DeployConfigObj.Apps[i]))
                    {
                        isNewSetup = true;
                    }
                }
            }

            if (DeployConfigObj.Apps != null && DeployConfigObj.Apps.Count > 0)
            {
                if (!isNewSetup)
                {
                    StopService();
                    StopIIS();
                }

                AggregationAppRunning();

                for (int i = 0; i < DeployConfigObj.Apps.Count; i++)
                {
                    WriteToConsole($"Processing files for {DeployConfigObj.Apps[i]}.");

                    CreateDirectory(DeployConfigObj.DestinationDirectory + @"\" + DeployConfigObj.Apps[i]);

                    if (isNewSetup && DeployConfigObj.Apps[i].ToUpper() == "PORTAL")
                    {
                        CopyFilesRecursively(DeployConfigObj.SourceDirectory + @"\BuildPortal", DeployConfigObj.DestinationDirectory + @"\" + DeployConfigObj.Apps[i]);
                    }
                    else
                    {
                        CopyFiles(DeployConfigObj.Apps[i]);
                    }

                    if (!string.IsNullOrEmpty(DeployConfigObj.Apps[i]))
                    {

                        if (DeployConfigObj.ConfigNames.ContainsKey(DeployConfigObj.Apps[i]))
                        {
                            string configFileName = deployConfigObj.ConfigNames[DeployConfigObj.Apps[i]];
                            CopyFile(configFileName, DeployConfigObj.SourceDirectory + @"\ConfigurationFiles\" + DeployConfigObj.InstanceName, DeployConfigObj.DestinationDirectory + @"\" + DeployConfigObj.Apps[i]);
                        }

                    }
                }

                CopyFile("InstallService.bat", DeployConfigObj.SourceDirectory + @"\BatchFiles", DeployConfigObj.DestinationDirectory);
                CopyFile("UnInstallService.bat", DeployConfigObj.SourceDirectory + @"\BatchFiles", DeployConfigObj.DestinationDirectory);

                ExecuteSqlScript();

                if (!isNewSetup)
                {
                    StartIIS();
                    StartService();
                }
            }
        }

        //static void DeploySql()
        //{
        //    ProcessStartInfo info = new ProcessStartInfo("sqlcmd", $@" -S {DeployConfigObj.DbServerName} -d {DeployConfigObj.DbName} -U {DeployConfigObj.DbUserId} -P {DeployConfigObj.DbPassword} -i C:\WorkSpace\ig-enms-dataaggregation\Scripts\{DeployConfigObj.InstanceName}\Deployment.sql");
        //    info.UseShellExecute = false;
        //    info.CreateNoWindow = true;
        //    info.WindowStyle = ProcessWindowStyle.Hidden;
        //    info.RedirectStandardOutput = true;
        //    Process proc = new Process();
        //    proc.StartInfo = info;
        //    proc.Start();
        //    string output = proc.StandardOutput.ReadToEnd();
        //    proc.WaitForExit();
        //    Console.WriteLine($"output stream are:\n'{output}'");
        //}

        static void ExecuteSqlScript()
        {
            string connectionString = $"data source={DeployConfigObj.DbServerName};initial catalog={DeployConfigObj.DbName};user id={DeployConfigObj.DbUserId};password={DeployConfigObj.DbPassword};MultipleActiveResultSets=True;Encrypt=False;";
            string[] sqlScripts = {
                "CleanUp.sql",
                "DropObjects.sql",
                "Tables.sql",
                "DataFix_Release.sql",
                "PopulateData.sql",
                "Functions.sql",
                "{InstanceName}\\Instance.sql",
                "Views.sql",
                "StoredProcedures.sql"
            };
            List<string> sqlLines = new List<string>();
            int res = 0;

            foreach (string sqlScript in sqlScripts)
            {
                string sqlScriptFilename = sqlScript.Replace("{InstanceName}", DeployConfigObj.InstanceName);
                WriteToConsole($"Executing {sqlScriptFilename}");
                FileInfo file = new FileInfo(DeployConfigObj.SourceDirectory + $@"\Scripts\{sqlScriptFilename}");

                string script = file.OpenText().ReadToEnd();
                string[] splitChar = { "\r\nGO\r\n" };
                var lines = script.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
                sqlLines.AddRange(lines.ToArray());
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (string sqlLine in sqlLines)
                {
                    SqlCommand command = new SqlCommand(sqlLine.Replace("\r\nGO", ""), connection)
                    {
                        CommandTimeout = 300
                    };
                    try
                    {
                        res = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine($"Exception: {ex}, SQL Statement: {sqlLine}");
                    }
                }
                connection.Close();
            }
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void PriorProcessRunning()
        {
            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);

            foreach (Process p in procs)
            {
                if ((p.Id != curr.Id) &&
                    (p.MainModule?.FileName == curr.MainModule?.FileName))
                {
                    Console.WriteLine("Prior process is already running and process can't continue.");
                    Terminate(-1);
                }
            }
        }

        static void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    Console.WriteLine("AppSettings is empty.");
                }
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        Console.WriteLine("Key: {0} Value: {1}", key, appSettings[key]);
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Exception: Error reading app settings");
            }
        }

        static string ReadSetting(string key)
        {
            string result;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "Not Found";
                Console.WriteLine(result);
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                return "";
            }
        }

        static void StartIIS()
        {
            if (IsWebSiteExist)
            {
                var server = new ServerManager();
                var site = server.Sites.FirstOrDefault(s => s.Name == DeployConfigObj.SiteName);
                if (site != null)
                {
                    WriteToConsole("Starting IIS - Start!");
                    site.Start();
                    WriteToConsole("Starting IIS - End!");
                }

            }
        }

        static void StartService()
        {
            if (IsServiceinstalled)
            {
                WriteToConsole("Starting Aggregation Service - Start!");

                WriteToConsole("Press any key to start Aggregation Service - Start!");
                Console.ReadKey();

                try
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(DeployConfigObj.TimeoutMilliseconds);

                    ServiceController service = new ServiceController(DeployConfigObj.ServiceName);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);

                    WriteToConsole("Starting Aggregation Service - End!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }
        }

        static void StopService()
        {
            try
            {
                ServiceController service = new ServiceController(DeployConfigObj.ServiceName);

                if (service.DisplayName != null)
                {
                    WriteToConsole("Stopping Aggregation Service - Start!");
                    WriteToConsole("Press any key to stop Aggregation Service!");
                    Console.ReadKey();

                    IsServiceinstalled = true;

                    try
                    {
                        TimeSpan timeout = TimeSpan.FromMilliseconds(DeployConfigObj.TimeoutMilliseconds);

                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                        WriteToConsole("Stop Aggregation Service - End!");
                    }
                    catch (Exception ex)
                    {
                        string label = (ex.InnerException != null && ex.InnerException.Message != "The service has not been started") ? "Warning" : "Exception";
                        Console.WriteLine($"{label}: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                IsServiceinstalled = false;
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        static void StopIIS()
        {
            var server = new ServerManager();
            var site = server.Sites.FirstOrDefault(s => s.Name == DeployConfigObj.SiteName);
            if (site != null)
            {
                WriteToConsole("Stopping IIS - Start!");
                IsWebSiteExist = true;
                site.Stop();
                WriteToConsole("Stopping IIS - End!");
            }
            else
            {
                Console.WriteLine($"Warning: Website {DeployConfigObj.SiteName} does not exist.");
            }
        }

        static void Terminate(int ExitCode)
        {
            Console.WriteLine("Process ended!");
            Console.WriteLine("Press any key to close this windows.");
            Console.ReadKey();
            System.Environment.Exit(ExitCode);
        }

        static void UpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Exception: Error writing app settings");
            }
        }

        static void WriteToConsole(string text)
        {
            if (DeployConfigObj.LogToConsole && !String.IsNullOrEmpty(text))
            {
                Console.WriteLine(text);
            }
        }
    }
}
