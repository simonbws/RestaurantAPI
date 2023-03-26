using RestaurantAPI.Entities;
using RestaurantAPI.Middleware;
using RestaurantAPI.Services;

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
            //assembly czyli zrodlo typu w ktorym automapper przeszuka wszystkie typy do potrzebnej konfiguracji
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddSwaggerGen();
        }
    

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder)
    {
        seeder.Seed(); //po wywolaniu tej metody, dane zostana zseedowane
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
            });
            
        app.UseRouting();
       
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        }
    }
}
