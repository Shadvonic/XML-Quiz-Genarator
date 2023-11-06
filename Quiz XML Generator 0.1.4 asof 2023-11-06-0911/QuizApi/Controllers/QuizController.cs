using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly string _connectionString;

        public QuizController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default");
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> SaveQuizData([FromBody] string xmlData)
        {
            try
            {
                var quizId = Guid.NewGuid();

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Insert the XML string into the database
                    await connection.ExecuteAsync(
                        "INSERT INTO Quiz (Id, XmlData) VALUES (@Id, @XmlData)",
                        new { Id = quizId, XmlData = xmlData },
                        commandType: CommandType.Text
                      
                    );
                }

                return quizId;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
