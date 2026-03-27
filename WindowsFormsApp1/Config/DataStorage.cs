using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Config
{
    public static class DataStorage
    {
        private static string path = "account_data.json";

        public static void Save(AccountInfo data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
