using System;

namespace SharedKernel.DTOs.Customers
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        
        // From Users Table
        public string CustomerName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }

        // From Customers Table
        public string ShopName { get; set; } = string.Empty;
        public string ShopAddress { get; set; } = string.Empty;
        public decimal Bhada { get; set; }
        public int? DisplayIndex { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
