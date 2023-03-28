using RestaurantAPI.DTO;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Services
{
    /// <summary>
    /// Ten interfejs, ktory pozniej wstrzykniemy bedzie mial deklaracje jednej metody na podstawie DTO wyslanego od klienta
    /// </summary>
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDTO dto);
    }

    /// <summary>
    /// Serwis odpowiedzialny za tworzenie nowych kont 
    /// oraz za logowanie uzytkowników
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly RestaurantDbContext _context;

        public AccountService(RestaurantDbContext context)
        {
            _context = context;
        }

        public void RegisterUser(RegisterUserDTO dto)
        {
            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                RoleId = dto.RoleId
                //hasla nie dodajemy bo pierwsze musimy je zahashować!
            };
            //a zeby dodac uzytkownika do tabeli, musimy odniesc sie do kontekstu bazy danych, ktory musimy wstrzyknac przez konstruktor do AccountService
            //dodajemy nowo utworzonego uzytkownika do tabeli Users korzystajac z kontekstu
            _context.Users.Add(newUser);
            _context.SaveChanges();
        }
    }
}
