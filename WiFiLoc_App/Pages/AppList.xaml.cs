
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
using WiFiLoc_App;
using WiFiLoc_Service;
using System.Threading;
using System.Windows.Interop;
using Microsoft.Win32;
using System.Management;
using System.Drawing;
using System.IO;
using System.Collections;


namespace WiFiLoc_App
{
    /// <summary>
    /// Logica di interazione per AppList.xaml
    /// </summary>
    public partial class AppList : Page
    {
        static public Page handle;
        public AppList()
        {
            InitializeComponent();
            handle = this;
           
        }

        public void ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {


        }


        public delegate void updateAppList(System.Windows.Controls.ListBox l, string s);

        private void ListaApplicaz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void GetPosizione_Click(object sender, RoutedEventArgs e)
        {
            ArrayList listLuoghi = new ArrayList();
            listLuoghi=WiFiLoc_Service.Luogo.getPossibiliLuoghi();
            Luogo l = Locator.locate();
            ListaReti.Items.Clear();
            if (l != null)
            {
                foreach (DictionaryEntry d in l.NetwList.Hash) {
                    Network n = (Network)d.Value;
                    string rete = n.Mac + "     " + n.Potenza;
                    ListaReti.Items.Add(rete);
                }
                MessageBox.Show("Posizione corrente -->" + l.NomeLuogo);
            }
            else {
                MessageBox.Show("Posizione sconosciuta");
            }
        }

        private void SalvaLuogo_Click(object sender, RoutedEventArgs e)
        {
            string error = "Impossibile salvare luogo";
            bool err = false;
            Luogo l=null;
            if (NomeLuogo.Text != "")
            {
                l = new Luogo(NomeLuogo.Text);
                if (l.NetwList.Hash.Count == 0)
                {
                    err=true;
                    error += "\nNo APs rilevati";
                }
  
                if (l.checkIfNameExist()) {
                   err=true;
                   error += "\nLuogo gia' presente nel DB ";
                }
            }
            else {
                err=true;
                error += "\n Il nome e' obbligatorio";
            }

            if (err)
                MessageBox.Show(error);
            else
                l.luogoToDB();
        }

        private void RimuoviLuogo_Click(object sender, RoutedEventArgs e)
        {
            if (NomeLuogoRimuovere.Text != "")
            {
                Luogo.removeLuogoFromDB(NomeLuogoRimuovere.Text);
            }
        }

        private void AggiornaLuogo_Click(object sender, RoutedEventArgs e)
        {
            if (NomeLuogoAggiornare.Text != "")
            {
               
            }
            else {
                MessageBox.Show("inserisci un nome per aggiornare");
            }

        }


         





    }
}
