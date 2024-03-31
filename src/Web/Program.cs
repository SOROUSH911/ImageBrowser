using ImageBrowser.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddKeyVaultIfConfigured(builder.Configuration);
builder.Services.AddSsmParametersIfConfigured(builder.Configuration);


builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

var app = builder.Build();
var somevalue = builder.Configuration.GetValue<string>("random_value");

Console.WriteLine("This is my value*************" + somevalue + "\n ***************");
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

app.MapEndpoints();

app.Run();

public partial class Program { }
