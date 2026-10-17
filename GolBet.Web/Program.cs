
using System.Globalization;
using GolBet.Entities;
using GolBet.Repositories.Data;
using GolBet.Repositories.Implementations;
using GolBet.Repositories.Interfaces;
using GolBet.Services.Implementations;
using GolBet.Services.Interfaces;
using GolBet.Services.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Configuración regional para Colombia.
var culture = new CultureInfo("es-CO");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la conexión a SQL Server.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Configuración de ASP.NET Core Identity.
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = true;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Configuración de las cookies de autenticación.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Registro de repositorios genéricos.
builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>)
);

// Registro de repositorios específicos.
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

// Registro de servicios de negocio.
builder.Services.AddScoped<IMatchService, MatchService>();

// CORRECCIÓN: registrar el servicio de equipos requerido por MatchesController.
builder.Services.AddScoped<ITeamService, TeamService>();

// Configuración de AutoMapper.
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configuración de MVC.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Inicialización de la base de datos y datos iniciales.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    await DbSeeder.SeedAsync(
        context,
        roleManager,
        userManager
    );
}

// Configuración del entorno.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// La autenticación debe ejecutarse antes de la autorización.
app.UseAuthentication();
app.UseAuthorization();

// Archivos estáticos.
app.MapStaticAssets();

// Ruta MVC predeterminada.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
).WithStaticAssets();

app.Run();
