using Application;
using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Domain.Entity;
using Google;
using Infrastructures.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Infrastructures
{
    public static class DenpendencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBidLimitRepository, BidLimitRepository>();
            services.AddScoped<IBidLimitService, BidLimitService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IImageValuationRepository, ImageValuationRepository>();
            services.AddScoped<IValuationRepository, ValuationRepository>();
            services.AddScoped<IValuationService, ValuationService>();
            services.AddScoped<ICurrentTime, CurrentTime>();

            services.AddDbContext<AppDbContext>(option =>
            {
                option.UseNpgsql(databaseConnection);
            });

            services.AddAutoMapper(typeof(Mapper).Assembly);
            //services.AddSingleton<UserManager<Account>>();
            return services;

        }
    }
}
