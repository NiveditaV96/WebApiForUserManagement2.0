using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Requests;

namespace UserManagement.Repository
{
    public interface ISearchUsers
    {
        IEnumerable<string> GetUsersByRole(string role);

        IEnumerable<UserModel> GetUsersBySearchKeyword(string searchKeyword);
    }

    public interface IUpdateUsers
    {
        bool UpdateUserRole(string currentUsername, string currentRole, string newRole);
    }

    public interface IRepository<T1> where T1 : class
    {
        int Create(T1 t);
        
        bool Update(T1 t);

        IEnumerable<T1> GetAll();

        bool Delete(int id);
    }
}
