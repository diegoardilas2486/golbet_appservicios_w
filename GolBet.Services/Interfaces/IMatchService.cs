//GolBet.Services/Implementations/MatchService.cs

using GolBet.Entities.Enums;
using GolBet.Services.DTOs;

namespace GolBet.Services.Interfaces;

public interface IMatchService
{
    // ==========================================
    // 1. CONSULTA DE PARTIDOS
    // ==========================================

    /// <summary>
    /// Obtiene la cartelera de partidos activos, ordenados por fecha.
    /// Permite filtrar opcionalmente por estado.
    /// </summary>
    Task<IEnumerable<MatchDto>> GetBoardAsync(
        MatchStatus? status = null,
        string? search = null
    );

    /// <summary>
    /// Obtiene el detalle de un partido por su identificador.
    /// </summary>
    Task<MatchDetailDto?> GetDetailAsync(int id);


    // ==========================================
    // 2. GESTIÓN DE PARTIDOS
    // ==========================================

    /// <summary>
    /// Obtiene los datos de un partido para editarlo.
    /// Retorna null si no existe.
    /// </summary>
    Task<MatchFormDto?> GetForEditAsync(int id);

    /// <summary>
    /// Crea un nuevo partido.
    /// </summary>
    Task CreateAsync(MatchFormDto dto);

    /// <summary>
    /// Actualiza los datos de un partido existente.
    /// </summary>
    Task UpdateAsync(MatchFormDto dto);

    /// <summary>
    /// Desactiva un partido.
    /// </summary>
    Task DeactivateAsync(int id);
}
