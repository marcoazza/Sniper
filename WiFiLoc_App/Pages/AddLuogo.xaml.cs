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
using System.Threading;
using System.Collections;
using NativeWifi;
using WiFiLoc_Service;
namespace WiFiLoc_App
{
    /// <summary>
    /// Interaction logic for AddLuogo.xaml
    /// </summary>
    public partial class AddLuogo : Page
    {
        private Luogo place;
        private WlanClient.WlanInterface wInterface = null;
        public AddLuogo()
        {
            InitializeComponent();
            ThreadPool.QueueUserWorkItem(ActionManager.getInstalledSoftware, this.ListaAzioniPredefinite);

        }

        private void AggiungiAzioneButton_Click(object sender, RoutedEventArgs e)
        {
            //get selected item from ListaAzioniPredefinite
            ActionManager.itemApp selectedItem = (ActionManager.itemApp)ListaAzioniPredefinite.SelectedItem;

            //add item to AzioniLuogo
            ListaAzioniLuogo.Items.Add(new ActionManager.itemApp { applicazione = selectedItem.applicazione, icon = selectedItem.icon, name = selectedItem.name });
        }

        private void AggiungiLuogoButton_Click(object sender, RoutedEventArgs e)
        {
            String nome = (String)NomeLuogo.Text;
            if (nome == null || nome == "")
            {
                MessageBox.Show("Nome luogo obbligatorio!");
            }
            else
            {
                WiFiLoc_Service.Luogo l = new Luogo(nome);
                l.ActionsList.SaveActions(ActionManager.SaveActions(ListaAzioniLuogo.Items));
                this.place = l;
                scanAndSave();
            }
        }

        private void scanAndSave()
        {
            WlanClient wc = WlanClient.getInstance();
            wInterface = wc.Interfaces[0];
            wInterface.Scan();
            wInterface.WlanNotification += notify;

        }

        public void notify(Wlan.WlanNotificationData s)
        {
            if (s.notificationCode == 7) {
                wInterface.WlanNotification -= notify;
                place.luogoToDB();
            }
        }



        private void AddCustomAction_Click(object sender, RoutedEventArgs e)
        {
            CustomActionWindow customAct = new CustomActionWindow();
            customAct.ShowDialog();

            if (customAct.DialogResult == true)
            {
                string s = customAct.ActionPath.Text;
                string n = System.IO.Path.GetFileNameWithoutExtension(s);
                String icon = "\\images\\action_icon.png";
                ListaAzioniLuogo.Items.Add(new ActionManager.itemApp { applicazione = s, icon = icon, name = n });

            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ListaAzioniLuogo.Items.Remove(ListaAzioniLuogo.SelectedItem);
        }

        private void ListaAzioniLuogo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
