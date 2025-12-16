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
        private string[] direction = { "right", "right", "right" };
        private static BitmapImage[] persos1 = new BitmapImage[8];
        private static readonly double[,] plateformes = { { 0, 20, 0, 800}, { 107.5, 127.5, 0, 650}, { 215, 235, 150, 800}, { 322.5, 342.5, 0, 650 } };
        private static readonly Rect hitboxPlateforme0 = new Rect(0, 0, 800, 20);
        private static readonly Rect hitboxPlateforme1 = new Rect(0, 107.5, 700, 20);
        private static readonly Rect hitboxPlateforme2 = new Rect(100, 215, 700, 20);
        private static readonly Rect hitboxPlateforme3 = new Rect(0, 322.5, 700, 20);
        private static readonly Rect hitboxLadder1 = new Rect(720, 20, 70, 195);
        private static readonly Rect hitboxLadder2 = new Rect(10, 127.5, 70, 195);
        private static readonly Rect hitboxLadder3 = new Rect(720, 235, 70, 195);
        private static readonly Rect hitboxGorille = new Rect(0, 342.5, 80, 100);
        private DateTime tempsPrecedent;
        private double vitesseVerticalePerso = 0.0;
        private double[] vitesseVerticaleTonneau = { 0.0, 0.0, 0.0 };
        private bool persoAuSol = false;
        private bool[] tonneauAuSol = { false, false, false };
        private bool estSurEchelle = false;
        private double vitesseGrimpe = 200;
        private BitmapImage[] persoMarche = new BitmapImage[3];
        private BitmapImage[] gorilles = new BitmapImage[3];
        private int pas = 0;
        private double nbTonneaux = 1;
        private double vitesseTonneaux = 2;
        private Image[] imgTonneau = new Image[3];
        private int vies = 3;
        public bool perdu = false;
        public bool win = false;
        private int minuteri = 0;
        private bool testg = true;

        public UCJeu()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeImages();
            animegorille();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ParametreWindow parametreWindow = new ParametreWindow();
            bool? rep = parametreWindow.ShowDialog();
            nbTonneaux = parametreWindow.Slidnombre.Value;
            vitesseTonneaux = parametreWindow.Slidvitesse.Value;
            Console.WriteLine("Nombre de tonneaux : " + nbTonneaux);
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
            for (int i = 0; i < nbTonneaux; i++)
                tonneauAuSol[i] = EstSurPlateforme(imgTonneau[i], WHIDTHTONNEAU, HEIGTHTONNEAU);
            MoveTonneaux(vitesseTonneaux);
            colisionTonneau();
            DateTime tempsActuel = DateTime.Now;
            double deltaTime = (tempsActuel - tempsPrecedent).TotalSeconds;
            tempsPrecedent = tempsActuel;
            AppliquerPhysique(imgPerso,WIDTHPERSO,HEIGHTPERSO,ref vitesseVerticalePerso,ref persoAuSol,deltaTime);
            AppliquerPhysique(imgTonneau[0],WHIDTHTONNEAU,HEIGTHTONNEAU,ref vitesseVerticaleTonneau[0],ref tonneauAuSol[0],deltaTime);
            CompteTonneaux(deltaTime);
            MoveTonneaux(vitesseTonneaux);
            colisionTonneau();
            win = WinCheck();
            estSurEchelle = VerifierEchelle();
            if (!estSurEchelle)
            {
                AppliquerPhysique(imgPerso,WIDTHPERSO,HEIGHTPERSO,ref vitesseVerticalePerso,ref persoAuSol,deltaTime);
            }
            else
            {
                vitesseVerticalePerso = 0;
                imgPerso.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/Perso{MainWindow.Perso}/perso{MainWindow.Perso}grimpe1.png"));
            }
            DeplaceHearts();
            if (vies <= 0)
            {
                minuterie.Stop();
                perdu = true;
                AfficherFin();
            }
            else if (win)
            {
                minuterie.Stop();
                AfficherFin();
            }
        }

        private bool WinCheck()
        {
            double x = Canvas.GetLeft(imgPerso);
            double y = Canvas.GetBottom(imgPerso);
            Rect hitboxPerso = new Rect(x, y, WIDTHPERSO, HEIGHTPERSO);
            return hitboxPerso.IntersectsWith(hitboxGorille);
        }

        private void AfficherFin()
        {
            if (perdu)
            {
                labFin.Content = "Game Over !";
                labFin.Foreground = Brushes.Red;
                imgPerso.Visibility = Visibility.Hidden;
            }
            else if (win)
            {
                labFin.Content = "Bien joué !";
                labFin.Foreground = Brushes.Green;
            }
            imgParcheminFin.Visibility = Visibility.Visible;
            labFin.Visibility = Visibility.Visible;
            butRestart.Visibility = Visibility.Visible;
        }

        private void butRestart_Click(object sender, RoutedEventArgs e)
        {
            minuterie?.Stop();
            ((MainWindow)Application.Current.MainWindow).AfficheDemarrage();
        }

        private void CompteTonneaux(double deltaTime)
        {
            if (nbTonneaux == 1)
            {
                imgTonneau2.Visibility = Visibility.Hidden;
                imgTonneau3.Visibility = Visibility.Hidden;
            }
            else if (nbTonneaux == 2)
            {
                AppliquerPhysique(imgTonneau[1], WHIDTHTONNEAU, HEIGTHTONNEAU, ref vitesseVerticaleTonneau[1], ref tonneauAuSol[1], deltaTime);
                imgTonneau2.Visibility = Visibility.Visible;
                imgTonneau3.Visibility = Visibility.Hidden;
            }
            else if (nbTonneaux == 3)
            {
                AppliquerPhysique(imgTonneau[1], WHIDTHTONNEAU, HEIGTHTONNEAU, ref vitesseVerticaleTonneau[1], ref tonneauAuSol[1], deltaTime);
                AppliquerPhysique(imgTonneau[2], WHIDTHTONNEAU, HEIGTHTONNEAU, ref vitesseVerticaleTonneau[2], ref tonneauAuSol[2], deltaTime);
                imgTonneau2.Visibility = Visibility.Visible;
                imgTonneau3.Visibility = Visibility.Visible;
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
            return hitbox.IntersectsWith(hitboxPlateforme0) || hitbox.IntersectsWith(hitboxPlateforme1) || hitbox.IntersectsWith(hitboxPlateforme2) || hitbox.IntersectsWith(hitboxPlateforme3);
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
            if (perdu || win)
                return;
            double objX = Canvas.GetLeft(imgPerso);
            double objY = Canvas.GetBottom(imgPerso);
            Rect hitboxPerso = new Rect(objX, objY, WIDTHPERSO, HEIGHTPERSO);
            if (e.Key == Key.Right && Canvas.GetLeft(imgPerso) < 764)
            {
                pas++;
                Canvas.SetLeft(imgPerso, Canvas.GetLeft(imgPerso) + 5);
                imgPerso.RenderTransform = new ScaleTransform(1, 1, imgPerso.Width / 2, imgPerso.Height / 2);
                if (pas % 9 < 3)
                {
                    imgPerso.Source = persoMarche[0];
                }
                else if (pas % 9 < 6 && pas % 9 >= 3)
                {
                    imgPerso.Source = persoMarche[1];
                }
                else
                {
                    imgPerso.Source = persoMarche[2];
                }
            }
            if (e.Key == Key.Left && Canvas.GetLeft(imgPerso) > 0)
            {
                pas++;
                Canvas.SetLeft(imgPerso, Canvas.GetLeft(imgPerso) - 5);
                imgPerso.RenderTransform = new ScaleTransform(-1, 1, imgPerso.Width / 2, imgPerso.Height / 2);
                if (pas % 9 < 3)
                {
                    imgPerso.Source = persoMarche[0];
                }
                else if (pas % 9 < 6 && pas % 9 >= 3)
                {
                    imgPerso.Source = persoMarche[1];
                }
                else
                {
                    imgPerso.Source = persoMarche[2];
                }
            }
            if (e.Key == Key.Up)
            {
                Saut();
            }
            if (estSurEchelle)
            {
                double y = Canvas.GetBottom(imgPerso);
                if (e.Key == Key.Up && !hitboxPerso.IntersectsWith(hitboxPlateforme1) && !hitboxPerso.IntersectsWith(hitboxPlateforme2) && !hitboxPerso.IntersectsWith(hitboxPlateforme3))
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

        private void DeplaceHearts()
        {
            double y = Canvas.GetBottom(imgPerso) + HEIGHTPERSO;
            double x = Canvas.GetLeft(imgPerso);
            Canvas.SetBottom(imgHeart1, y);
            Canvas.SetLeft(imgHeart1, x);
            Canvas.SetBottom(imgHeart2, y);
            Canvas.SetLeft(imgHeart2, x + 10);
            Canvas.SetBottom(imgHeart3, y);
            Canvas.SetLeft(imgHeart3, x + 20);
            if (vies == 3)
            {
                imgHeart1.Visibility = Visibility.Visible;
                imgHeart2.Visibility = Visibility.Visible;
                imgHeart3.Visibility = Visibility.Visible;
            }
            else if (vies == 2)
            {
                imgHeart1.Visibility = Visibility.Visible;
                imgHeart2.Visibility = Visibility.Visible;
                imgHeart3.Visibility = Visibility.Hidden;
            }
            else if (vies == 1)
            {
                imgHeart1.Visibility = Visibility.Visible;
                imgHeart2.Visibility = Visibility.Hidden;
                imgHeart3.Visibility = Visibility.Hidden;
            }
            else if (vies <= 0)
            {
                imgHeart1.Visibility = Visibility.Hidden;
                imgHeart2.Visibility = Visibility.Hidden;
                imgHeart3.Visibility = Visibility.Hidden;
            }
        }

        private void canvasJeu_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.Left)
            {
                imgPerso.Source = persoMarche[0];
            }
        }

        private void Saut()
        {
            if (persoAuSol)
            {
                imgPerso.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/Perso{MainWindow.Perso}/perso{MainWindow.Perso}saut.png"));
                vitesseVerticalePerso = JUMPSTRENGTH;
                persoAuSol = false;
            }
        }

        private void MoveTonneaux(double pxmv)
        {
            for (int i = 0; i < nbTonneaux; i++)
            {
                if (Canvas.GetLeft(imgTonneau[i])==30 && Canvas.GetBottom(imgTonneau[i])==415)
                    direction[i] = "right";
                if (Canvas.GetLeft(imgTonneau[i]) > 770)
                {
                    if (verif == false)
                    {
                        direction[i] = "left";
                    }
                    verif = true;
                }

                else if (Canvas.GetLeft(imgTonneau[i]) <= 0)
                {
                    if (Canvas.GetBottom(imgTonneau[i]) == 20)
                        Canvas.SetBottom(imgTonneau[i], 400);
                    if (verif == false)
                    {
                        direction[i] = "right";
                    }
                    verif = true;
                }
                else
                {
                    verif = false;
                }

                if (direction[i] == "right")
                    Canvas.SetLeft(imgTonneau[i], Canvas.GetLeft(imgTonneau[i]) + pxmv);
                else if (direction[i] == "left")
                    Canvas.SetLeft(imgTonneau[i], Canvas.GetLeft(imgTonneau[i]) - pxmv);
            }
        }


        private void colisionTonneau()
        {
            for (int i = 0; i < nbTonneaux; i++)
            {
                double X = Canvas.GetLeft(imgTonneau[i]);
                double Y = Canvas.GetBottom(imgTonneau[i]);
                Rect hitboxtonneau = new Rect(X, Y, WHIDTHTONNEAU, HEIGTHTONNEAU);
                double objV = Canvas.GetLeft(imgPerso);
                double objW = Canvas.GetBottom(imgPerso);
                Rect hitboxPerso = new Rect(objV, objW, WIDTHPERSO, HEIGHTPERSO);
                if (hitboxPerso.IntersectsWith(hitboxtonneau))
                {
                    Canvas.SetLeft(imgTonneau[i], 30);
                    Canvas.SetBottom(imgTonneau[i], 415);
                    Canvas.SetLeft(imgPerso, 0);
                    Canvas.SetBottom(imgPerso, 20);
                    vies--;
                }
            }
        }
        private void InitializeImages()
        {
            for (int i = 0; i < persoMarche.Length; i++)
            {
                persoMarche[i] = new BitmapImage(new Uri($"pack://application:,,,/Images/Perso{MainWindow.Perso}/perso{MainWindow.Perso + (i + 1)}.png"));
                Console.WriteLine("Image " + i + " " + persoMarche[i]);
            }
            for(int i = 0; i < 3; i++)
            {
                gorilles[i] = new BitmapImage(new Uri($"pack://application:,,,/Images/gorille/gorille{i}.png"));
                Console.WriteLine("Image " + i + " " + gorilles[i]);
            }
            Console.WriteLine(persoMarche);
            imgTonneau[0] = imgTonneau1;
            imgTonneau[1] = imgTonneau2;
            imgTonneau[2] = imgTonneau3;
            imgTonneau2.Visibility = Visibility.Hidden;
            imgTonneau3.Visibility = Visibility.Hidden;
        }
        private bool VerifierEchelle()
        {
            double x = Canvas.GetLeft(imgPerso);
            double y = Canvas.GetBottom(imgPerso);
            Rect hitboxPerso = new Rect(x, y, WIDTHPERSO, HEIGHTPERSO);
            return hitboxPerso.IntersectsWith(hitboxLadder1) || hitboxPerso.IntersectsWith(hitboxLadder2) || hitboxPerso.IntersectsWith(hitboxLadder3);
        }


        private void animegorille()
        {
            for (int i = 0; i < nbTonneaux; i++)
            {
                
                double X = Canvas.GetLeft(imgTonneau[i]);
                double Y = Canvas.GetBottom(imgTonneau[i]);
                imgkonkeydong.Source = gorilles[2];
                if (Canvas.GetBottom(imgTonneau[i]) >= 400 || testg== true)
                {
                    imgkonkeydong.Source = gorilles[0];
                    testg = false;

                }
                if (minuteri == 4)
                {
                    imgkonkeydong.Source = gorilles[1];
                    testg = true;
                }
            }
        }
    }
}