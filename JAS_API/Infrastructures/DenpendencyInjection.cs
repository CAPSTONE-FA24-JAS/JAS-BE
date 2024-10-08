using Application;
using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Application.Utils;
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
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBidLimitRepository, BidLimitRepository>();
            services.AddScoped<IBidLimitService, BidLimitService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IImageValuationRepository, ImageValuationRepository>();
            services.AddScoped<IValuationRepository, ValuationRepository>();
            services.AddScoped<IValuationService, ValuationService>();
            services.AddScoped<IAddressToShipService,AddressToShipService>();
            services.AddScoped<IAddressToShipRepository, AddressToShipRepository>();
            services.AddScoped<IWardService, WardService>();
            services.AddScoped<IWardRepository, WardRepository>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<IDistrictRepository,DistrictRepository>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<IValuationDocumentRepository, ValuationDocumentRepository>();
            services.AddScoped<ICurrentTime, CurrentTime>();

            services.AddDbContext<AppDbContext>(option =>
            {
                option.UseLazyLoadingProxies().UseNpgsql(databaseConnection);
            });
            services.AddAutoMapper(typeof(Mapper).Assembly);
            //services.AddSingleton<UserManager<Account>>();
            return services;

        }
    }
}
