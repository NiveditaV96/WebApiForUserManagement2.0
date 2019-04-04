using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UserManagement.Model;
using UserManagement.Repository;

namespace UserManagement.Controllers
{
    public class LoginController : ApiController
    {
        ILoginRepository<User> _loginRepository;

        public LoginController(ILoginRepository<User> loginRepository)
        {
            _loginRepository = loginRepository;
        }

        [Route("UserLogin")]
        [HttpPost]
        public IHttpActionResult Login(User user)
        {
            int validationStatus = _loginRepository.Login(user);

            if (validationStatus == 1)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "Login successful."));
            }


            else if (validationStatus == 0)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. Invalid Username or Password."));
                //return BadRequest("Sorry. Invalid Username or Password.");
            }

            else
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Username does not exist."));
            }

        }
    }
}
