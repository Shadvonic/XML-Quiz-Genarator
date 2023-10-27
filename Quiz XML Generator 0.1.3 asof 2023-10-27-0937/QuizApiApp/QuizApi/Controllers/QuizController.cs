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
        public IActionResult SaveQuizToSql(Quiz quizDto)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertQuizSql = "INSERT INTO dbo.Quizzes (QuizName) " +
                        "OUTPUT INSERTED.Id " +
                        "VALUES (@QuizName)";

                    var quiz = new Quiz()
                    {
                        QuizName = quizDto.QuizName
                    };

                    int newQuizId = connection.ExecuteScalar<int>(insertQuizSql, quiz);

                    if (newQuizId > 0)
                    {
                        // Iterate through the questions in the quiz and insert them
                        foreach (var question in quizDto.Questions)
                        {
                            string insertQuestionSql = "INSERT INTO dbo.Questions (QuizId, QuestionText, CorrectChoiceIndex, CorrectExplanation, IncorrectExplanation) " +
                                "VALUES (@QuizId, @QuestionText, @CorrectChoiceIndex, @CorrectExplanation, @IncorrectExplanation)";

                            var questionToInsert = new Question
                            {
                                QuizId = newQuizId,
                                QuestionText = question.QuestionText,
                                CorrectChoiceIndex = question.CorrectChoiceIndex,
                                CorrectExplanation = question.CorrectExplanation,
                                IncorrectExplanation = question.IncorrectExplanation
                            };

                            connection.Execute(insertQuestionSql, questionToInsert);

                            // Retrieve the ID of the newly inserted question
                            string selectQuestionSql = "SELECT IDENT_CURRENT('dbo.Questions')";

                            int newQuestionId = connection.ExecuteScalar<int>(selectQuestionSql);

                            if (newQuestionId > 0)
                            {
                                // Iterate through the choices in the question and insert them
                                foreach (var choice in question.Choices)
                                {
                                    string insertChoiceSql = "INSERT INTO dbo.Choices (QuestionId, ChoiceText) " +
                                        "VALUES (@QuestionId, @ChoiceText)";

                                    var choiceToInsert = new Choice
                                    {
                                        QuestionId = newQuestionId,
                                        ChoiceText = choice.ChoiceText
                                    };

                                    connection.Execute(insertChoiceSql, choiceToInsert);
                                }
                            }
                        }

                        // Now that you have inserted the quiz, questions, and choices, you can return the newly created quiz.
                        string selectQuizSql = "SELECT * FROM dbo.Quizzes WHERE Id = @Id";
                        var newQuiz = connection.QuerySingleOrDefault<Quiz>(selectQuizSql, new { Id = newQuizId });

                        if (newQuiz != null)
                        {
                            return Ok(newQuiz);
                        }
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
                    var quiz = connection.QuerySingleOrDefault<Quiz>(selectQuizSql, new { Id = id });

                    if (quiz != null)
                    {
                        // Retrieve questions for the quiz
                        string selectQuestionsSql = "SELECT Id, QuestionText, CorrectChoiceIndex, CorrectExplanation, IncorrectExplanation FROM dbo.Questions WHERE QuizId = @QuizId";
                        quiz.Questions = connection.Query<Question>(selectQuestionsSql, new { QuizId = id }).ToList();

                        foreach (var question in quiz.Questions)
                        {
                            // Retrieve choices for each question
                            string selectChoicesSql = "SELECT Id, ChoiceText FROM dbo.Choices WHERE QuestionId = @QuestionId";
                            question.Choices = connection.Query<Choice>(selectChoicesSql, new { QuestionId = question.Id }).ToList();
                        }

                        return Ok(quiz);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("We have an exception \n" + ex.Message);
                return BadRequest();
            }

            return NotFound();
        }



    }

}




   