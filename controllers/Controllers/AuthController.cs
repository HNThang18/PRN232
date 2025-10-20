using Microsoft.AspNetCore.Mvc;
using services.Interfaces;
using repositories.Models;
using applications.DTOs.Request;
using applications.DTOs.Response;

namespace controllers.Controllers
{
   
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
            private readonly IUserService _userService;
            private readonly IJwtService _jwtService;
            private readonly IAuditLogService _auditLogService;


        public AuthController(IUserService userService, IJwtService jwtService, IAuditLogService auditLogService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _auditLogService = auditLogService; // <-- Khởi tạo
        }

        [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
            {
            try
            {
                var newUser = await _userService.RegisterAsync(request);

                var result = new
                {
                    UserId = newUser.UserId, //
                    Username = newUser.Username, //
                    Email = newUser.Email, //

                    Role = newUser.Role.ToString(), //

                    LevelId = newUser.LevelId, //
                    GradeLevel = newUser.GradeLevel, //
                    IsActive = newUser.IsActive, //
                    CreatedAt = newUser.CreatedAt //
                };

                return CreatedAtAction(nameof(Register), new { id = newUser.UserId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
            {
            var user = await _userService.ValidateUserAsync(request.Username, request.Password);

            if (user == null)
            {
                // Ghi log login thất bại
                await _auditLogService.LogManualAsync(
                    userId: null,
                    action: LogAction.Login, //
                    entityName: "User",
                    entityId: null,
                    details: $"Đăng nhập thất bại cho username: {request.Username}");

                return Unauthorized("Tên đăng nhập hoặc mật khẩu không hợp lệ.");
            }
            await _auditLogService.LogManualAsync(
                userId: user.UserId, //
                action: LogAction.Login,
                entityName: "User",
                entityId: user.UserId, //
                details: "Đăng nhập thành công.");

            // Tạo JWT
            string token = _jwtService.GenerateToken(user);


                // TODO: Ghi log login thành công (phần AuditLog)

                // Trả về DTO
                var response = new AuthResponseDto
                {
                    Token = token,
                    UserId = user.UserId,
                    Username = user.Username,
                    Role = (user.Role).ToString()
                };

                return Ok(response);
            }
        }
    }

