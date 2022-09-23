using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;

namespace WpfLecteurMp3
{
    public delegate void LastPlayed(object sender, LastMp3EventArgs arg);

    /// <summary>
    /// Lecteur Mp3.
    /// </summary>
    public sealed class Lecteur
    {
        public event LastPlayed FilePlayed;

        // Un lecteur contient un mediaplayer et une playlist.
        MediaPlayer mp; 
        ObservableCollection<Mp3> playList;

        string cheminDuDossier; // Chemin du dossier de lecture.
        string tmpmp3; // nom du fichier mp3.
        bool enPause = false;

        int numberOfMp3; // nombre de mp3 dans la playlist.
        //int mp3EnCours; // numero du mp3 en cours dans la playlist.
        bool lectureEnBoucle; // la lecture en boucle est demandée ?.

        public string NomMp3 { get { return tmpmp3; }}
        public string CheminDuFichier { get { return cheminDuDossier; } }
        public bool Enboucle { get { return lectureEnBoucle; } set { lectureEnBoucle = value; } }
        public int NumberOfMp3 { get { return numberOfMp3; }}
        public bool EnPause { get { return enPause; } set { enPause = value; } }

        /// <summary>
        /// Nom du fichier mp3.
        /// </summary>
        public string FichierALire {
            get { return tmpmp3;}
            set {
                int pos = value.LastIndexOf('\\');
                cheminDuDossier = value.Substring(0, pos);
                tmpmp3 = value.Substring(pos+1);
                playList = RemplirPlayList(cheminDuDossier, "mp3");
            } 
        }

        /// <summary>
        /// Mediaplayer du lecteur.
        /// </summary>
        public MediaPlayer Mp { 
            get{ 
                return mp;
            }
        }

        /// <summary>
        /// Ouverture du fichier mp3 dans le media player.
        /// </summary>
        /// <param name="fichierMp3">Fichier a ouvrir. string</param>
        public void Ouvrir(string fichierMp3)
        {
            var chanson = new Uri(fichierMp3);
            mp.Open(chanson);
            FilePlayed?.Invoke(this, new LastMp3EventArgs() { LastMp3 = chanson.AbsoluteUri });
        }


        public void Jouer()
        {
            if (mp != null && mp.Source.IsFile)
            {
                mp.Play();
            }
        }

        public void MettreEnPause()
        {
            if (mp != null && mp.Source.IsFile)
            {
                mp.Pause();    
            }
        }

        public ObservableCollection<Mp3> PlayList {
            get { return playList; }
        }

        private ObservableCollection<Mp3> RemplirPlayList(string dossier, string extension)
        {
            playList = new ObservableCollection<Mp3>();

            foreach (var item in Directory.EnumerateFiles(dossier))
            {
                //if (item.Contains(extension))
                if (item.EndsWith(extension,StringComparison.OrdinalIgnoreCase))// FIX
                {
                    Mp3 tmp_mp3 = RemplirTags(item);
                    playList.Add(tmp_mp3);
                }
            }
            numberOfMp3 = playList.Count;
            return playList;
        }

        // Genere le fichier mp3 avec les tags s'ils existent
        private Mp3 RemplirTags(string fichier)
        {
            try
            {
                TagLib.File file = TagLib.File.Create(fichier);
                Mp3 ficMp3;
                
                if (file.Tag.IsEmpty)
                {
                    ficMp3 = new Mp3
                    {
                        FilenameAdress = fichier,
                        Album = "",
                        Artist = "",
                        Song = "",
                        Year = "",
                        Duree = TimeSpan.Zero//.MinValue//file.Properties.Duration
                    };
                }
                else
                {
                    String title = file.Tag.Title;
                    String album = file.Tag.Album;
                    TimeSpan length = file.Properties.Duration;
                    ficMp3 = new Mp3
                    {
                        FilenameAdress = fichier,
                        Album = album,
                        Artist = file.Tag.FirstPerformer,
                        Song = file.Tag.Title,
                        Year = file.Tag.Year.ToString(),
                        Duree = length
                    };
                }
                return ficMp3;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                throw;
            }
        }

        #region --- Ctor ---
        private Lecteur()
        {
            mp = new MediaPlayer();
            tmpmp3 = "";
            cheminDuDossier = "";
            enPause = false;
            Enboucle = false;
            numberOfMp3 = 0;
        }
        #endregion

        #region --- Implementation Singleton ---
        private static Lecteur instanceLecteur = null;

        public static Lecteur InstanceLecteur
        {
            get
            {
                if (instanceLecteur == null)
                {
                    instanceLecteur = new Lecteur();
                }
                return instanceLecteur;
            }
        }
        #endregion

    }

    public class LastMp3EventArgs : EventArgs
    {
        // Dernier Mp3 joué.
        public string LastMp3 { get; set; }
    }
}
