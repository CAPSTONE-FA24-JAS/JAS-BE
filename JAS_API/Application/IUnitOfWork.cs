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

        public IValuationDocumentRepository ValuationDocumentRepository { get; }
        public Task<int> SaveChangeAsync();
    }
}
