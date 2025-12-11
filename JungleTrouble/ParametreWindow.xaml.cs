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

namespace JungleTrouble
{
    /// <summary>
    /// Logique d'interaction pour ParametreWindow.xaml
    /// </summary>
    public partial class ParametreWindow : Window
    {
        public ParametreWindow()
        {
            InitializeComponent();
        }

        private void butOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void butclause_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
