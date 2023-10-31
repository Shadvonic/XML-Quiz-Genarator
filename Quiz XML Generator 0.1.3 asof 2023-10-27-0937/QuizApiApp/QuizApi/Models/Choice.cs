namespace QuizApi.Models
{
    public class Choice
    {
        public Guid Id { get; set; }
        public string ChoiceText { get; set; } = "";
        public  Guid QuestionId { get; set; }
    }
}
