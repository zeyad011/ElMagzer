using ElMagzer.Core.Repositories;
using ElMagzer.Core.Services;
using ElMagzer.Repository;
using ElMagzer.Service;
using ElMagzer.Shared.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;

namespace ElMagzer.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IDeviceServices, DeviceServices>();
            services.AddScoped<IFrontServices, FrontServices>();
            services.AddScoped<IFlutterServices, FlutterService>();
            services.AddSignalR()
                  .AddHubOptions<CowHub>(options =>
                  {
                      options.EnableDetailedErrors = true;
                  })
                  .AddJsonProtocol();
            services.AddHttpClient();
            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", builder =>
                {
                    builder.SetIsOriginAllowed((host) => true)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });


            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value?.Errors?.Count() > 0)
                                                         .SelectMany(P => P.Value?.Errors ?? Enumerable.Empty<ModelError>())
                                                         .Select(E => E.ErrorMessage ?? "Unknown error")
                                                         .ToArray();
                    var validationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });


            return services;
        }
    }
}
