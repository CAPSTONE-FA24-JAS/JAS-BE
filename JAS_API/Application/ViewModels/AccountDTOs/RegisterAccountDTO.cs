
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AccountDTOs
{
    public class RegisterAccountDTO
    {
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public RegisterCustomerDTO RegisterCustomer { get; set; }
    }
    public class RegisterCustomerDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? CitizenIdentificationCard { get; set; }
        public DateTime? IDIssuanceDate { get; set; }
        public DateTime? IDExpirationDate { get; set; }
    }
}

