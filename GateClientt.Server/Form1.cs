using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace GateClientt.Server
{
    public partial class Form1 : Form
    {
        HttpClient HttpClient = null;

        public Form1()
        {
            InitializeComponent();
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("user-agent", "GateClientt.Server");
            this.Load += Form1_Load;
        }
        double timeout = 0;
        double maxTimeout = 0;
        private async void Form1_Load(object sender, EventArgs e)
        {
            do
            {
                var startTime = DateTime.Now;
                var result = await UseHttpJson("http://localhost:4738/gate/ws/getGateInfo", new
                {
                    code = "SCD03",
                    password = "SCD03",
                });
                var logUseTime = DateTime.Now - startTime;
                timeout = (logUseTime.TotalSeconds + timeout) / 2;
                this.label1.Text = "平均耗时" + timeout.ToString();
                maxTimeout = Math.Max(maxTimeout, logUseTime.TotalSeconds);
                this.label2.Text = "最大耗时" + maxTimeout.ToString();
                await Task.Delay(5000);
            } while (true);
        }


        public   async Task<HttpResponse> UseHttpJson(string api, object args)
        {
            var httpId = DateTime.Now.ToString("ffff");
            var jsonContent = JsonConvert.SerializeObject(args);

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
                        resultData.Error = new Exception("网络请求错误，日志代码：" + httpId);
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
                resultData.Error = new Exception("网络请求错误，日志代码：" + httpId);
            }

            return resultData;
        }
    }

    public class HttpResponse
    {
        public bool RequestSuccess { get; set; }
        public string JsonData { get; set; }
        public Exception Error { get; set; }

        public T GetData<T>()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.JsonData))
                {
                    return JsonConvert.DeserializeObject<T>(JsonData);
                }
                return default;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
