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
    public class ChoiceController : ControllerBase
    {
        private readonly string connectionString;

        public ChoiceController(IConfiguration config)
        {
            connectionString = config.GetConnectionString("Default")!;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Choice>>> GetChoices()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    var choices = await db.QueryAsync<Choice>("SELECT * FROM Choices");
                    return Ok(choices);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Choice>> GetChoiceById(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {


                    var choice = await db.QueryFirstOrDefaultAsync<Choice>("SELECT * FROM Choices WHERE Id = @Id", new { Id = id });
                    if (choice == null)
                        return NotFound();

                    return Ok(choice);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveChoices([FromBody] List<Choice> choices)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    Guid questionId = Guid.NewGuid();

                    foreach (var choice in choices)
                    {
                        // Generate a new unique Guid for each choice
                        choice.Id = Guid.NewGuid();
                        choice.QuestionId = questionId;

                        var query = @"INSERT INTO Choices (Id, ChoiceText, QuestionId) VALUES (@Id, @ChoiceText, @QuestionId)";
                        await db.ExecuteAsync(query, choice);
                    }

                    return Ok(choices);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}