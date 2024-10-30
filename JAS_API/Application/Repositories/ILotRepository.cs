using Domain.Entity;
namespace Application.Repositories
{
    public interface ILotRepository : IGenericRepository<Lot>
    {
        List<Lot> GetLotsAsync(string lotType, string status);
        List<Lot> GetLotsByAuctionAsync(int auctionId);
    }
}
