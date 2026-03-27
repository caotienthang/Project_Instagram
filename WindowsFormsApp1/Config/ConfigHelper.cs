using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsFormsApp1.Config
{
    public static class ConfigHelper
    {
        private static readonly string ConfigPath = "config.json";

        public static AppConfig Load()
        {
            if (!File.Exists(ConfigPath))
            {
                return new AppConfig();
            }

            string json = File.ReadAllText(ConfigPath);
            return JsonConvert.DeserializeObject<AppConfig>(json);
        }

        public static void Save(AppConfig config)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
