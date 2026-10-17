// GolBet.Repositories/Implementations/MatchRepository.cs 
using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using GolBet.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Implementations;

public class MatchRepository : GenericRepository<Match>, IMatchRepository
{
    public MatchRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Match>> GetAllWithTeamsAsync(
        MatchStatus? status = null,
        string? search = null)
    {
        var query = _dbSet
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.IsActive)
            .AsNoTracking()
            .AsQueryable();

        // Filtro opcional por estado.
        if (status.HasValue)
        {
            query = query.Where(m => m.Status == status.Value);
        }

        // Filtro opcional por nombre del equipo local o visitante.
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(m =>
                m.HomeTeam.Name.Contains(term) ||
                m.AwayTeam.Name.Contains(term)
            );
        }

        // La consulta se ejecuta en SQL al llegar a ToListAsync.
        return await query
            .OrderBy(m => m.Date)
            .ToListAsync();
    }

    public async Task<Match?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Bets)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}