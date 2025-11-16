using Microsoft.EntityFrameworkCore;
using repositories.Basic;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;

namespace repositories.Repositories
{
    public class LevelRepository : GenericRepository<Level>, ILevelRepository
    {
        public LevelRepository(MathLpContext context) : base(context) { }

        public async Task<IEnumerable<Level>> GetAllAsync()
        {
            return await _context.levels
                .OrderBy(l => l.Order)
                .ToListAsync();
        }

        public async Task<Level?> GetByIdAsync(int id)
        {
            return await _context.levels
                .FirstOrDefaultAsync(l => l.LevelId == id);
        }
    }
}
