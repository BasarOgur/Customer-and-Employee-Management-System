using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace SiparisSistemi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString = "server=127.0.0.1;port=3306;database=Polisan;user=basar;password=basar123;";

        [HttpGet("testConnection")]
        public IActionResult TestConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    return Ok("Database connection successful");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database connection failed: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginModel loginModel)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM users WHERE Username = @Username AND Password = @Password";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", loginModel.Username);
                command.Parameters.AddWithValue("@Password", loginModel.Password);  

                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var role = reader["Role"].ToString();
                    return Ok(new { Role = role });
                }
                return Unauthorized();
            }
        }


       [HttpPost("signup")]
        public IActionResult SignUp([FromBody] UserSignUpModel signUpModel)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    var query = "INSERT INTO users (Username, Password, Role) VALUES (@Username, @Password, @Role)";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", signUpModel.Username);
                    command.Parameters.AddWithValue("@Password", signUpModel.Password); 
                    command.Parameters.AddWithValue("@Role", signUpModel.Role);

                    command.ExecuteNonQuery();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SignUp: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

       


    }

    public class UserLoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserSignUpModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
