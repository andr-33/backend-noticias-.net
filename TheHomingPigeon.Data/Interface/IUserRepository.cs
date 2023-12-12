using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheHomingPigeon.Model;

namespace TheHomingPigeon.Data.Interface
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserResponse>> GetActiveUsers();

        Task<bool> InsertUser(User user);

        Task<bool> ValidateExistUser(string username, string email);
    }
}
