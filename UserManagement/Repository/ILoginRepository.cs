using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Repository
{
    public interface ILoginRepository<T> where T : class
    {
        int Login(T t);
    }
}
