using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Authorization;
using RestaurantAPI.DTO;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using System.Security.Claims;
using System.Security.Cryptography;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDTO dto, int userId);
        IEnumerable<RestaurantDTO> GetAll();
        RestaurantDTO GetById(int id);
        void Delete(int id, ClaimsPrincipal user);
        void Update(int id, UpdateRestaurantDTO dto, ClaimsPrincipal user);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger, IAuthorizationService authorizationService )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        public void Update(int id, UpdateRestaurantDTO dto, ClaimsPrincipal user)
        {
            
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }
            var authorizationResult = _authorizationService.AuthorizeAsync(user, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            //jesli autoryzacja nie powiodla sie
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;

            _dbContext.SaveChanges();
            
        }

        public void Delete(int id, ClaimsPrincipal user)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");

            var restaurant = _dbContext
              .Restaurants
              .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(user, restaurant,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }
            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();

            


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
                throw new NotFoundException("Restaurant not found");
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
        public int Create(CreateRestaurantDTO dto, int userId)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = userId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }
    }
}
