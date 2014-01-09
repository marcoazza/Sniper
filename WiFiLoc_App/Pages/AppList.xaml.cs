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

        private void GeneraListaAzioni(object sender, RoutedEventArgs e)
        {
            //Class1.getInstalledSoftware(this.ListaApplicaz);
            ThreadPool.QueueUserWorkItem(ActionManager.getInstalledSoftware,this.ListaApplicaz);    
        }

        public delegate void updateAppList(System.Windows.Controls.ListBox l, string s);

        private void ListaApplicaz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SelectedItems(object sender, RoutedEventArgs e)
        {
            
            ActionManager.itemApp sel = (ActionManager.itemApp) ListaApplicaz.SelectedItem;
            if (sel != null)
            {
                MessageBox.Show(sel.applicazione);
            }
            else {
                MessageBox.Show("Nessuna applicazione selezionata");            
            }
        }

        private void AggiungiApplicazione(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".exe"; // Default program extension
            dlg.Filter = "Application (.exe)|*.exe"; // Filter programs by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open file
                string filename = dlg.FileName;
                Icon ico = Icon.ExtractAssociatedIcon(filename);

                Bitmap b = ico.ToBitmap();
                string name = System.IO.Path.GetFileName(filename);
                

                object[] myArray = new object[4];
                myArray[0] = ListaApplicaz;
                myArray[1] = filename;
                string rp = Environment.CurrentDirectory.ToString();

                b.Save(rp + "\\images\\" + System.IO.Path.GetFileNameWithoutExtension(filename) + ".ico");
                myArray[2] = rp + "\\images\\" + System.IO.Path.GetFileNameWithoutExtension(filename) + ".ico";
                myArray[3] = System.IO.Path.GetFileNameWithoutExtension(filename);

                ListaApplicaz.Dispatcher.BeginInvoke(new ActionManager.updateAppList(ActionManager.addItem), myArray);

            }
        }

        private void GetPosizione_Click(object sender, RoutedEventArgs e)
        {
            ArrayList listLuoghi = new ArrayList();
            listLuoghi=WiFiLoc_Service.Luogo.getPossibiliLuoghi();
            Triangolazione t = new Triangolazione();
            Luogo l = t.triangolaPosizione();
            if (l != null)
            {
                MessageBox.Show("Posizione corrente -->" + l.NomeLuogo);
            }
            else {
                MessageBox.Show("Posizione sconosciuta");
            }
        }

        private void SalvaLuogo_Click(object sender, RoutedEventArgs e)
        {
            if (NomeLuogo.Text != "")
            {
                Luogo l = new Luogo(NomeLuogo.Text);
                if (!l.check())
                {
                    l.luogoToDB();
                }
            }
        }

        private void RimuoviLuogo_Click(object sender, RoutedEventArgs e)
        {
            if (NomeLuogoRimuovere.Text != "")
            {
                Luogo.removeLuogoFromDB(NomeLuogoRimuovere.Text);
            }
        }


         





    }
}
