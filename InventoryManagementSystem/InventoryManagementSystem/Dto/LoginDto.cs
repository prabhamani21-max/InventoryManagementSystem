using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.DTO
{
    public class LoginDto
    {
        [Required]
        public string? Identifier { get; set; } // Can be Email or Phone Number

        [Required]
        public string Password { get; set; }
    }
}