
using GolBet.Services.DTOs;

namespace GolBet.Services.Interfaces;

public interface ITeamService
{
    // Consulta de equipos
    Task<IEnumerable<TeamDto>> GetAllAsync();

    // Consulta de un equipo para editar
    Task<TeamFormDto?> GetForEditAsync(int id);

    // Creación de equipos
    Task CreateAsync(TeamFormDto dto);

    // Actualización de equipos
    Task UpdateAsync(TeamFormDto dto);

    // Desactivación de equipos
    Task DeactivateAsync(int id);
}
