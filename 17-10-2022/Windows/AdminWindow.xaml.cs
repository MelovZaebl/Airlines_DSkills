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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow(Users user)
        {
            InitializeComponent();
        }

        private void Update()
        {
            if (CBOffice.SelectedIndex == 0)
            {
                DG.ItemsSource = MainWindow.DB.Users.ToList();
            }
            else
            {
                DG.ItemsSource = MainWindow.DB.Users.Where(x => x.Offices.Title == CBOffice.SelectedValue.ToString()).ToList();
            }
            DG.SelectedItem = null;
        }

        private void AddUser(object sender, RoutedEventArgs e)
        {
            Windows.UserEdit win = new UserEdit();
            win.ShowDialog();
            Update();
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

        private void OfficeChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void ChangeRole(object sender, RoutedEventArgs e)
        {
            Windows.RoleEditWindow win = new RoleEditWindow(DG.SelectedItem as Users);
            win.ShowDialog();
            Update();
        }

        private void ActiveChanger(object sender, RoutedEventArgs e)
        {
            if (DG.SelectedItem != null)
            {
                Users selUser = DG.SelectedItem as Users;
                selUser.Active = !selUser.Active;
                MainWindow.DB.SaveChanges();
                Update();
            }
            else MessageBox.Show("Выберите пользователя", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Load(object sender, RoutedEventArgs e)
        {

            CBOffice.Items.Add("All offices");
            foreach (Offices of in MainWindow.DB.Offices)
            {
                CBOffice.Items.Add($"{of.Title}");
            }
            CBOffice.SelectedIndex = 0;

            Update();

        }
    }
}
