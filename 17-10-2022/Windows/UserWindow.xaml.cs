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
using System.Windows.Shapes;

namespace _17_10_2022.Windows
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow.track.StopDate = DateTime.Now;
            MainWindow.track.ErrorType = 3;
            MainWindow.DB.Tracking.Add(MainWindow.track);
            MainWindow.DB.SaveChanges();

            MainWindow win = new MainWindow();
            win.Show();
            this.Close();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            DG.ItemsSource = MainWindow.DB.Tracking.Where(x => x.UserID == MainWindow.loginedUser.ID).ToList();

            TimeSpan totalSpend = TimeSpan.FromSeconds(0);
            foreach (Tracking t in MainWindow.DB.Tracking.Where(x => x.UserID == MainWindow.loginedUser.ID))
            {
                totalSpend += t.TimeSpend;
            }

            StatusText.Text = $"Time spend on system: {totalSpend}      Number of crashes: {MainWindow.DB.Tracking.Where(x => x.UserID == MainWindow.loginedUser.ID && (x.ErrorType == 1) || x.ErrorType == 2).Count()}";

            WelcomeText.Text = $"Hi {MainWindow.loginedUser.Firstname}, Welcome to AMONIC Airlines";
        }
    }
}
