using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.Interfaces
{
    public interface ICurrentUserService
    {
        int? GetUserId();
        string? GetIpAddress();
    }
}
