using Application.ViewModels.AccountDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceReponse
{
    public class LoginResponseDTO
    {
        public AccountDTO Account { get; set; }
        public string AccessToken { get; set; }
    }
}
