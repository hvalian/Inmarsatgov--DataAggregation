﻿using System.ServiceProcess;

namespace WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AggregationService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
