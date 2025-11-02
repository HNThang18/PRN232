using repositories.Basic;
using repositories.Interfaces;
using repositories.Models;
using repositories.Dbcontext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace repositories.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MathLpContext context) : base(context)
        {
        }

        public async Task<IEnumerable<User>> GetAllUsersWithDetailsAsync()
        {
            // Dùng Include để lấy thông tin Level
            return await _context.users
                .Include(u => u.Level) //
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdWithDetailsAsync(int id)
        {
            return await _context.users
                .Include(u => u.Level) //
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByUsernameOrEmailAsync(string username, string email)
        {
            return await _context.users
                 .FirstOrDefaultAsync(u => u.Username == username || u.Email == email);
        }
    }
}
