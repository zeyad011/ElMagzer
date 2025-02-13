namespace ElMagzer.Extensions
{
    public static class SwaggerServicesExtentions
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
    }
