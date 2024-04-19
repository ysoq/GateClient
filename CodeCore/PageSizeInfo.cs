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
        public double ErrorMsgFontSize { get; set; } = 30;
        public Thickness LeftTopTextMargin { get; set; } = new Thickness(74, 68, 74, 68);
        public double RightBottomTextFontSize { get; set; } = 40;
        public Thickness RightBottomTextMargin { get; set; } = new Thickness(30, 53, 30, 53);
        public double StatusNameFontSize { get; set; } = 100;
        public Thickness StatusNameMargin { get; set; } = new Thickness(0, 100, 0, 0);
        public Thickness BorderMargin { get; set; } = new Thickness(195, 0, 195, 271);
        public int BorderWidth { get; set; } = 686;
        public int IconWidth { get; set; } = 180;
    }
}
