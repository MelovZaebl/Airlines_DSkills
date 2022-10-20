using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для ExecptionWindow.xaml
    /// </summary>
    public partial class ExecptionWindow : Window
    {
        public ExecptionWindow(Exception e, int style)
        {
            InitializeComponent();
            if (style == 0)
            {
                Reason.Text = e.Message;
                MainWindow.track.StopDate = DateTime.Now;
                MainWindow.track.Reason = Reason.Text;
                MainWindow.track.UserID = MainWindow.loginedUser.ID;

                TB.Text = e.Message + " " + MainWindow.track.StopDate;
            }
            else
            {
                TB.Text = "No logout detected" + e.Message.Split(' ')[0] + e.Message.Split(' ')[1];
                Reason.Text = "No logout decected" + e.Message.Split(' ')[0] + e.Message.Split(' ')[1];

                MainWindow.track.StartDate = DateTime.ParseExact($"{e.Message.Split(' ')[0]} {e.Message.Split(' ')[1]}", "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                MainWindow.track.StopDate = DateTime.Now;
                MainWindow.track.Reason = Reason.Text;
                MainWindow.track.UserID = int.Parse(e.Message.Split(' ')[2]);
                RBSoft.IsChecked = false;
                RBSys.IsChecked = true;
            }
        }

        private void btn_Confirm(object sender, RoutedEventArgs e)
        {
            if (RBSoft.IsChecked == true)
            {
                MainWindow.track.ErrorType = 1;
            }
            else
            {
                MainWindow.track.ErrorType = 2;
            }
            MainWindow.DB.Tracking.Add(MainWindow.track);
            MainWindow.DB.SaveChanges();
            this.Close();
        }
    }
}
