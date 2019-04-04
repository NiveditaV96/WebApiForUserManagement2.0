using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using UserManagement.Model;

namespace UserManagement.Repository
{

    public class LoginRepository : ILoginRepository<User>
    {

        string connectionString = ConfigurationManager.ConnectionStrings["SqlConString"].ConnectionString;

        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("Login");
        /// <summary>
        /// Method for User login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int Login(User user)
        {
            var username = user.Username;
            var password = user.Password;

            string userIdCountQuery = "select count(m.[UserId]) from[dbo].[aspnet_Users] u inner join[dbo].[aspnet_Membership] m " +
                                     "on u.UserId = m.UserId " +
                                      " where m.[Password] = '" + password + "' and u.[UserName] = '" + username + "' " +
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
    }



   
}