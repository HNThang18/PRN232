using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);

        Task<int> CreateAsync(T entity);

        Task<int> UpdateAsync(T entity);
        Task<bool> RemoveAsync(T entity);

    }
}
