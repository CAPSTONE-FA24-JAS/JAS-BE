﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class MainDiamond : BaseEntity
    {
        public string? Color { get; set; }
        public string? Cut { get; set; }
        public string? Clarity { get; set; }
        public int? Quantity { get; set; }
        public string? SettingType { get; set; }  
        public float? Dimension { get; set; }
        public string? Shape { get; set; }
        public string? Certificate { get; set; }
        public string? Fluorescence { get; set; }
        public float? LengthWidthRatio { get; set; }
        public int? JewelryId { get; set; }

        //
        public virtual IEnumerable<DocumentMainDiamond>? DocumentMainDiamonds {  get; set; }
        public virtual IEnumerable<ImageMainDiamond>? ImageMainDiamonds { get; set; }
        public virtual Jewelry? Jewelry { get; set; }
    }
}