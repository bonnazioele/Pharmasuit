using System.ComponentModel.DataAnnotations;

namespace Pharmasuit.Dto
{
    public class OrderCreateDto
    {
        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string Address { get; set; }

        public List<OrderProductDto> Medicines { get; set; } = new();
    }

    public class OrderProductDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}