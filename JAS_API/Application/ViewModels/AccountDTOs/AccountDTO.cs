
using Application.ViewModels.BidLimitDTOs;

namespace Application.ViewModels.AccountDTOs
{
    public class AccountDTO
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? PasswordHash { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public CustomerDTO? CustomerDTO { get; set; }
        public StaffDTO? StaffDTO { get; set; }
    }
    public class CustomerDTO
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? CitizenIdentificationCard { get; set; }
        public DateTime? IDIssuanceDate { get; set; }
        public DateTime? IDExpirationDate { get; set; }
        public float? PriceLimit { get; set; }
        public AccountDTO? AccountDTO { get; set; }

    }

}
