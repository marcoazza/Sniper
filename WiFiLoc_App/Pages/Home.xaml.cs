using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WiFiLoc_App;
using System.Threading;
using System.ComponentModel;

namespace WiFiLoc_App.Pages
{
    /// <summary>
    /// Logica di interazione per Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        const string FAIL_LOCATE= "I'm lost.... :(";
        Luogo currentPlace = null;

        public Home()
        {
            InitializeComponent();
            this.currentPlaceLabel.Content = FAIL_LOCATE;
            //ThreadPool.QueueUserWorkItem(this.locate);
            BackGroundWork.onScan b;
            BackGroundWork.PlaceChanged ocplace;
            b = onScanHandler;
            ocplace = onChangePlaceHandler;
            BackGroundWork.registerHandlers(b, ocplace);
        }


        private void onScanHandler( NetworkList nwlist)
        {
             DelNetList dlgNetList = updateNetWorkList;
            NetworkList.Dispatcher.BeginInvoke(dlgNetList, nwlist);
        }

        private void onChangePlaceHandler(Luogo l)
        {
            Del dlgLabel = updateLabel;
            if (l != null)
            {
                currentPlaceLabel.Dispatcher.BeginInvoke(dlgLabel, l.NomeLuogo);
            }
            else {
                currentPlaceLabel.Dispatcher.BeginInvoke(dlgLabel, FAIL_LOCATE);
            }
        }

        public delegate void Del(string message);
        public delegate void DelNetList(NetworkList netList);

        private void updateLabel(string name) {
            currentPlaceLabel.Content = name;
        }

        private void updateNetWorkList(NetworkList nl)
        {
            NetworkList.Items.Clear();
            foreach(DictionaryEntry n in nl.Hash ){
                Network net = (Network) n.Value;
                if(!NetworkList.Items.Contains(net.SSID))
                    NetworkList.Items.Add(net.SSID);
            }
            
        }
        private void locate(Object stateInfo){
        
            Luogo placeToLocate = new Luogo();

            while (true) {
                currentPlace = Locator.locate();
                Del dlgLabel = updateLabel;
                DelNetList dlgNetList = updateNetWorkList;
                placeToLocate.saveNextList();

                if (currentPlace != null)
                {
                    currentPlaceLabel.Dispatcher.BeginInvoke(dlgLabel, currentPlace.NomeLuogo);
                    NetworkList.Dispatcher.BeginInvoke(dlgNetList, placeToLocate.NetwList);
                }
                else {
                    currentPlaceLabel.Dispatcher.BeginInvoke(dlgLabel, FAIL_LOCATE);
                    
                    NetworkList.Dispatcher.BeginInvoke(dlgNetList, placeToLocate.NetwList);
                }
                
                Thread.Sleep(1000);

            }
               
        
        }
    }
}
