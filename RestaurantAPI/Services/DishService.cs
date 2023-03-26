using AutoMapper;
using RestaurantAPI.DTO;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDTO dto);
    }

    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _context;
        private readonly IMapper _mapper;
        public DishService(RestaurantDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public int Create(int restaurantId, CreateDishDTO dto)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(x => x.Id == restaurantId);
            if (restaurant is null)
              throw new NotFoundException("Restaurant not found");
            
            //aby dodac nowe dania do konkretnej restauracji musimy zmapowac Dish z z CreateDishDTO
            var dishEntity = _mapper.Map<Dish>(dto);

            dishEntity.RestaurantId = restaurantId;

            _context.Dishes.Add(dishEntity);
            _context.SaveChanges();

            return dishEntity.Id;


        }
    }
}
