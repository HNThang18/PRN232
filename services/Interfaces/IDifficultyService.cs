using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services.Interfaces
{
    public interface IDifficultyService
    {
        Task<IEnumerable<Difficulty>> GetAllDifficultiesAsync();
        Task<Difficulty?> GetDifficultyByIdAsync(int id);
        Task AddDifficultyAsync(Difficulty difficulty);
        Task UpdateDifficultyAsync(Difficulty difficulty);
        Task DeleteDifficultyAsync(int id);
    }
}
