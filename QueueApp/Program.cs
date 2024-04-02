using System.Text;
using Amazon.S3;
using Humanizer.Configuration;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Infrastructure;
using ImageBrowser.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueueApp;

internal static class Program
{
    static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

   

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {


            //var configuration = hostContext.Configuration;
            //var solrServerAddress = configuration["SolrSettings:SolrServerAddress"];

            //var username = configuration["SOLR_USER"];
            //var password = configuration["SOLR_PASSWORD"];

            //var basicAuthHeader = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
            //    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));


            ConfigurationBuilder builder = new ConfigurationBuilder();
            var ssmParameterPath = hostContext.Configuration["SsmParameterPath"];
            builder.AddSystemsManager(configureSource =>
            {
                configureSource.Path = ssmParameterPath;
                //configureSource.Prefix = ssmParameterPath;
                configureSource.ReloadAfter = TimeSpan.FromMinutes(5); // Set the reload interval
                configureSource.AwsOptions = hostContext.Configuration.GetAWSOptions(); // Use the default AWS options
            });
            IConfigurationRoot newConf = builder.Build();
            services.AddSingleton(newConf);
            services.AddAWSService<IAmazonS3>();
            services.AddDatabaseContext(hostContext.Configuration);
            //services
            //    .Configure<FieldValueTypes>(configuration.GetSection("FieldValueTypes"))
            //    .AddDatabases(configuration)
            //    .AddSolrNet(configuration["SolrSettings:SolrServerAddress"],
            //        options => { options.HttpClient.DefaultRequestHeaders.Authorization = basicAuthHeader; });
            //services.AddSolrNet<VDDataSchema>($"{configuration["SolrSettings:SolrServerAddress"]}/{configuration["SolrSettings:CollectionName"]}",
            //    options => { options.HttpClient.DefaultRequestHeaders.Authorization = basicAuthHeader; })
            services.AddApplicationServices();
            services.AddSingleton(TimeProvider.System);
            services.AddTransient<IAppUserIdService, MockAppUserIdService>();
            services.AddTransient<IUser, MockCurrentUser>();
            services.AddTransient<IFileProvider, FileProvider>();
            //.AddTransient<ISearchEngineServices, SearchEngineServices>()



            services.AddMassTransit(x =>
            {

                x.AddConsumer<ExtractImageAttributesConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", 5673, "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddHostedService<Worker>();
        });
    }
}
