using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfLecteurMp3
{
    public partial class MainWindow : Window
    {
        string tmpmp3;
        MediaPlayer mp = null;

        int numberOfMp3;
        int mp3EnCours;
        bool lectureEnBoucle;

        DispatcherTimer timer;
        bool isDragging;

        Lecteur monLecteur = null;

        public MainWindow()
        {
            InitializeComponent();

            //TODO : remove after finish dev.
            //LogWriter.LogToFile("Demarrage de l'application.");

            tmpmp3 = "";
            //cheminDuDossier = "";
            monLecteur = Lecteur.InstanceLecteur;
            mp = monLecteur.Mp;
            mp.MediaOpened += Mp_MediaOpened;
            mp.MediaEnded += Mp_MediaEnded;

            lectureEnBoucle = false;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            isDragging = false;

            monLecteur.FilePlayed += (se, ev) => { tmpmp3 = ev.LastMp3; };

            //this.Closing += (s, e) =>
            //{
            //    if (MessageBoxResult.No == MessageBox.Show($"Fermer le player après {tmpmp3} ?",
            //        "Fermeture...",
            //        MessageBoxButton.YesNo))
            //    {
            //        e.Cancel = true;
            //    }
            //};
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging)
            {
                sliderDuree.Value = mp.Position.TotalSeconds;
            }
        }

        // Lorsque la chanson est ouverte !
        void Mp_MediaOpened(object sender, EventArgs e)
        {
            if (mp.NaturalDuration.HasTimeSpan)
            {
                sliderDuree.Maximum = mp.NaturalDuration.TimeSpan.TotalSeconds;
                sliderDuree.SmallChange = 1;
            }
            timer.Start();
        }

        // Lorsque la chanson est finie !
        void Mp_MediaEnded(object sender, EventArgs e)
        {
            timer.Stop();
            sliderDuree.Value = 0;
            int enBoucle = 0;
            if (lectureEnBoucle == true)
            {
                enBoucle = 1;
            }
            btnPlayPause.Content = "PLAY";
            monLecteur.EnPause = false;
            if (mp3EnCours < (numberOfMp3 - enBoucle) && numberOfMp3 > 2)
            {
                mp3EnCours = mp3EnCours + 1;
            }
            else
            {
                mp3EnCours = 0;
            }
            SelectionnerDansLaListe(mp3EnCours);
        }
    
        // Bouton Choisir MP3
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();  // Recherche de fichiers
            Nullable<bool> ret = ofd.ShowDialog();
            if (ret == true)
            {
                tmpmp3 = ofd.FileName;
                monLecteur.FichierALire = ofd.FileName;
                numberOfMp3 = monLecteur.NumberOfMp3;
                ListeChansons.ItemsSource = monLecteur.PlayList;
                SelectionnerDansLaListe(tmpmp3);
            }
        }

        private void SelectionnerDansLaListe(string fichier)
        {
            int monIndex = 0;
            for (int i = 0; i < ListeChansons.Items.Count; i++)
            {
                if((ListeChansons.Items[i] as Mp3).FilenameAdress == fichier)
                {
                    monIndex = i;
                }
            }
            mp3EnCours = monIndex;
            if (ListeChansons.Items.Count > 0)
                ListeChansons.ScrollIntoView(ListeChansons.Items[monIndex]);
            SelectionnerDansLaListe(monIndex);
        }

        private void SelectionnerDansLaListe(int nroDansLaListe)
        {
            ListeChansons.SelectedIndex = nroDansLaListe;
            //if (nroDansLaListe == 0)
            //{
            //    ListeChansons.ScrollIntoView(ListeChansons.Items[0]); // Scroll to first.
            //}
            //ListeChansons.s
        }

        // Bouton Stop.
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            monLecteur.Mp.Stop();
            btnPlayPause.Content = "PLAY";
            monLecteur.EnPause = false;
        }

        // Bouton play.
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (monLecteur.EnPause == true)
            {
                MettreEnPauseChanson();
            }
            else
            {
                if (ListeChansons.SelectedIndex != -1)
                {
                    JouerChanson();
                }
            }
        }

        private void MettreEnPauseChanson()
        {
            monLecteur.Mp.Pause();
            monLecteur.EnPause = false;
            btnPlayPause.Content = "PLAY";
        }

        private void JouerChanson()
        {
            monLecteur.Mp.Play();
            btnPlayPause.Content = "PAUSE";
            monLecteur.EnPause = true;
        }

        // Choix dans la liste.
        private void ListeChansons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem != null)
            {
                Mp3 lbItem = ((sender as ListBox).SelectedItem as Mp3);
                var artiste = lbItem.Artist;

                //trouver l'index dans la liste pour mettre a jour mp3EnCours !!!
                mp3EnCours = (sender as ListBox).SelectedIndex;

                monLecteur.Mp.Open(new Uri(lbItem.FilenameAdress));
                
                JouerChanson();
                base.DataContext = lbItem;
            }
        }

        // Verifie si la lecture en boucle est demandée
        private void CheckLireEnBoucle_Checked(object sender, RoutedEventArgs e)
        {
            if (checkLireEnBoucle.IsChecked == true)
            {
                lectureEnBoucle = true;
            }
            else
            {
                lectureEnBoucle = false;
            }
        }

        // Bouton piste suivante
        private void BtnPlayPrecedent_Click(object sender, RoutedEventArgs e)
        {
            if (mp3EnCours > 0)
            {
                mp3EnCours = mp3EnCours - 1;
            }
            else
            {
                mp3EnCours = numberOfMp3 - 1;
            }

            SelectionnerDansLaListe(mp3EnCours);
        }
        
        // Bouton piste precedente
        private void BtnPlaySuivant_Click(object sender, RoutedEventArgs e)
        {
            if (mp3EnCours < numberOfMp3 - 1)
            {
                mp3EnCours = mp3EnCours + 1;
            }
            else
            {
                mp3EnCours = 0;
            }
            SelectionnerDansLaListe(mp3EnCours);
        }

        #region --- Actions sur la fenêtre ---
        // Drag de la fenêtre
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        // Ferme l'application.
        private void BtnFermer_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            var exitApp = new AppExit(this);
            exitApp.ShowDialog();
        }

        // Minimize.
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        #endregion
    }
}
