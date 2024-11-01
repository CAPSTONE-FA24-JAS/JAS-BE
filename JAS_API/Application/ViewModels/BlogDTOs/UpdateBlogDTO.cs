using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.BlogDTOs
{
    public class UpdateBlogDTO
    {
        public int BlogId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public IFormFile? fileImage { get; set; }
        public int? AccountId { get; set; }
    }
}
