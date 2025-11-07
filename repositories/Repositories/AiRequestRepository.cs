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

        public async Task<IEnumerable<AiRequest>> GetByRequestTypeAsync(RequestType requestType)
        {
            return await _context.AiRequests
                .Include(a => a.User)
                .Include(a => a.Level)
                .Where(a => a.RequestType == requestType)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AiRequest>> GetRequestHistoryAsync(
            int? userId = null,
            RequestType? requestType = null,
            AiRequestStatus? status = null,
            string? search = null,
            int page = 1,
            int limit = 20)
        {
            var query = _context.AiRequests
                .Include(a => a.User)
                .Include(a => a.Level)
                .Include(a => a.LessonPlans)
                .Include(a => a.Questions)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(a => a.UserId == userId.Value);
            }

            if (requestType.HasValue)
            {
                query = query.Where(a => a.RequestType == requestType.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.Prompt.Contains(search) || (a.Response != null && a.Response.Contains(search)));
            }

            return await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(
            int? userId = null,
            RequestType? requestType = null,
            AiRequestStatus? status = null,
            string? search = null)
        {
            var query = _context.AiRequests.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(a => a.UserId == userId.Value);
            }

            if (requestType.HasValue)
            {
                query = query.Where(a => a.RequestType == requestType.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.Prompt.Contains(search) || (a.Response != null && a.Response.Contains(search)));
            }

            return await query.CountAsync();
        }
    }
}
