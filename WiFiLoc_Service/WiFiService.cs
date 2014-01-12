using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NativeWifi;
using WiFiLoc_App;
using Microsoft;

namespace WiFiLoc_Service
{
    public partial class WiFiLoc_Service : ServiceBase
    {

        protected Thread m_thread;
        const int REFRESH_TIME = 10000;
        protected delegate void PlaceChanged(Luogo l);
        protected delegate void PlaceOnContinue(Luogo l);
        protected delegate void PlaceOnLongContinue(Luogo l);

        protected PlaceChanged pc;
        protected PlaceOnContinue poc;
        protected PlaceOnLongContinue polc;


        public WiFiLoc_Service()
        {

            InitializeComponent();

            if (!System.Diagnostics.EventLog.SourceExists("WiFiLoc_ServiceSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "WiFiLoc_ServiceSource", "WiFiLoc_ServiceLog");
            }
            eventLog1.Source = "WiFiLoc_ServiceSource";
            eventLog1.Log = "WiFiLoc_ServiceLog";
            pc += launchActions;
        }

        private bool launchAction(string action) {

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe ";
            startInfo.Arguments = "/C " + action;
            process.StartInfo = startInfo;
            try{
                process.Start();
            } catch{
                return false;
            }

            return true;
        }

        private void launchActions(Luogo l ) {
            foreach (ActionList.Action a in l.ActionsList.GetAll()) {
                launchAction(a.Path);
            }
            return;
        }

        private void updateStats(Luogo l) { 
            
        
        }


        protected override void OnStart(string[] args)
        {
            try
            {
                ThreadStart ts = new ThreadStart(this.showInterfaces);
                m_thread = new Thread(ts);
                m_thread.Start();
                eventLog1.WriteEntry("WiFiLoc_Service è partita");
            }
            catch (Exception e)
            {
                eventLog1.WriteEntry(e.Message);
                throw;
            }
        }

        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        //convert MAC Address to String
        //static string MACToString(byte[] ba)
        //{
        //    StringBuilder sb = new StringBuilder(ba.Length * 2);
        //    int i = 0;
        //    foreach (byte b in ba)
        //    {
        //        if (i < ba.Length - 1)
        //        {
        //            sb.AppendFormat("{0:x2}:", b);
        //        }
        //        else
        //        {
        //            sb.AppendFormat("{0:x2}", b);
        //        }
        //        i++;
        //    }
        //    return sb.ToString();
        //}
        //static string ConvertMACAddr(byte[] ba)
        //{
        //    StringBuilder sb = new StringBuilder(ba.Length * 2);
        //    foreach (byte b in ba)
        //    {
        //        sb.AppendFormat("{0:x2}", b);
        //    }
        //    return sb.ToString();
        //}


        private void showInterfaces()
        {
            Luogo prevPlace = null;
            Luogo currentPlace = null;
            int inPlace = 0;
            eventLog1.WriteEntry("WiFiLoc_Service alg");
            while (true)
            {

                WlanClient wc = WlanClient.getInstance();
                eventLog1.WriteEntry("WiFiLoc_Service before locate");
                try
                {
                    currentPlace = Locator.locate();
                }
                catch (Exception e) {
                    currentPlace = null;
                }
                
                eventLog1.WriteEntry("WiFiLoc_Service after locate");
                if (inPlace == 3) { 
                    //call delegate
                    
                    pc(currentPlace);
                }

                //update Stats
                if (inPlace % 10 != 0 && inPlace != 10) {
                    //poc(currentPlace);
                }
                //update Stats
                if (inPlace % 50 == 0)
                {
                    //polc(currentPlace);
                }
                //count times which consecutive find same place
                if (currentPlace != null)
                {
                    if (currentPlace.Equals(prevPlace))
                    {
                        inPlace++;
                    }
                    else
                    {
                        inPlace = 1;
                        prevPlace = currentPlace;
                    }
                }
                else {
                    prevPlace = currentPlace;
                    inPlace = 1;
                }
                if (wc.Interfaces.Length != 0)
                    wc.Interfaces[0].Scan();
                eventLog1.WriteEntry("WiFiLoc_Service before sleep");
                Thread.Sleep(REFRESH_TIME);
            }


        }

        public delegate void handler();

        public void f()
        {
            //if (sonosicuro) {
            //    handler();
            //}

        }

        protected override void OnStop()
        {
            m_thread.Abort();
            eventLog1.WriteEntry("WiFiLoc_Service finita1");
            eventLog1.WriteEntry("WiFiLoc_Service finita2");
            eventLog1.WriteEntry("WiFiLoc_Service ");

        }


    }
}
