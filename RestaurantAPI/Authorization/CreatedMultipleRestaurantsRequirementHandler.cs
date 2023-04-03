using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Entities;
using System.Security.Claims;

namespace RestaurantAPI.Authorization
{
    public class CreatedMultipleRestaurantsRequirementHandler : AuthorizationHandler<CreatedMultipleRestaurantsRequirement>
    {
        private readonly RestaurantDbContext _context;
        public CreatedMultipleRestaurantsRequirementHandler(RestaurantDbContext context)
        {
            _context = context;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatedMultipleRestaurantsRequirement requirement)
        {
            //id zalogowanego uzytkownika i sprawdzamy czy w bazie istnieje konkretna liczba restauracji ktora utworzyl uzytkownik
             var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //liczymy ile restauracji utworzyl dany uzytkownik

             var createdRestaurantsCount = _context
                .Restaurants
                //chcemy pobrac takie restauracje dla ktorych warunek createdbyid = userid
                .Count(r => r.CreatedById == userId);

            //sprawdzamy czy ta wartosc jest taka sama jaka okresla warunek CreatedMultipleRestRequirement

            if (createdRestaurantsCount >= requirement.MinimumRestaurantsCreated)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;

        }
    }
}
