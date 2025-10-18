using repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using applications.DTOs;

namespace services.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterRequestDto request);
        Task<User?> ValidateUserAsync(string username, string password);

        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(int id, UpdateUserRequestDto request);
        Task<bool> DeleteUserAsync(int id);
    }
}
