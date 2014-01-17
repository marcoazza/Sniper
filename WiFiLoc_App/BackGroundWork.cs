using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiFiLoc_App;
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
            WlanClient wc = WlanClient.getInstance();
            Luogo prevPlace = null;
            Luogo currentPlace = null;
            Luogo placeToLocate = null;
            Luogo locatedPlace = null;

            pc += launchActions;
            poc += updateStats;
            polc += updatePlace;


            int inPlace = 0;
            int notInPlace = 0;
            while (true)
            {
                placeToLocate = new Luogo();
                placeToLocate.saveNextList();

                if (os != null)
                    os(placeToLocate.NetwList);

                
                try
                {
                    currentPlace = Locator.locate();
                }
                catch (Exception e)
                {
                    currentPlace = null;
                }


                if (locatedPlace == null)
                {
                    if (currentPlace != null)
                    {
                        if (currentPlace.Equals(prevPlace))
                        {
                            inPlace++;
                        }
                        if (inPlace == 3)
                        {
                            locatedPlace = currentPlace;
                            pc(currentPlace);
                        }
                    }
                    prevPlace = currentPlace;
                }
                else {
                    if (locatedPlace.Equals(currentPlace))
                    {
                        notInPlace = 0;
                        inPlace++;
                        //update Stats
                        if (inPlace % 10 == 0 && inPlace != 10 && inPlace != 0)
                        {
                            poc(currentPlace);
                        }
                        //update Stats
                        if (inPlace % 30 == 0 && inPlace != 0)
                        {
                            polc(currentPlace);
                        }
                    }
                    else {
                        notInPlace++;
                        if (notInPlace == 3) {
                            locatedPlace = null;
                            inPlace = 0;
                            notInPlace = 0;
                        }
                    }

                    prevPlace = currentPlace;
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
            startInfo.Arguments = "/C " + action;
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

        private static void updatePlace(Luogo l) {
            l.UpdatePlacePosition();
        }
    }

}
