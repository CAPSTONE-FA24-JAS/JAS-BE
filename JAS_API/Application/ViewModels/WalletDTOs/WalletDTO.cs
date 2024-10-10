using Application.ViewModels.AccountDTOs;

namespace Application.ViewModels.WalletDTOs
{
    public class WalletDTO
    {
        public string? Balance { get; set; }
        public virtual CustomerDTO? CustomerDTO { get; set; }
    }
}
