using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using TeacherReviews.API.Authentication;
using TeacherReviews.Domain;

namespace TeacherReviews.API.Tests;

public class ApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection>? _serviceOverride;

    public ApplicationFactory(Action<IServiceCollection>? serviceOverride = null)
    {
        _serviceOverride = serviceOverride;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        if (_serviceOverride is not null)
        {
            builder.ConfigureServices(_serviceOverride);
        }

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));

            services.Remove(dbContextDescriptor!);

            dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<UsersDbContext>));

            services.Remove(dbContextDescriptor!);

            services.AddDbContext<ApplicationDbContext>(opts =>
            {
                opts
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseInMemoryDatabase("db");
            });

            services.AddDbContext<UsersDbContext>(options =>
            {
                options
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseInMemoryDatabase("usersdb");
            });
        });
        builder.UseEnvironment("Test");
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);

        var authHeaderValue = Convert.ToBase64String("AbsolutelyUnpredictableUsername:MegaPassword"u8.ToArray());

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthentication.SchemeName, authHeaderValue);
    }
}