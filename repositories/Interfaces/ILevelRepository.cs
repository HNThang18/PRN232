using repositories.Models;

namespace repositories.Interfaces
{
    public interface ILevelRepository
    {
        Task<IEnumerable<Level>> GetAllAsync();
        Task<Level?> GetByIdAsync(int id);
    }
}
