namespace QuizApi.Models
{
    public class QuizDto
    {
        public string QuizName { get; set; } = "";
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
