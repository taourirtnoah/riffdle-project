using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Riffdle.Data;
using Xunit;

namespace Riffdle.Tests;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public ApiWebApplicationFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<RiffdleDbContext>>();
            services.RemoveAll<RiffdleDbContext>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DbConnection>();
            services.RemoveAll<IDbContextFactory<RiffdleDbContext>>();

            services.AddSingleton<DbConnection>(_connection);
            services.AddDbContext<RiffdleDbContext>((sp, options) =>
            {
                options.UseSqlite(sp.GetRequiredService<DbConnection>());
            });
            services.AddDbContextFactory<RiffdleDbContext>((sp, options) =>
            {
                options.UseSqlite(sp.GetRequiredService<DbConnection>());
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                options.DefaultForbidScheme = TestAuthHandler.SchemeName;
            }).AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.SchemeName,
                _ => { });
        });
    }

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        _connection.Dispose();
        return Task.CompletedTask;
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RiffdleDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        RiffdleSeeder.Seed(db);
    }
}
