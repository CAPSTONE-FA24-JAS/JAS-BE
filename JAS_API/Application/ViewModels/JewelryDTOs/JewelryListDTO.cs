using Application.ViewModels.ArtistDTOs;
using Application.ViewModels.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.JewelryDTOs
{
    public class JewelryListDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float? EstimatePriceMin { get; set; }
        public float? EstimatePriceMax { get; set; }
        public float? StartingPrice { get; set; }
        public float? SpecificPrice { get; set; }    
        public string? ForGender { get; set; }
        public string? Title { get; set; }
        public string? BidForm { get; set; }
        public DateTime? Time_Bidding { get; set; }        
      
        public IEnumerable<ImageJewelryDTO>? ImageJewelries { get; set; }
        
    }
}
