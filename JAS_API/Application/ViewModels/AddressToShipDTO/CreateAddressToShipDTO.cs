﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AddressToShipDTO
{
    public class CreateAddressToShipDTO
    {
        public string? AddressLine { get; set; }
        public int? AccountId { get; set; }
        public int? WardId { get; set; }
    }
}
