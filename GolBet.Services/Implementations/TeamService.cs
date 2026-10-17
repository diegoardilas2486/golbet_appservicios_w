
using AutoMapper;
using GolBet.Entities;
using GolBet.Repositories.Interfaces;
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;

namespace GolBet.Services.Implementations;

public class TeamService : ITeamService
{
    // ==========================================
    // 1. DEPENDENCIAS
    // ==========================================

    private readonly IGenericRepository<Team> _teamRepository;
    private readonly IMapper _mapper;

    public TeamService(
        IGenericRepository<Team> teamRepository,
        IMapper mapper)
    {
        _teamRepository = teamRepository;
        _mapper = mapper;
    }

    // ==========================================
    // 2. CONSULTA DE EQUIPOS
    // ==========================================

    public async Task<IEnumerable<TeamDto>> GetAllAsync()
    {
        var teams = await _teamRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<TeamDto>>(teams);
    }

    // ==========================================
    // 3. CONSULTA PARA EDICIÓN
    // ==========================================

    public async Task<TeamFormDto?> GetForEditAsync(int id)
    {
        var team = await _teamRepository.GetByIdAsync(id);

        return team is null
            ? null
            : _mapper.Map<TeamFormDto>(team);
    }

    // ==========================================
    // 4. CREACIÓN DE EQUIPOS
    // ==========================================

    public async Task CreateAsync(TeamFormDto dto)
    {
        var team = _mapper.Map<Team>(dto);

        await _teamRepository.AddAsync(team);
    }

    // ==========================================
    // 5. ACTUALIZACIÓN DE EQUIPOS
    // ==========================================

    public async Task UpdateAsync(TeamFormDto dto)
    {
        var team = await _teamRepository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException(
                $"Team {dto.Id} not found.");

        _mapper.Map(dto, team);

        await _teamRepository.UpdateAsync(team);
    }

    // ==========================================
    // 6. DESACTIVACIÓN DE EQUIPOS
    // ==========================================

    public async Task DeactivateAsync(int id)
    {
        await _teamRepository.DeactivateAsync(id);
    }
}
