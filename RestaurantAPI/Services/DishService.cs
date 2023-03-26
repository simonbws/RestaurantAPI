using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTO;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDTO dto);
        DishDTO GetById(int restaurantId, int dishId);
        List<DishDTO> GetAll(int restaurantId);
        void RemoveAll(int restaurantId);
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
            var restaurant = GetRestaurantById(restaurantId);

            //aby dodac nowe dania do konkretnej restauracji musimy zmapowac Dish z z CreateDishDTO
            var dishEntity = _mapper.Map<Dish>(dto);

            dishEntity.RestaurantId = restaurantId;

            _context.Dishes.Add(dishEntity);
            _context.SaveChanges();

            return dishEntity.Id;


        }

        public DishDTO GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            //pobieranie dania z bazy
            var dish = _context.Dishes.FirstOrDefault(d => d.Id == dishId);
            //jesli encja nie istnieje w bazie danych albo nie jest polaczona z konkretna restauracja o ktora odpytywal klient
            if (dish is null || dish.RestaurantId != restaurantId)
            {
                throw new NotFoundException("Dish not found");
            }
            //a jesli danie zostanie poprawnie znalezione i polaczone z konkretna restauracja mozemy je zmapowac do modelu dto.
            //wtedy chcemy zmapowac z encji dish na model DishDTO
            //jako parametr generyczny DishDTO a jako zrodlo encja dish
            var dishDto = _mapper.Map<DishDTO>(dish);
            return dishDto;
        }

        public List<DishDTO> GetAll(int restaurantId)
        {

            var restaurant = GetRestaurantById(restaurantId);
            var dishDtos = _mapper.Map<List<DishDTO>>(restaurant.Dishes);

            return dishDtos;
        }

        public void RemoveAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            //jesli restauracja istnieje, aby usunac, to na kontekscie baz danych wywolujemy metode remove range
            _context.RemoveRange(restaurant.Dishes);
            _context.SaveChanges();
        }
        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _context
               .Restaurants
               //chcemy tutaj wywolac dania z bazy danych, za pomoca include
               .Include(r => r.Dishes)
               .FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant is null)

                throw new NotFoundException("Restaurant not found");

            return restaurant;
        }
    }
}
