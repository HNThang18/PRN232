using applications.DTOs.Level;
using repositories.Models;

namespace services.Interfaces
{
    public interface ILevelService
    {
        Task<IEnumerable<LevelResponseDto>> GetAllLevelsAsync();
        Task<LevelResponseDto?> GetLevelByIdAsync(int id);
    }
}
