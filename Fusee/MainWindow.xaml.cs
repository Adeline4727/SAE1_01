using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Fusee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region constantes
        //Caractéristiques du jeu
        private static readonly int VITESSEOISEAUX = 400;
        private static readonly int DEPLACEMENTBOUTEILLE = 25;
        private static readonly int VITESSEENPLUSCLICVERT = 200;            //5.55555; //en m.s-1
        private static readonly int VITESSEENPLUSCLICJAUNE = 150;               //4.16666;
        private static readonly int VITESSEENPLUSCLICROUGE = 100;               //2.77777;//Faire recherches et calculs physique
        private static readonly int ACCELERATIONFUSEE = -1;                //-10.00000; //en m.s-2
        private static readonly int DEPLACEMENTOISEAUX = 8;
        private static readonly int NBCHANCESMINIMUM = 5;
        private static readonly int NBCHANCESMAXIMUM = 10;
        private static readonly int VITESSEFINVOL = 150;
        //Nombre de chaque images pour en rajouter facilement et faciliter les chargements
        private static readonly int NBIMGCOIFFE = 1;
        private static readonly int NBIMGAILERONS = 1;
        private static readonly int NBIMGCORPS = 1;
        private static readonly int NBIMGPOMPE = 1;
        private static readonly int NBIMAGESOISEAU = 2;
        private static readonly int NBIMAGESAIGUILLE = 8;
        private static readonly int NBIMAGESEAU = 3;
        private static readonly int NBETATSEAU = 2;
        private static readonly int NBIMAGESEAUTIREE = 1;
        //Nombre d'oiseaux
        private static readonly int NBOISEAUXENJEUGAUCHE = 1;
        private static readonly int NBOISEAUXENJEUDROITE = 1;
        //Canvas.Top de départs
        private static readonly int POSDEPARTSOCLE = 566;
        private static readonly int POSDEPARTPOMPE = 502;
        private static readonly int POSDEPARTAIGUILLE = 576;
        private static readonly int POSDEPARTJAUGE = 576;
        //Canvas.Left de départ
        private static readonly int POSDEPARTCORPS = 539;
        private static readonly int POSDEPARTAILERONS = 512;
        private static readonly int POSDEPARTCOIFFE = 539;
        private static readonly int POSDEPARTEAU = 539;
        //Taille des oiseaux
        private static readonly int LARGEUROISEAU = 64;
        private static readonly int HAUTEUROISEAU = 64;
        //
        private static readonly int FREQUENCERAFRAICHISSEMENT = 60;
        private static readonly int TICKSENTREIMGAIGUILLE = 5;
        #endregion
        #region Tableaux, listes
        //Tableaux de Bitmaps
        private static BitmapImage[] imagesCoiffe;
        private static BitmapImage[] imagesCorps;
        private static BitmapImage[] imagesAilerons;
        private static BitmapImage[] imagesPompe;
        private static BitmapImage[] imagesOiseauGauche;
        private static BitmapImage[] imagesOiseauDroite;
        private static BitmapImage[] imagesNuage;
        private static BitmapImage[] imagesNuageDeux;
        private static BitmapImage[] imagesAiguille;
        private static BitmapImage[] imagesEauTiree;
        private static BitmapImage[,] imagesEau;
        //Tableaux de sons et de musiques
        private static SoundPlayer[] sons;
        private static MediaPlayer[] musiques;
        //Tableaux d'images
        private static Image[] oiseauxEnJeuGauche;
        private static Image[] oiseauxEnJeuDroite;
        private static Image[] nuages;
        //Listes pour les objets personnalisables
        List<BitmapImage> imgCoiffeDispos = new List<BitmapImage>(NBIMGCOIFFE-1); //Liste des objets disponibles à l'achat
        List<BitmapImage> imgCorpsDispos = new List<BitmapImage>(NBIMGCORPS-1);
        List<BitmapImage> imgAileronsDispos = new List<BitmapImage>(NBIMGAILERONS-1);
        List<BitmapImage> imgPompeDispos = new List<BitmapImage>(NBIMGPOMPE-1);
        List<BitmapImage> imgCoiffeAchetes = new List<BitmapImage>(1);  //Liste des objets déjà achetés
        List<BitmapImage> imgCorpsAchetes = new List<BitmapImage>(1);
        List<BitmapImage> imgAileronsAchetes = new List<BitmapImage>(1);
        List<BitmapImage> imgPompeAchetes = new List<BitmapImage>(1);
        #endregion
        #region variables globales
        //Variables globales
        private static int score = 0;
        private static int meilleurScore = 0;
        private static int argentDispo = 0;
        private static int volumeMusiques = 100; //Pour paramètres de son
        private static int volumeSons = 100;
        private static int coiffeSelectionnee = 1; //Pour pouvoir personnaliser après
        private static int corpsSelectionne = 1;
        private static int aileronsSelectionnes = 1;
        private static int pompeSelectionnee = 1;
        private static int vitesse;
        private static int nbChances;
        private static int numChance;
        private static int numAiguille;
        private static int etatEau = 0;
        

        private static bool tremblements = true; //Pour activer/desactiver tremblements de la fusée (vent)
        private static bool gauche, droite;
        private static bool minuterieActive;
        private static bool vol;
        private static bool jauge;
        private static bool jaugeEnCours = false;
        private static bool cliqueEspace = false;
        private static bool continuer1 = false;
        private static bool continuer2 = false;

        private static Random rnd;

        private static DispatcherTimer minuterie;
        #endregion
        #region mainwindow

        public MainWindow()
        {
            rnd = new Random();
            InitializeComponent();
            Pause pause = new Pause();
            if (pause.ShowDialog() == true)
            {
                InitTimer();
                InitBitmaps();
                InitSons();
                InitMusiques();
                InitOiseaux();
                InitNuages(); //A coder, initialiser nuages en C# et pas xaml
                jauge = true;
                vol = false;
            }
            else
            {
                this.Close();
            }
        }
        #endregion
        #region touches appuyées

        private void fenetrePrincipale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                gauche = true;
                droite= false;
            }
            if (e.Key == Key.Right)
            {
                gauche = false;
                droite = true;
            }
            if (e.Key == Key.Enter)
            {
                if (minuterieActive == true)
                {
                    minuterie.Stop();
                    minuterieActive = false;
                    MessagePause();
                }
                else
                {
                    minuterie.Start();
                    minuterieActive = true;
                    FinMessage();
                }
                
            }
            if (e.Key == Key.Space)
            {
                if (jaugeEnCours == true)
                {
                    cliqueEspace = true;
                }
                else if (!jauge && !vol)
                {
                    if(!continuer1 && !continuer2)
                    {
                        continuer1 = true;
                    }
                    else if (continuer1 && !continuer2)
                    {
                        continuer2 = true;
                    }
                }
            }
        }
        #endregion
        #region timer
        private void InitTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Tick += Jeu;
            minuterie.Start();
            minuterieActive = true;
        }
        #endregion
        #region Boucle du jeu
        private void Jeu(object? sender, EventArgs e)
        {
#if DEBUG
            Console.WriteLine(" jauge : " + jauge);
            Console.WriteLine(" vol : " + vol);
#endif
            if (jauge == true && vol == false)
            {
                Jauge();
            }
            else if (vol == true && jauge == false)
            {
                DeplacementFusee(imgCoiffe);
                DeplacementFusee(imgCorps);
                DeplacementFusee(imgAilerons);
                DeplacementFusee(imgEau);
                DeplacementOiseau();
                DeplacementVertOiseaux(); 
                DeplacementNuages(); 
                Score();
                vitesse += ACCELERATIONFUSEE;
#if DEBUG
                Console.WriteLine("Left: " + Canvas.GetLeft(imgNuage1) + " Top: " + Canvas.GetTop(imgNuage1));
#endif
                if (Collision())
                {
#if DEBUG
                    Console.WriteLine("Collision");
#endif
                    FinVol();
                    MessageCollision();
                }
                if (vitesse <= VITESSEFINVOL)
                {
#if DEBUG
                    Console.WriteLine("Fin du vol");
#endif
                    FinVol();
                    MessageFinVol();
                }
                gauche = false;
                droite = false;
            }
            else
            {
                if (!continuer1)
                {

                }
                else
                {
                    if (!continuer2)
                    {
                        MessageContinuer();
                    }
                    else
                    {
                        FinMessage();
                        jauge = true;
                        vol = false;
                        continuer1 = false;
                        continuer2 = false;
                        score = 0; //Reinitialise le score
                        labScore.Content = "Score : " + score;
                    }
                }
            }
           
        }
        #endregion
        #region Pendant la jauge
        private void Jauge()
        {
            imgAiguille.Source = imagesAiguille[0];
            if (jaugeEnCours == false)
            {
                jaugeEnCours = true;
                numAiguille = 0;
                numChance = 1;
                nbChances = rnd.Next(NBCHANCESMINIMUM, NBCHANCESMAXIMUM);
                vitesse = 0;
                MessageJauge();
            }
            if (jaugeEnCours)
            {
                numAiguille++;
                imgAiguille.Source = imagesAiguille[numAiguille/ TICKSENTREIMGAIGUILLE % 8];
                if (cliqueEspace)
                {

                    cliqueEspace = false;
                    if((numAiguille / TICKSENTREIMGAIGUILLE % 8)==4 || (numAiguille / TICKSENTREIMGAIGUILLE % 8) == 0)
                    {
                        vitesse += VITESSEENPLUSCLICROUGE;
                    }
                    else if ((numAiguille / TICKSENTREIMGAIGUILLE % 8) == 1 || (numAiguille / TICKSENTREIMGAIGUILLE % 8) == 3 || (numAiguille / TICKSENTREIMGAIGUILLE % 8) == 5 || (numAiguille / TICKSENTREIMGAIGUILLE % 7) == 7)
                    {
                        vitesse += VITESSEENPLUSCLICJAUNE;
                    }
                    else
                    {
                        vitesse += VITESSEENPLUSCLICVERT;
                    }
                    numChance++;
#if DEBUG
                    Console.WriteLine("numChance : " + numChance);
                    Console.WriteLine("nbChance : " + nbChances);
#endif
                    if (numChance > nbChances)
                    {
                        jaugeEnCours = false;
                        jauge = false;
                        imgAiguille.Source = imagesAiguille[0];
                        FinMessage();
                        DebutVol();
                    }
                }
            }
        }
        #endregion
        #region déplacement de la fusée
        private void DeplacementFusee(Image img)
        {
            double nouvellePosition = Canvas.GetLeft(img);
            if (gauche && !droite)
                nouvellePosition -= DEPLACEMENTBOUTEILLE;
            else if (droite && !gauche)
                nouvellePosition += DEPLACEMENTBOUTEILLE;

            if (nouvellePosition <= (canFond.ActualWidth-imgAilerons.ActualWidth/2-img.ActualWidth/2) && nouvellePosition > (imgAilerons.ActualWidth/2 -img.ActualWidth/2))
            {
                Canvas.SetLeft(img, nouvellePosition);
            }

        }
        #endregion
        #region Déplacement horizontal de l'oiseau
        private void DeplacementOiseau()
        {
            double nouvellePosition;
            //Oiseaux venant de la gauche
            for (int i = 0;i < oiseauxEnJeuGauche.Length; i++)
            {
                nouvellePosition = Canvas.GetLeft(oiseauxEnJeuGauche[i]);
                nouvellePosition += DEPLACEMENTOISEAUX;
                if (nouvellePosition >=canFond.Width)
                {
                    RepositionneOiseau(oiseauxEnJeuGauche[i], true);
                }
                Canvas.SetLeft(oiseauxEnJeuGauche[i], nouvellePosition);
            }
            //Oiseaux venant de la droite
            for (int i = 0; i < oiseauxEnJeuDroite.Length; i++)
            {
                nouvellePosition = Canvas.GetLeft(oiseauxEnJeuDroite[i]);
                nouvellePosition -= DEPLACEMENTOISEAUX;
                if (nouvellePosition<=-LARGEUROISEAU)
                {
                    RepositionneOiseau(oiseauxEnJeuDroite[i], false);
                }
                Canvas.SetLeft(oiseauxEnJeuDroite[i], nouvellePosition);
            }
        }
        #endregion
        #region Changements de fin du vol
        private void FinVol()
        {
#if DEBUG
            Console.WriteLine("Ca marche");
#endif
            Canvas.SetTop(imgSol, canFond.Height-imgSol.Height);
#if DEBUG
            Console.WriteLine("Ca marche");
#endif//positions du décor
            Canvas.SetTop(imgSocle, POSDEPARTSOCLE);
            Canvas.SetTop(imgPompe, POSDEPARTPOMPE);
            Canvas.SetTop(imgAiguille, POSDEPARTAIGUILLE);
            Canvas.SetTop(imgJauge, POSDEPARTJAUGE);
            for (int i = 0; i < oiseauxEnJeuDroite.Length; i++) //Position des oiseaux
            {
                RepositionneOiseau(oiseauxEnJeuDroite[i], false);
            }
            for (int i = 0;i<oiseauxEnJeuGauche.Length; i++)
            {
                RepositionneOiseau(oiseauxEnJeuGauche[i], true);
            }
            Canvas.SetLeft(imgAilerons, POSDEPARTAILERONS); //Position de la fusée et de l'eau
            Canvas.SetLeft(imgCoiffe, POSDEPARTCOIFFE);
            Canvas.SetLeft(imgCorps, POSDEPARTCORPS);
            Canvas.SetLeft(imgEau, POSDEPARTEAU);
#if DEBUG
            Console.WriteLine("Ca marche");
#endif
            for (int i = 0; i< nuages.Length;i++) //Repositionne les nuages
            {
                RepositionneNuage(nuages[i]);
            }
            etatEau = 0; //met eau dans etat de base
            vol = false; //fin du vol

        }
        #endregion
        #region Debut du vol
        private void DebutVol()
        {
            Canvas.SetTop(imgSol, canFond.Height + imgSol.Height);
            Canvas.SetTop(imgSocle, canFond.Height+imgSocle.Height);
            Canvas.SetTop(imgPompe, canFond.Height + imgPompe.Height);
            Canvas.SetTop(imgAiguille, canFond.Height+ imgAiguille.Height);
            Canvas.SetTop(imgJauge, canFond.Height + imgJauge.Height);
            vol = true;
            jauge = false;
        }
        #endregion
        #region Tests des collisions
        private bool Collision()
        { 
            //Tableau des ennemis à tester
            int nboiseaux = oiseauxEnJeuGauche.Length + oiseauxEnJeuDroite.Length;
            Image[] oiseauxEnjeu = new Image[nboiseaux];
            for (int i = 0; i<oiseauxEnJeuGauche.Length; i++)
            {
                oiseauxEnjeu[i] = oiseauxEnJeuGauche[i];
            }
            for (int i = oiseauxEnJeuGauche.Length; i< oiseauxEnJeuGauche.Length + oiseauxEnJeuDroite.Length; i++)
            {
                oiseauxEnjeu[i]= oiseauxEnJeuDroite[i - oiseauxEnJeuGauche.Length];
            }
            //Tableau des pieces de fusee à tester
            Image[] fusee = new Image[] {imgAilerons, imgCoiffe, imgCorps};

            //Test des collisions possibles entre ennemis et morceaux de fusée
            bool collision = false;
            foreach (Image pieceFusee in fusee)
            {
                foreach (Image ennemi in oiseauxEnjeu)
                {
                    if ((Canvas.GetLeft(ennemi) + ennemi.ActualWidth > Canvas.GetLeft(pieceFusee)) &&
                (Canvas.GetLeft(ennemi) < pieceFusee.ActualWidth + Canvas.GetLeft(pieceFusee)) &&
                (Canvas.GetTop(ennemi) + ennemi.ActualHeight > Canvas.GetTop(pieceFusee)) &&
                (Canvas.GetTop(ennemi) < Canvas.GetTop(pieceFusee) + pieceFusee.ActualHeight))
                    {
                        collision = true;
                        break;
                    }
                }
            }
            return collision;
        }
        #endregion
        #region Déplacements verticaux des nuages et oiseaux
        private void DeplacementNuages()
        {
            for (int i = 0; i < nuages.Length; i++)
            {
                double nouvellePosition = Canvas.GetTop(nuages[i]) + vitesse / FREQUENCERAFRAICHISSEMENT;
                Canvas.SetTop(nuages[i], nouvellePosition);
                if (nouvellePosition >= canFond.Height)
                {
                    RepositionneNuage(nuages[i]);
                }
            }
        }
        private void DeplacementVertOiseaux()
        {
            for (int i = 0; i < oiseauxEnJeuGauche.Length; i++)
            {
                double NouvellePosition = Canvas.GetTop(oiseauxEnJeuGauche[i]) + VITESSEOISEAUX / FREQUENCERAFRAICHISSEMENT;
                Canvas.SetTop(oiseauxEnJeuGauche[i], NouvellePosition);
                if (NouvellePosition >= canFond.Height)
                {
                    RepositionneOiseau(oiseauxEnJeuGauche[i], true);
                }
                
            }
            for (int i = 0; i < oiseauxEnJeuDroite.Length; i++)
            {
                double NouvellePosition = Canvas.GetTop(oiseauxEnJeuDroite[i]) + VITESSEOISEAUX / FREQUENCERAFRAICHISSEMENT;
                Canvas.SetTop(oiseauxEnJeuDroite[i], NouvellePosition);
                if (NouvellePosition >= canFond.Height)
                {
                    RepositionneOiseau(oiseauxEnJeuDroite[i], false);
                }
                
            }
        }
        #endregion
        #region Mise à jour du score
        private void Score()
        {
            score += vitesse;
            if (score > meilleurScore)
            {
                meilleurScore = score;
            }
            labMeilleurScore.Content= ("Meilleur score : "+meilleurScore);
            labScore.Content = ("Score : " + score);
        }
        #endregion
        #region Création des oiseaux et leur repositionnement
        private void InitOiseaux()
        {
            oiseauxEnJeuGauche = new Image[NBOISEAUXENJEUGAUCHE];
            for (int i = 0; i < oiseauxEnJeuGauche.Length; i++)
            {
                oiseauxEnJeuGauche[i] = new Image();
                oiseauxEnJeuGauche[i].Width = LARGEUROISEAU;
                oiseauxEnJeuGauche[i].Height = HAUTEUROISEAU;
                oiseauxEnJeuGauche[i].Source = imagesOiseauGauche[0];
                canFond.Children.Add(oiseauxEnJeuGauche[i]);
                Canvas.SetLeft(oiseauxEnJeuGauche[i],-LARGEUROISEAU);
                Canvas.SetTop(oiseauxEnJeuGauche[i], rnd.Next(0, (int)canFond.Height - HAUTEUROISEAU));
                RenderOptions.SetBitmapScalingMode(oiseauxEnJeuGauche[i], BitmapScalingMode.NearestNeighbor);
            }
            oiseauxEnJeuDroite = new Image[NBOISEAUXENJEUDROITE];
            for (int i = 0; i < oiseauxEnJeuDroite.Length; i++)
            {
                oiseauxEnJeuDroite[i] = new Image();
                oiseauxEnJeuDroite[i].Width = LARGEUROISEAU;
                oiseauxEnJeuDroite[i].Height = HAUTEUROISEAU;
                oiseauxEnJeuDroite[i].Source = imagesOiseauDroite[0];
                canFond.Children.Add(oiseauxEnJeuDroite[i]);
                Canvas.SetLeft(oiseauxEnJeuDroite[i], (int)canFond.Width + LARGEUROISEAU);
                Canvas.SetTop(oiseauxEnJeuDroite[i], rnd.Next(0, (int)canFond.Height - HAUTEUROISEAU));
                RenderOptions.SetBitmapScalingMode(oiseauxEnJeuDroite[i], BitmapScalingMode.NearestNeighbor);
            }
        }

        private void RepositionneOiseau (Image oiseau, bool gauche)
        {
            if (gauche == true)
            {
                Canvas.SetLeft(oiseau,  -rnd.Next(LARGEUROISEAU, (int)canFond.Width/4));
                Canvas.SetTop(oiseau, rnd.Next(0, (int)(canFond.Height-oiseau.Height)));
            }
            else
            {
                Canvas.SetLeft(oiseau, rnd.Next((int)canFond.Width, (int)canFond.Width+ (int)canFond.Width/4));
                Canvas.SetTop(oiseau, rnd.Next(0, (int)(canFond.Height - oiseau.Height)));
            }
        }
        #endregion
        #region Repositionnement des nuages
        private void RepositionneNuage(Image Nuage)
        {
            int nouveauTop = -rnd.Next((int)Nuage.Height, (int)(Nuage.Height + canFond.Height));
            Canvas.SetTop(Nuage, nouveauTop);
            int nouveauLeft = rnd.Next((int)-Nuage.Width, (int)(canFond.Width + Nuage.Width));
            Canvas.SetLeft(Nuage, nouveauLeft);
        }
        private void InitNuages()
        {
            nuages = new Image[] { imgNuage1, imgNuage2 };
        }
        #endregion
        #region Gestion des messages affichés
        private void MessagePause()
        {
            imgMessage.Visibility = Visibility.Visible;
            labMessage.Content = "PAUSE";
            labMessage2.Content = "Appuyez sur entrée pour enlever la pause";
        }

        private void MessageCollision()
        {
            imgMessage.Visibility = Visibility.Visible;
            labMessage.Content = "VOUS AVEZ HEURTE UN OISEAU";
            labMessage2.Content = "Appuyez sur espace pour continuer";
        }

        private void MessageFinVol()
        {
            imgMessage.Visibility = Visibility.Visible;
            labMessage.Content = "VOUS AVEZ EVITE TOUTES LES COLLISIONS";
            labMessage2.Content = "Appuyez sur espace pour continuer";
        }

        private void MessageJauge()
        {
            imgMessage.Visibility = Visibility.Visible;
            labMessage.Content = "Appuyez sur la touche espace";
            labMessage2.Content = "lorsque la jauge est dans le vert.";
        }

        private void MessageContinuer()
        {
            imgMessage.Visibility = Visibility.Visible;
            labMessage.Content = "Vol terminé. Votre score :" + score;
            labMessage2.Content = "Appuyer sur espace pour continuer";
        }

        private void FinMessage()
        {
            imgMessage.Visibility = Visibility.Hidden;
            labMessage.Content = "";
            labMessage2.Content = "";
        }
        #endregion
        #region Initialisation des Bitmaps et des sons
        private void InitBitmaps()
        {
            InitBitmapsOiseaux();
            InitBitmapsEauTiree();
            InitBitmapsFusee();
            InitBitmapsEau();
            InitBitmapsAiguille();
        }

        private void InitSons()
        {

        }

        private void InitMusiques()
        {

        }

        private void InitBitmapsEau()
        {
            imagesEau = new BitmapImage[NBETATSEAU, NBIMAGESEAU];
            for (int i = 0; i < NBETATSEAU; i++)
            {
                for (int j = 0; j < NBIMAGESEAU; j++)
                {
                    imagesEau[i,j] = new BitmapImage(new Uri($"pack://application:,,,/img/Eau/Eau{i}{j}.png"));
                }
            }
        }

        private void InitBitmapsFusee()
        {
            imagesCoiffe = new BitmapImage[NBIMGCOIFFE];
            imagesCorps = new BitmapImage[NBIMGCORPS];
            imagesAilerons = new BitmapImage[NBIMGAILERONS];
            for (int i = 0;i < NBIMGCOIFFE; i++)
            {
                imagesCoiffe[i] = new BitmapImage(new Uri($"pack://application:,,,/img/Fusee/Coiffe{i}.png"));
            }
            for (int i = 0;i < NBIMGCORPS; i++)
            {
                imagesCorps[i] = new BitmapImage(new Uri($"pack://application:,,,/img/Fusee/Corps{i}.png"));
            }
            for (int i = 0;i < NBIMGAILERONS; i++)
            {
                imagesAilerons[i] = new BitmapImage(new Uri($"pack://application:,,,/img/Fusee/Ailerons{i}.png"));
            }
        }

        private void InitBitmapsEauTiree()
        {
            imagesEauTiree = new BitmapImage[NBIMAGESEAUTIREE];
            for(int i = 0;i < NBIMAGESEAUTIREE; i++)
            {
                imagesEauTiree[i] = new BitmapImage(new Uri($"pack://application:,,,/img/Eau_Tiree/EauTiree{i}.png"));
            }
        }

        private void InitBitmapsOiseaux()
        {
            imagesOiseauDroite = new BitmapImage[NBIMAGESOISEAU];
            imagesOiseauGauche = new BitmapImage[NBIMAGESOISEAU];
            for (int i = 0;i < NBIMAGESOISEAU; i++)
            {
                imagesOiseauDroite[i] = new BitmapImage(new Uri($"pack://application:,,,/img/OiseauDroite/OiseauDroite{i}.png"));
                imagesOiseauGauche[i] = new BitmapImage(new Uri($"pack://application:,,,/img/OiseauGauche/OiseauGauche{i}.png"));
            }
        }

        private void InitBitmapsAiguille()
        {
            imagesAiguille = new BitmapImage[NBIMAGESAIGUILLE];
            for (int i = 0; i < NBIMAGESAIGUILLE; i++)
            {
                imagesAiguille[i] = new BitmapImage(new Uri($"pack://application:,,,/img/Aiguille/Aiguille{i}.png"));
            }
        }
        #endregion
    }
}