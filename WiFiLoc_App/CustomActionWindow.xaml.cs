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
using System.Windows.Shapes;

namespace WiFiLoc_App
{
    /// <summary>
    /// Interaction logic for CustomActionWindow.xaml
    /// </summary>
    public partial class CustomActionWindow : Window
    {
        public CustomActionWindow()
        {
            InitializeComponent();
        }

        private void CustomAction_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".exe"; // Default file extension
            dlg.Filter = "Exe Files (.exe)|*.exe|Script Files (.bat)|*.bat "; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            
            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                ActionPath.Text = filename;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
