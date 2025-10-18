using repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using repositories.Basic; 

namespace repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByUsernameOrEmailAsync(string username, string email);

        Task<User?> GetUserByIdWithDetailsAsync(int id);
        Task<IEnumerable<User>> GetAllUsersWithDetailsAsync();
    }
}
