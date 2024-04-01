using Azure.Identity;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Infrastructure.Configurations;
using ImageBrowser.Infrastructure.Data;
using ImageBrowser.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {


        //this might be adding the html page for database related exceptions like migration.
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IUser, CurrentUser>();

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddExceptionHandler<CustomExceptionHandler>();

        //services.AddRazorPages();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();

        services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "ImageBrowser API";

            // Add JWT
            configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });

            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });

        return services;
    }

    public static IServiceCollection AddKeyVaultIfConfigured(this IServiceCollection services, ConfigurationManager configuration)
    {
        var keyVaultUri = configuration["KeyVaultUri"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }

        return services;
    }

    public static IServiceCollection AddSsmParametersIfConfigured(this IServiceCollection services, ConfigurationManager configuration)
    {
        var ssmParameterPath = configuration["SsmParameterPath"];
        if (!string.IsNullOrWhiteSpace(ssmParameterPath))
        {
            // Add the AWS Systems Manager Parameter Store configuration source
            configuration.AddSystemsManager(configureSource =>
            {
                configureSource.Path = ssmParameterPath;
                //configureSource.Prefix = ssmParameterPath;
                configureSource.ReloadAfter = TimeSpan.FromMinutes(5); // Set the reload interval
                configureSource.AwsOptions = configuration.GetAWSOptions(); // Use the default AWS options
            });
        }

        return services;
    }

    public static IServiceCollection AddConfigurations(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<AmazonConfiguration>(opts => configuration.GetSection("AmazonConfiguration").Bind(opts));
        var tokenConfig = JsonConvert.DeserializeObject<TokenConfiguration>(configuration.GetSection("TokenConfiguration").Value);
        services.Configure<TokenConfiguration>(opts =>
        {
            opts.VarifiedSecretUrl = tokenConfig.VarifiedSecretUrl;
            opts.SecretToken = tokenConfig.SecretToken;
            opts.EncRefPassword = tokenConfig.EncRefPassword;
        });

        return services;
    }


}
