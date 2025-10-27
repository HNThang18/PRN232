using repositories.EventSourcing.Events;
using repositories.EventSourcing.Models;
using repositories.Models;

namespace repositories.EventSourcing.Aggregates
{
    public class QuizGenerationAggregate : AggregateRoot
    {
        // Current state
        public int? TeacherId { get; private set; }
        public int? LevelId { get; private set; }
        public string Title { get; private set; }
        public int? GradeLevel { get; private set; }
        public string Topic { get; private set; }
        public int? QuestionCount { get; private set; }
        public int? Duration { get; private set; }
        public string Prompt { get; private set; }
        
        public int? AiRequestId { get; private set; }
        public AiRequestStatus? AiRequestStatus { get; private set; }
        public string AiResponse { get; private set; }
        
        public int? QuizId { get; private set; }
        public QuizStatus? QuizStatus { get; private set; }
        public decimal? TotalScore { get; private set; }
        public List<int> QuestionIds { get; private set; }
        
        public bool IsCompleted { get; private set; }
        public bool IsFailed { get; private set; }
        public string ErrorMessage { get; private set; }
        
        public DateTime? InitiatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public TimeSpan? ProcessingDuration { get; private set; }

        public QuizGenerationAggregate(string aggregateId) : base(aggregateId)
        {
            QuestionIds = new List<int>();
        }

        public void InitiateGeneration(
            int teacherId,
            int levelId,
            string title,
            int gradeLevel,
            string topic,
            int questionCount,
            int duration,
            string prompt,
            int? userId)
        {
            if (IsCompleted || IsFailed)
            {
                throw new InvalidOperationException("Cannot initiate generation on completed or failed aggregate");
            }

            var @event = new QuizGenerationInitiatedEvent(
                Id,
                teacherId,
                levelId,
                title,
                gradeLevel,
                topic,
                questionCount,
                duration,
                prompt,
                userId);

            ApplyEvent(@event);
        }

        public void CreateAiRequest(int aiRequestId, int? userId)
        {
            if (AiRequestId.HasValue)
            {
                throw new InvalidOperationException("AI request already created");
            }

            var @event = new QuizAiRequestCreatedEvent(
                Id,
                aiRequestId,
                RequestType.GenerateQuiz,
                userId);

            ApplyEvent(@event);
        }

        public void RecordAiContentGenerated(
            string aiResponse,
            int questionCount,
            int totalPoints,
            int? userId)
        {
            if (!AiRequestId.HasValue)
            {
                throw new InvalidOperationException("Cannot generate content without AI request");
            }

            var @event = new QuizContentGeneratedEvent(
                Id,
                AiRequestId.Value,
                aiResponse,
                questionCount,
                totalPoints,
                userId);

            ApplyEvent(@event);
        }

        public void CreateQuiz(
            int quizId,
            int teacherId,
            int levelId,
            string title,
            int timeLimit,
            decimal totalScore,
            int? userId)
        {
            if (QuizId.HasValue)
            {
                throw new InvalidOperationException("Quiz already created");
            }

            var @event = new QuizCreatedEvent(
                Id,
                quizId,
                teacherId,
                levelId,
                title,
                timeLimit,
                totalScore,
                userId);

            ApplyEvent(@event);
        }

        public void AddQuestions(List<int> questionIds, int? userId)
        {
            if (!QuizId.HasValue)
            {
                throw new InvalidOperationException("Cannot add questions without quiz");
            }

            var @event = new QuizQuestionsAddedEvent(
                Id,
                QuizId.Value,
                questionIds,
                userId);

            ApplyEvent(@event);
        }

        public void CompleteGeneration(int? userId)
        {
            if (IsCompleted)
            {
                throw new InvalidOperationException("Generation already completed");
            }

            if (!QuizId.HasValue || !AiRequestId.HasValue)
            {
                throw new InvalidOperationException("Cannot complete without quiz and AI request");
            }

            var duration = DateTime.UtcNow - InitiatedAt.Value;

            var @event = new QuizGenerationCompletedEvent(
                Id,
                QuizId.Value,
                AiRequestId.Value,
                QuestionCount ?? 0,
                TotalScore ?? 0,
                duration,
                userId);

            ApplyEvent(@event);
        }

        public void FailGeneration(
            string errorMessage,
            string errorType,
            string stackTrace,
            int? userId)
        {
            if (IsCompleted)
            {
                throw new InvalidOperationException("Cannot fail completed generation");
            }

            var @event = new QuizGenerationFailedEvent(
                Id,
                AiRequestId,
                errorMessage,
                errorType,
                stackTrace,
                userId);

            ApplyEvent(@event);
        }

        #region Event Handlers

        public void Apply(QuizGenerationInitiatedEvent @event)
        {
            TeacherId = @event.TeacherId;
            LevelId = @event.LevelId;
            Title = @event.Title;
            GradeLevel = @event.GradeLevel;
            Topic = @event.Topic;
            QuestionCount = @event.QuestionCount;
            Duration = @event.Duration;
            Prompt = @event.Prompt;
            InitiatedAt = @event.OccurredAt;
        }

        public void Apply(QuizAiRequestCreatedEvent @event)
        {
            AiRequestId = @event.AiRequestId;
            AiRequestStatus = @event.Status;
        }

        public void Apply(QuizContentGeneratedEvent @event)
        {
            AiResponse = @event.AiResponse;
            AiRequestStatus = repositories.Models.AiRequestStatus.Success;
            TotalScore = @event.TotalPoints;
        }

        public void Apply(QuizCreatedEvent @event)
        {
            QuizId = @event.QuizId;
            QuizStatus = @event.Status;
            TotalScore = @event.TotalScore;
        }

        public void Apply(QuizQuestionsAddedEvent @event)
        {
            QuestionIds.AddRange(@event.QuestionIds);
        }

        public void Apply(QuizGenerationCompletedEvent @event)
        {
            IsCompleted = true;
            CompletedAt = @event.OccurredAt;
            ProcessingDuration = @event.Duration;
        }

        public void Apply(QuizGenerationFailedEvent @event)
        {
            IsFailed = true;
            ErrorMessage = @event.ErrorMessage;
            CompletedAt = @event.OccurredAt;
            if (AiRequestId.HasValue)
            {
                AiRequestStatus = repositories.Models.AiRequestStatus.Failed;
            }
        }

        #endregion
    }
}
