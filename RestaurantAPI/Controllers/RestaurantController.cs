using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase
    {
        //wstrzykniecie dbcontex w celu pobrania z bazy danych restauracji
        private readonly RestaurantDbContext _dbContext;
        public RestaurantController(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //akcja ktora bedzie odpowiadac na zapytania get i zwroci wszystkie
        //restauracje z bazy danych do klienta
        [HttpGet]
        public ActionResult<IEnumerable<Restaurant>> GetAll()
        {
            //w takiej sytuacji ms sql stworzy zapytanie ktore pobierze z bazy danych resutracje i zwroci pod postacia restaurants
            var restaurant = _dbContext
                .Restaurants   
                .ToList();
            
            return Ok(restaurant);
        }
        [HttpGet("{id}")]
        public ActionResult<Restaurant> Get([FromRoute] int id)
        {
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
            {
                return NotFound();
            }
            return Ok(restaurant);
        }
    }
}
