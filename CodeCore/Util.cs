using CodeCore.Impl;
using CodeCore.ProwayGate;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
                AppWidth = 800,
                AppHeight = 600,
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

            services.AddSingleton(s => new GateUtil());

            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<ICertUtil, CertUtil>();
            services.AddSingleton<IQrUtil, QrUtil>();

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

        static HttpClient HttpClient = new HttpClient();
        public static async Task<HttpResponse> UseHttpJson(string api, object args)
        {
            var httpId = Random.Shared.Next(1000, 9999).ToString();

            var logger = Injection.GetService<ILogger>()!;
            logger.Info(httpId, api, JsonConvert.SerializeObject(args));
            var resultData = new HttpResponse();
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, api);
                req.Content = JsonContent.Create(args);
                var response = await HttpClient.SendAsync(req);
                if (!response.IsSuccessStatusCode)
                {
                    resultData.Success = false;
                    resultData.Error = new Exception("网络请求错误，日志代码：" + httpId);
                    logger.Error(httpId, "网络请求错误，错误代码：", response.StatusCode.ToString());
                    return resultData;
                }

                resultData.Success = true;
                resultData.JsonData = await response.Content.ReadAsStringAsync();
                logger.Error(httpId, resultData.JsonData);
            }
            catch (Exception ex)
            {
                resultData.Success = false;
                resultData.Error = new Exception("网络请求错误，日志代码：" + httpId);
                logger.Error(ex, httpId);
            }

            return resultData;
        }

        public static IServiceProvider Injection { get; internal set; }
        public static void ConfigureServices(Action<ServiceCollection> register)
        {
            var services = new ServiceCollection();
            services.RegisterCodeCore();
            register?.Invoke(services);
            Injection = services.BuildServiceProvider();
        }
    }
}
