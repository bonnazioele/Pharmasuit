using Pharmasuit.Models;

namespace Pharmasuit.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public List<ProductOrder> Medicines { get; set; } = new();
        public decimal TotalPrice { get; set; }
        
    }
}

public class ProductOrder
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public Product Product { get; set; }
}
