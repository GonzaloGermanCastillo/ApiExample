using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devsu.Data.App_Start;

public static class RdbmsDataWireUp
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DevsuContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DevsuContext")));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
