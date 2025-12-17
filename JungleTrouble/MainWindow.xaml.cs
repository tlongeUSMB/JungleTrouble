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
using System.Xml.Serialization;

namespace JungleTrouble
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double volume = 0.5;
        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer.Open(new System.Uri("Musique/musique.mp3", System.UriKind.Relative));
            mediaPlayer.Play();
            AfficheDemarrage();
        }

        public MediaPlayer mediaPlayer = new MediaPlayer();

        public void AfficheDemarrage()
        {
            UCRegles uc = new UCRegles();

            ZoneJeu.Content = uc;
            uc.butstart.Click += AfficherChoixPerso;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ParametreWindow parametreWindow = new ParametreWindow();
            bool? rep = parametreWindow.ShowDialog();
        }

        private void AfficheDemarrage(object sender, RoutedEventArgs e)
        {
            AfficheDemarrage();
        }

        private void AfficherChoixPerso(object sender, RoutedEventArgs e)
        {
            UCChoixPerso uc = new UCChoixPerso();
            ZoneJeu.Content = uc;
            uc.butJouer.Click += AfficherJeu;
        }

        private void AfficherJeu(object sender, RoutedEventArgs e)
        {
            UCJeu uc = new UCJeu();
            ZoneJeu.Content = uc;
        }

        public static string Perso { get; set; }
    }
}