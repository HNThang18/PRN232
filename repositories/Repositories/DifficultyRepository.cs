using Microsoft.EntityFrameworkCore;
using repositories.Basic;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace repositories.Repositories
{
    public class DifficultyRepository : GenericRepository<Difficulty>, IDifficultyRepository
    {
        //public DifficultyRepository() : base() { }

        public DifficultyRepository(MathLpContext context) : base(context) { }

        public async Task<IEnumerable<Difficulty>> GetAllDifficultiesAsync()
        {
            return await _context.difficulties.ToListAsync();
        }

        public async Task<Difficulty?> GetDifficultyByIdAsync(int id)
        {
            return await _context.difficulties.FindAsync(id);
        }

        public async Task AddDifficultyAsync(Difficulty difficulty)
        {
            await _context.difficulties.AddAsync(difficulty);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDifficultyAsync(Difficulty difficulty)
        {
            _context.Entry(difficulty).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDifficultyAsync(int id)
        {
            var entity = await _context.difficulties.FindAsync(id);
            if (entity != null)
            {
                _context.difficulties.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
