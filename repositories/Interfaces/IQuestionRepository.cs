using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task AddQuestionAsync(Question question);
    }
}