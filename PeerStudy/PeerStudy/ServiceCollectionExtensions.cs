using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeerStudy.Infrastructure.AppDbContext;

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
    }
}
