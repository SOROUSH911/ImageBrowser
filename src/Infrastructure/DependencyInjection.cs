using System.Text;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Domain.Constants;
using ImageBrowser.Domain.SearchEngine;
using ImageBrowser.Infrastructure.Configurations;
using ImageBrowser.Infrastructure.Data;
using ImageBrowser.Infrastructure.Data.Interceptors;
using ImageBrowser.Infrastructure.Extensiosn;
using ImageBrowser.Infrastructure.Identity;
using ImageBrowser.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{

    private const string CorsName = "api";
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDatabaseContext(configuration);



        

        var tokenConfig = JsonConvert.DeserializeObject<TokenConfiguration>(configuration.GetSection("TokenConfiguration").Value);
        var secretToken = tokenConfig.SecretToken; //configuration.GetSection("TokenConfiguration:SecretToken");
        var secreturl = tokenConfig.VarifiedSecretUrl; //configuration.GetSection("TokenConfiguration:VarifiedSecretUrl");



        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters()
                 {

                     ValidateIssuer = true,
                     ValidateAudience = false,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = secreturl,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretToken)),
                     ClockSkew = TimeSpan.Zero
                 };
                 options.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         var accessToken = context.Request.Query["access_token"];

                         return Task.CompletedTask;
                     }
                 };
             });

        services.AddCors(options =>
        {
            options.AddPolicy(CorsName, builder =>
            {
                builder.WithOrigins()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyOrigin();
            });
        });


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


    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
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

        return services;
    }

}
