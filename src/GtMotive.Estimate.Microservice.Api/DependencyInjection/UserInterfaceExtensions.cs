using GtMotive.Estimate.Microservice.Api.UseCases.AddVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.AddVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;

using Microsoft.Extensions.DependencyInjection;

namespace GtMotive.Estimate.Microservice.Api.DependencyInjection
{
    public static class UserInterfaceExtensions
    {
        public static IServiceCollection AddPresenters(this IServiceCollection services)
        {
            services.AddScoped<AddVehiclePresenter>();
            services.AddScoped<IOutputPortStandard<AddVehicleOutput>>(sp => sp.GetRequiredService<AddVehiclePresenter>());

            services.AddScoped<GetAvailableVehiclesPresenter>();
            services.AddScoped<IOutputPortStandard<GetAvailableVehiclesOutput>>(sp => sp.GetRequiredService<GetAvailableVehiclesPresenter>());

            services.AddScoped<RentVehiclePresenter>();
            services.AddScoped<IRentVehicleOutputPort>(sp => sp.GetRequiredService<RentVehiclePresenter>());

            services.AddScoped<ReturnVehiclePresenter>();
            services.AddScoped<IReturnVehicleOutputPort>(sp => sp.GetRequiredService<ReturnVehiclePresenter>());

            return services;
        }
    }
}
