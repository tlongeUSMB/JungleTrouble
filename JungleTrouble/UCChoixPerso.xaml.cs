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

namespace JungleTrouble
{
    /// <summary>
    /// Logique d'interaction pour UCChoixPerso.xaml
    /// </summary>
    public partial class UCChoixPerso : UserControl
    {
        public UCChoixPerso()
        {
            InitializeComponent();
        }

        private void rbperso1_Click(object sender, RoutedEventArgs e)
        {
            butJouer.IsEnabled = true;
        }
        private void rbperso2_Click(object sender, RoutedEventArgs e)
        {
            butJouer.IsEnabled = true;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ParametreWindow parametreWindow = new ParametreWindow();
            bool? rep = parametreWindow.ShowDialog();
        }

    }
}
