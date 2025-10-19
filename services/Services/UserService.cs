using applications.DTOs.Request;
using applications.DTOs.Response;
using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.IsActive = false; //

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersWithDetailsAsync();
            return users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                LevelId = u.LevelId,
                LevelName = u.Level?.LevelName, 
                GradeLevel = u.GradeLevel,
                Credit = u.Credit,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var u = await _userRepository.GetUserByIdWithDetailsAsync(id);
            if (u == null) return null;

            // Map từ Model sang DTO
            return new UserResponseDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                LevelId = u.LevelId,
                LevelName = u.Level?.LevelName, //
                GradeLevel = u.GradeLevel,
                Credit = u.Credit,
                CreatedAt = u.CreatedAt
            };
        }

        public async Task<User> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Kiểm tra tồn tại
            var existingUser = await _userRepository.GetUserByUsernameOrEmailAsync(request.Username, request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username hoặc Email đã tồn tại.");
            }

            // 2. Băm mật khẩu (Sử dụng BCrypt.Net-Next)
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Tạo User
            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = hashedPassword,
                Role = (UserRole)Enum.Parse(typeof(UserRole), request.Role),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,

                LevelId = request.LevelId,
                GradeLevel = request.GradeLevel
            };

            // 4. Thêm vào DB
            await _userRepository.CreateAsync(newUser);
            return newUser;
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            if (request.Email != null) user.Email = request.Email;
            if (request.Role != null) user.Role = (UserRole)Enum.Parse(typeof(UserRole), request.Role);
            if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;
            if (request.LevelId.HasValue) user.LevelId = request.LevelId.Value;
            if (request.GradeLevel != null) user.GradeLevel = request.GradeLevel;
            if (request.Credit.HasValue) user.Credit = request.Credit.Value;

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword); //
            }

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            // 1. Tìm người dùng
            var user = await _userRepository.GetUserByUsernameAsync(username);

            // 2. Kiểm tra
            if (user == null || !user.IsActive)
            {
                return null;
            }

            // 3. Xác thực mật khẩu
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            return isPasswordValid ? user : null;
        }
    }
    }

