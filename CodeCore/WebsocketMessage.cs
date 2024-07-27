using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCore
{
    public class WebsocketMessage: CommunityToolkit.Mvvm.Messaging.Messages.AsyncRequestMessage<string>
    {
        public WebsocketMessage(string httpId, string api, string jsonContent)
        {
            HttpId = httpId;
            Api = api;
            JsonContent = jsonContent;
        }

        public string HttpId { get; }
        public string Api { get; }
        public string JsonContent { get; }
    }
}
