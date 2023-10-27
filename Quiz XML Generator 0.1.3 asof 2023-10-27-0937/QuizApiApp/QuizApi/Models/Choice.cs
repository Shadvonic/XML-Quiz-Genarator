namespace QuizApi.Models
{
    public class Choice
    {
        public int Id { get; set; }
        public string ChoiceText { get; set; } = "";
        public int QuestionId { get; set; }
    }
}
