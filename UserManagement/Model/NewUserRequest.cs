using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserManagement.Requests
{
    public class NewUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
    }
}