using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserManagement.Entities
{
    public class User
    {
        public string ApplicationId { get; set; }

        public Guid UserID { get; set; }

        public string Username { get; set; }

        public string LoweredUsername { get; set; }
        public string MobileAlias { get; set; }
        public string IsAnonymous { get; set; }
        public string LastActivityDate { get; set; }

    }
}