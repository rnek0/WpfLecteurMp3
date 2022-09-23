using System;
using System.Windows;
using System.IO;

namespace WpfLecteurMp3
{
    public static class LogWriter
    {
        private static readonly string LogFile = AppDomain.CurrentDomain.BaseDirectory + "\\DevLog.txt";

        public static void LogToFile(string msg)
        {
            //var t = DateTime.Now.ToLocalTime().ToShortTimeString();
            var d = DateTime.Now.ToLocalTime().ToLongDateString();
            try
            {
                File.AppendAllText(LogFile, $"{d} >>> {msg} " + Environment.NewLine + "..." + Environment.NewLine);
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Je peux pas écrire dans le log !\n{exc.Message}", 
                    App.Current.MainWindow.Title, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
                throw exc;
            }
        }
    }
}