﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UserManagement.Entities;

namespace UserManagement.Models
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public IEnumerable<string> Roles;
     
    }

}
