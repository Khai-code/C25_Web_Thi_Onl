using Blazor_Server.Data;
using Blazor_Server.Services;
using Blazored.Toast;using Blazored.Toast;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddBlazoredToast();
builder.Services.AddHttpClient();
builder.Services.AddScoped(C =>
 new HttpClient { BaseAddress = new Uri("https://localhost:7187") });

builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthSerrvice>();
builder.Services.AddScoped<ExamService>();
builder.Services.AddScoped<Notification>();
builder.Services.AddScoped<Inforservice>();
builder.Services.AddScoped<ExammanagementService>();
builder.Services.AddScoped<Package_Test_ERP>();
builder.Services.AddScoped<LoginPackge>();
builder.Services.AddScoped<ClassServices>();
builder.Services.AddScoped<CreateExam>();
builder.Services.AddScoped<ExamService>();
builder.Services.AddScoped<Inforservice>();
builder.Services.AddScoped<Learning_SummaryService>();
builder.Services.AddScoped<LoginPackge>();
builder.Services.AddScoped<ReviewExam>();
builder.Services.AddScoped<ScoreServices>();
builder.Services.AddScoped<TeacherManagerService>();
builder.Services.AddScoped<Test>();
builder.Services.AddScoped<PackageManager>();
builder.Services.AddScoped<HisService>();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
