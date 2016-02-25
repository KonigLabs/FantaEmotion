using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NewFanta
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += (s, ev) =>
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ev.Exception);
                ((IDisposable)Current.MainWindow).Dispose();
            };
        }
    }
}
