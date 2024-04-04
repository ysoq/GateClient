using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CodeCore
{
    public class Appsettings
    {
        public static Appsettings? Default { get; set; }
        public  Appsettings()
        {
            var jsonContent = File.ReadAllText("appsettings.json");
            keyValuePairs = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(jsonContent);
            AppWidth = SystemParameters.PrimaryScreenWidth;
            AppHeight = SystemParameters.PrimaryScreenHeight;
#if DEBUG
            AppWidth = 768;
            AppHeight = 1366;
#endif
        }

        private JObject? keyValuePairs = null;

        public T? Get<T>(string key)
        {
            return keyValuePairs == null ? default(T) : keyValuePairs.Value<T>(key);
        }

        public JObject? GetNode(string key)
        {
            return Get<JObject>(key);
        }

        public double AppWidth { get; private set; }

        public double AppHeight { get; private set; }

    }
}
