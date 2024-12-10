using System.Media;
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
using System.Windows.Threading;

namespace Fusee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly int NBIMGCOIFFE = 1; //permet de modifier facilement le nombre de coiffes, pompes, ... disponibles
        private static readonly int NBIMGCORPS = 1;
        private static readonly int NBIMGAILERONS = 1;
        private static readonly int NBIMGPOMPE = 1;

        private static readonly int LARGEURFUSEE = 32;  //A voir si vraiment utile de conserver tailles
        private static readonly int HAUTEURCORPS = 64;
        private static readonly int HAUTEURCOIFFEAILERONS = 32;
        private static readonly int LARGEUROISEAU = 32;
        private static readonly int HAUTEUROISEAU = 32;

        private static BitmapImage[] imagesCoiffe;
        private static BitmapImage[] imagesCorps;
        private static BitmapImage[] imagesAilerons;
        private static BitmapImage[] imagesPompe;
        private static BitmapImage[] imagesOiseauUn;
        private static BitmapImage[] imagesOiseauDeux;
        private static BitmapImage[] imagesNuage;
        private static BitmapImage[] imagesNuageDeux;
        private static BitmapImage[] imagesAiguille;

        private static SoundPlayer[] sons;
        private static MediaPlayer[] musiques;

        List<BitmapImage> imgCoiffeDispos = new List<BitmapImage>(NBIMGCOIFFE-1); //Liste des objets disponibles à l'achat
        List<BitmapImage> imgCorpsDispos = new List<BitmapImage>(NBIMGCORPS-1);
        List<BitmapImage> imgAileronsDispos = new List<BitmapImage>(NBIMGAILERONS-1);
        List<BitmapImage> imgPompeDispos = new List<BitmapImage>(NBIMGPOMPE-1);
        List<BitmapImage> imgCoiffeAchetes = new List<BitmapImage>(1);  //Liste des objets déjà achetés
        List<BitmapImage> imgCorpsAchetes = new List<BitmapImage>(1);
        List<BitmapImage> imgAileronsAchetes = new List<BitmapImage>(1);
        List<BitmapImage> imgPompeAchetes = new List<BitmapImage>(1);

        private static int score = 0;
        private static int meilleurScore = 0;
        private static int argentDispo = 0;
        private static int volumeMusiques = 100;
        private static int volumeSons = 100;
        private static bool tremblements = true;

        private static DispatcherTimer minuterie;
        public MainWindow()
        {
            InitializeComponent();
            InitTimer();
            InitBitmaps();
            InitSons();
            InitMusiques();
        }

        private void fenetrePrincipale_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void InitTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Tick += Jeu;
            minuterie.Start();
        }

        private void Jeu(object? sender, EventArgs e)
        {
            
        }

        private void InitBitmaps()
        {

        }

        private void InitSons()
        {

        }

        private void InitMusiques()
        {

        }
    }
}