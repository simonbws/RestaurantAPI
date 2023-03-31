using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.DTO;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantAPI.Services
{
    /// <summary>
    /// Ten interfejs, ktory pozniej wstrzykniemy bedzie mial deklaracje jednej metody na podstawie DTO wyslanego od klienta
    /// </summary>
    public interface IAccountService
    {
        string GenerateJwt(LoginDTO dto);
        void RegisterUser(RegisterUserDTO dto);
    }

    /// <summary>
    /// Serwis odpowiedzialny za tworzenie nowych kont 
    /// oraz za logowanie uzytkowników
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly RestaurantDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;

        public AccountService(RestaurantDbContext context, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }

        public string GenerateJwt(LoginDTO dto)
        {
            // pierwsze musimy pobrac uzytkownika z db, jezeli taki uzytkownik istnieje
            //to musimy zahashowac haslo i porownac je z tym ktore istnieje w db
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (user is null)
            {
                throw new BadRequestException("Invalid username or password");
            }

            //weryfikacja hashu hasla do tego w bazie danych z tym ktory wyslal uzytkownik
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid username or password");
            }
            //jesli uzytkownik podal prawidlowe haslo, generujemy token z konretnymi wlasciwosciami (claims) np nazwa, numer
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")),
                
            };

            if (!string.IsNullOrEmpty(user.Nationality))
            {
                claims.Add(
                    new Claim("Nationality", user.Nationality)
                    );
            }
            //generacja klucza prywatnego z appsetings.json (po wywolaniu singleton z startup.cs)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            //kredencjały
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            //utworzenie tokenu
            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            //generacja tokenu do typu string
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
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

            //wywolujemy metode hashPassword dla Usera do ktorej przekazemy wartosc modelu dto, 
            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
            //przed zapisem na obiekcie new User ustalimy wartosc PasswordHash która bedzie równa hashPassword
            newUser.PasswordHash = hashedPassword;
            _context.Users.Add(newUser);
            _context.SaveChanges();
        }
    }
}
