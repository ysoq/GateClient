using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CodeCore
{
    public class RatioZoom : Behavior<DependencyObject>
    {


        public static double GetDesign(DependencyObject obj)
        {
            return (double)obj.GetValue(DesignProperty);
        }

        public static void SetDesign(DependencyObject obj, double value)
        {
            obj.SetValue(DesignProperty, value);
        }

        // Using a DependencyProperty as the backing store for Design.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DesignProperty =
            DependencyProperty.RegisterAttached("Design", typeof(double), typeof(RatioZoom), new PropertyMetadata(1920.0));

        public static RatioZommType GetType(DependencyObject obj)
        {
            return (RatioZommType)obj.GetValue(TypeProperty);
        }

        public static void SetType(DependencyObject obj, RatioZommType value)
        {
            obj.SetValue(TypeProperty, value);
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.RegisterAttached("Type", typeof(RatioZommType), typeof(RatioZoom), new PropertyMetadata(RatioZommType.Width));

        protected override void OnAttached()
        {
            var control = this.AssociatedObject;
            var design = GetDesign(this);
            var type = GetType(this);
            var ratio = Appsettings.Default.AppWidth / design;
            switch (type)
            {
                case RatioZommType.Width:
                    var width = Convert.ToDouble(control.GetValue(FrameworkElement.WidthProperty));
                    control.SetValue(FrameworkElement.WidthProperty, width * ratio);
                    break;
                case RatioZommType.Height:
                    var height = Convert.ToDouble(control.GetValue(FrameworkElement.HeightProperty));
                    control.SetValue(FrameworkElement.HeightProperty, height * ratio);
                    break;
                case RatioZommType.Margin:
                    var margin = (Thickness)control.GetValue(FrameworkElement.MarginProperty);
                    margin.Top *= ratio;
                    margin.Bottom *= ratio;
                    margin.Left *= ratio;
                    margin.Right *= ratio;
                    control.SetValue(FrameworkElement.MarginProperty, margin);
                    break;
                case RatioZommType.FontSize:
                    var fontsize = (double)control.GetValue(TextBlock.FontSizeProperty);
                    control.SetValue(TextBlock.FontSizeProperty, fontsize * ratio);
                    break;
                case RatioZommType.Padding:
                    var padding = (Thickness)control.GetValue(Border.PaddingProperty);
                    padding.Top *= ratio;
                    padding.Bottom *= ratio;
                    padding.Left *= ratio;
                    padding.Right *= ratio;
                    control.SetValue(Border.PaddingProperty, padding);
                    break;
                default:
                    break;
            }
        }

    }
    public enum RatioZommType
    {
        Width,
        Height,
        FontSize,
        Padding,
        Margin
    }

}