using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class InstagramSession
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Cookie { get; set; }
        public string FbDtsg { get; set; }
        public string Lsd { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
