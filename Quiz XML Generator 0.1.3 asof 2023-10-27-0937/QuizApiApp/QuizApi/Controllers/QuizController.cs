using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {

        private readonly string connectionString;

        // read connnection string from appsetting
        public QuizController(IConfiguration config)
        {
            connectionString = config.GetConnectionString("Default")!;

        }


        [HttpPost("Save")]
        public IActionResult SaveQuizToSql(QuizDto quizDto)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert the quiz
                    var quiz = new Quiz
                    {
                        QuizName = quizDto.QuizName
                    };

                    string insertQuizSql = "INSERT INTO dbo.Quizzes (QuizName) " +
                        "OUTPUT INSERTED.Id " +
                        "VALUES (@QuizName)";

                    int newQuizId = connection.ExecuteScalar<int>(insertQuizSql, quiz);

                    if (newQuizId > 0)
                    {
                        var questions = new List<Question>();

                        // Iterate through the questions in the quiz and insert them
                        foreach (var questionDto in quizDto.Questions)
                        {
                            var question = new Question
                            {
                                QuizId = newQuizId,
                                QuestionText = questionDto.QuestionText,
                                CorrectChoiceIndex = questionDto.CorrectChoiceIndex,
                                CorrectExplanation = questionDto.CorrectExplanation,
                                IncorrectExplanation = questionDto.IncorrectExplanation
                            };

                            questions.Add(question);
                        }

                        string insertQuestionSql = "INSERT INTO dbo.Questions (QuizId, QuestionText, CorrectChoiceIndex, CorrectExplanation, IncorrectExplanation) " +
                            "VALUES (@QuizId, @QuestionText, @CorrectChoiceIndex, @CorrectExplanation, @IncorrectExplanation)";

                        connection.Execute(insertQuestionSql, questions);

                        // Iterate through the choices in the questions and insert them
                        foreach (var questionDto in quizDto.Questions)
                        {
                            var question = questions.FirstOrDefault(q => q.QuestionText == questionDto.QuestionText);

                            if (question != null)
                            {
                                var choices = questionDto.Choices.Select(choiceDto => new Choice
                                {
                                    QuestionId = question.Id,
                                    ChoiceText = choiceDto.ChoiceText
                                }).ToList();

                                string insertChoiceSql = "INSERT INTO dbo.Choices (QuestionId, ChoiceText) " +
                                    "VALUES (@QuestionId, @ChoiceText)";

                                connection.Execute(insertChoiceSql, choices);
                            }
                        }

                        // Retrieve the newly created quiz without the IDs
                        var newQuiz = new Quiz
                        {
                            QuizName = quizDto.QuizName,
                            Questions = questions
                        };

                        return Ok(newQuiz);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("We have an exception: \n" + ex.Message);
            }

            return BadRequest();
        }

        [HttpGet("Load/{id}")]
        public IActionResult LoadQuizFromSql(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuizSql = "SELECT QuizName FROM dbo.Quizzes WHERE Id = @Id";
                    var quiz = connection.QuerySingleOrDefault<QuizDto>(selectQuizSql, new { Id = id });

                    if (quiz != null)
                    {
                        // Retrieve questions for the quiz
                        string selectQuestionsSql = "SELECT Id, QuestionText, CorrectChoiceIndex, CorrectExplanation, IncorrectExplanation FROM dbo.Questions WHERE QuizId = @QuizId";
                        var questions = connection.Query<QuestionDto>(selectQuestionsSql, new { QuizId = id }).ToList();

                        foreach (var question in questions)
                        {
                            // Retrieve choices for each question
                            string selectChoicesSql = "SELECT ChoiceText FROM dbo.Choices WHERE QuestionId = @QuestionId";
                            question.Choices = connection.Query<ChoiceDto>(selectChoicesSql, new { QuestionId = question.Choices}).ToList();
                
                        }

                        quiz.Questions = questions;
                        return Ok(quiz);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("We have an exception \n" + ex.Message);
            }

            return NotFound();
        }


    }

}




   