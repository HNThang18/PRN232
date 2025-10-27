using repositories.Models;
using repositories.EventSourcing.Models;

namespace repositories.EventSourcing.Events
{
    public class QuizGenerationInitiatedEvent : BaseEvent
    {
        public int TeacherId { get; set; }
        public int LevelId { get; set; }
        public string Title { get; set; }
        public int GradeLevel { get; set; }
        public string Topic { get; set; }
        public int QuestionCount { get; set; }
        public int Duration { get; set; }
        public string Prompt { get; set; }

        public QuizGenerationInitiatedEvent(
            string aggregateId,
            int teacherId,
            int levelId,
            string title,
            int gradeLevel,
            string topic,
            int questionCount,
            int duration,
            string prompt,
            int? userId = null)
            : base(aggregateId, userId)
        {
            TeacherId = teacherId;
            LevelId = levelId;
            Title = title;
            GradeLevel = gradeLevel;
            Topic = topic;
            QuestionCount = questionCount;
            Duration = duration;
            Prompt = prompt;
        }

        // For deserialization
        public QuizGenerationInitiatedEvent() : base() { }
    }

    public class QuizAiRequestCreatedEvent : BaseEvent
    {
        public int AiRequestId { get; set; }
        public RequestType RequestType { get; set; }
        public AiRequestStatus Status { get; set; }

        public QuizAiRequestCreatedEvent(
            string aggregateId,
            int aiRequestId,
            RequestType requestType,
            int? userId = null)
            : base(aggregateId, userId)
        {
            AiRequestId = aiRequestId;
            RequestType = requestType;
            Status = AiRequestStatus.Pending;
        }

        public QuizAiRequestCreatedEvent() : base() { }
    }

    public class QuizContentGeneratedEvent : BaseEvent
    {
        public int AiRequestId { get; set; }
        public string AiResponse { get; set; }
        public int QuestionCount { get; set; }
        public int TotalPoints { get; set; }

        public QuizContentGeneratedEvent(
            string aggregateId,
            int aiRequestId,
            string aiResponse,
            int questionCount,
            int totalPoints,
            int? userId = null)
            : base(aggregateId, userId)
        {
            AiRequestId = aiRequestId;
            AiResponse = aiResponse;
            QuestionCount = questionCount;
            TotalPoints = totalPoints;
        }

        public QuizContentGeneratedEvent() : base() { }
    }

    public class QuizCreatedEvent : BaseEvent
    {
        public int QuizId { get; set; }
        public int TeacherId { get; set; }
        public int LevelId { get; set; }
        public string Title { get; set; }
        public int TimeLimit { get; set; }
        public decimal TotalScore { get; set; }
        public QuizStatus Status { get; set; }
        public bool IsAIGenerated { get; set; }

        public QuizCreatedEvent(
            string aggregateId,
            int quizId,
            int teacherId,
            int levelId,
            string title,
            int timeLimit,
            decimal totalScore,
            int? userId = null)
            : base(aggregateId, userId)
        {
            QuizId = quizId;
            TeacherId = teacherId;
            LevelId = levelId;
            Title = title;
            TimeLimit = timeLimit;
            TotalScore = totalScore;
            Status = QuizStatus.Draft;
            IsAIGenerated = true;
        }

        public QuizCreatedEvent() : base() { }
    }

    public class QuizQuestionsAddedEvent : BaseEvent
    {
        public int QuizId { get; set; }
        public List<int> QuestionIds { get; set; }
        public int QuestionCount { get; set; }

        public QuizQuestionsAddedEvent(
            string aggregateId,
            int quizId,
            List<int> questionIds,
            int? userId = null)
            : base(aggregateId, userId)
        {
            QuizId = quizId;
            QuestionIds = questionIds;
            QuestionCount = questionIds.Count;
        }

        public QuizQuestionsAddedEvent() : base() { QuestionIds = new List<int>(); }
    }

    public class QuizGenerationCompletedEvent : BaseEvent
    {
        public int QuizId { get; set; }
        public int AiRequestId { get; set; }
        public int QuestionCount { get; set; }
        public decimal TotalScore { get; set; }
        public TimeSpan Duration { get; set; }

        public QuizGenerationCompletedEvent(
            string aggregateId,
            int quizId,
            int aiRequestId,
            int questionCount,
            decimal totalScore,
            TimeSpan duration,
            int? userId = null)
            : base(aggregateId, userId)
        {
            QuizId = quizId;
            AiRequestId = aiRequestId;
            QuestionCount = questionCount;
            TotalScore = totalScore;
            Duration = duration;
        }

        public QuizGenerationCompletedEvent() : base() { }
    }

    public class QuizGenerationFailedEvent : BaseEvent
    {
        public int? AiRequestId { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
        public string StackTrace { get; set; }

        public QuizGenerationFailedEvent(
            string aggregateId,
            int? aiRequestId,
            string errorMessage,
            string errorType,
            string stackTrace,
            int? userId = null)
            : base(aggregateId, userId)
        {
            AiRequestId = aiRequestId;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
            StackTrace = stackTrace;
        }

        public QuizGenerationFailedEvent() : base() { }
    }
}
