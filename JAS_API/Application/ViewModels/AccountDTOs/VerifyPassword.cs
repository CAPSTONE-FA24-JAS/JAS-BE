namespace Application.ViewModels.AccountDTOs
{
    public class VerifyPassword
    {
        public int UserId { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
