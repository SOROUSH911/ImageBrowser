using Amazon.S3;
using ImageBrowser.Application.Common.Middlewares;
using ImageBrowser.Infrastructure.Configurations;
using ImageBrowser.Infrastructure.Data;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddKeyVaultIfConfigured(builder.Configuration);
builder.Services.AddSsmParametersIfConfigured(builder.Configuration);
builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddApplicationServices();
builder.Services.AddScoped<ExceptionHandlingMiddleware>();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();
builder.Services.AddConfigurations(builder.Configuration);


var somevalue = builder.Configuration.GetValue<string>("TokenConfiguration");



var app = builder.Build();
//Console.WriteLine("Test SSM value" + somevalue);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
    app.UseDeveloperExceptionPage();

}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseExceptionHandler("/Error");
    app.UseHsts();
}
//app.UseDeveloperExceptionPage();
app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller}/{action=Index}/{id?}");

//app.MapRazorPages();

//app.MapFallbackToFile("index.html");

//app.UseDeveloperExceptionPage();
//app.UseExceptionHandler(options => { });




app.Map("/", () => Results.Redirect("/api"));
//app.UseAntiforgery();
app.MapEndpoints();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.Run();

public partial class Program { }
