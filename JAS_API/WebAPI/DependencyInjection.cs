using Application.ViewModels.AccountDTO;
using Application;
using FluentValidation.AspNetCore;
using FluentValidation;
using Infrastructures;
using System.Diagnostics;
using WebAPI.Middlewares;
using WebAPI.Service;
using Application.Interfaces;

namespace WebAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddFluentValidation();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHealthChecks();
            services.AddSingleton<GlobalExceptionMiddleware>();
            services.AddSingleton<PerformanceMiddleware>();
            services.AddSingleton<Stopwatch>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IClaimsService, ClaimsService>();


            services.AddHttpContextAccessor();
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();


            services.AddMemoryCache();
            return services;
        }

    }
}
