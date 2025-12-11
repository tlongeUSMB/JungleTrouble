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
        private double GRAVITY = 0.9;
        private static readonly double[,] plateformes = { { 0, 20, 0, 800}, { 107.5, 127.5, 0, 650}, { 215, 235, 150, 800}, { 322.5, 342.5, 0, 650 } };
        public UCJeu()
        {
            InitializeComponent();
        }
        private void InitializeTimer(object sender, EventArgs e)
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Tick += GameLoop;
            minuterie.Start();
        }
        private void GameLoop(object sender, EventArgs e)
        {
            Collision(imgPerso);
        }
        private void Collision(Image obj)
        {
            double objYBottom = Canvas.GetBottom(obj);
            double objYTop = objYBottom + obj.Height;
            int nbPlateforme = 0;
            bool found = false;
            do
            {
                int i = 0;
                if ( objYBottom > plateformes[i, 1] && objYBottom <  plateformes[i, 0])
                {
                    nbPlateforme = i;
                    found = true;
                }
                i++;

            }
            while (!found);
            while (objYBottom > plateformes[nbPlateforme, 1])
            {
                Gravity(obj, objYBottom);
            }
        }
        private void Gravity(Image obj, double objYBottom)
        {
            double vitesseYGrav = 0;
            Canvas.SetBottom(obj, objYBottom - vitesseYGrav);
            vitesseYGrav += GRAVITY;
        }
    }
}
