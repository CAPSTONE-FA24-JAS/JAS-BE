using Application.ViewModels.AccountDTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceReponse
{
    public class LoginResponseDTO
    {
        public Account User { get; set; }
        public string AccessToken { get; set; }
    }
}
