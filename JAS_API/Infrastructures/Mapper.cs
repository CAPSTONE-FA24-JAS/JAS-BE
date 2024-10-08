using Application.Commons;
using Application.ViewModels.AccountDTOs;
using Application.ViewModels.AddressToShipDTO;
using Application.ViewModels.BidLimitDTOs;
using Application.ViewModels.DistrictDTOs;
using Application.ViewModels.ProvinceDTOs;
using Application.ViewModels.RoleDTOs;
using Application.ViewModels.ValuationDTOs;
using Application.ViewModels.WardDTOs;
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
            CreateMap<Account, CreateAccountDTO>().ReverseMap();
            CreateMap<Account, AccountDTO>().ReverseMap()
                .ForPath(x => x.Role.Name, y => y.MapFrom(x => x.RoleName))
                .ReverseMap();
            CreateMap<Account, UpdateProfileDTO>().ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<BidLimit, BidLimitDTO>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreationDate))
                .ReverseMap();
            CreateMap<BidLimit, CreateBidLimitDTO>().ReverseMap();
            CreateMap<BidLimit, UpdateBidLimitDTO>().ReverseMap();
            CreateMap<ConsignAnItemDTO, Valuation>()
                .ForMember(dest => dest.ImageValuations, opt => opt.Ignore());
            CreateMap<ImageValuation, ImageValuationDTO>().ReverseMap();
            CreateMap<Valuation, ValuationDTO>()
                .ForMember(dest => dest.Seller, opt => opt.MapFrom(src => src.Seller))
                .ForMember(dest => dest.ImageValuations, opt => opt.MapFrom(src => src.ImageValuations))
                .ForMember(dest => dest.ValuationDocuments, opt => opt.MapFrom(src => src.ValuationDocuments))
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.Staff));
            CreateMap<ValuationDocument, ValuationDocumentDTO>().ReverseMap();
            CreateMap<AddressToShip, CreateAddressToShipDTO>().ReverseMap();
            CreateMap<AddressToShip, ViewAddressToShipDTO>().ReverseMap();
            CreateMap<Ward, CreateWardDTO>().ReverseMap();
            CreateMap<Ward, ViewWardDTO>().ReverseMap();
            CreateMap<District, CreateDistrictDTO>().ReverseMap();
            CreateMap<District, ViewDistrictDTO>().ReverseMap();
            CreateMap<Province, CreateProvinceDTO>().ReverseMap();
            CreateMap<Province, ViewProvinceDTO>().ReverseMap();
                

        }
    }
}
