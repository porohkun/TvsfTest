using System;
using System.Windows;
using System.Windows.Threading;

namespace TvsfTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"{e.Exception.Message}\r\n\r\n{e.Exception.StackTrace}");
        }

    }
}
