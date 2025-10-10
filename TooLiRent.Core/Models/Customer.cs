using System.ComponentModel.DataAnnotations;

namespace TooLiRent.Models
{
    public class Customer : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
