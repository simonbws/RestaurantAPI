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
        int Create(CreateRestaurantDTO dto);
        PagedResult<RestaurantDTO> GetAll(RestaurantQuery query);
        RestaurantDTO GetById(int id);
        void Delete(int id);
        void Update(int id, UpdateRestaurantDTO dto);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
            
        }

        public void Update(int id, UpdateRestaurantDTO dto)
        {
            
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }
            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;

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

        public void Delete(int id)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");

            var restaurant = _dbContext
              .Restaurants
              .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant,
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
        public PagedResult<RestaurantDTO> GetAll(RestaurantQuery query)
        {
            var baseQuery = _dbContext
                .Restaurants
                //na tej podstawie Entity Fr. dolacza odpowiednie tabele do wyniku zapytania
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                || r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));
            //w takiej sytuacji ms sql stworzy zapytanie ktore pobierze z bazy danych resutracje i zwroci pod postacia restaurants
            var restaurants = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();
            var totalItemsCount = baseQuery.Count();
            //obiekt restaurantsDtos musi byc zmapowany na podst restrauracji z DB
            //jako generyczny parametr typ na ktory mapujemy i jako arguement zrodlo z ktorego chcemy mapowac (restauracje)
            var restaurantsDtos = _mapper.Map<List<RestaurantDTO>>(restaurants);
            //obiekt typu PagedResult dla typu RestaurantDTO
            var result = new PagedResult<RestaurantDTO>(restaurantsDtos,totalItemsCount, query.PageSize, query.PageNumber);
            return restaurantsDtos;
        }
        public int Create(CreateRestaurantDTO dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        } 
    }
}
