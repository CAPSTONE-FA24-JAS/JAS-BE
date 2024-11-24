using Application.ViewModels.AccountDTOs;
using Application;
using FluentValidation.AspNetCore;
using FluentValidation;
using Infrastructures;
using System.Diagnostics;
using WebAPI.Middlewares;
using WebAPI.Service;
using Application.Interfaces;
using Application.Services;

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
            services.AddScoped<IBidPriceService, BidPriceService>();
            services.AddScoped<IValuationService, ValuationService>();
            services.AddScoped<IJewelryService, JewelryService>();
            services.AddScoped<IGeneratePDFService, GeneratePdfService>();
            services.AddScoped<ILotService, LotService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddHttpContextAccessor();
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddHostedService<AutoBidBackgroundService>();

            services.AddMemoryCache();
            return services;
        }

    }
}
