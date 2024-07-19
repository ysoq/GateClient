using CodeCore.Impl;
using CodeCore.ProwayGate;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog.Core;
using System.Net;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Media;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using Logger = CodeCore.Impl.Logger;

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
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
                UseCookies = true,
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            };
            HttpClient = new HttpClient(handler);

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
                data.StatusNameMargin = new Thickness(0, 80, 0, 0);
                data.LeftTopTextFontSize = 18;
                data.LeftTopTextMargin = new Thickness(10);
                data.RightBottomTextFontSize = 18;
                data.RightBottomTextMargin = new Thickness(10);
                data.ErrorMsgFontSize = 30;
            }
            PageSizeInfo.Default = data;
        }

        public static ILogger logger => Injection.GetService<ILogger>()!;

        static HttpClient HttpClient = null;
        public static async Task<HttpResponse> UseHttpJson1(string api, object args, bool writeLog = true)
        {
            if (!Accredit)
            {
                return new HttpResponse()
                {
                    RequestSuccess = false,
                    Error = new Exception("授权失败"),
                };
            }
            var httpId = Random.Shared.Next(1000, 9999).ToString();
            string apiUrl = api + $"?rand={httpId}";

            var jsonSetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            var jsonContent = JsonConvert.SerializeObject(args, Formatting.None, jsonSetting);
            logger.IfInfo(writeLog, httpId, apiUrl, jsonContent);

            var resultData = new HttpResponse();
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                req.Content = JsonContent.Create(JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent));
                var startTime = DateTime.Now;
                var response = await HttpClient.SendAsync(req);
                var logUseTime = DateTime.Now - startTime;

                if (!response.IsSuccessStatusCode)
                {
                    logger.Error(httpId, "网络请求错误，错误代码：", response.StatusCode.ToString());

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        resultData.JsonData = await response.Content.ReadAsStringAsync();
                        var jsonData = resultData.GetData<JObject>();
                        var msg = jsonData?.Value<string>("msg") ?? "网络请求错误";
                        resultData.RequestSuccess = false;
                        resultData.Error = new Exception(msg);
                        logger.Error(httpId, resultData.JsonData);
                    }
                    else
                    {
                        resultData.RequestSuccess = false;
                        resultData.Error = new Exception("网络请求错误，日志代码：" + httpId);
                    }

                    return resultData;
                }

                resultData.RequestSuccess = true;
                resultData.JsonData = await response.Content.ReadAsStringAsync();

                var responseJson = resultData.JsonData?.Replace("\t", "")?.Replace("\n", "");
                logger.IfInfo(writeLog, httpId, responseJson);
                if (!writeLog && logUseTime.TotalSeconds > 2)
                {
                    logger.Info(httpId, api, jsonContent, responseJson);
                }
                logger.IfInfo(logUseTime.TotalSeconds > 2, httpId, $"耗时{logUseTime.TotalSeconds:0.00}s");
            }
            catch (Exception ex)
            {
                resultData.RequestSuccess = false;
                resultData.Error = new Exception("网络请求错误，日志代码：" + httpId);
                logger.Error(ex, httpId);
            }

            return resultData;
        }

        static RestClient RestRequest = new RestClient();
        public static HttpResponse UseHttpJson(string logType, string api, object args, bool writeLog = true)
        {
            try
            {
                var httpId = Random.Shared.Next(1000, 9999).ToString();
                string apiUrl = api + $"?rand={httpId}";

                var jsonContent = JsonConvert.SerializeObject(args);

                logger.IfInfo(writeLog, httpId, apiUrl, jsonContent);

                var startTime = DateTime.Now;
                var response = _useHttpJson(apiUrl, jsonContent);
                var logUseTime = DateTime.Now - startTime;

                logger.IfInfo(writeLog, httpId, response.JsonData ?? "", response.Error?.Message ?? "");

                if (!writeLog && logUseTime.TotalSeconds > 2)
                {
                    logger.Info(httpId, api, jsonContent, response.JsonData ?? "");
                }
                logger.IfInfo(logUseTime.TotalSeconds > 2, httpId, $"{logType}耗时{logUseTime.TotalSeconds:0.00}s");

                return response;

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new HttpResponse { RequestSuccess = false, Error = new Exception("网络请求错误") };
            }
        }

        public static async Task<HttpResponse> _useHttpJsonAsync(string api, string jsonContent)
        {
            var request = new RestRequest(api, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "*/*");
            request.AddJsonBody(jsonContent);

            var response = await RestRequest.ExecuteAsync(request);
            var resData = new HttpResponse();
            if (response.IsSuccessStatusCode)
            {
                resData.RequestSuccess = true;
                resData.JsonData = response.Content;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    resData.JsonData = response.Content;
                    var jsonData = resData.GetData<JObject>();
                    var msg = jsonData?.Value<string>("msg") ?? "网络请求错误";
                    resData.RequestSuccess = false;
                    resData.Error = new Exception(msg);
                }
                else
                {
                    resData.RequestSuccess = false;
                    resData.Error = new Exception("网络请求错误");
                }
            }
            return resData;
        }

        public static async Task<HttpResponse> UseHttpJsonAsync(string logType, string api, object args, bool writeLog = true)
        {
            try
            {

                var httpId = Random.Shared.Next(1000, 9999).ToString();
                string apiUrl = api + $"?rand={httpId}";

                var jsonContent = JsonConvert.SerializeObject(args);

                logger.IfInfo(writeLog, httpId, apiUrl, jsonContent);

                var startTime = DateTime.Now;
                var response = await _useHttpJsonAsync(apiUrl, jsonContent);
                var logUseTime = DateTime.Now - startTime;

                logger.IfInfo(writeLog, httpId, response.JsonData ?? "", response.Error?.Message ?? "");

                if (!writeLog && logUseTime.TotalSeconds > 2)
                {
                    logger.Info(httpId, api, jsonContent, response.JsonData ?? "");
                }
                logger.IfInfo(logUseTime.TotalSeconds > 2, httpId, $"{logType}耗时{logUseTime.TotalSeconds:0.00}s");

                return response;

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new HttpResponse { RequestSuccess = false, Error = new Exception("网络请求错误") };
            }
        }

        public static HttpResponse _useHttpJson(string api, string jsonContent)
        {
            var request = new RestRequest(api, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "*/*");
            request.AddJsonBody(jsonContent);

            var response = RestRequest.Execute(request);
            var resData = new HttpResponse();
            if (response.IsSuccessStatusCode)
            {
                resData.RequestSuccess = true;
                resData.JsonData = response.Content;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    resData.JsonData = response.Content;
                    var jsonData = resData.GetData<JObject>();
                    var msg = jsonData?.Value<string>("msg") ?? "网络请求错误";
                    resData.RequestSuccess = false;
                    resData.Error = new Exception(msg);
                }
                else
                {
                    resData.RequestSuccess = false;
                    resData.Error = new Exception("网络请求错误");
                }
            }
            return resData;
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
