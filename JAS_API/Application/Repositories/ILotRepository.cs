using Domain.Entity;
namespace Application.Repositories
{
    public interface ILotRepository : IGenericRepository<Lot>
    {
        Task<(IEnumerable<Lot> data, int totalItems)> GetBidsOfCustomer(int? customerIId, string? status, int? pageIndex, int? pageSize);
    }
}
