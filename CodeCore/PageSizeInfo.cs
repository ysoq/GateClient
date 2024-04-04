using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CodeCore
{

    public class PageSizeInfo
    {
        public static PageSizeInfo? Default { get; set; }

        public double LeftTopTextFontSize { get; set; } = 40;
        public Thickness LeftTopTextMargin { get; set; } = new Thickness(74, 68, 74, 68);
        public double RightBottomTextFontSize { get; set; } = 40;
        public Thickness RightBottomTextMargin { get; set; } = new Thickness(30, 53, 30, 53);
        public double StatusNameFontSize { get; set; } = 100;
        public Thickness StatusNameMargin { get; set; } = new Thickness(0, 0, 0, 300);
        public Thickness BorderMargin { get; set; } = new Thickness(195, 0, 195, 271);
        public Thickness BorderPadding { get; set; } = new Thickness(120);

        public PageSizeInfo()
        {
            if (Appsettings.Default?.AppWidth == 768)
            {
                BorderMargin = new Thickness(84, 0, 84, 117);
                BorderPadding = new Thickness(100);
                LeftTopTextFontSize = 24;
                LeftTopTextMargin = new Thickness(17, 24, 0, 0);
                StatusNameFontSize = 60;
                StatusNameMargin = new Thickness(0, 0, 0, 200);
                RightBottomTextFontSize = 24;
                RightBottomTextMargin = new Thickness(18);
            }
        }
    }
}
