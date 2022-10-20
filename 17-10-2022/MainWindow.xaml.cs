using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _17_10_2022
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DBEntities DB = new DBEntities(); // переменная бд

        public static Tracking track = new Tracking(); // переменная записи лога в БД
        private int loginCounts = 0; // счетчик попыток авторизации

        private static bool loginNow = false; // была ли авторизация за данный сеанс

        public static Users loginedUser; // пользователь под которым был вход

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            int userCounts = 0; // переменная для хранения кол-ва неподходящих пользователей
            foreach(Users user in DB.Users) // цикл проверки всех пользователей из базы
            {
                if (TBLogin.Text == user.Email) // проверка совпадения логина
                {
                    if (CreateMD5(TBPass.Text) == user.Password.ToUpper()) // проверка пароля (изначально выглядик как TBPass.Text == user.Password.ToUpper()  )
                    {
                        if (user.Active) // проверка активен ли пользователь
                        {
                            loginedUser = user;
                            loginNow = true;
                            track.UserID = loginedUser.ID; // запись ИД авторизированного пользователя в строку лога
                            using (StreamWriter logwriter = new StreamWriter("log.txt", true)) // запись лога авторизации в текстовый файл
                            {
                                logwriter.WriteLine("Login " + DateTime.Now + " " + loginedUser.ID);
                            }
                            
                            // открытие окна в зависимости от роли пользователя
                            if (user.RoleID == 1)
                            {
                                Windows.AdminWindow win = new Windows.AdminWindow(user);
                                win.Show();
                                this.Close();
                            }
                            else
                            {
                                Windows.UserWindow win = new Windows.UserWindow();
                                win.Show();
                                this.Close();
                            }
                        }
                        else // если пользователь заблокирован в системе, то ему выдаст ошибку
                        {
                            MessageBox.Show("Данный пользователь заблокирован в системе", "Вход не выполнен", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else // если неверный логин или пароль, то выдает пользователю ошибку
                    {
                        MessageBox.Show("Неверный логин или пароль", "Вход не выполнен", MessageBoxButton.OK, MessageBoxImage.Warning);
                        loginCounts += 1; // счетчик попыток входа
                        if (loginCounts == 3) // если 3 неудачных попытки, то вызывается 
                        {
                            loginCD();// метод задержки входа (будет представлен ниже)
                        }
                    }
                }
                else
                {
                    userCounts += 1; // счетчик неподходящих данных
                }
            }
            if (userCounts == DB.Users.Count()) //если ниодного правильного логина, то выводит ошибку
            {
                MessageBox.Show("Неверный логин или пароль", "Вход не выполнен", MessageBoxButton.OK, MessageBoxImage.Warning);
                loginCounts += 1;//счетчик попыток входа
                if (loginCounts == 3)// если 3 неудачных попытки, то вызывается
                {
                    loginCD(); // метод задержки входа (будет представлен ниже)
                }
            }
        }

        //метод кнопки выхода из программы
        private void Exit(object sender, RoutedEventArgs e)
        {
            using (StreamWriter logwriter = new StreamWriter("log.txt", true)) //запись о выходе в текстовый лог
            {
                logwriter.WriteLine("Exit " + DateTime.Now);
            }
            Application.Current.Shutdown(); // полный выход из программы
        }

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            track.StartDate = DateTime.Now; // Задает переменной трекинга дату начала записи

            if (!File.Exists("log.txt")) // создает файл лога, если его не было
            {
                using (FileStream fs = File.Create("log.txt"))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes($"Start logging {DateTime.Now}");
                    fs.Write(info, 0, info.Length);
                }
            }

            if (loginNow == false) // проверяет правильность предыдущего выключения программы
            {
                string log = null;
                using (StreamReader logreader = new StreamReader("log.txt", true))
                {
                    string line;
                    while ((line = logreader.ReadLine()) != null) // чтение лог файла
                    {
                        log = line;
                    }
                }
                if (log != null && log.Split(' ')[0] == "Login") // если последняя строка - авторизация, то выдает исключение и отправляет его в БД
                {
                    Exception exception = new Exception($"{log.Split(' ')[1]} {log.Split(' ')[2]} {log.Split(' ')[3]}");
                    Windows.ExecptionWindow win = new Windows.ExecptionWindow(exception, 1);
                    win.ShowDialog();
                    using (StreamWriter logwriter = new StreamWriter("log.txt", true))
                    {
                        logwriter.WriteLine("Send report into DB " + DateTime.Now);
                    }
                }
                else if (log == null)
                {
                    LogReset();
                    throw new Exception("Замечено внедрение в систему ведения логов.");
                }
            }
        }

        private void LogReset()
        {
            using (StreamWriter logwriter = new StreamWriter("log.txt", false))
            {
                logwriter.WriteLine("Start logging " + DateTime.Now);
            }
        }

        private async void loginCD() //Метод задержки входа
        {
            btn_login.IsEnabled = false;
            for (int i = 10; i >= 0; i--)
            {
                TBTimer.Text = $"Повторите попытку через {i} секунд";
                await Task.Delay(1000);
            }
            TBTimer.Text = "";
            btn_login.IsEnabled = true;
        }
    }
}
