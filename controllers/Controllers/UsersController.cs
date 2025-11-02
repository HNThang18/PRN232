using applications.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using services.Interfaces;
// using System.Net; // <-- XÓA DÒNG NÀY ĐI
using System.Threading.Tasks;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// (Admin) Lấy danh sách tất cả người dùng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// (Admin) Lấy thông tin chi tiết của 1 người dùng
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không tìm thấy người dùng với ID: {id}");
            }
            return Ok(user);
        }

        /// <summary>
        /// (Admin) Cập nhật thông tin của 1 người dùng
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequestDto request)
        {
            var result = await _userService.UpdateUserAsync(id, request);
            if (!result)
            {
                return NotFound($"Không tìm thấy người dùng với ID: {id}");
            }

            // Tự động ghi Audit Log (Update)
            return Ok(new { message = "Cập nhật người dùng thành công." });
        }

        /// <summary>
        /// (Admin) Xóa mềm 1 người dùng (vô hiệu hóa tài khoản)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound($"Không tìm thấy người dùng với ID: {id}");
            }

            // Tự động ghi Audit Log (Update: IsActive = false)
            return Ok(new { message = "Vô hiệu hóa người dùng thành công." });
        }
    }
}