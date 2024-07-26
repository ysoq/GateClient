using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CodeCore
{
    public class HttpMessage: AsyncRequestMessage<string>
    {
        public HttpMessage(string httpId, string url, string jsonContent)
        {
            HttpId = httpId;
            Url = url;
            JsonContent = jsonContent;
        }

        public string HttpId { get; }
        public string Url { get; }
        public string JsonContent { get; }
    }
}
