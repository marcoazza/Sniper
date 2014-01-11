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
using NativeWifi;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using WiFiLoc_App;
using WiFiLoc_Service;
using WiFiLoc_App.Menu;
using MahApps.Metro;
using MahApps.Metro.Controls;

using System.Drawing;
using System.Threading;


namespace WiFiLoc_App
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
       public static Window win;

        public MainWindow()
        {
            InitializeComponent();
            win = this;
            
            _mainFrame.NavigationService.Navigate(new Uri("Pages/Home.xaml", UriKind.Relative));
            _menuFrame.NavigationService.Navigate(new Uri("Menu/MainMenu.xaml", UriKind.Relative));

        }




        void HandleClick(object sender, RoutedEventArgs args)
        {
            // Get the element that raised the event. 
            FrameworkElement fe = (FrameworkElement)args.OriginalSource;

            switch (fe.Name) {
                case "Home":
                    _mainFrame.NavigationService.Navigate(new Uri("Pages/Home.xaml", UriKind.Relative));
                    break;
                case "AddPlace":
                    _mainFrame.NavigationService.Navigate(new Uri("Pages/AddLuogo.xaml", UriKind.Relative));
                    break;
                case "Places":
                    _mainFrame.NavigationService.Navigate(new Uri("Pages/Places.xaml", UriKind.Relative));
                    break;
                case "Stats":
                    _mainFrame.NavigationService.Navigate(new Uri("Pages/ModLuogo.xaml", UriKind.Relative));
                    break;
            
            }
            
            
        }

        private void GestisciLuoghi_Click(object sender, RoutedEventArgs e)
        {
           _mainFrame.NavigationService.Navigate(new Uri("Pages/WelcomeLuogo.xaml", UriKind.Relative));
            _menuFrame.NavigationService.Navigate(new Uri("Menu/MenuLuogo.xaml", UriKind.Relative));
        }

        public void _mainFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.NavigationService.Navigate(new Uri("Pages/AppList.xaml", UriKind.Relative));
            _menuFrame.NavigationService.Navigate(new Uri("Menu/menuClear.xaml", UriKind.Relative));
        }


 
    }
}
