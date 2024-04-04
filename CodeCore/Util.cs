using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CodeCore
{
    public class Util
    {
        public static Color ToColor(string rgb)
        {
            return (Color)ColorConverter.ConvertFromString(rgb);
        }

        public static SolidColorBrush ToBrush(string rgb)
        {
            try
            {
                return new SolidColorBrush(ToColor(rgb));
            }
            catch (Exception)
            {
                return Brushes.Red;
            }
        }
    }
}
