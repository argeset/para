using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Para.Server.Business.Helpers
{
    public class ConfigHelper
    {
        public static T GetValue<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrWhiteSpace(value))
                return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static bool IsProxy => GetValue<bool>("IsProxy");
        public static string Proxy => GetValue<string>("Proxy");
        public static int ProxyPort => GetValue<int>("ProxyPort");
    }
}
