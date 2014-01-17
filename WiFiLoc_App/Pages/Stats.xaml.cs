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
using System.Windows.Controls.DataVisualization.Charting;
using WiFiLoc_App;
using System.Collections;

namespace WiFiLoc_App.Pages
{
    /// <summary>
    /// Logica di interazione per Stats.xaml
    /// </summary>
    public partial class Stats : Page
    {
        public Stats()
        {
            InitializeComponent();
            List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>() ;
            ArrayList luoghi = Luogo.getPossibiliLuoghi();
            foreach (Luogo l in luoghi )
            {
                items.Add(new KeyValuePair<string, int>(l.NomeLuogo, (int)l.Timestat));
            
            }
            ((PieSeries)mcChart.Series[0]).ItemsSource = items.ToArray();

        }
    }
}
