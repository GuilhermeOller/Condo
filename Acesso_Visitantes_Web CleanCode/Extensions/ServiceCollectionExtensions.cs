using Acesso_Moradores_Visitantes.Repositorys.Implementations;
using Acesso_Moradores_Visitantes.Repositorys.Interfaces;
using Acesso_Moradores_Visitantes.Services.Implementations;
using Acesso_Moradores_Visitantes.Services.Interfaces;

namespace Acesso_Moradores_Visitantes.Extensions
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
