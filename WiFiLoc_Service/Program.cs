using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WiFiLoc_Service
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        static void Main()
        {
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new WiFiLoc_Service() 
            };
            ServiceBase.Run(ServicesToRun);
        }

        private static void CurrentDomain_UnhandledException(
                                                 Object sender,
                                                 UnhandledExceptionEventArgs e)
        {
            if (e != null && e.ExceptionObject != null)
            {
                // log exception:
            }
        }
    }
}
