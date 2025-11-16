using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.Quiz
{
    public class AddQuestionsToQuizRequestDto
    {
        [Required]
        public List<int> QuestionIds { get; set; }
    }
}
