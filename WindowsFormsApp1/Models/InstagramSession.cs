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

        // ── Web session (lấy qua AccountsCenterForm / WebView2) ──────
        public string Cookie { get; set; }
        public string FbDtsg { get; set; }
        public string Lsd { get; set; }

        // ── Phone session (lấy từ đăng nhập bằng điện thoại) ────────
        public string SessionIdPhone { get; set; }
        public string DsUserIdPhone { get; set; }
        public string CsrfTokenPhone { get; set; }
        public string AuthorizationPhone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
