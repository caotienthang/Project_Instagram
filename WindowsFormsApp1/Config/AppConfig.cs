using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
namespace WindowsFormsApp1.Config
{
    public class AppConfig
    {
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string ClientSecret { get; set; }
        public string AccessToken { get; set; }
    }
}
