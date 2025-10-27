using repositories.Models;

namespace repositories.Interfaces
{
    public interface IAiRequestRepository
    {
        Task<AiRequest> AddAsync(AiRequest aiRequest);
        Task<AiRequest?> GetByIdAsync(int id);
        Task UpdateAsync(AiRequest aiRequest);
        Task<IEnumerable<AiRequest>> GetByUserIdAsync(int userId);
        Task<IEnumerable<AiRequest>> GetByStatusAsync(AiRequestStatus status);
    }
}
