using System.ComponentModel.DataAnnotations;

namespace TooLiRent.Models
{
    public class Tool : BaseEntity
    {


        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        public int Stock { get; set; }

        [MaxLength(20)]
        public string? CatalogNumber { get; set; }

        public string Category { get; set; } = string.Empty;

        public string Status { get; set; } = "Available"; // T.ex. "Available", "Rented", "Broken"

        public bool IsAvailable { get; set; } = true;

         
    }
}
