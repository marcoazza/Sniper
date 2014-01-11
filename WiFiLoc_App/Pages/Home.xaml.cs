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
using WiFiLoc_Service;
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
        string currentPlaceValue = "";
        Luogo currentPlace = null;

        public Home()
        {
            InitializeComponent();
            currentPlaceLabel.Content = currentPlaceValue;
            ThreadPool.QueueUserWorkItem(this.locate);


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
                NetworkList.Items.Add(net.Mac);
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
