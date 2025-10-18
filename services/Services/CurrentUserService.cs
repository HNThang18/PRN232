using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using services.Interfaces;
using applications.Interfaces;

namespace services.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string? GetIpAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetUserId()
        {
            // Lấy claim "sub" (SubjectId) từ token, đây là UserId
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            // Trả về null nếu không có user nào đăng nhập (ví dụ: một tiến trình nền)
            return null;
        }
    }
}
