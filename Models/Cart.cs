namespace Pharmasuit.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public virtual ICollection<CartItem> Items { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

}
