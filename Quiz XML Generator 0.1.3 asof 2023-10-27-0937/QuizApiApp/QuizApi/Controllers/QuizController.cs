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

           
        [HttpGet("{id}")]
        public async Task<ActionResult<Quiz>> GetQuizById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var quiz = await connection.QueryFirstOrDefaultAsync<Quiz>("SELECT * FROM Quizzes WHERE Id = @Id", new { Id = id });

                if (quiz == null)
                {
                    return NotFound();
                }



                var questions = await connection.QueryAsync<Question>("SELECT * FROM Questions WHERE QuizId = @Id", new { Id = id });

                foreach (var question in questions)
                {
                    var choices = await connection.QueryAsync<Choice>("SELECT * FROM Choices WHERE QuestionId = @QuestionId", new { QuestionId = question.Id });
                    question.Choices = choices.AsList();
                }

                quiz.Questions = questions.AsList();

                return Ok(quiz);
            }
        }


        [HttpGet("{name}")]
        public async Task<ActionResult<Quiz>> GetQuizByName(string name)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var quiz = await connection.QueryFirstOrDefaultAsync<Quiz>("SELECT * FROM Quizzes WHERE QuizName = @Name", new { Name = name });

                if (quiz == null)
                {
                    return NotFound();
                }

                var questions = await connection.QueryAsync<Question>("SELECT * FROM Questions WHERE QuizId = @Id", new { Id = quiz.Id });

                foreach (var question in questions)
                {
                    var choices = await connection.QueryAsync<Choice>("SELECT * FROM Choices WHERE QuestionId = @QuestionId", new { QuestionId = question.Id });
                    question.Choices = choices.AsList();
                }

                quiz.Questions = questions.AsList();

                return Ok(quiz);
            }
              
        }

        [HttpPost]
        public async Task<ActionResult<Quiz>> AddQuiz([FromBody] Quiz quiz)
        {
            if (quiz == null)
            {
                return BadRequest("Invalid quiz data");
            }

            using (var connection = new SqlConnection(connectionString))
            {
                var query = "INSERT INTO Quizzes (QuizName) VALUES (@QuizName); SELECT CAST(SCOPE_IDENTITY() as int)";
                var id = await connection.QueryFirstOrDefaultAsync<int>(query, new { QuizName = quiz.QuizName });

                foreach (var question in quiz.Questions)
                {
                    var questionQuery = "INSERT INTO Questions (QuizId, QuestionText, CorrectChoiceIndex, CorrectExplanation, IncorrectExplanation) " +
                                       "VALUES (@QuizId, @QuestionText, @CorrectChoiceIndex, @CorrectExplanation, @IncorrectExplanation); " +
                                       "SELECT CAST(SCOPE_IDENTITY() as int)";
                    var questionId = await connection.QueryFirstOrDefaultAsync<int>(questionQuery, new
                    {
                        QuizId = id,
                        QuestionText = question.QuestionText,
                        CorrectChoiceIndex = question.CorrectChoiceIndex,
                        CorrectExplanation = question.CorrectExplanation,
                        IncorrectExplanation = question.IncorrectExplanation
                    });

                    foreach (var choice in question.Choices)
                    {
                        var choiceQuery = "INSERT INTO Choices (ChoiceText, QuestionId) VALUES (@ChoiceText, @QuestionId)";
                        await connection.ExecuteAsync(choiceQuery, new { ChoiceText = choice.ChoiceText, QuestionId = questionId });
                    }
                }

                return CreatedAtAction(nameof(GetQuizById), new { id = id }, quiz);
            }
        }
                
            
    }
}
