﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class SecondaryDiamond : BaseEntity
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
        public virtual IEnumerable<DocumentSecondaryDiamond>? DocumentSecondaryDiamonds { get; set; }
        public virtual IEnumerable<ImageSecondaryDiamond>? ImageSecondaryDiamonds { get; set; }
        public virtual Jewelry? Jewelry { get; set; }
    }
}
