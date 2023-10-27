namespace QuizApi.Models
{
    public class QuestionDto
    {
        public string QuestionText { get; set; } = "";
        public int CorrectChoiceIndex { get; set; }
        public string CorrectExplanation { get; set; } = "";
        public string IncorrectExplanation { get; set; } = "";
        public List<ChoiceDto> Choices { get; set; } = new List<ChoiceDto>();

    }
}
