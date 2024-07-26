using CodeCore;
using CommunityToolkit.Mvvm.Messaging;
using GateClient.Messager;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GateClient
{
    /// <summary>
    /// WebControl.xaml 的交互逻辑
    /// </summary>
    public partial class WebControl : UserControl
    {
        public WebControl()
        {
            InitializeComponent();
            this.Loaded += WebControl_Loaded;

            WeakReferenceMessenger.Default.Register<HttpMessage>(this, HttpMessageReceived);
        }
        Dictionary<string, TaskCompletionSource<string>> HttpCallback = new Dictionary<string, TaskCompletionSource<string>>();
        private void HttpMessageReceived(object recipient, HttpMessage message)
        {
            var completionTask = new TaskCompletionSource<string>();
            HttpCallback[message.HttpId] = completionTask;
            message.Reply(completionTask.Task);
            SendMessage($@"sendHttpPostJson('{message.HttpId}',`{message.Url}`, `{message.JsonContent}`)");
        }

        private async void WebControl_Loaded(object sender, RoutedEventArgs e)
        {
            await webView.EnsureCoreWebView2Async();
            this.webView.WebMessageReceived += webView_WebMessageReceived;

            webView.CoreWebView2.AddWebResourceRequestedFilter("http://www.example.com/empty.html", CoreWebView2WebResourceContext.All);
            webView.CoreWebView2.WebResourceRequested += (sender, args) =>
            {
                string str = @"<html>
<body>

<script>
   function callback(httpid, content) {
            const str = `${httpid}^^${content}`
            window.chrome.webview.postMessage(str);
        }
        function sendHttpPostJson(httpid, url, jsonData) {
            return fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=UTF-8'
                },
                body: jsonData
            })
                .then(async response => {
                    const json = await response.text();
                    callback(httpid, json);
                })
                .catch(() => {
                    callback(httpid, `error`);
                });
            }
</script>

</body>

</html>";             //convert string 2 stream            
                byte[] array = Encoding.ASCII.GetBytes(str);
                MemoryStream stream = new MemoryStream(array);             //convert stream 2 string      

                CoreWebView2WebResourceResponse response = webView.CoreWebView2.Environment.CreateWebResourceResponse(
                    stream, 200, "OK", "Content-Type: text/html");
                args.Response = response;
            };

            webView.Source = new Uri("http://www.example.com/empty.html");

            var json = JsonConvert.SerializeObject(new
            {
                code = "SZG-SCD03",
                password = "SZG-SCD03"
            });

        }

        private void webView_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string msg = e.TryGetWebMessageAsString();
            var arr = msg.Split("^^");
            if (arr.Length == 2)
            {
                if (HttpCallback.ContainsKey(arr[0]))
                {
                    HttpCallback[arr[0]].SetResult(arr[1]);
                }
            }
        }

        private void SendMessage(string message)
        {
            webView.CoreWebView2.ExecuteScriptAsync(message);
        }
    }
}
