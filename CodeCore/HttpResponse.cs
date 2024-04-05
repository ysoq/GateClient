using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCore
{
    public class HttpResponse
    {
        public bool Success { get; set; }
        public string JsonData { get; set; }
        public Exception? Error { get; set; }

        public T? GetData<T>()
        {
            if (Success)
            {
                return JsonConvert.DeserializeObject<T>(JsonData);
            }
            else
            {
                return default;
            }
        }
    }
}
