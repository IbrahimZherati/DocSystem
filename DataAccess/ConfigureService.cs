using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace DataAccess
{
    public static class ConfigureService
    {
        public static IServiceCollection AddInfratructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<AppDbContext>(op =>
            op.UseSqlite(configuration.GetConnectionString("SQLite"))
            );
            services.AddScoped(typeof(IRepo<,>), typeof(Repo<,>));
            services.AddScoped(typeof(IRepo<>), typeof(Repo<>));
            return services;
        }
    }
}
