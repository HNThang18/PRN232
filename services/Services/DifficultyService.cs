using repositories.Models;
using repositories.Interfaces;
using services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services.Services
{
    public class DifficultyService : IDifficultyService
    {
        private readonly IDifficultyRepository _difficultyRepository;

        public DifficultyService(IDifficultyRepository difficultyRepository)
        {
            _difficultyRepository = difficultyRepository;
        }

        public async Task<IEnumerable<Difficulty>> GetAllDifficultiesAsync()
        {
            return await _difficultyRepository.GetAllDifficultiesAsync();
        }

        public async Task<Difficulty?> GetDifficultyByIdAsync(int id)
        {
            return await _difficultyRepository.GetDifficultyByIdAsync(id);
        }

        public async Task AddDifficultyAsync(Difficulty difficulty)
        {
            await _difficultyRepository.AddDifficultyAsync(difficulty);
        }

        public async Task UpdateDifficultyAsync(Difficulty difficulty)
        {
            await _difficultyRepository.UpdateDifficultyAsync(difficulty);
        }

        public async Task DeleteDifficultyAsync(int id)
        {
            await _difficultyRepository.DeleteDifficultyAsync(id);
        }
    }
}
