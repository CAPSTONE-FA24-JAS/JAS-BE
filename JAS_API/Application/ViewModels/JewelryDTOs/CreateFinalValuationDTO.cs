﻿using Application.ViewModels.KeyCharacteristicDTOs;
using Domain.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.JewelryDTOs
{
    public class CreateFinalValuationDTO
    {

        public string? Name { get; set; }
       
        public float? EstimatePriceMin { get; set; }
        public float? EstimatePriceMax { get; set; }

        public float? SpecificPrice { get; set; } 
        public string? VideoLink { get; set; }
        public string? ForGender { get; set; }
        public string? Title { get; set; }
        public int? ArtistId { get; set; }
        public int? CategoryId { get; set; }

        public int ValuationId { get; set; }

        public List<IFormFile>? ImageJewelries { get; set; }
        public List<CreateKeyCharacteristicDetailDTO>? KeyCharacteristicDetails { get; set; }
        public List<CreateDiamondDTO>? MainDiamonds { get; set; }
        public List<CreateDiamondDTO>? SecondaryDiamonds { get; set; }
        public List<CreateShaphieDTO>? MainShaphies { get; set; }
        public List<CreateShaphieDTO>? SecondaryShaphies { get; set; }


    }
}
