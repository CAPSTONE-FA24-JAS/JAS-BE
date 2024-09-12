using Application.Commons;
using Application.ViewModels.AccountDTO;
using Application.ViewModels.BidLimitDTO;
using Application.ViewModels.RoleDTO;
using Application.ViewModels.ValuationDTOs;
using AutoMapper;
using Domain.Entity;


namespace Infrastructures
{
    public class Mapper : Profile
    {
        public Mapper() 
        {
            CreateMap(typeof(Pagination<>), typeof(Pagination<>));
            CreateMap<Account, RegisterAccountDTO>().ReverseMap();
            CreateMap<Account, AccountDTO>().ReverseMap()
                .ForPath(x => x.Role.Name, y => y.MapFrom(x => x.RoleName))
                .ReverseMap();
            CreateMap<Account, UpdateProfileDTO>().ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<BidLimit, BidLimitDTO>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreationDate))
                .ReverseMap();
            CreateMap<BidLimit, CreateBidLimitDTO>().ReverseMap();
            CreateMap<ConsignAnItemDTO, Valuation>()
                .ForMember(dest => dest.ImageValuations, opt => opt.Ignore());
            CreateMap<ImageValuation, ImageValuationDTO>().ReverseMap();
        }
    }
}
