using Application.Commons;
using Application.ViewModels.AccountDTO;
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
            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<Account, UpdateProfileDTO>().ReverseMap();
        }
    }
}
