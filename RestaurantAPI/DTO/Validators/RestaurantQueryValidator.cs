using FluentValidation;
using RestaurantAPI.Entities;

namespace RestaurantAPI.DTO.Validators
{
    public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
    {
        private int[] allowedPageSizes = new[] { 5, 10, 15 };
        private string[] allowedSortByColumnNames = { nameof(Restaurant.Name), nameof(Restaurant.Category), nameof(Restaurant.Description), };
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
            //klient nie poda innej kolumny przed ktora chcialby posortowac
            RuleFor(r => r.SortBy)
                //sprawdzamy czy wartosc jest nullem albo pusta albo czy nasza tablica z dozwolonymi kolumnami zawiera wartosc(value)
                .Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
                //jesli warunek nie bedzie spelniony - blad walidacji z metoda WithMessage
                .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
        }

    }
}
