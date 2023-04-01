using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Entities;
using System.Security.Claims;

namespace RestaurantAPI.Authorization
{
    public class ResourceOperationRequirementHandler : AuthorizationHandler<ResourceOperationRequirement, Restaurant>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, Restaurant restaurant)
        {
            // sprawdzamy czy naszym wymaganiem jest operacja read lub create
            if (requirement.ResourceOperation == ResourceOperation.Read ||
                requirement.ResourceOperation == ResourceOperation.Create)
            {
                context.Succeed(requirement);
            }
            //w przeciwnym wypadku wiemy ze uzytkownik chce albo zmodyfikowac albo usunac restauracje, musimy sprawdzic czy uzytkownik jest tym ktory dana restauracja pobral albo utworzyl
            //pobieramy id uzytkownika z kontekstu autoryzacji
            var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

            //sprawdzamy czy na naszej restauracji wartosc z kolumny createdbyid jest rowna wartosci usedid
            if (restaurant.CreatedById == int.Parse(userId))
            {
                //zezwalamy na autoryzacje
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
