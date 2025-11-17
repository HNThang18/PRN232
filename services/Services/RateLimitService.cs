using Microsoft.Extensions.Configuration;
using repositories.Interfaces;
using repositories.Models;

namespace services.Services
{
    public interface IRateLimitService
    {
        Task<bool> CanMakeRequestAsync(int userId, RequestType requestType);
        Task RecordRequestAsync(int userId, RequestType requestType);
    }

    public class RateLimitService : IRateLimitService
    {
        private readonly IAiRequestRepository _aiRequestRepository;
        private readonly IConfiguration _configuration;
        private const int DEFAULT_USER_DAILY_LIMIT = 50;
        private const int DEFAULT_GLOBAL_DAILY_LIMIT = 1000;

        public RateLimitService(
            IAiRequestRepository aiRequestRepository,
            IConfiguration configuration)
        {
            _aiRequestRepository = aiRequestRepository;
            _configuration = configuration;
        }

        public async Task<bool> CanMakeRequestAsync(int userId, RequestType requestType)
        {
            var userDailyLimit = _configuration.GetValue<int>("RateLimit:UserDailyLimit", DEFAULT_USER_DAILY_LIMIT);
            var globalDailyLimit = _configuration.GetValue<int>("RateLimit:GlobalDailyLimit", DEFAULT_GLOBAL_DAILY_LIMIT);

            var startOfDay = DateTime.UtcNow.Date;
            var endOfDay = startOfDay.AddDays(1);

            var userRequestCount = await _aiRequestRepository.GetRequestCountByUserIdAsync(userId, startOfDay, endOfDay);
            if (userRequestCount >= userDailyLimit)
            {
                return false;
            }

            var globalRequestCount = await _aiRequestRepository.GetRequestCountByTypeAsync(requestType, startOfDay, endOfDay);
            if (globalRequestCount >= globalDailyLimit)
            {
                return false;
            }

            return true;
        }

        public async Task RecordRequestAsync(int userId, RequestType requestType)
        {
            await Task.CompletedTask;
        }
    }
}
