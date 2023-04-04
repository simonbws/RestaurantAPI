using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTO
{
    public class CreateRestaurantDTO
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool HasDelivery { get; set; }
        public string? Email { get; set; }
        public string? TelephoneNumber { get; set; }
        [Required]
        [MaxLength(50)]
        public string City { get; set; }
        [Required]
        [MaxLength(50)]
        public string Street { get; set; }
        public string? PostalCode { get; set; }
    }
}
