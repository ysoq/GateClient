using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CodeCore
{
    public class WebsocketClient
    {
        private ClientWebSocket _webSocket = new ClientWebSocket();

        public WebsocketClient()
        {
            WeakReferenceMessenger.Default.Register<WebsocketMessage>(this, HandleWebsocketMessage);
        }

        Dictionary<string, TaskCompletionSource<string>> HttpCallback = new Dictionary<string, TaskCompletionSource<string>>();
        private void HandleWebsocketMessage(object recipient, WebsocketMessage message)
        {
            var completionTask = new TaskCompletionSource<string>();
            HttpCallback[message.HttpId] = completionTask;
            message.Reply(completionTask.Task);
            SendMessageAsync(message.HttpId, message.Api, message.JsonContent);
        }

        public async Task ConnectAsync(string uri)
        {
            while (true)
            {
                try
                {
                    _webSocket = new ClientWebSocket();
                    await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                    // 接收消息的任务
                    await ReceiveMessages();
                }
                catch (Exception ex)
                {
                    if (HttpCallback.Any())
                    {
                        try
                        {
                            foreach (var pair in HttpCallback)
                            {
                                pair.Value.SetResult("error");
                            }
                        }
                        catch (Exception)
                        {

                        }
                        HttpCallback.Clear();
                    }
                }
                // 等待一段时间后再次尝试连接
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        private async Task ReceiveMessages()
        {
            var buffer = new byte[1024 * 1024];
            while (_webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                var segment = new ArraySegment<byte>(buffer);
                do
                {
                    result = await _webSocket.ReceiveAsync(segment, CancellationToken.None);
                    var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                    AnalyzeMessage(message);
                }
                while (!result.EndOfMessage && _webSocket.State == WebSocketState.Open);
            }
        }

        private void AnalyzeMessage(string message)
        {
            try
            {
                var responseData = JsonConvert.DeserializeObject<JObject>(message);
                var httpId = responseData.Value<string>("httpId");
                var response = responseData.Value<string>("response");
                if (HttpCallback.ContainsKey(httpId))
                {
                    HttpCallback[httpId].SetResult(response);
                }
                HttpCallback.Remove(httpId);
            }
            catch (Exception ex)
            {
            }
        }

        public async Task SendMessageAsync(string httpId, string api, string body)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                var message = JsonConvert.SerializeObject(new
                {
                    api,
                    body,
                    httpType = httpId,
                    httpId 
                });
                var buffer = System.Text.Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(buffer);
                await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (_webSocket.State != WebSocketState.Connecting)
            {
                AnalyzeMessage(JsonConvert.SerializeObject(new
                {
                    httpType = httpId,
                    httpId,
                    success = false,
                    response = "error"
                }));
            }
        }
    }
}
