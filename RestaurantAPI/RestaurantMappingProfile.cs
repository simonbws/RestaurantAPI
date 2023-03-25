using AutoMapper;
using RestaurantAPI.DTO;
using RestaurantAPI.Entities;

namespace RestaurantAPI
{
    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile()
        {
            CreateMap<Restaurant, RestaurantDTO>()
            .ForMember(m => m.City, c => c.MapFrom(s => s.Address.City))
            .ForMember(m => m.Street, c => c.MapFrom(s => s.Address.Street))
            .ForMember(m => m.PostalCode, c => c.MapFrom(s => s.Address.PostalCode));

            CreateMap<Dish, DishDTO>();

            CreateMap<CreateRestaurantDTO, Restaurant>()
                .ForMember(r=>r.Address, c=>c.MapFrom(dto=> new Address() { City = dto.City, PostalCode = dto.PostalCode, Street = dto.Street}));
        }
        
    }
}
