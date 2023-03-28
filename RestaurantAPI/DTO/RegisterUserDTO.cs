using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTO
{
    public class RegisterUserDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        public string Nationality { get; set; }
        public DateTime? DateOfBirth { get; set; }

        //domyslnie uzytkownik bedzie w roli user czyli id = 1
        public int RoleId { get; set; } = 1;
    }
}
