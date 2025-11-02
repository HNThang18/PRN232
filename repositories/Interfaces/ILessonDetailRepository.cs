using repositories.Basic;
using repositories.Models;

namespace repositories.Interfaces
{
    public interface ILessonDetailRepository : IGenericRepository<LessonDetail>
    {
        // Basic CRUD
        Task<IEnumerable<LessonDetail>> GetAllLessonDetailsAsync(CancellationToken cancellationToken = default);
        Task<LessonDetail?> GetLessonDetailByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<LessonDetail?> GetLessonDetailWithAttachmentsAsync(int id, CancellationToken cancellationToken = default);
        Task AddLessonDetailAsync(LessonDetail lessonDetail, CancellationToken cancellationToken = default);
        Task UpdateLessonDetailAsync(LessonDetail lessonDetail, CancellationToken cancellationToken = default);
        Task DeleteLessonDetailAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> LessonDetailExistsAsync(int id, CancellationToken cancellationToken = default);

        // Query methods
        Task<IEnumerable<LessonDetail>> GetLessonDetailsByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);
        Task<IEnumerable<LessonDetail>> GetLessonDetailsByContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default);
        Task<int> CountLessonDetailsByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);
        Task<int> GetMaxOrderByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);
        Task<bool> IsOrderExistsAsync(int lessonId, int order, int? excludeLessonDetailId = null, CancellationToken cancellationToken = default);

        // Business methods
        Task<bool> HasAttachmentsAsync(int lessonDetailId, CancellationToken cancellationToken = default);

        // Bulk operations
        Task UpdateRangeAsync(IEnumerable<LessonDetail> lessonDetails, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(IEnumerable<LessonDetail> lessonDetails, CancellationToken cancellationToken = default);
    }
}
