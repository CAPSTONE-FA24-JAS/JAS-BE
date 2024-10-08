using Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IUnitOfWork
    {
        public IAccountRepository AccountRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IBidLimitRepository BidLimitRepository { get; }
        public IImageValuationRepository ImageValuationRepository { get; }
        public IValuationRepository ValuationRepository { get; }
        public IAddressToShipRepository AddressToShipRepository { get; }
        public IWardRepository WardRepository { get; }
        public IDistrictRepository IDistrictRepository { get; }
        public IProvinceRepository ProvininceRepository { get; }
        public Task<int> SaveChangeAsync();
    }
}
