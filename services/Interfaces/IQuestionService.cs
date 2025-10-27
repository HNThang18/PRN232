using repositories.Models;

namespace services.Interfaces
{
    public interface IQuestionService
    {
        Task AddQuestionAsync(Question question);
    }
}