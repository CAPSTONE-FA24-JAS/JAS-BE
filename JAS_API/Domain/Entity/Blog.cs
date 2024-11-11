using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Blog : BaseEntity
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int? AccountId { get; set; }
        public virtual Account? Account { get; set; }
        public virtual List<ImageBlog>? ImageBlogs { get; set; }
    }
}
