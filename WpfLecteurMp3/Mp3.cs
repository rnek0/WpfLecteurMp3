using System;

namespace WpfLecteurMp3
{
    public class Mp3
    {
        public string FilenameAdress { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public string Song { get; set; }
        public string Year { get; set; }
        public TimeSpan Duree { get; set; }

        private string LimitString(string str)
        {
            int limit = 40;

            string SongToStr = "";
            if (String.IsNullOrEmpty(str))
            {
                SongToStr = "(Pas de Titre)"; // Bug Fixed.
            }
            else { 
                SongToStr = (str.Length > limit) ? str.Substring(0, limit) + "..." : str;
            }
            return SongToStr;
        }

        public override string ToString()
        {
            if (Song != "")
            {
                var SongToStr = LimitString(Song);//(Song.Length > 40) ? Song.Substring(0, 40) + "..." : Song;
                return SongToStr;// Song;    
            }
            return LimitString(FilenameAdress);
        }

        /// <summary>
        /// Egalité definie par le path + nom de fichier.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            bool result = false;
            Mp3 autre = obj as Mp3;
            if (autre.FilenameAdress == this.FilenameAdress)
            {
                result = true;
            }
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}