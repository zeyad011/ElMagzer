using AutoMapper;
using ElMagzer.Core.Models;
using ElMagzer.Shared.Dtos;

namespace ElMagzer.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Cows, cowDetailsDto>()
            .ForMember(dest => dest.CowId,opt => opt.MapFrom(src => src.CowsId))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Batch.Order.OrderCode ?? "N/A"))
            .ForMember(dest => dest.BatchCode, opt => opt.MapFrom(src => src.Batch.BatchCode ?? "N/A"))
            .ForMember(dest => dest.TypeOfCow, opt => opt.MapFrom(src => src.TypeofCows.TypeName ?? "N/A"))
            .ForMember(dest => dest.ProductionDate, opt => opt.MapFrom(src => src.Batch.Order.CreatedDate.ToString("dd/MM/yyyy")))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.CowsSeed.weight))
            .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor_Id ?? "N/A"))
            .ForMember(dest => dest.Worker, opt => opt.MapFrom(src => src.techOfDevice1 ?? "N/A"));
        }
    }
}
