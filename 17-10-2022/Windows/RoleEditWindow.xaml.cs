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
    /// Логика взаимодействия для RoleEditWindow.xaml
    /// </summary>
    public partial class RoleEditWindow : Window
    {

        public Users EditedUser;
        public RoleEditWindow(Users user)
        {
            InitializeComponent();
            EditedUser = user;
            DataContext = EditedUser;
            if (EditedUser.RoleID == 1)
            {
                RBAdmin.IsChecked = true;
            }
            else RBUser.IsChecked = true;
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            if (RBAdmin.IsChecked == true)
            {
                EditedUser.RoleID = 1;
            }
            else EditedUser.RoleID = 2;

            MainWindow.DB.SaveChanges();
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
