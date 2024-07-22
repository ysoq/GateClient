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
using System.Diagnostics;

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
                var result = UseHttpJson("http://cdapi2.qualitrip.cn/gate/ws/getGateInfo", new
                {
                    code = "SCD03",
                    password = "SCD03",
                });
                var logUseTime = DateTime.Now - startTime;
                timeout = (logUseTime.TotalSeconds + timeout) / 2;
                this.label1.Text = "平均耗时" + timeout.ToString();
                maxTimeout = Math.Max(maxTimeout, logUseTime.TotalSeconds);
                this.label2.Text = "最大耗时" + maxTimeout.ToString();
                this.label3.Text = result.JsonData;
                await Task.Delay(5000);
            } while (true);
        }


        public HttpResponse UseHttpJson(string api, object args)
        {
            var jsonData = JsonConvert.SerializeObject(JsonConvert.SerializeObject(args));

            string command = $"curl -X POST -H \"Content-Type: application/json\" -d {jsonData} {api}";
            return ExecuteCommand("cmd.exe", command);
        }

        public HttpResponse ExecuteCommand(string fileName, string command)
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
                    Error = new Exception(error),
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
