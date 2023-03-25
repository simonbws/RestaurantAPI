using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTO;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
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
            
            _restaurantService.Update(id, dto);
            return Ok();

        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
           _restaurantService.Delete(id);
            return NoContent(); //oznacza 200 ale nic nie zwraca
        }

        [HttpPost]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDTO dto)
        {  
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
            
            return Ok(restaurant);
        }
    }
}
