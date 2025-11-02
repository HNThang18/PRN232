using repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
