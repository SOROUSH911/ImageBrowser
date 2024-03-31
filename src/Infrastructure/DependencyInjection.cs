using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Domain.Constants;
using ImageBrowser.Infrastructure.Data;
using ImageBrowser.Infrastructure.Data.Interceptors;
using ImageBrowser.Infrastructure.Extensiosn;
using ImageBrowser.Infrastructure.Identity;
using ImageBrowser.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);


        services.AddAuthorizationBuilder();

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ITokenFactory, TokenFactory>();
        //services.AddFluentEmailWithSmtp(configuration);
        //services.AddTransient<IEmailService, EmailService>();
        //services.AddTransient<IEmailSender, EmailSender>();
        services.AddTransient<IFileProvider, FileProvider>();
        services.AddTransient<IAppUserIdService, AppUserIdService>();

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator));

        //services.AddAntiforgery();

        //services.AddIdentityApiEndpoints<IdentityUser>(opt =>
        //{
        //    opt.Password.RequiredLength = 8;
        //    opt.User.RequireUniqueEmail = true;
        //    opt.Password.RequireNonAlphanumeric = false;
        //    opt.SignIn.RequireConfirmedEmail = true;
        //});
        return services;
    }
}
