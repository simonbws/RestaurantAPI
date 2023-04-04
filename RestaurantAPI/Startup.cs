using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Authorization;
using RestaurantAPI.DTO;
using RestaurantAPI.DTO.Validators;
using RestaurantAPI.Entities;
using RestaurantAPI.Middleware;
using RestaurantAPI.Services;
using System.Text;

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
            var authenticationSettings = new AuthenticationSettings();
            //bindowanie = polaczenie wartosci z tej sekcji do zmiennej authenticationSettings
            Configuration.GetSection("Authentication").Bind(authenticationSettings);

            services.AddSingleton(authenticationSettings);
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });
            services.AddAuthorization(options =>
            {
                //sprawdzamy czy dany claim (Nationality) istnieje bez sprawdzania wartosci - musi byc to spelnione w autoryzacji
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality"));
                options.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinimumAgeRequirement(29)));
                options.AddPolicy("CreatedAtleast2Restaurants", builder => builder.AddRequirements(new CreatedMultipleRestaurantsRequirement(2)));


                //mozliwe sa rownie naglowki z wartosciami (German, Polish)
                // options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Polish"));
            });
            services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantsRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
            services.AddControllers().AddFluentValidation();
            services.AddDbContext<RestaurantDbContext>(); //dodanie db contextu
            services.AddScoped<RestaurantSeeder>(); //rejestracja serwisu seedującego
            //assembly czyli zrodlo typu w ktorym automapper przeszuka wszystkie typy do potrzebnej konfiguracji
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IValidator<RegisterUserDTO>, RegisterUserDTOValidator>();
            services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
            services.AddScoped<RequestTimeMiddleware>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen();
            services.AddCors(options =>
            {
                options.AddPolicy("FrontEndClient", builder =>

                    builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(Configuration["AllowedOrigins"])
                    );
            });
        }
    

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder)
    {
        app.UseStaticFiles();
        app.UseCors("FrontEndClient");
        seeder.Seed(); //po wywolaniu tej metody, dane zostana zseedowane
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<RequestTimeMiddleware>();
        app.UseAuthentication();
        app.UseHttpsRedirection();
        app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
            });
            
        app.UseRouting(); // api mapuje zapytanie do konkretnej akcji w kontrolerze
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        }
    }
}
