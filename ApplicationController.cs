using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace SiparisSistemi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly string connectionString = "server=127.0.0.1;port=3306;database=Polisan;user=basar;password=basar123;";

        [HttpGet("testConnection")]
        public IActionResult TestConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
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

        // Veritabanından tüm siparişleri (başvuruları) çekip frontend'e gönderiyoruz
        [HttpGet("getAllApplications")]
        public IActionResult GetAllApplications()
        {
            List<Application> applications = new List<Application>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Applications";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            applications.Add(new Application
                            {
                                UserID = reader.GetInt32("UserID"),   
                                ApplicationDate = reader.GetDateTime("ApplicationDate"),
                                ProductID = reader.GetInt32("ProductID"),
                                Quantity = reader.GetInt32("Quantity"),
                                Price = reader.GetDecimal("Price"),
                                ApplicantName = reader.GetString("ApplicantName"),
                                IsApproved = reader.GetBoolean("IsApproved"),
                                ShippingAddress = reader.GetString("ShippingAddress")
                            });
                        }
                    }
                }
            }

            return Ok(applications);
        }
    }

    // Application sınıfı
    public class Application
    {
        public int UserID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ApplicantName { get; set; }
        public bool IsApproved { get; set; }
        public string ShippingAddress { get; set; }
    }
}
