using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTO;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase

    {
        //wstrzykniecie dbcontex w celu pobrania z bazy danych restauracji
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        public RestaurantController(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        //akcja ktora bedzie odpowiadac na zapytania get i zwroci wszystkie
        //restauracje z bazy danych do klienta
        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDTO>> GetAll()
        {
            //w takiej sytuacji ms sql stworzy zapytanie ktore pobierze z bazy danych resutracje i zwroci pod postacia restaurants
            var restaurants = _dbContext
                .Restaurants
                //na tej podstawie Entity Fr. dolacza odpowiednie tabele do wyniku zapytania
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .ToList();
            //obiekt restaurantsDtos musi byc zmapowany na podst restrauracji z DB
            //jako generyczny parametr typ na ktory mapujemy i jako arguement zrodlo z ktorego chcemy mapowac (restauracje)
            var restaurantsDtos = _mapper.Map<List<RestaurantDTO>>(restaurants);

            return Ok(restaurantsDtos);
        }
        [HttpGet("{id}")]
        public ActionResult<RestaurantDTO> Get([FromRoute] int id)
        {
            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
            {
                return NotFound();
            }

            var restaurantDto = _mapper.Map<RestaurantDTO>(restaurant);
            return Ok(restaurantDto);
        }
    }
}
