using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UserManagement.DAL;
using UserManagement.Model;
using UserManagement.Models;
using UserManagement.Repository;
using System.Reflection;
using System.Resources;
using System.Globalization;

namespace UserManagement.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        IUserRepository _irepository;

        //ResourceManager rm = new ResourceManager("UsingRESX.UserControllerMessages",
        //        Assembly.GetExecutingAssembly());

        public UserController(IUserRepository irepository)
        {
            _irepository = irepository;
        }
        
        //include exception handling here 
        //make a separate ctlr for login

        [Route("Create")]
        [HttpPost]

        //public IHttpActionResult Create([FromBody](string username,string password,string role))
        //public IHttpActionResult Create(string username, string password, string role)
        public IHttpActionResult UserCreation([FromBody]JObject data)
        {
            try
            {
                
               UserModel user = data["Username"].ToObject<UserModel>();
               MembershipModel membership = data["Password"].ToObject<MembershipModel>();
               RoleModel role = data["RoleName"].ToObject<RoleModel>();
                
                var creationStatus = _irepository.CreateUser(user.Username, membership.Password, role.RoleName);

                if (creationStatus == 0)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "User creation failed"));
                    // return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, rm.GetString("creationSuccessful")));

                }

                else if(creationStatus == 1)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Password creation failed."));
                   
                }

                else if(creationStatus==2)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Role creation failed."));
                }

                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, "User created successfully."));
                }
            }
            catch(Exception)
            {
                throw;
                //              System.Resources.MissingManifestResourceException occurred
                //HResult = 0x80131532

                



                //throw new InvalidOperationException("User creation");
            }

        }



        //[Route("Login/{username}/{password}")]
        //[HttpGet]
        //public IHttpActionResult Login(string username, string password)
        //{
        //    int validationStatus = _irepository.LoginUser(username, password);

        //    if (validationStatus == 1)
        //    {
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "Login successful."));
        //    }


        //    else if (validationStatus == 0)
        //    {
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Please enter the correct password."));
        //        //return BadRequest("Sorry. Invalid Username or Password.");
        //    }

        //    else
        //    {
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Username does not exist."));
        //    }

        //}
        
        [HttpPut]
        [Route("UpdateUserName")]
        public IHttpActionResult UpdateUserName(string currentUsername, string newUsername)
        {
           
            
                bool updateUsernameStatus = _irepository.UpdateUserName(currentUsername, newUsername);

                if (updateUsernameStatus)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "Username updated."));
                }
                else
                {
                    if(String.IsNullOrEmpty(currentUsername) || String.IsNullOrEmpty(newUsername))
                    {
                        var exceptionMessage = new ArgumentNullException("Existing or New username cannot be null.");
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, exceptionMessage));
                    }

                    else
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Username updation failed."));
                    }
                 }
           
        }

        [HttpPut]
        [Route("UpdateUserRole")]
        public IHttpActionResult UpdateUserRole(string currentUsername, string currentRole, string newRole)
        {
            bool updateUserRoleStatus = _irepository.UpdateUserRole(currentUsername, currentRole, newRole);

            //how do we pass a variable value in the create respone method
            //like <User025> role has been updated successully to <HR>
            if (updateUserRoleStatus)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "User Role updated."));
            }
            else
            {
                return BadRequest("User Role updation failed. Please retry.");
            }
        }


        [Route("Delete")]
        [HttpDelete]
        public IHttpActionResult Delete(string userName)
        {
            bool deleteUserStatus = _irepository.DeleteUser(userName);
           // string Userid = UserId;

            if(deleteUserStatus)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "Requested user " + userName + " has been deleted."));
            }

            else
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Requested user " + userName + " is not found."));
            }

        }

        [Route("GetUsersByRole/{role}")]
        [HttpGet]
        public IHttpActionResult GetUsersByRole(string role)
        {
            IEnumerable<string> usersList = _irepository.GetUsersByRole(role);
            //Object[] username = _irepositoryGetUsersByRole(Role);
            var message = string.Format("List of users with {0} role {1}", role, usersList);

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, message));

        }
    }
}
