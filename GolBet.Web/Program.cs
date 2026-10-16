
using System.Globalization;

using GolBet.Repositories.Data;
using GolBet.Repositories.Implementations;
using GolBet.Repositories.Interfaces;
using GolBet.Services.Mapping;
using GolBet.Services.Interfaces;
using GolBet.Services.Implementations;
using Microsoft.EntityFrameworkCore;

// ==========================================
// 1. CONFIGURACIÓN DE CULTURA (Colombia)
// ==========================================
var culture = new CultureInfo("es-CO");

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// ==========================================
// 2. CREACIÓN DEL BUILDER
// ==========================================
var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 3. REGISTRO DE SERVICIOS (Dependency Injection)
// ==========================================

// Base de datos (Entity Framework Core con SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Repositorios (Genéricos y Específicos)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

// Servicios de Negocio
builder.Services.AddScoped<IMatchService, MatchService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Controladores con Vistas (MVC)
builder.Services.AddControllersWithViews();

// ==========================================
// 4. CONSTRUCCIÓN DE LA APLICACIÓN
// ==========================================
var app = builder.Build();

// ==========================================
// 5. SIEMBRA DE DATOS INICIALES (Data Seeding)
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(context);
}

// ==========================================
// 6. PIPELINE DE PETICIONES HTTP (Middleware)
// ==========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// ==========================================
// 7. INICIO DE LA APLICACIÓN
// ==========================================
app.Run();
