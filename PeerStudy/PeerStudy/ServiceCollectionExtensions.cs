using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Infrastructure.AppDbContext;
using PeerStudy.Infrastructure.Interfaces;
using PeerStudy.Infrastructure.Services;
using PeerStudy.Infrastructure.UnitOfWork;
using PeerStudy.Services;
using PeerStudy.Services.Interfaces;

namespace PeerStudy
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(
                  connectionString)
            );
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            RegisterDomainServices(services);
            RegisterClientServices(services);
            RegisterServicesFromInfrastructure(services);
        }

        private static void RegisterDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void RegisterClientServices(this IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();

            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, PeerStudyAuthStateProvider>();

            services.AddScoped<IAuthService, AuthService>();
        }

        private static void RegisterServicesFromInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IImageService, ImageService>();
        }
    }
}
