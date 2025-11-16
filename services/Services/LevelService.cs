using applications.DTOs.Level;
using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;

namespace services.Services
{
    public class LevelService : ILevelService
    {
        private readonly ILevelRepository _levelRepository;

        public LevelService(ILevelRepository levelRepository)
        {
            _levelRepository = levelRepository;
        }

        public async Task<IEnumerable<LevelResponseDto>> GetAllLevelsAsync()
        {
            var levels = await _levelRepository.GetAllAsync();
            return levels.Select(l => new LevelResponseDto
            {
                LevelId = l.LevelId,
                LevelName = l.LevelName,
                EducationLevel = l.EducationLevel.ToString(),
                Order = l.Order
            });
        }

        public async Task<LevelResponseDto?> GetLevelByIdAsync(int id)
        {
            var level = await _levelRepository.GetByIdAsync(id);
            if (level == null) return null;

            return new LevelResponseDto
            {
                LevelId = level.LevelId,
                LevelName = level.LevelName,
                EducationLevel = level.EducationLevel.ToString(),
                Order = level.Order
            };
        }
    }
}
