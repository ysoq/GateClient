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
            Services = ConfigureServices();
        }

        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; }
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.RegisterCodeCore();
            services.AddSingleton<IMainWindow, MainWindow>();
            services.AddSingleton<MainViewModel>();

            return services.BuildServiceProvider();
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
            var window = App.Current.Services.GetService<IMainWindow>()!;
            window.Show();
        }
    }
}
