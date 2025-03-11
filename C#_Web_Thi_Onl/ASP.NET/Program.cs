using Data_Base.App_DbContext;
using Data_Base.GenericRepositories;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var m = "m";
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Db_Context>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddCors(option =>
{
    option.AddPolicy(name: m, policy => policy.AllowAnyOrigin()
                                              .AllowAnyMethod()
                                              .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var provider = app.Services.GetRequiredService<IActionDescriptorCollectionProvider>();
Console.WriteLine("\n📌 DANH SÁCH API ĐƯỢC NHẬN DIỆN:");
var data = provider.ActionDescriptors.Items.Count;
foreach (var descriptor in provider.ActionDescriptors.Items)
{
    if (descriptor is ControllerActionDescriptor controllerAction)
    {
        Console.WriteLine($"✅ {controllerAction.ControllerName} -> {controllerAction.ActionName} -> {controllerAction.AttributeRouteInfo?.Template}");
    }
}
Console.WriteLine("\n===========================");
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseRouting();
app.UseCors(m);
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapControllers();


app.Run();
