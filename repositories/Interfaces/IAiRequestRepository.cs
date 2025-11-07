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
        Task<IEnumerable<AiRequest>> GetByRequestTypeAsync(RequestType requestType);
        Task<IEnumerable<AiRequest>> GetRequestHistoryAsync(
            int? userId = null,
            RequestType? requestType = null,
            AiRequestStatus? status = null,
            string? search = null,
            int page = 1,
            int limit = 20);
        Task<int> GetTotalCountAsync(
            int? userId = null,
            RequestType? requestType = null,
            AiRequestStatus? status = null,
            string? search = null);
    }
}
