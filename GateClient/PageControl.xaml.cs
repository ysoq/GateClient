﻿using System.Windows.Media;
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


        public Color StartBg
        {
            get { return (Color)GetValue(StartBgProperty); }
            set { SetValue(StartBgProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartBg.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartBgProperty =
            DependencyProperty.Register("StartBg", typeof(Color), typeof(PageControl), new PropertyMetadata(Color.FromArgb(1, 204, 232, 252)));


        public Color EndBg
        {
            get { return (Color)GetValue(EndBgProperty); }
            set { SetValue(EndBgProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndBg.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndBgProperty =
            DependencyProperty.Register("EndBg", typeof(Color), typeof(PageControl), new PropertyMetadata(Color.FromArgb(1, 59, 185, 243)));




        public Brush ThemeBg
        {
            get { return (Brush)GetValue(ThemeBgProperty); }
            set { SetValue(ThemeBgProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThemeBg.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeBgProperty =
            DependencyProperty.Register("ThemeBg", typeof(Brush), typeof(PageControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(1, 2, 167, 240))));




        public Geometry ThemeIcon
        {
            get { return (Geometry)GetValue(ThemeIconProperty); }
            set { SetValue(ThemeIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThemeIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeIconProperty =
            DependencyProperty.Register("ThemeIcon", typeof(Geometry), typeof(PageControl));



    }
}
