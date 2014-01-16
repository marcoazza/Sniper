using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace WiFiLoc_App
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            Thread m_thread;
            ThreadStart ts = new ThreadStart(BackGroundWork.start);
            m_thread = new Thread(ts);
            m_thread.IsBackground = true;
            m_thread.Start();
            // Create main application window, starting minimized if specified
        }


    }
}
