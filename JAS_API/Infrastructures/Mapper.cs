using Application.Commons;
using Application.ViewModels.AccountDTOs;
using Application.ViewModels.AddressToShipDTO;
using Application.ViewModels.AuctionDTOs;
using Application.ViewModels.BidLimitDTOs;
using Application.ViewModels.DistrictDTOs;
using Application.ViewModels.ProvinceDTOs;
using Application.ViewModels.RoleDTOs;
using Application.ViewModels.ValuationDTOs;
using Application.ViewModels.WalletDTOs;
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
            CreateMap<Account, RegisterAccountDTO>().ReverseMap()
                .ForMember(customer => customer.Customer, c => c.MapFrom(src => src.RegisterCustomerDTO));
            CreateMap<RegisterCustomerDTO, Customer>().ReverseMap();
            CreateMap<Account, CreateAccountDTO>()
                .ForMember(dest => dest.CreateStaffDTO, opt => opt.MapFrom(src => src.Staff))
                .ReverseMap();
            CreateMap<Staff, CreateStaffDTO>().ReverseMap();
            CreateMap<Staff, StaffDTO>()
                .ForMember(dest => dest.AccountDTO, opt => opt.MapFrom(src => src.Account))
                .ReverseMap();
            CreateMap<Account, AccountDTO>()
                .ForMember(dest => dest.CustomerDTO, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.StaffDTO, opt => opt.MapFrom(src => src.Staff))
                .ForPath(x => x.RoleName, y => y.MapFrom(x => x.Role.Name))
                .ReverseMap();
            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.AccountDTO, opt => opt.MapFrom(src => src.Account))
                .ReverseMap();          
            CreateMap<Account, UpdateProfileDTO>()
                .ForMember(dest => dest.CustomerProfileDTO, opt => opt.MapFrom(src => src.Customer))
                .ReverseMap();
            CreateMap<Customer, CustomerProfileDTO>().ReverseMap();
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
            CreateMap<Wallet, CreateWalletDTO>().ReverseMap();
            CreateMap<Wallet, WalletDTO>().ReverseMap()
                .ForMember(desc => desc.Customer, src => src.MapFrom(x => x.CustomerDTO));
            CreateMap<Auction, AuctionDTO>().ReverseMap();
            CreateMap<Auction, CreateAuctionDTO>().ReverseMap();
            CreateMap<Auction, UpdateAuctionDTO>().ReverseMap();
            CreateMap<HistoryValuation, HistoryValuationDTO>().ReverseMap();
        }
    }
}
