using Application.ViewModels.JewelryDTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IJewelryRepository : IGenericRepository<Jewelry>
    {
        Task<(IEnumerable<Jewelry> data, int totalItem)> GetAllJewelryAynsc(int? pageIndex = null, int? pageSize = null);

        Task<(IEnumerable<Jewelry> data, int totalItem)> GetAllJewelryNoLotAynsc(int? pageIndex = null, int? pageSize = null);
        Task<(IEnumerable<Jewelry> data, int totalItem)> GetAllJewelryByCategoryAynsc(int categoryId, int? pageIndex = null, int? pageSize = null);
    }
}
