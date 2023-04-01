using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public enum ResourceOperation
    {
        Create,
        Read,
        Update,
        Delete
    }

    public class ResourceOperationRequirement : IAuthorizationRequirement
    {
        //w tym konstruktorze ustalamy wartosc dla tej wlasciwosci, dlatego brak set w prop ResourceOperation
        public ResourceOperationRequirement(ResourceOperation resourceOperation)
        {
            ResourceOperation = resourceOperation;
        }

        public ResourceOperation ResourceOperation { get; }
    }
}
