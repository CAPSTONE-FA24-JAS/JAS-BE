﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.WalletDTOs
{
    public class CreateWalletDTO
    {
        public int? CustomerId { get; set; }
        public string? Password { get; set; }
    }
}
