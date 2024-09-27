public class Order
{
    public int ApplicationID { get; set; }
    public int ProductID { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string ApplicantName { get; set; }
    public string ShippingAddress { get; set; }
    public bool IsApproved { get; set; }
}
