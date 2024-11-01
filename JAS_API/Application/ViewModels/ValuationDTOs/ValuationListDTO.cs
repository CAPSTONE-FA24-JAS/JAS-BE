using Application.ViewModels.AccountDTOs;
using Application.ViewModels.JewelryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ValuationDTOs
{
    public class ValuationListDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }        
        public int? SellerId { get; set; }
        public int? StaffId { get; set; }
        public DateTime CreationDate { get; set; }
        public string? Email { get; set; }
        public string? FirstNameSeller { get; set; }
        public string? LastNameSeller { get; set; }
        public string? NameJewelry { get; set; }
    }
}
