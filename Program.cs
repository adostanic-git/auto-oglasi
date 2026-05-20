using AutoOglasi.Models;
using AutoOglasi.Services;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// MongoDB podesavanja
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// MongoDB Identity konfiguracija
var mongoDbSettings = builder.Configuration
    .GetSection("MongoDbSettings").Get<MongoDbSettings>();

builder.Services.AddIdentity<ApplicationUser, MongoIdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddMongoDbStores<ApplicationUser, MongoIdentityRole<Guid>, Guid>(
    mongoDbSettings!.ConnectionString,
    mongoDbSettings.DatabaseName)
.AddDefaultTokenProviders();

// Registruj VehicleService
builder.Services.AddSingleton<VehicleService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();