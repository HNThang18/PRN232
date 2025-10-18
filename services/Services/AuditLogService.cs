using repositories.Dbcontext;
using repositories.Models;
using services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using applications.Interfaces;

namespace services.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly MathLpContext _context;
        private readonly ICurrentUserService _currentUserService;

        public AuditLogService(MathLpContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task LogManualAsync(int? userId, LogAction action, string entityName, int? entityId, string details)
        {
            var logEntry = new AuditLog
            {
                UserId = userId ?? _currentUserService.GetUserId(),
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                Timestamp = DateTime.UtcNow,
                IpAddress = _currentUserService.GetIpAddress()
            };

            await _context.auditLogs.AddAsync(logEntry);
            await _context.SaveChangesAsync();
        }
    }
}
