
using AutoMapper;
using GolBet.Entities;
using GolBet.Services.DTOs;

namespace GolBet.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ==========================================
        // 1. MAPEO DE PARTIDOS
        // ==========================================

        // Cartelera general de partidos
        CreateMap<Match, MatchDto>();

        // Detalle del partido
        CreateMap<Match, MatchDetailDto>()
            .ForMember(
                dto => dto.TotalBets,
                options => options.MapFrom(match => match.Bets.Count)
            );

        // ==========================================
        // 2. MAPEO DE EQUIPOS
        // ==========================================

        // Entidad Team hacia TeamDto
        CreateMap<Team, TeamDto>();

        // TeamFormDto hacia Team y viceversa
        CreateMap<TeamFormDto, Team>().ReverseMap();

        // ==========================================
        // 3. MAPEO DE FORMULARIOS DE PARTIDOS
        // ==========================================

        // MatchFormDto hacia Match y viceversa
        CreateMap<MatchFormDto, Match>().ReverseMap();
    }
}
