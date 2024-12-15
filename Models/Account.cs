namespace Pharmasuit.Models
{
    public class Account
    {
        
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; } // e.g., Admin, Customer, Pharmacist
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            // Navigation property
            public virtual ICollection<Order> Orders { get; set; }
        
    }


}
