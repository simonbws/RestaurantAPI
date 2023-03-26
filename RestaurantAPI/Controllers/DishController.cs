using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RestaurantAPI.DTO;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        //wstrzykujemy referencje do typu IDishService
        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpPost]
        public ActionResult Post([FromRoute] int restaurantId, [FromBody] CreateDishDTO dto)
        {
            var newDishId = _dishService.Create(restaurantId, dto);
            return Created($"api/{restaurantId}/dish/{newDishId}", null);
        }
    }
}
