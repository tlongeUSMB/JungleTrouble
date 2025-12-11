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
using System.Windows.Threading;

namespace JungleTrouble
{
    
    public partial class UCJeu : UserControl
    {
        private DispatcherTimer minuterie;
        private double GRAVITY = 0.99;
        private double WIDTHPERSO = 18, HEIGHTPERSO = 30;
        private static readonly double[,] plateformes = { { 0, 20, 0, 800}, { 107.5, 127.5, 0, 650}, { 215, 235, 150, 800}, { 322.5, 342.5, 0, 650 } };
        private static readonly Rect hitboxPlateforme0 = new Rect(0, 0, 800, 20);
        private static readonly Rect hitboxPlateforme1 = new Rect(0, 107.5, 700, 20);
        private static readonly Rect hitboxPlateforme2 = new Rect(100, 215, 700, 20);
        private static readonly Rect hitboxPlateforme3 = new Rect(0, 322.5, 700, 20);
        public UCJeu()
        {
            InitializeComponent();
            InitializeTimer();

        }
        private void InitializeTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(1);
            minuterie.Tick += GameLoop;
            minuterie.Start();
        }
        private void GameLoop(object sender, EventArgs e)
        {
            Collision(imgPerso);
        }
        private void Collision(Image obj)
        {
            double objX = Canvas.GetLeft(obj);
            double objY = Canvas.GetBottom(obj);
            Rect hitboxPerso = new Rect(objX, objY, WIDTHPERSO, HEIGHTPERSO);

            if (!hitboxPerso.IntersectsWith(hitboxPlateforme0) && !hitboxPerso.IntersectsWith(hitboxPlateforme1) && !hitboxPerso.IntersectsWith(hitboxPlateforme2) && !hitboxPerso.IntersectsWith(hitboxPlateforme3))
            {
                Canvas.SetBottom(obj, objY * GRAVITY);
            }
            else if (hitboxPerso.IntersectsWith(hitboxPlateforme0))
            {
                Canvas.SetBottom(obj, plateformes[0, 1]);
            }
            else if (hitboxPerso.IntersectsWith(hitboxPlateforme1))
            {
                Canvas.SetBottom(obj, plateformes[1, 1]);
            }
            else if (hitboxPerso.IntersectsWith(hitboxPlateforme2))
            {
                Canvas.SetBottom(obj, plateformes[2, 1]);
            }
            else if (hitboxPerso.IntersectsWith(hitboxPlateforme3))
            {
                Canvas.SetBottom(obj, plateformes[3, 1]);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += canvasJeu_KeyDown;
        }

      

        private void canvasJeu_KeyDown(object sender, KeyEventArgs e)
        {
            double objX = Canvas.GetLeft(imgPerso);
            double objY = Canvas.GetBottom(imgPerso);
            Rect hitboxPerso = new Rect(objX, objY, WIDTHPERSO, HEIGHTPERSO);
            if (e.Key == Key.Right && Canvas.GetLeft(imgPerso) < 764)
                Canvas.SetLeft(imgPerso, Canvas.GetLeft(imgPerso) + 5);
            if (e.Key == Key.Left && Canvas.GetLeft(imgPerso) > 0)
                Canvas.SetLeft(imgPerso, Canvas.GetLeft(imgPerso) - 5);
            if (e.Key == Key.Up && (hitboxPerso.IntersectsWith(hitboxPlateforme0) || hitboxPerso.IntersectsWith(hitboxPlateforme1) || hitboxPerso.IntersectsWith(hitboxPlateforme2) || hitboxPerso.IntersectsWith(hitboxPlateforme3)))
                Saut();
        }

        private void Saut()
        {
            
        }
    }
}
