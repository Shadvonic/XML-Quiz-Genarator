using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly string connectionString;

        public QuestionController(IConfiguration config)
        {
            connectionString = config.GetConnectionString("Default")!;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    var questions = await db.QueryAsync<Question>("SELECT * FROM Questions");
                    return Ok(questions);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestionById(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    var question = await db.QueryFirstOrDefaultAsync<Question>("SELECT * FROM Questions WHERE Id = @Id", new { Id = id });
                    if (question == null)
                        return NotFound();

                    return Ok(question);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveQuestion([FromBody] Question question)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    var query = @"INSERT INTO Questions (QuizId, QuestionText, CorrectChoiceIndex, CorrectExplanation, IncorrectExplanation)
                              VALUES (@QuizId, @QuestionText, @CorrectChoiceIndex, @CorrectExplanation, @IncorrectExplanation)";
                    await db.ExecuteAsync(query, question);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
