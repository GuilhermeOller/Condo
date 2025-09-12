using Project_Condo.Repositorys.Implementations;
using Project_Condo.Repositorys.Interfaces;
using Project_Condo.Services.Implementations;
using Project_Condo.Services.Interfaces;

namespace Project_Condo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IGeralRepository, GeralRepository>();
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<IVisitanteRepository, VisitanteRepository>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ZapiService>();
            services.AddScoped<Geral>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IVisitanteService, VisitanteService>();
            return services;
        }
    }
}
