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
using UserManagement.Filters;
using UserManagement.Requests;

namespace UserManagement.Controllers
{
    [RoutePrefix("api/User")]
    [Logger]
    public class UserController : ApiController
    {
        IRepository<UserModel> _userrepository;
        ISearchUsers _searchUserRepository;
        IUpdateUsers _updateUserRepository;

        //ResourceManager rm = new ResourceManager("UsingRESX.UserControllerMessages",
        //        Assembly.GetExecutingAssembly());

        public UserController(IRepository<UserModel> irepository, 
            ISearchUsers searchUserRepository, 
            IUpdateUsers updateUserRepository)
        {
            _userrepository = irepository;
            _searchUserRepository = searchUserRepository;
            _updateUserRepository = updateUserRepository;
        }

        [Route("Create")]
        [HttpPost]
        public IHttpActionResult UserCreation([FromBody]NewUserRequest newUser)
        {
            try
            {
                var user = new UserModel();
                user.UserName = newUser.UserName;
                user.Password = newUser.Password;
                var creationStatus = _userrepository.CreateUser(newUser);

                if (creationStatus == 0)
                {
                    return InternalServerError(new Exception("User creation failed"));
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
           
            }

        }
        
        [HttpPut]
        [Route("UpdateUserName")]
        public IHttpActionResult UpdateUserName(string currentUsername, [FromBody]string newUsername)
        {
           
            
                bool updateUsernameStatus = _userrepository.UpdateUserName(currentUsername, newUsername);

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
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Username updation failed. Username does not exist."));
                    }
                 }
           
        }

        [HttpPut]
        [Route("UpdateUserRole")]
        public IHttpActionResult UpdateUserRole(string currentUsername, string currentRole, string newRole)
        {
            bool updateUserRoleStatus = _userrepository.UpdateUserRole(currentUsername, currentRole, newRole);

            //how do we pass a variable value in the create respone method
            //like <User025> role has been updated successully to <HR>
            if (updateUserRoleStatus)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "User Role updated."));
            }
            else
            {
                if (String.IsNullOrEmpty(currentUsername) || String.IsNullOrEmpty(currentRole) || String.IsNullOrEmpty(newRole))
                {
                    var exceptionMessage = new ArgumentNullException("Entered value cannot be null.");
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, exceptionMessage));
                }

                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "User Role updation failed. Please retry."));
                }
               
            }
        }


        [Route("Delete")]
        [HttpDelete]
        public IHttpActionResult Delete(string userName)
        {
            bool deleteUserStatus = _userrepository.DeleteUser(userName);
           // string Userid = UserId;

            if(deleteUserStatus)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "Requested user " + userName + " has been deleted."));
            }

            else
            {
                if (String.IsNullOrEmpty(userName))
                {
                    var exceptionMessage = new ArgumentNullException(userName," Username cannot be null.");
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, exceptionMessage));
                }
                else

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Requested user " + userName + " is not found."));
            }

        }

        [Route("GetUsersByRole/{role}")]
        [HttpGet]
        public IHttpActionResult GetUsersByRole(string role)
        {
            IEnumerable<string> usersList = _userrepository.GetUsersByRole(role);
            //Object[] username = _irepositoryGetUsersByRole(Role);
            var message = string.Format("List of users with {0} role {1}", role, usersList);

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, message));

        }
    }
}
