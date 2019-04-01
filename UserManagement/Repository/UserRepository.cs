﻿using System;
using System.Collections.Generic;
using UserManagement.Models;
using System.Configuration;
using System.Data.SqlClient;
using UserManagement.Model;

namespace UserManagement.Repository
{
    enum userCreateStatus { UserCreationFailed, PasswordCreationFailed, RoleCreationFailed, Sucessful,Failure };
    public class UserRepository : IUserRepository
        //<UserModel, MembershipModel, RoleModel, UserInRoleModel>
    { 
        string connectionString = ConfigurationManager.ConnectionStrings["SqlConString"].ConnectionString;

        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("UserManagementLogger");


        /// <summary>
        ///  Method to create user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public int CreateUser(string username, string password, string role)
        {
           
            
            string insertUserName = "Insert Into aspnet_Users ([ApplicationId], [UserId], [UserName], [LoweredUserName], [LastActivityDate] )" +
                              "values((select[ApplicationId] from aspnet_Applications where ApplicationName = 'UserApplication'), NEWID(), '"
                              + username + "', LOWER('" + username + "'), GETDATE())";

            string insertPassword = "Insert Into aspnet_Membership ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [IsApproved], [IsLockedOut], [CreateDate]," +
                              " [LastLoginDate],[LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart],[FailedPasswordAnswerAttemptCount], " +
                              "[FailedPasswordAnswerAttemptWindowStart]) values ((select[ApplicationId] from aspnet_Users where UserName = '" + username + "'), " +
                              "(select[UserId] from aspnet_Users where UserName = '" + username + "'), '" + password + "', 0, "+
                              "'NA', 0, 0, GETDATE(), GETDATE(), GETDATE(), GETDATE(), 0, GETDATE(), 0, GETDATE())";

            
            string insertRole = "Insert into [dbo].[aspnet_UsersInRoles] (UserId, RoleId) select u.UserId,r.RoleId from[aspnet_Users] u inner join[aspnet_Roles] r on u.ApplicationId = r.ApplicationId " +
                                "inner join [dbo].[aspnet_Applications] a on r.ApplicationId=a.ApplicationId where u.[UserName] = '" + username + "' and a.[ApplicationName] = 'UserApplication' " +
                                "and r.RoleName= '"+role+"'";




            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmdCreateUser = new SqlCommand(insertUserName, con))
                    {
                        con.Open();
                        int userRowsAffected = cmdCreateUser.ExecuteNonQuery();


                        if (userRowsAffected == 0)
                        {
                            _log.Info("User table not affected");
                            return (int)userCreateStatus.UserCreationFailed;

                          
                        }

                        else if(userRowsAffected!=0)
                        { 
                            cmdCreateUser.CommandText = insertPassword;
                            int membesrshipRowsAffected = cmdCreateUser.ExecuteNonQuery();

                            if (membesrshipRowsAffected == 0)
                            {
                                _log.Info("Membership table not affected");
                                return (int)userCreateStatus.PasswordCreationFailed;
                            }

                            else
                            {
                                cmdCreateUser.CommandText = insertRole;
                                int roleRowsAffected = cmdCreateUser.ExecuteNonQuery();

                                if (roleRowsAffected == 0)
                                {
                                    _log.Info("Roles table not affected");
                                    return (int)userCreateStatus.RoleCreationFailed;
                                }

                                
                            }

                        }

                        //cmdForUsername.ExecuteReader();

                    }
                }

                
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                // throw new SqlException("");
                   //pass message to this exception??
            }
            catch (Exception)
            {
                throw;

            }

            //System.Data.SqlClient.SqlException:
            //'Cannot insert duplicate key row in object 'dbo.aspnet_Users' with unique index
            return (int)userCreateStatus.Sucessful;   //    'aspnet_Users_Index'. The duplicate key value is (f5449681-64c3-4b20-9a94-5b862eeeee6e, nivedita v).
        }


        

        
        /// <summary>
        /// Method for User login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int LoginUser(string username, string password)
        {
            string userIdCountQuery = "select count(m.[UserId]) from[dbo].[aspnet_Users] u inner join[dbo].[aspnet_Membership] m "+
                                     "on u.UserId = m.UserId " +
                                      " where m.[Password] = '"+ password + "' and u.[UserName] = '"+ username +"' "+
                                  " and u.Applicationid = (select ApplicationId from aspnet_Applications where [ApplicationName] = 'UserApplication')";


            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    using (SqlCommand cmd1 = new SqlCommand(userIdCountQuery, con))
                    //SqlCommand cmd2 = new SqlCommand(passwordQuery, con);
                    {
                        con.Open();

                        int userIdRowCount = (int)cmd1.ExecuteScalar();
                        //int pwdRowCount = (int)cmd2.ExecuteScalar();

                        //valid user credentials
                        if (userIdRowCount == 1)
                        {

                            return 1;
                        }

                        //invalid credentials
                        else
                        {
                            return 0;
                        }
                    }

                   
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }

            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

     /// <summary>
     /// Updates current Username with new username
     /// </summary>
     /// <param name="currentUsername"></param>
     /// <param name="newUsername"></param>
     /// <returns></returns>
    public bool UpdateUserName(string currentUsername, string newUsername)
    {

            UserModel user = new UserModel();
            user.Username = newUsername;

            string updateUserNameQuery = "update[dbo].[aspnet_Users]" +
                                         "set [UserName] = '" + user.Username + "', [LoweredUserName] = lower('" + user.Username + "')" +
                                         "where [ApplicationId] = (select [ApplicationId] from [dbo].[aspnet_Applications] " +
                                         "where [ApplicationName]='UserApplication')" +
                                         "and [UserName] = '" + currentUsername + "'";


            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmdUpdaterUsername = new SqlCommand(updateUserNameQuery, con))
                    {
                        con.Open();
                        int rowUpdateCount = cmdUpdaterUsername.ExecuteNonQuery();
                        if (rowUpdateCount != 0)
                        {
                            _log.Info("Received request for Updating Username");
                            return true;
                        }

                        else
                            return false;
                    }
                }
            }
            //catch (SqlException ex)
            //{
            //    ex.ToString();
            //}

            catch (Exception e)
            {
                throw e;
            }    
    }


    /// <summary>
    /// Updates current User role with new user role
    /// </summary>
    /// <param name="currentUsername"></param>
    /// <param name="currentRole"></param>
    /// <param name="newRole"></param>
    /// <returns></returns>
    public bool UpdateUserRole(string currentUsername, string currentRole, string newRole)
    {
            string updateUserRoleQuery = "update [dbo].[aspnet_UsersInRoles]" +
                                         "set [RoleId] = (select [RoleId] from [dbo].[aspnet_Roles] where [RoleName] = '" + newRole + "'" +
                                         " and [ApplicationId] = (select [ApplicationId] from [dbo].[aspnet_Applications] where [ApplicationName] = 'UserApplication'))" +
                                         "where [RoleId] = (select [RoleId] from [dbo].[aspnet_Roles] where [RoleName] = '" + currentRole + "' " +
                                         "and [ApplicationId] = (select [ApplicationId] from [dbo].[aspnet_Applications] where [ApplicationName] = 'UserApplication'))" +
                                         "and [UserId] = (select [UserId] from [dbo].[aspnet_Users] where [UserName] = '" + currentUsername + "')";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmdUpdaterUserRole = new SqlCommand(updateUserRoleQuery, con))
                    {
                        con.Open();
                        int rowUpdateCount = cmdUpdaterUserRole.ExecuteNonQuery();
                        if (rowUpdateCount != 0)
                        {
                            _log.Info("Requested for user role updation.");
                            return true;
                        }
                        else
                            return false;
                    }
                }
            }

            catch (Exception e)
            {
                throw e;
            }


            
    }
     
    /// <summary>
    /// Deletes a user record
    /// </summary>
    /// <param name="userName"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public bool DeleteUser(string userName)
    {
            UserModel user = new UserModel();

            Guid userIdGuid;

            string userIdQuery = "select u.UserId from[dbo].[aspnet_Users] u inner join[dbo].[aspnet_Membership] m on u.UserId = m.UserId "+
                                    "inner join[dbo].[aspnet_Applications] a on u.ApplicationId = a.ApplicationId "+
                                    "where u.UserName='"+ userName + "' and "+
                                    "a.ApplicationId = (select ApplicationId from [dbo].[aspnet_Applications] where ApplicationName = 'UserApplication')";

            //string deleteUserFromUsersInRolesQuery = "delete from aspnet_UsersInRoles where UserId = '" + user.UserID + "'";
            //string deleteUserFromMembershipQuery = "delete from aspnet_Membership where UserId = '" + user.UserID + "'";
            //string deleteUserFromUsersQuery = "delete from aspnet_Users where UserId = '" + user.UserID + "'";

            try
            {

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmdDeleteUser = new SqlCommand(userIdQuery, con))
                    {
                        con.Open();
                        SqlDataReader dr = cmdDeleteUser.ExecuteReader();


                        while (dr.Read())
                        {
                            // user.UserID = dr["UserId"].ToString();

                            //converting string to guid
                            userIdGuid = Guid.Parse(dr["UserId"].ToString());

                            //storing converted guid value in userid properrty which is of guid type
                            user.UserID = userIdGuid;

                        }
                        dr.Close();

                        string deleteUserFromUsersInRolesQuery = "delete from aspnet_UsersInRoles where UserId = '" + user.UserID + "'";
                        string deleteUserFromMembershipQuery = "delete from aspnet_Membership where UserId = '" + user.UserID + "'";
                        string deleteUserFromUsersQuery = "delete from aspnet_Users where UserId = '" + user.UserID + "'";

                        cmdDeleteUser.CommandText = deleteUserFromUsersInRolesQuery;
                        cmdDeleteUser.ExecuteNonQuery();

                        cmdDeleteUser.CommandText = deleteUserFromUsersInRolesQuery;
                        cmdDeleteUser.ExecuteNonQuery();

                        cmdDeleteUser.CommandText = deleteUserFromUsersInRolesQuery;
                        cmdDeleteUser.ExecuteNonQuery();

                        return true;

                    }
                }
            }


            catch (Exception e)
            {
                throw e;
            }

    }

   

        /// <summary>
        /// Get users with a particular role corresponding to a particular application name -- check
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
    public IEnumerable<string> GetUsersByRole(string role)
    {
            string getUsersByRoleQuery = "select u.UserName from dbo.aspnet_Users u inner join dbo.aspnet_UsersInRoles ur "+
                                         "on u.UserId = ur.UserId inner join[dbo].[aspnet_Applications] a "+
                                       "on u.ApplicationId = a.ApplicationId "+
                                       "where  u.ApplicationId = (select ApplicationId from aspnet_Applications where[ApplicationName] = 'UserApplication') "+
                                         "and ur.RoleId in (select[RoleId] from [dbo].[aspnet_Roles] where[RoleName] = '"+ role +"') "+
                                         "order by u.UserName";

            List<string> usernameList = new List<string>();

        

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmdUsers = new SqlCommand(getUsersByRoleQuery, con);
                    con.Open();
                    SqlDataReader dr = cmdUsers.ExecuteReader();

                    //Object[] username = new Object[dr.FieldCount];

                    while (dr.Read())
                    {
                        string username = dr["UserName"].ToString();
                        usernameList.Add(username);
                        //int count = dr.GetValues(username);
                    }
                    _log.Info("user list based on role to be displayed.");
                    return usernameList;
                }
            }


             catch (Exception e)
            {
                throw e;
            }

            
        }

    public IEnumerable<UserModel> GetUsersBySearchKeyword(string searchKeyword)
    {
        throw new NotImplementedException();
    }

      
   }
}