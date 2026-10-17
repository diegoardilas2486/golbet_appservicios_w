
using GolBet.Entities;
using GolBet.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Data;

public static class DbSeeder
{
    // Roles oficiales de GolBet.
    public const string AdminRole = "Admin";
    public const string BettorRole = "Apostador";

    public static async Task SeedAsync(
        AppDbContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<AppUser> userManager)
    {
        // Aplicar las migraciones pendientes.
        await context.Database.MigrateAsync();

        // Cargar equipos y partidos de prueba.
        await SeedDomainAsync(context);

        // Crear roles y administrador inicial.
        await SeedIdentityAsync(roleManager, userManager);
    }

    // =========================================================
    // DATOS DEL DOMINIO: EQUIPOS Y PARTIDOS
    // =========================================================
    private static async Task SeedDomainAsync(AppDbContext context)
    {
        // Evitar duplicar los equipos y partidos del seeder original.
        if (await context.Teams.AnyAsync())
        {
            return;
        }

        var teams = new List<Team>
        {
            new()
            {
                Name = "Atlético Nacional",
                City = "Medellín",
                CrestUrl = "https://placehold.co/80x80/006633/ffffff?text=NAC"
            },
            new()
            {
                Name = "Independiente Medellín",
                City = "Medellín",
                CrestUrl = "https://placehold.co/80x80/cc0000/ffffff?text=DIM"
            },
            new()
            {
                Name = "Millonarios",
                City = "Bogotá",
                CrestUrl = "https://placehold.co/80x80/003399/ffffff?text=MIL"
            },
            new()
            {
                Name = "Independiente Santa Fe",
                City = "Bogotá",
                CrestUrl = "https://placehold.co/80x80/cc0000/ffffff?text=SFE"
            },
            new()
            {
                Name = "América de Cali",
                City = "Cali",
                CrestUrl = "https://placehold.co/80x80/e60000/ffffff?text=AME"
            },
            new()
            {
                Name = "Deportivo Cali",
                City = "Cali",
                CrestUrl = "https://placehold.co/80x80/00794d/ffffff?text=CAL"
            },
            new()
            {
                Name = "Junior de Barranquilla",
                City = "Barranquilla",
                CrestUrl = "https://placehold.co/80x80/d40026/ffffff?text=JUN"
            },
            new()
            {
                Name = "Once Caldas",
                City = "Manizales",
                CrestUrl = "https://placehold.co/80x80/ffffff/000000?text=ONC"
            }
        };

        context.Teams.AddRange(teams);
        await context.SaveChangesAsync();

        // Fecha base para los partidos de prueba.
        var today = DateTime.UtcNow.Date;

        var matches = new List<Match>
        {
            // Programado: clásico paisa.
            new()
            {
                HomeTeamId = teams[0].Id,
                AwayTeamId = teams[1].Id,
                Date = today.AddDays(3).AddHours(20),
                Status = MatchStatus.Scheduled,
                HomeOdds = 2.10m,
                DrawOdds = 3.20m,
                AwayOdds = 3.60m
            },

            // Programado: clásico capitalino.
            new()
            {
                HomeTeamId = teams[2].Id,
                AwayTeamId = teams[3].Id,
                Date = today.AddDays(5).AddHours(18),
                Status = MatchStatus.Scheduled,
                HomeOdds = 2.45m,
                DrawOdds = 3.00m,
                AwayOdds = 2.95m
            },

            // Programado: clásico vallecaucano.
            new()
            {
                HomeTeamId = teams[4].Id,
                AwayTeamId = teams[5].Id,
                Date = today.AddDays(7).AddHours(19),
                Status = MatchStatus.Scheduled,
                HomeOdds = 2.30m,
                DrawOdds = 3.10m,
                AwayOdds = 3.15m
            },

            // Programado.
            new()
            {
                HomeTeamId = teams[6].Id,
                AwayTeamId = teams[7].Id,
                Date = today.AddDays(10).AddHours(16),
                Status = MatchStatus.Scheduled,
                HomeOdds = 1.85m,
                DrawOdds = 3.40m,
                AwayOdds = 4.20m
            },

            // En curso: apuestas cerradas.
            new()
            {
                HomeTeamId = teams[3].Id,
                AwayTeamId = teams[6].Id,
                Date = DateTime.UtcNow.AddHours(-1),
                Status = MatchStatus.InProgress,
                HomeOdds = 2.60m,
                DrawOdds = 3.05m,
                AwayOdds = 2.80m
            },

            // Finalizado: marcador definitivo.
            new()
            {
                HomeTeamId = teams[1].Id,
                AwayTeamId = teams[2].Id,
                Date = today.AddDays(-4).AddHours(20),
                Status = MatchStatus.Finished,
                HomeGoals = 2,
                AwayGoals = 1,
                HomeOdds = 2.75m,
                DrawOdds = 3.10m,
                AwayOdds = 2.70m
            }
        };

        context.Matches.AddRange(matches);
        await context.SaveChangesAsync();
    }

    // =========================================================
    // IDENTITY: ROLES Y USUARIO ADMINISTRADOR
    // =========================================================
    private static async Task SeedIdentityAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<AppUser> userManager)
    {
        // Crear los roles si todavía no existen.
        foreach (var roleName in new[] { AdminRole, BettorRole })
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var roleResult = await roleManager.CreateAsync(
                new IdentityRole(roleName));

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    roleResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"No fue posible crear el rol '{roleName}': {errors}");
            }
        }

        // Crear el administrador inicial si no existe.
        const string adminEmail = "admin@golbet.com";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin is null)
        {
            admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Administrador GolBet",
                Balance = 0m,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(
                admin,
                "Admin123*");

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    createResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"No fue posible crear el administrador: {errors}");
            }
        }

        // Garantizar que el usuario administrador tenga el rol Admin.
        if (!await userManager.IsInRoleAsync(admin, AdminRole))
        {
            var roleResult = await userManager.AddToRoleAsync(
                admin,
                AdminRole);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    roleResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"No fue posible asignar el rol Admin: {errors}");
            }
        }
    }
}
