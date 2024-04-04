using CodeCore.Impl;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CodeCore
{
    public static class Util
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

        public static void RegisterCodeCore(this ServiceCollection services)
        {
            var jsonContent = File.ReadAllText("appsettings.json");

            var setting = new Appsettings()
            {
                KeyValuePairs = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(jsonContent),
#if DEBUG
                AppWidth = 768,
                AppHeight = 1366,
                Debug = true,
#endif
#if !DEBUG
                    AppWidth = SystemParameters.PrimaryScreenWidth,
                    AppHeight = SystemParameters.PrimaryScreenHeight,
#endif
            };
            Appsettings.Default = setting;
            services.AddSingleton((s) =>
            {
                return Appsettings.Default;
            });
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IQuartz, Impl.Quartz>();


            var data = new PageSizeInfo();

            if (Appsettings.Default?.AppWidth == 768)
            {
                data.BorderMargin = new Thickness(84, 0, 84, 117);
                data.BorderPadding = new Thickness(100);
                data.LeftTopTextFontSize = 24;
                data.LeftTopTextMargin = new Thickness(17, 24, 0, 0);
                data.StatusNameFontSize = 60;
                data.StatusNameMargin = new Thickness(0, 0, 0, 200);
                data.RightBottomTextFontSize = 24;
                data.RightBottomTextMargin = new Thickness(18);
            }
            PageSizeInfo.Default = data;
        }
    }
}   
