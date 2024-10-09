
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AccountDTOs
{
    public class CreateAccountDTO
    {
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public virtual StaffDTO StaffDTO { get; set; }
    }

    public class StaffDTO
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public AccountDTO? AccountDTO { get; set; }
    }
}
