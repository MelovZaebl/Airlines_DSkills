using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using _17_10_2022.Windows;


namespace _17_10_2022
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            RegisterExceptionHandler();
        }

        private void RegisterExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                HandlerException(e.ExceptionObject as Exception);
            };

            DispatcherUnhandledException += (sender, e) =>
            {
                e.Handled = true;
                HandlerException(e.Exception);
            };

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                HandlerException(e.Exception);
            };
        }
        private void HandlerException(Exception e)
        {
            Window win = new Windows.ExecptionWindow(e, 0);
            win.ShowDialog();
        }
    }
}
