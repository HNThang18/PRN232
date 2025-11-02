using applications.DTOs.Request.Progress;
using applications.DTOs.Response.Progress;
using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepo;
        private readonly ISubmissionDetailRepository _submissionDetailRepo;
        private readonly IQuizRepository _quizRepo; // Repo của bạn bạn
        private readonly IQuestionRepository _questionRepo; // Repo của bạn bạn

        public SubmissionService(
            ISubmissionRepository submissionRepo,
            ISubmissionDetailRepository submissionDetailRepo,
            IQuizRepository quizRepo,
            IQuestionRepository questionRepo) // Cần repo này để lấy đáp án
        {
            _submissionRepo = submissionRepo;
            _submissionDetailRepo = submissionDetailRepo;
            _quizRepo = quizRepo;
            _questionRepo = questionRepo;
        }

        // === 1. BẮT ĐẦU LÀM BÀI ===
        public async Task<QuizStartResponse> StartQuizAsync(int quizId, int studentId)
        {
            // 1. Lấy thông tin Quiz
            // (!!!) Yêu cầu bạn của bạn tạo hàm 'GetQuizWithDetailsAsync'
            // để lấy Quiz KÈM THEO Questions và Answers
            var quiz = await _quizRepo.GetQuizWithDetailsAsync(quizId);

            if (quiz == null || quiz.Status != QuizStatus.Published)
            {
                throw new Exception("Bài kiểm tra không tồn tại hoặc chưa được công bố.");
            }

            // 2. Kiểm tra giới hạn làm bài
            var attemptCount = await _submissionRepo.GetSubmissionCountAsync(studentId, quizId);
            if (quiz.AttemptLimit > 0 && attemptCount >= quiz.AttemptLimit)
            {
                throw new Exception($"Bạn đã hết {quiz.AttemptLimit} lượt làm bài.");
            }

            // 3. Tạo bản ghi Submission mới
            var newSubmission = new Submission
            {
                StudentId = studentId,
                QuizId = quizId,
                Status = SubissionStatus.InProgress,
                AttemptNumber = attemptCount + 1,
                SubmittedAt = DateTime.UtcNow // Dùng làm mốc thời gian bắt đầu
            };

            // Vì không dùng UoW, hàm này sẽ tự SaveChanges()
            await _submissionRepo.CreateAsync(newSubmission);

            // newSubmission.SubmissionId SẼ được cập nhật tự động bởi EF Core

            // 4. Map dữ liệu câu hỏi sang DTO (để gửi về cho Frontend)
            var response = new QuizStartResponse
            {
                SubmissionId = newSubmission.SubmissionId,
                TimeLimit = quiz.TimeLimit,
                Questions = quiz.Questions.Select(q => new QuestionResponse
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType.ToString(),
                    // Map các lựa chọn trả lời (không gửi đáp án đúng)
                    Answers = q.Answers.Select(a => new AnswerResponse
                    {
                        AnswerId = a.AnswerId,
                        AnswerText = a.AnswerText
                    }).ToList()
                }).ToList()
            };

            return response;
        }

        // === 2. NỘP BÀI VÀ CHẤM ĐIỂM ===
        public async Task<SubmissionResultResponse> SubmitQuizAsync(int submissionId, SubmitQuizRequest request, int studentId)
        {
            // 1. Lấy bài nộp
            var submission = await _submissionRepo.GetByIdAsync(submissionId);
            if (submission == null || submission.StudentId != studentId)
            {
                throw new Exception("Không tìm thấy bài nộp.");
            }
            if (submission.Status != SubissionStatus.InProgress)
            {
                throw new Exception("Bài nộp này đã hoàn thành.");
            }

            // 2. Lấy đáp án (cần QuestionRepository của bạn bạn)
            var questionIds = request.Answers.Select(a => a.QuestionId).ToList();
            // (Giả sử IQuestionRepository có hàm này)
            var questions = await _questionRepo.GetQuestionsWithCorrectAnswersAsync(questionIds);
            var questionMap = questions.ToDictionary(q => q.QuestionId);

            decimal totalScore = 0;
            int correctCount = 0;
            var detailsToSave = new List<SubmissionDetail>();

            // 3. Lặp qua từng câu trả lời và CHẤM ĐIỂM
            foreach (var studentAnswer in request.Answers)
            {
                if (!questionMap.TryGetValue(studentAnswer.QuestionId, out var question))
                {
                    continue; // Bỏ qua nếu không tìm thấy câu hỏi
                }

                // Logic chấm điểm (đơn giản, chỉ so sánh string)
                // (!!!) Logic này cần phức tạp hơn để xử lý MultipleChoice
                bool isCorrect = (studentAnswer.AnswerText == question.CorrectAnswer);

                if (isCorrect)
                {
                    correctCount++;
                    // (Giả sử mỗi câu 1 điểm)
                    totalScore += 1;
                }

                detailsToSave.Add(new SubmissionDetail
                {
                    SubmissionId = submissionId,
                    QuestionId = question.QuestionId,
                    StudentAnswer = studentAnswer.AnswerText,
                    IsCorrect = isCorrect,
                    ScoreEarned = isCorrect ? 1 : 0,
                    Explanation = question.Explanation
                });
            }

            // 4. LƯU chi tiết bài nộp
            // (!!!) Rủi ro: Hàm này tự SaveChanges()
            await _submissionDetailRepo.AddRangeAsync(detailsToSave);

            // 5. CẬP NHẬT bài nộp tổng
            submission.Status = SubissionStatus.Completed;
            submission.Score = totalScore;
            submission.DurationTaken = (int)(DateTime.UtcNow - submission.SubmittedAt).TotalSeconds;
            submission.SubmittedAt = DateTime.UtcNow; // Cập nhật lại thời gian nộp

            // (!!!) Rủi ro: Hàm này tự SaveChanges() lần 2
            await _submissionRepo.UpdateAsync(submission);

            // 6. Trả về kết quả
            return await GetSubmissionResultAsync(submissionId, studentId);
        }

        // === 3. XEM LẠI KẾT QUẢ ===
        public async Task<SubmissionResultResponse> GetSubmissionResultAsync(int submissionId, int studentId)
        {
            // (Bạn cần thêm ISubmissionDetailRepository vào UoW hoặc inject riêng)
            // (Giả sử ISubmissionRepository có hàm GetWithDetails)
            var submission = await _submissionRepo.GetSubmissionWithDetailsAsync(submissionId);

            if (submission == null || submission.StudentId != studentId)
            {
                throw new Exception("Không tìm thấy bài nộp.");
            }

            // Map ra DTO
            var response = new SubmissionResultResponse
            {
                SubmissionId = submission.SubmissionId,
                QuizId = submission.QuizId,
                QuizTitle = submission.Quiz.Title,
                Score = submission.Score,
                TotalQuestions = submission.SubmissionDetails.Count,
                CorrectAnswers = submission.SubmissionDetails.Count(d => d.IsCorrect),
                SubmittedAt = submission.SubmittedAt,
                DurationTaken = submission.DurationTaken,
                Status = submission.Status.ToString(),
                Details = submission.SubmissionDetails.Select(d => new SubmissionDetailResponse
                {
                    QuestionId = d.QuestionId,
                    QuestionText = d.Question.QuestionText,
                    StudentAnswer = d.StudentAnswer,
                    CorrectAnswer = d.Question.CorrectAnswer, // Gửi đáp án đúng
                    IsCorrect = d.IsCorrect,
                    ScoreEarned = d.ScoreEarned,
                    Explanation = d.Explanation
                }).ToList()
            };
            return response;
        }

        // === 4. XEM LỊCH SỬ LÀM BÀI ===
        public async Task<List<SubmissionSummaryResponse>> GetSubmissionHistoryAsync(int quizId, int studentId)
        {
            var submissions = await _submissionRepo.GetSubmissionsByStudentAndQuizAsync(studentId, quizId);

            return submissions.Select(s => new SubmissionSummaryResponse
            {
                SubmissionId = s.SubmissionId,
                Score = s.Score,
                SubmittedAt = s.SubmittedAt,
                AttemptNumber = s.AttemptNumber
            }).ToList();
        }
    }
}
