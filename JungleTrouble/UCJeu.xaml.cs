using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security;
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
        private double GRAVITY = -200;
        private double JUMPSTRENGTH = 140;
        private double WIDTHPERSO = 24, HEIGHTPERSO = 40;
        private double WHIDTHTONNEAU = 30, HEIGTHTONNEAU = 30;
        private bool verif = false;
        private string direction = "right";
        private static BitmapImage[] persos1 = new BitmapImage[8];
        private static readonly double[,] plateformes = { { 0, 20, 0, 800}, { 107.5, 127.5, 0, 650}, { 215, 235, 150, 800}, { 322.5, 342.5, 0, 650 } };
        private static readonly Rect hitboxPlateforme0 = new Rect(0, 0, 800, 20);
        private static readonly Rect hitboxPlateforme1 = new Rect(0, 107.5, 700, 20);
        private static readonly Rect hitboxPlateforme2 = new Rect(100, 215, 700, 20);
        private static readonly Rect hitboxPlateforme3 = new Rect(0, 322.5, 700, 20);
        private static readonly Rect hitboxLadder1 = new Rect(720, 20, 70, 195);
        private static readonly Rect hitboxLadder2 = new Rect(10, 127.5, 70, 195);
        private static readonly Rect hitboxLadder3 = new Rect(720, 235, 70, 195);
        private DateTime tempsPrecedent;
        private double vitesseVerticalePerso = 0.0;
        private double vitesseVerticaleTonneau = 0.0;
        private bool persoAuSol = false;
        private bool tonneauAuSol = false;
        private bool estSurEchelle = false;
        private double vitesseGrimpe = 200;
        private BitmapImage[] perso = new BitmapImage[3];

        public UCJeu()
        {
            
            InitializeComponent();
            InitializeTimer();
            InitializeImages();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ParametreWindow parametreWindow = new ParametreWindow();
            bool? rep = parametreWindow.ShowDialog();
        }
        

        private void InitializeTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(1);
            minuterie.Tick += GameLoop;
            tempsPrecedent = DateTime.Now;
            minuterie.Start();
        }
        private void GameLoop(object sender, EventArgs e)
        {
            EstSurPlateforme(imgPerso,WIDTHPERSO,HEIGHTPERSO);
            EstSurPlateforme(imgTonneau1, WHIDTHTONNEAU, HEIGTHTONNEAU);
            MoveTonneaux(2);
            colisionTonneau();
            DateTime tempsActuel = DateTime.Now;
            double deltaTime = (tempsActuel - tempsPrecedent).TotalSeconds;
            tempsPrecedent = tempsActuel;
            AppliquerPhysique(imgPerso,WIDTHPERSO,HEIGHTPERSO,ref vitesseVerticalePerso,ref persoAuSol,deltaTime);
            AppliquerPhysique(imgTonneau1,WHIDTHTONNEAU,HEIGTHTONNEAU,ref vitesseVerticaleTonneau,ref tonneauAuSol,deltaTime);
            MoveTonneaux(2);
            colisionTonneau();
            estSurEchelle = VerifierEchelle();
            if (!estSurEchelle)
            {
                AppliquerPhysique(
                    imgPerso,
                    WIDTHPERSO,
                    HEIGHTPERSO,
                    ref vitesseVerticalePerso,
                    ref persoAuSol,
                    deltaTime);
            }
            else
            {
                vitesseVerticalePerso = 0;
            }
        }
        private double HauteurPlateformeSousPerso(Image obj, double largeur, double hauteur, double yPrecedent)
        {
            double x = Canvas.GetLeft(obj);
            double y = Canvas.GetBottom(obj);
            Rect hitbox = new Rect(x, y, largeur, hauteur);
            double hauteurSol = -1;
            if (hitbox.IntersectsWith(hitboxPlateforme0) && yPrecedent >= plateformes[0, 1])
                hauteurSol = plateformes[0, 1];
            if (hitbox.IntersectsWith(hitboxPlateforme1) && yPrecedent >= plateformes[1, 1])
                hauteurSol = plateformes[1, 1];
            if (hitbox.IntersectsWith(hitboxPlateforme2) && yPrecedent >= plateformes[2, 1])
                hauteurSol = plateformes[2, 1];
            if (hitbox.IntersectsWith(hitboxPlateforme3) && yPrecedent >= plateformes[3, 1])
                hauteurSol = plateformes[3, 1];

            return hauteurSol;
        }
        private bool EstSurPlateforme(Image obj, double width, double height)
        {
            double x = Canvas.GetLeft(obj);
            double y = Canvas.GetBottom(obj);
            Rect hitbox = new Rect(x, y, width, height);

            return hitbox.IntersectsWith(hitboxPlateforme0)|| hitbox.IntersectsWith(hitboxPlateforme1) || hitbox.IntersectsWith(hitboxPlateforme2) || hitbox.IntersectsWith(hitboxPlateforme3);
        }

        private void AppliquerPhysique(Image obj,double largeur,double hauteur,ref double vitesseVerticale,ref bool estAuSol,double deltaTime)
        {
            double yPrecedent = Canvas.GetBottom(obj);
            double y = yPrecedent;
            vitesseVerticale += GRAVITY * deltaTime;
            y += vitesseVerticale * deltaTime;
            Canvas.SetBottom(obj, y);
            double hauteurSol = HauteurPlateformeSousPerso(obj, largeur, hauteur, yPrecedent);
            if (hauteurSol >= 0 && vitesseVerticale <= 0)
            {
                Canvas.SetBottom(obj, hauteurSol);
                vitesseVerticale = 0;
                estAuSol = true;
            }
            else
            {
                estAuSol = false;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += canvasJeu_KeyDown;
        }


        private int count = 0;
        private int countimage = 0;
        private void canvasJeu_KeyDown(object sender, KeyEventArgs e)
        {
            int pas = 1;
            double objX = Canvas.GetLeft(imgPerso);
            double objY = Canvas.GetBottom(imgPerso);
            Rect hitboxPerso = new Rect(objX, objY, WIDTHPERSO, HEIGHTPERSO);
            if (e.Key == Key.Right && Canvas.GetLeft(imgPerso) < 764)
            {
                pas++;
                Console.WriteLine(pas);
                Canvas.SetLeft(imgPerso, Canvas.GetLeft(imgPerso) + 5);
                imgPerso.Source = perso[(pas%3) + 1];
            }
            if (e.Key == Key.Left && Canvas.GetLeft(imgPerso) > 0)
                Canvas.SetLeft(imgPerso, Canvas.GetLeft(imgPerso) - 5);
            if (e.Key == Key.Up)
            {
                    Saut();
            }
            if (estSurEchelle)
            {
                double y = Canvas.GetBottom(imgPerso);
                if (e.Key == Key.Up)
                {
                    y += vitesseGrimpe * 0.016;
                    Canvas.SetBottom(imgPerso, y);
                }
                else if (e.Key == Key.Down)
                {
                    y -= vitesseGrimpe * 0.016;
                    Canvas.SetBottom(imgPerso, y);
                }
            }
        }

        private void Saut()
        {
            if (persoAuSol)
            {
                vitesseVerticalePerso = JUMPSTRENGTH;
                persoAuSol = false;
            }
        }

        private void MoveTonneaux(double pxmv)
        {
            if (Canvas.GetLeft(imgTonneau1) > 770)
            {
                if (verif == false)
                {
                    direction = "left";
                }
                verif = true;
            }

            else if (Canvas.GetLeft(imgTonneau1) <= 0)
            {
                if (Canvas.GetBottom(imgTonneau1) == 20)
                    Canvas.SetBottom(imgTonneau1, 400);
                if (verif == false)
                {
                    direction = "right";
                }
                verif = true;
            }
            else
            {
                verif = false;
            }
            if (direction == "right")
                Canvas.SetLeft(imgTonneau1, Canvas.GetLeft(imgTonneau1) + pxmv);
            else if (direction == "left")
                Canvas.SetLeft(imgTonneau1, Canvas.GetLeft(imgTonneau1) - pxmv);

        }


        private void colisionTonneau()
        {

            double objX = Canvas.GetLeft(imgTonneau1);
            double objY = Canvas.GetBottom(imgTonneau1);
            Rect hitboxtonneau = new Rect(objX, objY, WHIDTHTONNEAU, HEIGTHTONNEAU);
            double objV = Canvas.GetLeft(imgPerso);
            double objW = Canvas.GetBottom(imgPerso);
            Rect hitboxPerso = new Rect(objV, objW, WIDTHPERSO, HEIGHTPERSO);
            if(hitboxPerso.IntersectsWith(hitboxtonneau))
            {
                Canvas.SetLeft(imgPerso, 0);
                Canvas.SetBottom(imgPerso, 20);
            }

        }
        private void InitializeImages()
        {
            for (int i = 0; i < perso.Length; i++)
            {
                perso[i] = new BitmapImage(new Uri($"pack://application:,,,/Images/Perso{MainWindow.Perso}/perso{MainWindow.Perso + (i + 1)}.png"));
                Console.WriteLine("Image " + i + " " + perso[i]);
            }
            Console.WriteLine(perso);
        }

        private bool VerifierEchelle()
        {
            double x = Canvas.GetLeft(imgPerso);
            double y = Canvas.GetBottom(imgPerso);
            Rect hitboxPerso = new Rect(x, y, WIDTHPERSO, HEIGHTPERSO);

            return hitboxPerso.IntersectsWith(hitboxLadder1) ||
                   hitboxPerso.IntersectsWith(hitboxLadder2) ||
                   hitboxPerso.IntersectsWith(hitboxLadder3);
        }

    }
}
