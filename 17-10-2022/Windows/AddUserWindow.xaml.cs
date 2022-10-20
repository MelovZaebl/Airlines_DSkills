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
    /// Логика взаимодействия для UserEdit.xaml
    /// </summary>
    public partial class UserEdit : Window
    {
        public UserEdit()
        {
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (TBEmail.Text != null && TBFName != null && TBLName != null && CBOffice.SelectedItem != null && TBBirthdate != null)
            {
                Users user = new Users();
                user.Email = TBEmail.Text;
                user.Firstname = TBFName.Text;
                user.Lastname = TBLName.Text;
                user.OfficeID = (CBOffice.SelectedItem as Offices).ID;
                user.Birthdate = DateTime.ParseExact($"{TBBirthdate.Text}", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                user.Password = MainWindow.CreateMD5(TBPass.Text);

                user.RoleID = 2;
                user.Active = true;

                MainWindow.DB.Users.Add(user);
                MainWindow.DB.SaveChanges();
                this.Close();
            }
            else MessageBox.Show("Проверьте правильность введенных данных", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FNameInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsLetter(e.Text, 0)) e.Handled = true;
        }

        private void LNameInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsLetter(e.Text, 0)) e.Handled = true;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            CBOffice.ItemsSource = MainWindow.DB.Offices.ToList();
        }
    }
}
