using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using repositories.Dbcontext;
using repositories.EventSourcing.Aggregates;
using repositories.EventSourcing.Events;
using repositories.EventSourcing.Models;
using repositories.EventSourcing.Projections;
using System.Text.Json;

namespace services.EventSourcing.Projectors
{
    public class QuizProjectorService : IQuizProjectorService
    {
        private readonly MathLpContext _context;
        private readonly ILogger<QuizProjectorService> _logger;

        public QuizProjectorService(
            MathLpContext context,
            ILogger<QuizProjectorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ProjectEventAsync(IEvent @event)
        {
            _logger.LogInformation(
                "Projecting event {EventType} for aggregate {AggregateId}",
                @event.EventType, @event.AggregateId);

            try
            {
                switch (@event)
                {
                    case QuizGenerationInitiatedEvent e:
                        await HandleInitiatedAsync(e);
                        break;

                    case QuizAiRequestCreatedEvent e:
                        await HandleAiRequestCreatedAsync(e);
                        break;

                    case QuizContentGeneratedEvent e:
                        await HandleContentGeneratedAsync(e);
                        break;

                    case QuizCreatedEvent e:
                        await HandleQuizCreatedAsync(e);
                        break;

                    case QuizQuestionsAddedEvent e:
                        await HandleQuestionsAddedAsync(e);
                        break;

                    case QuizGenerationCompletedEvent e:
                        await HandleCompletedAsync(e);
                        break;

                    case QuizGenerationFailedEvent e:
                        await HandleFailedAsync(e);
                        break;
                }

                await UpdateStatisticsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error projecting event {EventType}", @event.EventType);
                throw;
            }
        }

        public async Task RebuildProjectionsAsync()
        {
            _logger.LogInformation("Rebuilding all projections...");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Clear existing projections
                _context.QuizListProjections.RemoveRange(_context.QuizListProjections);
                _context.QuizStatisticsProjections.RemoveRange(_context.QuizStatisticsProjections);
                await _context.SaveChangesAsync();

                // Get all events ordered by aggregate and version
                var events = await _context.EventStore
                    .Where(e => e.AggregateType == "QuizGenerationAggregate")
                    .OrderBy(e => e.AggregateId)
                    .ThenBy(e => e.Version)
                    .ToListAsync();

                _logger.LogInformation("Found {EventCount} events to replay", events.Count);

                // Group events by aggregate and replay
                var eventsByAggregate = events.GroupBy(e => e.AggregateId);

                foreach (var aggregateEvents in eventsByAggregate)
                {
                    foreach (var eventEntity in aggregateEvents)
                    {
                        var @event = DeserializeEvent(eventEntity);
                        await ProjectEventAsync(@event);
                    }
                }

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Rebuilt projections from {EventCount} events",
                    events.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error rebuilding projections");
                throw;
            }
        }

        #region Event Handlers

        private async Task HandleInitiatedAsync(QuizGenerationInitiatedEvent e)
        {
            var projection = new QuizListProjection
            {
                AggregateId = e.AggregateId,
                Title = e.Title,
                Topic = e.Topic,
                GradeLevel = e.GradeLevel,
                TeacherId = e.TeacherId,
                LevelId = e.LevelId,
                QuestionCount = e.QuestionCount,
                Duration = e.Duration,
                Status = "Processing",
                IsCompleted = false,
                IsFailed = false,
                InitiatedAt = e.OccurredAt,
                LastUpdated = e.OccurredAt,
                Version = e.Version
            };

            await _context.QuizListProjections.AddAsync(projection);
            await _context.SaveChangesAsync();
        }

        private async Task HandleAiRequestCreatedAsync(QuizAiRequestCreatedEvent e)
        {
            var projection = await _context.QuizListProjections
                .FirstOrDefaultAsync(p => p.AggregateId == e.AggregateId);

            if (projection != null)
            {
                projection.AiRequestId = e.AiRequestId;
                projection.LastUpdated = e.OccurredAt;
                projection.Version = e.Version;

                await _context.SaveChangesAsync();
            }
        }

        private async Task HandleContentGeneratedAsync(QuizContentGeneratedEvent e)
        {
            var projection = await _context.QuizListProjections
                .FirstOrDefaultAsync(p => p.AggregateId == e.AggregateId);

            if (projection != null)
            {
                projection.QuestionCount = e.QuestionCount;
                projection.TotalScore = e.TotalPoints;
                projection.LastUpdated = e.OccurredAt;
                projection.Version = e.Version;

                await _context.SaveChangesAsync();
            }
        }

        private async Task HandleQuizCreatedAsync(QuizCreatedEvent e)
        {
            var projection = await _context.QuizListProjections
                .FirstOrDefaultAsync(p => p.AggregateId == e.AggregateId);

            if (projection != null)
            {
                projection.QuizId = e.QuizId;
                projection.Title = e.Title;
                projection.TotalScore = (int)e.TotalScore;
                projection.Duration = e.TimeLimit;
                projection.LastUpdated = e.OccurredAt;
                projection.Version = e.Version;

                await _context.SaveChangesAsync();
            }
        }

        private async Task HandleQuestionsAddedAsync(QuizQuestionsAddedEvent e)
        {
            var projection = await _context.QuizListProjections
                .FirstOrDefaultAsync(p => p.AggregateId == e.AggregateId);

            if (projection != null)
            {
                projection.QuestionCount = e.QuestionIds.Count;
                projection.LastUpdated = e.OccurredAt;
                projection.Version = e.Version;

                await _context.SaveChangesAsync();
            }
        }

        private async Task HandleCompletedAsync(QuizGenerationCompletedEvent e)
        {
            var projection = await _context.QuizListProjections
                .FirstOrDefaultAsync(p => p.AggregateId == e.AggregateId);

            if (projection != null)
            {
                projection.Status = "Completed";
                projection.IsCompleted = true;
                projection.CompletedAt = e.OccurredAt;
                projection.ProcessingDuration = e.Duration.TotalSeconds; // Convert TimeSpan to seconds
                projection.LastUpdated = e.OccurredAt;
                projection.Version = e.Version;

                await _context.SaveChangesAsync();
            }
        }

        private async Task HandleFailedAsync(QuizGenerationFailedEvent e)
        {
            var projection = await _context.QuizListProjections
                .FirstOrDefaultAsync(p => p.AggregateId == e.AggregateId);

            if (projection != null)
            {
                projection.Status = "Failed";
                projection.IsFailed = true;
                projection.ErrorMessage = e.ErrorMessage;
                projection.CompletedAt = e.OccurredAt;
                projection.LastUpdated = e.OccurredAt;
                projection.Version = e.Version;

                await _context.SaveChangesAsync();
            }
        }

        private async Task UpdateStatisticsAsync()
        {
            var stats = await _context.QuizStatisticsProjections
                .FirstOrDefaultAsync();

            if (stats == null)
            {
                stats = new QuizStatisticsProjection();
                _context.QuizStatisticsProjections.Add(stats);
            }

            var projections = await _context.QuizListProjections.ToListAsync();

            stats.TotalInitiated = projections.Count;
            stats.TotalCompleted = projections.Count(p => p.IsCompleted);
            stats.TotalFailed = projections.Count(p => p.IsFailed);
            stats.SuccessRate = stats.TotalInitiated > 0
                ? (double)stats.TotalCompleted / stats.TotalInitiated * 100
                : 0;

            var completedProjections = projections
                .Where(p => p.IsCompleted && p.ProcessingDuration.HasValue)
                .ToList();

            if (completedProjections.Any())
            {
                stats.AverageDuration = completedProjections
                    .Average(p => p.ProcessingDuration!.Value);
                stats.MinDuration = completedProjections
                    .Min(p => p.ProcessingDuration!.Value);
                stats.MaxDuration = completedProjections
                    .Max(p => p.ProcessingDuration!.Value);
            }

            stats.TotalQuestionsGenerated = projections
                .Sum(p => p.QuestionCount);
            stats.AverageQuestionsPerQuiz = stats.TotalInitiated > 0
                ? (double)stats.TotalQuestionsGenerated / stats.TotalInitiated
                : 0;

            // Grade level distribution
            var gradeDistribution = projections
                .GroupBy(p => p.GradeLevel)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());
            stats.GradeLevelDistribution = JsonSerializer.Serialize(gradeDistribution);

            // Top topics
            var topTopics = projections
                .GroupBy(p => p.Topic)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new { Topic = g.Key, Count = g.Count() })
                .ToList();
            stats.TopTopics = JsonSerializer.Serialize(topTopics);

            stats.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        #endregion

        private IEvent DeserializeEvent(EventStoreEntity entity)
        {
            var assemblyQualifiedName = $"repositories.EventSourcing.Events.{entity.EventType}, repositories";
            var eventType = Type.GetType(assemblyQualifiedName);

            if (eventType == null)
            {
                throw new InvalidOperationException($"Unknown event type: {entity.EventType}");
            }

            var @event = JsonSerializer.Deserialize(entity.EventData, eventType) as IEvent;

            if (@event == null)
            {
                throw new InvalidOperationException($"Failed to deserialize event: {entity.EventType}");
            }

            return @event;
        }
    }
}
