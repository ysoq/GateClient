using CodeCore.Impl;
using CodeCore.ProwayGate;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Windows;
using System.Windows.Media;
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
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true,
                Proxy = null,
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

        public static async Task<HttpResponse> UseHttpJsonAsync(string logType, string api, object args, bool writeLog = true)
        {
            try
            {
                var httpId = Random.Shared.Next(1000, 9999).ToString();
                string apiUrl = api + $"?rand={httpId}";
                var jsonContent = JsonConvert.SerializeObject(args);

                logger.IfInfo(writeLog, httpId, apiUrl, jsonContent);

                var startTime = DateTime.Now;
                var response = await _useHttpJsonByCurl(apiUrl, jsonContent);
                var logUseTime = DateTime.Now - startTime;

                if (!string.IsNullOrEmpty(response.JsonData))
                {
                    logger.IfInfo(writeLog, httpId, response.JsonData);
                }
                else
                {
                    logger.IfInfo(writeLog, httpId, response.ResponseError!);
                }

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


        static HttpClient HttpClient = null;
        public static async Task<HttpResponse> _useHttpJsonByHttpClient(string api, string jsonContent)
        {

            var resultData = new HttpResponse();
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, api);
                req.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await HttpClient.SendAsync(req);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        resultData.JsonData = await response.Content.ReadAsStringAsync();
                        var jsonData = resultData.GetData<JObject>();
                        var msg = jsonData?.Value<string>("msg") ?? "网络请求错误";
                        resultData.RequestSuccess = false;
                        resultData.Error = new Exception(msg);
                    }
                    else
                    {
                        resultData.RequestSuccess = false;
                        resultData.Error = new Exception("网络请求错误");
                    }

                    return resultData;
                }

                resultData.RequestSuccess = true;
                resultData.JsonData = await response.Content.ReadAsStringAsync();
                resultData.JsonData = resultData.JsonData?.Replace("\t", "")?.Replace("\n", "");

            }
            catch (Exception ex)
            {
                resultData.RequestSuccess = false;
                resultData.Error = new Exception("网络请求错误");
            }

            return resultData;
        }

        private static Task<HttpResponse> _useHttpJsonByCurl(string api, string jsonContent)
        {
            TaskCompletionSource<HttpResponse> completionTask = new TaskCompletionSource<HttpResponse>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                var jsonData = JsonConvert.SerializeObject(jsonContent);

                string command = $"curl -m 10 -X POST -H \"Content-Type: application/json\" -d {jsonData} {api}";
                var res = ExecuteCommand("cmd.exe", command);
                cancellationTokenSource.Cancel();
                completionTask.SetResult(res);
            });
            Task.Delay(11000, cancellationTokenSource.Token).ContinueWith((s) =>
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    completionTask.SetResult(new HttpResponse()
                    {
                        Error = new Exception("网络请求错误"),
                        RequestSuccess = false
                    });
                }
            });

            return completionTask.Task;

        }

        public static HttpResponse ExecuteCommand(string fileName, string command)
        {
            try
            {
                StringBuilder successSb = new StringBuilder();
                StringBuilder errorSb = new StringBuilder();

                //创建一个进程
                Process process = new Process();
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = "/c" + command;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;

                // 必须禁用操作系统外壳程序
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                //启动进程
                process.Start();

                //等待退出
                process.WaitForExit();

                var standardOutput = process.StandardOutput;
                var standardError = process.StandardError;

                var output = standardOutput.ReadToEnd();
                var error = standardError.ReadToEnd();


                //关闭进程
                process.Close();
                return new HttpResponse()
                {
                    JsonData = output,
                    Error = new Exception("网络错误"),
                    ResponseError = error,
                    RequestSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new HttpResponse()
                {
                    Error = ex,
                    RequestSuccess = false,
                };
            }
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
