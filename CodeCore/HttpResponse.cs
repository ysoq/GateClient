using Newtonsoft.Json;

namespace CodeCore
{
    public class HttpResponse
    {
        public bool RequestSuccess { get; set; }
        public string? JsonData { get; set; }
        public Exception? Error { get; set; }
        public string? ResponseError { get; set; }

        public T? GetData<T>()
        {
            try
            {
                if (!string.IsNullOrEmpty(JsonData))
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
