using RestaurantAPI.Entities;

namespace RestaurantAPI
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDbContext _dbContext;
        public RestaurantSeeder(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Roles.Any())
                {
                    //jesli nie ma zadnej roli, pobierzemy z metody getRoles
                    //nastepnie metoda addrange dodamy te 3 role i zapiszemy save changes
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();
                    _dbContext.Restaurants.AddRange(restaurants);
                    _dbContext.SaveChanges();
                }
            }
        }
        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "User"
                },
                new Role()
                {
                    Name = "Manager"
                },
                new Role()
                {
                    Name = "Admin"
                },
            };
            return roles;
        }
        private IEnumerable<Restaurant> GetRestaurants() //kolekcja restauracji
        {
            var restaurants = new List<Restaurant>() //ta metoda zwraca restauracje istniejace w tabeli restaurant
            {
                new Restaurant()
                {
                    Name = "McDonald",
                    Category = "Fast Food",
                    Description= "U.S. fast-food chain, selling mainly burgers, fries and drinks",
                    Email = "contactMc@mc.com",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "McChicken",
                            Price = 10.20M,
                        },
                        new Dish()
                        {
                            Name = "McRoyal",
                            Price = 15.10M,
                        },
                    },
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "Kijowska 5",
                        PostalCode = "32-205"
                    }
                },
                new Restaurant()
                {
                    Name = "BurgerKing",
                    Category = "Fast Food",
                    Description= "U.S. fast-food chain, selling mainly burgers, fries and drinks",
                    Email = "contactBK@bk.com",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "BurgerOne",
                            Price = 10.10M,
                        },
                        new Dish()
                        {
                            Name = "BurgerTwo",
                            Price = 15.40M,
                        },
                    },
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "Długa 6",
                        PostalCode = "32-205"
                    }
                }

            };
            return restaurants;
        }
    }
}
