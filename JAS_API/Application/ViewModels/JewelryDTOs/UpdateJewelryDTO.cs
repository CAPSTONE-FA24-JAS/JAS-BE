using Application.ViewModels.KeyCharacteristicDTOs;
using Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.JewelryDTOs
{
    public class UpdateBaseEntity
    {
        public int Id { get; set; } 
    }
    public class UpdateJewelryDTO : UpdateBaseEntity
        {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float? EstimatePriceMin { get; set; }
        public float? EstimatePriceMax { get; set; }
        public float? StartingPrice { get; set; }
        public float? SpecificPrice { get; set; }
        public string? VideoLink { get; set; }
        public string? ForGender { get; set; }
        public string? Title { get; set; }
        public string? BidForm { get; set; }
        public DateTime? Time_Bidding { get; set; }
        public int? ArtistId { get; set; }
        public int? CategoryId { get; set; }

        [FromForm]
        public  List<UpdateImageJewelryDTO>? UpdateImageJewelryDTOs { get; set; }
        [FromForm]
        public  IEnumerable<UpdateKeyCharacteristicDetailDTO>? UpdateKeyCharacteristicDetailDTOs { get; set; }
        [FromForm]
        public  IEnumerable<UpdateMainDiamondDTO>? UpdateMainDiamondDTOs { get; set; }
        [FromForm]
        public  IEnumerable<UpdateSecondaryDiamondDTO>? UpdateSecondaryDiamondDTOs { get; set; }
        [FromForm]
        public  IEnumerable<UpdateMainShaphieDTO>? UpdateMainShaphieDTOs { get; set; }
        [FromForm]
        public  IEnumerable<UpdateSecondaryShaphieDTO>? UpdateSecondaryShaphieDTOs { get; set; }
    }

    public class UpdateImageJewelryDTO : UpdateBaseEntity
        {
        [FromForm]
        public IFormFile? ImageLink { get; set; }
        public string? Title { get; set; }
        [FromForm]
        public IFormFile? ThumbnailImage { get; set; }
    }

    public class UpdateKeyCharacteristicDetailDTO : UpdateBaseEntity
    {
        public string? Description { get; set; }
        public int? KeyCharacteristicId { get; set; }
    }

    public class UpdateMainDiamondDTO : UpdateBaseEntity
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? Cut { get; set; }
        public string? Clarity { get; set; }
        public int? Quantity { get; set; }
        public string? SettingType { get; set; }
        public string? Dimension { get; set; }
        public string? Shape { get; set; }
        public string? Certificate { get; set; }
        public string? Fluorescence { get; set; }
        public string? LengthWidthRatio { get; set; }
        public string? Type { get; set; }
        //
        [FromForm]
        public virtual IEnumerable<UpdateDocumentMainDiamondDTO>? UpdateDocumentMainDiamondDTOs { get; set; }
        [FromForm]
        public virtual IEnumerable<UpdateImageMainDiamondDTO>? UpdateImageMainDiamondDTOs { get; set; }
    }

    public class UpdateDocumentMainDiamondDTO : UpdateBaseEntity
    {
        [FromForm]
        public string? DocumentLink { get; set; }
        public string? DocumentTitle { get; set; }
    }

    public class UpdateImageMainDiamondDTO : UpdateBaseEntity
    {
        [FromForm]
        public IFormFile? ImageLink { get; set; }
    }

    public class UpdateSecondaryDiamondDTO : UpdateBaseEntity
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? Cut { get; set; }
        public string? Clarity { get; set; }
        public int? Quantity { get; set; }
        public string? SettingType { get; set; }
        public string? Dimension { get; set; }
        public string? Shape { get; set; }
        public string? Certificate { get; set; }
        public string? Fluorescence { get; set; }
        public string? LengthWidthRatio { get; set; }
        public string? Type { get; set; }
        public float? TotalCarat { get; set; }

        //
        [FromForm]
        public virtual IEnumerable<UpdateDocumentSecondaryDiamondDTO>? UpdateDocumentSecondaryDiamondDTOs { get; set; }
        [FromForm]
        public virtual IEnumerable<UpdateImageSecondaryDiamondDTO>? UpdateImageSecondaryDiamondDTOs { get; set; }
    }

    public class UpdateDocumentSecondaryDiamondDTO : UpdateBaseEntity
    {
        [FromForm]
        public string? DocumentLink { get; set; }
        public string? DocumentTitle { get; set; }
    }

    public class UpdateImageSecondaryDiamondDTO : UpdateBaseEntity
    {
        [FromForm]
        public IFormFile? ImageLink { get; set; }
    }

    public class UpdateMainShaphieDTO : UpdateBaseEntity
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public float? Carat { get; set; }
        public string? EnhancementType { get; set; }
        public int? Quantity { get; set; }
        public string? SettingType { get; set; }
        public string? Dimension { get; set; }

        //
        [FromForm]
        public virtual IEnumerable<UpdateeDocumentMainShaphieDTO>? UpdateeDocumentMainShaphieDTOs { get; set; }
        [FromForm]
        public virtual IEnumerable<UpdateImageMainShaphieDTO>? UpdateImageMainShaphieDTOs { get; set; }
    }

    public class UpdateeDocumentMainShaphieDTO : UpdateBaseEntity
    {
        [FromForm]
        public string? DocumentLink { get; set; }
        public string? DocumentTitle { get; set; }
    }

    public class UpdateImageMainShaphieDTO : UpdateBaseEntity
    {
        [FromForm]
        public IFormFile? ImageLink { get; set; }
    }

    public class UpdateSecondaryShaphieDTO : UpdateBaseEntity
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public float? Carat { get; set; }
        public string? EnhancementType { get; set; }
        public int? Quantity { get; set; }
        public string? SettingType { get; set; }
        public string? Dimension { get; set; }
        public float? TotalCarat { get; set; }

        //
        [FromForm]
        public virtual IEnumerable<UpdateDocumentSecondaryShaphieDTO>? UpdateDocumentSecondaryShaphieDTOs { get; set; }
        [FromForm]
        public virtual IEnumerable<UpdateImageSecondaryShaphieDTO>? UpdateImageSecondaryShaphieDTOs { get; set; }
    }

    public class UpdateDocumentSecondaryShaphieDTO : UpdateBaseEntity
    {
        [FromForm]
        public string? DocumentLink { get; set; }
        public string? DocumentTitle { get; set; }
    }

    public class UpdateImageSecondaryShaphieDTO : UpdateBaseEntity
    {
        [FromForm]
        public IFormFile? ImageLink { get; set; }
    }
}
