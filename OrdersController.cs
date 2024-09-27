using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly string _connectionString = "server=127.0.0.1;port=3306;database=polisan;user=basar;password=basar123;";

    [HttpPost]
    public IActionResult CreateOrder([FromBody] OrderRequest orderRequest)
{
    using (var connection = new MySqlConnection(_connectionString))
    {
        try
        {
            connection.Open();

            // Username ile UserID'yi buluyoruz
            var getUserIdQuery = "SELECT UserID FROM Users WHERE Username = @Username";
            var userCommand = new MySqlCommand(getUserIdQuery, connection);
            userCommand.Parameters.AddWithValue("@Username", orderRequest.ApplicantName);

            var userID = (int?)userCommand.ExecuteScalar(); 

            if (userID == null)
            {
                return NotFound("User not found.");
            }

            // Siparişi ekliyoruz ve IsApproved alanı varsayılan değerini kullanıyor
            // When creating an order, make sure IsApproved is set to 99
            var query = "INSERT INTO Applications (ApplicationDate, Quantity, Price, ApplicantName, ShippingAddress, ProductID, UserID, IsApproved) " +
                        "VALUES (@ApplicationDate, @Quantity, @Price, @ApplicantName, @ShippingAddress, @ProductID, @UserID, 99)";

            var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", userID);
            command.Parameters.AddWithValue("@ApplicationDate", DateTime.Now);
            command.Parameters.AddWithValue("@ProductID", orderRequest.ProductID);
            command.Parameters.AddWithValue("@Quantity", orderRequest.Quantity);
            command.Parameters.AddWithValue("@Price", orderRequest.Price);
            command.Parameters.AddWithValue("@ApplicantName", orderRequest.ApplicantName);
            command.Parameters.AddWithValue("@ShippingAddress", orderRequest.ShippingAddress);

            command.ExecuteNonQuery();
            return Ok("Order placed successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

   [HttpGet]
    public IActionResult GetOrders()
{
    List<OrderResponse> orders = new List<OrderResponse>();

    using (var connection = new MySqlConnection(_connectionString))
    {
        connection.Open();
        var query = "SELECT ApplicationID, ProductID, Quantity, Price, ApplicantName, ShippingAddress, IsApproved FROM Applications";  

        using (var command = new MySqlCommand(query, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    orders.Add(new OrderResponse
                    {
                        ApplicationID = reader.GetInt32("ApplicationID"),
                        ProductID = reader.GetInt32("ProductID"),
                        Quantity = reader.GetInt32("Quantity"),
                        Price = reader.GetDecimal("Price"),
                        ApplicantName = reader.GetString("ApplicantName"),
                        ShippingAddress = reader.GetString("ShippingAddress"),
                        IsApproved = reader.GetInt32("IsApproved")
                    });
                }
            }
        }
    }

    return Ok(orders); 
}




  [HttpPut("{id}/confirm")]
public IActionResult ConfirmOrder(int id)
{
    using (var connection = new MySqlConnection(_connectionString))
    {
        try
        {
            connection.Open();
            // Update IsApproved to 1 (confirmed)
            var query = "UPDATE Applications SET IsApproved = TRUE WHERE ApplicationID = @id";
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                return Ok("Order confirmed successfully.");
            }
            else
            {
                return NotFound("Order not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

   [HttpPut("{id}/reject")]
public IActionResult RejectOrder(int id)
{
    using (var connection = new MySqlConnection(_connectionString))
    {
        try
        {
            connection.Open();
            var query = "UPDATE Applications SET IsApproved = 0 WHERE ApplicationID = @id";
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                Console.WriteLine("Order rejected successfully in DB");
                return Ok("Order rejected successfully.");
            }
            else
            {
                Console.WriteLine("Order not found in DB");
                return NotFound("Order not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}







        [HttpGet("test")]
    public IActionResult TestOrders()
    {
        return Ok("Orders API is working");
    }


}





public class OrderRequest
{
    public int ProductID { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string ApplicantName { get; set; }  
    public string ShippingAddress { get; set; }
}

public class OrderResponse
{
    public int ApplicationID { get; set; }
    public int ProductID { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string ApplicantName { get; set; }
    public string ShippingAddress { get; set; }
    public int IsApproved { get; set; }  
}
