using CodeCore;
using GateClient.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GateClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            DispatcherHelper.Initialize();
            Util.ConfigureServices(ConfigureServices);
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IMainWindow, MainWindow>();
            services.AddSingleton<MainViewModel>();

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Appsettings.Default.Version = $"V{version!.Major}.{version!.Minor}";
        }

        private static Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new Mutex(true, "GateClient");
            if (mutex.WaitOne(0, false))
            {
                base.OnStartup(e);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("程序已启动");
                this.Shutdown();
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var window = Util.Injection.GetService<IMainWindow>()!;
            window.Show();
        }
    }
}
