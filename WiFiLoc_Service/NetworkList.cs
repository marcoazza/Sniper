using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using NativeWifi;

namespace WiFiLoc_Service
{
    class NetworkList
    {
        Hashtable _hash = new Hashtable();
        /// <summary>
        /// acquisique i network circostanti
        /// </summary>
        /// 

        public Hashtable Hash {
            get {
                return _hash;
            }
        }
        public void acquireNetworkList() {

            WlanClient client = new WlanClient();
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                // Lists all networks with WEP security
                Wlan.WlanBssEntry[] bsss = wlanIface.GetNetworkBssList();
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);


                foreach (Wlan.WlanBssEntry bssentry in bsss)
                {
                    Network n= new Network();
                    n.setNetwork( bssentry.getBssIdString(),bssentry.rssi);
                    
                    _hash.Add(n.Mac,n);



                }
            }
        
        
        
        
        }

        public Network this[string key]
        {

            set
            {
                Network v = value as Network;
                if (v != null)
                    _hash.Add(key, value);
                else
                {

                    System.ApplicationException ex = new System.ApplicationException("non è un Network");
                    throw ex;
                }
            }
            get
            {
                Network n = _hash[key] as Network;
                if (n != null)
                    return (Network)_hash[key];
                else
                {
                    System.ApplicationException ex = new System.ApplicationException("non è un Network");

                    throw ex;
                }
            }
        }


    }
}
