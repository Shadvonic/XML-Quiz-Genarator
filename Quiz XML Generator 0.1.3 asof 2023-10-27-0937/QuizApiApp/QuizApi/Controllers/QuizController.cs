using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuizApi.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly string connectionString;

        public QuizController(IConfiguration config)
        {
            connectionString = config.GetConnectionString("Default")!;
        }


        [HttpPost("save")]

        public async Task<IActionResult> SaveQuiz([FromBody] List<Quiz> quizData)

        {

            try

            {

                using (IDbConnection db = new SqlConnection(connectionString))

                {

                    foreach (var data in quizData)

                    {
                        string query = "INSERT INTO Quizzes (QuizName) VALUES (@QuizName); SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        int quizId = await Task.Run(() => db.QuerySingle<int>(query, new { data.QuizName }));





                        foreach (var question in data.Questions)

                        {

                            var questionId = await Task.Run(() => db.QuerySingle<int>("INSERT INTO Questions (QuizId, QuestionText, CorrectChoiceIndex, CorrectExplanation, IncorrectExplanation) VALUES (@QuizId, @QuestionText, @CorrectChoiceIndex, @CorrectExplanation, @IncorrectExplanation); SELECT CAST(SCOPE_IDENTITY() AS INT)",

                                new { QuizId = quizId, QuestionText = question.QuestionText, CorrectChoiceIndex = question.CorrectChoiceIndex, CorrectExplanation = question.CorrectExplanation, IncorrectExplanation = question.IncorrectExplanation }));



                            foreach (var choice in question.Choices)

                            {

                                await Task.Run(() => db.Execute("INSERT INTO Choices (ChoiceText, QuestionId) VALUES (@ChoiceText, @QuestionId)", new { ChoiceText = choice.ChoiceText, QuestionId = questionId }));

                            }

                        }

                    }

                }



                return Ok("Quiz data saved successfully.");

            }

            catch (Exception ex)

            {

                Console.WriteLine(ex.Message);

                return StatusCode(500, "An error occurred while saving the quiz data.");

            }

        }




        [HttpGet("load/{quizId}")]
        public async Task<IActionResult> GetQuiz(int quizId)

        {

            try

            {

                using (IDbConnection db = new SqlConnection(connectionString))

                {

                    var quiz = RetrieveQuiz(db, quizId);



                    if (quiz != null)

                    {

                        var questions = RetrieveQuestions(db, quizId);



                        foreach (var question in questions)

                        {

                            question.Choices = RetrieveChoices(db, question.Id);

                        }



                        quiz.Questions = questions;

                        return Ok(quiz);

                    }



                    return NotFound();

                }

            }

            catch (Exception ex)

            {

               
                Console.WriteLine(ex.Message);

                return StatusCode(500, "An error occurred while fetching the quiz data.");

            }

        }




        private Quiz RetrieveQuiz(IDbConnection db, int quizId)

        {

            string query = "SELECT Id, QuizName FROM Quizzes WHERE Id = @QuizId";

           
            return db.QueryFirstOrDefault<Quiz>(query, new { QuizId = quizId });

        }



        private List<Question> RetrieveQuestions(IDbConnection db, int quizId)

        {

            string query = "SELECT Id, QuizId, QuestionText, CorrectChoiceIndex, CorrectExplanation, IncorrectExplanation FROM Questions WHERE QuizId = @QuizId";



            var questions = db.Query<Question>(query, new { QuizId = quizId });



            foreach (var question in questions)

            {

                // Check for null values and assign defaults if necessary

                if (question.CorrectExplanation == null)

                {

                    question.CorrectExplanation = string.Empty; // Or any default value that fits your model

                }



                if (question.IncorrectExplanation == null)

                {

                    question.IncorrectExplanation = string.Empty; // Or any default value that fits your model

                }

            }



            return questions.ToList();

        }







        private List<Choice> RetrieveChoices(IDbConnection db, int questionId)

        {

            string query = "SELECT Id, ChoiceText, QuestionId FROM Choices WHERE QuestionId = @QuestionId";

            return db.Query<Choice>(query, new { QuestionId = questionId }).ToList();

        }


    }
}
