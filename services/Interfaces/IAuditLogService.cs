using repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using repositories.Models;

namespace services.Interfaces
{
    public interface IAuditLogService
    {
        Task LogManualAsync(int? userId, LogAction action, string entityName, int? entityId, string details);
    }
}
