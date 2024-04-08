using System.Reflection;
using CleanArchitectureTemplate.Domain.Mapping;
using ImageBrowser.Application.Common.Behaviours;
using ImageBrowser.Application.Common.Middlewares;
using Microsoft.AspNetCore.Http;
using ImageBrowser.Application.Common.Mappings;
using ImageBrowser.Application.Common.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));

        services.AddSingleton(provider => new MapperConfiguration(config =>
        {
            var scope = provider.CreateScope();
            config.AddProfile(new ObjectMappingProfile(scope.ServiceProvider.GetService<IAppUserIdService>(), scope.ServiceProvider.GetService<IFileProvider>()));
        }));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });




        //services.AddMassTransit(x =>
        //{
        //    x.AddConsumer<SubmitOrderConsumer>();

        //    x.UsingRabbitMq((context, cfg) =>
        //    {
        //        cfg.ConfigureEndpoints(context);
        //    });
        //});
        //services.AddTransient<IMiddleware, ExceptionHandlingMiddleware>();
        return services;
    }
}
