using Application.ViewModels.AccountDTOs;

namespace Application.ViewModels.WalletDTOs
{
    public class WalletDTO
    {
        public int Id { get; set; }
        public Decimal? Balance { get; set; }
        public Decimal? AvailableBalance { get; set; }
        public Decimal? FrozenBalance { get; set; }
        public virtual CustomerDTO? CustomerDTO { get; set; }
    }
}
