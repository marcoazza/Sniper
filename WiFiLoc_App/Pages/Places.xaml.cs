using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Collections;
using System.Threading;

namespace WiFiLoc_App.Pages
{
    /// <summary>
    /// Logica di interazione per Places.xaml
    /// </summary>
    public partial class Places : Page
    {
        public Places()
        {
            InitializeComponent();
            ThreadPool.QueueUserWorkItem(this.retrievePlaces);

            
        }
        public delegate void Del(ArrayList places);


        private void fillPlaces(ArrayList places) {
            foreach (Luogo p in places)
            {
                placesList.Items.Add(p.NomeLuogo);
            }
        }

        private void retrievePlaces(object state) {
            ArrayList places = Luogo.getPossibiliLuoghi();
            Del d = fillPlaces;
            placesList.Dispatcher.BeginInvoke(d,places);
        }

        private void erase(object sender, RoutedEventArgs e)
        {
            string eracePlace = (string)placesList.SelectedItem;
            if (eracePlace != null)
            {
                Luogo.removeLuogoFromDB(eracePlace);
            }
        

        }

        private void modify(object sender, RoutedEventArgs e)
        {

        }
    }
}
