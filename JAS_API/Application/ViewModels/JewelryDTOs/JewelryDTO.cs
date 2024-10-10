using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.JewelryDTOs
{
    public class JewelryDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float? EstimatePriceMin { get; set; }
        public float? EstimatePriceMax { get; set; }
        public float? StartingPrice { get; set; }
        public string? VideoLink { get; set; }
        public string? ForGender { get; set; }
        public string? Title { get; set; }
        public string? BidForm { get; set; }
        public DateTime? Time_Bidding { get; set; }
        public int? ArtistId { get; set; }
        public int? CategoryId { get; set; }
        public IEnumerable<ImageJewelryDTO>? ImageJewelries { get; set; }
        public IEnumerable<KeyCharacteristicDetailDTO>? KeyCharacteristicDetails { get; set; }
        public IEnumerable<DiamondDTO>? MainDiamonds { get; set; }
        public IEnumerable<DiamondDTO>? SecondaryDiamonds { get; set; }
        public IEnumerable<ShapieDTO>? MainShaphies { get; set; }
        public IEnumerable<ShapieDTO>? SecondaryShaphies { get; set; }
    }
}
