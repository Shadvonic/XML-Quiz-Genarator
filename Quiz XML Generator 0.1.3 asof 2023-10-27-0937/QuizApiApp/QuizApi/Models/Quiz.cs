﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApi.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string QuizName { get; set; } = "";
       public List<Question> Questions { get; set; } = new List<Question>();
    }
}
