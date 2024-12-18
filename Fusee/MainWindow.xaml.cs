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
        private static readonly int VITESSEENPLUSCLICVERT = 200;
        private static readonly int VITESSEENPLUSCLICJAUNE = 150;
        private static readonly int VITESSEENPLUSCLICROUGE = 100;
        private static readonly int ACCELERATIONFUSEE = -1;
        private static readonly int DEPLACEMENTOISEAUX = 8;
        private static readonly int NBCHANCESMINIMUM = 5;
        private static readonly int NBCHANCESMAXIMUM = 10;
        private static readonly int VITESSEFINVOL = 0;
        //Nombre de chaque images pour en rajouter facilement et faciliter les chargements
        private static readonly int NBIMGCOIFFE = 1; //Utile lors du rajout d'image pour personnaliser la fusée par exemple, ou rajouter des frames d'animation
        private static readonly int NBIMGAILERONS = 1;
        private static readonly int NBIMGCORPS = 1;
        private static readonly int NBIMGPOMPE = 1;
        private static readonly int NBIMAGESOISEAU = 2;
        private static readonly int NBIMAGESAIGUILLE = 8;
        private static readonly int NBIMAGESEAU = 3;
        private static readonly int NBETATSEAU = 2;
        private static readonly int NBIMAGESEAUTIREE = 1;
        //Nombre d'oiseaux
        private static readonly int NBOISEAUXENJEUGAUCHE = 1; //A modifier pour plus ou moins d'oiseaux en jeu, d'un côté ou l'autre
        private static readonly int NBOISEAUXENJEUDROITE = 1;
        //Canvas.Top de départs
        private static readonly int POSDEPARTSOCLE = 566;  //Positions .Top de départs pour repositionner éléments de décor après vol
        private static readonly int POSDEPARTPOMPE = 502;
        private static readonly int POSDEPARTAIGUILLE = 576;
        private static readonly int POSDEPARTJAUGE = 576;
        //Canvas.Left de départ
        private static readonly int POSDEPARTCORPS = 539;  //Positions .Left de départ pour repositionner la fusée en fin de vol
        private static readonly int POSDEPARTAILERONS = 512;
        private static readonly int POSDEPARTCOIFFE = 539;
        private static readonly int POSDEPARTEAU = 539;
        //Taille des oiseaux
        private static readonly int LARGEUROISEAU = 64; //Pour créer les oiseaux
        private static readonly int HAUTEUROISEAU = 64;
        //
        private static readonly int FREQUENCERAFRAICHISSEMENT = 60; //Pour les animations
        private static readonly int TICKSENTREIMGAIGUILLE = 5;
        private static readonly int TICKSDUREEMESSAGEENCORE = 15;
        #endregion
        #region Tableaux, listes, sons
        //Tableaux de Bitmaps
        private static BitmapImage[] imagesCoiffe; //Tableaux créés pour faiciliter le changement de source d'une image
        private static BitmapImage[] imagesCorps;  //Bien qu'ils ne soient pas utiles actuellement, ils devaient permettre de gérer les différentes
        private static BitmapImage[] imagesAilerons;//images facilement pour la personnalisation de la fusée.
        private static BitmapImage[] imagesPompe;
        private static BitmapImage[] imagesOiseauGauche; //Ces tableaux servent pour les animations des éléments, bien que certaines n'aient pas pu
        private static BitmapImage[] imagesOiseauDroite; //être développées
        private static BitmapImage[] imagesNuage;
        private static BitmapImage[] imagesNuageDeux;
        private static BitmapImage[] imagesAiguille;
        private static BitmapImage[] imagesEauTiree;
        private static BitmapImage[,] imagesEau;         //Il s'agit d'un tableau à deux dimensions car le niveau de l'eau devait diminuer en cours de vol
                                                         // Et chaque niveau d'eau devait être animé
        //Musiques
        private static MediaPlayer musiqueVol;
        private static MediaPlayer musiqueAmbiance;
        private static SoundPlayer sonJauge;
        //Tableaux d'images
        private static Image[] oiseauxEnJeuGauche;      //Tableaux d'images pour les créer et les déplacer facilement
        private static Image[] oiseauxEnJeuDroite;
        private static Image[] nuages;
        //Listes pour les objets personnalisables
        //Les listes n'ont pas été utilisées
        //Elles devaient servir à stocker les objets déjà achetés et ceux disponibles à l'achat, pour la personnalisation de la fusée
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
        private static int argentDispo = 0; //Pour achat des pièces de fusée
        private static int volumeMusiques = 100; //Pour paramètres de son
        private static int volumeSons = 100;
        private static int coiffeSelectionnee = 1; //Pour changer facilement de source lors de la personnalisation de la fusée
        private static int corpsSelectionne = 1;
        private static int aileronsSelectionnes = 1;
        private static int pompeSelectionnee = 1;
        private static int vitesse;
        private static int nbChances;
        private static int numChance;
        private static int numAiguille;
        private static int etatEau = 0; //Pour faire diminuer le niveau de l'eau
        private static int nbTicksMessageEncore=16;
        

        private static bool tremblements = true; //Pour activer/desactiver tremblements de la fusée (vent) (n'a pas été dev non plus)
        private static bool gauche, droite;
        private static bool minuterieActive;
        private static bool vol; //Les différents moments du jeu
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
            rnd = new Random(); //Initialise la MainWindow et lance l'écran de lancement du jeu
            InitializeComponent();
            InitMusiqueMenu();
            musiqueAmbiance.Play();
            Pause pause = new Pause();
            if (pause.ShowDialog() == true) //Le jeu ne se lance que si l'on clique sur jouer 
            {
                InitTimer();
                InitBitmaps();
                InitMusiques();
                InitSonJauge();
                InitOiseaux();
                InitNuages();
                jauge = true;
                vol = false;
            }
            else
            {
                this.Close(); //Si on ne clique pas sur jouer, le jeu se ferme
            }
        }
        #endregion
        #region touches appuyées

        private void fenetrePrincipale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) //Touches de déplacement
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
                if (minuterieActive == true) //Permet de mettre pause à tout moment, utile pour DEBUG
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
            if (e.Key == Key.Space) //Permet de continuer dans les messages, et de cliquer dans la jauge
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
        private void InitTimer() //Initialisation de la minuterie
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(16);//En 60 FPS
            minuterie.Tick += Jeu;
            minuterie.Start();
            minuterieActive = true;
        }
        #endregion
        #region Boucle du jeu
        private void Jeu(object? sender, EventArgs e) //Moteur de jeu
        {
#if DEBUG
            Console.WriteLine(" jauge : " + jauge);
            Console.WriteLine(" vol : " + vol);
#endif
            if (jauge == true && vol == false) //Avant le vol, lance la jauge
            {
                Jauge();
            }
            else if (vol == true && jauge == false) //Pendant le vol, gère le déplacement de la fusée, des oiseaux, des nuages, du score
            {
                DeplacementFusee(imgCoiffe);
                DeplacementFusee(imgCorps);
                DeplacementFusee(imgAilerons);
                DeplacementFusee(imgEau);
                DeplacementOiseau();
                DeplacementVertOiseaux(); 
                DeplacementNuages(); 
                Score();
                vitesse += ACCELERATIONFUSEE; //Actualise la vitesse, qui diminue car c'est un fusée en plastique en chute libre (je n'ai pas pris en compte les frottements, et la gravité n'est pas réaliste)
#if DEBUG
                Console.WriteLine("Left: " + Canvas.GetLeft(imgNuage1) + " Top: " + Canvas.GetTop(imgNuage1));
#endif
                if (Collision()) //Teste s'il y a une collsion
                {
#if DEBUG
                    Console.WriteLine("Collision");
#endif
                    FinVol();
                    MessageCollision();
                }
                if (vitesse <= VITESSEFINVOL) //Teste si la vitesse a diminué assez pour arrêter le vol 
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
            else //Après le vol
            {
                if (!continuer1)
                {
                    //On ne fait rien et le message précédent reste affiché tant que le joueur ne clique pas sur espace pour continuer
                }
                else
                {
                    if (!continuer2)
                    {
                        MessageContinuer(); //Le message suivant s'affiche quand le joueur a cliqué une fois sur espace
                    }
                    else
                    { //Le joueur a passé les deux messages, le jeu recommence
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
                jaugeEnCours = true; //Lance la jauge lorsque l'on rentre dans la méthode pour la première fois depuis le dernier vol
                numAiguille = 0;
                numChance = 1;
                nbChances = rnd.Next(NBCHANCESMINIMUM, NBCHANCESMAXIMUM); //Le nombre de clics est aléatoire, on ne sait pas quand la fusée aura trop de pression et lachera
                vitesse = 0; //Réinitialise la vitesse de la fusée
                MessageJauge(); //Affiche les instructions de la jauge
            }
            if (jaugeEnCours) //Pendant la jauge
            {
                numAiguille++;
                imgAiguille.Source = imagesAiguille[numAiguille/ TICKSENTREIMGAIGUILLE % 8]; //Anime l'aiguille
                if (cliqueEspace)
                {
                    sonJauge.Play();
                    cliqueEspace = false;
                    if((numAiguille / TICKSENTREIMGAIGUILLE % 8)==4 || (numAiguille / TICKSENTREIMGAIGUILLE % 8) == 0) //Détecte l'emplacement de l'aiguille
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
                        jaugeEnCours = false; //Arrête la jauge et lance le vol lorsque toutes les chances de clic sont passées
                        jauge = false;
                        imgAiguille.Source = imagesAiguille[0];
                        FinMessage();
                        DebutVol();
                    }
                    else
                    {
                        nbTicksMessageEncore = 0; //Remet le temps écoulé d'affichage du message "encore" à 0, pour qu'il se relance lorsque sa méthode sera appelée
                    }
                    
                }
                MessageContinuerAppuyer();//Lance la méthode du message "encore"
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
            { //Vérifie que la fusée ne sort pas du canvas
                Canvas.SetLeft(img, nouvellePosition);
            }

        }
        #endregion
        #region Déplacement horizontal de l'oiseau
        private void DeplacementOiseau()
        {
            double nouvellePosition;
            //Anime et déplace horizontalement les oiseaux venant de la gauche
            for (int i = 0;i < oiseauxEnJeuGauche.Length; i++)
            {
                nouvellePosition = Canvas.GetLeft(oiseauxEnJeuGauche[i]);
                nouvellePosition += DEPLACEMENTOISEAUX;
                if (nouvellePosition >=canFond.Width)  //Vérfie que l'oiseau ne sort pas du Canvas
                {
                    RepositionneOiseau(oiseauxEnJeuGauche[i], true);
                }
                Canvas.SetLeft(oiseauxEnJeuGauche[i], nouvellePosition);
                oiseauxEnJeuGauche[i].Source = imagesOiseauGauche[(Array.IndexOf(imagesOiseauGauche, oiseauxEnJeuGauche[i].Source)+1)%NBIMAGESOISEAU];
            }
            //Anime et déplace horizontalement les oiseaux venant de la gauche
            for (int i = 0; i < oiseauxEnJeuDroite.Length; i++)
            {
                nouvellePosition = Canvas.GetLeft(oiseauxEnJeuDroite[i]);
                nouvellePosition -= DEPLACEMENTOISEAUX;
                if (nouvellePosition<=-LARGEUROISEAU) //Vérfie que l'oiseau ne sort pas du Canvas
                {
                    RepositionneOiseau(oiseauxEnJeuDroite[i], false);
                }
                Canvas.SetLeft(oiseauxEnJeuDroite[i], nouvellePosition);
                oiseauxEnJeuDroite[i].Source = imagesOiseauDroite[(Array.IndexOf(imagesOiseauDroite, oiseauxEnJeuDroite[i].Source) + 1) % NBIMAGESOISEAU];
            }
        }
        #endregion
        #region Changements de fin du vol
        private void FinVol() //Est déclenché à la fin du vol
        {
#if DEBUG
            Console.WriteLine("Ca marche");
#endif
            Canvas.SetTop(imgSol, canFond.Height-imgSol.Height); //Replace les éléments du décor
#if DEBUG
            Console.WriteLine("Ca marche");
#endif
            Canvas.SetTop(imgSocle, POSDEPARTSOCLE);
            Canvas.SetTop(imgPompe, POSDEPARTPOMPE);
            Canvas.SetTop(imgAiguille, POSDEPARTAIGUILLE);
            Canvas.SetTop(imgJauge, POSDEPARTJAUGE);
            for (int i = 0; i < oiseauxEnJeuDroite.Length; i++) //Repositionne les oiseaux
            {
                RepositionneOiseau(oiseauxEnJeuDroite[i], false);
            }
            for (int i = 0;i<oiseauxEnJeuGauche.Length; i++)
            {
                RepositionneOiseau(oiseauxEnJeuGauche[i], true);
            }
            Canvas.SetLeft(imgAilerons, POSDEPARTAILERONS); //Rositionne la fusée et l'eau dans la fusée
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
            etatEau = 0; //met eau dans etat de base, pas utile actuellement mais plus tard pour diminution niveau de l'eau
            vol = false; //fin du mode vol
            musiqueVol.Stop();
            musiqueAmbiance.Play();

        }
        #endregion
        #region Debut du vol
        private void DebutVol()
        {
            Canvas.SetTop(imgSol, canFond.Height + imgSol.Height); //Déplace les éléments de décor qui doivent partir pour le vol
            Canvas.SetTop(imgSocle, canFond.Height+imgSocle.Height);
            Canvas.SetTop(imgPompe, canFond.Height + imgPompe.Height);
            Canvas.SetTop(imgAiguille, canFond.Height+ imgAiguille.Height);
            Canvas.SetTop(imgJauge, canFond.Height + imgJauge.Height);
            vol = true; //Lance le mode vol
            jauge = false;
            musiqueAmbiance.Stop(); //Lance la musique du vol
            musiqueVol.Play();

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
        private void RepositionneNuage(Image Nuage) //Repositionnes les nuages à un Top et Left alétoire mais au-dessus du Canvas et de manière à ce que le nuage dépasse au moins peu dans l'écran
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
        private void MessageContinuerAppuyer()
        {
            if (nbTicksMessageEncore <= TICKSDUREEMESSAGEENCORE)
            {
                labEncore.Visibility = Visibility.Visible;
                nbTicksMessageEncore++;
#if DEBUG
                Console.WriteLine("nbTicksMessageEncore : "+nbTicksMessageEncore);
#endif
            }
            else
            {
                labEncore.Visibility= Visibility.Hidden;
            }
            if (jauge == false)
            {
                labEncore.Visibility = Visibility.Hidden;
            }

        }
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
            labMessage.Content = "VOUS AVEZ EVITE LES OISEAUX";
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
        {//Lance les méthodes d'init des Bitmaps
            InitBitmapsOiseaux();
            InitBitmapsEauTiree();
            InitBitmapsFusee();
            InitBitmapsEau();
            InitBitmapsAiguille();
        }

        private void InitMusiqueMenu()
        {
            musiqueAmbiance = new MediaPlayer();
            musiqueAmbiance.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "musique/Ambiance.mp3"));
            musiqueAmbiance.MediaEnded += RelanceMusiqueMenu;
            musiqueAmbiance.Volume = 1.0;
        }
        private void RelanceMusiqueMenu(object? sender, EventArgs e)
        {
            musiqueAmbiance.Position = TimeSpan.Zero;
            musiqueAmbiance.Play();
        }

        private void InitMusiques()
        {
            musiqueVol = new MediaPlayer();
            musiqueVol.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "musique/Unity_TheFatRat.mp3"));
            musiqueVol.MediaEnded += RelanceMusique;
            musiqueVol.Volume = 1.0;
        }

        private void RelanceMusique(object? sender, EventArgs e)
        {
            musiqueVol.Position = TimeSpan.Zero;
            musiqueVol.Play();
        }

        private void InitSonJauge()
        {
            sonJauge = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/sons/SonJauge.wav")).Stream);
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