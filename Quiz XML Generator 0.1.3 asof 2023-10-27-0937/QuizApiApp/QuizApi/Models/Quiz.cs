using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApi.Models
{
    public class Quiz
    {
        public Guid Id { get; set; }
        public string QuizName { get; set; } = "";
       

    }
}
