using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiFiLoc_App
{
    public class Network
    {
        private string _mac;
        private int _potenza;
        private string _ssid;
        public string Mac {
            get { return _mac; }
            set { _mac = value; }
        }
        public string SSID
        {
            get { return _ssid; }
            set { _ssid = value; }
        }



        public int Potenza
        {
            get { return _potenza; }
            set { _potenza = value ; }
        }


        public void setNetwork(string mac, int potenza,string ssid) { 
            _mac=mac;
            _potenza= potenza;
            _ssid = ssid;
        }
        public void setNetwork(string mac, int potenza)
        {
            _mac = mac;
            _potenza = potenza;
        }

       




    }
}
