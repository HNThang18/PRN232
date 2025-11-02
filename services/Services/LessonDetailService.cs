using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;

namespace services.Services
{
    public class LessonDetailService : ILessonDetailService
    {
        private readonly ILessonDetailRepository _lessonDetailRepository;
        private readonly ILessonRepository _lessonRepository;

        public LessonDetailService(
            ILessonDetailRepository lessonDetailRepository,
            ILessonRepository lessonRepository)
        {
            _lessonDetailRepository = lessonDetailRepository;
            _lessonRepository = lessonRepository;
        }

        // ===== BASIC CRUD =====

        public async Task<IEnumerable<LessonDetail>> GetAllLessonDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _lessonDetailRepository.GetAllLessonDetailsAsync(cancellationToken);
        }

        public async Task<LessonDetail?> GetLessonDetailByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("LessonDetailId phải lớn hơn 0", nameof(id));

            return await _lessonDetailRepository.GetLessonDetailByIdAsync(id, cancellationToken);
        }

        public async Task<LessonDetail?> GetLessonDetailWithAttachmentsAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("LessonDetailId phải lớn hơn 0", nameof(id));

            return await _lessonDetailRepository.GetLessonDetailWithAttachmentsAsync(id, cancellationToken);
        }

        public async Task<LessonDetail> AddLessonDetailAsync(LessonDetail lessonDetail, CancellationToken cancellationToken = default)
        {
            // ===== VALIDATION =====

            if (lessonDetail == null)
                throw new ArgumentNullException(nameof(lessonDetail));

            if (string.IsNullOrWhiteSpace(lessonDetail.Content))
                throw new ArgumentException("Content không được để trống");

            if (lessonDetail.LessonId <= 0)
                throw new ArgumentException("LessonId không hợp lệ");

            // Check lesson exists
            var lessonExists = await _lessonRepository.LessonExistsAsync(lessonDetail.LessonId);
            if (!lessonExists)
                throw new KeyNotFoundException($"Không tìm thấy Lesson với ID {lessonDetail.LessonId}");

            // ===== AUTO-SET ORDER =====
            // If Order is 0 or not provided, auto-increment to max + 1
            if (lessonDetail.Order <= 0)
            {
                var maxOrder = await _lessonDetailRepository.GetMaxOrderByLessonIdAsync(lessonDetail.LessonId, cancellationToken);
                lessonDetail.Order = maxOrder + 1;
            }
            else
            {
                // Validate Order uniqueness if manually set
                var orderExists = await _lessonDetailRepository.IsOrderExistsAsync(
                    lessonDetail.LessonId,
                    lessonDetail.Order,
                    null,
                    cancellationToken);

                if (orderExists)
                    throw new InvalidOperationException($"Order {lessonDetail.Order} đã tồn tại trong Lesson {lessonDetail.LessonId}");
            }

            // ===== SET TIMESTAMPS =====
            lessonDetail.CreatedAt = DateTime.UtcNow;
            lessonDetail.UpdatedAt = null;

            await _lessonDetailRepository.AddLessonDetailAsync(lessonDetail, cancellationToken);

            return lessonDetail;
        }

        public async Task UpdateLessonDetailAsync(LessonDetail lessonDetail, CancellationToken cancellationToken = default)
        {
            // ===== VALIDATION =====

            if (lessonDetail == null)
                throw new ArgumentNullException(nameof(lessonDetail));

            if (lessonDetail.LessonDetailId <= 0)
                throw new ArgumentException("LessonDetailId không hợp lệ");

            if (string.IsNullOrWhiteSpace(lessonDetail.Content))
                throw new ArgumentException("Content không được để trống");

            // Check exists
            var exists = await _lessonDetailRepository.LessonDetailExistsAsync(lessonDetail.LessonDetailId, cancellationToken);
            if (!exists)
                throw new KeyNotFoundException($"Không tìm thấy LessonDetail với ID {lessonDetail.LessonDetailId}");

            // Validate Order uniqueness if changed
            if (lessonDetail.Order > 0)
            {
                var orderExists = await _lessonDetailRepository.IsOrderExistsAsync(
                    lessonDetail.LessonId,
                    lessonDetail.Order,
                    lessonDetail.LessonDetailId,
                    cancellationToken);

                if (orderExists)
                    throw new InvalidOperationException($"Order {lessonDetail.Order} đã tồn tại trong Lesson {lessonDetail.LessonId}");
            }

            // ===== UPDATE TIMESTAMP =====
            lessonDetail.UpdatedAt = DateTime.UtcNow;

            await _lessonDetailRepository.UpdateLessonDetailAsync(lessonDetail, cancellationToken);
        }

        public async Task DeleteLessonDetailAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("LessonDetailId phải lớn hơn 0", nameof(id));

            var exists = await _lessonDetailRepository.LessonDetailExistsAsync(id, cancellationToken);
            if (!exists)
                throw new KeyNotFoundException($"Không tìm thấy LessonDetail với ID {id}");

            // Note: Attachments will be cascade deleted by EF Core if configured
            await _lessonDetailRepository.DeleteLessonDetailAsync(id, cancellationToken);
        }

        // ===== QUERY METHODS =====

        public async Task<IEnumerable<LessonDetail>> GetLessonDetailsByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default)
        {
            if (lessonId <= 0)
                throw new ArgumentException("LessonId phải lớn hơn 0", nameof(lessonId));

            return await _lessonDetailRepository.GetLessonDetailsByLessonIdAsync(lessonId, cancellationToken);
        }

        public async Task<IEnumerable<LessonDetail>> GetLessonDetailsByContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default)
        {
            return await _lessonDetailRepository.GetLessonDetailsByContentTypeAsync(contentType, cancellationToken);
        }

        public async Task<int> CountLessonDetailsByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default)
        {
            if (lessonId <= 0)
                throw new ArgumentException("LessonId phải lớn hơn 0", nameof(lessonId));

            return await _lessonDetailRepository.CountLessonDetailsByLessonIdAsync(lessonId, cancellationToken);
        }

        // ===== BUSINESS METHODS =====

        /// <summary>
        /// Reorder multiple lesson details at once (performance optimized with bulk update)
        /// </summary>
        public async Task ReorderLessonDetailsAsync(int lessonId, Dictionary<int, int> newOrders, CancellationToken cancellationToken = default)
        {
            if (lessonId <= 0)
                throw new ArgumentException("LessonId phải lớn hơn 0", nameof(lessonId));

            if (newOrders == null || !newOrders.Any())
                throw new ArgumentException("Danh sách reorder không được rỗng", nameof(newOrders));

            // Get all lesson details for this lesson
            var lessonDetails = (await _lessonDetailRepository.GetLessonDetailsByLessonIdAsync(lessonId, cancellationToken)).ToList();

            // Update orders in memory
            foreach (var kvp in newOrders)
            {
                var lessonDetail = lessonDetails.FirstOrDefault(ld => ld.LessonDetailId == kvp.Key);
                if (lessonDetail != null)
                {
                    lessonDetail.Order = kvp.Value;
                    lessonDetail.UpdatedAt = DateTime.UtcNow;
                }
            }

            // Validate no duplicate orders
            var duplicateOrders = lessonDetails
                .GroupBy(ld => ld.Order)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateOrders.Any())
                throw new InvalidOperationException($"Phát hiện Order trùng lặp: {string.Join(", ", duplicateOrders)}");

            // Bulk update (1 SaveChanges instead of N)
            await _lessonDetailRepository.UpdateRangeAsync(lessonDetails, cancellationToken);
        }

        /// <summary>
        /// Check if lesson detail can be deleted (currently always true, but can add business rules)
        /// </summary>
        public async Task<bool> CanDeleteLessonDetailAsync(int lessonDetailId, CancellationToken cancellationToken = default)
        {
            if (lessonDetailId <= 0)
                throw new ArgumentException("LessonDetailId phải lớn hơn 0", nameof(lessonDetailId));

            var exists = await _lessonDetailRepository.LessonDetailExistsAsync(lessonDetailId, cancellationToken);
            if (!exists)
                return false;

            // Can add business rules here, e.g., check if has attachments
            // var hasAttachments = await _lessonDetailRepository.HasAttachmentsAsync(lessonDetailId, cancellationToken);
            // if (hasAttachments) return false;

            return true;
        }

        /// <summary>
        /// Delete all lesson details for a lesson (bulk operation)
        /// </summary>
        public async Task DeleteLessonDetailsByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default)
        {
            if (lessonId <= 0)
                throw new ArgumentException("LessonId phải lớn hơn 0", nameof(lessonId));

            var lessonDetails = await _lessonDetailRepository.GetLessonDetailsByLessonIdAsync(lessonId, cancellationToken);

            if (lessonDetails.Any())
            {
                await _lessonDetailRepository.DeleteRangeAsync(lessonDetails, cancellationToken);
            }
        }

        /// <summary>
        /// Duplicate a lesson detail (optionally to a different lesson)
        /// </summary>
        public async Task<LessonDetail> DuplicateLessonDetailAsync(int lessonDetailId, int? targetLessonId = null, CancellationToken cancellationToken = default)
        {
            if (lessonDetailId <= 0)
                throw new ArgumentException("LessonDetailId phải lớn hơn 0", nameof(lessonDetailId));

            // Get source lesson detail
            var source = await _lessonDetailRepository.GetLessonDetailByIdAsync(lessonDetailId, cancellationToken);
            if (source == null)
                throw new KeyNotFoundException($"Không tìm thấy LessonDetail với ID {lessonDetailId}");

            // Use target lesson or same lesson
            var lessonId = targetLessonId ?? source.LessonId;

            // Validate target lesson exists
            var lessonExists = await _lessonRepository.LessonExistsAsync(lessonId);
            if (!lessonExists)
                throw new KeyNotFoundException($"Không tìm thấy Lesson với ID {lessonId}");

            // Create duplicate
            var duplicate = new LessonDetail
            {
                LessonId = lessonId,
                ContentType = source.ContentType,
                Content = source.Content,
                ContentLaTeX = source.ContentLaTeX,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                // Order will be auto-set by AddLessonDetailAsync
                Order = 0
            };

            // Add duplicate (Order will be auto-incremented)
            await AddLessonDetailAsync(duplicate, cancellationToken);

            return duplicate;
        }
    }
}
