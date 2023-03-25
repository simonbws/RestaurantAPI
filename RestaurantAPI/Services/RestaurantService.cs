using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTO;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDTO dto);
        IEnumerable<RestaurantDTO> GetAll();
        RestaurantDTO GetById(int id);
        bool Delete(int id);
        public bool Update(int id, UpdateRestaurantDTO dto);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public bool Update(int id, UpdateRestaurantDTO dto)
        {
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
            {
                return false;
            }
            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var restaurant = _dbContext
              .Restaurants
              .FirstOrDefault(r => r.Id == id);
            if (restaurant is null)
            {
                return false;
            }
            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();

            return true;


        }
        public RestaurantDTO GetById(int id)
        {
            var restaurant = _dbContext
               .Restaurants
               .Include(r => r.Address)
               .Include(r => r.Dishes)
               .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
            {
                return null;
            }

            var result = _mapper.Map<RestaurantDTO>(restaurant);
            return result;
        }
        public IEnumerable<RestaurantDTO> GetAll()
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
            return restaurantsDtos;
        }
        public int Create(CreateRestaurantDTO dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }
    }
}
