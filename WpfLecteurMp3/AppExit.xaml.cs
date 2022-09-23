using System.Windows;

namespace WpfLecteurMp3
{
    /// <summary>
    /// Confirmation de la sortie de l'application.
    /// </summary>
    public partial class AppExit : Window
    {
        public AppExit(Window windowOwner)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            BtnOk.Focus();
            BtnOk.Click += (sender, e) =>
            {
                //TODO : remove after finish dev.
                //LogWriter.LogToFile("Fermeture de l'application.");
                Application.Current.Shutdown();
            };

            BtnCancel.Click += (sender,e) => this.Close();
        }
    }
}