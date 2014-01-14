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
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows;
namespace WiFiLoc_App
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
       public static Window win;

       private System.Windows.Forms.NotifyIcon ni;
       private System.Windows.Forms.ContextMenuStrip contextMenu = new System.Windows.Forms.ContextMenuStrip();
       private Uri home = new Uri("Pages/Home.xaml", UriKind.Relative);
       private Pages.Home homeObj = new Pages.Home();

    private bool  closing = false;
        public MainWindow()
        {
            InitializeComponent();
            win = this;

            _mainFrame.NavigationService.Navigate(homeObj);
            _menuFrame.NavigationService.Navigate(new Uri("Menu/MainMenu.xaml", UriKind.Relative));

               // Add menu items to context menu.
               ToolStripItem show = contextMenu.Items.Add("S&how interface");
               ToolStripItem exit = contextMenu.Items.Add("E&xit");


               show.Click += showInterface;
               exit.Click += exitApplication;


               ni = new System.Windows.Forms.NotifyIcon();
               ni.ContextMenuStrip = contextMenu;
               ni.Icon = new System.Drawing.Icon(SystemIcons.Application, 40, 40);
               ni.Visible = false;

               System.Windows.Forms.MouseEventHandler d;
               d = clickIconTray;
               ni.MouseClick += d;
  
        }


        private void clickIconTray(object sender, System.Windows.Forms.MouseEventArgs arg)
        {

            if (arg.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.WindowState = WindowState.Normal;
                this.ShowInTaskbar = true;
            }
            if (arg.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ni.ContextMenuStrip.Show();
            }

        }

        private void showInterface(object sender, EventArgs arg)
        {

               this.WindowState = WindowState.Normal;
               this.ShowInTaskbar = true;
               ni.Visible = false;

        }

        private void exitApplication(object sender, EventArgs arg)
        {
            ni.Visible = false;
            System.Windows.Application.Current.Shutdown();
            closing = true;

        }

        protected override void OnStateChanged(EventArgs e)
        {
            ni.BalloonTipText = "minimized";
            ni.ShowBalloonTip(1000);
            if (WindowState == WindowState.Minimized)
                ni.Visible = false;
            if (WindowState == WindowState.Normal)
            {
                ni.Visible = false;
                this.ShowInTaskbar = true;
            }
            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!closing)
            {
                this.WindowState = WindowState.Minimized;
                this.ShowInTaskbar = false;
                ni.Visible = true;
                base.OnClosing(e);
                e.Cancel = true;

            }


        }


        void HandleClick(object sender, RoutedEventArgs args)
        {
            // Get the element that raised the event. 
            FrameworkElement fe = (FrameworkElement)args.OriginalSource;


            switch (fe.Name) {
                case "Home":
                    _mainFrame.NavigationService.Navigate(homeObj);
                    break;
                case "AddPlace":
                    _mainFrame.NavigationService.Navigate(new Uri("Pages/AddLuogo.xaml", UriKind.Relative));
                    break;
                case "Places":
                    _mainFrame.NavigationService.Navigate(new Uri("Pages/Places.xaml", UriKind.Relative));
                    break;
                case "Stats":
                    _mainFrame.NavigationService.Navigate(new Uri("Pages/Stats.xaml", UriKind.Relative));
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
