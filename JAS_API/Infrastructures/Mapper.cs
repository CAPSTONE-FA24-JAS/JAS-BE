﻿using Application.Commons;
using Application.ViewModels.AccountDTOs;
using Application.ViewModels.AddressToShipDTO;
using Application.ViewModels.AuctionDTOs;
using Application.ViewModels.ArtistDTOs;
using Application.ViewModels.BidLimitDTOs;
using Application.ViewModels.CategoryDTOs;
using Application.ViewModels.DistrictDTOs;
using Application.ViewModels.JewelryDTOs;
using Application.ViewModels.KeyCharacteristicDTOs;
using Application.ViewModels.ProvinceDTOs;
using Application.ViewModels.RoleDTOs;
using Application.ViewModels.ValuationDTOs;
using Application.ViewModels.WalletDTOs;
using Application.ViewModels.WardDTOs;
using AutoMapper;
using Domain.Entity;
using Application.ViewModels.LotDTOs;
using Application.ViewModels.InvoiceDTOs;
using Application.ViewModels.CustomerLotDTOs;
using Application.ViewModels.BidPriceDTOs;


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
                .ForPath(dest => dest.WalletId, opt => opt.MapFrom(src => src.Wallet.Id))
                .ForMember(dest => dest.WalletDTO, opt => opt.MapFrom(src => src.Wallet))
                .ReverseMap();          
            CreateMap<Account, UpdateProfileDTO>()
                .ForMember(dest => dest.CustomerProfileDTO, opt => opt.MapFrom(src => src.Customer))
                .ReverseMap();
            CreateMap<Customer, CustomerProfileDTO>().ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<BidLimit, BidLimitDTO>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    return context.Items.ContainsKey("StaffName") && context.Items["StaffName"] != null
                        ? (string)context.Items["StaffName"] : dest.StaffName; 
                }))
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
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.Staff))
                .ForMember(dest => dest.Appraiser, opt => opt.MapFrom(src => src.Appraiser))
                .ForMember(dest => dest.Jewelry, opt => opt.MapFrom(src => src.Jewelry))
                .ReverseMap();
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
            CreateMap<Auction, AuctionDTO>()
                .ForPath(dest => dest.TotalLot, src => src.MapFrom(x => x.Lots.Count()))
                .ReverseMap();
            CreateMap<Auction, CreateAuctionDTO>().ReverseMap();
            CreateMap<Auction, UpdateAuctionDTO>().ReverseMap();
            CreateMap<HistoryValuation, HistoryValuationDTO>().ReverseMap();
            CreateMap<Jewelry, JewelryDTO>()                
                .ForMember(dest => dest.ImageJewelries, opt => opt.MapFrom(src => src.ImageJewelries))
                .ForMember(dest => dest.KeyCharacteristicDetails, opt => opt.MapFrom(src => src.KeyCharacteristicDetails))
                .ForMember(dest => dest.MainDiamonds, opt => opt.MapFrom(src => src.MainDiamonds))
                .ForMember(dest => dest.SecondaryDiamonds, opt => opt.MapFrom(src => src.SecondaryDiamonds))
                .ForMember(dest => dest.MainShaphies, opt => opt.MapFrom(src => src.MainShaphies))
                .ForMember(dest => dest.SecondaryShaphies, opt => opt.MapFrom(src => src.SecondaryShaphies))
                .ForMember(dest => dest.Valuation, opt => opt.MapFrom(src => src.Valuation))
                .ForMember(dest => dest.Artist, opt => opt.MapFrom(src => src.Artist))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ReverseMap();
            CreateMap<Jewelry, JewelryListDTO>()
                .ForMember(dest => dest.ImageJewelries, opt => opt.MapFrom(src => src.ImageJewelries))
                .ReverseMap();
            CreateMap<CreateFinalValuationDTO, Jewelry>()
                .ForMember(dest => dest.ImageJewelries, opt => opt.Ignore())
                .ForMember(dest => dest.KeyCharacteristicDetails, opt => opt.Ignore())
                .ReverseMap();

            //CreateMap<CreateFinalValuationDTO, Jewelry>()
            //    .ForMember(dest => dest.ImageJewelries, opt => opt.Ignore())
            //    .ForMember(dest => dest.KeyCharacteristicDetails, opt => opt.Ignore())
            //    .ForMember(dest => dest.MainDiamonds, opt => opt.Ignore())
            //    .ForMember(dest => dest.SecondaryDiamonds, opt => opt.Ignore())
            //    .ForMember(dest => dest.MainShaphies, opt => opt.Ignore())
            //    .ForMember(dest => dest.SecondaryShaphies, opt => opt.Ignore())
            //    .ReverseMap();
            CreateMap<CreateDiamondDTO, MainDiamond>()
                .ForMember(dest => dest.ImageMainDiamonds, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentMainDiamonds, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<CreateDiamondDTO, SecondaryDiamond>()
                .ForMember(dest => dest.ImageSecondaryDiamonds, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentSecondaryDiamonds, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<CreateShaphieDTO, MainShaphie>()
                .ForMember(dest => dest.ImageMainShaphies, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentMainShaphies, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<CreateShaphieDTO, SecondaryShaphie>()
                .ForMember(dest => dest.ImageSecondaryShaphies, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentSecondaryShaphies, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<MainDiamond, DiamondDTO>()
                .ForMember(dest => dest.ImageDiamonds, opt => opt.MapFrom(src => src.ImageMainDiamonds))
                .ForMember(dest => dest.DocumentDiamonds, opt => opt.MapFrom(src => src.DocumentMainDiamonds))
                .ReverseMap();
            CreateMap<SecondaryDiamond, DiamondDTO>()
                .ForMember(dest => dest.ImageDiamonds, opt => opt.MapFrom(src => src.ImageSecondaryDiamonds))
                .ForMember(dest => dest.DocumentDiamonds, opt => opt.MapFrom(src => src.DocumentSecondaryDiamonds))
                .ReverseMap();
            CreateMap<MainShaphie, ShapieDTO>()
                .ForMember(dest => dest.ImageShaphies, opt => opt.MapFrom(src => src.ImageMainShaphies))
                .ForMember(dest => dest.DocumentShaphies, opt => opt.MapFrom(src => src.DocumentMainShaphies))
                .ReverseMap();
            CreateMap<SecondaryShaphie, ShapieDTO>()
                .ForMember(dest => dest.ImageShaphies, opt => opt.MapFrom(src => src.ImageSecondaryShaphies))
                .ForMember(dest => dest.DocumentShaphies, opt => opt.MapFrom(src => src.DocumentSecondaryShaphies))
                .ReverseMap();
            CreateMap<ImageJewelry, ImageJewelryDTO>().ReverseMap();
            CreateMap<ImageMainDiamond, ImageDiamondDTO>().ReverseMap();
            CreateMap<ImageSecondaryDiamond, ImageDiamondDTO>().ReverseMap();
            CreateMap<ImageMainShaphie, ImageShaphieDTO>().ReverseMap();
            CreateMap<ImageSecondaryShaphie, ImageShaphieDTO>().ReverseMap();
            CreateMap<KeyCharacteristicDetail, KeyCharacteristicDetailDTO>()
                .ForMember(dest => dest.KeyCharacteristic, opt => opt.MapFrom(src => src.KeyCharacteristic)).ReverseMap();
            CreateMap<KeyCharacteristicDetail, CreateKeyCharacteristicDetailDTO>().ReverseMap();
            CreateMap<DocumentMainDiamond, DocumentDiamondDTO>().ReverseMap();
            CreateMap<DocumentSecondaryDiamond, DocumentDiamondDTO>().ReverseMap();
            CreateMap<DocumentMainShaphie, DocumentShaphieDTO>().ReverseMap();
            CreateMap<DocumentSecondaryShaphie, DocumentShaphieDTO>().ReverseMap();
            CreateMap<KeyCharacteristic, KeyCharacteristicDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Artist, ArtistDTO>().ReverseMap();
            CreateMap<Lot, CreateLotFixedPriceDTO>().ReverseMap();
            CreateMap<Lot, CreateLotSecretAuctionDTO>().ReverseMap();
            CreateMap<Lot, CreateLotPublicAuctionDTO>().ReverseMap();
            CreateMap<Lot, CreateLotAuctionPriceGraduallyReducedDTO>().ReverseMap();
            CreateMap<Lot, LotFixedPriceDTO>().ReverseMap();
            CreateMap<Lot, LotSecretAuctionDTO>().ReverseMap();
            CreateMap<Lot, LotPublicAuctionDTO>().ReverseMap();
            CreateMap<Lot, LotAuctionPriceGraduallyReducedDTO>().ReverseMap();
            CreateMap<Lot, LotDTO>()
                .ForPath(x => x.ImageLinkJewelry, x => x.MapFrom(x => x.Jewelry.ImageJewelries.FirstOrDefault().ImageLink))
                .ReverseMap();
            CreateMap<Customer, SellerDTO>().ReverseMap();
            CreateMap<CustomerLot, RegisterToLotDTO>().ReverseMap();
            CreateMap<CustomerLot, CustomerLotDTO>().ReverseMap();
            CreateMap<Invoice, InvoiceDTO>()             
                .ForMember(dest => dest.MyBidDTO, opt => opt.MapFrom(src => src.CustomerLot))
                .ReverseMap();
            CreateMap<CustomerLot, MyBidDTO>()
                .ForMember(dest => dest.LotDTO, opt => opt.MapFrom(src => src.Lot))
                .ReverseMap();
            CreateMap<BidPrice, BidPriceDTO>()
                .ForPath(x => x.LastName, x => x.MapFrom( x => x.Customer.LastName))
                .ForPath(x => x.FirstName, x=> x.MapFrom( x => x.Customer.FirstName))
                .ReverseMap();

        }
    }
}
