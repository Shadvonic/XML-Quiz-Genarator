using QuizApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApi.Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string QuestionText { get; set; } = "";
        public int CorrectChoiceIndex { get; set; } 
        public string CorrectExplanation { get; set; } = "";
        public string IncorrectExplanation { get; set; } = "";
   



    }
}
