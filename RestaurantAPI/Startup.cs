using RestaurantAPI.Entities;

namespace RestaurantAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllers();
            services.AddDbContext<RestaurantDbContext>(); //dodanie db contextu
            services.AddScoped<RestaurantSeeder>(); //rejestracja serwisu seedującego
        }
    

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder)
    {
        seeder.Seed(); //po wywolaniu tej metody, dane zostana zseedowane
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        }
    }
}
