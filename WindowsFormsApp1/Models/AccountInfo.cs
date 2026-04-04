using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class AccountInfo
    {
        public int Id { get; set; }
        public string FbAccountId { get; set; }  // Instagram Facebook user ID (fbid_v2)
        public string PhoneAccountId { get; set; }  // Instagram phone user ID (pk_id)

        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        // Local cached avatar path (downloaded image on disk)
        public string LocalPathAvatar { get; set; }
        // Remote avatar URL returned by Instagram API
        public string LinkAvatar { get; set; }
        public string Birthday { get; set; }
        public string Status { get; set; }
        public string Password { get; set; }
    }
}
