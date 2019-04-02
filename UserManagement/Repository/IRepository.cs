using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Repository
{
    public interface IRepository<T> where T : class
    {
        int Create(T t);
        bool Delete(T t);
       

    }

    public interface IUpdateUsersRepository
    {
        bool UpdateUserName(string currentUsername, string newUsername);
        bool UpdateUserRole(string currentUsername, string currentRole, string newRole);
    }

    public interface IFindUsersRepository
    {
        IEnumerable<string> GetUsersByRole(string role);
        IEnumerable<UserModel> GetUsersBySearchKeyword(string searchKeyword);
    }

    //public interface IRepository<T1,T2,T3,T4> where T1:class
    //                                          where T2:class
    //{
    //    int CreateUser(string username, string password, string role);

    //    int LoginUser(string username, string password);

    //    bool UpdateUserName(string currentUsername, string newUsername);
    //    bool UpdateUserRole(string currentUsername, string currentRole, string newRole);

    //    IEnumerable<string> GetUsersByRole(string role);

    //    IEnumerable<UserModel> GetUsersBySearchKeyword(string searchKeyword);

    //    bool DeleteUser(string userName);
    //}
}
