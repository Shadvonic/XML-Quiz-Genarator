using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuizApi.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;



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


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    var quizzes = await db.QueryAsync<Quiz>("SELECT * FROM Quizzes");
                    return Ok(quizzes);
                }
            }
            
           
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
           
           
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Quiz>> GetQuizById(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    var quiz = await db.QueryFirstOrDefaultAsync<Quiz>("SELECT * FROM Quizzes WHERE Id = @Id", new { Id = id });
                    if (quiz == null)
                        return NotFound();

                    return Ok(quiz);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveQuiz([FromBody] Quiz quiz)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    var query = "INSERT INTO Quizzes (QuizName) VALUES (@QuizName)";
                    await db.ExecuteAsync(query, new { quiz.QuizName });
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


