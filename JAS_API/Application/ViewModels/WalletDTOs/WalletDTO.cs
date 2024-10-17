using Application.ViewModels.AccountDTOs;

namespace Application.ViewModels.WalletDTOs
{
    public class WalletDTO
    {
        public int Id { get; set; }
        public string? Balance { get; set; }
        public virtual CustomerDTO? CustomerDTO { get; set; }
    }
}
