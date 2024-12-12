using Domain.Entity;
namespace Application.Repositories
{
    public interface ILotRepository : IGenericRepository<Lot>
    {
        List<Lot> GetLotsAsync(string lotType, string status);

        Task<List<Lot>> GetLotsForAutoServiceAsync(string lotType, string status);
        Task<Lot?> GetLotsByAuctionAsync(int auctionId);
    }
}
