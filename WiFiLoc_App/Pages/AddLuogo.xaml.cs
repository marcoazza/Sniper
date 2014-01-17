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
using WiFiLoc_App;
using System.Data;
namespace WiFiLoc_App
{
    /// <summary>
    /// Interaction logic for AddLuogo.xaml
    /// </summary>
    public partial class AddLuogo : Page
    {
        private Luogo place;
        private WlanClient.WlanInterface wInterface = null;
        private bool modify = false;
        public AddLuogo( )
        {
            InitializeComponent();
            ThreadPool.QueueUserWorkItem(ActionManager.getInstalledSoftware, this.ListaAzioniPredefinite);
            //NavigationService.LoadCompleted += new LoadCompletedEventHandler(handlerLoad);
            foreach (DictionaryEntry d in ActionManager.getFirewallRules()) {
                ListaAzioniFirewall.Items.Add(ActionManager.getFirewallItem(d.Value.ToString(), d.Key.ToString()));
            }
            place = new Luogo();
        }
        public AddLuogo(Luogo l)
        {
            InitializeComponent();
            ThreadPool.QueueUserWorkItem(ActionManager.getInstalledSoftware, this.ListaAzioniPredefinite);
            //NavigationService.LoadCompleted += new LoadCompletedEventHandler(handlerLoad);
            foreach (DictionaryEntry d in ActionManager.getFirewallRules())
            {
                ListaAzioniFirewall.Items.Add(ActionManager.getFirewallItem(d.Value.ToString(), d.Key.ToString()));
            }
            foreach (ActionList.Action a in l.ActionsList.GetAll())
            {
                ListaAzioniLuogo.Items.Add(ActionManager.getAssociatedItem(a.Path));
            }

            NomeLuogo.Text = l.NomeLuogo;
            place = l;
            modify = true;
            AggiungiLuogoButton.Content = "Update";
        }

        void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {

            place = (Luogo)e.ExtraData;
            //foreach (ActionList.Action a in place.ActionsList.GetAll())
            //{
            //    ListaAzioniLuogo.Items.Add(ActionManager.getAssociatedItem(a.Path));

            //}
        }

        private void AggiungiAzioneButton_Click(object sender, RoutedEventArgs e)
        {

            if (ListaAzioniPredefinite.SelectedItem != null)
            {
                //get selected item from ListaAzioniPredefinite
                ActionManager.itemApp selectedItem = (ActionManager.itemApp)ListaAzioniPredefinite.SelectedItem;
                ListaAzioniPredefinite.Items.Remove(selectedItem);
                //add item to AzioniLuogo
                ListaAzioniLuogo.Items.Add(new ActionManager.itemApp { applicazione = selectedItem.applicazione, icon = selectedItem.icon, name = selectedItem.name, type = "app" });
            }
        }

        private void AggiungiAzioneFirewallButton_Click(object sender, RoutedEventArgs e)
        {

            if (ListaAzioniFirewall.SelectedItem != null)
            {
                //get selected item from ListaAzioniPredefinite
                ActionManager.itemApp selectedItem = (ActionManager.itemApp)ListaAzioniFirewall.SelectedItem;
                ListaAzioniFirewall.Items.Remove(selectedItem);
                //add item to AzioniLuogo
                ListaAzioniLuogo.Items.Add(new ActionManager.itemApp { applicazione = selectedItem.applicazione, icon = selectedItem.icon, name = selectedItem.name, type = "firewall" });
            }
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
                string oldname = place.NomeLuogo;
                place.ActionsList.SaveActions(ActionManager.SaveActions(ListaAzioniLuogo.Items));
                place.NomeLuogo = nome;
                if (!modify)
                {
                    if(!place.checkIfNameExist())
                        scanAndSave();
                    else 
                        MessageBox.Show("Place already exist");
                }
                else {
                    Luogo.updatePlace(place, oldname);
                }

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
                place.saveNextList();
                wInterface.WlanNotification -= notify;
                try {
                    if ( place.NetwList.Hash.Count != 0)

                        place.luogoToDB();
                    else {
                             MessageBox.Show("Cannot find informations here!");           
                    }
                       
                }
                catch (ConstraintException e)
                {
                    MessageBox.Show("Error saving the new place");
                } 
                
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
                ListaAzioniLuogo.Items.Add(new ActionManager.itemApp { applicazione = s, icon = icon, name = n, type="custom" });

            }
        }


        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ListaAzioniLuogo.Items.Remove(ListaAzioniLuogo.SelectedItem);
        }

        private void removeActionHandler(object sender, RoutedEventArgs e)
        {
            ActionManager.itemApp selectedItem = (ActionManager.itemApp)ListaAzioniLuogo.SelectedItem;
            if (selectedItem != null)
            {
                if (selectedItem.type == "app")
                {
                    ListaAzioniPredefinite.Items.Add(selectedItem);
                }
                if (selectedItem.type == "firewall")
                {
                    ListaAzioniFirewall.Items.Add(selectedItem);
                }

                ListaAzioniLuogo.Items.Remove(ListaAzioniLuogo.SelectedItem);

                    
            }

        }

    }
}
