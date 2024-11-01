using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.BlogDTOs
{
    public class ViewBlogDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int? AccountId { get; set; }
        public IEnumerable<ImageBlogDTO> imageBlogDTOs { get; set; }
    }
    public class ImageBlogDTO
    {
        public int Id { get; set; }
        public string? ImageLink { get; set; }
        public int? BlogId { get; set; }

    }

}
