using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Humanizer.Configuration;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Domain.SearchEngine;
using ImageBrowser.Infrastructure;
using ImageBrowser.Infrastructure.Configurations;
using ImageBrowser.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueueApp;
using SolrNet;

internal static class Program
{
    static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }



    public static async Task<List<Parameter>> retrieveParameters(string parameterName)
    {

        var ssmClient = new AmazonSimpleSystemsManagementClient(RegionEndpoint.USEast1); // Change region as needed

        try
        {
            var request = new GetParametersByPathRequest
            {
                Path = parameterName,
                WithDecryption = true // Set to true if the parameter is encrypted
            };

            var response = await ssmClient.GetParametersByPathAsync(request);

            //if (response.Parameters.Count > 0)
            //{
            //    Console.WriteLine($"Parameter Value: {String.Join(", ", response.Parameters.Select(p => p.Value))}");
            //}
            //else
            //{
            //    Console.WriteLine($"Parameter '{parameterName}' not found.");
            //}

            return response.Parameters;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving parameter: {ex.Message}");
            throw new Exception("Something went wrong");
        }
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

            services.AddAWSService<IAmazonSimpleSystemsManagement>();
            ConfigurationBuilder builder = new ConfigurationBuilder();
            var ssmParameterPath = hostContext.Configuration["SsmParameterPath"];
            var parameters = retrieveParameters(ssmParameterPath).Result;

            //builder.AddSystemsManager(configureSource =>
            //{
            //    configureSource.Path = ssmParameterPath;
            //    //configureSource.Prefix = ssmParameterPath;
            //    configureSource.ReloadAfter = TimeSpan.FromMinutes(5); // Set the reload interval
            //    configureSource.AwsOptions = hostContext.Configuration.GetAWSOptions(); // Use the default AWS options
            //});
            //IConfigurationRoot newConf = builder.Build();
            //services.AddSingleton(newConf);
            services.AddAWSService<IAmazonS3>();
            services.Configure<AmazonConfiguration>(opts => hostContext.Configuration.GetSection("AmazonConfiguration").Bind(opts));

            services.AddDatabaseContext(hostContext.Configuration);

            //var solrServerAddress = hostContext.Configuration["SolrServerAddress"];
            var solrServerAddress = parameters.Single(p=> p.Name.Contains("SolrServerAddress")).Value;

            var collectionName = parameters.Single(p => p.Name.Contains("SolrCollectionName")).Value;
            //var collectionName = hostContext.Configuration["SolrCollectionName"];

            //var username = configuration["SOLR_USER"];
            //var password = configuration["SOLR_PASSWORD"];

            //var basicAuthHeader = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
            //    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
            services.AddSolrNet<IBDataSchema>($"{solrServerAddress}/{collectionName}"/*, options => options.HttpClient.DefaultRequestHeaders.Authorization = basicAuthHeader*/);
            services.AddApplicationServices();
            services.AddSingleton(TimeProvider.System);
            services.AddTransient<IAppUserIdService, MockAppUserIdService>();
            services.AddTransient<IUser, MockCurrentUser>();
            services.AddTransient<IFileProvider, FileProvider>();
            services.AddTransient<IOCRService, OCRService>();

            //.AddTransient<ISearchEngineServices, SearchEngineServices>()



            services.AddMassTransit(x =>
            {

                x.AddConsumer<ExtractImageAttributesConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
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
