namespace WebAPI.Service
{
    public interface ILiveBiddingService
    {
        Task CheckLotStartAsync();
        Task ChecKLotEndAsync();
    }
}
