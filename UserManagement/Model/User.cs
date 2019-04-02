using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserManagement.Model
{
    public class User
    {
        public string ApplicationId { get; set; }

        public Guid UserID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}