using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTO;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/account")]
    [ApiController] //waliduje automatycznie model wyslany przez klienta(RegisterUserDTO)
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        //na poziomie podstawowej sciezki do kontrolera bedzie to rowniez register
        [HttpPost("register")]
        //do tej metody chcemy przeslac informacje o nowym uzytkowniku
        //wiec z ciala zapytania frombody przesylamy model RegisterUserDTO
        public ActionResult RegisterUser([FromBody]RegisterUserDTO dto)
        {
            //wywolamy metode z serwisu do ktorego przekazemy model wyslany od klienta
            _accountService.RegisterUser(dto);
            //jesli walidacja tego zapytania powiedzie sie, a serwis utworzy konto uzytkownika w db to zwracamy kod 200
            return Ok();
        }
    }
}
