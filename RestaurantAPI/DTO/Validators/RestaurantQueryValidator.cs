using FluentValidation;

namespace RestaurantAPI.DTO.Validators
{
    public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
    {
        private int[] allowedPageSizes = new[] { 5, 10, 15 };    
        public RestaurantQueryValidator()
        {
            //strona musi byc co najmniej rowna 1
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
            //ta wartosc musi byc konkretna z jakiejs listy ktora dopuszczamy
            RuleFor(r => r.PageSize).Custom((value, context) =>
            {
                if (!allowedPageSizes.Contains(value))
                {
                    //wyswietlamy wartosci po przecinku (string Join)
                    context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", allowedPageSizes)}]");
                }
            });
        }

    }
}
