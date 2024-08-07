﻿using CodeCore;
using GateClient.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
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

            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            //UI线程未捕获异常处理事件（UI主线程）
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            //非UI线程未捕获异常处理事件(例如自己创建的一个子线程)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Util.Injection.GetService<ILogger>()?.Info("version:", Appsettings.Default?.Version);
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            var log = Util.Injection.GetService<ILogger>();
            try
            {
                log?.Error(e.Exception, "程序运行出错1:Task");
            }
            catch (Exception ex)
            {
                log?.Error(ex, "程序运行出错1:Task");
            }
            finally
            {
                e.SetObserved();
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var log = Util.Injection.GetService<ILogger>();
            log?.Error("程序运行出错2");
            MessageBox.Show("程序运行出错[2]");
            if (e.ExceptionObject is System.Exception)
            {
                log?.Error((System.Exception)e.ExceptionObject, "程序运行出错2");
            }
            Thread.Sleep(1000);
            Restart();
        }

        /// <summary>
        /// 重启程序
        /// </summary>
        public static void Restart()
        {
            var log = Util.Injection.GetService<ILogger>();

            log?.Info("重启程序");
            Task.Delay(300).ContinueWith((a) =>
            {
                string bat = Environment.CurrentDirectory + @"\restart.bat";
                if (!File.Exists(bat))
                {
                    File.WriteAllText(bat, @"
taskkill /f /im GateClient.exe
ping /n 3 127.0.0.1 >nul
ping /n 3 127.0.0.1 >nul
start GateClient.exe
                    ");
                }

                System.Diagnostics.Process.Start(bat);
            });
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var log = Util.Injection.GetService<ILogger>();
            log?.Error(e.Exception, "程序运行出错3");
            e.Handled = true;
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
