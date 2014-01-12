﻿using System;
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

namespace WiFiLoc_Service
{
    public partial class WiFiLoc_Service : ServiceBase
    {

        protected Thread m_thread;

        public WiFiLoc_Service()
        {

            InitializeComponent();

            if (!System.Diagnostics.EventLog.SourceExists("WiFiLoc_ServiceSource")) 
		{         
				System.Diagnostics.EventLog.CreateEventSource(
					"WiFiLoc_ServiceSource","WiFiLoc_ServiceLog");
		}
		eventLog1.Source = "WiFiLoc_ServiceSource";
		eventLog1.Log = "WiFiLoc_ServiceLog";

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
            catch (Exception e) {
                eventLog1.WriteEntry(e.Message);
                throw;
            }
        }

        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
       {
           return Encoding.ASCII.GetString( ssid.SSID, 0, (int) ssid.SSIDLength );
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
        const int REFRESH_TIME = 10000;
        protected delegate void placeChanged(Luogo l);
        private delegate void placeOnContinue(Luogo l);
        private delegate void placeOnLongContinue(Luogo l);


        private void showInterfaces() {
            Luogo prevPlace = null;
            Luogo currentPlace = null;
            int inPlace = 0;
            while (true) {
                WlanClient wc = WlanClient.getInstance();
                currentPlace = Locator.locate();
                if (inPlace == 10) { 
                    //call delegate
                    placeChanged(currentPlace);
                }

                //update Stats
                if (inPlace % 10 != 0 && inPlace != 10) {
                    placeOnContinue(currentPlace);
                }
                //update Stats
                if (inPlace % 50)
                {
                    placeOnLongContinue(currentPlace);
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
                wc.Interfaces[0].Scan();
                Thread.Sleep(REFRESH_TIME);
            }

        }
        
        public delegate void handler();

        public void f(){
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
