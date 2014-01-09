using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiFiLoc_Service
{
    class Network
    {
        private string _mac;
        private int _potenza;

        public string Mac {
            get { return _mac; }
            set { _mac = value; }
        }




        public int Potenza
        {
            get { return _potenza; }
            set { _potenza = value ; }
        }


        public void setNetwork(string mac, int potenza) { 
            _mac=mac;
            _potenza= potenza;
        }

       




    }
}
