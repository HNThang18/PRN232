using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace repositories.Interfaces
{
    public interface IDifficultyRepository
    {
        Task<IEnumerable<Difficulty>> GetAllDifficultiesAsync();
        Task<Difficulty?> GetDifficultyByIdAsync(int id);
        Task AddDifficultyAsync(Difficulty difficulty);
        Task UpdateDifficultyAsync(Difficulty difficulty);
        Task DeleteDifficultyAsync(int id);
    }
}
