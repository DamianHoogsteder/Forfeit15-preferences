using Forfeit15.Postgres.Contexts;
using Forfeit15.Postgres.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Forfeit15.Preferences.Postgres.Extensions;

public static class HostExtensions
{
    public static void MigrateDatabases(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PreferenceDbContext>();
         
        // Migrate the database to the latest version
        host.MigrateDatabase<PreferenceDbContext>();
    }
}