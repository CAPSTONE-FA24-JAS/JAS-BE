using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AccountDTOs
{
    //public class UpdateProfileDTO
    //{
    //    public string? FirstName { get; set; }
    //    public string? LastName { get; set; }
    //    public IFormFile ProfileImage { get; set; } 
    //    public string? Email { get; set; }
    //    public string? Gender { get; set; }
    //    public DateTime? DateOfBirth { get; set; }
    //    public string? Address { get; set; }
    //    public string? PhoneNumber { get; set; }
    //}
    public class UpdateProfileDTO
    {
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public IFormFile ProfileImage { get; set; }
        public virtual CustomerProfileDTO? CustomerProfileDTO { get; set; }
    }
    public class CustomerProfileDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? CitizenIdentificationCard { get; set; }
        public DateTime? IDIssuanceDate { get; set; }
        public DateTime? IDExpirationDate { get; set; }
    }
}

