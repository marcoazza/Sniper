using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WiFiLoc_App.Pages
{
    /// <summary>
    /// Interaction logic for ModLuogo.xaml
    /// </summary>
    public partial class ModLuogo : Page
    {
        ArrayList listLuoghi;
        public ModLuogo()
        {
            InitializeComponent();
            listLuoghi = Luogo.getPossibiliLuoghi();
            if (listLuoghi.Count > 0)
            {
                foreach (Luogo l in listLuoghi)
                {
                    LuoghiSalvati.Items.Add(l.NomeLuogo);
                }
            }
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
                Luogo l = new Luogo(NomeLuogo.Text);
                l.ActionsList.SaveActions(ActionManager.SaveActions(ListaAzioniLuogo.Items));
                Luogo.updatePlace(l,(string)LuoghiSalvati.SelectedItem);
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

        private void LuoghiSalvati_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Luogo selected=null;
            foreach(Luogo l in listLuoghi){
                if(l.NomeLuogo == LuoghiSalvati.SelectedItem){
                   selected =l;
                }
            }
            //Luogo l = (Luogo) LuoghiSalvati.SelectedItem;
            NomeLuogo.Text = selected.NomeLuogo;
            ListaAzioniLuogo.Items.Clear();
            ListaAzioniPredefinite.Items.Clear();
            ThreadPool.QueueUserWorkItem(ActionManager.getInstalledSoftware, ListaAzioniPredefinite);
            foreach (ActionList.Action a in selected.ActionsList.GetAll()) {
                ListaAzioniLuogo.Items.Add(ActionManager.getAssociatedItem(a.Path));
            
            }

        }


    }
}
