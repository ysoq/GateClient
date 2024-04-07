using CodeCore;
using GateClient.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GateClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        private readonly ILogger logger;

        public MainWindow(MainViewModel vm, ILogger logger)
        {
            InitializeComponent();
            this.DataContext = vm;

#if DEBUG
            this.MouseMove += (s, e) =>
            {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
#endif

            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;

            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = Appsettings.Default.AppWidth;
            this.Height = Appsettings.Default.AppHeight;
            this.logger = logger;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2) return;

            logger.Info("程序主动关闭");
            Close();
        }
    }
}
