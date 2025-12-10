using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AfficheDemarrage();
        }
        private void AfficheDemarrage()
        {
            // crée et charge l'écran de démarrage
            UCRegles uc = new UCRegles();

            // associe l'écran au conteneur
            ZoneJeu.Content = uc;
            uc.butstart.Click += AfficherChoixPerso;
        }

        private void AfficherChoixPerso(object sender, RoutedEventArgs e)
        {
            UCChoixPerso uc = new UCChoixPerso();
            ZoneJeu.Content = uc;
        }


    }
}