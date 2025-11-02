using repositories.Models;

namespace services.Interfaces
{
    public interface ILessonDetailService
    {
        // Basic CRUD
        Task<IEnumerable<LessonDetail>> GetAllLessonDetailsAsync(CancellationToken cancellationToken = default);
        Task<LessonDetail?> GetLessonDetailByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<LessonDetail?> GetLessonDetailWithAttachmentsAsync(int id, CancellationToken cancellationToken = default);
        Task<LessonDetail> AddLessonDetailAsync(LessonDetail lessonDetail, CancellationToken cancellationToken = default);
        Task UpdateLessonDetailAsync(LessonDetail lessonDetail, CancellationToken cancellationToken = default);
        Task DeleteLessonDetailAsync(int id, CancellationToken cancellationToken = default);

        // Query methods
        Task<IEnumerable<LessonDetail>> GetLessonDetailsByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);
        Task<IEnumerable<LessonDetail>> GetLessonDetailsByContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default);
        Task<int> CountLessonDetailsByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);

        // Business methods
        Task ReorderLessonDetailsAsync(int lessonId, Dictionary<int, int> newOrders, CancellationToken cancellationToken = default);
        Task<bool> CanDeleteLessonDetailAsync(int lessonDetailId, CancellationToken cancellationToken = default);
        Task DeleteLessonDetailsByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);
        Task<LessonDetail> DuplicateLessonDetailAsync(int lessonDetailId, int? targetLessonId = null, CancellationToken cancellationToken = default);
    }
}
