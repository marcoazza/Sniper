using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiFiLoc_Service;
using NativeWifi;
using System.Threading;
using System.Diagnostics;


namespace WiFiLoc_App
{
    class BackGroundWork
    {
         public delegate void onScan(NetworkList l);
         public delegate void PlaceChanged(Luogo l);
         delegate void PlaceOnContinue(Luogo l);
         delegate void PlaceOnLongContinue(Luogo l);
         static onScan os;
         static PlaceChanged pc;
         static PlaceOnContinue poc;
         static PlaceOnLongContinue polc;

         const int REFRESH_TIME = 5000;

         public static void registerHandlers(onScan b,PlaceChanged c) {
             os += b;
             pc += c;

         }

        public static void start()
        {
            
            Luogo prevPlace = null;
            Luogo currentPlace = null;
            Luogo placeToLocate = null;

            pc += launchActions;
            poc += updateStats;

            int inPlace = 0;
            while (true)
            {
                placeToLocate = new Luogo();
                placeToLocate.saveNextList();


                WlanClient wc = WlanClient.getInstance();
                try
                {
                    currentPlace = Locator.locate();
                }
                catch (Exception e)
                {
                    currentPlace = null;
                }
                if (os != null)
                    os(placeToLocate.NetwList);
                if (inPlace == 3)
                {
                    //call delegate

                    pc(currentPlace);
                }

                //update Stats
                if (inPlace % 10 == 0 && inPlace != 10 && inPlace != 0)
                {
                    poc(currentPlace);
                }
                //update Stats
                if (inPlace % 50 == 0 && inPlace != 0)
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
                else
                {
                    prevPlace = currentPlace;
                    inPlace = 1;
                }
                if (wc.Interfaces.Length != 0)
                    wc.Interfaces[0].Scan();
                Thread.Sleep(REFRESH_TIME);
            }


        }

        private static void launchActions(Luogo l)
        {
            if (l != null) {
                foreach (ActionList.Action a in l.ActionsList.GetAll())
                {
                    launchAction(a.Path);
                }
            }

            return;
        }

        private static void updateStats(Luogo l) {
            if (l != null)
                l.UpdateStats(10 * REFRESH_TIME/1000);
        }

        private static bool launchAction(string action)
        {


            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe ";
            startInfo.Arguments = "/C \"" + action + "\"";
            process.StartInfo = startInfo;
            try
            {

                process.Start();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

}
