using CodeCore.Impl;
using CodeCore.ProwayGate;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
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

        public static bool Accredit { get; private set; } = true;

        public static void RegisterCodeCore(this ServiceCollection services)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
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

            if (Appsettings.Default?.AppWidth == 800 && Appsettings.Default?.AppHeight == 1280)
            {
                data.BorderMargin = new Thickness(0, 640, 0, 0);
                data.BorderWidth = 465;
                data.IconWidth = 268;
                data.StatusNameFontSize = 60;
                data.StatusNameMargin = new Thickness(0, 458, 0, 0);
                data.LeftTopTextFontSize = 36;
                data.LeftTopTextMargin = new Thickness(12, 18, 12, 18);
                data.RightBottomTextFontSize = 36;
                data.RightBottomTextMargin = new Thickness(12, 18, 12, 18);
            }
            else if (Appsettings.Default?.AppWidth == 800 && Appsettings.Default?.AppHeight == 600)
            {
                data.BorderMargin = new Thickness(0, 219, 0, 0);
                data.BorderWidth = 283;
                data.IconWidth = 180;
                data.StatusNameFontSize = 60;
                data.StatusNameMargin = new Thickness(0, 107, 0, 0);
                data.LeftTopTextFontSize = 24;
                data.LeftTopTextMargin = new Thickness(12, 18, 12, 18);
                data.RightBottomTextFontSize = 24;
                data.RightBottomTextMargin = new Thickness(12, 18, 12, 18);
            }
            PageSizeInfo.Default = data;
        }

        static HttpClient HttpClient = new HttpClient();
        public static async Task<HttpResponse> UseHttpJson(string api, object args)
        {
            if (!Accredit)
            {
                return new HttpResponse()
                {
                    Success = false,
                    Error = new Exception("授权失败"),
                };
            }
            var httpId = Random.Shared.Next(1000, 9999).ToString();

            var logger = Injection.GetService<ILogger>()!;
            var jsonSetting = new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore ,
            };
            var jsonContent = JsonConvert.SerializeObject(args, Formatting.Indented, jsonSetting);
            logger.Info(httpId, api, jsonContent);

            var resultData = new HttpResponse();
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, api);
                req.Content = JsonContent.Create(JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent));
                var response = await HttpClient.SendAsync(req);
                if (!response.IsSuccessStatusCode)
                {
                    logger.Error(httpId, "网络请求错误，错误代码：", response.StatusCode.ToString());

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        resultData.JsonData = await response.Content.ReadAsStringAsync();
                        var jsonData = resultData.GetData<JObject>();
                        var msg = jsonData?.Value<string>("msg") ?? "网络请求错误";
                        resultData.Success = false;
                        resultData.Error = new Exception(msg);
                    }
                    else
                    {
                        resultData.Success = false;
                        resultData.Error = new Exception("网络请求错误，日志代码：" + httpId);
                    }

                    return resultData;
                }

                resultData.Success = true;
                resultData.JsonData = await response.Content.ReadAsStringAsync();
                logger.Info(httpId, resultData.JsonData);
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
