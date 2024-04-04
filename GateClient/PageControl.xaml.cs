using System.Windows;
using System.Windows.Controls;

namespace GateClient
{
    /// <summary>
    /// PageControl.xaml 的交互逻辑
    /// </summary>
    public partial class PageControl : UserControl
    {
        public PageControl()
        {
            InitializeComponent();
        }




        public string StatusName
        {
            get { return (string)GetValue(StatusNameProperty); }
            set { SetValue(StatusNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusNameProperty =
            DependencyProperty.Register("StatusName", typeof(string), typeof(PageControl), new PropertyMetadata(""));


        public string LeftTopText
        {
            get { return (string)GetValue(LeftTopTextProperty); }
            set { SetValue(LeftTopTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftTopText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftTopTextProperty =
            DependencyProperty.Register("LeftTopText", typeof(string), typeof(PageControl), new PropertyMetadata(""));




        public string RightBottomText
        {
            get { return (string)GetValue(RightBottomTextProperty); }
            set { SetValue(RightBottomTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightBottomText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightBottomTextProperty =
            DependencyProperty.Register("RightBottomText", typeof(string), typeof(PageControl), new PropertyMetadata(""));

    }
}
