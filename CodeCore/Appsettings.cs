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

        public JObject? KeyValuePairs { get; internal set; }

        public T? Get<T>(string key)
        {
            return KeyValuePairs == null ? default(T) : KeyValuePairs.Value<T>(key);
        }

        public JObject? Node(string key)
        {
            return Get<JObject>(key);
        }

        public double AppWidth { get; internal set; }

        public double AppHeight { get; internal set; }

        public bool Debug { get; internal set; }

    }
}
