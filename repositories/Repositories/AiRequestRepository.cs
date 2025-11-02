using Microsoft.EntityFrameworkCore;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;

namespace repositories.Repositories
{
    public class AiRequestRepository : IAiRequestRepository
    {
        private readonly MathLpContext _context;

        public AiRequestRepository(MathLpContext context)
        {
            _context = context;
        }

        public async Task<AiRequest> AddAsync(AiRequest aiRequest)
        {
            await _context.AiRequests.AddAsync(aiRequest);
            await _context.SaveChangesAsync();
            return aiRequest;
        }

        public async Task<AiRequest?> GetByIdAsync(int id)
        {
            return await _context.AiRequests
                .Include(a => a.User)
                .Include(a => a.Level)
                .FirstOrDefaultAsync(a => a.AIRequestId == id);
        }

        public async Task UpdateAsync(AiRequest aiRequest)
        {
            _context.AiRequests.Update(aiRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AiRequest>> GetByUserIdAsync(int userId)
        {
            return await _context.AiRequests
                .Include(a => a.User)
                .Include(a => a.Level)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AiRequest>> GetByStatusAsync(AiRequestStatus status)
        {
            return await _context.AiRequests
                .Include(a => a.User)
                .Include(a => a.Level)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}
