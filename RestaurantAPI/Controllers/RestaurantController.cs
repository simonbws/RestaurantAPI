using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTO;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    [Authorize]
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
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Manager")]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDTO dto)
        {
            //pobieramy parametr
           
            var id = _restaurantService.Create(dto);
            return Created($"/api/restaurant/{id}", null);
        }
        //akcja ktora bedzie odpowiadac na zapytania get i zwroci wszystkie
        //restauracje z bazy danych do klienta
        [HttpGet]
        [Authorize(Policy ="CreatedAtleast2Restaurants")]
        public ActionResult<IEnumerable<RestaurantDTO>> GetAll([FromQuery]string searchPhrase, [FromQuery]int pageNumber, [FromQuery]int pageSize)
        {
            var restaurantDtos = _restaurantService.GetAll(searchPhrase);

            return Ok(restaurantDtos);
        }
        [HttpGet("{id}")]
        [AllowAnonymous] //ta akcja zezwala na zapytania bez naglowka autoryzacji
        public ActionResult<RestaurantDTO> Get([FromRoute] int id)
        {

            var restaurant = _restaurantService.GetById(id);
            
            return Ok(restaurant);
        }
    }
}
