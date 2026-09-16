using GolBet.Repositories.Data;
using GolBet.Repositories.Implementations;
using GolBet.Repositories.Interfaces;
using GolBet.Services.Mapping;
using Microsoft.EntityFrameworkCore;
using GolBet.Services.Interfaces;    
using GolBet.Services.Implementations; 

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. REGISTRO DE SERVICIOS (Dependency Injection)
// ==========================================

// Base de datos (Entity Framework Core con SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorios (Genéricos y Específicos)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

// Servicios de Negocio
builder.Services.AddScoped<IMatchService, MatchService>();

// AutoMapper (Escanea el ensamblado que contiene MappingProfile)
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Controladores con Vistas (MVC)
builder.Services.AddControllersWithViews();


// ==========================================
// 2. CONSTRUCCIÓN DE LA APLICACIÓN
// ==========================================
var app = builder.Build();


// ==========================================
// 3. SIEMBRA DE DATOS INICIALES (Data Seeding)
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(context);
}


// ==========================================
// 4. PIPELINE DE PETICIONES HTTP (Middleware)
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
// 5. INICIO DE LA APLICACIÓN
// ==========================================
app.Run();