using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTO;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase

    {
        //wstrzykniecie dbcontex w celu pobrania z bazy danych restauracji
        private readonly IRestaurantService _restaurantService;
        

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }
        [HttpPut("{id}")]
        //przyjmuje od klienta poprzez cialo zapytania(frombody)
        public ActionResult Update([FromBody] UpdateRestaurantDTO dto, [FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var isUpdated = _restaurantService.Update(id, dto);
            if (!isUpdated)
            {
                return NotFound();
            }
            return Ok();

        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
           var isDeleted = _restaurantService.Delete(id);

            if (isDeleted)
            {
                return NoContent(); //oznacza 200 ale nic nie zwraca
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDTO dto)
        {
            //sprawdza czy atrybuty walidacji sa poprawnie wprowadzone
            //np [Required]
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = _restaurantService.Create(dto);
            return Created($"/api/restaurant/{id}", null);
        }
        //akcja ktora bedzie odpowiadac na zapytania get i zwroci wszystkie
        //restauracje z bazy danych do klienta
        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDTO>> GetAll()
        {
            var restaurantDtos = _restaurantService.GetAll();

            return Ok(restaurantDtos);
        }
        [HttpGet("{id}")]
        public ActionResult<RestaurantDTO> Get([FromRoute] int id)
        {

            var restaurant = _restaurantService.GetById(id);
            if (restaurant is null)
            {
                return NotFound();
            }
            return Ok(restaurant);
        }
    }
}
