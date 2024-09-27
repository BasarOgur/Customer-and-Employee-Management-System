using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly string _connectionString = "server=127.0.0.1;port=3306;database=Polisan;user=basar;password=basar123;";

    [HttpGet]
    public IActionResult GetProducts()
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT ProductID, ProductName, Price FROM Products";  // Yalnızca gerekli alanları çekiyoruz
            var command = new MySqlCommand(query, connection);
            var reader = command.ExecuteReader();

            var products = new List<Product>();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    ProductID = reader.GetInt32("ProductID"),
                    ProductName = reader["ProductName"].ToString(), 
                    Price = Convert.ToDecimal(reader["Price"])
                });
            }

            return Ok(products);
        }
    }
}

public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
}
