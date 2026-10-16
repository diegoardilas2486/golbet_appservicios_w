// GolBet.Services/Implementations/MatchService.cs 

using AutoMapper;
using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Interfaces;
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;

namespace GolBet.Services.Implementations;

public class MatchService : IMatchService
{
    // ==========================================
    // 1. DEPENDENCIAS
    // ==========================================

    private readonly IMatchRepository _matchRepository;
    private readonly IMapper _mapper;

    public MatchService(
        IMatchRepository matchRepository,
        IMapper mapper)
    {
        _matchRepository = matchRepository;
        _mapper = mapper;
    }

    // ==========================================
    // 2. CONSULTA DE PARTIDOS
    // ==========================================

    /// <summary>
    /// Obtiene la cartelera de partidos y permite filtrar por estado.
    /// </summary>
    public async Task<IEnumerable<MatchDto>> GetBoardAsync(
        MatchStatus? status = null)
    {
        var matches = await _matchRepository.GetAllWithTeamsAsync(status);

        return _mapper.Map<IEnumerable<MatchDto>>(matches);
    }

    /// <summary>
    /// Obtiene el detalle de un partido por su identificador.
    /// </summary>
    public async Task<MatchDetailDto?> GetDetailAsync(int id)
    {
        var match = await _matchRepository.GetByIdWithDetailsAsync(id);

        return match is null
            ? null
            : _mapper.Map<MatchDetailDto>(match);
    }

    /// <summary>
    /// Obtiene los datos de un partido para editarlo.
    /// Convierte la fecha UTC a la hora local de Colombia.
    /// </summary>
    public async Task<MatchFormDto?> GetForEditAsync(int id)
    {
        var match = await _matchRepository.GetByIdAsync(id);

        if (match is null)
            return null;

        var dto = _mapper.Map<MatchFormDto>(match);

        dto.Date = dto.Date.ToColombiaTime();

        return dto;
    }

    // ==========================================
    // 3. CREACIÓN DE PARTIDOS
    // ==========================================

    /// <summary>
    /// Valida y crea un nuevo partido.
    /// Convierte la fecha de Colombia a UTC antes de guardarla.
    /// </summary>
    public async Task CreateAsync(MatchFormDto dto)
    {
        ValidateBusinessRules(dto);

        var match = _mapper.Map<Match>(dto);

        match.Date = dto.Date.ToUtcFromColombia();

        await _matchRepository.AddAsync(match);
    }

    // ==========================================
    // 4. ACTUALIZACIÓN DE PARTIDOS
    // ==========================================

    /// <summary>
    /// Actualiza los datos de un partido existente.
    /// </summary>
    public async Task UpdateAsync(MatchFormDto dto)
    {
        ValidateBusinessRules(dto);

        var match = await _matchRepository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException(
                $"Match {dto.Id} not found.");

        _mapper.Map(dto, match);

        match.Date = dto.Date.ToUtcFromColombia();

        await _matchRepository.UpdateAsync(match);
    }

    // ==========================================
    // 5. DESACTIVACIÓN DE PARTIDOS
    // ==========================================

    /// <summary>
    /// Desactiva un partido existente.
    /// </summary>
    public async Task DeactivateAsync(int id)
    {
        await _matchRepository.DeactivateAsync(id);
    }

    // ==========================================
    // 6. VALIDACIÓN DE REGLAS DE NEGOCIO
    // ==========================================

    /// <summary>
    /// Verifica que los equipos sean diferentes
    /// y que la fecha del partido sea futura.
    /// </summary>
    private static void ValidateBusinessRules(MatchFormDto dto)
    {
        if (dto.HomeTeamId == dto.AwayTeamId)
        {
            throw new InvalidOperationException(
                "El equipo local y el visitante no pueden ser el mismo.");
        }

        if (dto.Date.ToUtcFromColombia() <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "La fecha del partido debe ser futura.");
        }
    }
}
